# Jewelry System

**Authority**: TIER 2 - Implementation Spec
**Authoritative Source**: `Core/DESIGN_DECISIONS.md` §7
**Related**: `TALISMAN_SYSTEM.md`, `Factions/SIEGE_REWARDS.md`

---

## Overview

Two jewelry systems exist:
1. **Siege Jewelry** - Temporary stat boosts from faction participation
2. **PvP Jewelry** - Permanent stat boosts for PvP-focused characters

Both are disabled when a talisman is equipped.

---

## PvP Jewelry Slots

| Slot | Stats Per Piece | Layer Notes |
|------|-----------------|-------------|
| Ring | +2 Str/Dex/Int | Standard layer |
| Earrings | +2 Str/Dex/Int | Standard layer |
| Bracelet | +3 Str/Dex/Int | Standard layer |
| Glasses | +3 Str/Dex/Int | **NEW LAYER** (separate from helmet) |
| **Full Set** | **+10 Str/Dex/Int** | |

**Implementation Note**: Glasses require a new equipment layer, not a virtual slot on the helmet layer.

---

## Timer System

### 7-Day Continuous Timer

- Starts on first equip
- Counts **continuously** (even offline/unequipped)
- After 7 days: Item expires, needs replacement

**Note**: Different from siege rewards which use logged-in time for clothing.

### Timer Display

- Visible decay timer on item
- Warning at 24 hours remaining
- Warning at 1 hour remaining

---

## Stat Caps

### Hard Cap: 125

All stat bonuses are subject to the 125 hard cap per stat.

**Stacking Example:**
```
Base STR:              100
Jewelry (full set):    +10
Bless Spell:           +5
Strength Potion:       +10
Greater Str Potion:    +10
─────────────────────────
Raw Total:             135 → Capped at 125
```

### Jewelry Cap: +10

Jewelry bonuses do NOT stack beyond +10 per stat (full set).

---

## Talisman Interaction

**Rule**: If talisman equipped → Jewelry bonuses DISABLED

### Behavior

- Items may remain equipped
- Stat bonuses dynamically turn off
- Bonuses return when talisman unequipped

### Rationale

- PvM players choose: Talisman power OR Jewelry power
- No PvM → PvP power stacking
- Clear build identity

---

## Siege Jewelry vs PvP Jewelry

| Property | Siege Jewelry | PvP Jewelry |
|----------|--------------|-------------|
| Source | Siege coin vendor | Crafted/Looted |
| Duration | 7 days continuous | 7 days continuous |
| Stats | +2/+3 per piece | +2/+3 per piece |
| Talisman | Disabled | Disabled |
| Character Bound | Yes | Yes |

---

## Binding Rules

- **Bound on Equip**: Cannot be traded after wearing
- **Character Bound**: Not transferable between accounts
- **Single Owner**: Once equipped, permanently bound

---

## Obtaining PvP Jewelry

### Crafting

- **Tinkering**: Required skill
- **Materials**: Gems + precious metals
- **Skill Level**: GM required for full stat pieces

### Loot

- High-end dungeon bosses
- Rare PvM drops
- Event rewards

### Siege Vendor

See `Factions/SIEGE_REWARDS.md` for siege coin acquisition.

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_jewelry_timers | Decay timer tracking |
| s51a_jewelry_bindings | Character binding |
| s51a_player_stats | Stat bonus tracking |

---

## Configuration

```json
{
  "jewelry.duration_days": 7,
  "jewelry.ring_bonus": 2,
  "jewelry.earring_bonus": 2,
  "jewelry.bracelet_bonus": 3,
  "jewelry.glasses_bonus": 3,
  "jewelry.stat_cap_per_type": 10,
  "jewelry.global_stat_cap": 125,
  "jewelry.talisman_disables": true
}
```

---

## Buff Stacking Reference

### All Sources That Stack

| Source | STR | DEX | INT |
|--------|-----|-----|-----|
| PvP Jewelry (full) | +10 | +10 | +10 |
| Siege Mount | - | +10 | - |
| Bless Spell | +5 | +5 | +5 |
| Minor Str/Agi/Int | +5 | +5 | +5 |
| Strength Potion | +5/+10 | - | - |
| Agility Potion | - | +5/+10 | - |
| Special Fish | - | - | +5/+10 |

### What Doesn't Stack

- Multiple jewelry sets
- Multiple of same potion
- Jewelry + active talisman

---

## FAQ

**Q: Can I wear jewelry with a talisman?**
A: Yes, but the jewelry bonuses won't apply while talisman is equipped.

**Q: Does jewelry work in PvP?**
A: Yes, jewelry bonuses apply in PvP (unlike runic item bonuses).

**Q: What happens when jewelry expires?**
A: Item is destroyed. You need to obtain a new piece.

**Q: Can I trade jewelry?**
A: Only before equipping. Once worn, it's bound to that character.
