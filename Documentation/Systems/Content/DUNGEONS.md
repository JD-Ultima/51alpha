# Dungeon System

**Authority**: TIER 2 - Implementation Spec
**Authoritative Source**: `Core/DESIGN_DECISIONS.md` §10
**Related**: `Crafting/DUNGEON_COOKING.md`, `Economy/CURRENCIES.md`

---

## Overview

Dungeons are the primary source of relics and challenging PvE content. All dungeons follow Felucca rules (full PvP enabled).

---

## Available Dungeons

| Dungeon | Access | Relic Type | Theme |
|---------|--------|------------|-------|
| Despise | Always | Combat | Undead/Vermin |
| Shame | Always | Combat | Elemental |
| Covetous | Always | Combat | Harpy/Gazer |
| Deceit | Always | Arcane | Undead/Mage |
| Destard | Always | Dragon | Dragons |
| Hythloth | Always | Demonic | Demons |
| Wrong | Always | Undead | Undead/Torture |
| Fire | Always | Elemental | Fire creatures |
| Ice | Always | Elemental | Ice creatures |

---

## Dungeon Rules

| Rule | Details |
|------|---------|
| Mounts | Auto-dismissed on entry |
| Pets | Allowed |
| PvP | Full (Felucca rules) |
| Young Players | Cannot enter |
| Recall | Blocked in combat |

---

## Relic System

### Drop Rates (Base)

| Relic Tier | Base Rate | Bonus Rate |
|------------|-----------|------------|
| Common | 5% | 7.5% |
| Uncommon | 1% | 1.5% |
| Rare | 0.1% | 0.15% |
| Legendary | Boss only | 0.01% |

### Drop Sources

- Regular mobs: Common only
- Mini-bosses: Common, Uncommon
- Dungeon bosses: All tiers
- Champion spawns: Higher rates

**IMPORTANT**: Relics ONLY drop from dungeons. Outside world gives gold, not relics.

---

## Weekly Rotation

Weekly bonus rotation announced via Town Cryer:

| Bonus Type | Effect |
|------------|--------|
| Relic Drop | +50% relic drop rate |
| Material | +25% ore/wood/leather yield |
| Gold | +25% gold drops |

### Schedule

- Resets: Monday 5 AM UTC (Midnight EST)
- Duration: 7 days
- Selection: Random dungeon each week

---

## Dungeon Preparation

See `Crafting/DUNGEON_COOKING.md` for catalyst system.

### Access Catalysts

- Per-dungeon crafted items
- Account-wide permanent unlock
- Requires world ingredient gathering

### Buff Catalysts

- Temporary dungeon-specific buffs
- Monster-specific bonuses
- No PvP impact

---

## Boss Mechanics

### Regular Bosses

| Parameter | Value |
|-----------|-------|
| Spawn Rate | 30 min - 2 hours |
| HP Range | 5,000 - 15,000 |
| Loot | Guaranteed relic roll |

### Champion Spawns

| Parameter | Value |
|-----------|-------|
| Activation | Altar click |
| Waves | 4 levels + champion |
| Duration | 30-60 minutes |
| Rewards | Power scrolls, relics |

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_dungeon_bonuses | Weekly rotation |
| s51a_dungeon_boss_spawns | Boss tracking |
| s51a_dungeon_visits | Player analytics |
| s51a_relic_drops | Drop history |

---

## Configuration

```json
{
  "dungeon.mount_auto_dismiss": true,
  "dungeon.young_blocked": true,
  "dungeon.pvp_enabled": true,
  "dungeon.recall_blocked_in_combat": true,
  "dungeon.bonus_rotation_days": 7,
  "dungeon.relic_bonus_multiplier": 1.5,
  "dungeon.gold_bonus_multiplier": 1.25,
  "dungeon.material_bonus_multiplier": 1.25
}
```
