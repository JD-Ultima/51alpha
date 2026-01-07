// =====================================================
// 51ALPHA FACTION SYSTEM - GUILD FACTION INFO
// =====================================================
// File: GuildFactionInfo.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: PHASE1_VERIFICATION_REPORT_v3.md (Q1: Faction System, Lines 40-61)
// =====================================================

using System;

namespace Server.Sphere51a.Factions
{
    /// <summary>
    /// Represents a guild's faction assignment with cooldown tracking.
    /// Implements the 7-day cooldown rule per Phase 1 specification.
    /// </summary>
    public class GuildFactionInfo
    {
        /// <summary>
        /// ModernUO Guild.Serial (unique guild identifier)
        /// </summary>
        public Serial GuildSerial { get; set; }

        /// <summary>
        /// The faction this guild belongs to
        /// </summary>
        public S51aFaction Faction { get; set; }

        /// <summary>
        /// Timestamp when this guild first joined ANY faction
        /// (Never changes after first assignment)
        /// </summary>
        public DateTime JoinedAt { get; set; }

        /// <summary>
        /// Timestamp of the last faction change
        /// (Updated every time guild changes faction)
        /// </summary>
        public DateTime? LastChangeAt { get; set; }

        /// <summary>
        /// Pre-calculated timestamp when guild can change faction again
        /// (LastChangeAt + 7 days)
        /// Null if no cooldown active
        /// </summary>
        public DateTime? CanChangeAfter { get; set; }

        // =====================================================
        // COOLDOWN VALIDATION METHODS
        // =====================================================

        /// <summary>
        /// Check if guild can change faction right now.
        /// Validates both Week 1 lockout and 7-day cooldown.
        /// </summary>
        /// <returns>True if faction change allowed, false otherwise</returns>
        public bool CanChangeFaction()
        {
            // Check Week 1 server-wide lockout
            // (Handled by S51aConfig.IsFactionChangeLocked)
            if (Server.Sphere51a.Core.S51aConfig.IsFactionChangeLocked)
                return false;

            // Check 7-day cooldown (per-guild)
            if (CanChangeAfter.HasValue && DateTime.UtcNow < CanChangeAfter.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Get remaining cooldown duration.
        /// </summary>
        /// <returns>TimeSpan remaining or null if no cooldown active</returns>
        public TimeSpan? GetRemainingCooldown()
        {
            if (!CanChangeAfter.HasValue || DateTime.UtcNow >= CanChangeAfter.Value)
                return null;

            return CanChangeAfter.Value - DateTime.UtcNow;
        }

        /// <summary>
        /// Get human-readable cooldown status message.
        /// </summary>
        /// <returns>Status message (e.g., "5 days, 12 hours remaining" or "No cooldown")</returns>
        public string GetCooldownStatusMessage()
        {
            var remaining = GetRemainingCooldown();

            if (!remaining.HasValue)
                return "No cooldown active - faction change allowed";

            int days = remaining.Value.Days;
            int hours = remaining.Value.Hours;
            int minutes = remaining.Value.Minutes;

            if (days > 0)
                return $"Cooldown: {days} day{(days != 1 ? "s" : "")}, {hours} hour{(hours != 1 ? "s" : "")} remaining";

            if (hours > 0)
                return $"Cooldown: {hours} hour{(hours != 1 ? "s" : "")}, {minutes} minute{(minutes != 1 ? "s" : "")} remaining";

            return $"Cooldown: {minutes} minute{(minutes != 1 ? "s" : "")} remaining";
        }

        // =====================================================
        // STRING REPRESENTATION
        // =====================================================

        public override string ToString()
        {
            return $"Guild {GuildSerial} -> {Faction?.Name ?? "No Faction"} (Joined: {JoinedAt:yyyy-MM-dd})";
        }
    }
}
