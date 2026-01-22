# 51alpha Weapon Cooking Quests System

**Authority**: TIER 2 - Implementation Spec (see `DESIGN_DECISIONS.md` for authoritative decisions)

> **Note**: This document requires re-verification against DESIGN_DECISIONS.md for:
> - Dungeon relic types vs Content&Events.md
> - Weapon-dungeon mappings

## Document Metadata
- Version: v1.0.0
- Last Updated: 2025-12-27
- Primary System: Crafting
- Secondary Systems: PvM, Economy, Items, World
- Status: Draft (NEEDS REVIEW)
- CHANGELOG Reference: [2.4.0]

## 1. Overview

The Weapon Cooking Quests System implements a comprehensive progression loop integrating specialized weapons, cooking mechanics, and dungeon access. This system creates meaningful interdependencies between crafting, monster hunting, cooking, and dungeon progression while maintaining clear boundaries with existing systems like talismans.

### Core Design Philosophy
- **Weapons are NOT just damage upgrades** - They counter specific monster mechanics and enable efficient resource farming
- **Cooking is mandatory but meaningful** - Permanent dungeon access and repeatable buffs create retention
- **Dungeons remain challenging** - Preparation enables mastery, not trivialization
- **No system overlap** - Clear boundaries between weapons, cooking, and talismans

### The Big Loop
```
Quest → Weapon → Efficient Monster Farming → Cooking Materials → Dungeon Access → Dungeon Buffs → Deeper Progression → Next Quest
```

### Weapon–Dungeon Authority
The Despise/Shame/Wrong/Destard quest weapons are Blood Tentacle, Wolf Bane, Diamond Katana, and Dwarven Axe respectively.
This mapping is user-locked and supersedes all legacy drafts.

## 2. System Classification

### Primary System: Crafting
This system is primarily focused on weapon crafting and cooking integration, creating specialized tools for resource gathering and dungeon preparation.

### Secondary Systems:
- **PvM**: Dungeon progression mechanics and monster-specific weapon advantages
- **Economy**: Resource gathering efficiency and cooking material flows
- **Items**: Specialized weapon system with contextual bonuses
- **World**: Dungeon access mechanics and world integration

## 3. Design Specifications

### 3.1 Authoritative First-4-Dungeon Weapons

#### Blood Tentacle (Kryss)
- **Dungeon**: Despise (Tier 1 - Easiest)
- **Theme**: Anti-humanoid, organic counter
- **Monsters Countered**: Humanoids, organic creatures, fleshy monsters
- **Mechanical Advantages**:
  - Bonus damage vs humanoid monster family
  - Efficient harvesting of organic cooking materials
  - Reduced combat duration vs fleshy targets

#### Wolf Bane (Broad Sword)
- **Dungeon**: Shame (Tier 2)
- **Theme**: Anti-beast, feral counter
- **Monsters Countered**: Beasts, feral creatures, animalistic monsters
- **Mechanical Advantages**:
  - Bonus damage vs beast monster family
  - Crowd control vs swarming creatures
  - Efficient harvesting of beast-related materials

#### Diamond Katana
- **Dungeon**: Wrong (Tier 3)
- **Theme**: Precision, armor penetration, timing
- **Monsters Countered**: Crystal elementals, stone guardians, armored undead
- **Mechanical Advantages**:
  - Katana: Ignores % of physical resistance, bonus damage on precise timing, reduced durability loss
  - Chu Ko Nu: Multi-bolt burst breaks defensive stances, applies "fracture" debuff

#### Dwarven Battle Axe
- **Dungeon**: Destard (Tier 4 - Hardest)
- **Theme**: Brutal force, ancient wards, breaking defenses
- **Monsters Countered**: Armored monsters, constructs, ancient guardians
- **Mechanical Advantages**:
  - Armor break stacks
  - Bonus damage to stationary/guarding enemies
  - Shatters environmental defenses

### 3.2 Reserved for Future Development

#### Hell's Halberd
- **Status**: Reserved for future dungeon releases
- **Theme**: Reach, crowd control, hellspawn suppression
- **Monsters Countered**: Demons, fire elementals, hell hounds
- **Mechanical Advantages**: Bonus damage vs burning/enraged targets, sweeping arc with spacing requirements, suppresses demon rage
- **Note**: Must not be used for first four dungeons (Despise, Shame, Wrong, Destard)

#### Black Widow
- **Status**: Reserved for future dungeon releases
- **Theme**: Poison mastery, fear, ambush control
- **Monsters Countered**: Arachnids, fast attackers, regenerating enemies
- **Mechanical Advantages**: Poison prevents regeneration, backstab bonus, stacking venom slows attack speed
- **Note**: Must not be used for first four dungeons (Despise, Shame, Wrong, Destard)

#### Judgement Hammer
- **Status**: Reserved for future dungeon releases
- **Theme**: Law vs corruption, execution weapon
- **Monsters Countered**: Evil humanoids, cultists, corrupted paladins
- **Mechanical Advantages**: Bonus damage vs evil-aligned, chance to stun, executes low-HP corrupted enemies faster
- **Note**: Must not be used for first four dungeons (Despise, Shame, Wrong, Destard)

### 3.2 Cooking System Integration

**System Reference**: See [Puzzle Cooking System](PVM_DUNGEON_PREPARATION_SYSTEM.md#34-puzzle-cooking-system) for full technical details.

**Integration Model**: This quest system provides the narrative and progression framework. The actual cooking mechanics use the **Puzzle Cooking System** with gump-driven interaction and Stone Oven house addons.

#### Permanent Dungeon Access
- **Core Rule**: Dungeon access becomes permanent after consuming access food
- **Mechanics**: Character-bound cooked item consumed once, grants permanent unlock
- **Ingredients**: Requires materials from weapon-enabled external monsters
- **Implementation**: Crafted using Stone Oven + Puzzle Cooking Gump (see [Section 3.8](PVM_DUNGEON_PREPARATION_SYSTEM.md#38-stone-oven--puzzle-cooking-gump))

#### Repeatable Dungeon Buffs
- **Core Rule**: Buffs are always timed, consumable, and dungeon-specific
- **Mechanics**: Temporary resistances, reduced stamina drain, monster-specific mitigations
- **Purpose**: Enable deeper dungeon progression and elite encounters
- **Implementation**: Crafted using Puzzle Cooking System with configurable failure modes (Mode A: learning-friendly, Mode B: high-risk)

### 3.3 Quest Structure (4-Phase Canonical Chain)

#### Phase 1: Weapon Recipe Quest
- **Purpose**: Introduce dungeon ecosystem and provide efficient farming tool
- **Structure**: Investigation, puzzle/riddle, environmental interaction
- **Reward**: Weapon recipe (not weapon itself)
- **Example**: "The Facet That Cuts" - solve resonance puzzle to get Diamond Katana recipe

#### Phase 2: Weapon-Boosted Mob Loop
- **Purpose**: Make weapon feel necessary through efficiency
- **Scope**: Occurs OUTSIDE dungeon in surrounding regions
- **Mechanics**: Monsters technically killable without weapon but inefficient/dangerous
- **Balance**: Weapon reduces combat duration, damage spikes, swarm risk

#### Phase 3: Dungeon Access via Cooking
- **Purpose**: Permanently unlock dungeon through preparation
- **Mechanics**: Cook and consume access food once
- **Enforcement**: Character flag stored in progression table
- **Failure State**: Dungeon accessible without access food (but with penalties)

#### Phase 4: Dungeon Boost via Cooking
- **Purpose**: Enable deeper dungeon progression
- **Mechanics**: Timed buffs for resistances, stamina, monster mitigation
- **Repeatable**: Creates ongoing demand for cooking

## 4. Related Systems

- **Crafting System** → Weapon Crafting → Specialized Weapon Recipes
- **PvM System** → Dungeon Progression → Weapon-Enabled Farming
- **Economy System** → Resource Flows → Cooking Material Drops
- **Items System** → Special Items → Monster-Specific Weapons
- **World System** → Dungeon Access → Permanent Unlock Mechanics

**Dependency Graph**:
```
Crafting System
├─ Weapon Crafting
│  └─ Weapon Cooking Quests System
│     ├─ Specialized Weapons
│     ├─ Quest Chain Structure
│     │  ├─ Weapon Recipe Quests
│     │  ├─ Mob Farming Loops
│     │  ├─ Dungeon Access Cooking
│     │  └─ Dungeon Buff Cooking
│     └─ Cooking Integration
└─ Economy System
   └─ Resource Flows
      └─ Cooking Material Drops
```

## 5. Implementation Requirements

### Code Targets:
- Projects/UOContent/Sphere51a/WeaponSystems/WeaponCookingQuests.cs
- Projects/UOContent/Sphere51a/Crafting/WeaponRecipes.cs
- Projects/UOContent/Sphere51a/DungeonSystems/DungeonAccessController.cs
- Projects/UOContent/Sphere51a/Items/SpecializedWeapons.cs
- Projects/UOContent/Sphere51a/QuestSystems/WeaponQuestChain.cs

### Configuration:
- Weapon bonus values and monster tag mappings
- Cooking recipe requirements and durations
- Dungeon access and buff mechanics
- Quest chain progression tracking

### Database:
- Character dungeon unlock flags
- Quest progression tracking
- Cooking recipe knowledge
- Weapon crafting requirements

## 6. Cross-System Questions

### Q1: Balance Impact - Weapon Contextual Power
**Question**: Do the specialized weapons create unacceptable power gaps in their contextual scenarios?
**Analysis**: Mitigation factors:
- Weapons only provide advantages vs specific monster types
- No global power increases
- Contextual efficiency, not absolute power
- **Risk**: Low - Contextual advantages are balanced

### Q2: Economy Impact - Cooking Material Flows
**Question**: Does the system create inflationary or deflationary pressure on cooking economies?
**Analysis**: Balancing factors:
- Permanent access reduces long-term demand
- Repeatable buffs create ongoing demand
- Weapon efficiency gates material supply
- **Risk**: Medium - Requires monitoring of material flows

### Q3: Progression Impact - Permanent Access
**Question**: Does permanent dungeon access reduce long-term retention?
**Analysis**: Retention factors:
- Permanent progression feels rewarding
- Repeatable buff cooking remains relevant
- Weapon specialization creates identity
- New dungeon tiers add new buffs, not new access grinds
- **Risk**: Low - Design supports multiple retention vectors

### Q4: System Conflict - Talisman Boundaries
**Question**: Are the boundaries between weapons, cooking, and talismans sufficiently clear?
**Analysis**: Boundary enforcement:
- Weapons: External farming only, no dungeon bonuses
- Cooking: Dungeon access/buffs only, no combat stats
- Talismans: Playstyle modifiers only, no resource advantages
- **Risk**: Low - Clear system separation maintained

### Q5: Exploit Risk - Quest Efficiency
**Question**: Are there exploit paths to bypass intended progression loops?
**Analysis**: Anti-exploit measures:
- Weapon efficiency vs raw grinding
- Permanent access prevents key farming
- No weapon requirements for dungeon entry
- **Risk**: Medium - Requires monitoring of progression paths

### Q6: Missing Information - Implementation Details
**Question**: What are the specific implementation requirements for weapon-monster tag validation?
**Analysis**: Needs resolution:
- Monster tag system design
- Weapon bonus application logic
- Validation timing (on-hit vs on-equip)
- **Status**: TBD - Needs technical design

### Q7: Unknown Tuning - Balance Values
**Question**: What are the optimal values for weapon bonuses and cooking requirements?
**Analysis**: Requires:
- Playtesting data
- Economy impact analysis
- Progression curve testing
- **Status**: TBD - Needs tuning phase

### Q8: Integration Risk - Existing Systems
**Question**: How does this system integrate with existing dungeon and cooking systems?
**Analysis**: Integration points:
- Dungeon access system compatibility
- Cooking recipe system extensions
- Quest tracking system requirements
- **Risk**: Medium - Requires careful integration planning

## 7. Risk Assessment

**Overall Risk**: Medium
**Justification**: Complex system with multiple dependencies, but follows established patterns (dungeon preparation system) and includes comprehensive safeguards. The system touches crafting, PvM, economy, and items systems, requiring careful balancing but with clear boundaries.

**Risk Breakdown**:
- **Implementation Complexity**: Medium (multiple subsystems, but modular)
- **Balance Risk**: Medium (weapon bonuses and cooking requirements)
- **Exploit Risk**: Medium (progression efficiency monitoring needed)
- **Economy Risk**: Medium (material flow balancing required)
- **Integration Risk**: Medium (multiple system touchpoints)

## 8. Traceability

### Design Links:
- [Master Architecture - Crafting Section](../../Systems/crafting.md)
- [PvM System - Dungeon Progression](../../Systems/pvm.md)
- [Economy System - Resource Flows](../../Systems/economy.md)
- [Items System - Specialized Weapons](../../Systems/items.md)
- [CHANGELOG Entry - 2.4.0](../../../CHANGELOG.md#240---2025-12-27)

### Implementation Status:
- [Link to IMPLEMENTATION_STATUS.md](../../Implementation/Navigation/IMPLEMENTATION_STATUS.md)

### Known Conflicts:
- None identified

## 9. Approval

- [ ] Design Review Complete
- [ ] Architecture Review Complete
- [ ] Crafting System Review Complete
- [ ] PvM System Review Complete
- [ ] Economy Impact Review Complete
- [ ] Implementation Approved
- [ ] CHANGELOG Entry Created
- [ ] Traceability Established
- [ ] Cross-System Questions Documented
- [ ] Weapon-Dungeon Authority Compliance Verified

## 14. Design Authority Compliance

### Weapon–Dungeon Authority Block
**AUTHORITATIVE MAPPING (LOCKED)**:
- **Despise** (Tier 1): Blood Tentacle (Kryss) - Anti-humanoid/organic
- **Shame** (Tier 2): Wolf Bane (Broad Sword) - Anti-beast/feral
- **Wrong** (Tier 3): Diamond Katana - Anti-crystal/stone
- **Destard** (Tier 4): Dwarven Axe (Battle Axe) - Anti-armored/high-HP

**RESERVED FOR FUTURE DEVELOPMENT**:
- Hell's Halberd
- Black Widow
- Judgement Hammer

**COMPLIANCE RULES**:
1. Only the 4 authoritative weapons may be used for the first 4 dungeons
2. Reserved weapons must not appear in Despise, Shame, Wrong, or Destard contexts
3. This mapping supersedes all prior documentation and drafts
4. Any document referencing these weapons must comply with this authority block

**CROSS-LINKS**:
- [CHANGELOG Entry - 2.4.0](../../../CHANGELOG.md#240---2025-12-27)
- [Implementation Status](../../Implementation/Navigation/IMPLEMENTATION_STATUS.md)
- [Dungeon Preparation System](../../Design/PVM_DUNGEON_PREPARATION_SYSTEM.md)

**VERIFICATION**:
- [ ] All weapon references updated to comply with authoritative mapping
- [ ] No reserved weapons used in first 4 dungeons
- [ ] Design Authority block added to all affected documents
- [ ] Cross-links established to prevent regression

## 10. Authoritative Design Statements

### Final System Truth Table
| System | Purpose |
|--------|---------|
| Weapons | Efficiently farm external resource mobs |
| External Mobs | Gate cooking materials |
| Cooking (Access) | Permanently unlock dungeon |
| Cooking (Buffs) | Enable dungeon depth & efficiency |
| Dungeon | End-point challenge, not weapon-gated |

### Core Principles (DO NOT VIOLATE)
1. **Talismans define how you fight** - Playstyle modifiers only
2. **Weapons define what you can efficiently hunt** - Monster-specific bonuses only
3. **Cooking defines where you can go and how deep you can endure** - Access and buffs only
4. **No system replaces another** - Clear boundaries maintained

### Updated Final Design Statement
**Weapons teach you how to harvest the world. Cooking allows you to enter the dungeon. Cooking again allows you to master it.**

This statement replaces all previous summaries and should be used as the authoritative system description.

## 11. Implementation Notes (ModernUO)

### Dungeon Access System
- Unlock stored as character flag in progression table
- Access food OnConsume handler validates recipe and sets flag
- Dungeon entry checks HasUnlockedDungeon flag (no timers or re-consumption)

### Weapon Bonus System
- Monster tag comparison for bonus application
- Zone validation to prevent dungeon bonus creep
- Bonus application on-hit with tag validation

### Cooking Buff System
- Uses Puzzle Cooking System (gump-driven, Stone Oven house addon)
- Standard timed buff system with dungeon-scoped validation
- Buff duration and cooldown configuration
- Dungeon-specific buff validation
- Configurable failure modes (Mode A: no ingredient loss, Mode B: full ingredient loss)

### Quest Tracking
- 4-phase quest chain progression tracking
- Weapon recipe acquisition tracking
- Cooking consumption event handling
- Dungeon unlock status monitoring

## 12. Retention System Integration

This system supports retention through:
- **Permanent Progression**: Dungeon access never lost
- **Repeatable Value**: Buff cooking remains relevant
- **Role Identity**: Weapon specialists and cooks matter
- **Expandability**: New dungeon tiers add buffs, not access grinds
- **Freedom of Choice**: No hard gating, strong incentives

## 13. Balance Safeguards

### Anti-Exploit Measures
- Weapon efficiency vs raw grinding (no bypass)
- Permanent access prevents key farming
- No weapon requirements for dungeon entry
- Contextual bonuses only (no global power)

### Failure States
- Dungeons accessible without weapons (but inefficient)
- Dungeons accessible without buffs (but punishing)
- No permanent progression loss
- No resource hoarding requirements

### System Constraints
- Weapons: External farming only, no dungeon bonuses
- Cooking: No combat stats, only access and buffs
- Talismans: No resource advantages, only playstyle
- Dungeons: End-point challenges, not power sources
