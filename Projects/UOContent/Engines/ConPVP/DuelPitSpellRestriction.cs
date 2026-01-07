using Server.Mobiles;
using Server.Spells;
using System;
using System.Collections.Generic;

namespace Server.Engines.ConPVP
{
    public static class DuelPitSpellRestriction
    {
        private static readonly Dictionary<Mobile, DuelPitSession> ActiveDuelSessions = new Dictionary<Mobile, DuelPitSession>();

        public static void RegisterSession(DuelPitSession session)
        {
            if (!ActiveDuelSessions.ContainsKey(session.Challenger))
            {
                ActiveDuelSessions.Add(session.Challenger, session);
            }

            if (!ActiveDuelSessions.ContainsKey(session.Challenged))
            {
                ActiveDuelSessions.Add(session.Challenged, session);
            }
        }

        public static void UnregisterSession(DuelPitSession session)
        {
            ActiveDuelSessions.Remove(session.Challenger);
            ActiveDuelSessions.Remove(session.Challenged);
        }

        public static bool IsSpellAllowed(Mobile caster, Spell spell)
        {
            if (ActiveDuelSessions.TryGetValue(caster, out var session))
            {
                return session.IsSpellAllowed(caster, spell);
            }

            return true; // Not in a duel session, spell is allowed
        }

        // This method should be called from Mobile.CheckSpellCast override
        public static bool CheckDuelSpellRestriction(Mobile caster, Spell spell)
        {
            Console.WriteLine($"[DuelPit] CheckDuelSpellRestriction called - Caster: {caster?.Name}, Spell: {spell?.GetType().Name}");
            bool result = IsSpellAllowed(caster, spell);
            Console.WriteLine($"[DuelPit] CheckDuelSpellRestriction result: {result}");
            return result;
        }
    }
}
