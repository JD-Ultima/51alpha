// =====================================================
// 51ALPHA FACTION SYSTEM - VENDOR DISCOUNT
// =====================================================
// File: FactionVendorDiscount.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: PHASE1_VERIFICATION_REPORT_v3.md (Q1: 10% NPC vendor discount)
//                PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q6: Event hooks)
// =====================================================

using System;
using Server.Guilds;
using Server.Mobiles;

namespace Server.Sphere51a.Factions
{
    /// <summary>
    /// Provides 10% NPC vendor discount for guild members in their faction's home city.
    /// Hooks into ModernUO vendor system without modifying defaults.
    /// </summary>
    public static class FactionVendorDiscount
    {
        private static bool _isInitialized = false;

        /// <summary>
        /// Initialize vendor discount system.
        /// Called during server startup.
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                Console.WriteLine("[Sphere51a] Vendor discount system already initialized - skipping");
                return;
            }

            // Note: ModernUO may not expose VendorSell events directly
            // This is a placeholder for the event hook pattern
            // Actual implementation depends on ModernUO's extensibility points

            _isInitialized = true;

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine("[Sphere51a] Faction vendor discount system initialized");
            Utility.PopColor();
        }

        /// <summary>
        /// Calculate price modifier for buyer at vendor.
        /// Returns 0.90 for 10% discount if conditions met.
        /// </summary>
        /// <param name="buyer">Player buying from vendor</param>
        /// <param name="vendor">Vendor NPC</param>
        /// <returns>Price multiplier (0.90 = 10% discount, 1.0 = no discount)</returns>
        public static double GetPriceModifier(Mobile buyer, Mobile vendor)
        {
            if (buyer == null || vendor == null)
                return 1.0;

            // Get buyer's guild
            var guild = buyer.Guild as Guild;
            if (guild == null)
                return 1.0;

            // Get guild's faction
            var faction = guild.GetFaction();
            if (faction == null)
                return 1.0;

            // Get vendor's city
            string vendorCity = GetVendorCity(vendor.Location, vendor.Map);
            if (string.IsNullOrEmpty(vendorCity))
                return 1.0;

            // Apply 10% discount if city matches faction home city
            if (faction.HomeCity.Equals(vendorCity, StringComparison.OrdinalIgnoreCase))
            {
                return 0.90; // 10% discount
            }

            return 1.0; // No discount
        }

        /// <summary>
        /// Determine which faction city the vendor is in.
        /// Uses region-based detection.
        /// </summary>
        /// <param name="location">Vendor location</param>
        /// <param name="map">Map</param>
        /// <returns>City name ("Trinsic", "Vesper", "Moonglow") or null</returns>
        private static string GetVendorCity(Point3D location, Map map)
        {
            if (map == null)
                return null;

            var region = Region.Find(location, map);
            if (region == null)
                return null;

            // Check region names for city matches
            string regionName = region.Name?.ToLower() ?? string.Empty;

            // Trinsic (The Golden Shield home city)
            if (regionName.Contains("trinsic"))
                return "Trinsic";

            // Vesper (The Bridgefolk home city)
            if (regionName.Contains("vesper"))
                return "Vesper";

            // Moonglow (The Lycaeum Order home city)
            if (regionName.Contains("moonglow"))
                return "Moonglow";

            return null;
        }

        /// <summary>
        /// Check if buyer gets faction discount at this vendor.
        /// Helper method for vendor logic.
        /// </summary>
        /// <param name="buyer">Player buying from vendor</param>
        /// <param name="vendor">Vendor NPC</param>
        /// <returns>True if discount applies</returns>
        public static bool HasDiscountAt(Mobile buyer, Mobile vendor)
        {
            return GetPriceModifier(buyer, vendor) < 1.0;
        }

        /// <summary>
        /// Apply faction discount to price.
        /// </summary>
        /// <param name="originalPrice">Original price</param>
        /// <param name="buyer">Player buying</param>
        /// <param name="vendor">Vendor NPC</param>
        /// <returns>Discounted price</returns>
        public static int ApplyDiscount(int originalPrice, Mobile buyer, Mobile vendor)
        {
            double modifier = GetPriceModifier(buyer, vendor);
            return (int)(originalPrice * modifier);
        }
    }
}
