// =====================================================
// File: RemoveAllFCCommand.cs
// Created: 2025-12-15
// Phase: Phase 3 - Spell System
// Documentation: 51alpha_Spell_System_Specification.md
// =====================================================

using Server.Commands;
using Server.Items;

namespace Server.Sphere51a.Commands
{
    /// <summary>
    /// Admin command to remove Focus Cost (FC) from all items in the world.
    /// Sphere51a removes FC from spell casting for players, so existing FC on equipment is obsolete.
    /// </summary>
    public static class RemoveAllFCCommand
    {
        public static void Configure()
        {
            CommandSystem.Register("RemoveAllFC", AccessLevel.Administrator, RemoveAllFC_OnCommand);
        }

        [Usage("RemoveAllFC")]
        [Description("Removes Focus Cost (AosAttribute.CastSpeed) from all items in the world (admin only)")]
        private static void RemoveAllFC_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            int count = 0;

            from.SendMessage("Scanning all items for Focus Cost attribute...");

            foreach (var item in World.Items.Values)
            {
                AosAttributes attrs = null;

                if (item is BaseArmor armor)
                {
                    attrs = armor.Attributes;
                }
                else if (item is BaseWeapon weapon)
                {
                    attrs = weapon.Attributes;
                }
                else if (item is BaseJewel jewel)
                {
                    attrs = jewel.Attributes;
                }

                if (attrs != null && attrs.CastSpeed > 0)
                {
                    attrs.CastSpeed = 0;
                    count++;
                }
            }

            from.SendMessage($"Removed Focus Cost from {count} items.");
        }
    }
}
