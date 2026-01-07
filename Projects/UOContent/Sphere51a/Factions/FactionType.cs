// =====================================================
// 51ALPHA FACTION SYSTEM - FACTION TYPE ENUM
// =====================================================
// File: FactionType.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: PHASE1_VERIFICATION_REPORT_v3.md (Q1: Faction System, Lines 40-61)
// =====================================================

namespace Server.Sphere51a.Factions
{
    /// <summary>
    /// Represents the three factions in the 51alpha server.
    /// All factions have equal power at launch per Phase 1 specification.
    /// </summary>
    public enum FactionType
    {
        /// <summary>
        /// No faction assigned (default state for new guilds)
        /// </summary>
        None = 0,

        /// <summary>
        /// The Golden Shield faction
        /// Home City: Trinsic
        /// Hue: 2721
        /// </summary>
        GoldenShield = 1,

        /// <summary>
        /// The Bridgefolk faction
        /// Home City: Vesper
        /// Hue: 2784
        /// </summary>
        Bridgefolk = 2,

        /// <summary>
        /// The Lycaeum Order faction
        /// Home City: Moonglow
        /// Hue: 2602
        /// </summary>
        LycaeumOrder = 3
    }
}
