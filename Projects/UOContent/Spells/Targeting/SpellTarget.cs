using Server.Targeting;

namespace Server.Spells;

public class SpellTarget<T> : Target, ISpellTarget<T> where T : class, IPoint3D
{
    private static readonly bool _canTargetStatic = typeof(T).IsAssignableFrom(typeof(StaticTarget));
    private static readonly bool _canTargetMobile = typeof(T).IsAssignableFrom(typeof(Mobile));
    private static readonly bool _canTargetItem = typeof(T).IsAssignableFrom(typeof(Item));

    private readonly bool _retryOnLos;
    protected readonly ITargetingSpell<T> _spell;

    public SpellTarget(
        ITargetingSpell<T> spell,
        TargetFlags flags,
        bool retryOnLos = false
    ) : this(spell, false, flags, retryOnLos)
    {
    }

    public SpellTarget(
        ITargetingSpell<T> spell,
        bool allowGround = false,
        TargetFlags flags = TargetFlags.None,
        bool retryOnLos = false
    ) : base(spell.TargetRange, allowGround, flags)
    {
        _spell = spell;
        _retryOnLos = retryOnLos;
    }

    public ITargetingSpell<T> Spell => _spell;

    protected override bool CanTarget(Mobile from, StaticTarget staticTarget, ref Point3D loc, ref Map map)
        => base.CanTarget(from, staticTarget, ref loc, ref map) && _canTargetStatic;

    protected override bool CanTarget(Mobile from, Mobile mobile, ref Point3D loc, ref Map map) =>
        base.CanTarget(from, mobile, ref loc, ref map) && _canTargetMobile;

    protected override bool CanTarget(Mobile from, Item item, ref Point3D loc, ref Map map) =>
        base.CanTarget(from, item, ref loc, ref map) && _canTargetItem;

    protected override void OnCantSeeTarget(Mobile from, object o)
    {
        from.SendLocalizedMessage(500237); // Target can not be seen.
    }

    protected override void OnTarget(Mobile from, object o) => _spell.Target(o as T);

    protected override void OnTargetOutOfLOS(Mobile from, object o)
    {
        if (!_retryOnLos)
        {
            return;
        }

        from.SendLocalizedMessage(501943); // Target cannot be seen. Try again.
        from.Target = new SpellTarget<T>(_spell, AllowGround, Flags, true);
        from.Target.BeginTimeout(from, TimeoutTime - Core.TickCount);
    }

    protected override void OnTargetFinish(Mobile from)
    {
        // Sphere51a: For player spells, check if spell is still the active spell
        // If not, it means a new spell was selected - DON'T abort the old spell
        if (_spell is Spell spell && from.Player)
        {
            // If this spell is still the active spell on the caster, clean it up
            // Otherwise, a new spell has replaced it - leave the old spell alone
            if (from.Spell == _spell)
            {
                // Only finish if delay timer is NOT running
                if (!spell.HasDelayTimerRunning)
                {
                    _spell.FinishSequence();
                }
            }
            // If from.Spell != _spell, a new spell was selected
            // The old spell remains in Casting state with target cursor
            // It will be interrupted later when the new spell commits resources
            return;
        }

        // Original behavior for NPCs or non-delayed spells
        _spell?.FinishSequence();
    }
}
