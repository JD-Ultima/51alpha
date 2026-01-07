namespace Server.Spells
{
    public enum DisturbType
    {
        Unspecified,
        EquipRequest,
        UseRequest,
        Hurt,
        Kill,
        NewCast,
        UseItem,        // Sphere51a: Using items like bandages
        WarModeChange   // Sphere51a: Toggling war mode
    }
}
