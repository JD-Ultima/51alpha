# Economy System Design

## Overview
Comprehensive economy system balancing gold flow, resource gathering, crafting, and player trade.

## Core Economy Components

### 1. Gold Flow
- Monster drops (scaled by difficulty)
- Quest rewards
- Crafting sales
- Player trading
- Gold sinks (repairs, fees, taxes)

### 2. Resource Gathering
- Traditional gathering (mining, lumberjacking, fishing)
- Dungeon Catalyst Resources (New)
- World spawns
- Monster drops

### 3. Crafting & Production
- Traditional crafting economy
- Dungeon Catalyst Cooking (New)
- Consumable production
- Equipment crafting

### 4. Player Trade
- Direct player trading
- Vendor sales
- Auction systems
- Guild trade networks

## Dungeon Catalyst Economy Integration

### Resource Gathering
**Requirements**:
- Several monster drop per dungeon
- Thematically linked ingredients

**Impact**:
- Revives low/mid-tier mob farming
- Encourages world exploration
- Prevents dungeon self-sufficiency

### Consumable Production
**Features**:
- Temporary, dungeon-specific buffs
- Time-limited duration
- No permanent power creep
- Natural gold sinks via:
  - Failed experiments
  - Ingredient costs
  - Time-limited effects

### Trade Specialization
**Opportunities**:
- Recipe knowledge trading
- Ingredient specialization
- Catalyst crafting services
- Buff optimization consulting

### Gold Sink Mechanics
**Sources**:
- Cooking failures (ingredient loss)
- Time-limited buffs (repeated crafting)
- Experimental discovery (trial and error)

## Implementation Status

### Completed
- [ ] Dungeon Catalyst resource requirements
- [ ] Consumable production balance
- [ ] Trade specialization opportunities
- [ ] Gold sink mechanics

### In Progress
- [ ] Resource gathering implementation
- [ ] Crafting economy integration
- [ ] Trade system enhancements

### Planned
- [ ] Dynamic pricing algorithms
- [ ] Faction-based trade networks
- [ ] Seasonal economy events

## Related Documents
- [Dungeon Preparation System](../Design/PVM_DUNGEON_PREPARATION_SYSTEM.md)
- [Siege Coin Reward System](../Design/51alpha_Siege_Coin_Reward_System.md)
- [PvM System](pvm.md)
- [Crafting System](crafting.md)

## Traceability
- **CHANGELOG**: [2.2.0] - 2025-12-26
- **Design Links**: All questions resolved
- **Implementation Status**: Feature approved
- **Known Conflicts**: None
