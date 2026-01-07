// =====================================================
// 51ALPHA FACTION SYSTEM - POSTGRESQL REPOSITORY
// =====================================================
// File: FactionRepository.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q5: Database Schema)
// =====================================================

using System;
using System.Collections.Generic;
using Server.Sphere51a.Core.Database;
using Npgsql;

namespace Server.Sphere51a.Factions
{
    /// <summary>
    /// PostgreSQL repository for faction data.
    /// Provides CRUD operations for guild faction assignments.
    /// All methods use write-through strategy (immediate DB sync).
    /// </summary>
    public static class FactionRepository
    {
        // =====================================================
        // READ OPERATIONS
        // =====================================================

        /// <summary>
        /// Get guild's current faction from PostgreSQL.
        /// </summary>
        /// <param name="guildSerial">ModernUO Guild.Serial</param>
        /// <returns>GuildFactionInfo or null if guild not in any faction</returns>
        public static GuildFactionInfo GetGuildFaction(Serial guildSerial)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(
                    @"SELECT gf.faction_id, gf.joined_at, gf.last_change_at, gf.can_change_after,
                             f.faction_name, f.faction_hue, f.home_city
                      FROM s51a_guild_factions gf
                      JOIN s51a_factions f ON gf.faction_id = f.faction_id
                      WHERE gf.guild_serial = @serial",
                    conn);

                cmd.Parameters.AddWithValue("serial", (long)guildSerial.Value);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                    return null;

                return new GuildFactionInfo
                {
                    GuildSerial = guildSerial,
                    Faction = S51aFaction.GetById(reader.GetInt32(0)),
                    JoinedAt = reader.GetDateTime(1),
                    LastChangeAt = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                    CanChangeAfter = reader.IsDBNull(3) ? null : reader.GetDateTime(3)
                };
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get guild faction for {guildSerial}: {ex.Message}");
                Utility.PopColor();
                return null;
            }
        }

        /// <summary>
        /// Get all guilds in a specific faction.
        /// </summary>
        /// <param name="factionId">Faction ID (1, 2, 3)</param>
        /// <returns>List of guild serials</returns>
        public static List<Serial> GetFactionGuilds(int factionId)
        {
            var guilds = new List<Serial>();

            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(
                    "SELECT guild_serial FROM s51a_guild_factions WHERE faction_id = @factionId",
                    conn);

                cmd.Parameters.AddWithValue("factionId", factionId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    guilds.Add((Serial)(uint)reader.GetInt64(0));
                }
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get guilds for faction {factionId}: {ex.Message}");
                Utility.PopColor();
            }

            return guilds;
        }

        /// <summary>
        /// Get faction statistics (guild count per faction).
        /// </summary>
        /// <returns>Dictionary: FactionId -> Guild Count</returns>
        public static Dictionary<int, int> GetFactionStatistics()
        {
            var stats = new Dictionary<int, int>
            {
                { 1, 0 }, // GoldenShield
                { 2, 0 }, // Bridgefolk
                { 3, 0 }  // LycaeumOrder
            };

            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(
                    @"SELECT faction_id, COUNT(*)
                      FROM s51a_guild_factions
                      GROUP BY faction_id",
                    conn);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int factionId = reader.GetInt32(0);
                    int count = reader.GetInt32(1);
                    stats[factionId] = count;
                }
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get faction statistics: {ex.Message}");
                Utility.PopColor();
            }

            return stats;
        }

        // =====================================================
        // WRITE OPERATIONS
        // =====================================================

        /// <summary>
        /// Assign guild to faction (INSERT or UPDATE).
        /// Sets cooldown to NOW + 7 days.
        /// </summary>
        /// <param name="guildSerial">ModernUO Guild.Serial</param>
        /// <param name="factionId">Faction ID (1, 2, 3)</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SetGuildFaction(Serial guildSerial, int factionId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(
                    @"INSERT INTO s51a_guild_factions
                          (guild_serial, faction_id, joined_at, last_change_at, can_change_after)
                      VALUES
                          (@serial, @factionId, NOW(), NOW(), NOW() + INTERVAL '7 days')
                      ON CONFLICT (guild_serial) DO UPDATE SET
                          faction_id = @factionId,
                          last_change_at = NOW(),
                          can_change_after = NOW() + INTERVAL '7 days'",
                    conn);

                cmd.Parameters.AddWithValue("serial", (long)guildSerial.Value);
                cmd.Parameters.AddWithValue("factionId", factionId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to set guild faction for {guildSerial} -> {factionId}: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        /// <summary>
        /// Remove guild from faction (DELETE).
        /// </summary>
        /// <param name="guildSerial">ModernUO Guild.Serial</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool RemoveGuildFaction(Serial guildSerial)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(
                    "DELETE FROM s51a_guild_factions WHERE guild_serial = @serial",
                    conn);

                cmd.Parameters.AddWithValue("serial", (long)guildSerial.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to remove guild faction for {guildSerial}: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        // =====================================================
        // UTILITY OPERATIONS
        // =====================================================

        /// <summary>
        /// Verify faction tables exist and are accessible.
        /// </summary>
        /// <returns>True if tables exist, false otherwise</returns>
        public static bool VerifySchema()
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(
                    @"SELECT COUNT(*) FROM information_schema.tables
                      WHERE table_name IN ('s51a_factions', 's51a_guild_factions')",
                    conn);

                var result = cmd.ExecuteScalar();
                int tableCount = Convert.ToInt32(result);

                if (tableCount == 2)
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine("[Sphere51a] Faction database schema verified");
                    Utility.PopColor();
                    return true;
                }
                else
                {
                    Utility.PushColor(ConsoleColor.Red);
                    Console.WriteLine($"[Sphere51a] Faction schema incomplete - found {tableCount}/2 tables");
                    Utility.PopColor();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Schema verification failed: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        /// <summary>
        /// Get total count of guilds in factions.
        /// </summary>
        /// <returns>Total guild count</returns>
        public static int GetTotalGuildCount()
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM s51a_guild_factions",
                    conn);

                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get total guild count: {ex.Message}");
                Utility.PopColor();
                return 0;
            }
        }
    }
}
