// =====================================================
// 51ALPHA CORE - SYSTEM INITIALIZER
// =====================================================
// File: S51aInitializer.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q1: Architecture)
// =====================================================

using System;
using Server.Sphere51a.Commands;
using Server.Sphere51a.Factions;
using Server.Sphere51a.Tests;

namespace Server.Sphere51a.Core
{
    /// <summary>
    /// Sphere51a system initializer.
    /// Coordinates startup sequence for all Sphere51a subsystems.
    /// </summary>
    public static class S51aInitializer
    {
        private static bool _isConfigured = false;

        /// <summary>
        /// Configure Sphere51a event hooks.
        /// Called early in ModernUO startup sequence.
        /// </summary>
        public static void Configure()
        {
            if (_isConfigured)
            {
                Console.WriteLine("[Sphere51a] Already configured - skipping");
                return;
            }

            // Hook into ModernUO events
            EventSink.WorldLoad += OnWorldLoad;
            EventSink.WorldSave += OnWorldSave;
            EventSink.Logout += OnPlayerLogout;
            EventSink.Disconnected += OnPlayerDisconnected;

            // Configure authentication hooks
            S51aAuthenticationHooks.Configure();

            // Register Sphere51a commands
            RemoveAllFCCommand.Configure();

            _isConfigured = true;

            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine("[Sphere51a] Event hooks configured");
            Console.WriteLine("[Sphere51a] Commands registered");
            Utility.PopColor();
        }

        /// <summary>
        /// World load event handler.
        /// Initializes all Sphere51a systems after world data is loaded.
        /// </summary>
        private static void OnWorldLoad()
        {
            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine("========================================");
            Console.WriteLine("=== Sphere51a System Initialization ===");
            Console.WriteLine("========================================");
            Utility.PopColor();

            try
            {
                // Step 1: Load configuration
                Console.WriteLine("[Sphere51a] Step 1/4: Loading configuration...");
                S51aConfig.Initialize();

                // Step 2: Initialize faction system
                Console.WriteLine("[Sphere51a] Step 2/4: Initializing faction system...");
                S51aFactionSystem.Initialize();

                // Step 3: Initialize vendor discounts
                Console.WriteLine("[Sphere51a] Step 3/4: Initializing vendor discount system...");
                FactionVendorDiscount.Initialize();

                // Step 4: Register test commands
                Console.WriteLine("[Sphere51a] Step 4/4: Registering test commands...");
                FactionTests.Initialize();

                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine("========================================");
                Console.WriteLine("=== Sphere51a Initialization Complete ===");
                Console.WriteLine("========================================");
                Utility.PopColor();
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine("========================================");
                Console.WriteLine("=== Sphere51a Initialization FAILED ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                Console.WriteLine("========================================");
                Utility.PopColor();
            }
        }

        /// <summary>
        /// World save event handler.
        /// PostgreSQL uses write-through strategy, so no action needed on save.
        /// </summary>
        private static void OnWorldSave()
        {
            // Note: Faction data is already synced to PostgreSQL via write-through
            // Binary serialization is handled by ModernUO's existing save process
            // No additional action required here
        }

        /// <summary>
        /// Player logout event handler.
        /// Cleanup PvP damage tracking when player logs out.
        /// </summary>
        private static void OnPlayerLogout(Mobile m)
        {
            if (m == null || !m.Player)
                return;

            // Clear outgoing damage tracking
            S51aDamageTracker.ClearDamageTracking(m);
        }

        /// <summary>
        /// Player disconnected event handler.
        /// Cleanup PvP damage tracking when player disconnects.
        /// </summary>
        private static void OnPlayerDisconnected(Mobile m)
        {
            if (m == null || !m.Player)
                return;

            // Clear outgoing damage tracking
            S51aDamageTracker.ClearDamageTracking(m);
        }
    }
}
