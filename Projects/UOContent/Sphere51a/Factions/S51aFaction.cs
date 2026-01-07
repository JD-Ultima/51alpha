// =====================================================
// 51ALPHA FACTION SYSTEM - FACTION DEFINITIONS
// =====================================================
// File: S51aFaction.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: PHASE1_VERIFICATION_REPORT_v3.md (Q1: Faction System, Lines 40-61)
// =====================================================

using System;

namespace Server.Sphere51a.Factions
{
    /// <summary>
    /// Represents a faction with all its properties.
    /// Three static factions are defined per Phase 1 specification.
    /// </summary>
    public class S51aFaction
    {
        /// <summary>
        /// Database faction ID (1, 2, 3)
        /// </summary>
        public int FactionId { get; set; }

        /// <summary>
        /// Faction type enum
        /// </summary>
        public FactionType Type { get; set; }

        /// <summary>
        /// Full faction name (e.g., "The Golden Shield")
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Guild name hue (displayed to all players)
        /// Phase 1 values: 2721 (GoldenShield), 2784 (Bridgefolk), 2602 (LycaeumOrder)
        /// </summary>
        public int Hue { get; set; }

        /// <summary>
        /// Home city where guild members receive 10% NPC vendor discount
        /// Phase 1 values: Trinsic, Vesper, Moonglow
        /// </summary>
        public string HomeCity { get; set; }

        // =====================================================
        // STATIC FACTION DEFINITIONS (LOCKED PER PHASE 1)
        // =====================================================

        /// <summary>
        /// The Golden Shield faction
        /// Home City: Trinsic
        /// Hue: 2721 (golden/yellow)
        /// </summary>
        public static readonly S51aFaction GoldenShield = new S51aFaction
        {
            FactionId = 1,
            Type = FactionType.GoldenShield,
            Name = "The Golden Shield",
            Hue = 2721,
            HomeCity = "Trinsic"
        };

        /// <summary>
        /// The Bridgefolk faction
        /// Home City: Vesper
        /// Hue: 2784 (blue/teal)
        /// </summary>
        public static readonly S51aFaction Bridgefolk = new S51aFaction
        {
            FactionId = 2,
            Type = FactionType.Bridgefolk,
            Name = "The Bridgefolk",
            Hue = 2784,
            HomeCity = "Vesper"
        };

        /// <summary>
        /// The Lycaeum Order faction
        /// Home City: Moonglow
        /// Hue: 2602 (purple/violet)
        /// </summary>
        public static readonly S51aFaction LycaeumOrder = new S51aFaction
        {
            FactionId = 3,
            Type = FactionType.LycaeumOrder,
            Name = "The Lycaeum Order",
            Hue = 2602,
            HomeCity = "Moonglow"
        };

        /// <summary>
        /// Array of all factions (for iteration)
        /// </summary>
        public static S51aFaction[] AllFactions => new[] { GoldenShield, Bridgefolk, LycaeumOrder };

        // =====================================================
        // LOOKUP METHODS
        // =====================================================

        /// <summary>
        /// Get faction by database ID (1, 2, 3)
        /// </summary>
        /// <param name="id">Faction ID from database</param>
        /// <returns>Faction instance or null if invalid ID</returns>
        public static S51aFaction GetById(int id)
        {
            return id switch
            {
                1 => GoldenShield,
                2 => Bridgefolk,
                3 => LycaeumOrder,
                _ => null
            };
        }

        /// <summary>
        /// Get faction by FactionType enum
        /// </summary>
        /// <param name="type">Faction type</param>
        /// <returns>Faction instance or null if None</returns>
        public static S51aFaction GetByType(FactionType type)
        {
            return type switch
            {
                FactionType.GoldenShield => GoldenShield,
                FactionType.Bridgefolk => Bridgefolk,
                FactionType.LycaeumOrder => LycaeumOrder,
                _ => null
            };
        }

        /// <summary>
        /// Get faction by name (case-insensitive)
        /// </summary>
        /// <param name="name">Faction name (e.g., "GoldenShield" or "The Golden Shield")</param>
        /// <returns>Faction instance or null if not found</returns>
        public static S51aFaction GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            name = name.Trim();

            // Support both enum name and full name
            if (name.Equals("GoldenShield", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("The Golden Shield", StringComparison.OrdinalIgnoreCase))
                return GoldenShield;

            if (name.Equals("Bridgefolk", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("The Bridgefolk", StringComparison.OrdinalIgnoreCase))
                return Bridgefolk;

            if (name.Equals("LycaeumOrder", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("The Lycaeum Order", StringComparison.OrdinalIgnoreCase))
                return LycaeumOrder;

            return null;
        }

        /// <summary>
        /// Get faction by home city (case-insensitive)
        /// </summary>
        /// <param name="cityName">City name (Trinsic, Vesper, Moonglow)</param>
        /// <returns>Faction instance or null if not a faction home city</returns>
        public static S51aFaction GetByHomeCity(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                return null;

            cityName = cityName.Trim();

            if (cityName.Equals("Trinsic", StringComparison.OrdinalIgnoreCase))
                return GoldenShield;

            if (cityName.Equals("Vesper", StringComparison.OrdinalIgnoreCase))
                return Bridgefolk;

            if (cityName.Equals("Moonglow", StringComparison.OrdinalIgnoreCase))
                return LycaeumOrder;

            return null;
        }

        // =====================================================
        // EQUALITY & STRING REPRESENTATION
        // =====================================================

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is S51aFaction other)
                return FactionId == other.FactionId;

            return false;
        }

        public override int GetHashCode()
        {
            return FactionId.GetHashCode();
        }
    }
}
