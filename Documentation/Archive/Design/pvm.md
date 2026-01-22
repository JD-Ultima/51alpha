# PvM System Design

## Overview
Player vs Monster (PvM) system encompassing all monster combat, dungeon mechanics, and progression systems.

## Core Components

### 1. Dungeon Preparation & Access System
**Status**: Approved (v2.2.0)
**Document**: [PVM_DUNGEON_PREPARATION_SYSTEM.md](../Design/PVM_DUNGEON_PREPARATION_SYSTEM.md)

#### Features:
- Per-dungeon, per-level buff model
- External resource dependency
- Puzzle Cooking System (gump-driven)
- World interaction requirements
- Temporary power with risk

#### Integration:
- Wraps dungeon access (zero regression risk)
- Account-wide access tracking
- Thematic resource gathering
- Seasonal recipe rotation

### 2. Monster AI & Spawn System
- Aggressive/passive behavior patterns
- Faction-based aggression
- Dynamic spawn rules
- Loot table integration

### 3. Combat Mechanics
- Damage calculation formulas
- Resistance/immunity system
- Special attack patterns
- Status effect application

### 4. Progression & Rewards
- Experience scaling
- Loot distribution
- Quest integration
- Achievement system

## Dungeon Preparation System Details

### Access Model
- Universal access catalyst per dungeon (crafted once)
- Account-wide permanent unlock
- Thematic ingredient requirements
- No dungeon internal changes

### Economy Integration
- Consumable demand without permanent wealth
- Natural gold sinks via failures
- Low/mid-tier mob relevance
- Trade specialization opportunities

### PvP/PvM Separation
- Dungeon access never revoked
- Buffs are monster-specific (no PvP impact)
- Works with talisman equipped
- Matches Siege/faction item philosophy

### Cooking System
- Puzzle Cooking System (gump-driven interaction)
- Stone Oven house addon (crafted by carpenters)
- Experimental discovery (no recipe lists)
- Configurable failure modes (Mode A: learning-friendly, Mode B: high-risk)
- Account-wide recipe knowledge
- Category-restricted ingredient slots
- Atomic transactions (no container state)

### Resource Gathering
- 1 fishing aspect + 1 mob + 1 environmental per dungeon
- Thematically linked ingredients
- Forces world movement
- Territorial relevance

## Implementation Status

### Completed
- [x] Dungeon Preparation System design
- [x] CHANGELOG entry [2.2.0]
- [x] Feature document creation
- [x] All design questions resolved

### In Progress
- [ ] Database schema implementation
- [ ] Cooking apparatus development
- [ ] Resource gathering integration

### Planned
- [ ] Seasonal rotation system
- [ ] Faction integration hooks
- [ ] Advanced recipe discovery

## Related Documents
- [Dungeon Preparation System](../Design/PVM_DUNGEON_PREPARATION_SYSTEM.md)
- [Crafting System](crafting.md)
- [Economy System](economy.md)
- [World System](world.md)

## Traceability
- **CHANGELOG**: [2.2.0] - 2025-12-26
- **Design Links**: All cross-system questions resolved
- **Implementation Status**: Feature document approved
- **Known Conflicts**: None
