using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class GreaterManaPotion : BaseManaPotion
{
    [Constructible]
    public GreaterManaPotion() : base(PotionEffect.ManaGreater)
    {
    }

    public override int MinMana => 40;
    public override int MaxMana => 40;
    public override double Delay => 10.0;
}
