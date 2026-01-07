using System;
using System.Collections.Generic;
using Server.Engines.BuffIcons;
using Server.Mobiles;

namespace Server.Spells
{
    /// <summary>
    /// Single source of truth for global stat accumulator system.
    /// Manages Bless/Curse interactions with a numeric accumulator (range: [-10, +10]).
    /// BuffIcons are visual only - the accumulator determines actual stat offsets.
    /// </summary>
    public static class GlobalStatController
    {
        private static readonly Dictionary<Mobile, GlobalStatData> _data = new();

        private class GlobalStatData
        {
            public int Offset; // Range: [-10, +10]
            public Timer BlessTimer;
            public Timer CurseTimer;
        }

        /// <summary>
        /// Apply global modifier to target's stat accumulator.
        /// </summary>
        /// <param name="caster">The caster of the spell</param>
        /// <param name="target">The target mobile</param>
        /// <param name="delta">+10 for Bless, -10 for Curse</param>
        /// <param name="duration">Duration of the effect</param>
        /// <returns>True if accumulator changed, false if already at cap</returns>
        public static bool ApplyGlobalModifier(Mobile caster, Mobile target, int delta, TimeSpan duration)
        {
            if (duration == TimeSpan.Zero)
            {
                return false;
            }

            // Get or create data
            if (!_data.TryGetValue(target, out var data))
            {
                data = new GlobalStatData { Offset = 0 };
                _data[target] = data;
            }

            // Calculate new offset with clamping
            var newOffset = Math.Clamp(data.Offset + delta, -10, 10);

            // If no change (already at cap in this direction), return false
            if (newOffset == data.Offset)
            {
                // Refresh timer if same direction
                if (delta > 0 && data.BlessTimer != null)
                {
                    data.BlessTimer.Stop();
                    data.BlessTimer = Timer.DelayCall(duration, () => OnTimerExpired(target, delta));
                    RefreshBlessBuffIcon(target, duration);
                    return true;
                }
                else if (delta < 0 && data.CurseTimer != null)
                {
                    data.CurseTimer.Stop();
                    data.CurseTimer = Timer.DelayCall(duration, () => OnTimerExpired(target, delta));
                    RefreshCurseBuffIcon(target, duration);
                    return true;
                }

                return false;
            }

            // Update accumulator
            var oldOffset = data.Offset;
            data.Offset = newOffset;

            // Apply the global stat modifiers based on the accumulator value
            ApplyGlobalStatMods(target, newOffset, duration);

            // Add/refresh BuffIcon and timer
            if (delta > 0) // Bless
            {
                data.BlessTimer?.Stop();
                data.BlessTimer = Timer.DelayCall(duration, () => OnTimerExpired(target, delta));
                AddBlessBuffIcon(target, caster, duration);
            }
            else // Curse
            {
                data.CurseTimer?.Stop();
                data.CurseTimer = Timer.DelayCall(duration, () => OnTimerExpired(target, delta));
                AddCurseBuffIcon(target, caster, duration);
            }

            return true;
        }

        /// <summary>
        /// Apply global StatMods based on accumulator value.
        /// Global StatMods stack with individual StatMods - they cancel each other out naturally.
        /// Example: Strength (+10) + Curse (-10) = net 0 effect, both BuffIcons remain visible.
        /// </summary>
        private static void ApplyGlobalStatMods(Mobile target, int offset, TimeSpan duration)
        {
            // Remove old global StatMods
            target.RemoveStatMod("[Magic] Str Global");
            target.RemoveStatMod("[Magic] Dex Global");
            target.RemoveStatMod("[Magic] Int Global");

            // Apply new global StatMods if offset is non-zero
            if (offset != 0)
            {
                // Simply apply global offset to all stats - let them stack naturally with individual effects
                target.AddStatMod(new StatMod(StatType.Str, "[Magic] Str Global", offset, duration));
                target.AddStatMod(new StatMod(StatType.Dex, "[Magic] Dex Global", offset, duration));
                target.AddStatMod(new StatMod(StatType.Int, "[Magic] Int Global", offset, duration));
            }
        }

        /// <summary>
        /// Check if individual buffs (Str/Agi/Cun) can be applied.
        /// Cap is now handled in GetStatOffset() - no blocking needed.
        /// </summary>
        public static bool CanApplyIndividualBuff(Mobile caster, Mobile target)
        {
            // Cap is now handled in GetStatOffset() - allow all spells to apply
            return true;
        }

        /// <summary>
        /// Check if individual curses (Wea/Clu/Fee) can be applied.
        /// Cap is now handled in GetStatOffset() - no blocking needed.
        /// </summary>
        public static bool CanApplyIndividualCurse(Mobile caster, Mobile target)
        {
            // Cap is now handled in GetStatOffset() - allow all spells to apply
            return true;
        }

        /// <summary>
        /// Get current global stat offset for stat calculation.
        /// </summary>
        public static int GetGlobalOffset(Mobile target)
        {
            if (!_data.TryGetValue(target, out var data))
            {
                return 0;
            }

            return data.Offset;
        }

        /// <summary>
        /// Check if target is under Curse effect (has negative global offset).
        /// </summary>
        public static bool UnderCurseEffect(Mobile target)
        {
            if (!_data.TryGetValue(target, out var data))
            {
                return false;
            }

            return data.Offset < 0;
        }

        /// <summary>
        /// Clear all stat effects (used by Dispel).
        /// Resets accumulator to 0, stops all timers, removes all BuffIcons and StatMods.
        /// </summary>
        public static void ClearAllStatEffects(Mobile target)
        {
            ClearAllStatEffectsWithResult(target);
        }

        /// <summary>
        /// Clear all stat effects and return true if any effects were removed.
        /// </summary>
        public static bool ClearAllStatEffectsWithResult(Mobile target)
        {
            // Reset accumulator and stop timers
            bool hadData = _data.Remove(target, out var data);
            if (hadData)
            {
                data.BlessTimer?.Stop();
                data.CurseTimer?.Stop();
            }

            // Remove all BuffIcons (Bless, Curse, and individual spells)
            if (target is PlayerMobile pm)
            {
                pm.RemoveBuff(BuffIcon.Bless);
                pm.RemoveBuff(BuffIcon.Curse);
                pm.RemoveBuff(BuffIcon.Strength);
                pm.RemoveBuff(BuffIcon.Agility);
                pm.RemoveBuff(BuffIcon.Cunning);
                pm.RemoveBuff(BuffIcon.Weaken);
                pm.RemoveBuff(BuffIcon.Clumsy);
                pm.RemoveBuff(BuffIcon.FeebleMind);
            }

            // Remove all global StatMods (from Bless/Curse)
            target.RemoveStatMod("[Magic] Str Global");
            target.RemoveStatMod("[Magic] Dex Global");
            target.RemoveStatMod("[Magic] Int Global");

            // Remove all individual StatMods
            target.RemoveStatMod("[Magic] Str Buff");
            target.RemoveStatMod("[Magic] Dex Buff");
            target.RemoveStatMod("[Magic] Int Buff");
            target.RemoveStatMod("[Magic] Str Curse");
            target.RemoveStatMod("[Magic] Dex Curse");
            target.RemoveStatMod("[Magic] Int Curse");

            return hadData; // Return true if there was any global effect to remove
        }

        /// <summary>
        /// Timer expired - reduce accumulator by delta and remove corresponding BuffIcon.
        /// </summary>
        private static void OnTimerExpired(Mobile target, int delta)
        {
            if (!_data.TryGetValue(target, out var data))
            {
                return;
            }

            // Reduce accumulator by delta (reverse the original effect)
            var oldOffset = data.Offset;
            data.Offset = Math.Clamp(data.Offset - delta, -10, 10);

            // Update global StatMods to reflect the new offset
            // Need to determine remaining duration - use a long duration since we don't track it separately
            ApplyGlobalStatMods(target, data.Offset, TimeSpan.FromHours(1));

            // Remove corresponding BuffIcon
            if (target is PlayerMobile pm)
            {
                if (delta > 0) // Bless timer expired
                {
                    pm.RemoveBuff(BuffIcon.Bless);
                    data.BlessTimer = null;
                }
                else // Curse timer expired
                {
                    pm.RemoveBuff(BuffIcon.Curse);
                    data.CurseTimer = null;
                }
            }

            // Clean up if no effects remain
            if (data.Offset == 0 && data.BlessTimer == null && data.CurseTimer == null)
            {
                _data.Remove(target);
            }
        }

        /// <summary>
        /// Add Bless BuffIcon to target.
        /// </summary>
        private static void AddBlessBuffIcon(Mobile target, Mobile caster, TimeSpan duration)
        {
            if (target is PlayerMobile pm)
            {
                var percentage = (int)(SpellHelper.GetOffsetScalar(caster, target, false) * 100);
                var args = $"{percentage}\t{percentage}\t{percentage}";
                pm.AddBuff(new BuffInfo(BuffIcon.Bless, 1075847, 1075848, duration, args));
            }
        }

        /// <summary>
        /// Refresh Bless BuffIcon timer.
        /// </summary>
        private static void RefreshBlessBuffIcon(Mobile target, TimeSpan duration)
        {
            if (target is PlayerMobile pm)
            {
                pm.RemoveBuff(BuffIcon.Bless);
                var percentage = 10; // Fixed magnitude for refreshes
                var args = $"{percentage}\t{percentage}\t{percentage}";
                pm.AddBuff(new BuffInfo(BuffIcon.Bless, 1075847, 1075848, duration, args));
            }
        }

        /// <summary>
        /// Add Curse BuffIcon to target.
        /// </summary>
        private static void AddCurseBuffIcon(Mobile target, Mobile caster, TimeSpan duration)
        {
            if (target is PlayerMobile pm)
            {
                var percentage = (int)(SpellHelper.GetOffsetScalar(caster, target, true) * 100);
                var args = $"{percentage}\t{percentage}\t{percentage}\t{10}\t{10}\t{10}\t{10}";
                pm.AddBuff(new BuffInfo(BuffIcon.Curse, 1075835, 1075836, duration, args));
            }
        }

        /// <summary>
        /// Refresh Curse BuffIcon timer.
        /// </summary>
        private static void RefreshCurseBuffIcon(Mobile target, TimeSpan duration)
        {
            if (target is PlayerMobile pm)
            {
                pm.RemoveBuff(BuffIcon.Curse);
                var percentage = 10; // Fixed magnitude for refreshes
                var args = $"{percentage}\t{percentage}\t{percentage}\t{10}\t{10}\t{10}\t{10}";
                pm.AddBuff(new BuffInfo(BuffIcon.Curse, 1075835, 1075836, duration, args));
            }
        }
    }
}
