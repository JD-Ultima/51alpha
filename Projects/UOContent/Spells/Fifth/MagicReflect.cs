using System.Collections.Generic;
using ModernUO.CodeGeneratedEvents;
using Server.Engines.BuffIcons;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Fifth
{
    public class MagicReflectSpell : MagerySpell, ITargetingSpell<Mobile>
    {
        private static readonly SpellInfo _info = new(
            "Magic Reflection",
            "In Jux Sanct",
            242,
            9012,
            Reagent.Garlic,
            Reagent.MandrakeRoot,
            Reagent.SpidersSilk
        );

        private static readonly Dictionary<Mobile, ResistanceMod[]> _table = new();

        public MagicReflectSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Fifth;

        public override bool CheckCast()
        {
            if (Core.AOS)
            {
                return true;
            }

            if (Caster.MagicDamageAbsorb > 0)
            {
                Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
                return false;
            }

            if (!Caster.CanBeginAction<DefensiveSpell>())
            {
                Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
                return false;
            }

            return true;
        }

        public void Target(Mobile m)
        {
            if (Core.AOS)
            {
                /* The magic reflection spell decreases the caster's physical resistance, while increasing the caster's elemental resistances.
                 * Physical decrease = 25 - (Inscription/20).
                 * Elemental resistance = +10 (-20 physical, +10 elemental at GM Inscription)
                 * The magic reflection spell has an indefinite duration, becoming active when cast, and deactivated when re-cast.
                 * Reactive Armor, Protection, and Magic Reflection will stay on even after logging out,
                 * even after dying, until you turn them off by casting them again.
                 */

                // Sphere51a: Use callback pattern for delayed execution (players only)
                if (CheckBSequence(m, () =>
                {
                    if (_table.Remove(m, out var mods))
                    {
                        m.PlaySound(0x1ED);
                        m.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);

                        for (var i = 0; i < mods.Length; ++i)
                        {
                            m.RemoveResistanceMod(mods[i]);
                        }

                        (m as PlayerMobile)?.RemoveBuff(BuffIcon.MagicReflection);
                    }
                    else
                    {
                        m.PlaySound(0x1E9);
                        m.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);

                        var physiMod = -25 + (int)(Caster.Skills.Inscribe.Value / 20);
                        const int otherMod = 10;

                        mods =
                        [
                            new ResistanceMod(ResistanceType.Physical, "PhysicalResistMagicResist", physiMod),
                            new ResistanceMod(ResistanceType.Fire, "FireResistMagicResist", otherMod),
                            new ResistanceMod(ResistanceType.Cold, "ColdResistMagicResist", otherMod),
                            new ResistanceMod(ResistanceType.Poison, "PoisonResistMagicResist", otherMod),
                            new ResistanceMod(ResistanceType.Energy, "EnergyResistMagicResist", otherMod)
                        ];

                        _table[m] = mods;

                        for (var i = 0; i < mods.Length; ++i)
                        {
                            m.AddResistanceMod(mods[i]);
                        }

                        var buffFormat = $"{physiMod}\t+{otherMod}\t+{otherMod}\t+{otherMod}\t+{otherMod}";

                        (m as PlayerMobile)?.AddBuff(
                            new BuffInfo(BuffIcon.MagicReflection, 1075817, args: buffFormat, retainThroughDeath: true)
                        );
                    }
                }))
                {
                    // Validation succeeded
                    // For players: Timer started, execution deferred
                    // For NPCs: Already executed
                }
            }
            else
            {
                if (m.MagicDamageAbsorb > 0)
                {
                    Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
                }
                else if (!m.CanBeginAction<DefensiveSpell>())
                {
                    Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
                }
                // Sphere51a: Use callback pattern for delayed execution (players only)
                else if (CheckBSequence(m, () =>
                {
                    if (m.BeginAction<DefensiveSpell>())
                    {
                        var value = (int)(Caster.Skills.Magery.Value + Caster.Skills.Inscribe.Value);
                        value = (int)(8 + value / 200.0 * 7.0); // absorb from 8 to 15 "circles"

                        m.MagicDamageAbsorb = value;

                        m.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                        m.PlaySound(0x1E9);
                    }
                    else
                    {
                        Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
                    }
                }))
                {
                    // Validation succeeded
                    // For players: Timer started, execution deferred
                    // For NPCs: Already executed
                }
            }
        }

        public override void OnCast()
        {
            Caster.Target = new SpellTarget<Mobile>(this, TargetFlags.Beneficial);
        }

        [OnEvent(nameof(PlayerMobile.PlayerDeletedEvent))]
        public static void EndReflect(Mobile m)
        {
            if (!_table.Remove(m, out var mods))
            {
                return;
            }

            for (var i = 0; i < mods?.Length; ++i)
            {
                m.RemoveResistanceMod(mods[i]);
            }

            (m as PlayerMobile)?.RemoveBuff(BuffIcon.MagicReflection);
        }
    }
}
