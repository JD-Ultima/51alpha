using Server.Targeting;

namespace Server.Spells.Seventh
{
    public class FlameStrikeSpell : MagerySpell, ITargetingSpell<Mobile>
    {
        private static readonly SpellInfo _info = new(
            "Flame Strike",
            "Kal Vas Flam",
            245,
            9042,
            Reagent.SpidersSilk,
            Reagent.SulfurousAsh
        );

        public FlameStrikeSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Seventh;

        // Sphere51a: No additional delay - callback pattern handles timing
        public override bool DelayedDamage => false;

        public void Target(Mobile m)
        {
            // Sphere51a: Use callback pattern for delayed execution (players only)
            if (CheckHSequence(m, () =>
            {
                SpellHelper.Turn(Caster, m);

                SpellHelper.CheckReflect((int)Circle, Caster, ref m);

                double damage;

                if (Core.AOS)
                {
                    damage = GetNewAosDamage(48, 1, 5, m);
                }
                else
                {
                    damage = Utility.Random(27, 22);

                    if (CheckResisted(m))
                    {
                        damage *= 0.6;

                        m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

                    damage *= GetDamageScalar(m);
                }

                m.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                m.PlaySound(0x208);

                SpellHelper.Damage(this, m, damage, 0, 100, 0, 0, 0);
            }))
            {
                // Validation succeeded
                // For players: Timer started, execution deferred
                // For NPCs: Already executed
            }
        }

        public override void OnCast()
        {
            Caster.Target = new SpellTarget<Mobile>(this, TargetFlags.Harmful);
        }
    }
}
