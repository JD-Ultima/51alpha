// =====================================================
// 51ALPHA CORE - NPE (NEW PLAYER EXPERIENCE) REPOSITORY
// =====================================================
// File: NPERepository.cs
// Created: 2025-12-16
// Phase: Sprint 1 - Repository Pattern Expansion
// Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q5: NPE tables)
// =====================================================

using System;
using System.Collections.Generic;
using Npgsql;

namespace Server.Sphere51a.Core.Database
{
    /// <summary>
    /// Repository for New Player Experience (NPE) data access.
    /// Manages tutorial checkpoint definitions and player progression.
    /// </summary>
    public static class NPERepository
    {
        /// <summary>
        /// Complete a tutorial checkpoint for a player.
        /// </summary>
        public static bool CompleteCheckpoint(string accountId, int checkpointId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();

                // Check if already completed
                using (var checkCmd = new NpgsqlCommand(@"
                    SELECT COUNT(*)
                    FROM s51a_npe_progress
                    WHERE account_id = @accountId AND checkpoint_id = @checkpointId
                ", conn))
                {
                    checkCmd.Parameters.AddWithValue("accountId", accountId);
                    checkCmd.Parameters.AddWithValue("checkpointId", checkpointId);

                    var count = (long)checkCmd.ExecuteScalar();
                    if (count > 0)
                        return false; // Already completed
                }

                // Record completion
                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO s51a_npe_progress (account_id, checkpoint_id)
                    VALUES (@accountId, @checkpointId)
                    ON CONFLICT (account_id, checkpoint_id) DO NOTHING
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("checkpointId", checkpointId);

                var rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to complete checkpoint: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        /// <summary>
        /// Get all completed checkpoints for a player.
        /// </summary>
        public static List<int> GetCompletedCheckpoints(string accountId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT checkpoint_id
                    FROM s51a_npe_progress
                    WHERE account_id = @accountId
                    ORDER BY checkpoint_id
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);

                var checkpoints = new List<int>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    checkpoints.Add(reader.GetInt32(0));
                }

                return checkpoints;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get completed checkpoints: {ex.Message}");
                Utility.PopColor();
                return new List<int>();
            }
        }

        /// <summary>
        /// Get next incomplete checkpoint for a player.
        /// </summary>
        public static int? GetNextCheckpoint(string accountId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT c.checkpoint_id
                    FROM s51a_npe_checkpoints c
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM s51a_npe_progress p
                        WHERE p.account_id = @accountId AND p.checkpoint_id = c.checkpoint_id
                    )
                    ORDER BY c.checkpoint_order
                    LIMIT 1
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);

                var result = cmd.ExecuteScalar();
                return result != null ? (int)result : null;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get next checkpoint: {ex.Message}");
                Utility.PopColor();
                return null;
            }
        }

        /// <summary>
        /// Check if player has completed the tutorial.
        /// </summary>
        public static bool HasCompletedTutorial(string accountId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT EXISTS (
                        SELECT 1
                        FROM s51a_npe_progress
                        WHERE account_id = @accountId
                          AND checkpoint_id = (SELECT checkpoint_id FROM s51a_npe_checkpoints WHERE checkpoint_name = 'Tutorial Complete')
                    )
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);

                return (bool)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to check tutorial completion: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        /// <summary>
        /// Get checkpoint details (name, order, rewards).
        /// </summary>
        public static (string name, int order, int rewardGold, int rewardPoints)? GetCheckpointDetails(int checkpointId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT checkpoint_name, checkpoint_order, reward_gold, reward_points
                    FROM s51a_npe_checkpoints
                    WHERE checkpoint_id = @checkpointId
                ", conn);

                cmd.Parameters.AddWithValue("checkpointId", checkpointId);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return (
                        reader.GetString(0),
                        reader.GetInt32(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3)
                    );
                }

                return null;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get checkpoint details: {ex.Message}");
                Utility.PopColor();
                return null;
            }
        }

        /// <summary>
        /// Get all checkpoint definitions (for UI/display).
        /// </summary>
        public static List<(int id, string name, int order, int rewardGold, int rewardPoints)> GetAllCheckpoints()
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT checkpoint_id, checkpoint_name, checkpoint_order, reward_gold, reward_points
                    FROM s51a_npe_checkpoints
                    ORDER BY checkpoint_order
                ", conn);

                var checkpoints = new List<(int, string, int, int, int)>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    checkpoints.Add((
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3),
                        reader.GetInt32(4)
                    ));
                }

                return checkpoints;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get all checkpoints: {ex.Message}");
                Utility.PopColor();
                return new List<(int, string, int, int, int)>();
            }
        }

        /// <summary>
        /// Get tutorial completion percentage for a player.
        /// </summary>
        public static decimal GetCompletionPercentage(string accountId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    WITH totals AS (
                        SELECT COUNT(*) as total_checkpoints FROM s51a_npe_checkpoints
                    ),
                    completed AS (
                        SELECT COUNT(*) as completed_count
                        FROM s51a_npe_progress
                        WHERE account_id = @accountId
                    )
                    SELECT CASE
                        WHEN totals.total_checkpoints = 0 THEN 0
                        ELSE (completed.completed_count::decimal / totals.total_checkpoints * 100)
                    END
                    FROM totals, completed
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0m;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get completion percentage: {ex.Message}");
                Utility.PopColor();
                return 0m;
            }
        }

        /// <summary>
        /// Check if a specific checkpoint is completed.
        /// </summary>
        public static bool IsCheckpointCompleted(string accountId, int checkpointId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT EXISTS (
                        SELECT 1
                        FROM s51a_npe_progress
                        WHERE account_id = @accountId AND checkpoint_id = @checkpointId
                    )
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("checkpointId", checkpointId);

                return (bool)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to check checkpoint completion: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }
    }
}
