using ModernUO.Serialization;
using Server.Targets;

namespace Server.Items
{
    [SerializationGenerator(0, false)]
    public abstract partial class BaseSword : BaseMeleeWeapon
    {
        public BaseSword(int itemID) : base(itemID)
        {
        }

        public override SkillName DefSkill => SkillName.Swords;
        public override WeaponType DefType => WeaponType.Slashing;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Slash1H;

        public override void OnDoubleClick(Mobile from)
        {
            // Check if Sphere51a equipment swap should apply (item in backpack or on ground within 2 tiles, NOT equipped on mobile)
            if (Server.Sphere51a.Core.S51aConfig.EquipmentSwapEnabled &&
                Parent != from &&
                (IsChildOf(from.Backpack) || (Parent == null && from.InRange(GetWorldLocation(), 2))))
            {
                // Use base equipment swap logic
                base.OnDoubleClick(from);
                return;
            }

            // Default bladed item behavior (when equipped on mobile, too far, or Sphere51a disabled)
            from.SendLocalizedMessage(1010018); // What do you want to use this item on?
            from.Target = new BladedItemTarget(this);
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus = 1)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (!Core.AOS && Poison != null && PoisonCharges > 0)
            {
                --PoisonCharges;

                if (Utility.RandomBool()) // 50% chance to poison
                {
                    defender.ApplyPoison(attacker, Poison);
                }
            }
        }
    }
}
