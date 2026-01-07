using Server.Items;
using ModernUO.Serialization;

namespace Server.Engines.ConPVP
{
    // Base class for infinite-use duel pit items
    public abstract class DuelPitInfiniteItem : Item
    {
        protected DuelPitInfiniteItem(int itemID) : base(itemID)
        {
            Movable = true;
            LootType = LootType.Cursed; // Prevents leaving the player
        }

        // Deserialization constructor for SerializationGenerator
        protected DuelPitInfiniteItem(Serial serial) : base(serial)
        {
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D location)
        {
            // Delete items if dropped in the world
            Delete();
            return false;
        }

        public override bool OnDroppedOnto(Mobile from, Item target)
        {
            // Prevent dropping onto other items
            from.SendMessage("You cannot drop this item.");
            return false;
        }

        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            // Prevent trading
            from.SendMessage("You cannot give this item away.");
            return false;
        }
    }

    // Infinite-use potions
    [SerializationGenerator(0)]
    public partial class DuelPitGreaterHealPotion : DuelPitInfiniteItem
    {
        [Constructible]
        public DuelPitGreaterHealPotion() : base(0xF0C) // Greater Heal Potion graphic
        {
            Name = "Greater Heal Potion (Infinite)";
            Hue = 0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            from.Heal(30); // Greater Heal Potion heals ~30 HP
            from.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            from.PlaySound(0x1F2);
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitGreaterManaPotion : DuelPitInfiniteItem
    {
        [Constructible]
        public DuelPitGreaterManaPotion() : base(0xF0D) // Mana Potion graphic
        {
            Name = "Greater Mana Potion (Infinite)";
            Hue = 0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            from.Mana += 20; // Greater Mana Potion restores ~20 mana
            if (from.Mana > from.ManaMax)
                from.Mana = from.ManaMax;

            from.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            from.PlaySound(0x1F2);
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitGreaterDexterityPotion : DuelPitInfiniteItem
    {
        [Constructible]
        public DuelPitGreaterDexterityPotion() : base(0xF08) // Dexterity Potion graphic
        {
            Name = "Greater Dexterity Potion (Infinite)";
            Hue = 0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            int offset = 15; // Greater Dex potion
            from.Dex += offset;
            from.SendMessage($"Your dexterity has increased by {offset}.");
            from.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            from.PlaySound(0x1F2);
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitGreaterStrengthPotion : DuelPitInfiniteItem
    {
        [Constructible]
        public DuelPitGreaterStrengthPotion() : base(0xF09) // Strength Potion graphic
        {
            Name = "Greater Strength Potion (Infinite)";
            Hue = 0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            int offset = 15; // Greater Str potion
            from.Str += offset;
            from.SendMessage($"Your strength has increased by {offset}.");
            from.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            from.PlaySound(0x1F2);
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitGreaterRefreshPotion : DuelPitInfiniteItem
    {
        [Constructible]
        public DuelPitGreaterRefreshPotion() : base(0xF0B) // Refresh Potion graphic
        {
            Name = "Greater Refresh Potion (Infinite)";
            Hue = 0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            from.Stam = from.StamMax;
            from.SendMessage("You feel completely refreshed.");
            from.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            from.PlaySound(0x1F2);
        }
    }

    // Infinite-use bandages
    [SerializationGenerator(0)]
    public partial class DuelPitBandages : DuelPitInfiniteItem
    {
        [Constructible]
        public DuelPitBandages() : base(0xE21) // Bandage graphic
        {
            Name = "Bandages (Infinite)";
            Amount = 1;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            // Use standard bandage heal behavior
            BandageContext context = BandageContext.GetContext(from);
            if (context != null)
            {
                from.SendLocalizedMessage(500119); // You must wait a few moments before using another bandage.
                return;
            }

            from.SendLocalizedMessage(500956); // You begin applying the bandages.
            BandageContext.BeginHeal(from, from);
        }
    }

    // Infinite-use scrolls
    [SerializationGenerator(0)]
    public partial class DuelPitLightningScroll : DuelPitInfiniteItem
    {
        [Constructible]
        public DuelPitLightningScroll() : base(0x1F5F) // Scroll graphic
        {
            Name = "Lightning Scroll (Infinite)";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            // Cast Lightning spell
            var spell = new Spells.Fourth.LightningSpell(from, this);
            spell.Cast();
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitFlameStrikeScroll : DuelPitInfiniteItem
    {
        [Constructible]
        public DuelPitFlameStrikeScroll() : base(0x1F5F)
        {
            Name = "Flame Strike Scroll (Infinite)";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            var spell = new Spells.Seventh.FlameStrikeSpell(from, this);
            spell.Cast();
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitGreaterHealScroll : DuelPitInfiniteItem
    {
        [Constructible]
        public DuelPitGreaterHealScroll() : base(0x1F5F)
        {
            Name = "Greater Heal Scroll (Infinite)";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            var spell = new Spells.Fourth.GreaterHealSpell(from, this);
            spell.Cast();
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitReflectScroll : DuelPitInfiniteItem
    {
        [Constructible]
        public DuelPitReflectScroll() : base(0x1F5F)
        {
            Name = "Reflect Scroll (Infinite)";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            var spell = new Spells.Fifth.MagicReflectSpell(from, this);
            spell.Cast();
        }
    }

    // Infinite-use reagents - each reagent type needs its own class
    // These reagents have Amount = 2 but never decrease when consumed

    [SerializationGenerator(0)]
    public partial class DuelPitBlackPearl : BaseReagent
    {
        [Constructible]
        public DuelPitBlackPearl() : base(0xF7A, 2)
        {
            Name = "Black Pearl (Infinite)";
            Movable = true;
            LootType = LootType.Cursed;
        }

        public new int Amount
        {
            get => 2;
            set
            {
                // Always maintain amount at 2, never decrease
                // Ignore any attempts to change the amount
            }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D location)
        {
            Delete();
            return false;
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitBloodmoss : BaseReagent
    {
        [Constructible]
        public DuelPitBloodmoss() : base(0xF7B, 2)
        {
            Name = "Bloodmoss (Infinite)";
            Movable = true;
            LootType = LootType.Cursed;
        }

        public new int Amount
        {
            get => 2;
            set { /* Ignore - maintain amount at 2 */ }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D location)
        {
            Delete();
            return false;
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitGarlic : BaseReagent
    {
        [Constructible]
        public DuelPitGarlic() : base(0xF84, 2)
        {
            Name = "Garlic (Infinite)";
            Movable = true;
            LootType = LootType.Cursed;
        }

        public new int Amount
        {
            get => 2;
            set { /* Ignore - maintain amount at 2 */ }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D location)
        {
            Delete();
            return false;
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitGinseng : BaseReagent
    {
        [Constructible]
        public DuelPitGinseng() : base(0xF85, 2)
        {
            Name = "Ginseng (Infinite)";
            Movable = true;
            LootType = LootType.Cursed;
        }

        public new int Amount
        {
            get => 2;
            set { /* Ignore - maintain amount at 2 */ }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D location)
        {
            Delete();
            return false;
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitMandrakeRoot : BaseReagent
    {
        [Constructible]
        public DuelPitMandrakeRoot() : base(0xF86, 2)
        {
            Name = "Mandrake Root (Infinite)";
            Movable = true;
            LootType = LootType.Cursed;
        }

        public new int Amount
        {
            get => 2;
            set { /* Ignore - maintain amount at 2 */ }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D location)
        {
            Delete();
            return false;
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitNightshade : BaseReagent
    {
        [Constructible]
        public DuelPitNightshade() : base(0xF88, 2)
        {
            Name = "Nightshade (Infinite)";
            Movable = true;
            LootType = LootType.Cursed;
        }

        public new int Amount
        {
            get => 2;
            set { /* Ignore - maintain amount at 2 */ }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D location)
        {
            Delete();
            return false;
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitSulfurousAsh : BaseReagent
    {
        [Constructible]
        public DuelPitSulfurousAsh() : base(0xF8C, 2)
        {
            Name = "Sulfurous Ash (Infinite)";
            Movable = true;
            LootType = LootType.Cursed;
        }

        public new int Amount
        {
            get => 2;
            set { /* Ignore - maintain amount at 2 */ }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D location)
        {
            Delete();
            return false;
        }
    }

    [SerializationGenerator(0)]
    public partial class DuelPitSpidersSilk : BaseReagent
    {
        [Constructible]
        public DuelPitSpidersSilk() : base(0xF8D, 2)
        {
            Name = "Spiders' Silk (Infinite)";
            Movable = true;
            LootType = LootType.Cursed;
        }

        public new int Amount
        {
            get => 2;
            set { /* Ignore - maintain amount at 2 */ }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D location)
        {
            Delete();
            return false;
        }
    }
}
