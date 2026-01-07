using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Spells.Fifth;

public class SummonCreatureGump : StaticGump<SummonCreatureGump>
{
    private readonly SummonCreatureSpell _spell;

    // Mount category
    private static readonly Type[] m_Mounts =
    {
        typeof(Horse),
        typeof(Llama)
    };

    // Other category
    private static readonly Type[] m_Others =
    {
        typeof(PolarBear),
        typeof(GrizzlyBear),
        typeof(BlackBear),
        typeof(Walrus),
        typeof(Chicken),
        typeof(Scorpion),
        typeof(GiantSerpent),
        typeof(Alligator),
        typeof(GreyWolf),
        typeof(Slime),
        typeof(Eagle),
        typeof(Gorilla),
        typeof(SnowLeopard),
        typeof(Pig),
        typeof(Hind),
        typeof(Rabbit)
    };

    public SummonCreatureGump(SummonCreatureSpell spell) : base(50, 50)
    {
        _spell = spell;
    }

    protected override void BuildLayout(ref StaticGumpBuilder builder)
    {
        builder.AddPage();
        builder.AddBackground(0, 0, 300, 200, 9200);
        builder.AddLabel(80, 20, 0, "Choose Creature Type");

        // Mount button
        builder.AddButton(50, 70, 4005, 4007, 1);
        builder.AddLabel(90, 70, 0, "Mount");

        // Other button
        builder.AddButton(50, 120, 4005, 4007, 2);
        builder.AddLabel(90, 120, 0, "Other");
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var caster = _spell.Caster;

        if (caster == null || caster.Deleted || !caster.Alive)
        {
            return;
        }

        Type[] selectedTypes = null;

        switch (info.ButtonID)
        {
            case 1: // Mount
                selectedTypes = m_Mounts;
                break;
            case 2: // Other
                selectedTypes = m_Others;
                break;
            default: // Closed gump or cancelled
                _spell.FinishSequence();
                return;
        }

        if (selectedTypes == null || selectedTypes.Length == 0)
        {
            _spell.FinishSequence();
            return;
        }

        // Store the selected creature types in the spell effect callback
        _spell.SpellEffect = () =>
        {
            try
            {
                var creature = selectedTypes.RandomElement().CreateInstance<BaseCreature>();

                var duration = Core.Expansion switch
                {
                    Expansion.None => TimeSpan.FromSeconds(caster.Skills.Magery.Value),
                    _ => TimeSpan.FromSeconds((int)caster.Skills.Magery.Value * 4)
                };

                SpellHelper.Summon(creature, caster, 0x215, duration, false, false);
            }
            catch
            {
                // ignored
            }
        };

        // Now start the casting delay and validation
        _spell.CheckSequenceAndStartTimer();
    }

    // Public accessor for combined types (for NPC use)
    public static Type[] GetAllTypes()
    {
        var allTypes = new Type[m_Mounts.Length + m_Others.Length];
        m_Mounts.CopyTo(allTypes, 0);
        m_Others.CopyTo(allTypes, m_Mounts.Length);
        return allTypes;
    }
}
