// =====================================================
// 51ALPHA CORE - TOURNAMENT REPOSITORY
// =====================================================
// File: TournamentRepository.cs
// Created: 2025-12-16
// Phase: Sprint 1 - Repository Pattern Expansion
// Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q5: Tournament System tables)
// =====================================================

using System;
using System.Collections.Generic;
using Npgsql;

namespace Server.Sphere51a.Core.Database
{
    /// <summary>
    /// Repository for tournament data access.
    /// Manages tournament instances, participants, matches, and brackets.
    /// </summary>
    public static class TournamentRepository
    {
        /// <summary>
        /// Create a new tournament.
        /// </summary>
        public static int CreateTournament(string name, string type, DateTime startTime, int maxParticipants, int prizePoolGold)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO s51a_tournaments (tournament_name, tournament_type, start_time, max_participants, prize_pool_gold, status)
                    VALUES (@name, @type, @startTime, @maxParticipants, @prizePool, 'registration')
                    RETURNING tournament_id
                ", conn);

                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("type", type);
                cmd.Parameters.AddWithValue("startTime", startTime);
                cmd.Parameters.AddWithValue("maxParticipants", maxParticipants);
                cmd.Parameters.AddWithValue("prizePool", prizePoolGold);

                return (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to create tournament: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Register a participant for a tournament.
        /// </summary>
        public static bool RegisterParticipant(int tournamentId, string accountId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();

                // Check if tournament is full
                using (var checkCmd = new NpgsqlCommand(@"
                    SELECT current_participants, max_participants
                    FROM s51a_tournaments
                    WHERE tournament_id = @tournamentId AND status = 'registration'
                ", conn))
                {
                    checkCmd.Parameters.AddWithValue("tournamentId", tournamentId);
                    using var reader = checkCmd.ExecuteReader();

                    if (!reader.Read())
                        return false; // Tournament not found or not in registration

                    var current = reader.GetInt32(0);
                    var max = reader.GetInt32(1);

                    if (current >= max)
                        return false; // Tournament is full
                }

                // Register participant
                using var transaction = conn.BeginTransaction();
                try
                {
                    using (var cmd = new NpgsqlCommand(@"
                        INSERT INTO s51a_tournament_participants (tournament_id, account_id)
                        VALUES (@tournamentId, @accountId)
                        ON CONFLICT (tournament_id, account_id) DO NOTHING
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("tournamentId", tournamentId);
                        cmd.Parameters.AddWithValue("accountId", accountId);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new NpgsqlCommand(@"
                        UPDATE s51a_tournaments
                        SET current_participants = current_participants + 1
                        WHERE tournament_id = @tournamentId
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("tournamentId", tournamentId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
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
                Console.WriteLine($"[Sphere51a] Failed to register tournament participant: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        /// <summary>
        /// Start a tournament (change status to in_progress).
        /// </summary>
        public static void StartTournament(int tournamentId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    UPDATE s51a_tournaments
                    SET status = 'in_progress'
                    WHERE tournament_id = @tournamentId
                ", conn);

                cmd.Parameters.AddWithValue("tournamentId", tournamentId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to start tournament: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Create a match in a tournament bracket.
        /// </summary>
        public static long CreateMatch(int tournamentId, int roundNumber, int matchNumber, string player1AccountId, string player2AccountId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO s51a_tournament_matches (tournament_id, round_number, match_number, player1_account_id, player2_account_id, status)
                    VALUES (@tournamentId, @roundNumber, @matchNumber, @player1, @player2, 'pending')
                    RETURNING match_id
                ", conn);

                cmd.Parameters.AddWithValue("tournamentId", tournamentId);
                cmd.Parameters.AddWithValue("roundNumber", roundNumber);
                cmd.Parameters.AddWithValue("matchNumber", matchNumber);
                cmd.Parameters.AddWithValue("player1", player1AccountId);
                cmd.Parameters.AddWithValue("player2", player2AccountId);

                return (long)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to create tournament match: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Record match result and advance winner.
        /// </summary>
        public static void RecordMatchResult(long matchId, string winnerAccountId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    // Update match
                    using (var cmd = new NpgsqlCommand(@"
                        UPDATE s51a_tournament_matches
                        SET winner_account_id = @winnerId,
                            match_end_time = NOW(),
                            status = 'completed'
                        WHERE match_id = @matchId
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("matchId", matchId);
                        cmd.Parameters.AddWithValue("winnerId", winnerAccountId);
                        cmd.ExecuteNonQuery();
                    }

                    // Get loser account ID
                    string loserAccountId;
                    using (var cmd = new NpgsqlCommand(@"
                        SELECT CASE WHEN player1_account_id = @winnerId THEN player2_account_id ELSE player1_account_id END
                        FROM s51a_tournament_matches
                        WHERE match_id = @matchId
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("matchId", matchId);
                        cmd.Parameters.AddWithValue("winnerId", winnerAccountId);
                        loserAccountId = (string)cmd.ExecuteScalar();
                    }

                    // Update winner's round
                    using (var cmd = new NpgsqlCommand(@"
                        UPDATE s51a_tournament_participants
                        SET current_round = current_round + 1
                        WHERE tournament_id = (SELECT tournament_id FROM s51a_tournament_matches WHERE match_id = @matchId)
                          AND account_id = @winnerId
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("matchId", matchId);
                        cmd.Parameters.AddWithValue("winnerId", winnerAccountId);
                        cmd.ExecuteNonQuery();
                    }

                    // Eliminate loser
                    using (var cmd = new NpgsqlCommand(@"
                        UPDATE s51a_tournament_participants
                        SET is_eliminated = true
                        WHERE tournament_id = (SELECT tournament_id FROM s51a_tournament_matches WHERE match_id = @matchId)
                          AND account_id = @loserId
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("matchId", matchId);
                        cmd.Parameters.AddWithValue("loserId", loserAccountId);
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
                Console.WriteLine($"[Sphere51a] Failed to record match result: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Complete a tournament and record the winner.
        /// </summary>
        public static void CompleteTournament(int tournamentId, string winnerAccountId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var transaction = conn.BeginTransaction();

                try
                {
                    // Update tournament
                    using (var cmd = new NpgsqlCommand(@"
                        UPDATE s51a_tournaments
                        SET status = 'completed',
                            end_time = NOW(),
                            winner_account_id = @winnerId
                        WHERE tournament_id = @tournamentId
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("tournamentId", tournamentId);
                        cmd.Parameters.AddWithValue("winnerId", winnerAccountId);
                        cmd.ExecuteNonQuery();
                    }

                    // Record in history
                    using (var cmd = new NpgsqlCommand(@"
                        INSERT INTO s51a_tournament_history (tournament_id, winner_account_id, total_participants)
                        SELECT tournament_id, @winnerId, current_participants
                        FROM s51a_tournaments
                        WHERE tournament_id = @tournamentId
                    ", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("tournamentId", tournamentId);
                        cmd.Parameters.AddWithValue("winnerId", winnerAccountId);
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
                Console.WriteLine($"[Sphere51a] Failed to complete tournament: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Get active participants for a tournament.
        /// </summary>
        public static List<string> GetActiveParticipants(int tournamentId)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT account_id
                    FROM s51a_tournament_participants
                    WHERE tournament_id = @tournamentId AND is_eliminated = false
                    ORDER BY seed_position
                ", conn);

                cmd.Parameters.AddWithValue("tournamentId", tournamentId);

                var participants = new List<string>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    participants.Add(reader.GetString(0));
                }

                return participants;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get active participants: {ex.Message}");
                Utility.PopColor();
                return new List<string>();
            }
        }
    }
}
