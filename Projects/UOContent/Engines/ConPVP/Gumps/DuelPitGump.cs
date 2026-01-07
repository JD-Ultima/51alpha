using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;

namespace Server.Engines.ConPVP.Gumps
{
    public class DuelPitGump : Gump
    {
        private readonly DuelPitController _controller;

        public DuelPitGump(DuelPitController controller) : base(50, 50)
        {
            _controller = controller;

            Closable = true;
            Disposable = true;
            Draggable = true;
            Resizable = false;

            AddPage(0);

            // Use brighter background - gump 9200 is lighter than 9270
            AddBackground(0, 0, 300, 180, 9200);

            // Title
            AddHtml(20, 20, 260, 40, "<CENTER><BIG>1v1 DUEL</BIG></CENTER>", false, false);

            // Select Opponent button
            AddButton(75, 80, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(115, 80, 150, 20, "Select Opponent", false, false);

            // Quit button
            AddButton(75, 120, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtml(115, 120, 150, 20, "Quit", false, false);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (sender.Mobile is not Mobile from)
                return;

            switch (info.ButtonID)
            {
                case 0: // Quit - just close
                    break;

                case 1: // Select Opponent
                    from.SendMessage("Select the player you wish to challenge.");
                    // Use default rules (everything allowed except polymorph/summons)
                    from.Target = new DuelPitTarget(_controller);
                    break;
            }
        }
    }

    public class DuelPitTarget : Target
    {
        private readonly DuelPitController _controller;

        public DuelPitTarget(DuelPitController controller) : base(12, false, TargetFlags.None)
        {
            _controller = controller;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Mobile target && target != from && target.Player)
            {
                // Check if target is already in a duel
                if (DuelPitController.IsInDuelSession(target))
                {
                    from.SendMessage($"{target.Name} is already in a duel.");
                    return;
                }

                // Check if target is too far away
                if (!from.InRange(target.Location, 18))
                {
                    from.SendMessage("Your opponent is too far away.");
                    return;
                }

                // Use default rules from controller
                _controller.StartDuelSession(from, target, _controller.CurrentRules);
            }
            else
            {
                from.SendMessage("You can only challenge other players.");
            }
        }
    }
}
