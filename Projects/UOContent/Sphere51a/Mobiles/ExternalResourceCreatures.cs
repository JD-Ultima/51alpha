using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles;

// ========================================
// DESPISE EXTERNAL RESOURCE CREATURES
// ========================================

/// <summary>
/// External resource Ettin that drops Blood Sacs for Despise cooking recipes.
/// These spawn outside Despise dungeon and are efficiently farmed with Blood Tentacle weapon.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceEttin : Ettin
{
    [Constructible]
    public ResourceEttin()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Humanoid;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Blood Sac drops for Despise cooking recipes
        // 10% drop rate with Blood Tentacle weapon bonus, 3% without
        if (Utility.RandomDouble() < 0.10) // Base rate assumes weapon bonus
        {
            PackItem(new EttinBloodSac(Utility.RandomMinMax(1, 3)));
        }
    }
}

/// <summary>
/// External resource Troll that drops Regenerative Tissue for Despise cooking recipes.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceTroll : Troll
{
    [Constructible]
    public ResourceTroll()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Humanoid;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Regenerative Tissue drops for Despise cooking recipes
        // 8% drop rate with weapon bonus, 2% without
        if (Utility.RandomDouble() < 0.08)
        {
            PackItem(new TrollRegenerativeTissue(Utility.RandomMinMax(1, 2)));
        }
    }
}

/// <summary>
/// External resource Ogre that drops Thick Hide for Despise cooking recipes.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceOgre : Ogre
{
    [Constructible]
    public ResourceOgre()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Giant;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Thick Hide drops for Despise cooking recipes
        // 12% drop rate with weapon bonus, 4% without
        if (Utility.RandomDouble() < 0.12)
        {
            PackItem(new OgreThickHide(Utility.RandomMinMax(1, 3)));
        }
    }
}

/// <summary>
/// External resource Cyclops that drops Cyclops Eye for advanced Despise cooking recipes.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceCyclops : Cyclops
{
    [Constructible]
    public ResourceCyclops()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Giant;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Cyclops Eye drops for Despise Level 3 buff food
        // 15% drop rate with weapon bonus, 5% without
        if (Utility.RandomDouble() < 0.15)
        {
            PackItem(new CyclopsEye(1));
        }
    }
}

// ========================================
// SHAME EXTERNAL RESOURCE CREATURES
// ========================================

/// <summary>
/// External resource Dire Wolf that drops Feral Fangs for Shame cooking recipes.
/// These spawn outside Shame dungeon and are efficiently farmed with Wolf Bane weapon.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceDireWolf : DireWolf
{
    [Constructible]
    public ResourceDireWolf()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Beast;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Feral Fangs drops for Shame cooking recipes
        // 10% drop rate with Wolf Bane weapon bonus, 3% without
        if (Utility.RandomDouble() < 0.10)
        {
            PackItem(new DireWolfFang(Utility.RandomMinMax(1, 2)));
        }
    }
}

/// <summary>
/// External resource Wild Boar that drops Beast Essence for Shame cooking recipes.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceBoar : Boar
{
    [Constructible]
    public ResourceBoar()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Beast;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Beast Essence drops for Shame cooking recipes
        // 8% drop rate with weapon bonus, 2% without
        if (Utility.RandomDouble() < 0.08)
        {
            PackItem(new WildBoarEssence(1));
        }
    }
}

/// <summary>
/// External resource Scorpion that drops Venom Glands for Shame cooking recipes.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceScorpion : Scorpion
{
    [Constructible]
    public ResourceScorpion()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Beast;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Venom Glands drops for Shame cooking recipes
        // 12% drop rate with weapon bonus, 4% without
        if (Utility.RandomDouble() < 0.12)
        {
            PackItem(new ScorpionVenomGland(Utility.RandomMinMax(1, 3)));
        }
    }
}

/// <summary>
/// External resource Kraken that drops Tentacles for advanced Shame cooking recipes.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceKraken : Kraken
{
    [Constructible]
    public ResourceKraken()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Aquatic;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Kraken Tentacle drops for Shame Level 2 buff food
        // 8% drop rate with weapon bonus, 2% without
        if (Utility.RandomDouble() < 0.08)
        {
            PackItem(new KrakenTentacle(1));
        }
    }
}

/// <summary>
/// External resource Phoenix (placeholder - using Phoenix if exists, else custom) for Shame cooking.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourcePhoenix : BaseCreature
{
    [Constructible]
    public ResourcePhoenix() : base(AIType.AI_Mage)
    {
        Body = 5; // Phoenix body
        BaseSoundID = 0x8F;
        Hue = 0x2B; // Fire hue

        SetStr(504, 700);
        SetDex(202, 300);
        SetInt(504, 700);

        SetHits(340, 383);

        SetDamage(25, 30);

        SetDamageType(ResistanceType.Physical, 50);
        SetDamageType(ResistanceType.Fire, 50);

        SetResistance(ResistanceType.Physical, 45, 55);
        SetResistance(ResistanceType.Fire, 60, 70);
        SetResistance(ResistanceType.Cold, 25, 35);
        SetResistance(ResistanceType.Poison, 30, 40);
        SetResistance(ResistanceType.Energy, 30, 40);

        SetSkill(SkillName.EvalInt, 90.2, 100.0);
        SetSkill(SkillName.Magery, 90.2, 100.0);
        SetSkill(SkillName.Meditation, 75.1, 100.0);
        SetSkill(SkillName.MagicResist, 86.0, 135.0);
        SetSkill(SkillName.Tactics, 80.1, 90.0);
        SetSkill(SkillName.Wrestling, 90.1, 100.0);

        Fame = 15000;
        Karma = 0;

        VirtualArmor = 50;
    }

    public override string CorpseName => "a phoenix corpse";
    public override string DefaultName => "a phoenix";
    public override MonsterFamily Family => MonsterFamily.Elemental;

    public override void GenerateLoot()
    {
        AddLoot(LootPack.FilthyRich);
        AddLoot(LootPack.Rich);

        // Sphere51a: Phoenix Feather drops for Shame Level 3 buff food
        // 6% drop rate with weapon bonus, 1% without
        if (Utility.RandomDouble() < 0.06)
        {
            PackItem(new PhoenixFeather(Utility.RandomMinMax(1, 2)));
        }
    }
}

/// <summary>
/// External resource Blood Elemental source for advanced Shame cooking recipes.
/// Note: This assumes BloodElemental exists in codebase. Adjust if needed.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceBloodElemental : BloodElemental
{
    [Constructible]
    public ResourceBloodElemental()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Elemental;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Blood Elemental Core drops for Shame Level 4 buff food
        // 5% drop rate with weapon bonus, 1% without
        if (Utility.RandomDouble() < 0.05)
        {
            PackItem(new BloodElementalCore(1));
        }
    }
}

// ========================================
// WRONG EXTERNAL RESOURCE CREATURES
// ========================================

/// <summary>
/// External resource Juka Scout that drops Crystal Shards for Wrong cooking recipes.
/// These spawn outside Wrong dungeon and are efficiently farmed with Diamond Katana weapon.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceJukaWarrior : JukaWarrior
{
    [Constructible]
    public ResourceJukaWarrior()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Crystalline;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Juka Crystal Shard drops for Wrong cooking recipes
        // 8% drop rate with Diamond Katana weapon bonus, 2% without
        if (Utility.RandomDouble() < 0.08)
        {
            PackItem(new JukaCrystalShard(Utility.RandomMinMax(1, 2)));
        }

        // Additional scout-specific drop for Level 1 buff
        if (Utility.RandomDouble() < 0.10)
        {
            PackItem(new JukaScoutShard(1));
        }
    }
}

/// <summary>
/// External resource Golem that drops Stone Hearts for Wrong cooking recipes.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceGolem : Golem
{
    [Constructible]
    public ResourceGolem()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Stone;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Golem Stone Heart drops for Wrong cooking recipes
        // 6% drop rate with weapon bonus, 1% without
        if (Utility.RandomDouble() < 0.06)
        {
            PackItem(new GolemStoneHeart(1));
        }
    }
}

/// <summary>
/// External resource Brigand that drops Stolen Tools for Wrong cooking recipes.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceBrigand : Brigand
{
    [Constructible]
    public ResourceBrigand()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Humanoid;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Stolen Tools drops for Wrong cooking recipes
        // 15% drop rate with weapon bonus, 10% without (easier to get)
        if (Utility.RandomDouble() < 0.15)
        {
            PackItem(new BrigandStolenTools(1));
        }
    }
}

/// <summary>
/// External resource Juka Lord that drops Crystal Core for advanced Wrong cooking recipes.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceJukaLord : JukaLord
{
    [Constructible]
    public ResourceJukaLord()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Crystalline;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Juka Lord Crystal Core drops for Wrong Level 3 buff food
        // 5% drop rate with weapon bonus, 1% without
        if (Utility.RandomDouble() < 0.05)
        {
            PackItem(new JukaLordCrystalCore(1));
        }
    }
}

// ========================================
// DESTARD EXTERNAL RESOURCE CREATURES
// ========================================

/// <summary>
/// External resource Drake that drops Lesser Dragon Scales for Destard cooking recipes.
/// These spawn outside Destard dungeon and are efficiently farmed with Dwarven Axe weapon.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceDrake : Drake
{
    [Constructible]
    public ResourceDrake()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Dragon;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Drake Lesser Scale drops for Destard access food
        // 6% drop rate with Dwarven Axe weapon bonus, 1% without
        if (Utility.RandomDouble() < 0.06)
        {
            PackItem(new DrakeLesserScale(Utility.RandomMinMax(1, 2)));
        }

        // Drake Scale for Level 1 buff
        if (Utility.RandomDouble() < 0.08)
        {
            PackItem(new DrakeScale(1));
        }
    }
}

/// <summary>
/// External resource Wyvern that drops Wing Membranes for Destard cooking recipes.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceWyvern : Wyvern
{
    [Constructible]
    public ResourceWyvern()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Dragon;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Wyvern Wing Membrane drops for Destard cooking recipes
        // 5% drop rate with weapon bonus, 1% without
        if (Utility.RandomDouble() < 0.05)
        {
            PackItem(new WyvernWingMembrane(Utility.RandomMinMax(1, 2)));
        }
    }
}

/// <summary>
/// External resource Giant Serpent that drops Venom Sacs for Destard cooking recipes.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceGiantSerpent : GiantSerpent
{
    [Constructible]
    public ResourceGiantSerpent()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Serpent;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Giant Serpent Venom Sac drops for Destard cooking recipes
        // 10% drop rate with weapon bonus, 3% without
        if (Utility.RandomDouble() < 0.10)
        {
            PackItem(new GiantSerpentVenomSac(Utility.RandomMinMax(1, 3)));
        }
    }
}

/// <summary>
/// External resource Shadow Wyrm that drops scales for advanced Destard cooking recipes.
/// Note: Rare spawn, higher value drops.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceShadowWyrm : ShadowWyrm
{
    [Constructible]
    public ResourceShadowWyrm()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Dragon;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Shadow Wyrm Scale drops for Destard Level 2 buff food
        // 5% drop rate with weapon bonus, 1% without
        if (Utility.RandomDouble() < 0.05)
        {
            PackItem(new ShadowWyrmScale(1));
        }
    }
}

/// <summary>
/// External resource Ancient Wyrm that drops hearts for master-level Destard cooking recipes.
/// Note: Very rare spawn, extremely valuable drops.
/// </summary>
[SerializationGenerator(0, false)]
public partial class ResourceAncientWyrm : AncientWyrm
{
    [Constructible]
    public ResourceAncientWyrm()
    {
    }

    public override MonsterFamily Family => MonsterFamily.Dragon;

    public override void GenerateLoot()
    {
        base.GenerateLoot();

        // Sphere51a: Ancient Wyrm Heart drops for Destard Level 3 buff food (100 GM Cooking required)
        // 3% drop rate with weapon bonus, 0.5% without
        if (Utility.RandomDouble() < 0.03)
        {
            PackItem(new AncientWyrmHeart(1));
        }
    }
}
