using ModernUO.Serialization;
using Server.Items;

namespace Server.Engines.ConPVP
{
    [SerializationGenerator(0, false)]
    public partial class DuelPitAddonWest : BaseAddon
    {
        private DuelPitController _controller;

        [Constructible]
        public DuelPitAddonWest()
        {
            CreateWalls();
            CreateFloor();

            // Controller on WEST side (1 tile outside west wall)
            _controller = new DuelPitController();
            AddComponent(_controller, -6, 0, 0);
        }

        private void CreateWalls()
        {
            // 10x10 perimeter from -5 to +5 on both X and Y
            // Match the East version: skip NE and SW corners
            for (int x = -5; x <= 5; x++)
            {
                for (int y = -5; y <= 5; y++)
                {
                    // Only create walls on perimeter
                    if (x == -5 || x == 5 || y == -5 || y == 5)
                    {
                        // Skip NE and SW corner tiles (match East version)
                        if ((x == 5 && y == -5) || (x == -5 && y == 5))
                            continue;

                        int wallId;

                        // SE corner uses 0x41
                        if (x == 5 && y == 5)
                            wallId = 0x41;
                        // NW corner uses 0x44
                        else if (x == -5 && y == -5)
                            wallId = 0x44;
                        // X-axis walls (north and south) use 0x42
                        else if (y == -5 || y == 5)
                            wallId = 0x42;
                        // Y-axis walls (west and east) use 0x43
                        else
                            wallId = 0x43;

                        AddComponent(new AddonComponent(wallId), x, y, 0);
                    }
                }
            }
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

            // Add floor tile at SE corner (5, 5)
            AddComponent(new AddonComponent(0x520), 5, 5, 0);

            // Add extra wall pieces to complete the pattern
            // West side extra wall piece
            AddComponent(new AddonComponent(0x43), -5, 5, 0);

            // North side extra wall piece
            AddComponent(new AddonComponent(0x42), 5, -5, 0);
        }

        public override BaseAddonDeed Deed => new DuelPitDeedWest();
    }

    [SerializationGenerator(0, false)]
    public partial class DuelPitDeedWest : BaseAddonDeed
    {
        [Constructible]
        public DuelPitDeedWest()
        {
        }

        public override BaseAddon Addon => new DuelPitAddonWest();
        public override int LabelNumber => 1041003; // duel pit (west)
    }
}
