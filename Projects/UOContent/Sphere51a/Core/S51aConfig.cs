// =====================================================
// 51ALPHA CORE - SERVER CONFIGURATION
// =====================================================
// File: S51aConfig.cs
// Created: 2025-12-14
// Updated: 2025-01-13 - Added SQLite support for local development
// Phase: Phase 1 - Three-Faction System Foundation
// =====================================================

using System;
using System.IO;
using Server.Sphere51a.Core.Database;

namespace Server.Sphere51a.Core
{
    /// <summary>
    /// Database provider selection.
    /// </summary>
    public enum DatabaseProvider
    {
        /// <summary>SQLite - Local development, no installation needed</summary>
        SQLite,
        /// <summary>PostgreSQL - Production server with multiple players</summary>
        PostgreSQL
    }

    /// <summary>
    /// Sphere51a server configuration.
    /// Supports SQLite (development) and PostgreSQL (production).
    /// </summary>
    public static class S51aConfig
    {
        /// <summary>
        /// Current database provider.
        /// </summary>
        public static DatabaseProvider Provider { get; private set; } = DatabaseProvider.SQLite;

        /// <summary>
        /// The active faction repository instance.
        /// Use this for all database operations.
        /// </summary>
        public static IFactionRepository FactionRepository { get; private set; }

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
        /// </summary>
        public static bool EquipmentSwapEnabled => true;

        private static bool _isInitialized = false;
        private static string _launchDateFile;

        /// <summary>
        /// Initialize Sphere51a configuration.
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

            // Determine which database provider to use
            SelectDatabaseProvider();

            // Initialize the selected provider
            InitializeDatabase();

            // Load server launch date
            LoadServerLaunchDate();

            _isInitialized = true;

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[Sphere51a] Database provider: {Provider}");
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
        /// Select database provider based on configuration.
        /// Defaults to SQLite for easy local development.
        /// </summary>
        private static void SelectDatabaseProvider()
        {
            // Check for explicit provider setting in ModernUO config
            string providerSetting = ServerConfiguration.GetSetting("s51a.database.provider", "sqlite");

            if (providerSetting.Equals("postgresql", StringComparison.OrdinalIgnoreCase) ||
                providerSetting.Equals("postgres", StringComparison.OrdinalIgnoreCase))
            {
                // Check if PostgreSQL connection string is configured
                string pgConnectionString = ServerConfiguration.GetSetting("s51a.postgres.connection", "");

                if (!string.IsNullOrWhiteSpace(pgConnectionString))
                {
                    Provider = DatabaseProvider.PostgreSQL;
                    Console.WriteLine("[Sphere51a] Using PostgreSQL database (production mode)");
                }
                else
                {
                    Provider = DatabaseProvider.SQLite;
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine("[Sphere51a] PostgreSQL requested but no connection string configured");
                    Console.WriteLine("[Sphere51a] Falling back to SQLite");
                    Utility.PopColor();
                }
            }
            else
            {
                Provider = DatabaseProvider.SQLite;
                Console.WriteLine("[Sphere51a] Using SQLite database (development mode)");
            }
        }

        /// <summary>
        /// Initialize the selected database provider.
        /// </summary>
        private static void InitializeDatabase()
        {
            switch (Provider)
            {
                case DatabaseProvider.SQLite:
                    InitializeSqlite();
                    break;

                case DatabaseProvider.PostgreSQL:
                    InitializePostgres();
                    break;
            }
        }

        /// <summary>
        /// Initialize SQLite database.
        /// </summary>
        private static void InitializeSqlite()
        {
            try
            {
                var repository = new SqliteFactionRepository();

                if (repository.Initialize())
                {
                    FactionRepository = repository;
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine("[Sphere51a] SQLite database ready");
                    Utility.PopColor();
                }
                else
                {
                    Utility.PushColor(ConsoleColor.Red);
                    Console.WriteLine("[Sphere51a] ERROR: SQLite initialization failed");
                    Utility.PopColor();
                }
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] ERROR: SQLite initialization failed: {ex.Message}");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// Initialize PostgreSQL database.
        /// </summary>
        private static void InitializePostgres()
        {
            try
            {
                string connectionString = ServerConfiguration.GetSetting(
                    "s51a.postgres.connection",
                    "Host=localhost;Database=sphere51a;Username=s51a;Password=changeme");

                PostgresConnection.Configure(connectionString);

                if (PostgresConnection.TestConnection())
                {
                    // Create PostgreSQL repository adapter
                    FactionRepository = new PostgresFactionRepositoryAdapter();

                    // Run migrations
                    RunDatabaseMigrations();

                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine("[Sphere51a] PostgreSQL connection successful");
                    Utility.PopColor();
                }
                else
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine("[Sphere51a] PostgreSQL connection failed - falling back to SQLite");
                    Utility.PopColor();

                    Provider = DatabaseProvider.SQLite;
                    InitializeSqlite();
                }
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[Sphere51a] PostgreSQL error: {ex.Message}");
                Console.WriteLine("[Sphere51a] Falling back to SQLite");
                Utility.PopColor();

                Provider = DatabaseProvider.SQLite;
                InitializeSqlite();
            }
        }

        /// <summary>
        /// Run PostgreSQL database migrations.
        /// </summary>
        private static void RunDatabaseMigrations()
        {
            try
            {
                var serverRoot = Server.Core.BaseDirectory;
                var migrationDir = Path.Combine(serverRoot, "Data", "Postgres", "Migrations");

                if (!Directory.Exists(migrationDir))
                {
                    Console.WriteLine($"[Sphere51a] Migration directory not found: {migrationDir}");
                    return;
                }

                Console.WriteLine($"[Sphere51a] Running migrations from: {migrationDir}");

                var appliedCount = MigrationRunner.RunMigrations(migrationDir);

                if (appliedCount > 0)
                {
                    Console.WriteLine($"[Sphere51a] Applied {appliedCount} migration(s)");
                }

                var currentVersion = MigrationRunner.GetCurrentSchemaVersion();
                Console.WriteLine($"[Sphere51a] Schema version: {currentVersion}");
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[Sphere51a] Migration warning: {ex.Message}");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// Load server launch date.
        /// Uses a simple file for SQLite, database for PostgreSQL.
        /// </summary>
        private static void LoadServerLaunchDate()
        {
            // Use file-based launch date tracking (works with both providers)
            _launchDateFile = Path.Combine(Server.Core.BaseDirectory, "Saves", "sphere51a_launch.txt");

            if (File.Exists(_launchDateFile))
            {
                try
                {
                    var dateStr = File.ReadAllText(_launchDateFile).Trim();
                    ServerLaunchDate = DateTime.Parse(dateStr).ToUniversalTime();
                    return;
                }
                catch
                {
                    // Fall through to create new launch date
                }
            }

            // First launch - record the date
            ServerLaunchDate = DateTime.UtcNow;

            try
            {
                // Ensure directory exists
                var saveDir = Path.GetDirectoryName(_launchDateFile);
                if (!Directory.Exists(saveDir))
                    Directory.CreateDirectory(saveDir);

                File.WriteAllText(_launchDateFile, ServerLaunchDate.ToString("o"));
                Console.WriteLine("[Sphere51a] First launch - recorded server launch date");
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[Sphere51a] Could not save launch date: {ex.Message}");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// Check if configuration is initialized.
        /// </summary>
        public static bool IsInitialized => _isInitialized;
    }

    /// <summary>
    /// Adapter to make the existing static FactionRepository work with the interface.
    /// This wraps the PostgreSQL-specific implementation.
    /// </summary>
    internal class PostgresFactionRepositoryAdapter : IFactionRepository
    {
        public string ProviderName => "PostgreSQL";

        public bool Initialize()
        {
            return Factions.FactionRepository.VerifySchema();
        }

        public bool TestConnection()
        {
            return PostgresConnection.TestConnection();
        }

        public Factions.GuildFactionInfo GetGuildFaction(Serial guildSerial)
        {
            return Factions.FactionRepository.GetGuildFaction(guildSerial);
        }

        public System.Collections.Generic.List<Serial> GetFactionGuilds(int factionId)
        {
            return Factions.FactionRepository.GetFactionGuilds(factionId);
        }

        public System.Collections.Generic.Dictionary<int, int> GetFactionStatistics()
        {
            return Factions.FactionRepository.GetFactionStatistics();
        }

        public int GetTotalGuildCount()
        {
            return Factions.FactionRepository.GetTotalGuildCount();
        }

        public bool SetGuildFaction(Serial guildSerial, int factionId)
        {
            return Factions.FactionRepository.SetGuildFaction(guildSerial, factionId);
        }

        public bool RemoveGuildFaction(Serial guildSerial)
        {
            return Factions.FactionRepository.RemoveGuildFaction(guildSerial);
        }
    }
}
