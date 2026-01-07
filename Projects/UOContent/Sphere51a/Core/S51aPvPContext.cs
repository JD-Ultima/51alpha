// =====================================================
// File: S51aPvPContext.cs
// Created: 2025-12-15
// Updated: 2025-12-22
// Phase: Phase 1 - Combat System (Complete)
// Documentation: Master_Architecture.md Section 3.2
// =====================================================

using System;

namespace Server.Sphere51a.Core
{
    /// <summary>
    /// PvP context detection for Phase 4 talisman system.
    /// Determines if a mobile is currently engaged in PvP based on recent player damage.
    ///
    /// Updated 2025-12-22: Now tracks BOTH incoming and outgoing damage for complete detection.
    /// </summary>
    public static class S51aPvPContext
    {
        /// <summary>
        /// PvP context timeout - 2 minutes after last player damage
        /// </summary>
        private static readonly TimeSpan PvPContextDuration = TimeSpan.FromMinutes(2);

        /// <summary>
        /// Checks if mobile is currently in PvP context.
        /// Defined as: damaged by OR dealt damage to another player within last 2 minutes.
        /// </summary>
        /// <param name="m">Mobile to check</param>
        /// <returns>True if mobile is in PvP context, false otherwise</returns>
        public static bool IsInPvP(Mobile m)
        {
            if (m == null || !m.Player)
                return false;

            var now = DateTime.UtcNow;

            // Check 1: Has mobile been damaged by a player recently? (incoming damage)
            foreach (var entry in m.DamageEntries)
            {
                if (entry.Damager != null &&
                    entry.Damager.Player &&
                    entry.LastDamage > now - PvPContextDuration)
                {
                    return true;
                }
            }

            // Check 2: Has mobile dealt damage to a player recently? (outgoing damage)
            // Uses S51aDamageTracker for outgoing damage tracking
            var lastDamageDealt = S51aDamageTracker.GetLastDamageDealtToPlayer(m);
            if (lastDamageDealt > now - PvPContextDuration)
            {
                return true;
            }

            return false;
        }
    }
}
