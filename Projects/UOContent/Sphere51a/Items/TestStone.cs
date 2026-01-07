using System;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

namespace Server.Items;

/// <summary>
/// Test stone that provides configurable rewards to players when double-clicked
/// GM can double-click to configure reward amounts via gump
/// </summary>
public class TestStone : Item
{
    private int _skillCap = 100;

    private int _statCap = 100;

    private int _reagentAmount = 50;

    private int _scrollAmount = 20;

    private int _potionAmount = 10;

    public int SkillCap { get => _skillCap; set => _skillCap = Math.Clamp(value, 0, 120); }
    public int StatCap { get => _statCap; set => _statCap = Math.Clamp(value, 0, 150); }
    public int ReagentAmount { get => _reagentAmount; set => _reagentAmount = Math.Max(0, value); }
    public int ScrollAmount { get => _scrollAmount; set => _scrollAmount = Math.Max(0, value); }
    public int PotionAmount { get => _potionAmount; set => _potionAmount = Math.Max(0, value); }

    [Constructible]
    public TestStone() : base(0xED4) // Stone item ID
    {
        Name = "Test Stone";
        Hue = 0x33D; // Bright blue hue
        Movable = false; // Immovable by players, GMs use commands
        Weight = 100.0;
    }

    public TestStone(Serial serial) : base(serial) { }

    public override void OnDoubleClick(Mobile from)
    {
        if (!from.InRange(GetWorldLocation(), 2))
        {
            from.SendLocalizedMessage(500446); // That is too far away.
            return;
        }

        if (from.AccessLevel >= AccessLevel.GameMaster)
        {
            // GM gets config gump - temporarily disabled due to build issues
            // from.SendGump(new TestStoneGump(this));
            from.SendMessage("Test Stone GM access. Use [props] to configure SkillCap, StatCap, ReagentAmount, ScrollAmount, PotionAmount.");
        }
        else
        {
            // Player gets rewards
            GivePlayerRewards(from);
        }
    }

    private void GivePlayerRewards(Mobile from)
    {
        var pm = from as PlayerMobile;
        if (pm == null)
            return;

        // Set skills to configured cap
        foreach (var skill in pm.Skills)
        {
            skill.Base = _skillCap;
        }

        // Set stats
        pm.Str = pm.RawStr = _statCap;
        pm.Int = pm.RawInt = _statCap;
        pm.Dex = pm.RawDex = _statCap;

        // Spellbook
        var spellbook = new Spellbook { Content = ulong.MaxValue }; // All spells
        pm.BankBox.DropItem(spellbook);

        // Bandages
        var bandages = new Bandage() { Amount = 100 };
        pm.BankBox.DropItem(bandages);

        // Grey bag with plate armor
        var armorBag = new Bag { Name = "Plate Armor", Hue = 0x23 /* Grey */ };
        pm.BankBox.DropItem(armorBag);

        // Plate armor pieces - assuming standard ModernUO IDs
        armorBag.DropItem(new PlateHelm());
        armorBag.DropItem(new PlateGorget());
        armorBag.DropItem(new PlateGloves());
        armorBag.DropItem(new PlateArms());
        armorBag.DropItem(new PlateChest());
        armorBag.DropItem(new PlateLegs());
        // Dispersed randomly in bag
        RandomizeBagContents(armorBag);

        // Dark grey bag with weapons
        var weaponBag = new Bag { Name = "Weapons", Hue = 0x455 /* Dark grey */ };
        pm.BankBox.DropItem(weaponBag);

        weaponBag.DropItem(new ExecutionersAxe());
        weaponBag.DropItem(new Halberd());
        weaponBag.DropItem(new Bardiche());
        weaponBag.DropItem(new Katana());
        weaponBag.DropItem(new HeaterShield());
        weaponBag.DropItem(new VikingSword());
        weaponBag.DropItem(new HammerPick());
        weaponBag.DropItem(new WarHammer());
        RandomizeBagContents(weaponBag);

        // 1 Red bag with reagents
        var reagentBag = new Bag { Name = "Reagents", Hue = 0x21 }; // Red
        pm.BankBox.DropItem(reagentBag);

        AddReagent(reagentBag, typeof(BlackPearl), _reagentAmount);
        AddReagent(reagentBag, typeof(Bloodmoss), _reagentAmount);
        AddReagent(reagentBag, typeof(Garlic), _reagentAmount);
        AddReagent(reagentBag, typeof(Ginseng), _reagentAmount);
        AddReagent(reagentBag, typeof(MandrakeRoot), _reagentAmount);
        AddReagent(reagentBag, typeof(Nightshade), _reagentAmount);
        AddReagent(reagentBag, typeof(SulfurousAsh), _reagentAmount);
        AddReagent(reagentBag, typeof(SpidersSilk), _reagentAmount);

        // 1 Purple bag with scrolls
        var scrollBag = new Bag { Name = "Mage Scrolls", Hue = 0x17 }; // Purple
        pm.BankBox.DropItem(scrollBag);

        AddScroll(scrollBag, typeof(FlamestrikeScroll), _scrollAmount);
        AddScroll(scrollBag, typeof(LightningScroll), _scrollAmount);
        AddScroll(scrollBag, typeof(GreaterHealScroll), _scrollAmount);
        AddScroll(scrollBag, typeof(MagicReflectScroll), _scrollAmount);

        // 1 Orange bag with potions
        var potionBag = new Bag { Name = "Potions", Hue = 29 }; // Orange
        pm.BankBox.DropItem(potionBag);

        AddPotion(potionBag, typeof(GreaterHealPotion), _potionAmount);
        AddPotion(potionBag, typeof(GreaterManaPotion), _potionAmount);
        AddPotion(potionBag, typeof(RefreshPotion), _potionAmount);
        AddPotion(potionBag, typeof(GreaterAgilityPotion), _potionAmount);
        AddPotion(potionBag, typeof(GreaterStrengthPotion), _potionAmount);

        // Randomize positions of all bags in bank box
        RandomizeBankBagPositions(pm.BankBox);

        from.SendMessage("You have received your test rewards!");
    }

    private void AddReagent(Container bag, Type reagentType, int amount)
    {
        var reagent = Activator.CreateInstance(reagentType, amount) as Item;
        if (reagent != null)
        {
            bag.DropItem(reagent);
        }
    }

    private void AddScroll(Container bag, Type scrollType, int amount)
    {
        var scroll = Activator.CreateInstance(scrollType, amount) as Item;
        if (scroll != null)
        {
            bag.DropItem(scroll);
        }
    }

    private void AddPotion(Container bag, Type potionType, int amount)
    {
        var potion = Activator.CreateInstance(potionType) as Item;
        if (potion != null)
        {
            potion.Amount = amount; // Stack them together
            bag.DropItem(potion);
        }
    }

    private void RandomizeBagContents(Container bag)
    {
        // Simple randomization - scatter items in the bag
        foreach (var item in bag.Items)
        {
            if (item.X == 0 && item.Y == 0) // Unplaced items
            {
                item.X = Utility.Random(20, 60);
                item.Y = Utility.Random(20, 40);
            }
        }
    }

    private void RandomizeBankBagPositions(Container bank)
    {
        // Scatter the bags containing rewards around the bank box to avoid weight/item limit issues
        foreach (var item in bank.Items)
        {
            if (item is Container)
            {
                // Place bags at random positions
                item.X = Utility.Random(20, 100);
                item.Y = Utility.Random(20, 80);
            }
        }
    }

    public override void Serialize(IGenericWriter writer)
    {
        base.Serialize(writer);
        writer.Write(0); // version
        writer.Write(_skillCap);
        writer.Write(_statCap);
        writer.Write(_reagentAmount);
        writer.Write(_scrollAmount);
        writer.Write(_potionAmount);
    }

    public override void Deserialize(IGenericReader reader)
    {
        base.Deserialize(reader);
        var version = reader.ReadInt();
        _skillCap = reader.ReadInt();
        _statCap = reader.ReadInt();
        _reagentAmount = reader.ReadInt();
        _scrollAmount = reader.ReadInt();
        _potionAmount = reader.ReadInt();
    }
}
