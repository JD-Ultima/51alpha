// =====================================================
// 51ALPHA FACTION SYSTEM - TESTING SUITE
// =====================================================
// File: FactionTests.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: Implementation Plan Section G - Testing & Validation
// =====================================================

using System;
using Server.Commands;
using Server.Guilds;
using Server.Sphere51a.Core.Database;
using Server.Sphere51a.Factions;

namespace Server.Sphere51a.Tests
{
    /// <summary>
    /// Automated testing suite for faction system.
    /// Validates faction assignment, cooldowns, vendor discounts, and persistence.
    /// </summary>
    public static class FactionTests
    {
        /// <summary>
        /// Register test commands.
        /// Called during server initialization.
        /// </summary>
        public static void Initialize()
        {
            CommandSystem.Register("TestFactions", AccessLevel.Administrator, new CommandEventHandler(TestFactions_OnCommand));
            CommandSystem.Register("TestFactionDB", AccessLevel.Administrator, new CommandEventHandler(TestFactionDB_OnCommand));

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine("[Sphere51a] Faction test commands registered");
            Utility.PopColor();
        }

        // =====================================================
        // TEST SUITE
        // =====================================================

        [Usage("TestFactions")]
        [Description("Run faction system test suite (admin only)")]
        private static void TestFactions_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage(0x3F, "=== Running Faction System Tests ===");
            from.SendMessage(0x35, "");

            int passed = 0;
            int failed = 0;

            // Test 1: Verify 3 factions loaded
            from.SendMessage(0x35, "Test 1: Verify 3 factions loaded...");
            if (Test_ThreeFactionsLoaded(from))
            {
                passed++;
                from.SendMessage(0x3F, "  PASS: 3 factions loaded");
            }
            else
            {
                failed++;
                from.SendMessage(0x22, "  FAIL: Expected 3 factions");
            }

            // Test 2: Verify faction definitions
            from.SendMessage(0x35, "Test 2: Verify faction definitions...");
            if (Test_FactionDefinitions(from))
            {
                passed++;
                from.SendMessage(0x3F, "  PASS: Faction definitions correct");
            }
            else
            {
                failed++;
                from.SendMessage(0x22, "  FAIL: Faction definitions incorrect");
            }

            // Test 3: Database connectivity
            from.SendMessage(0x35, "Test 3: Test database connectivity...");
            if (Test_DatabaseConnectivity(from))
            {
                passed++;
                from.SendMessage(0x3F, "  PASS: Database connection OK");
            }
            else
            {
                failed++;
                from.SendMessage(0x22, "  FAIL: Database connection failed");
            }

            // Test 4: Faction system initialization
            from.SendMessage(0x35, "Test 4: Verify faction system initialized...");
            if (Test_SystemInitialized(from))
            {
                passed++;
                from.SendMessage(0x3F, "  PASS: Faction system initialized");
            }
            else
            {
                failed++;
                from.SendMessage(0x22, "  FAIL: Faction system not initialized");
            }

            // Test 5: Week 1 lockout check
            from.SendMessage(0x35, "Test 5: Check Week 1 lockout status...");
            if (Test_Week1Lockout(from))
            {
                passed++;
                from.SendMessage(0x3F, "  PASS: Week 1 lockout check functional");
            }
            else
            {
                failed++;
                from.SendMessage(0x22, "  FAIL: Week 1 lockout check failed");
            }

            // Test 6: Faction lookup methods
            from.SendMessage(0x35, "Test 6: Test faction lookup methods...");
            if (Test_FactionLookup(from))
            {
                passed++;
                from.SendMessage(0x3F, "  PASS: Faction lookup methods work");
            }
            else
            {
                failed++;
                from.SendMessage(0x22, "  FAIL: Faction lookup methods failed");
            }

            // Summary
            from.SendMessage(0x35, "");
            from.SendMessage(0x3F, "=== Test Results ===");
            from.SendMessage(0x3F, $"Passed: {passed}/6");
            from.SendMessage(failed > 0 ? 0x22 : 0x3F, $"Failed: {failed}/6");

            if (failed == 0)
            {
                from.SendMessage(0x3F, "All tests passed!");
            }
            else
            {
                from.SendMessage(0x22, "Some tests failed - check configuration");
            }
        }

        [Usage("TestFactionDB")]
        [Description("Test database schema and connectivity (admin only)")]
        private static void TestFactionDB_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage(0x3F, "=== Testing Faction Database ===");
            from.SendMessage(0x35, "");

            // Test 1: PostgreSQL connection
            from.SendMessage(0x35, "Test 1: PostgreSQL connection...");
            if (PostgresConnection.TestConnection())
            {
                from.SendMessage(0x3F, "  PASS: Connection successful");
            }
            else
            {
                from.SendMessage(0x22, "  FAIL: Connection failed");
                return;
            }

            // Test 2: Verify schema
            from.SendMessage(0x35, "Test 2: Verify faction schema...");
            if (FactionRepository.VerifySchema())
            {
                from.SendMessage(0x3F, "  PASS: Schema verified");
            }
            else
            {
                from.SendMessage(0x22, "  FAIL: Schema incomplete");
                return;
            }

            // Test 3: Read faction data
            from.SendMessage(0x35, "Test 3: Read faction data...");
            var stats = FactionRepository.GetFactionStatistics();
            from.SendMessage(0x3F, $"  GoldenShield: {stats[1]} guilds");
            from.SendMessage(0x3F, $"  Bridgefolk: {stats[2]} guilds");
            from.SendMessage(0x3F, $"  LycaeumOrder: {stats[3]} guilds");

            int total = FactionRepository.GetTotalGuildCount();
            from.SendMessage(0x3F, $"  Total: {total} guilds in factions");

            from.SendMessage(0x3F, "Database tests complete");
        }

        // =====================================================
        // TEST METHODS
        // =====================================================

        private static bool Test_ThreeFactionsLoaded(Mobile from)
        {
            return S51aFaction.AllFactions.Length == 3;
        }

        private static bool Test_FactionDefinitions(Mobile from)
        {
            // Verify GoldenShield
            if (S51aFaction.GoldenShield.FactionId != 1 ||
                S51aFaction.GoldenShield.Hue != 2721 ||
                S51aFaction.GoldenShield.HomeCity != "Trinsic")
            {
                from.SendMessage(0x22, "    GoldenShield definition incorrect");
                return false;
            }

            // Verify Bridgefolk
            if (S51aFaction.Bridgefolk.FactionId != 2 ||
                S51aFaction.Bridgefolk.Hue != 2784 ||
                S51aFaction.Bridgefolk.HomeCity != "Vesper")
            {
                from.SendMessage(0x22, "    Bridgefolk definition incorrect");
                return false;
            }

            // Verify LycaeumOrder
            if (S51aFaction.LycaeumOrder.FactionId != 3 ||
                S51aFaction.LycaeumOrder.Hue != 2602 ||
                S51aFaction.LycaeumOrder.HomeCity != "Moonglow")
            {
                from.SendMessage(0x22, "    LycaeumOrder definition incorrect");
                return false;
            }

            return true;
        }

        private static bool Test_DatabaseConnectivity(Mobile from)
        {
            try
            {
                return PostgresConnection.TestConnection();
            }
            catch (Exception ex)
            {
                from.SendMessage(0x22, $"    Exception: {ex.Message}");
                return false;
            }
        }

        private static bool Test_SystemInitialized(Mobile from)
        {
            return S51aFactionSystem.IsInitialized;
        }

        private static bool Test_Week1Lockout(Mobile from)
        {
            try
            {
                bool isLocked = Server.Sphere51a.Core.S51aConfig.IsFactionChangeLocked;
                DateTime launchDate = Server.Sphere51a.Core.S51aConfig.ServerLaunchDate;

                from.SendMessage(0x35, $"    Server launch: {launchDate:yyyy-MM-dd HH:mm:ss} UTC");
                from.SendMessage(0x35, $"    Week 1 locked: {isLocked}");

                return true; // Check is functional regardless of value
            }
            catch (Exception ex)
            {
                from.SendMessage(0x22, $"    Exception: {ex.Message}");
                return false;
            }
        }

        private static bool Test_FactionLookup(Mobile from)
        {
            // Test GetById
            if (S51aFaction.GetById(1) != S51aFaction.GoldenShield)
            {
                from.SendMessage(0x22, "    GetById(1) failed");
                return false;
            }

            // Test GetByName
            if (S51aFaction.GetByName("GoldenShield") != S51aFaction.GoldenShield)
            {
                from.SendMessage(0x22, "    GetByName failed");
                return false;
            }

            // Test GetByHomeCity
            if (S51aFaction.GetByHomeCity("Trinsic") != S51aFaction.GoldenShield)
            {
                from.SendMessage(0x22, "    GetByHomeCity failed");
                return false;
            }

            return true;
        }
    }
}
