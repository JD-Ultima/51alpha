using ModernUO.Serialization;

namespace Server.Items;

// ========================================
// DESPISE DUNGEON INGREDIENTS
// ========================================

#region Despise - Fishing Ingredients

[SerializationGenerator(0, false)]
public partial class SwampPike : Item
{
    [Constructible]
    public SwampPike(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x455; // Swamp green hue
    }

    public override string DefaultName => "swamp pike";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class RiverTrout : Item
{
    [Constructible]
    public RiverTrout(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x8A5; // Blue-grey hue
    }

    public override string DefaultName => "river trout";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class LakeBass : Item
{
    [Constructible]
    public LakeBass(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x96D; // Lake blue hue
    }

    public override string DefaultName => "lake bass";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class DeepSeaCod : Item
{
    [Constructible]
    public DeepSeaCod(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x8B0; // Deep sea hue
    }

    public override string DefaultName => "deep sea cod";
    public override double DefaultWeight => 0.1;
}

#endregion

#region Despise - External Monster Drops

[SerializationGenerator(0, false)]
public partial class EttinBloodSac : Item
{
    [Constructible]
    public EttinBloodSac(int amount = 1) : base(0x3188) // Placeholder for later adjustment with unique ID and art (using Muculent graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x25; // Blood red hue
    }

    public override string DefaultName => "ettin blood sac";
    public override double DefaultWeight => 1.0;
}

[SerializationGenerator(0, false)]
public partial class TrollRegenerativeTissue : Item
{
    [Constructible]
    public TrollRegenerativeTissue(int amount = 1) : base(0x09F1) // Placeholder for later adjustment with unique ID and art (using RawRibs graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x3F; // Green-grey hue
    }

    public override string DefaultName => "troll regenerative tissue";
    public override double DefaultWeight => 1.0;
}

[SerializationGenerator(0, false)]
public partial class OgreThickHide : Item
{
    [Constructible]
    public OgreThickHide(int amount = 1) : base(0x3188) // Placeholder for later adjustment with unique ID and art (using Muculent graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x7D1; // Brown hide hue
    }

    public override string DefaultName => "ogre thick hide";
    public override double DefaultWeight => 2.0;
}

[SerializationGenerator(0, false)]
public partial class CyclopsEye : Item
{
    [Constructible]
    public CyclopsEye(int amount = 1) : base(0x318E) // Placeholder for later adjustment with unique ID and art (using CapturedEssence graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x0; // Default hue (eye color)
    }

    public override string DefaultName => "cyclops eye";
    public override double DefaultWeight => 1.0;
}

#endregion

#region Despise - Environmental Resources

[SerializationGenerator(0, false)]
public partial class CaveMushrooms : Item
{
    [Constructible]
    public CaveMushrooms(int amount = 1) : base(0x3191) // Placeholder for later adjustment with unique ID and art (using LuminescentFungi graphic)
    {
        Stackable = true;
        Amount = amount;
    }

    public override string DefaultName => "cave mushrooms";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class MountainHerbs : Item
{
    [Constructible]
    public MountainHerbs(int amount = 1) : base(0x3190) // Placeholder for later adjustment with unique ID and art (using ParasiticPlant graphic)
    {
        Stackable = true;
        Amount = amount;
    }

    public override string DefaultName => "mountain herbs";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class ElementalDust : Item
{
    [Constructible]
    public ElementalDust(int amount = 1) : base(0x3183) // Placeholder for later adjustment with unique ID and art (using Blight graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x96D; // Earth tone hue
    }

    public override string DefaultName => "elemental dust";
    public override double DefaultWeight => 0.1;
}

#endregion

// ========================================
// SHAME DUNGEON INGREDIENTS
// ========================================

#region Shame - Fishing Ingredients

[SerializationGenerator(0, false)]
public partial class ElementalMackerel : Item
{
    [Constructible]
    public ElementalMackerel(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x5; // Elemental purple hue
    }

    public override string DefaultName => "elemental mackerel";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class CoastalCrab : Item
{
    [Constructible]
    public CoastalCrab(int amount = 1) : base(0x09F1) // Placeholder for later adjustment with unique ID and art (using RawRibs graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x8B0; // Coastal blue hue
    }

    public override string DefaultName => "coastal crab";
    public override double DefaultWeight => 0.2;
}

[SerializationGenerator(0, false)]
public partial class DeepOceanTuna : Item
{
    [Constructible]
    public DeepOceanTuna(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x8A5; // Deep ocean hue
    }

    public override string DefaultName => "deep ocean tuna";
    public override double DefaultWeight => 0.2;
}

[SerializationGenerator(0, false)]
public partial class MagmaFish : Item
{
    [Constructible]
    public MagmaFish(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x2B; // Magma red hue
    }

    public override string DefaultName => "magma fish";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class PrismaticEel : Item
{
    [Constructible]
    public PrismaticEel(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x2B2; // Prismatic multicolor hue
    }

    public override string DefaultName => "prismatic eel";
    public override double DefaultWeight => 0.1;
}

#endregion

#region Shame - External Monster Drops

[SerializationGenerator(0, false)]
public partial class DireWolfFang : Item
{
    [Constructible]
    public DireWolfFang(int amount = 1) : base(0x318C) // Placeholder for later adjustment with unique ID and art (using GrizzledBones graphic)
    {
        Stackable = true;
        Amount = amount;
    }

    public override string DefaultName => "dire wolf feral fangs";
    public override double DefaultWeight => 0.5;
}

[SerializationGenerator(0, false)]
public partial class WildBoarEssence : Item
{
    [Constructible]
    public WildBoarEssence(int amount = 1) : base(0x318E) // Placeholder for later adjustment with unique ID and art (using CapturedEssence graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x7D1; // Boar essence hue
    }

    public override string DefaultName => "wild boar beast essence";
    public override double DefaultWeight => 1.0;
}

[SerializationGenerator(0, false)]
public partial class ScorpionVenomGland : Item
{
    [Constructible]
    public ScorpionVenomGland(int amount = 1) : base(0x3188) // Placeholder for later adjustment with unique ID and art (using Muculent graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x44; // Venom green hue
    }

    public override string DefaultName => "scorpion venom glands";
    public override double DefaultWeight => 0.5;
}

[SerializationGenerator(0, false)]
public partial class KrakenTentacle : Item
{
    [Constructible]
    public KrakenTentacle(int amount = 1) : base(0x09F1) // Placeholder for later adjustment with unique ID and art (using RawRibs graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x8B0; // Kraken blue hue
    }

    public override string DefaultName => "kraken tentacle";
    public override double DefaultWeight => 3.0;
}

[SerializationGenerator(0, false)]
public partial class PhoenixFeather : Item
{
    [Constructible]
    public PhoenixFeather(int amount = 1) : base(0x318A) // Placeholder for later adjustment with unique ID and art (using DreadHornMane graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x2B; // Phoenix fire hue
    }

    public override string DefaultName => "phoenix feather";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class BloodElementalCore : Item
{
    [Constructible]
    public BloodElementalCore(int amount = 1) : base(0x318E) // Placeholder for later adjustment with unique ID and art (using CapturedEssence graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x25; // Blood red hue
    }

    public override string DefaultName => "blood elemental core";
    public override double DefaultWeight => 1.0;
}

#endregion

#region Shame - Environmental Resources

[SerializationGenerator(0, false)]
public partial class VolcanicAsh : Item
{
    [Constructible]
    public VolcanicAsh(int amount = 1) : base(0x3183) // Placeholder for later adjustment with unique ID and art (using Blight graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x455; // Ash grey hue
    }

    public override string DefaultName => "volcanic ash";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class DesertCacti : Item
{
    [Constructible]
    public DesertCacti(int amount = 1) : base(0x3190) // Placeholder for later adjustment with unique ID and art (using ParasiticPlant graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x3F; // Cactus green hue
    }

    public override string DefaultName => "desert cacti";
    public override double DefaultWeight => 0.5;
}

[SerializationGenerator(0, false)]
public partial class SeaSaltCrystals : Item
{
    [Constructible]
    public SeaSaltCrystals(int amount = 1) : base(0x3194) // Placeholder for later adjustment with unique ID and art (using PerfectEmerald graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x0; // White salt hue
    }

    public override string DefaultName => "sea salt crystals";
    public override double DefaultWeight => 0.2;
}

[SerializationGenerator(0, false)]
public partial class SulfurDeposits : Item
{
    [Constructible]
    public SulfurDeposits(int amount = 1) : base(0x3183) // Placeholder for later adjustment with unique ID and art (using Blight graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x31; // Sulfur yellow hue
    }

    public override string DefaultName => "sulfur deposits";
    public override double DefaultWeight => 0.5;
}

[SerializationGenerator(0, false)]
public partial class ElementalEssence : Item
{
    [Constructible]
    public ElementalEssence(int amount = 1) : base(0x318E) // Placeholder for later adjustment with unique ID and art (using CapturedEssence graphic)
    {
        Stackable = true;
        Amount = amount;
    }

    public override string DefaultName => "elemental essence";
    public override double DefaultWeight => 0.1;
}

#endregion

// ========================================
// WRONG DUNGEON INGREDIENTS
// ========================================

#region Wrong - Fishing Ingredients

[SerializationGenerator(0, false)]
public partial class CrystalBass : Item
{
    [Constructible]
    public CrystalBass(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x47E; // Crystal clear hue
    }

    public override string DefaultName => "crystal bass";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class ClearWaterTrout : Item
{
    [Constructible]
    public ClearWaterTrout(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x47F; // Clear water hue
    }

    public override string DefaultName => "clear water trout";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class StoneFish : Item
{
    [Constructible]
    public StoneFish(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x3D; // Stone grey hue
    }

    public override string DefaultName => "stone fish";
    public override double DefaultWeight => 0.2;
}

[SerializationGenerator(0, false)]
public partial class RadiantEel : Item
{
    [Constructible]
    public RadiantEel(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x480; // Radiant glow hue
    }

    public override string DefaultName => "radiant eel";
    public override double DefaultWeight => 0.1;
}

#endregion

#region Wrong - External Monster Drops

[SerializationGenerator(0, false)]
public partial class JukaCrystalShard : Item
{
    [Constructible]
    public JukaCrystalShard(int amount = 1) : base(0x3194) // Placeholder for later adjustment with unique ID and art (using PerfectEmerald graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x47E; // Crystal hue
    }

    public override string DefaultName => "juka crystal shard";
    public override double DefaultWeight => 0.5;
}

[SerializationGenerator(0, false)]
public partial class GolemStoneHeart : Item
{
    [Constructible]
    public GolemStoneHeart(int amount = 1) : base(0x3184) // Placeholder for later adjustment with unique ID and art (using Corruption graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x3D; // Stone grey hue
    }

    public override string DefaultName => "golem stone heart";
    public override double DefaultWeight => 2.0;
}

[SerializationGenerator(0, false)]
public partial class BrigandStolenTools : Item
{
    [Constructible]
    public BrigandStolenTools(int amount = 1) : base(0x318F) // Placeholder for later adjustment with unique ID and art (using BarkFragment graphic)
    {
        Stackable = true;
        Amount = amount;
    }

    public override string DefaultName => "stolen tools";
    public override double DefaultWeight => 1.0;
}

[SerializationGenerator(0, false)]
public partial class JukaScoutShard : Item
{
    [Constructible]
    public JukaScoutShard(int amount = 1) : base(0x3194) // Placeholder for later adjustment with unique ID and art (using PerfectEmerald graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x47F; // Scout crystal hue
    }

    public override string DefaultName => "juka scout shard";
    public override double DefaultWeight => 0.3;
}

[SerializationGenerator(0, false)]
public partial class JukaLordCrystalCore : Item
{
    [Constructible]
    public JukaLordCrystalCore(int amount = 1) : base(0x318E) // Placeholder for later adjustment with unique ID and art (using CapturedEssence graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x47E; // Lord crystal hue
    }

    public override string DefaultName => "juka lord crystal core";
    public override double DefaultWeight => 1.5;
}

#endregion

#region Wrong - Environmental Resources

[SerializationGenerator(0, false)]
public partial class MountainQuartz : Item
{
    [Constructible]
    public MountainQuartz(int amount = 1) : base(0x3194) // Placeholder for later adjustment with unique ID and art (using PerfectEmerald graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x0; // Clear quartz hue
    }

    public override string DefaultName => "mountain quartz";
    public override double DefaultWeight => 0.5;
}

[SerializationGenerator(0, false)]
public partial class SharpeningStone : Item
{
    [Constructible]
    public SharpeningStone(int amount = 1) : base(0x318B) // Placeholder for later adjustment with unique ID and art (using DiseasedBark graphic)
    {
        Stackable = true;
        Amount = amount;
    }

    public override string DefaultName => "sharpening stone";
    public override double DefaultWeight => 1.0;
}

[SerializationGenerator(0, false)]
public partial class DiamondDust : Item
{
    [Constructible]
    public DiamondDust(int amount = 1) : base(0x3183) // Placeholder for later adjustment with unique ID and art (using Blight graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x47E; // Diamond sparkle hue
    }

    public override string DefaultName => "diamond dust";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class ElementalCrystal : Item
{
    [Constructible]
    public ElementalCrystal(int amount = 1) : base(0x3194) // Placeholder for later adjustment with unique ID and art (using PerfectEmerald graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x2B2; // Elemental multicolor hue
    }

    public override string DefaultName => "elemental crystal";
    public override double DefaultWeight => 0.5;
}

#endregion

// ========================================
// DESTARD DUNGEON INGREDIENTS
// ========================================

#region Destard - Fishing Ingredients

[SerializationGenerator(0, false)]
public partial class VolcanicTuna : Item
{
    [Constructible]
    public VolcanicTuna(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x2B; // Volcanic fire hue
    }

    public override string DefaultName => "volcanic tuna";
    public override double DefaultWeight => 0.2;
}

[SerializationGenerator(0, false)]
public partial class LavaFish : Item
{
    [Constructible]
    public LavaFish(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x2B; // Lava red hue
    }

    public override string DefaultName => "lava fish";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class AncientDeepSeaBass : Item
{
    [Constructible]
    public AncientDeepSeaBass(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x8B0; // Ancient deep hue
    }

    public override string DefaultName => "ancient deep sea bass";
    public override double DefaultWeight => 0.3;
}

[SerializationGenerator(0, false)]
public partial class PrimordialEel : Item
{
    [Constructible]
    public PrimordialEel(int amount = 1) : base(0x097A) // Placeholder for later adjustment with unique ID and art (using RawFishSteak graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x455; // Primordial dark hue
    }

    public override string DefaultName => "primordial eel";
    public override double DefaultWeight => 0.2;
}

#endregion

#region Destard - External Monster Drops

[SerializationGenerator(0, false)]
public partial class DrakeLesserScale : Item
{
    [Constructible]
    public DrakeLesserScale(int amount = 1) : base(0x318F) // Placeholder for later adjustment with unique ID and art (using BarkFragment graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x2B; // Drake red hue
    }

    public override string DefaultName => "drake lesser dragon scale";
    public override double DefaultWeight => 1.0;
}

[SerializationGenerator(0, false)]
public partial class WyvernWingMembrane : Item
{
    [Constructible]
    public WyvernWingMembrane(int amount = 1) : base(0x3188) // Placeholder for later adjustment with unique ID and art (using Muculent graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x8B0; // Wyvern wing hue
    }

    public override string DefaultName => "wyvern wing membrane";
    public override double DefaultWeight => 0.5;
}

[SerializationGenerator(0, false)]
public partial class GiantSerpentVenomSac : Item
{
    [Constructible]
    public GiantSerpentVenomSac(int amount = 1) : base(0x3188) // Placeholder for later adjustment with unique ID and art (using Muculent graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x44; // Serpent venom green hue
    }

    public override string DefaultName => "giant serpent venom sac";
    public override double DefaultWeight => 0.8;
}

[SerializationGenerator(0, false)]
public partial class DrakeScale : Item
{
    [Constructible]
    public DrakeScale(int amount = 1) : base(0x318F) // Placeholder for later adjustment with unique ID and art (using BarkFragment graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x2B; // Drake scale hue
    }

    public override string DefaultName => "drake scale";
    public override double DefaultWeight => 1.5;
}

[SerializationGenerator(0, false)]
public partial class ShadowWyrmScale : Item
{
    [Constructible]
    public ShadowWyrmScale(int amount = 1) : base(0x318F) // Placeholder for later adjustment with unique ID and art (using BarkFragment graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x455; // Shadow wyrm dark hue
    }

    public override string DefaultName => "shadow wyrm scale";
    public override double DefaultWeight => 2.0;
}

[SerializationGenerator(0, false)]
public partial class AncientWyrmHeart : Item
{
    [Constructible]
    public AncientWyrmHeart(int amount = 1) : base(0x318E) // Placeholder for later adjustment with unique ID and art (using CapturedEssence graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x2B; // Ancient wyrm fire hue
    }

    public override string DefaultName => "ancient wyrm heart";
    public override double DefaultWeight => 5.0;
}

#endregion

#region Destard - Environmental Resources

[SerializationGenerator(0, false)]
public partial class DragonBloodMoss : Item
{
    [Constructible]
    public DragonBloodMoss(int amount = 1) : base(0x3190) // Placeholder for later adjustment with unique ID and art (using ParasiticPlant graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x25; // Dragon blood hue
    }

    public override string DefaultName => "dragon blood moss";
    public override double DefaultWeight => 0.1;
}

[SerializationGenerator(0, false)]
public partial class ObsidianOre : Item
{
    [Constructible]
    public ObsidianOre(int amount = 1) : base(0x3184) // Placeholder for later adjustment with unique ID and art (using Corruption graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x455; // Obsidian black hue
    }

    public override string DefaultName => "obsidian ore";
    public override double DefaultWeight => 2.0;
}

[SerializationGenerator(0, false)]
public partial class DraconicEssence : Item
{
    [Constructible]
    public DraconicEssence(int amount = 1) : base(0x318E) // Placeholder for later adjustment with unique ID and art (using CapturedEssence graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x2B; // Draconic fire hue
    }

    public override string DefaultName => "draconic essence";
    public override double DefaultWeight => 0.5;
}

[SerializationGenerator(0, false)]
public partial class PrimordialCrystal : Item
{
    [Constructible]
    public PrimordialCrystal(int amount = 1) : base(0x3194) // Placeholder for later adjustment with unique ID and art (using PerfectEmerald graphic)
    {
        Stackable = true;
        Amount = amount;
        Hue = 0x455; // Primordial dark crystal hue
    }

    public override string DefaultName => "primordial crystal";
    public override double DefaultWeight => 1.0;
}

#endregion
