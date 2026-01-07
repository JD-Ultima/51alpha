using System;
using Server.Mobiles;

namespace Server.Spells.Eighth
{
    public class WaterElementalSpell : MagerySpell, ITargetingSpell<IPoint3D>
    {
        private static readonly SpellInfo _info = new(
            "Water Elemental",
            "Kal Vas Xen An Flam",
            269,
            9070,
            false,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot,
            Reagent.SpidersSilk
        );

        public WaterElementalSpell(Mobile caster, Item scroll = null) : base(caster, scroll, _info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Eighth;

        public override bool CheckCast()
        {
            if (!base.CheckCast())
            {
                return false;
            }

            if (Caster.Followers + 3 > Caster.FollowersMax)
            {
                Caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return false;
            }

            return true;
        }

        public void Target(IPoint3D p)
        {
            var map = Caster.Map;

            SpellHelper.GetSurfaceTop(ref p);

            if (map?.CanSpawnMobile(p.X, p.Y, p.Z) != true)
            {
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (SpellHelper.CheckTown(p, Caster))
            {
                // Sphere51a: For player target-first spells, use delayed execution with callback
                if (Caster.Player)
                {
                    // Store the spell effect as a callback
                    SpellEffect = () =>
                    {
                        var duration = Core.Expansion switch
                        {
                            Expansion.None => TimeSpan.FromSeconds(Caster.Skills.Magery.Value),
                            // T2A -> Current
                            _  => TimeSpan.FromSeconds(4 * Math.Max(5, Caster.Skills.Magery.Value)),
                        };

                        BaseCreature.Summon(new SummonedWaterElemental(), false, Caster, new Point3D(p), 0x217, duration);
                    };

                    // Start validation and delay timer - spell executes after delay
                    // Note: State must remain "Casting" for timer to work correctly
                    CheckSequenceAndStartTimer();
                    return;
                }

                // NPC path: Original immediate execution
                if (CheckSequence())
                {
                    var duration = Core.Expansion switch
                    {
                        Expansion.None => TimeSpan.FromSeconds(Caster.Skills.Magery.Value),
                        // T2A -> Current
                        _  => TimeSpan.FromSeconds(4 * Math.Max(5, Caster.Skills.Magery.Value)),
                    };

                    BaseCreature.Summon(new SummonedWaterElemental(), false, Caster, new Point3D(p), 0x217, duration);
                }
            }
        }

        public override void OnCast()
        {
            Caster.Target = new SpellTarget<IPoint3D>(this, allowGround: true, retryOnLos: true);
        }
    }
}
