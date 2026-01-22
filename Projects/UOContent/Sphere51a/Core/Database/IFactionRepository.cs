// =====================================================
// 51ALPHA CORE - FACTION REPOSITORY INTERFACE
// =====================================================
// File: IFactionRepository.cs
// Created: 2025-01-13
// Purpose: Database abstraction layer to support SQLite (dev) and PostgreSQL (prod)
// =====================================================

using System;
using System.Collections.Generic;
using Server.Sphere51a.Factions;

namespace Server.Sphere51a.Core.Database
{
    /// <summary>
    /// Interface for faction data persistence.
    /// Implementations: SqliteFactionRepository (dev), PostgresFactionRepository (prod)
    /// </summary>
    public interface IFactionRepository
    {
        // =====================================================
        // READ OPERATIONS
        // =====================================================

        /// <summary>
        /// Get guild's current faction from database.
        /// </summary>
        /// <param name="guildSerial">ModernUO Guild.Serial</param>
        /// <returns>GuildFactionInfo or null if guild not in any faction</returns>
        GuildFactionInfo GetGuildFaction(Serial guildSerial);

        /// <summary>
        /// Get all guilds in a specific faction.
        /// </summary>
        /// <param name="factionId">Faction ID (1, 2, 3)</param>
        /// <returns>List of guild serials</returns>
        List<Serial> GetFactionGuilds(int factionId);

        /// <summary>
        /// Get faction statistics (guild count per faction).
        /// </summary>
        /// <returns>Dictionary: FactionId -> Guild Count</returns>
        Dictionary<int, int> GetFactionStatistics();

        /// <summary>
        /// Get total count of guilds in factions.
        /// </summary>
        /// <returns>Total guild count</returns>
        int GetTotalGuildCount();

        // =====================================================
        // WRITE OPERATIONS
        // =====================================================

        /// <summary>
        /// Assign guild to faction (INSERT or UPDATE).
        /// Sets cooldown to NOW + 7 days.
        /// </summary>
        /// <param name="guildSerial">ModernUO Guild.Serial</param>
        /// <param name="factionId">Faction ID (1, 2, 3)</param>
        /// <returns>True if successful, false otherwise</returns>
        bool SetGuildFaction(Serial guildSerial, int factionId);

        /// <summary>
        /// Remove guild from faction (DELETE).
        /// </summary>
        /// <param name="guildSerial">ModernUO Guild.Serial</param>
        /// <returns>True if successful, false otherwise</returns>
        bool RemoveGuildFaction(Serial guildSerial);

        // =====================================================
        // UTILITY OPERATIONS
        // =====================================================

        /// <summary>
        /// Initialize the database (create tables if needed).
        /// </summary>
        /// <returns>True if successful</returns>
        bool Initialize();

        /// <summary>
        /// Test database connectivity.
        /// </summary>
        /// <returns>True if connection successful</returns>
        bool TestConnection();

        /// <summary>
        /// Get the name of this database provider (for logging).
        /// </summary>
        string ProviderName { get; }
    }
}
