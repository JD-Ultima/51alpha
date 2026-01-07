using ModernUO.Serialization;
using Server.Items;

namespace Server.Engines.ConPVP
{
    [SerializationGenerator(0, false)]
    public partial class DuelPitAddonEast : BaseAddon
    {
        private DuelPitController _controller;

        [Constructible]
        public DuelPitAddonEast()
        {
            CreateWalls();
            CreateFloor();

            // Controller on EAST side (1 tile outside east wall)
            _controller = new DuelPitController();
            AddComponent(_controller, 6, 0, 0);
        }

        private void CreateWalls()
        {
            // 10x10 perimeter from -5 to +5 on both X and Y
            for (int x = -5; x <= 5; x++)
            {
                for (int y = -5; y <= 5; y++)
                {
                    // Only create walls on perimeter
                    if (x == -5 || x == 5 || y == -5 || y == 5)
                    {
                        int wallId;

                        // SE corner gets special corner piece
                        if (x == 5 && y == 5)
                        {
                            wallId = 0x41; // SE corner
                        }
                        // NW corner gets special corner piece
                        else if (x == -5 && y == -5)
                        {
                            wallId = 0x44; // NW corner
                        }
                        // NE corner (5, -5) and SW corner (-5, 5) - skip, no corner pieces needed
                        else if ((x == 5 && y == -5) || (x == -5 && y == 5))
                        {
                            continue; // Skip these corners
                        }
                        // X-axis walls (north and south)
                        else if (y == -5 || y == 5)
                        {
                            wallId = 0x42; // X-axis walls
                        }
                        // Y-axis walls (west and east)
                        else
                        {
                            wallId = 0x43; // Y-axis walls
                        }

                        AddComponent(new AddonComponent(wallId), x, y, 0);
                    }
                }
            }

            // Add 1 extra Y-axis wall piece on west side going south (x=-5, y=6)
            AddComponent(new AddonComponent(0x43), -5, 5, 0);

            // Add 1 extra X-axis wall piece on north side going east (x=6, y=-5)
            AddComponent(new AddonComponent(0x42), 5, -5, 0);
        }

        private void CreateFloor()
        {
            // Interior floor from -4 to +4 using 0x520
            for (int x = -4; x <= 4; x++)
            {
                for (int y = -4; y <= 4; y++)
                {
                    AddComponent(new AddonComponent(0x520), x, y, 0);
                }
            }

            // Add extra floor row on east side (x=5, y=-4 to 4)
            for (int y = -4; y <= 4; y++)
            {
                AddComponent(new AddonComponent(0x520), 5, y, 0);
            }

            // Add extra floor row on south side (y=5, x=-4 to 4)
            for (int x = -4; x <= 4; x++)
            {
                AddComponent(new AddonComponent(0x520), x, 5, 0);
            }

            // Add 1 extra floor tile under SE corner (5, 5)
            AddComponent(new AddonComponent(0x520), 5, 5, 0);
        }

        public override BaseAddonDeed Deed => new DuelPitDeedEast();
    }

    [SerializationGenerator(0, false)]
    public partial class DuelPitDeedEast : BaseAddonDeed
    {
        [Constructible]
        public DuelPitDeedEast()
        {
        }

        public override BaseAddon Addon => new DuelPitAddonEast();
        public override int LabelNumber => 1041001; // duel pit (east)
    }
}
