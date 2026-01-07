// =====================================================
// File: S51aDamageTracker.cs
// Created: 2025-12-22
// Phase: Phase 1 - Combat System
// Purpose: Track outgoing damage for PvP context detection
// =====================================================

using System;
using System.Collections.Generic;

namespace Server.Sphere51a.Core
{
    /// <summary>
    /// Tracks outgoing damage from players to complete PvP context detection.
    /// Complements Mobile.DamageEntries (incoming damage tracking).
    ///
    /// Design: Player-only tracking. NPCs/monsters/guards not affected.
    /// </summary>
    public static class S51aDamageTracker
    {
        /// <summary>
        /// Stores last damage dealt timestamp per player serial.
        /// Key: Player serial who dealt damage
        /// Value: Timestamp of last damage to another player
        /// </summary>
        private static readonly Dictionary<Serial, DateTime> _lastDamageDealtToPlayer = new();

        /// <summary>
        /// Lock for thread-safe dictionary access
        /// </summary>
        private static readonly object _lock = new();

        /// <summary>
        /// Records when a player deals damage to another player.
        /// Called from Mobile.Damage() for player-to-player combat only.
        /// </summary>
        /// <param name="attacker">Player dealing damage</param>
        /// <param name="victim">Player receiving damage</param>
        /// <param name="amount">Damage amount (not currently used but may be useful for future features)</param>
        public static void RecordPlayerDamage(Mobile attacker, Mobile victim, int amount)
        {
            // Validate: Both must be players
            if (attacker == null || !attacker.Player || victim == null || !victim.Player)
                return;

            // Validate: Must be different players
            if (attacker == victim)
                return;

            lock (_lock)
            {
                _lastDamageDealtToPlayer[attacker.Serial] = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Gets the last time this mobile dealt damage to another player.
        /// Used by S51aPvPContext.IsInPvP() for outgoing damage check.
        /// </summary>
        /// <param name="mobile">Mobile to check</param>
        /// <returns>DateTime of last damage dealt to player, or DateTime.MinValue if never</returns>
        public static DateTime GetLastDamageDealtToPlayer(Mobile mobile)
        {
            if (mobile == null || !mobile.Player)
                return DateTime.MinValue;

            lock (_lock)
            {
                return _lastDamageDealtToPlayer.TryGetValue(mobile.Serial, out var timestamp)
                    ? timestamp
                    : DateTime.MinValue;
            }
        }

        /// <summary>
        /// Clears damage tracking for a mobile (called on logout/deletion).
        /// </summary>
        /// <param name="mobile">Mobile to clear tracking for</param>
        public static void ClearDamageTracking(Mobile mobile)
        {
            if (mobile == null)
                return;

            lock (_lock)
            {
                _lastDamageDealtToPlayer.Remove(mobile.Serial);
            }
        }

        /// <summary>
        /// Periodic cleanup of old entries (call from timer if needed).
        /// Removes entries older than 2 minutes (PvP context duration).
        /// </summary>
        public static void CleanupOldEntries()
        {
            var cutoff = DateTime.UtcNow - TimeSpan.FromMinutes(2);
            var toRemove = new List<Serial>();

            lock (_lock)
            {
                foreach (var kvp in _lastDamageDealtToPlayer)
                {
                    if (kvp.Value < cutoff)
                        toRemove.Add(kvp.Key);
                }

                foreach (var serial in toRemove)
                {
                    _lastDamageDealtToPlayer.Remove(serial);
                }
            }
        }
    }
}
