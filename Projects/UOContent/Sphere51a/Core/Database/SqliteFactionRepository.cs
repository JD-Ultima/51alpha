// =====================================================
// 51ALPHA CORE - SQLITE FACTION REPOSITORY
// =====================================================
// File: SqliteFactionRepository.cs
// Created: 2025-01-13
// Purpose: SQLite implementation for local development (no installation required)
// Note: Use PostgresFactionRepository for production servers
// =====================================================

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using Server.Sphere51a.Factions;

namespace Server.Sphere51a.Core.Database
{
    /// <summary>
    /// SQLite implementation of faction repository.
    /// Perfect for local development - no database server installation needed.
    /// Database is stored as a single file in the Distribution folder.
    /// </summary>
    public class SqliteFactionRepository : IFactionRepository
    {
        private readonly string _connectionString;
        private readonly string _databasePath;
        private static bool _sqliteInitialized = false;

        public string ProviderName => "SQLite";

        /// <summary>
        /// Initialize SQLite native library.
        /// Must be called before any SQLite operations.
        /// </summary>
        private static void EnsureSqliteInitialized()
        {
            if (_sqliteInitialized)
                return;

            try
            {
                // Initialize the SQLite native library
                SQLitePCL.Batteries_V2.Init();
                _sqliteInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Sphere51a] SQLite native library initialization: {ex.Message}");
                // Continue anyway - some setups work without explicit init
                _sqliteInitialized = true;
            }
        }

        /// <summary>
        /// Create SQLite repository with database file in Distribution folder.
        /// </summary>
        /// <param name="databaseFileName">Database filename (default: sphere51a.db)</param>
        public SqliteFactionRepository(string databaseFileName = "sphere51a.db")
        {
            // Initialize SQLite native library
            EnsureSqliteInitialized();

            // Store database in Saves folder (BaseDirectory is already the Distribution folder)
            _databasePath = Path.Combine(Server.Core.BaseDirectory, "Saves", databaseFileName);
            _connectionString = $"Data Source={_databasePath}";
        }

        // =====================================================
        // INITIALIZATION
        // =====================================================

        public bool Initialize()
        {
            try
            {
                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[Sphere51a] SQLite database: {_databasePath}");
                Utility.PopColor();

                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                // Create tables if they don't exist
                CreateSchema(conn);

                // Seed faction definitions if empty
                SeedFactions(conn);

                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine("[Sphere51a] SQLite database initialized");
                Utility.PopColor();

                return true;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] SQLite initialization failed: {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        private void CreateSchema(SqliteConnection conn)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                -- Faction definitions (static data)
                CREATE TABLE IF NOT EXISTS s51a_factions (
                    faction_id INTEGER PRIMARY KEY,
                    faction_name TEXT NOT NULL,
                    faction_hue INTEGER NOT NULL,
                    home_city TEXT NOT NULL
                );

                -- Guild faction assignments
                CREATE TABLE IF NOT EXISTS s51a_guild_factions (
                    guild_serial INTEGER PRIMARY KEY,
                    faction_id INTEGER NOT NULL,
                    joined_at TEXT NOT NULL,
                    last_change_at TEXT,
                    can_change_after TEXT,
                    FOREIGN KEY (faction_id) REFERENCES s51a_factions(faction_id)
                );

                -- Index for faction lookups
                CREATE INDEX IF NOT EXISTS idx_guild_factions_faction
                ON s51a_guild_factions(faction_id);
            ";
            cmd.ExecuteNonQuery();
        }

        private void SeedFactions(SqliteConnection conn)
        {
            // Check if factions already seeded
            using var checkCmd = conn.CreateCommand();
            checkCmd.CommandText = "SELECT COUNT(*) FROM s51a_factions";
            var count = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (count > 0)
                return; // Already seeded

            // Seed the three factions
            using var seedCmd = conn.CreateCommand();
            seedCmd.CommandText = @"
                INSERT INTO s51a_factions (faction_id, faction_name, faction_hue, home_city) VALUES
                (1, 'The Golden Shield', 2721, 'Trinsic'),
                (2, 'The Bridgefolk', 2784, 'Vesper'),
                (3, 'The Lycaeum Order', 2602, 'Moonglow');
            ";
            seedCmd.ExecuteNonQuery();

            Console.WriteLine("[Sphere51a] Faction definitions seeded");
        }

        public bool TestConnection()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteScalar();

                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine("[Sphere51a] SQLite connection test: SUCCESS");
                Utility.PopColor();

                return true;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] SQLite connection test: FAILED - {ex.Message}");
                Utility.PopColor();
                return false;
            }
        }

        // =====================================================
        // READ OPERATIONS
        // =====================================================

        public GuildFactionInfo GetGuildFaction(Serial guildSerial)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT gf.faction_id, gf.joined_at, gf.last_change_at, gf.can_change_after,
                           f.faction_name, f.faction_hue, f.home_city
                    FROM s51a_guild_factions gf
                    JOIN s51a_factions f ON gf.faction_id = f.faction_id
                    WHERE gf.guild_serial = @serial
                ";
                cmd.Parameters.AddWithValue("@serial", (long)guildSerial.Value);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                    return null;

                return new GuildFactionInfo
                {
                    GuildSerial = guildSerial,
                    Faction = S51aFaction.GetById(reader.GetInt32(0)),
                    JoinedAt = DateTime.Parse(reader.GetString(1)),
                    LastChangeAt = reader.IsDBNull(2) ? null : DateTime.Parse(reader.GetString(2)),
                    CanChangeAfter = reader.IsDBNull(3) ? null : DateTime.Parse(reader.GetString(3))
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

        public List<Serial> GetFactionGuilds(int factionId)
        {
            var guilds = new List<Serial>();

            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT guild_serial FROM s51a_guild_factions WHERE faction_id = @factionId";
                cmd.Parameters.AddWithValue("@factionId", factionId);

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

        public Dictionary<int, int> GetFactionStatistics()
        {
            var stats = new Dictionary<int, int>
            {
                { 1, 0 }, // GoldenShield
                { 2, 0 }, // Bridgefolk
                { 3, 0 }  // LycaeumOrder
            };

            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT faction_id, COUNT(*)
                    FROM s51a_guild_factions
                    GROUP BY faction_id
                ";

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

        public int GetTotalGuildCount()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM s51a_guild_factions";

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Failed to get total guild count: {ex.Message}");
                Utility.PopColor();
                return 0;
            }
        }

        // =====================================================
        // WRITE OPERATIONS
        // =====================================================

        public bool SetGuildFaction(Serial guildSerial, int factionId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                var now = DateTime.UtcNow;
                var canChangeAfter = now.AddDays(7);

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO s51a_guild_factions
                        (guild_serial, faction_id, joined_at, last_change_at, can_change_after)
                    VALUES
                        (@serial, @factionId, @joinedAt, @lastChangeAt, @canChangeAfter)
                    ON CONFLICT(guild_serial) DO UPDATE SET
                        faction_id = @factionId,
                        last_change_at = @lastChangeAt,
                        can_change_after = @canChangeAfter
                ";
                cmd.Parameters.AddWithValue("@serial", (long)guildSerial.Value);
                cmd.Parameters.AddWithValue("@factionId", factionId);
                cmd.Parameters.AddWithValue("@joinedAt", now.ToString("o"));
                cmd.Parameters.AddWithValue("@lastChangeAt", now.ToString("o"));
                cmd.Parameters.AddWithValue("@canChangeAfter", canChangeAfter.ToString("o"));

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

        public bool RemoveGuildFaction(Serial guildSerial)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM s51a_guild_factions WHERE guild_serial = @serial";
                cmd.Parameters.AddWithValue("@serial", (long)guildSerial.Value);

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
    }
}
