using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class ManaPotion : BaseManaPotion
{
    [Constructible]
    public ManaPotion() : base(PotionEffect.Mana)
    {
    }

    public override int MinMana => 20;
    public override int MaxMana => 20;
    public override double Delay => 10.0;
}
