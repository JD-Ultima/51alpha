// =====================================================
// 51ALPHA CORE - SIEGE REPOSITORY
// =====================================================
// File: SiegeRepository.cs
// Created: 2025-12-16
// Phase: Sprint 1 - Repository Pattern Expansion
// Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q5: Siege System tables)
// =====================================================

using System;
using System.Collections.Generic;
using Npgsql;

namespace Server.Sphere51a.Core.Database
{
    /// <summary>
    /// Repository for siege battle data access.
    /// Manages siege instances, participants, kills, and town control.
    /// </summary>
    public static class SiegeRepository
    {
        /// <summary>
        /// Create a new siege battle instance.
        /// </summary>
        public static long CreateSiege(string townName, int attackingFactionId, int? defendingFactionId, DateTime startTime)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO s51a_sieges (town_name, attacking_faction_id, defending_faction_id, start_time, status)
                    VALUES (@townName, @attackingFactionId, @defendingFactionId, @startTime, 'scheduled')
                    RETURNING siege_id
                ", conn);

                cmd.Parameters.AddWithValue("townName", townName);
                cmd.Parameters.AddWithValue("attackingFactionId", attackingFactionId);
                cmd.Parameters.AddWithValue("defendingFactionId", (object)defendingFactionId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("startTime", startTime);

                return (long)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to create siege: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Start a siege battle (change status to active).
        /// </summary>
        public static void StartSiege(long siegeId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    UPDATE s51a_sieges
                    SET status = 'active'
                    WHERE siege_id = @siegeId
                ", conn);

                cmd.Parameters.AddWithValue("siegeId", siegeId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to start siege {siegeId}: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Complete a siege battle and record the winner.
        /// </summary>
        public static void CompleteSiege(long siegeId, int winnerFactionId, int attackingScore, int defendingScore)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    // Update siege record
                    using (var cmd = new NpgsqlCommand(@"
                        UPDATE s51a_sieges
                        SET status = 'completed',
                            end_time = NOW(),
                            winner_faction_id = @winnerId,
                            attacking_score = @attackingScore,
                            defending_score = @defendingScore
                        WHERE siege_id = @siegeId
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("siegeId", siegeId);
                        cmd.Parameters.AddWithValue("winnerId", winnerFactionId);
                        cmd.Parameters.AddWithValue("attackingScore", attackingScore);
                        cmd.Parameters.AddWithValue("defendingScore", defendingScore);
                        cmd.ExecuteNonQuery();
                    }

                    // Update town control
                    using (var cmd = new NpgsqlCommand(@"
                        UPDATE s51a_town_control
                        SET controlling_faction_id = @winnerId,
                            controlled_since = NOW(),
                            last_siege_at = NOW()
                        WHERE town_name = (SELECT town_name FROM s51a_sieges WHERE siege_id = @siegeId)
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("siegeId", siegeId);
                        cmd.Parameters.AddWithValue("winnerId", winnerFactionId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to complete siege {siegeId}: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Add a participant to a siege battle.
        /// </summary>
        public static void AddParticipant(long siegeId, string accountId, int factionId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO s51a_siege_participants (siege_id, account_id, faction_id)
                    VALUES (@siegeId, @accountId, @factionId)
                    ON CONFLICT (siege_id, account_id) DO NOTHING
                ", conn);

                cmd.Parameters.AddWithValue("siegeId", siegeId);
                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("factionId", factionId);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to add siege participant: {ex.Message}");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// Record a PvP kill during a siege.
        /// </summary>
        public static void RecordKill(long siegeId, string killerAccountId, string victimAccountId, int killerFactionId, int victimFactionId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    // Insert kill log
                    using (var cmd = new NpgsqlCommand(@"
                        INSERT INTO s51a_siege_kills (siege_id, killer_account_id, victim_account_id, killer_faction_id, victim_faction_id)
                        VALUES (@siegeId, @killerId, @victimId, @killerFactionId, @victimFactionId)
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("siegeId", siegeId);
                        cmd.Parameters.AddWithValue("killerId", killerAccountId);
                        cmd.Parameters.AddWithValue("victimId", victimAccountId);
                        cmd.Parameters.AddWithValue("killerFactionId", killerFactionId);
                        cmd.Parameters.AddWithValue("victimFactionId", victimFactionId);
                        cmd.ExecuteNonQuery();
                    }

                    // Update killer stats
                    using (var cmd = new NpgsqlCommand(@"
                        UPDATE s51a_siege_participants
                        SET kills = kills + 1,
                            points_earned = points_earned + 10
                        WHERE siege_id = @siegeId AND account_id = @killerId
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("siegeId", siegeId);
                        cmd.Parameters.AddWithValue("killerId", killerAccountId);
                        cmd.ExecuteNonQuery();
                    }

                    // Update victim stats
                    using (var cmd = new NpgsqlCommand(@"
                        UPDATE s51a_siege_participants
                        SET deaths = deaths + 1
                        WHERE siege_id = @siegeId AND account_id = @victimId
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("siegeId", siegeId);
                        cmd.Parameters.AddWithValue("victimId", victimAccountId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to record siege kill: {ex.Message}");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// Get active siege for a town.
        /// </summary>
        public static long? GetActiveSiege(string townName)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT siege_id
                    FROM s51a_sieges
                    WHERE town_name = @townName AND status = 'active'
                    ORDER BY start_time DESC
                    LIMIT 1
                ", conn);

                cmd.Parameters.AddWithValue("townName", townName);

                var result = cmd.ExecuteScalar();
                return result != null ? (long)result : null;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get active siege: {ex.Message}");
                Utility.PopColor();
                return null;
            }
        }

        /// <summary>
        /// Get controlling faction for a town.
        /// </summary>
        public static int? GetTownControllingFaction(string townName)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT controlling_faction_id
                    FROM s51a_town_control
                    WHERE town_name = @townName
                ", conn);

                cmd.Parameters.AddWithValue("townName", townName);

                var result = cmd.ExecuteScalar();
                return result != DBNull.Value && result != null ? (int)result : null;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get town controlling faction: {ex.Message}");
                Utility.PopColor();
                return null;
            }
        }

        /// <summary>
        /// Update participant damage/healing stats.
        /// </summary>
        public static void UpdateParticipantStats(long siegeId, string accountId, long damageDealt, long healingDone)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    UPDATE s51a_siege_participants
                    SET damage_dealt = damage_dealt + @damage,
                        healing_done = healing_done + @healing
                    WHERE siege_id = @siegeId AND account_id = @accountId
                ", conn);

                cmd.Parameters.AddWithValue("siegeId", siegeId);
                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("damage", damageDealt);
                cmd.Parameters.AddWithValue("healing", healingDone);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[Sphere51a] Failed to update participant stats: {ex.Message}");
                Utility.PopColor();
            }
        }
    }
}
