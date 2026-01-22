# Crafting System Design

## Overview
Comprehensive crafting system supporting traditional UO crafts plus new experimental systems.

## Core Crafting Systems

### 1. Traditional Crafting
- Blacksmithing
- Tailoring
- Alchemy
- Inscription
- Tinkering
- Cooking (basic)

### 2. Dungeon Catalyst Cooking (New)
**Status**: Approved (v2.2.0)
**Document**: [PVM_DUNGEON_PREPARATION_SYSTEM.md](../Design/PVM_DUNGEON_PREPARATION_SYSTEM.md)

#### Features:
- Puzzle-based gump cooking system
- Recipe discovery through experimentation
- Dungeon-specific catalyst creation
- Account-wide recipe knowledge
- Knowledge-driven economy

#### Integration:
- Gump-driven interaction (Stone Oven + Puzzle Gump)
- Category-restricted ingredient slots
- Configurable failure modes (learning-friendly vs high-risk)
- Atomic transactions (no container state)
- Thematic ingredient requirements

### 3. Specialized Crafting
- Runic tool crafting
- Potion brewing
- Scroll inscription
- Talisman creation

## Dungeon Catalyst Cooking Details

### Stone Oven & Puzzle Cooking Gump
- **Item**: `Stone Oven` (house addon, crafted by carpenters)
- **Interaction**: Gump-driven interface with ingredient selection
- **Slot Taxonomy**: Category-restricted slots (Common Fruit, Rare Fruit, Optional Catalyst)
- **Behavior**:
  - Opens Puzzle Cooking Gump on use
  - Player selects ingredients via inventory picker
  - Items remain in inventory until "Bake" is pressed
  - Server validates skill, proximity, and recipe match
  - Atomic transaction: all-or-nothing consumption
  - No container state or serialization

### Recipe Discovery
- **Tracking**: Account-wide binary knowledge
- **Storage**: `AccountKnownRecipes` table
- **Features**:
  - No progress tracking
  - No XP or skill requirements
  - Pure knowledge economy

### Success/Failure System
**Outcome Determined by Skill Threshold**:
- Success requires meeting minimum skill level for recipe tier
- Configurable failure modes:
  - **Mode A (Learning Friendly)**: No ingredient consumption on failure, encourages experimentation
  - **Mode B (High Risk)**: All ingredients consumed on failure, reinforces economic risk
- **Important**: Partial consumption is explicitly forbidden

**Cooking Skill Requirements**:
- **Dungeon Access**: 50 Cooking skill minimum
- **Level 1 Buffs**: 60 Cooking skill minimum
- **Level 2 Buffs**: 80 Cooking skill minimum
- **Level 3 Buffs**: 100 Cooking skill minimum
- **Level 4 Buffs**: 120 Cooking skill minimum

**Jewelry Bonuses**: Cooking skill can be augmented by jewelry bonuses, capped at +20 maximum

**Buff Constraints**: Buffs may reduce dungeon traversal friction but may NOT modify combat stats, damage to players, or affect PvP balance

### Resource Requirements
**Per Dungeon**:
- 1 fishing aspect
- 1 humanoid monster drop
- 1 environmental reagent
- Thematically linked ingredients

## Implementation Status

### Completed
- [x] Dungeon Catalyst Cooking design
- [x] Recipe discovery system
- [x] Failure outcome matrix
- [x] Resource gathering requirements

### In Progress
- [ ] Stone Oven house addon implementation
- [ ] Recipe database integration
- [ ] Discovery tracking system

### Planned
- [ ] Seasonal recipe rotation
- [ ] Faction-specific catalysts
- [ ] Advanced experimentation mechanics

## Related Documents
- [Dungeon Preparation System](../Design/PVM_DUNGEON_PREPARATION_SYSTEM.md)
- [PvM System](pvm.md)
- [Economy System](economy.md)

## Traceability
- **CHANGELOG**: [2.2.0] - 2025-12-26
- **Design Links**: All questions resolved
- **Implementation Status**: Feature approved
- **Known Conflicts**: None
