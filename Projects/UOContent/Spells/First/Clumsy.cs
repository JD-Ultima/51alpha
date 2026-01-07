using Server.Engines.BuffIcons;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.First
{
    public class ClumsySpell : MagerySpell, ITargetingSpell<Mobile>
    {
        private static readonly SpellInfo _info = new(
            "Clumsy",
            "Uus Jux",
            212,
            9031,
            Reagent.Bloodmoss,
            Reagent.Nightshade
        );

        public ClumsySpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
        {
        }

        public override SpellCircle Circle => SpellCircle.First;

        public void Target(Mobile m)
        {
            // Sphere51a: Use callback pattern for delayed execution (players only)
            if (CheckHSequence(m, () =>
            {
                SpellHelper.Turn(Caster, m);

                SpellHelper.CheckReflect((int)Circle, Caster, ref m);

                // Check with GlobalStatController
                if (!GlobalStatController.CanApplyIndividualCurse(Caster, m))
                {
                    Caster.SendMessage(0x22, "The target's enchantment negates your spell.");
                    // Visuals still play
                    m.FixedParticles(0x3779, 10, 15, 5002, EffectLayer.Head);
                    m.PlaySound(0x1DF);
                    return; // Don't apply StatMod
                }

                // Apply normally
                var length = SpellHelper.GetDuration(Caster, m);
                SpellHelper.AddStatCurse(Caster, m, StatType.Dex, length, false);

                m.Spell?.OnCasterHurt();

                m.Paralyzed = false;

                m.FixedParticles(0x3779, 10, 15, 5002, EffectLayer.Head);
                m.PlaySound(0x1DF);

                var percentage = (int)(SpellHelper.GetOffsetScalar(Caster, m, true) * 100);

                (m as PlayerMobile)?.AddBuff(new BuffInfo(BuffIcon.Clumsy, 1075831, length, percentage.ToString()));

                HarmfulSpell(m);
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
