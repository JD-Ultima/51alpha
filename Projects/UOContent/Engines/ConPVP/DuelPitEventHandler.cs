using Server.Mobiles;
using Server.Items;
using ModernUO.CodeGeneratedEvents;
using System;
using System.Collections.Generic;

namespace Server.Engines.ConPVP
{
    public static class DuelPitEventHandler
    {
        public static void Initialize()
        {
            // NOTE: Spell casting restrictions require core file modifications
            // This would need to be added to Spell.cs CheckCast() method
            // For now, spell restrictions are not enforced during countdown
        }

        [OnEvent(nameof(PlayerMobile.PlayerDeathEvent))]
        public static void OnPlayerDeath(PlayerMobile m)
        {
            // Check if the player is in a duel pit session
            if (DuelPitController.IsInDuelSession(m))
            {
                // Find the session and handle death
                var session = DuelPitController.GetActiveSession(m);
                session?.OnPlayerDeath(m, m.LastKiller);
            }
        }

        [OnEvent(nameof(PlayerMobile.PlayerLoginEvent))]
        public static void OnPlayerLogin(PlayerMobile pm)
        {
            if (pm?.Backpack == null)
                return;

            var marker = pm.Backpack.FindItemByType<DuelPitRecoveryMarker>();
            if (marker != null)
            {
                try
                {
                    RecoverPlayerFromCrash(pm, marker);
                    Console.WriteLine($"[DuelPit] Recovered {pm.Name} from server crash");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DuelPit] Error recovering {pm.Name}: {ex.Message}");
                    pm.SendMessage("There was an error recovering from your interrupted duel. Please contact a GM.");
                }
            }
        }

        private static void RecoverPlayerFromCrash(PlayerMobile pm, DuelPitRecoveryMarker marker)
        {
            // 1. Remove all arena gear
            RemoveAllArenaGear(pm);

            // 2. Restore original equipment
            foreach (var item in marker.SavedEquipment)
            {
                if (item != null && !item.Deleted)
                    pm.EquipItem(item);
            }

            // 3. Restore backpack items
            if (pm.Backpack != null)
            {
                foreach (var item in marker.SavedBackpackItems)
                {
                    if (item != null && !item.Deleted)
                        pm.Backpack.DropItem(item);
                }
            }

            // 4. Teleport back to original location
            pm.MoveToWorld(marker.OriginalLocation, marker.OriginalMap);

            // 5. Clear any frozen state
            pm.Frozen = false;

            // 6. Clear combatant
            pm.Combatant = null;

            // 7. Notify player
            pm.SendMessage("You have been recovered from an interrupted duel due to a server restart.");

            // 8. Delete marker
            marker.Delete();
        }

        private static void RemoveAllArenaGear(Mobile m)
        {
            // Remove equipped arena items
            var itemsToRemove = new List<Item>();
            foreach (var item in m.Items)
            {
                if (IsArenaItem(item))
                {
                    itemsToRemove.Add(item);
                }
            }

            // Remove backpack arena items
            if (m.Backpack != null)
            {
                foreach (var item in m.Backpack.Items)
                {
                    if (IsArenaItem(item))
                    {
                        itemsToRemove.Add(item);
                    }
                }
            }

            // Delete all arena items
            foreach (var item in itemsToRemove)
                item.Delete();
        }

        private static bool IsArenaItem(Item item)
        {
            // Check if item is an arena item by name or type
            if (item == null)
                return false;

            // Check name for arena-related keywords
            if (item.Name != null &&
                (item.Name.Contains("Arena") ||
                 item.Name.Contains("Invulnerable") ||
                 item.Name.Contains("(Infinite)")))
            {
                return true;
            }

            // Check if it's a DuelPit item (all arena items inherit from DuelPitInfiniteItem or have DuelPit in name)
            if (item is DuelPitInfiniteItem ||
                item.GetType().Name.StartsWith("DuelPit"))
            {
                return true;
            }

            // Check for arena surcoats (red hue 0x26 or blue hue 0x5)
            if (item is Surcoat surcoat && (surcoat.Hue == 0x26 || surcoat.Hue == 0x5))
            {
                return true;
            }

            // Check for arena spellbooks (full spellbook with all 64 spells)
            if (item is Spellbook spellbook && spellbook.Content == 0xFFFFFFFFFFFFFFFF)
            {
                return true;
            }

            return false;
        }
    }
}
