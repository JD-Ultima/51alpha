// =====================================================
// 51ALPHA CORE - DAILY CONTENT REPOSITORY
// =====================================================
// File: DailyContentRepository.cs
// Created: 2025-12-16
// Phase: Sprint 1 - Repository Pattern Expansion
// Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q5: Daily Content tables)
// =====================================================

using System;
using System.Collections.Generic;
using Npgsql;

namespace Server.Sphere51a.Core.Database
{
    /// <summary>
    /// Repository for daily content data access.
    /// Manages quest and dungeon rotation, player progress, and completions.
    /// </summary>
    public static class DailyContentRepository
    {
        /// <summary>
        /// Get active daily quests for a date.
        /// </summary>
        public static List<int> GetActiveDailyQuests(DateTime date)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT quest_id
                    FROM s51a_daily_quests
                    WHERE rotation_date = @date AND is_active = true
                ", conn);

                cmd.Parameters.AddWithValue("date", date.Date);

                var questIds = new List<int>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    questIds.Add(reader.GetInt32(0));
                }

                return questIds;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get active daily quests: {ex.Message}");
                Utility.PopColor();
                return new List<int>();
            }
        }

        /// <summary>
        /// Get active daily dungeons for a date.
        /// </summary>
        public static List<int> GetActiveDailyDungeons(DateTime date)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT dungeon_id
                    FROM s51a_daily_dungeons
                    WHERE rotation_date = @date AND is_active = true
                ", conn);

                cmd.Parameters.AddWithValue("date", date.Date);

                var dungeonIds = new List<int>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dungeonIds.Add(reader.GetInt32(0));
                }

                return dungeonIds;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get active daily dungeons: {ex.Message}");
                Utility.PopColor();
                return new List<int>();
            }
        }

        /// <summary>
        /// Update player quest progress.
        /// </summary>
        public static void UpdateQuestProgress(string accountId, int questId, int increment)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                var today = DateTime.UtcNow.Date;

                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO s51a_quest_progress (account_id, quest_id, rotation_date, current_count)
                    VALUES (@accountId, @questId, @date, @increment)
                    ON CONFLICT (account_id, quest_id, rotation_date)
                    DO UPDATE SET current_count = s51a_quest_progress.current_count + @increment
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("questId", questId);
                cmd.Parameters.AddWithValue("date", today);
                cmd.Parameters.AddWithValue("increment", increment);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to update quest progress: {ex.Message}");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// Complete a quest for a player.
        /// </summary>
        public static bool CompleteQuest(string accountId, int questId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                var today = DateTime.UtcNow.Date;

                // Check if quest is already completed today
                using (var checkCmd = new NpgsqlCommand(@"
                    SELECT completed_at
                    FROM s51a_quest_progress
                    WHERE account_id = @accountId AND quest_id = @questId AND rotation_date = @date
                ", conn))
                {
                    checkCmd.Parameters.AddWithValue("accountId", accountId);
                    checkCmd.Parameters.AddWithValue("questId", questId);
                    checkCmd.Parameters.AddWithValue("date", today);

                    var result = checkCmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        return false; // Already completed
                }

                // Mark as completed
                using var cmd = new NpgsqlCommand(@"
                    UPDATE s51a_quest_progress
                    SET completed_at = NOW()
                    WHERE account_id = @accountId AND quest_id = @questId AND rotation_date = @date
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("questId", questId);
                cmd.Parameters.AddWithValue("date", today);

                var rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to complete quest: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        /// <summary>
        /// Record dungeon completion.
        /// </summary>
        public static bool CompleteDungeon(string accountId, int dungeonId, string lootEarned)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                var today = DateTime.UtcNow.Date;

                // Check if already completed today
                using (var checkCmd = new NpgsqlCommand(@"
                    SELECT COUNT(*)
                    FROM s51a_dungeon_completions
                    WHERE account_id = @accountId AND dungeon_id = @dungeonId AND rotation_date = @date
                ", conn))
                {
                    checkCmd.Parameters.AddWithValue("accountId", accountId);
                    checkCmd.Parameters.AddWithValue("dungeonId", dungeonId);
                    checkCmd.Parameters.AddWithValue("date", today);

                    var count = (long)checkCmd.ExecuteScalar();
                    if (count > 0)
                        return false; // Already completed today
                }

                // Record completion
                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO s51a_dungeon_completions (account_id, dungeon_id, rotation_date, loot_earned)
                    VALUES (@accountId, @dungeonId, @date, @loot)
                    ON CONFLICT (account_id, dungeon_id, rotation_date) DO NOTHING
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("dungeonId", dungeonId);
                cmd.Parameters.AddWithValue("date", today);
                cmd.Parameters.AddWithValue("loot", (object)lootEarned ?? DBNull.Value);

                var rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to record dungeon completion: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        /// <summary>
        /// Get player's quest progress for today.
        /// </summary>
        public static Dictionary<int, int> GetPlayerQuestProgress(string accountId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                var today = DateTime.UtcNow.Date;

                using var cmd = new NpgsqlCommand(@"
                    SELECT quest_id, current_count
                    FROM s51a_quest_progress
                    WHERE account_id = @accountId AND rotation_date = @date AND completed_at IS NULL
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("date", today);

                var progress = new Dictionary<int, int>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    progress[reader.GetInt32(0)] = reader.GetInt32(1);
                }

                return progress;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get quest progress: {ex.Message}");
                Utility.PopColor();
                return new Dictionary<int, int>();
            }
        }

        /// <summary>
        /// Check if player completed a dungeon today.
        /// </summary>
        public static bool HasCompletedDungeon(string accountId, int dungeonId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                var today = DateTime.UtcNow.Date;

                using var cmd = new NpgsqlCommand(@"
                    SELECT COUNT(*)
                    FROM s51a_dungeon_completions
                    WHERE account_id = @accountId AND dungeon_id = @dungeonId AND rotation_date = @date
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("dungeonId", dungeonId);
                cmd.Parameters.AddWithValue("date", today);

                var count = (long)cmd.ExecuteScalar();
                return count > 0;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to check dungeon completion: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        /// <summary>
        /// Activate daily rotation for a specific date.
        /// </summary>
        public static void ActivateDailyRotation(DateTime date, List<int> questIds, List<int> dungeonIds)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    // Activate quests
                    foreach (var questId in questIds)
                    {
                        using var cmd = new NpgsqlCommand(@"
                            INSERT INTO s51a_daily_quests (quest_id, rotation_date, is_active)
                            VALUES (@questId, @date, true)
                            ON CONFLICT (rotation_date, quest_id) DO NOTHING
                        ", conn, transaction);

                        cmd.Parameters.AddWithValue("questId", questId);
                        cmd.Parameters.AddWithValue("date", date.Date);
                        cmd.ExecuteNonQuery();
                    }

                    // Activate dungeons
                    foreach (var dungeonId in dungeonIds)
                    {
                        using var cmd = new NpgsqlCommand(@"
                            INSERT INTO s51a_daily_dungeons (dungeon_id, rotation_date, is_active)
                            VALUES (@dungeonId, @date, true)
                            ON CONFLICT (rotation_date, dungeon_id) DO NOTHING
                        ", conn, transaction);

                        cmd.Parameters.AddWithValue("dungeonId", dungeonId);
                        cmd.Parameters.AddWithValue("date", date.Date);
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
                Console.WriteLine($"[Sphere51a] Failed to activate daily rotation: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }
    }
}
