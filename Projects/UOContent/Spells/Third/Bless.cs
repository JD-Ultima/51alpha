using Server.Engines.BuffIcons;
using Server.Engines.ConPVP;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Third
{
    public class BlessSpell : MagerySpell, ITargetingSpell<Mobile>
    {
        private static readonly SpellInfo _info = new(
            "Bless",
            "Rel Sanct",
            203,
            9061,
            Reagent.Garlic,
            Reagent.MandrakeRoot
        );

        public BlessSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Third;

        public void Target(Mobile m)
        {
            // Sphere51a: Use callback pattern for delayed execution (players only)
            if (CheckBSequence(m, () =>
            {
                SpellHelper.Turn(Caster, m);

                var length = SpellHelper.GetDuration(Caster, m);

                // Delegate to GlobalStatController
                GlobalStatController.ApplyGlobalModifier(Caster, m, +10, length);

                // Visual effects (ALWAYS play)
                m.FixedParticles(0x375A, 10, 15, 5018, EffectLayer.Waist);
                m.PlaySound(0x1EA);
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
