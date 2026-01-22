// =====================================================
// 51ALPHA FACTION SYSTEM - CENTRAL MANAGER
// =====================================================
// File: S51aFactionSystem.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: PHASE1_VERIFICATION_REPORT_v3.md (Q1: Faction System)
//                PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q5: Strong Consistency)
// =====================================================

using System;
using System.Collections.Generic;
using Server.Guilds;

namespace Server.Sphere51a.Factions
{
    /// <summary>
    /// Central manager for faction system with in-memory caching.
    /// Provides write-through caching to PostgreSQL for strong consistency.
    /// All faction operations must go through this class.
    /// </summary>
    public static class S51aFactionSystem
    {
        // In-memory cache: GuildSerial -> GuildFactionInfo
        // Populated on server startup from PostgreSQL
        private static readonly Dictionary<Serial, GuildFactionInfo> _cache = new Dictionary<Serial, GuildFactionInfo>();

        // Lock for thread-safe cache operations
        private static readonly object _cacheLock = new object();

        // Initialization flag
        private static bool _isInitialized = false;

        // =====================================================
        // INITIALIZATION
        // =====================================================

        /// <summary>
        /// Initialize faction system.
        /// Loads all guild factions from PostgreSQL into memory cache.
        /// Called during WorldLoad event.
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                Console.WriteLine("[Sphere51a] Faction system already initialized - skipping");
                return;
            }

            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine("=== Sphere51a Faction System Initialization ===");
            Utility.PopColor();

            LoadAllFactions();

            _isInitialized = true;

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[Sphere51a] Faction system initialized - {_cache.Count} guild(s) loaded");
            Utility.PopColor();

            // Print faction statistics
            PrintFactionStatistics();
        }

        /// <summary>
        /// Load all guild factions from database into memory cache.
        /// Uses the configured database provider (SQLite or PostgreSQL).
        /// </summary>
        private static void LoadAllFactions()
        {
            var repo = Server.Sphere51a.Core.S51aConfig.FactionRepository;
            if (repo == null)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine("[Sphere51a] WARNING: No database repository configured - factions will not persist");
                Utility.PopColor();
                return;
            }

            lock (_cacheLock)
            {
                _cache.Clear();

                Console.WriteLine($"[Sphere51a] Loading guild factions from {repo.ProviderName}...");

                foreach (var faction in S51aFaction.AllFactions)
                {
                    var guildSerials = repo.GetFactionGuilds(faction.FactionId);

                    foreach (var serial in guildSerials)
                    {
                        var info = repo.GetGuildFaction(serial);
                        if (info != null)
                        {
                            _cache[serial] = info;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Print faction statistics to console.
        /// </summary>
        private static void PrintFactionStatistics()
        {
            var repo = Server.Sphere51a.Core.S51aConfig.FactionRepository;
            if (repo == null) return;

            var stats = repo.GetFactionStatistics();

            Console.WriteLine("[Sphere51a] Faction Statistics:");
            Console.WriteLine($"  - The Golden Shield: {stats[1]} guild(s)");
            Console.WriteLine($"  - The Bridgefolk: {stats[2]} guild(s)");
            Console.WriteLine($"  - The Lycaeum Order: {stats[3]} guild(s)");
        }

        // =====================================================
        // READ OPERATIONS (Cache)
        // =====================================================

        /// <summary>
        /// Get guild's faction (from cache).
        /// Fast O(1) lookup.
        /// </summary>
        /// <param name="guild">Guild instance</param>
        /// <returns>Faction or null if guild not in any faction</returns>
        public static S51aFaction GetGuildFaction(Guild guild)
        {
            if (guild == null)
                return null;

            lock (_cacheLock)
            {
                if (_cache.TryGetValue(guild.Serial, out var info))
                    return info.Faction;
            }

            return null;
        }

        /// <summary>
        /// Get guild faction info (from cache).
        /// Includes cooldown information.
        /// </summary>
        /// <param name="guild">Guild instance</param>
        /// <returns>GuildFactionInfo or null</returns>
        public static GuildFactionInfo GetGuildFactionInfo(Guild guild)
        {
            if (guild == null)
                return null;

            lock (_cacheLock)
            {
                if (_cache.TryGetValue(guild.Serial, out var info))
                    return info;
            }

            return null;
        }

        /// <summary>
        /// Check if guild can change faction.
        /// Validates Week 1 lockout and 7-day cooldown.
        /// </summary>
        /// <param name="guild">Guild instance</param>
        /// <param name="cooldown">Remaining cooldown duration (if any)</param>
        /// <returns>True if faction change allowed</returns>
        public static bool CanChangeFaction(Guild guild, out TimeSpan? cooldown)
        {
            cooldown = null;

            if (guild == null)
                return false;

            // Check Week 1 server-wide lockout
            if (Server.Sphere51a.Core.S51aConfig.IsFactionChangeLocked)
            {
                var remaining = (Server.Sphere51a.Core.S51aConfig.ServerLaunchDate + TimeSpan.FromDays(7)) - DateTime.UtcNow;
                if (remaining.TotalSeconds > 0)
                    cooldown = remaining;

                return false;
            }

            // Check 7-day cooldown (per-guild)
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(guild.Serial, out var info))
                {
                    cooldown = info.GetRemainingCooldown();
                    return !cooldown.HasValue;
                }
            }

            return true; // No faction, can join
        }

        // =====================================================
        // WRITE OPERATIONS (Write-Through to PostgreSQL)
        // =====================================================

        /// <summary>
        /// Join guild to faction.
        /// Write-through: Writes to PostgreSQL immediately, then updates cache.
        /// </summary>
        /// <param name="guild">Guild instance</param>
        /// <param name="factionType">Faction to join</param>
        /// <param name="errorMessage">Error message if operation fails</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool JoinFaction(Guild guild, FactionType factionType, out string errorMessage)
        {
            errorMessage = null;

            if (guild == null)
            {
                errorMessage = "Invalid guild";
                return false;
            }

            if (factionType == FactionType.None)
            {
                errorMessage = "Invalid faction type";
                return false;
            }

            // Get faction definition
            var faction = S51aFaction.GetByType(factionType);
            if (faction == null)
            {
                errorMessage = "Faction not found";
                return false;
            }

            // Check if guild already in a faction
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(guild.Serial, out var existingInfo))
                {
                    // Check cooldown
                    if (!existingInfo.CanChangeFaction())
                    {
                        var remaining = existingInfo.GetRemainingCooldown();
                        if (remaining.HasValue)
                        {
                            int days = remaining.Value.Days;
                            int hours = remaining.Value.Hours;
                            errorMessage = $"Must wait {days} day(s), {hours} hour(s) before changing faction";
                        }
                        else
                        {
                            errorMessage = "Faction changes locked during Week 1";
                        }
                        return false;
                    }

                    // Check if already in this faction
                    if (existingInfo.Faction.FactionId == faction.FactionId)
                    {
                        errorMessage = $"Guild is already in {faction.Name}";
                        return false;
                    }
                }
            }

            // Write to database (source of truth)
            var repo = Server.Sphere51a.Core.S51aConfig.FactionRepository;
            if (repo == null)
            {
                errorMessage = "Database not configured";
                return false;
            }

            if (!repo.SetGuildFaction(guild.Serial, faction.FactionId))
            {
                errorMessage = "Database error - faction assignment failed";
                return false;
            }

            // Reload from database to get accurate timestamps
            var newInfo = repo.GetGuildFaction(guild.Serial);
            if (newInfo == null)
            {
                errorMessage = "Database error - unable to verify faction assignment";
                return false;
            }

            // Update in-memory cache
            lock (_cacheLock)
            {
                _cache[guild.Serial] = newInfo;
            }

            // Notify guild members
            NotifyGuildMembers(guild, $"Your guild has joined {faction.Name}!");

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[Sphere51a] Guild {guild.Name} ({guild.Serial}) joined {faction.Name}");
            Utility.PopColor();

            return true;
        }

        /// <summary>
        /// Leave faction.
        /// Write-through: Deletes from PostgreSQL immediately, then updates cache.
        /// </summary>
        /// <param name="guild">Guild instance</param>
        /// <param name="errorMessage">Error message if operation fails</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool LeaveFaction(Guild guild, out string errorMessage)
        {
            errorMessage = null;

            if (guild == null)
            {
                errorMessage = "Invalid guild";
                return false;
            }

            // Check if guild in a faction
            lock (_cacheLock)
            {
                if (!_cache.ContainsKey(guild.Serial))
                {
                    errorMessage = "Guild not in any faction";
                    return false;
                }

                var info = _cache[guild.Serial];

                // Check cooldown
                if (!info.CanChangeFaction())
                {
                    errorMessage = "Cannot leave faction during cooldown";
                    return false;
                }
            }

            // Delete from database
            var repo = Server.Sphere51a.Core.S51aConfig.FactionRepository;
            if (repo == null)
            {
                errorMessage = "Database not configured";
                return false;
            }

            if (!repo.RemoveGuildFaction(guild.Serial))
            {
                errorMessage = "Database error - faction removal failed";
                return false;
            }

            // Remove from cache
            lock (_cacheLock)
            {
                _cache.Remove(guild.Serial);
            }

            // Notify guild members
            NotifyGuildMembers(guild, "Your guild has left its faction.");

            Utility.PushColor(ConsoleColor.Yellow);
            Console.WriteLine($"[Sphere51a] Guild {guild.Name} ({guild.Serial}) left its faction");
            Utility.PopColor();

            return true;
        }

        // =====================================================
        // ADMIN OPERATIONS
        // =====================================================

        /// <summary>
        /// Force reload all factions from database.
        /// Useful for admin commands or debugging.
        /// </summary>
        public static void ReloadAllFactions()
        {
            Utility.PushColor(ConsoleColor.Yellow);
            Console.WriteLine("[Sphere51a] Force reloading all factions from database...");
            Utility.PopColor();

            LoadAllFactions();

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[Sphere51a] Reload complete - {_cache.Count} guild(s) loaded");
            Utility.PopColor();

            PrintFactionStatistics();
        }

        /// <summary>
        /// Get all guilds in a faction.
        /// </summary>
        /// <param name="factionType">Faction type</param>
        /// <returns>List of guild serials</returns>
        public static List<Serial> GetFactionMembers(FactionType factionType)
        {
            var faction = S51aFaction.GetByType(factionType);
            if (faction == null)
                return new List<Serial>();

            var repo = Server.Sphere51a.Core.S51aConfig.FactionRepository;
            if (repo == null)
                return new List<Serial>();

            return repo.GetFactionGuilds(faction.FactionId);
        }

        /// <summary>
        /// Get current faction statistics.
        /// </summary>
        /// <returns>Dictionary: Faction -> Guild Count</returns>
        public static Dictionary<S51aFaction, int> GetStatistics()
        {
            var repo = Server.Sphere51a.Core.S51aConfig.FactionRepository;
            var stats = new Dictionary<S51aFaction, int>();

            if (repo == null)
            {
                foreach (var faction in S51aFaction.AllFactions)
                    stats[faction] = 0;
                return stats;
            }

            var dbStats = repo.GetFactionStatistics();

            foreach (var faction in S51aFaction.AllFactions)
            {
                stats[faction] = dbStats.ContainsKey(faction.FactionId) ? dbStats[faction.FactionId] : 0;
            }

            return stats;
        }

        /// <summary>
        /// Check if faction system is initialized.
        /// </summary>
        public static bool IsInitialized => _isInitialized;

        // =====================================================
        // HELPER METHODS
        // =====================================================

        /// <summary>
        /// Notify all guild members with a message.
        /// </summary>
        private static void NotifyGuildMembers(Guild guild, string message)
        {
            if (guild == null)
                return;

            foreach (var member in guild.Members)
            {
                member?.SendMessage(0x3F, message);
            }
        }
    }
}
