// =====================================================
// 51ALPHA CORE - SERVER CONFIGURATION
// =====================================================
// File: S51aConfig.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: PHASE1_VERIFICATION_REPORT_v3.md (Q1: Week 1 lockout)
//                PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q2: Server launch date)
// =====================================================

using System;
using System.IO;
using Server.Sphere51a.Core.Database;
using Npgsql;

namespace Server.Sphere51a.Core
{
    /// <summary>
    /// Sphere51a server configuration.
    /// Loads configuration from PostgreSQL s51a_config table.
    /// </summary>
    public static class S51aConfig
    {
        /// <summary>
        /// Server launch date (first startup timestamp).
        /// Used for Week 1 faction change lockout.
        /// </summary>
        public static DateTime ServerLaunchDate { get; private set; }

        /// <summary>
        /// Check if faction changes are locked (Week 1 lockout).
        /// Blocks all faction changes for 7 days after server launch.
        /// </summary>
        public static bool IsFactionChangeLocked => DateTime.UtcNow < ServerLaunchDate + TimeSpan.FromDays(7);

        /// <summary>
        /// Enable equipment double-click swap feature.
        /// Allows players to double-click items in backpack or ground (within 2 tiles) to equip.
        /// </summary>
        public static bool EquipmentSwapEnabled => true;

        private static bool _isInitialized = false;

        /// <summary>
        /// Initialize Sphere51a configuration.
        /// Loads server launch date from PostgreSQL.
        /// Called during server startup.
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                Console.WriteLine("[Sphere51a] Configuration already initialized - skipping");
                return;
            }

            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine("=== Sphere51a Configuration Initialization ===");
            Utility.PopColor();

            // Configure PostgreSQL connection
            ConfigurePostgresConnection();

            // Load server launch date
            LoadServerLaunchDate();

            _isInitialized = true;

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[Sphere51a] Server launch date: {ServerLaunchDate:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"[Sphere51a] Faction changes locked: {IsFactionChangeLocked}");
            if (IsFactionChangeLocked)
            {
                var remaining = (ServerLaunchDate + TimeSpan.FromDays(7)) - DateTime.UtcNow;
                Console.WriteLine($"[Sphere51a] Week 1 lockout ends in: {remaining.Days} days, {remaining.Hours} hours");
            }
            Utility.PopColor();
        }

        /// <summary>
        /// Configure PostgreSQL connection from ModernUO settings.
        /// </summary>
        private static void ConfigurePostgresConnection()
        {
            // Try to get connection string from ServerConfiguration
            // Default fallback if not configured
            string connectionString = ServerConfiguration.GetSetting(
                "s51a.postgres.connection",
                "Host=localhost;Database=sphere51a;Username=s51a;Password=changeme");

            PostgresConnection.Configure(connectionString);

            // Test connectivity
            if (PostgresConnection.TestConnection())
            {
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine("[Sphere51a] PostgreSQL connection successful");
                Utility.PopColor();

                // Run database migrations
                RunDatabaseMigrations();
            }
            else
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine("[Sphere51a] WARNING: PostgreSQL connection failed - faction system will not work");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// Run database migrations to ensure schema is up to date.
        /// </summary>
        private static void RunDatabaseMigrations()
        {
            try
            {
                // Get migration directory path relative to server root
                var serverRoot = Server.Core.BaseDirectory;
                var migrationDir = Path.Combine(serverRoot, "Distribution", "Data", "Postgres", "Migrations");

                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[Sphere51a] Running database migrations from: {migrationDir}");
                Utility.PopColor();

                var appliedCount = MigrationRunner.RunMigrations(migrationDir);

                if (appliedCount > 0)
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[Sphere51a] Applied {appliedCount} migration(s)");
                    Utility.PopColor();
                }

                var currentVersion = MigrationRunner.GetCurrentSchemaVersion();
                Console.WriteLine($"[Sphere51a] Current schema version: {currentVersion}");
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] ERROR: Migration failed: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Load server launch date from PostgreSQL.
        /// Creates entry on first startup.
        /// </summary>
        private static void LoadServerLaunchDate()
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();

                // Insert launch date if not exists (first startup)
                using var insertCmd = new NpgsqlCommand(
                    @"INSERT INTO s51a_config (key, value, created_at)
                      VALUES ('server_launch_date', NOW()::TEXT, NOW())
                      ON CONFLICT (key) DO NOTHING",
                    conn);

                insertCmd.ExecuteNonQuery();

                // Read launch date
                using var selectCmd = new NpgsqlCommand(
                    "SELECT value FROM s51a_config WHERE key = 'server_launch_date'",
                    conn);

                var result = selectCmd.ExecuteScalar();
                if (result != null)
                {
                    ServerLaunchDate = DateTime.Parse(result.ToString()).ToUniversalTime();
                }
                else
                {
                    // Fallback: Use current time if database read fails
                    ServerLaunchDate = DateTime.UtcNow;
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine("[Sphere51a] WARNING: Could not read server launch date - using current time");
                    Utility.PopColor();
                }
            }
            catch (Exception ex)
            {
                // Fallback: Use current time if database access fails
                ServerLaunchDate = DateTime.UtcNow;
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] ERROR: Failed to load server launch date: {ex.Message}");
                Console.WriteLine("[Sphere51a] Using current time as fallback");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// Check if configuration is initialized.
        /// </summary>
        public static bool IsInitialized => _isInitialized;

        /// <summary>
        /// Get configuration value from database.
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value</returns>
        public static string GetConfigValue(string key, string defaultValue = null)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(
                    "SELECT value FROM s51a_config WHERE key = @key",
                    conn);

                cmd.Parameters.AddWithValue("key", key);

                var result = cmd.ExecuteScalar();
                return result?.ToString() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Set configuration value in database.
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="value">Configuration value</param>
        /// <returns>True if successful</returns>
        public static bool SetConfigValue(string key, string value)
        {
            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand(
                    @"INSERT INTO s51a_config (key, value, created_at, updated_at)
                      VALUES (@key, @value, NOW(), NOW())
                      ON CONFLICT (key) DO UPDATE SET value = @value, updated_at = NOW()",
                    conn);

                cmd.Parameters.AddWithValue("key", key);
                cmd.Parameters.AddWithValue("value", value);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to set config value '{key}': {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }
    }
}
