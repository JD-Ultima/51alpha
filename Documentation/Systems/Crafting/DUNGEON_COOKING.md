# Dungeon Catalyst Cooking

**Authority**: TIER 2 - Implementation Spec
**Related**: `Content/DUNGEONS.md`, `Economy/CURRENCIES.md`

---

## Overview

Puzzle-based cooking system for creating dungeon access catalysts and buff items. Uses gump-driven interface with experimental recipe discovery.

---

## Core Mechanics

### Stone Oven

- House addon crafted by Carpenters
- Opens Puzzle Cooking Gump on use
- Required for all catalyst crafting

### Cooking Process

1. Player interacts with Stone Oven
2. Puzzle Cooking Gump opens
3. Player selects ingredients via inventory picker
4. Items remain in inventory until "Bake" pressed
5. Server validates skill, proximity, recipe
6. Atomic transaction: all-or-nothing consumption

---

## Skill Requirements

| Recipe Tier | Cooking Skill |
|-------------|---------------|
| Dungeon Access | 50 |
| Level 1 Buffs | 60 |
| Level 2 Buffs | 80 |
| Level 3 Buffs | 100 |
| Level 4 Buffs | 120 |

**Jewelry Bonus**: Cooking skill augmented by jewelry, capped at +20

---

## Success/Failure System

### Configurable Modes

**Mode A (Learning Friendly)**:
- No ingredient consumption on failure
- Encourages experimentation
- Recommended for new players

**Mode B (High Risk)**:
- All ingredients consumed on failure
- Reinforces economic risk
- Higher stakes gameplay

**Rule**: Partial consumption is explicitly forbidden - all or nothing.

---

## Recipe Discovery

### Knowledge System

- Account-wide binary knowledge
- No progress tracking
- No XP requirements
- Pure knowledge economy

### Discovery Method

- Experimental combination of ingredients
- No recipe lists provided
- Community knowledge sharing encouraged

---

## Ingredient Categories

### Slot Taxonomy

| Slot | Category | Example |
|------|----------|---------|
| 1 | Common Fruit | Apples, Grapes |
| 2 | Rare Fruit | Dungeon-specific |
| 3 | Optional Catalyst | Monster drops |

### Per-Dungeon Requirements

Each dungeon requires:
- 1 fishing aspect ingredient
- 1 humanoid monster drop
- 1 environmental reagent

All thematically linked to dungeon.

---

## Catalyst Types

### Access Catalysts

- Universal access per dungeon
- Crafted once, account-wide unlock
- Permanent access (never revoked)

### Buff Catalysts

- Temporary dungeon-specific buffs
- Duration varies by tier
- Monster-specific bonuses (no PvP impact)

---

## Buff Constraints

**CRITICAL**: Buffs may NOT:
- Modify combat stats
- Affect damage to players
- Impact PvP balance

**Buffs MAY**:
- Reduce dungeon traversal friction
- Provide monster-specific advantages
- Grant quality-of-life bonuses

---

## Economy Integration

### Resource Flow

- Consumable demand without permanent wealth
- Natural gold sinks via failures
- Low/mid-tier mob relevance
- Trade specialization opportunities

### Knowledge Economy

- Recipe knowledge is valuable
- Discovery = competitive advantage
- Community trading of information

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_account_recipes | Known recipes |
| s51a_recipe_definitions | Recipe requirements |
| s51a_catalyst_timers | Active buff tracking |

---

## Configuration

```json
{
  "cooking.failure_mode": "A",
  "cooking.skill_jewelry_cap": 20,
  "cooking.discovery_enabled": true,
  "cooking.buff_pvp_disabled": true
}
```
