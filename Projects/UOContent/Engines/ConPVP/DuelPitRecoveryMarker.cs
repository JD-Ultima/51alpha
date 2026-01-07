using ModernUO.Serialization;
using System.Collections.Generic;

namespace Server.Engines.ConPVP
{
    /// <summary>
    /// Recovery marker placed in player's backpack during duel to enable recovery after server crash/restart.
    /// Stores original location, equipment, and opponent information for automatic restoration on login.
    /// </summary>
    [SerializationGenerator(0)]
    public partial class DuelPitRecoveryMarker : Item
    {
        [SerializableField(0)]
        [SerializedCommandProperty(AccessLevel.GameMaster)]
        private Point3D _originalLocation;

        [SerializableField(1)]
        [SerializedCommandProperty(AccessLevel.GameMaster)]
        private Map _originalMap;

        [SerializableField(2)]
        private Serial _opponentSerial;

        [SerializableField(3)]
        private bool _isChallenger;

        [SerializableField(4)]
        private List<Item> _savedEquipment;

        [SerializableField(5)]
        private List<Item> _savedBackpackItems;

        [Constructible]
        public DuelPitRecoveryMarker() : base(0x1)
        {
            Visible = false;
            Movable = false;
            LootType = LootType.Blessed;
            Name = "DuelPit Recovery Marker (System Item)";
            _savedEquipment = new List<Item>();
            _savedBackpackItems = new List<Item>();
        }

        public DuelPitRecoveryMarker(
            Point3D location,
            Map map,
            Mobile opponent,
            bool isChallenger,
            List<Item> equipment,
            List<Item> backpackItems) : base(0x1)
        {
            _originalLocation = location;
            _originalMap = map;
            _opponentSerial = opponent?.Serial ?? Serial.MinusOne;
            _isChallenger = isChallenger;
            _savedEquipment = new List<Item>(equipment ?? new List<Item>());
            _savedBackpackItems = new List<Item>(backpackItems ?? new List<Item>());

            Visible = false;
            Movable = false;
            LootType = LootType.Blessed;
            Name = "DuelPit Recovery Marker (System Item)";
        }

        // Prevent deletion/dropping by players
        public override bool OnDroppedToWorld(Mobile from, Point3D location) => false;
        public override bool OnDroppedToMobile(Mobile from, Mobile target) => false;
        public override void OnDoubleClick(Mobile from) { /* No action */ }
    }
}
