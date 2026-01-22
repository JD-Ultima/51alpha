# Ore & Materials System

**Authority**: TIER 2 - Implementation Spec
**Authoritative Source**: `Core/DESIGN_DECISIONS.md` §16, §18
**Related**: `RUNIC_CRAFTING.md`, `Combat/PVP_COMBAT.md`

---

## Overview

7-tier ore system with seasonal rotation. Each season (3 months), ore names and hues change while mechanical properties remain constant. Iron is always available.

---

## 7-Tier Ore System

### Tier Structure

| Tier | Mining | Plate AR | Gap | Find Rate |
|------|--------|----------|-----|-----------|
| 1 | 0 | 25 | - | 100% |
| 2 | 55 | 30 | +5 | 30% |
| 3 | 65 | 35 | +5 | 22% |
| 4 | 75 | 45 | +10 | 16% |
| 5 | 85 | 55 | +10 | 10% |
| 6 | 95 | 60 | +5 | 5% |
| 7 | 99 | 65 | +5 | 2% |

### Tier Groupings

- **Low (T1-T2)**: Entry level, 25-30 AR
- **Mid (T3-T4)**: Big jump at T4, 35-45 AR
- **High (T5-T6)**: End game, 55-60 AR
- **Elite (T7)**: Best in slot, 65 AR

---

## Seasonal Rotation

### Schedule

| Season | Real-World Dates | Theme |
|--------|-----------------|-------|
| Spring | March 20 - June 20 | Growth, renewal |
| Summer | June 21 - Sept 21 | Heat, ocean |
| Fall | Sept 22 - Dec 20 | Harvest, twilight |
| Winter | Dec 21 - March 19 | Ice, darkness |

### Season Change Behavior

- All ore veins switch to new season's ore types
- Existing items KEEP their original name and hue
- Creates collectible seasonal variants

---

## Seasonal Ore Names

### Spring Ores

| Tier | Name | Hue |
|------|------|-----|
| 1 | Iron | 0 |
| 2 | Dull Copper | 2419 |
| 3 | Copper | 2413 |
| 4 | Bronze | 2418 |
| 5 | Verite | 2207 |
| 6 | Agapite | 2427 |
| 7 | Valorite | 1281 |

### Summer Ores

| Tier | Name | Hue |
|------|------|-----|
| 1 | Iron | 0 |
| 2 | Old Copper | 1159 |
| 3 | Rose | 2948 |
| 4 | Gold | 2213 |
| 5 | Fire | 2527 |
| 6 | Azure | 1165 |
| 7 | Solarium | 2720 |

### Fall Ores

| Tier | Name | Hue |
|------|------|-----|
| 1 | Iron | 0 |
| 2 | Shadow Iron | 1904 |
| 3 | Amethyst | 1163 |
| 4 | Blood Rock | 1218 |
| 5 | Mytheril | 1325 |
| 6 | Dwarven | 1940 |
| 7 | Daemon Steel | 1171 |

### Winter Ores

| Tier | Name | Hue |
|------|------|-----|
| 1 | Iron | 0 |
| 2 | Silver | 1175 |
| 3 | Frost | 1152 |
| 4 | Ice | 2513 |
| 5 | Glacial | 1266 |
| 6 | Obsidian | 1109 |
| 7 | Black Rock | 1150 |

---

## Base Armor Values

| Armor Type | PvP Base AR | Str Req | Meditation |
|------------|-------------|---------|------------|
| Leather | 15 | 15-25 | Full |
| Studded Leather | 15 | 25-35 | Half |
| Bone | 25 (T1) | 40-60 | None |
| Chainmail Tunic | 25 (T1) | 60 | None |
| Ringmail Tunic | 25 (T1) | 40 | None |
| Platemail | 25 (T1) | 70-95 | None |
| Dragon Scale | 25 (T1) | 60-75 | None |

### Chainmail & Ringmail

**REMOVED**: All pieces EXCEPT chest tunics
- Chainmail Coif, Leggings, Gloves - REMOVED
- Ringmail Sleeves, Leggings, Gloves - REMOVED

**KEPT**: Chainmail Tunic and Ringmail Tunic only
- Both use plate AR tier system (T1-T7)
- Lower STR requirement than plate chest

---

## Mining Stones

Rare drops while mining. Apply RNG bonuses to weapons.

### Drop Chance

| Mining Skill | Chance/Swing |
|--------------|-------------|
| 0-49 | 0.1% |
| 50-79 | 0.2% |
| 80-99 | 0.3% |
| GM (100) | 0.5% |

*+0.1% per tier above T1*

### Sharpening Stone (Damage)

| Result | Flat Bonus | Prefix |
|--------|-----------|--------|
| Force | +6 | "a force [weapon]" |
| Power | +8 | "a power [weapon]" |
| Vanquishing | +10 | "a vanquishing [weapon]" |

**RNG by Mining Skill:**

| Skill | Force | Power | Vanq |
|-------|-------|-------|------|
| 0-49 | 80% | 18% | 2% |
| 50-79 | 70% | 25% | 5% |
| 80-99 | 55% | 35% | 10% |
| GM | 40% | 40% | 20% |

### Accuracy Stone (Hit Chance)

| Result | Tactics | Hit% | Prefix |
|--------|---------|------|--------|
| Accurate | +5 | +2% | "accurate" |
| Surpassingly | +10 | +4% | "surpassingly accurate" |
| Eminently | +15 | +6% | "eminently accurate" |
| Exceedingly | +20 | +8% | "exceedingly accurate" |
| Supremely | +25 | +10% | "supremely accurate" |

### Stone Rules

- Consumable (one-time use)
- Bonus is PERMANENT
- Only ONE stone per weapon per type
- Both types can be on same weapon
- Applied via Blacksmithing menu

---

## Creature-Derived Armor

Special armor sets crafted via Tailoring from creature materials.

### Armor Types

| Type | Theme | Per-Piece Bonus | Vulnerability |
|------|-------|-----------------|---------------|
| Lich Bone | Mana sustain | +3 Max Mana | +5% from Lichbane |
| Dragon Scale | Fire mitigation | -5% Fire damage | +5% from Dragonbane |
| Titan Skin | High HP | +4 Max HP | +5% from Titanbane |
| Ophidian Silk | Large mana pool | +5 Max Mana | +5% from Ophidianbane |

### Full Suit Bonuses (5+ pieces)

| Type | Full Suit Bonus |
|------|-----------------|
| Lich Bone | +10 Max Mana |
| Dragon Scale | -5% additional Fire damage |
| Titan Skin | +15 Max HP |
| Ophidian Silk | -10% mana cost |

### Vulnerability Scaling

| Pieces | Vulnerability |
|--------|---------------|
| 1 | +5% |
| 2 | +10% |
| 3 | +15% |
| 4 | +20% |
| 5+ | +25% (cap) |

### Weapon Counters (Bane Weapons)

Crafted bonuses applying +25% damage vs matching armor:
- Lichbane (vs Lich Bone)
- Dragonbane (vs Dragon Scale)
- Titanbane (vs Titan Skin)
- Ophidianbane (vs Ophidian Silk)

All four bonuses may exist on one weapon.

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_ore_tiers | Tier definitions |
| s51a_seasonal_ores | Season-specific names/hues |
| s51a_mining_stones | Stone drop tracking |

---

## Configuration

```json
{
  "ore.tier_count": 7,
  "ore.seasonal_rotation": true,
  "mining.stone_drop_base": 0.001,
  "mining.stone_drop_skill_bonus": 0.001,
  "mining.stone_drop_tier_bonus": 0.001
}
```
