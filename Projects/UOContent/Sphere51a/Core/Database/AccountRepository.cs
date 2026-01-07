// =====================================================
// 51ALPHA CORE - ACCOUNT REPOSITORY
// =====================================================
// File: AccountRepository.cs
// Created: 2025-12-16
// Phase: Sprint 1 - Core Authentication System
// Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q5: account_mapping table)
// =====================================================

using System;
using System.Collections.Generic;
using Npgsql;

namespace Server.Sphere51a.Core.Database
{
    /// <summary>
    /// Repository for account-to-character mapping in PostgreSQL.
    /// Tracks which characters belong to which accounts for account-bound features.
    /// </summary>
    public static class AccountRepository
    {
        /// <summary>
        /// Register a character to an account in the database.
        /// Creates or updates the account_mapping entry.
        /// </summary>
        /// <param name="accountId">ModernUO Account.Username</param>
        /// <param name="characterSerial">ModernUO Mobile.Serial</param>
        /// <param name="characterName">Character name</param>
        public static void RegisterCharacter(string accountId, int characterSerial, string characterName)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentException("Account ID cannot be null or empty", nameof(accountId));

            if (string.IsNullOrWhiteSpace(characterName))
                throw new ArgumentException("Character name cannot be null or empty", nameof(characterName));

            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO account_mapping (account_id, character_serial, character_name, last_login, created_at)
                    VALUES (@accountId, @serial, @name, NOW(), NOW())
                    ON CONFLICT (account_id, character_serial)
                    DO UPDATE SET character_name = @name, last_login = NOW()
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("serial", (long)characterSerial);
                cmd.Parameters.AddWithValue("name", characterName);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to register character {characterName} ({characterSerial}) for account {accountId}: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Get all character serials for an account.
        /// </summary>
        /// <param name="accountId">ModernUO Account.Username</param>
        /// <returns>List of character serials</returns>
        public static List<int> GetAccountCharacters(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                return new List<int>();

            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT character_serial
                    FROM account_mapping
                    WHERE account_id = @accountId
                    ORDER BY last_login DESC
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);

                var characters = new List<int>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    characters.Add((int)reader.GetInt64(0));
                }

                return characters;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get characters for account {accountId}: {ex.Message}");
                Utility.PopColor();
                return new List<int>();
            }
        }

        /// <summary>
        /// Get account ID for a character serial.
        /// </summary>
        /// <param name="characterSerial">ModernUO Mobile.Serial</param>
        /// <returns>Account ID or null if not found</returns>
        public static string GetCharacterAccount(int characterSerial)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT account_id
                    FROM account_mapping
                    WHERE character_serial = @serial
                ", conn);

                cmd.Parameters.AddWithValue("serial", (long)characterSerial);

                var result = cmd.ExecuteScalar();
                return result?.ToString();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get account for character {characterSerial}: {ex.Message}");
                Utility.PopColor();
                return null;
            }
        }

        /// <summary>
        /// Update last login timestamp for a character.
        /// </summary>
        /// <param name="characterSerial">ModernUO Mobile.Serial</param>
        public static void UpdateLastLogin(int characterSerial)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    UPDATE account_mapping
                    SET last_login = NOW()
                    WHERE character_serial = @serial
                ", conn);

                cmd.Parameters.AddWithValue("serial", (long)characterSerial);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Non-critical error - log but don't throw
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[Sphere51a] Failed to update last login for character {characterSerial}: {ex.Message}");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// Check if a character belongs to an account.
        /// </summary>
        /// <param name="accountId">ModernUO Account.Username</param>
        /// <param name="characterSerial">ModernUO Mobile.Serial</param>
        /// <returns>True if character belongs to account</returns>
        public static bool IsCharacterOwnedByAccount(string accountId, int characterSerial)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                return false;

            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT COUNT(*)
                    FROM account_mapping
                    WHERE account_id = @accountId AND character_serial = @serial
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);
                cmd.Parameters.AddWithValue("serial", (long)characterSerial);

                var count = (long)cmd.ExecuteScalar();
                return count > 0;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to check character ownership: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        /// <summary>
        /// Get character name from database.
        /// </summary>
        /// <param name="characterSerial">ModernUO Mobile.Serial</param>
        /// <returns>Character name or null if not found</returns>
        public static string GetCharacterName(int characterSerial)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT character_name
                    FROM account_mapping
                    WHERE character_serial = @serial
                ", conn);

                cmd.Parameters.AddWithValue("serial", (long)characterSerial);

                var result = cmd.ExecuteScalar();
                return result?.ToString();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get character name for {characterSerial}: {ex.Message}");
                Utility.PopColor();
                return null;
            }
        }

        /// <summary>
        /// Get total character count for an account.
        /// </summary>
        /// <param name="accountId">ModernUO Account.Username</param>
        /// <returns>Number of characters</returns>
        public static int GetAccountCharacterCount(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                return 0;

            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT COUNT(*)
                    FROM account_mapping
                    WHERE account_id = @accountId
                ", conn);

                cmd.Parameters.AddWithValue("accountId", accountId);

                return (int)(long)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get character count for account {accountId}: {ex.Message}");
                Utility.PopColor();
                return 0;
            }
        }
    }
}
