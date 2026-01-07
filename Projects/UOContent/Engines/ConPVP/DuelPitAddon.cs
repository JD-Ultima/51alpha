using ModernUO.Serialization;
using Server.Items;

namespace Server.Engines.ConPVP
{
    [SerializationGenerator(0, false)]
    public partial class DuelPitAddon : BaseAddon
    {
        private DuelPitController _controller;

        [Constructible]
        public DuelPitAddon()
        {
            CreateWalls();
            CreateFloor();

            // Controller on NORTH side (1 tile outside north wall)
            _controller = new DuelPitController();
            AddComponent(_controller, 0, -6, 0);
        }

        private void CreateWalls()
        {
            // 10x10 perimeter from -5 to +5 on both X and Y
            // Skip certain corner tiles to prevent protrusion
            for (int x = -5; x <= 5; x++)
            {
                for (int y = -5; y <= 5; y++)
                {
                    // Only create walls on perimeter
                    if (x == -5 || x == 5 || y == -5 || y == 5)
                    {
                        // Skip the NW, SW corner tiles (remove 1 tile from N, S, W corners)
                        if ((x == -5 && y == -5) || (x == -5 && y == 5))
                            continue;

                        int wallId = GetWallId(x, y);
                        AddComponent(new AddonComponent(wallId), x, y, 0);
                    }
                }
            }
        }

        private int GetWallId(int x, int y)
        {
            // Corners use 0x41
            if ((x == -5 && y == -5) || (x == -5 && y == 5) ||
                (x == 5 && y == -5) || (x == 5 && y == 5))
                return 0x41; // Corner wall

            // X-axis walls (north and south) use 0x42
            if (y == -5 || y == 5)
                return 0x42; // Wall X

            // Y-axis walls (west and east) use 0x43
            return 0x43; // Wall Y
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

            // Add extra floor row on NE wall (x=5, y=-4 to 4)
            for (int y = -4; y <= 4; y++)
            {
                AddComponent(new AddonComponent(0x520), 5, y, 0);
            }

            // Add extra floor row on SE wall (x=5, y=-4 to 4) - already covered above
            // This extends the floor 1 tile east
        }

        public override BaseAddonDeed Deed => new DuelPitDeed();
    }

    [SerializationGenerator(0, false)]
    public partial class DuelPitDeed : BaseAddonDeed
    {
        [Constructible]
        public DuelPitDeed()
        {
        }

        public override BaseAddon Addon => new DuelPitAddon();
        public override int LabelNumber => 1041000; // duel pit
    }
}
