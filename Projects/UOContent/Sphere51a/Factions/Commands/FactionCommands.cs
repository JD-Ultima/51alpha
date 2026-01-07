// =====================================================
// 51ALPHA FACTION SYSTEM - PLAYER COMMANDS
// =====================================================
// File: FactionCommands.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: PHASE1_VERIFICATION_REPORT_v3.md (Q1: Faction System)
// =====================================================

using System;
using Server.Commands;
using Server.Guilds;

namespace Server.Sphere51a.Factions.Commands
{
    /// <summary>
    /// Player commands for faction system.
    /// Provides [FactionJoin], [FactionLeave], and [FactionInfo] commands.
    /// </summary>
    public static class FactionCommands
    {
        /// <summary>
        /// Register all faction commands.
        /// Called during server initialization.
        /// </summary>
        public static void Initialize()
        {
            CommandSystem.Register("FactionJoin", AccessLevel.Player, new CommandEventHandler(FactionJoin_OnCommand));
            CommandSystem.Register("FactionLeave", AccessLevel.Player, new CommandEventHandler(FactionLeave_OnCommand));
            CommandSystem.Register("FactionInfo", AccessLevel.Player, new CommandEventHandler(FactionInfo_OnCommand));

            // Admin commands
            CommandSystem.Register("FactionReload", AccessLevel.Administrator, new CommandEventHandler(FactionReload_OnCommand));
            CommandSystem.Register("FactionStats", AccessLevel.Administrator, new CommandEventHandler(FactionStats_OnCommand));

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine("[Sphere51a] Faction commands registered");
            Utility.PopColor();
        }

        // =====================================================
        // PLAYER COMMANDS
        // =====================================================

        [Usage("FactionJoin <faction>")]
        [Description("Join a faction (guild leader only). Factions: GoldenShield, Bridgefolk, LycaeumOrder")]
        private static void FactionJoin_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Check if player in guild
            var guild = from.Guild as Guild;
            if (guild == null)
            {
                from.SendMessage(0x22, "You must be in a guild to join a faction.");
                return;
            }

            // Check if guild leader
            if (guild.Leader != from)
            {
                from.SendMessage(0x22, "Only the guild leader can change faction affiliation.");
                return;
            }

            // Check arguments
            if (e.Length < 1)
            {
                from.SendMessage(0x35, "Usage: [FactionJoin <faction>");
                from.SendMessage(0x35, "Available factions:");
                from.SendMessage(0x35, "  - GoldenShield (Home: Trinsic)");
                from.SendMessage(0x35, "  - Bridgefolk (Home: Vesper)");
                from.SendMessage(0x35, "  - LycaeumOrder (Home: Moonglow)");
                return;
            }

            // Parse faction name
            string factionName = e.GetString(0);
            var faction = S51aFaction.GetByName(factionName);

            if (faction == null)
            {
                from.SendMessage(0x22, $"Invalid faction: {factionName}");
                from.SendMessage(0x35, "Valid factions: GoldenShield, Bridgefolk, LycaeumOrder");
                return;
            }

            // Attempt to join faction
            if (S51aFactionSystem.JoinFaction(guild, faction.Type, out string error))
            {
                from.SendMessage(0x3F, $"Your guild has joined {faction.Name}!");
                from.SendMessage(0x3F, $"Guild members receive 10% vendor discount in {faction.HomeCity}.");
            }
            else
            {
                from.SendMessage(0x22, $"Cannot join faction: {error}");
            }
        }

        [Usage("FactionLeave")]
        [Description("Leave your current faction (guild leader only)")]
        private static void FactionLeave_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Check if player in guild
            var guild = from.Guild as Guild;
            if (guild == null)
            {
                from.SendMessage(0x22, "You must be in a guild.");
                return;
            }

            // Check if guild leader
            if (guild.Leader != from)
            {
                from.SendMessage(0x22, "Only the guild leader can change faction affiliation.");
                return;
            }

            // Check if guild in a faction
            var currentFaction = guild.GetFaction();
            if (currentFaction == null)
            {
                from.SendMessage(0x22, "Your guild is not in any faction.");
                return;
            }

            // Attempt to leave faction
            if (S51aFactionSystem.LeaveFaction(guild, out string error))
            {
                from.SendMessage(0x3F, $"Your guild has left {currentFaction.Name}.");
            }
            else
            {
                from.SendMessage(0x22, $"Cannot leave faction: {error}");
            }
        }

        [Usage("FactionInfo")]
        [Description("Display faction information for your guild")]
        private static void FactionInfo_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Check if player in guild
            var guild = from.Guild as Guild;
            if (guild == null)
            {
                from.SendMessage(0x22, "You are not in a guild.");
                from.SendMessage(0x35, "Use [FactionJoin <faction> to join a faction after creating/joining a guild.");
                return;
            }

            // Get faction info
            var factionInfo = guild.GetFactionInfo();

            if (factionInfo == null)
            {
                from.SendMessage(0x35, "=== Guild Faction Status ===");
                from.SendMessage(0x35, $"Guild: {guild.Name}");
                from.SendMessage(0x35, "Faction: None");
                from.SendMessage(0x35, "");
                from.SendMessage(0x35, "Available factions:");
                from.SendMessage(0x35, "  - The Golden Shield (Home: Trinsic, Hue: Gold)");
                from.SendMessage(0x35, "  - The Bridgefolk (Home: Vesper, Hue: Blue)");
                from.SendMessage(0x35, "  - The Lycaeum Order (Home: Moonglow, Hue: Purple)");
                from.SendMessage(0x35, "");
                from.SendMessage(0x35, "Use [FactionJoin <faction> to join (guild leader only)");
                return;
            }

            // Display faction information
            from.SendMessage(0x3F, "=== Guild Faction Status ===");
            from.SendMessage(0x3F, $"Guild: {guild.Name}");
            from.SendMessage(0x3F, $"Faction: {factionInfo.Faction.Name}");
            from.SendMessage(0x3F, $"Home City: {factionInfo.Faction.HomeCity}");
            from.SendMessage(0x3F, $"Vendor Discount: 10% in {factionInfo.Faction.HomeCity}");
            from.SendMessage(0x3F, $"Joined: {factionInfo.JoinedAt:yyyy-MM-dd}");

            // Display cooldown status
            var cooldown = factionInfo.GetRemainingCooldown();
            if (cooldown.HasValue)
            {
                int days = cooldown.Value.Days;
                int hours = cooldown.Value.Hours;
                from.SendMessage(0x35, $"Cooldown: {days} day(s), {hours} hour(s) remaining");
                from.SendMessage(0x35, "Cannot change faction during cooldown");
            }
            else if (Server.Sphere51a.Core.S51aConfig.IsFactionChangeLocked)
            {
                var remaining = (Server.Sphere51a.Core.S51aConfig.ServerLaunchDate + TimeSpan.FromDays(7)) - DateTime.UtcNow;
                from.SendMessage(0x35, $"Week 1 Lockout: {remaining.Days} day(s), {remaining.Hours} hour(s) remaining");
                from.SendMessage(0x35, "Faction changes locked during first week after server launch");
            }
            else
            {
                from.SendMessage(0x3F, "Status: Can change faction now");
            }
        }

        // =====================================================
        // ADMIN COMMANDS
        // =====================================================

        [Usage("FactionReload")]
        [Description("Reload all faction data from database (admin only)")]
        private static void FactionReload_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage(0x35, "Reloading faction data from database...");
            S51aFactionSystem.ReloadAllFactions();
            from.SendMessage(0x3F, "Faction data reloaded successfully.");
        }

        [Usage("FactionStats")]
        [Description("Display faction statistics (admin only)")]
        private static void FactionStats_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage(0x3F, "=== Faction Statistics ===");

            var stats = S51aFactionSystem.GetStatistics();

            foreach (var kvp in stats)
            {
                from.SendMessage(0x3F, $"{kvp.Key.Name}: {kvp.Value} guild(s)");
            }

            int total = FactionRepository.GetTotalGuildCount();
            from.SendMessage(0x3F, $"Total: {total} guild(s) in factions");
        }
    }
}
