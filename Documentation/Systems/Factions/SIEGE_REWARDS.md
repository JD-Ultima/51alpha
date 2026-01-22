# Siege Rewards

**Authority**: TIER 2 - Implementation Spec
**Authoritative Source**: `Core/DESIGN_DECISIONS.md` §7, §8
**Related**: `FACTION_OVERVIEW.md`, `SIEGE_BATTLES.md`

---

## Overview

Performance-based PvP reward system providing temporary stat boosts through siege participation. Rewards active engagement while maintaining strict PvP/PvM separation.

---

## Siege Coins (Currency)

| Property | Value |
|----------|-------|
| Tradable | No (character-bound) |
| Source | Siege participation only |
| Reset | Wiped each season |

### Acquisition (Contribution-Based)

Coins awarded based on contribution score, not faction victory alone.

**PvP Combat:**
- Damage dealt to enemy players
- Healing done to allied players
- Killing blows (victim must have dealt damage back)
- Damage received while alive

**Objective Play:**
- Time contesting objectives
- Successful captures/defenses
- Interrupting enemy captures

**Victory Modifier:**
- Winning faction: Small multiplier bonus
- Losing faction: Still earns meaningful coins
- Result: Losing well > Winning AFK

---

## Jewelry System

**Authority**: See `Core/DESIGN_DECISIONS.md` §7

### Slots (4 pieces)

| Slot | Bonus |
|------|-------|
| Ring | +2.5 Str/Dex/Int |
| Bracelet | +2.5 Str/Dex/Int |
| Earrings | +2.5 Str/Dex/Int |
| Glasses | +2.5 Str/Dex/Int |
| **Full Set** | **+10 Str/Dex/Int** |

### Duration

- 7-day continuous timer
- Starts on first equip
- Counts even when offline/unequipped
- After 7 days: Item expires

### Restrictions

- Subject to 125 hard stat cap
- Disabled when talisman equipped
- Bonuses do NOT stack beyond +10

---

## Clothing System

### Slots (4 pieces)

| Slot Options | AR Bonus |
|--------------|----------|
| Faction Apron | +2.5 |
| Faction Robe OR Sash | +2.5 |
| Faction Doublet OR Tunic | +2.5 |
| Faction Skirt OR Kilt | +2.5 |
| **Full Set** | **+10 AR** |

### Properties

- Counts as clothing, not armor
- Faction-hued (visually identifiable)
- 180 hours logged-in duration
- Disabled when talisman equipped

---

## Faction Mount

| Property | Value |
|----------|-------|
| Stat Bonus | +10 Dexterity |
| Duration | 180 hours logged-in |
| Visual | Faction-colored |
| Talisman | Disabled when equipped |

Dex bonus counts toward the +10 stat cap.

---

## Faction Banner

| Property | Value |
|----------|-------|
| Type | Cosmetic/social |
| Name Format | "Bridgefolk S1 Banner" |
| Placement | Houses or carried |
| Duration | No decay (cosmetic) |

---

## Stat Stacking Rules

### Hard Cap: 125

All sources stack together, capped at 125 per stat.

**Example DEX Stack:**
```
Base DEX:              100
Siege Jewelry:         +10
Faction Mount:         +10
Bless Spell:           +5
Greater Agility Pot:   +10
─────────────────────────
Raw Total:             135 → Capped at 125
```

### Buff Sources

| Source | STR | DEX | INT |
|--------|-----|-----|-----|
| Siege Jewelry (full) | +10 | +10 | +10 |
| Faction Mount | - | +10 | - |
| Bless Spell | +5 | +5 | +5 |
| Greater Str Potion | +10 | - | - |
| Greater Agi Potion | - | +10 | - |
| Special Fish (GM Cooking) | - | - | +5/+10 |

---

## Talisman Separation

**Absolute Rule**: If talisman equipped → ALL siege bonuses DISABLED

### Affected Items
- Jewelry stat bonuses
- Clothing AR bonuses
- Mount stat bonuses

### Behavior
- Items may remain equipped
- Bonuses dynamically turn off
- Bonuses return when talisman unequipped

### Still Work With Talismans
- Spell buffs (Bless, Minor buffs)
- Potions
- Food buffs (fish)

---

## Anti-Abuse Safeguards

| Protection | Method |
|------------|--------|
| AFK abuse | Minimum damage thresholds |
| Kill trading | Victim must fight back |
| Target farming | Diminishing returns per target |
| Alt accounts | IP/account linkage |

---

## Seasonal Reset

At season end:
- Siege coins: Wiped
- Siege items: Expire naturally or wiped
- Cosmetics/titles: May persist
- System: Resets cleanly each quarter

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_siege_coins | Player balances |
| s51a_siege_contribution | Per-siege scores |
| s51a_siege_items | Item decay tracking |

---

## Configuration

```json
{
  "siege.coin_victory_multiplier": 1.25,
  "siege.jewelry_duration_days": 7,
  "siege.clothing_duration_hours": 180,
  "siege.mount_duration_hours": 180,
  "siege.stat_hard_cap": 125,
  "siege.jewelry_stat_bonus": 2.5,
  "siege.clothing_ar_bonus": 2.5,
  "siege.mount_dex_bonus": 10
}
```
