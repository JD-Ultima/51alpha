# Changelog

All notable changes to 51alpha documentation and design.

## [2.4.0] - 2025-12-27

### Primary System: Crafting
### Secondary Systems: PvM, Economy, Items, World

### Summary
Implements comprehensive Weapon Cooking Quests System with authoritative weapon-dungeon mapping, permanent dungeon access via cooking, and integrated progression loops

### Player Impact
- 4 authoritative quest weapons for first 4 dungeons (Blood Tentacle, Wolf Bane, Diamond Katana, Dwarven Battle Axe)
- 3 weapons reserved for future dungeon development (Hell's Halberd, Black Widow, Judgement Hammer)
- Permanent dungeon access through one-time cooking consumption
- Weapon-enabled efficient farming of external monsters
- Cooking-based dungeon buffs for deeper progression
- Integrated weapon-cooking-dungeon progression loops

### Developer Impact
- New weapon system with monster-specific bonuses and authoritative dungeon mapping
- Cooking system integration for dungeon access and buffs
- Quest chain implementation (4-phase structure)
- Dungeon access tracking system
- Monster tag validation for weapon bonuses
- Design Authority compliance enforcement

### Backward Compatibility
✅ Yes - Additive system that doesn't affect existing mechanics

### Design Links
- docs/Design/51alpha_weapon_cooking_quests.md
- docs/Systems/crafting.md
- docs/Systems/pvm.md
- docs/Systems/economy.md
- docs/Systems/items.md

### Code Targets
- Projects/UOContent/Sphere51a/WeaponSystems/WeaponCookingQuests.cs
- Projects/UOContent/Sphere51a/Crafting/WeaponRecipes.cs
- Projects/UOContent/Sphere51a/DungeonSystems/DungeonAccessController.cs
- Projects/UOContent/Sphere51a/Items/SpecializedWeapons.cs
- Projects/UOContent/Sphere51a/QuestSystems/WeaponQuestChain.cs

### Risk Assessment
Medium - Complex system with multiple dependencies, but follows established patterns and includes comprehensive safeguards. Requires careful balancing of weapon bonuses and cooking requirements. Design Authority compliance adds governance layer.

### Changes
- Added Weapon Cooking Quests System with authoritative weapon-dungeon mapping
- Implemented 4 authoritative weapons for first 4 dungeons: Blood Tentacle (Despise), Wolf Bane (Shame), Diamond Katana (Wrong), Dwarven Axe (Destard)
- Reserved 3 weapons for future development: Hell's Halberd, Black Widow, Judgement Hammer
- Implemented permanent dungeon access via cooking consumption
- Created 4-phase quest structure (Weapon Recipe → Mob Farming → Dungeon Access → Dungeon Mastery)
- Added weapon-enabled efficient farming mechanics
- Integrated cooking-based dungeon buff system
- Established clear system boundaries with talismans
- Added Design Authority compliance block to prevent regression
- Added anti-exploit safeguards and balance constraints

---

## [2.3.0] - 2025-12-27

### Primary System: PvP
### Secondary Systems: Economy, Progression, Items

### Summary
Implements comprehensive Siege Coin Reward System with performance-based PvP rewards, temporary stat boosts, and strict PvM/PvP separation

### Player Impact
- New faction siege reward currency (Siege Coins)
- Temporary stat boosts through jewelry system (+10 max)
- Temporary AR boosts through faction clothing (+10 max)
- Faction mounts with mobility advantages
- Strict separation: siege bonuses disabled when talismans equipped

### Developer Impact
- New currency system with account-wide tracking
- Siege coin vendor implementation
- Temporary item decay system (180h logged-in time)
- PvP/PvM separation enforcement logic
- Contribution scoring system for siege participation

### Backward Compatibility
✅ Yes - Additive system that doesn't affect existing mechanics

### Design Links
- docs/Design/51alpha_Siege_Coin_Reward_System.md
- docs/Systems/pvp.md
- docs/Systems/economy.md
- docs/Systems/progression.md
- docs/Systems/items.md

### Code Targets
- Projects/UOContent/Sphere51a/FactionSystems/SiegeCoinSystem.cs
- Projects/UOContent/Sphere51a/FactionSystems/SiegeCoinVendor.cs
- Projects/UOContent/Sphere51a/FactionSystems/SiegeContributionTracker.cs
- Projects/UOContent/Sphere51a/Items/SiegeJewelry.cs
- Projects/UOContent/Sphere51a/Items/SiegeClothing.cs
- Projects/UOContent/Sphere51a/Items/SiegeMount.cs

### Risk Assessment
High - Complex system with multiple dependencies, stat stacking implications, and PvP balance concerns. Requires careful tuning and monitoring.

### Changes
- Added Siege Coin Reward System with performance-based acquisition
- Implemented temporary stat boost jewelry system (+2.5 per piece, +10 max)
- Added temporary AR boost faction clothing system (+2.5 per piece, +10 max)
- Created faction mount with +10 DEX bonus
- Implemented strict PvM/PvP separation (siege bonuses disabled with talismans)
- Added anti-abuse safeguards and seasonal integration

---

## [2.2.0] - 2025-12-26

### Primary System: PvM
### Secondary Systems: Crafting, Economy, World, Progression

### Summary
Implements comprehensive PvM Dungeon Preparation & Access System with per-dungeon buffs, external resource dependency, and Horadric-style cooking

### Player Impact
- New dungeon preparation mechanics requiring world interaction
- Temporary, dungeon-specific buff system with risk/reward
- Recipe discovery and experimentation mechanics
- Revives fishing, hunting, and exploration relevance

### Developer Impact
- New dungeon access system with account-wide tracking
- Cooking apparatus implementation (Horadric Cube-style)
- Resource gathering integration with thematic mapping
- Recipe management system with seasonal rotation

### Backward Compatibility
✅ Yes - Additive system that doesn't affect existing dungeons or mechanics

### Design Links
- docs/Design/PVM_DUNGEON_PREPARATION_SYSTEM.md
- docs/Design/Systems/pvm.md
- docs/Design/Systems/crafting.md
- docs/Design/Systems/economy.md

### Code Targets
- Projects/UOContent/Sphere51a/DungeonSystems/DungeonAccessController.cs
- Projects/UOContent/Sphere51a/DungeonSystems/DungeonAccessProfile.cs
- Projects/UOContent/Sphere51a/DungeonSystems/DungeonRegionGate.cs
- Projects/UOContent/Sphere51a/Crafting/DungeonOven.cs
- Projects/UOContent/Sphere51a/WorldInteraction/ResourceGathering.cs

### Risk Assessment
Medium - Complex system with multiple dependencies, but follows established patterns (Siege prep logic) and includes comprehensive safeguards

### Changes
- Added PvM Dungeon Preparation & Access System
- Implemented Horadric-style cooking with experimental discovery
- Added external resource dependency for dungeon access
- Created recipe discovery mechanics with seasonal rotation
- Enhanced world interaction requirements for resource gathering

---

## [2.1.0] - 2025-12-08

### Spell System Clarifications
- **Damage Interruption**: Confirmed damage does NOT interrupt spells (major change from OSI)
- **Equipment Interruption**: Equipping items does NOT interrupt spells
- **War Mode**: Toggling war mode DOES interrupt spells
- **Bandages**: Using bandages DOES interrupt spells
- **Targeting**: Target cursor appears IMMEDIATELY on cast (before delay)
- **Scroll System**: Added 43% mana reduction + 0.5s speed bonus (circle 3+)

### Updated Files
- `specs/spell-system.md` - Complete rewrite v2.0.0 with verified flow
- `docs/systems/combat.md` - Corrected interruption rules, added scroll section
- `docs/reference/config.md` - Added interruption and scroll config keys

### Implementation Notes
- FC still being removed from all equipment
- 50% mana on fizzle confirmed (configurable)
- Target-first flow requires verification against ModernUO SpellState.Sequencing usage

---

## [2.0.0] - 2025-12-08

### Restructured
- Complete documentation reorganization into professional dev structure
- Consolidated 17 files (525KB) into 15 files (153KB)
- Created hierarchical organization: Architecture → Systems → Implementation → Reference

### Added
- `README.md` - Project overview with quick start
- `docs/architecture/overview.md` - Design principles and decisions
- `docs/architecture/database-schema.md` - Condensed PostgreSQL schema
- `docs/architecture/modernuo-integration.md` - Hooks and extension points
- `docs/systems/combat.md` - Sphere-style combat mechanics
- `docs/systems/factions.md` - VvV and siege warfare
- `docs/systems/progression.md` - Talismans and Glicko ratings
- `docs/systems/economy.md` - Crafting, BODs, housing
- `docs/systems/content.md` - Dungeons, bounties, NPE, tournaments
- `docs/implementation/phases.md` - 8-phase development plan
- `docs/implementation/checklist.md` - 70+ actionable tasks
- `docs/implementation/ai-prompts.md` - Copy-paste implementation prompts
- `docs/reference/locations.md` - World coordinates
- `docs/reference/config.md` - All configuration parameters
- `docs/reference/commands.md` - Admin command reference
- `specs/spell-system.md` - Verified ModernUO code analysis
- `specs/talisman-system.md` - PvP/PvM separation logic

### Verified (Against ModernUO GitHub)
- SpellState enum: 3 states (None, Casting, Sequencing), not 6
- DisturbType enum: 5 types, no Movement type
- CheckSequence() flow: Reagents first, mana after fizzle check
- GetCastDelay() FC calculation: Protection subtracts from FC
- Timer wheel: O(1) operations, event-driven

### Decisions Finalized
- **Option C Mana**: 50% consumed on fizzle (configurable)
- **Faster Casting**: Removed from all equipment
- **Protection Spell**: 0 FC penalty (configurable)
- **Movement**: 100% free during casting
- **50Hz Microtick**: NOT REQUIRED (ModernUO timer wheel sufficient)
- **Talisman Timer**: Elapsed time tracking with pause on unequip

### Removed
- 50Hz Microtick Engine (unnecessary complexity)
- ML-Predictive Cancellation (no benefit)
- Relic Freshness Timer (removed from design)
- Zero-Penalty Fizzle (contradicts skill-based design)

### Archived
- All original documentation moved to `_archive/`
- Preserved for reference during implementation

---

## [1.1.0] - 2025-12-05

### Added
- Deep spell system analysis with ModernUO verification
- 50Hz analysis document (concluded not needed)
- Option A/B/C mana consumption comparison
- FC removal implementation guide

### Changed
- Mana consumption from Option A to Option C
- Movement threshold from 50% to 100% free

---

## [1.0.0] - 2025-01

### Added
- Initial architecture documentation
- Database schema (31 tables)
- World locations (22KB)
- ModernUO integration guide
- Implementation checklist (70+ tasks)
- VvV siege system design
- Tournament system design
- Daily content system
- New player experience
- Static house rental system

### Identified Issues
- 6 design errors found during ModernUO verification
- Multiple conflicting specifications across documents
- Overcomplicated 50Hz engine proposal
