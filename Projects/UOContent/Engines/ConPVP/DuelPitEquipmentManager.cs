using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Engines.ConPVP
{
    public class DuelPitEquipmentManager
    {
        private readonly Mobile _mobile;
        private readonly bool _isChallenger;
        private readonly List<Item> _savedEquipment = new List<Item>();
        private readonly List<Item> _savedBackpackItems = new List<Item>();
        private readonly List<Item> _arenaItems = new List<Item>();

        public DuelPitEquipmentManager(Mobile mobile, bool isChallenger)
        {
            _mobile = mobile;
            _isChallenger = isChallenger;
        }

        public void SaveAndClearEquipment()
        {
            // Debug logging - show what player is wearing BEFORE saving
            Console.WriteLine($"[DuelPit] SaveAndClearEquipment - {_mobile.Name} currently wearing {_mobile.Items.Count} items:");
            foreach (var item in _mobile.Items)
            {
                Console.WriteLine($"  - {item.GetType().Name}: {item.Name ?? "(unnamed)"} on layer {item.Layer}");
            }

            // Save and remove all equipped items
            var equipItems = new List<Item>(_mobile.Items);
            foreach (var item in equipItems)
            {
                if (item is Container) // Skip backpack itself
                {
                    Console.WriteLine($"[DuelPit] Skipping container: {item.GetType().Name}");
                    continue;
                }

                _savedEquipment.Add(item);
                _mobile.RemoveItem(item);
                Console.WriteLine($"[DuelPit] Saved and removed: {item.GetType().Name} ({item.Name}) Serial: {item.Serial}");
            }

            // Save and remove all backpack items
            Container backpack = _mobile.Backpack;
            if (backpack != null)
            {
                var backpackItems = new List<Item>(backpack.Items);
                Console.WriteLine($"[DuelPit] Backpack contains {backpackItems.Count} items");
                foreach (var item in backpackItems)
                {
                    _savedBackpackItems.Add(item);
                    backpack.RemoveItem(item);
                }
            }

            // Debug logging
            Console.WriteLine($"[DuelPit] Saved equipment for {_mobile.Name}: {_savedEquipment.Count} equipped items, {_savedBackpackItems.Count} backpack items");
        }

        public void EquipArenaGear()
        {
            // Equip Invulnerable Plate Mail Armor
            EquipArenaItem(new PlateChest { Quality = ArmorQuality.Exceptional, Name = "Invulnerable Plate Chest"});
            EquipArenaItem(new PlateArms { Quality = ArmorQuality.Exceptional, Name = "Invulnerable Plate Arms" });
            EquipArenaItem(new PlateGloves { Quality = ArmorQuality.Exceptional, Name = "Invulnerable Plate Gloves" });
            EquipArenaItem(new PlateGorget { Quality = ArmorQuality.Exceptional, Name = "Invulnerable Plate Gorget" });
            EquipArenaItem(new PlateLegs { Quality = ArmorQuality.Exceptional, Name = "Invulnerable Plate Legs" });
            EquipArenaItem(new PlateHelm { Quality = ArmorQuality.Exceptional, Name = "Invulnerable Plate Helm" });

            // Equip team-colored surcoat (Red for challenger, Blue for challenged)
            EquipArenaItem(new Surcoat { Hue = _isChallenger ? 0x26 : 0x5 });

            Container backpack = _mobile.Backpack;
            if (backpack == null)
                return;

            // Add Vanquishing weapons to backpack
            AddArenaItem(backpack, new Bardiche { Name = "Arena Bardiche", DamageLevel = WeaponDamageLevel.Vanq });
            AddArenaItem(backpack, new Halberd { Name = "Arena Halberd", DamageLevel = WeaponDamageLevel.Vanq });
            AddArenaItem(backpack, new WarHammer { Name = "Arena War Hammer", DamageLevel = WeaponDamageLevel.Vanq });
            AddArenaItem(backpack, new HammerPick { Name = "Arena Hammer Pick", DamageLevel = WeaponDamageLevel.Vanq });
            AddArenaItem(backpack, new HeaterShield { Name = "Invulnerable Heater Shield" });
            AddArenaItem(backpack, new QuarterStaff { Name = "Arena Quarter Staff", DamageLevel = WeaponDamageLevel.Vanq });
            AddArenaItem(backpack, new Spear { Name = "Arena Spear", DamageLevel = WeaponDamageLevel.Vanq });
            AddArenaItem(backpack, new Katana { Name = "Arena Katana", DamageLevel = WeaponDamageLevel.Vanq });
            AddArenaItem(backpack, new Kryss { Name = "Arena Kryss", DamageLevel = WeaponDamageLevel.Vanq });
            AddArenaItem(backpack, new VikingSword { Name = "Arena Viking Sword", DamageLevel = WeaponDamageLevel.Vanq });

            // Add full spellbook
            var spellbook = new Spellbook { Content = 0xFFFFFFFFFFFFFFFF }; // All spells
            AddArenaItem(backpack, spellbook);

            // Add infinite-use potions
            AddArenaItem(backpack, new DuelPitGreaterHealPotion());
            AddArenaItem(backpack, new DuelPitGreaterManaPotion());
            AddArenaItem(backpack, new DuelPitGreaterDexterityPotion());
            AddArenaItem(backpack, new DuelPitGreaterStrengthPotion());
            AddArenaItem(backpack, new DuelPitGreaterRefreshPotion());

            // Add infinite-use bandages
            AddArenaItem(backpack, new DuelPitBandages());

            // Add infinite-use scrolls
            AddArenaItem(backpack, new DuelPitLightningScroll());
            AddArenaItem(backpack, new DuelPitFlameStrikeScroll());
            AddArenaItem(backpack, new DuelPitGreaterHealScroll());
            AddArenaItem(backpack, new DuelPitReflectScroll());

            // Add infinite-use reagents (2 of each type)
            AddArenaItem(backpack, new DuelPitBlackPearl());
            AddArenaItem(backpack, new DuelPitBloodmoss());
            AddArenaItem(backpack, new DuelPitGarlic());
            AddArenaItem(backpack, new DuelPitGinseng());
            AddArenaItem(backpack, new DuelPitMandrakeRoot());
            AddArenaItem(backpack, new DuelPitNightshade());
            AddArenaItem(backpack, new DuelPitSulfurousAsh());
            AddArenaItem(backpack, new DuelPitSpidersSilk());
        }

        private void EquipArenaItem(Item item)
        {
            // CRITICAL: Make arena items blessed so they don't go to corpses on death
            // This prevents them from being deleted when corpse is handled
            item.LootType = LootType.Blessed;

            _arenaItems.Add(item);
            _mobile.EquipItem(item);
        }

        private void AddArenaItem(Container container, Item item)
        {
            // CRITICAL: Make arena items blessed so they don't go to corpses on death
            item.LootType = LootType.Blessed;

            _arenaItems.Add(item);
            container.DropItem(item);
        }

        public void RemoveArenaGear()
        {
            Console.WriteLine($"[DuelPit] RemoveArenaGear for {_mobile.Name} - {_arenaItems.Count} arena items to remove");

            // CRITICAL: Check if any arena items are in the saved equipment list
            foreach (var arenaItem in _arenaItems)
            {
                if (_savedEquipment.Contains(arenaItem))
                {
                    Console.WriteLine($"[DuelPit] CRITICAL ERROR: Arena item {arenaItem.GetType().Name} (Serial {arenaItem.Serial}) is in SAVED EQUIPMENT!");
                }
                if (_savedBackpackItems.Contains(arenaItem))
                {
                    Console.WriteLine($"[DuelPit] CRITICAL ERROR: Arena item {arenaItem.GetType().Name} (Serial {arenaItem.Serial}) is in SAVED BACKPACK!");
                }
            }

            // Remove and delete all arena items
            foreach (var item in _arenaItems)
            {
                if (item != null && !item.Deleted)
                {
                    Console.WriteLine($"[DuelPit] Removing arena item: {item.GetType().Name} ({item.Name}) Serial: {item.Serial}");

                    // Remove from mobile (equipped items)
                    if (item.Parent == _mobile)
                    {
                        _mobile.RemoveItem(item);
                    }
                    // Remove from container (backpack items)
                    else if (item.Parent is Container container)
                    {
                        container.RemoveItem(item);
                    }

                    item.Delete();
                    Console.WriteLine($"[DuelPit] Deleted arena item: {item.GetType().Name} Serial: {item.Serial}");
                }
            }
            _arenaItems.Clear();

            // Verify saved equipment is still intact
            Console.WriteLine($"[DuelPit] After RemoveArenaGear - {_mobile.Name} still has {_savedEquipment.Count} saved equipment items:");
            foreach (var item in _savedEquipment)
            {
                if (item != null && !item.Deleted)
                {
                    Console.WriteLine($"  - SAVED (OK): {item.GetType().Name} ({item.Name}) Serial: {item.Serial}");
                }
                else
                {
                    Console.WriteLine($"  - SAVED (PROBLEM!): {(item == null ? "null" : item.Deleted ? $"DELETED! {item.GetType().Name} Serial: {item.Serial}" : "unknown")}");
                }
            }
        }

        public void RestoreOriginalEquipment()
        {
            // Debug logging - show detailed list of what we're trying to restore
            Console.WriteLine($"[DuelPit] RestoreOriginalEquipment for {_mobile.Name}: {_savedEquipment.Count} equipped items, {_savedBackpackItems.Count} backpack items");
            Console.WriteLine($"[DuelPit] Current stats - Str: {_mobile.Str}/{_mobile.RawStr}, Dex: {_mobile.Dex}/{_mobile.RawDex}, Int: {_mobile.Int}/{_mobile.RawInt}");
            Console.WriteLine($"[DuelPit] Equipped items to restore:");
            foreach (var item in _savedEquipment)
            {
                if (item != null && !item.Deleted)
                {
                    Console.WriteLine($"  - READY TO RESTORE: {item.GetType().Name} ({item.Name})");
                }
                else
                {
                    Console.WriteLine($"  - CANNOT RESTORE: {(item == null ? "null item" : "DELETED ITEM - " + item.GetType().Name)}");
                }
            }

            // Restore equipped items
            int restoredEquip = 0;
            Container backpack = _mobile.Backpack;

            foreach (var item in _savedEquipment)
            {
                if (item != null && !item.Deleted)
                {
                    // Try to equip the item
                    if (_mobile.EquipItem(item))
                    {
                        restoredEquip++;
                        Console.WriteLine($"[DuelPit] Successfully restored: {item.GetType().Name} ({item.Name})");
                    }
                    else
                    {
                        // If equip fails (e.g., stat requirements not met), put in backpack as fallback
                        Console.WriteLine($"[DuelPit] WARNING: Failed to equip {item.GetType().Name} ({item.Name}) - placing in backpack (Str: {_mobile.Str})");

                        if (backpack != null)
                        {
                            backpack.DropItem(item);
                        }
                        else
                        {
                            // Last resort: drop at player's feet
                            Console.WriteLine($"[DuelPit] ERROR: No backpack for {_mobile.Name} - dropping {item.GetType().Name} at feet");
                            item.MoveToWorld(_mobile.Location, _mobile.Map);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"[DuelPit] Skipped equipment item: {(item == null ? "null" : item.Deleted ? "deleted" : "unknown")}");
                }
            }

            // Restore backpack items
            int restoredBackpack = 0;
            if (backpack != null)
            {
                foreach (var item in _savedBackpackItems)
                {
                    if (item != null && !item.Deleted)
                    {
                        backpack.DropItem(item);
                        restoredBackpack++;
                    }
                    else
                    {
                        Console.WriteLine($"[DuelPit] Skipped backpack item: {(item == null ? "null" : item.Deleted ? "deleted" : "unknown")}");
                    }
                }
            }

            Console.WriteLine($"[DuelPit] Restored {restoredEquip}/{_savedEquipment.Count} equipped items and {restoredBackpack}/{_savedBackpackItems.Count} backpack items");

            _savedEquipment.Clear();
            _savedBackpackItems.Clear();
        }

        // Equipment accessors for recovery marker
        public List<Item> GetSavedEquipment() => new List<Item>(_savedEquipment);
        public List<Item> GetSavedBackpackItems() => new List<Item>(_savedBackpackItems);
    }
}
