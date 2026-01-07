// =====================================================
// 51ALPHA CORE - AUTHENTICATION EVENT HOOKS
// =====================================================
// File: S51aAuthenticationHooks.cs
// Created: 2025-12-16
// Phase: Sprint 1 - Core Authentication System
// Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q6: Authentication)
// =====================================================

using System;
using Server.Accounting;
using Server.Sphere51a.Core.Database;

namespace Server.Sphere51a.Core
{
    /// <summary>
    /// Authentication event hooks for Sphere51a.
    /// Tracks player logins and maintains account-to-character mapping.
    /// </summary>
    public static class S51aAuthenticationHooks
    {
        private static bool _isConfigured = false;

        /// <summary>
        /// Configure authentication event hooks.
        /// Called during server startup.
        /// </summary>
        public static void Configure()
        {
            if (_isConfigured)
            {
                Console.WriteLine("[Sphere51a] Authentication hooks already configured - skipping");
                return;
            }

            // Hook into login events
            EventSink.Connected += OnPlayerConnected;
            EventSink.Logout += OnPlayerLogout;

            _isConfigured = true;

            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine("[Sphere51a] Authentication hooks configured");
            Utility.PopColor();
        }

        /// <summary>
        /// Handle player connected event.
        /// Registers character-to-account mapping and updates last login timestamp.
        /// </summary>
        private static void OnPlayerConnected(Mobile mobile)
        {
            if (mobile == null || mobile.Account == null)
                return;

            try
            {
                var account = mobile.Account as Account;

                if (account == null)
                    return;

                var accountId = account.Username;
                var characterSerial = (int)mobile.Serial.Value;
                var characterName = mobile.Name ?? "Unknown";

                // Register character to account in PostgreSQL
                if (PostgresConnection.IsConfigured)
                {
                    AccountRepository.RegisterCharacter(accountId, characterSerial, characterName);

                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[Sphere51a] Player connected: {characterName} (Account: {accountId}, Serial: {characterSerial})");
                    Utility.PopColor();
                }
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Connected event error: {ex.Message}");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// Handle player logout event.
        /// Updates last login timestamp (used for account-bound features).
        /// </summary>
        private static void OnPlayerLogout(Mobile mobile)
        {
            if (mobile == null)
                return;

            try
            {
                var characterSerial = (int)mobile.Serial.Value;

                // Update last login timestamp
                if (PostgresConnection.IsConfigured)
                {
                    AccountRepository.UpdateLastLogin(characterSerial);

                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[Sphere51a] Player logout: {mobile.Name} (Serial: {characterSerial})");
                    Utility.PopColor();
                }
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Logout event error: {ex.Message}");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// Get account ID for a mobile (helper method).
        /// </summary>
        /// <param name="mobile">Mobile to check</param>
        /// <returns>Account username or null</returns>
        public static string GetAccountId(Mobile mobile)
        {
            if (mobile?.Account is not Account account)
                return null;

            return account.Username;
        }

        /// <summary>
        /// Check if a mobile is owned by a specific account.
        /// </summary>
        /// <param name="mobile">Mobile to check</param>
        /// <param name="accountId">Account ID to verify</param>
        /// <returns>True if mobile belongs to account</returns>
        public static bool IsMobileOwnedByAccount(Mobile mobile, string accountId)
        {
            if (mobile == null || string.IsNullOrWhiteSpace(accountId))
                return false;

            var mobileAccountId = GetAccountId(mobile);
            return string.Equals(mobileAccountId, accountId, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get all characters for a mobile's account.
        /// </summary>
        /// <param name="mobile">Mobile to check</param>
        /// <returns>List of character serials</returns>
        public static System.Collections.Generic.List<int> GetAccountCharacters(Mobile mobile)
        {
            var accountId = GetAccountId(mobile);
            if (string.IsNullOrWhiteSpace(accountId))
                return new System.Collections.Generic.List<int>();

            return AccountRepository.GetAccountCharacters(accountId);
        }
    }
}
