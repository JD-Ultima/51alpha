using Server.Engines.BuffIcons;
using Server.Engines.ConPVP;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Second
{
    public class CunningSpell : MagerySpell, ITargetingSpell<Mobile>
    {
        private static readonly SpellInfo _info = new(
            "Cunning",
            "Uus Wis",
            212,
            9061,
            Reagent.MandrakeRoot,
            Reagent.Nightshade
        );

        public CunningSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Second;

        public void Target(Mobile m)
        {
            // Sphere51a: Use callback pattern for delayed execution (players only)
            if (CheckBSequence(m, () =>
            {
                SpellHelper.Turn(Caster, m);

                // Check with GlobalStatController
                if (!GlobalStatController.CanApplyIndividualBuff(Caster, m))
                {
                    Caster.SendMessage(0x22, "The target's enchantment negates your spell.");
                    // Visuals still play
                    m.FixedParticles(0x375A, 10, 15, 5011, EffectLayer.Head);
                    m.PlaySound(0x1EB);
                    return; // Don't apply StatMod
                }

                // Apply normally
                var length = SpellHelper.GetDuration(Caster, m);
                SpellHelper.AddStatBonus(Caster, m, StatType.Int, length, false);

                m.FixedParticles(0x375A, 10, 15, 5011, EffectLayer.Head);
                m.PlaySound(0x1EB);

                var percentage = (int)(SpellHelper.GetOffsetScalar(Caster, m, false) * 100);

                (m as PlayerMobile)?.AddBuff(new BuffInfo(BuffIcon.Cunning, 1075843, length, percentage.ToString()));
            }))
            {
                // Validation succeeded
                // For players: Timer started, execution deferred
                // For NPCs: Already executed
            }
        }

        public override bool CheckCast()
        {
            if (DuelContext.CheckSuddenDeath(Caster))
            {
                Caster.SendMessage(0x22, "You cannot cast this spell when in sudden death.");
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            Caster.Target = new SpellTarget<Mobile>(this, TargetFlags.Beneficial);
        }
    }
}
