using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.ConPVP.Gumps;
using System.Collections.Generic;

namespace Server.Engines.ConPVP
{
    // DuelPitController extends AddonComponent to work with DuelPitAddon
    // It acts as the interactive tombstone controller for duel pit sessions
    public class DuelPitController : AddonComponent
    {
        private static readonly Dictionary<Mobile, DuelPitSession> ActiveSessions = new Dictionary<Mobile, DuelPitSession>();
        private DuelPitRules _currentRules;

        public DuelPitController() : base(0xED4) // Tombstone item ID
        {
            Name = "Duel Pit Controller";
            Hue = 0x835; // Gray hue for tombstone
            _currentRules = new DuelPitRules();
        }

        public DuelPitController(Serial serial) : base(serial)
        {
        }

        public DuelPitRules CurrentRules
        {
            get => _currentRules;
            set => _currentRules = value ?? new DuelPitRules();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendMessage("You are too far away to use the duel pit controller.");
                return;
            }

            // Check if player is already in a session
            if (ActiveSessions.ContainsKey(from))
            {
                from.SendMessage("You are already in a duel session.");
                return;
            }

            // Open the duel pit configuration gump
            from.SendGump(new DuelPitGump(this));
        }

        public void StartDuelSession(Mobile challenger, Mobile challenged, DuelPitRules rules)
        {
            // Create a new duel session
            var session = new DuelPitSession(challenger, challenged, rules, this);
            ActiveSessions[challenger] = session;
            ActiveSessions[challenged] = session;

            // Send challenge to the opponent
            challenged.SendGump(new DuelPitChallengeGump(challenger, session));
            challenger.SendMessage($"Duel challenge sent to {challenged.Name}.");
        }

        public void CancelDuelSession(Mobile mobile)
        {
            if (ActiveSessions.TryGetValue(mobile, out var session))
            {
                ActiveSessions.Remove(mobile);

                // Remove both players from active sessions
                if (session.Challenger == mobile)
                {
                    ActiveSessions.Remove(session.Challenged);
                }
                else
                {
                    ActiveSessions.Remove(session.Challenger);
                }
            }
        }

        public void EndDuelSession(Mobile mobile)
        {
            // Simply remove the mobile from active sessions
            // This is called by DuelPitSession.EndDuel() to safely clean up
            ActiveSessions.Remove(mobile);
        }

        public static bool IsInDuelSession(Mobile mobile)
        {
            return ActiveSessions.ContainsKey(mobile);
        }

        public static DuelPitSession GetActiveSession(Mobile mobile)
        {
            return ActiveSessions.TryGetValue(mobile, out var session) ? session : null;
        }

        // Allow free reagent consumption for duel pit participants
        public static bool IsFreeConsume(Mobile mobile)
        {
            return IsInDuelSession(mobile);
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            // Initialize CurrentRules to default values after deserialization
            _currentRules = new DuelPitRules();
        }
    }

    public class DuelPitRules
    {
        public bool AllowPotions { get; set; } = true;
        public bool AllowBandages { get; set; } = true;
        public bool AllowMounts { get; set; } = true; // Everything allowed except polymorph/summons (blocked by spell system)
        public bool AllowSpellcasting { get; set; } = true;
        public bool AllowSpecialMoves { get; set; } = true;
        public int TimeLimitMinutes { get; set; } = 15;

        public string GetDescription()
        {
            return $"Potions: {(AllowPotions ? "Allowed" : "Banned")}, " +
                   $"Bandages: {(AllowBandages ? "Allowed" : "Banned")}, " +
                   $"Mounts: {(AllowMounts ? "Allowed" : "Banned")}, " +
                   $"Spellcasting: {(AllowSpellcasting ? "Allowed" : "Banned")}, " +
                   $"Special Moves: {(AllowSpecialMoves ? "Allowed" : "Banned")}, " +
                   $"Time Limit: {TimeLimitMinutes} minutes";
        }
    }
}
