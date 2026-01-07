using System;
using System.Collections.Generic;
using Server.Engines.BuffIcons;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Fourth
{
    public class CurseSpell : MagerySpell, ITargetingSpell<Mobile>
    {
        private static readonly SpellInfo _info = new(
            "Curse",
            "Des Sanct",
            227,
            9031,
            Reagent.Nightshade,
            Reagent.Garlic,
            Reagent.SulfurousAsh
        );


        public CurseSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Fourth;

        public static bool DoCurse(Mobile caster, Mobile m)
        {
            var duration = SpellHelper.GetDuration(caster, m);

            if (duration == TimeSpan.Zero)
            {
                return false;
            }

            // Delegate to GlobalStatController
            bool applied = GlobalStatController.ApplyGlobalModifier(caster, m, -10, duration);

            // Visual effects (ALWAYS play)
            m.FixedParticles(0x3779, 10, 15, 5028, EffectLayer.Waist);
            m.PlaySound(0x1E1);

            m.Spell?.OnCasterHurt();
            m.Paralyzed = false;

            return applied;
        }

        public void Target(Mobile m)
        {
            // Sphere51a: Use callback pattern for delayed execution (players only)
            if (CheckHSequence(m, () =>
            {
                SpellHelper.Turn(Caster, m);

                SpellHelper.CheckReflect((int)Circle, Caster, ref m);

                if (DoCurse(Caster, m))
                {
                    HarmfulSpell(m);
                }
                else
                {
                    DoHurtFizzle();
                }
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
