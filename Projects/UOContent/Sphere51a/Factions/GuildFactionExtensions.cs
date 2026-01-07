// =====================================================
// 51ALPHA FACTION SYSTEM - GUILD EXTENSIONS
// =====================================================
// File: GuildFactionExtensions.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: PHASE1_VERIFICATION_REPORT_v3.md (Q1: Faction System)
// =====================================================

using System;
using Server.Guilds;

namespace Server.Sphere51a.Factions
{
    /// <summary>
    /// Extension methods for ModernUO Guild class.
    /// Provides faction-related functionality without modifying ModernUO defaults.
    /// </summary>
    public static class GuildFactionExtensions
    {
        /// <summary>
        /// Get faction for this guild.
        /// </summary>
        /// <param name="guild">Guild instance</param>
        /// <returns>Faction or null if guild not in any faction</returns>
        public static S51aFaction GetFaction(this Guild guild)
        {
            if (guild == null)
                return null;

            return S51aFactionSystem.GetGuildFaction(guild);
        }

        /// <summary>
        /// Get detailed faction information for this guild (includes cooldown).
        /// </summary>
        /// <param name="guild">Guild instance</param>
        /// <returns>GuildFactionInfo or null</returns>
        public static GuildFactionInfo GetFactionInfo(this Guild guild)
        {
            if (guild == null)
                return null;

            return S51aFactionSystem.GetGuildFactionInfo(guild);
        }

        /// <summary>
        /// Check if guild has faction discount at vendor's city.
        /// Used by vendor pricing logic.
        /// </summary>
        /// <param name="guild">Guild instance</param>
        /// <param name="cityName">City name (e.g., "Trinsic", "Vesper", "Moonglow")</param>
        /// <returns>True if guild gets discount in this city</returns>
        public static bool HasFactionDiscountAt(this Guild guild, string cityName)
        {
            if (guild == null || string.IsNullOrWhiteSpace(cityName))
                return false;

            var faction = guild.GetFaction();
            if (faction == null)
                return false;

            return faction.HomeCity.Equals(cityName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if guild can change faction.
        /// </summary>
        /// <param name="guild">Guild instance</param>
        /// <returns>True if faction change allowed</returns>
        public static bool CanChangeFaction(this Guild guild)
        {
            if (guild == null)
                return false;

            return S51aFactionSystem.CanChangeFaction(guild, out _);
        }

        /// <summary>
        /// Get remaining faction change cooldown.
        /// </summary>
        /// <param name="guild">Guild instance</param>
        /// <returns>Remaining cooldown or null if no cooldown active</returns>
        public static TimeSpan? GetFactionChangeCooldown(this Guild guild)
        {
            if (guild == null)
                return null;

            S51aFactionSystem.CanChangeFaction(guild, out var cooldown);
            return cooldown;
        }
    }
}
