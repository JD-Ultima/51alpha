using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.ConPVP.Gumps
{
    public class DuelPitChallengeGump : Gump
    {
        private readonly Mobile _challenger;
        private readonly DuelPitSession _session;

        public DuelPitChallengeGump(Mobile challenger, DuelPitSession session) : base(50, 50)
        {
            _challenger = challenger;
            _session = session;

            Closable = true;
            Disposable = true;
            Draggable = true;
            Resizable = false;

            AddPage(0);

            // Use brighter background - gump 9200 is lighter than 9270
            AddBackground(0, 0, 300, 160, 9200);

            // Title
            AddHtml(20, 20, 260, 40, "<CENTER><BIG>DUEL CHALLENGE</BIG></CENTER>", false, false);

            // Challenge message
            AddHtml(20, 60, 260, 20, $"<CENTER>{_challenger.Name} challenges you!</CENTER>", false, false);

            // Accept button
            AddButton(50, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(90, 100, 80, 20, "Accept", false, false);

            // Decline button
            AddButton(170, 100, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtml(210, 100, 80, 20, "Decline", false, false);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (sender.Mobile is not Mobile from)
                return;

            switch (info.ButtonID)
            {
                case 1: // Accept
                    _session.AcceptChallenge(from);
                    break;
                case 2: // Decline
                    _session.DeclineChallenge(from);
                    break;
            }
        }
    }
}
