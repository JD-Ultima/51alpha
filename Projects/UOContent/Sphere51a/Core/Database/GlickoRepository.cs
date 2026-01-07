// =====================================================
// 51ALPHA CORE - GLICKO REPOSITORY
// =====================================================
// File: GlickoRepository.cs
// Created: 2025-12-16
// Phase: Sprint 1 - Repository Pattern Expansion
// Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q5: Glicko tables)
// =====================================================

using System;
using System.Collections.Generic;
using Npgsql;

namespace Server.Sphere51a.Core.Database
{
    /// <summary>
    /// Repository for Glicko-2 rating data access.
    /// Manages player ratings, rating history, and leaderboards.
    /// </summary>
    public static class GlickoRepository
    {
        /// <summary>
        /// Get or create player Glicko-2 rating.
        /// </summary>
        public static (decimal rating, decimal rd, decimal volatility) GetPlayerRating(string accountId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();

                // Try to get existing rating
                using (var cmd = new NpgsqlCommand(@"
                    SELECT rating, rating_deviation, volatility
                    FROM s51a_glicko_ratings
                    WHERE account_id = @accountId
                ", conn))
                {
                    cmd.Parameters.AddWithValue("accountId", accountId);

                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return (reader.GetDecimal(0), reader.GetDecimal(1), reader.GetDecimal(2));
                    }
                }

                // Create default rating if not exists
                using (var cmd = new NpgsqlCommand(@"
                    INSERT INTO s51a_glicko_ratings (account_id, rating, rating_deviation, volatility)
                    VALUES (@accountId, 1500.0, 350.0, 0.06)
                    RETURNING rating, rating_deviation, volatility
                ", conn))
                {
                    cmd.Parameters.AddWithValue("accountId", accountId);

                    using var reader = cmd.ExecuteReader();
                    reader.Read();
                    return (reader.GetDecimal(0), reader.GetDecimal(1), reader.GetDecimal(2));
                }
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get player rating: {ex.Message}");
                Utility.PopColor();
                return (1500m, 350m, 0.06m);
            }
        }

        /// <summary>
        /// Update player Glicko-2 rating after a match.
        /// </summary>
        public static void UpdatePlayerRating(string accountId, decimal newRating, decimal newRd, decimal newVolatility,
            long? matchId, string opponentAccountId, string matchResult)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    // Get old rating for history
                    decimal oldRating, oldRd;
                    using (var cmd = new NpgsqlCommand(@"
                        SELECT rating, rating_deviation
                        FROM s51a_glicko_ratings
                        WHERE account_id = @accountId
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("accountId", accountId);

                        using var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            oldRating = reader.GetDecimal(0);
                            oldRd = reader.GetDecimal(1);
                        }
                        else
                        {
                            oldRating = 1500m;
                            oldRd = 350m;
                        }
                    }

                    // Update rating
                    using (var cmd = new NpgsqlCommand(@"
                        INSERT INTO s51a_glicko_ratings (account_id, rating, rating_deviation, volatility, last_match_at, total_matches, wins, losses)
                        VALUES (@accountId, @rating, @rd, @volatility, NOW(), 1, @wins, @losses)
                        ON CONFLICT (account_id)
                        DO UPDATE SET
                            rating = @rating,
                            rating_deviation = @rd,
                            volatility = @volatility,
                            last_match_at = NOW(),
                            total_matches = s51a_glicko_ratings.total_matches + 1,
                            wins = s51a_glicko_ratings.wins + @wins,
                            losses = s51a_glicko_ratings.losses + @losses,
                            updated_at = NOW()
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("accountId", accountId);
                        cmd.Parameters.AddWithValue("rating", newRating);
                        cmd.Parameters.AddWithValue("rd", newRd);
                        cmd.Parameters.AddWithValue("volatility", newVolatility);
                        cmd.Parameters.AddWithValue("wins", matchResult == "win" ? 1 : 0);
                        cmd.Parameters.AddWithValue("losses", matchResult == "loss" ? 1 : 0);

                        cmd.ExecuteNonQuery();
                    }

                    // Record history
                    using (var cmd = new NpgsqlCommand(@"
                        INSERT INTO s51a_glicko_history (account_id, match_id, old_rating, new_rating, old_rd, new_rd, opponent_account_id, match_result)
                        VALUES (@accountId, @matchId, @oldRating, @newRating, @oldRd, @newRd, @opponentId, @result)
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("accountId", accountId);
                        cmd.Parameters.AddWithValue("matchId", (object)matchId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("oldRating", oldRating);
                        cmd.Parameters.AddWithValue("newRating", newRating);
                        cmd.Parameters.AddWithValue("oldRd", oldRd);
                        cmd.Parameters.AddWithValue("newRd", newRd);
                        cmd.Parameters.AddWithValue("opponentId", (object)opponentAccountId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("result", matchResult);

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
                Console.WriteLine($"[Sphere51a] Failed to update player rating: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Get top players from Glicko leaderboard.
        /// </summary>
        public static List<(string accountId, decimal rating, int wins, int losses)> GetLeaderboard(int limit = 100)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT account_id, rating, wins, losses
                    FROM s51a_glicko_ratings
                    WHERE total_matches >= 10
                    ORDER BY rating DESC
                    LIMIT @limit
                ", conn);

                cmd.Parameters.AddWithValue("limit", limit);

                var leaderboard = new List<(string, decimal, int, int)>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    leaderboard.Add((
                        reader.GetString(0),
                        reader.GetDecimal(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3)
                    ));
                }

                return leaderboard;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get leaderboard: {ex.Message}");
                Utility.PopColor();
                return new List<(string, decimal, int, int)>();
            }
        }

        /// <summary>
        /// Get player's rating history.
        /// </summary>
        public static List<(DateTime timestamp, decimal oldRating, decimal newRating, string result)> GetRatingHistory(string accountId, int limit = 50)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT timestamp, old_rating, new_rating, match_result
                    FROM s51a_glicko_history
                    WHERE account_id = @accountId
                    ORDER BY timestamp DESC
                    LIMIT @limit
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("limit", limit);

                var history = new List<(DateTime, decimal, decimal, string)>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    history.Add((
                        reader.GetDateTime(0),
                        reader.GetDecimal(1),
                        reader.GetDecimal(2),
                        reader.GetString(3)
                    ));
                }

                return history;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get rating history: {ex.Message}");
                Utility.PopColor();
                return new List<(DateTime, decimal, decimal, string)>();
            }
        }

        /// <summary>
        /// Get player's rank on the leaderboard.
        /// </summary>
        public static int GetPlayerRank(string accountId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    WITH ranked AS (
                        SELECT account_id, RANK() OVER (ORDER BY rating DESC) as rank
                        FROM s51a_glicko_ratings
                        WHERE total_matches >= 10
                    )
                    SELECT rank
                    FROM ranked
                    WHERE account_id = @accountId
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get player rank: {ex.Message}");
                Utility.PopColor();
                return 0;
            }
        }
    }
}
