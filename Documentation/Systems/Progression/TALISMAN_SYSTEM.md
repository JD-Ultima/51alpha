# Talisman System

**Authority**: TIER 2 - Implementation Spec
**Authoritative Source**: `Core/DESIGN_DECISIONS.md` §6
**Related**: `Combat/PVP_COMBAT.md`, `JEWELRY_SYSTEM.md`

---

## Overview

Talismans provide PvM bonuses and gate specific abilities. All talisman benefits are **disabled in PvP** to maintain skill-based combat.

---

## Core Design Principle

PvM advantages must not apply in PvP combat. Talismans:
- Provide significant PvM bonuses
- Gate certain abilities to prevent "do everything" builds
- Instantly drop on PvP engagement

---

## Four Talisman Types

| Type | Build Archetype | PvM Bonuses | PvP-Gated Abilities |
|------|-----------------|-------------|---------------------|
| **Dexer** | Pure melee | +15% weapon damage, +5% attack speed | Bushido, Weapon Special Moves |
| **Tamer** | Pet master | +15% pet damage, +10% pet healing | Taming bonuses |
| **Sampire** | Paladin | +10% life leech | Chivalry, Necromancy |
| **Treasure Hunter** | Explorer | +15% chest quality, +25% gold find | Lockpicking bonuses |

---

## Three Tiers Per Type

| Tier | Name Pattern | Relic Cost | Mage Stones | Crafting Skill | Timer |
|------|--------------|------------|-------------|----------------|-------|
| T3 | "Bloodthorn" | 5 Common | 1 | 80 (30% fail) | 7 days |
| T2 | "Bloodmage" | 8 Uncommon + 3 Common | 3 | 100 (40% fail) | 7 days |
| T1 | "Bloodlord" | 10 Rare + 5 Uncommon + 1 Legendary | 5 | 120 (50% fail) | 7 days |

**Mage Stones**: Specific relic drops from dungeon mobs, required for crafting.

---

## Skill Reduction Mechanic

**CRITICAL**: When equipped, talisman **temporarily zeroes** ALL OTHER skills.

### Behavior
- Skills revert to original values when talisman unequipped
- No "skill changed" notifications shown to player
- Skill values stored internally, not permanently modified
- Visually shows 0.0 but original value preserved

This creates OSI 7-skill templates:
- **Dexer**: Keep melee/combat skills, lose mage/craft
- **Tamer**: Keep taming/vet skills, lose combat
- **Sampire**: Keep chiv/necro skills, paladin build
- **TH**: Keep lockpicking/cartography, lose combat

---

## PvP Behavior

### On PvP Engagement

1. Talisman instantly drops to backpack
2. All gated abilities stop working
3. 5-minute cooldown before re-equip allowed
4. Both attacker and defender affected

### Timer Rules

| Action | Timer Effect |
|--------|-------------|
| Enter PvP | Start 5-min countdown |
| Unequip | Pause countdown |
| Re-equip | Resume countdown |
| Timer expires | Bonuses restore |
| Logout | Timer pauses (no talisman equipped) |

---

## PvP Context Detection

PvP context triggers on:
- Player attacks player
- Player attacks player's pet
- Player's pet attacks player
- Player's pet attacks another player's pet
- AoE spell hits player

```csharp
public static bool IsPvPContext(Mobile attacker, Mobile defender)
{
    // Direct player vs player
    if (attacker is PlayerMobile && defender is PlayerMobile)
        return true;

    // Player attacking player's pet
    if (defender is BaseCreature bc && bc.ControlMaster is PlayerMobile)
        return true;

    // Player's pet attacking player
    if (attacker is BaseCreature bc2 && bc2.ControlMaster is PlayerMobile
        && defender is PlayerMobile)
        return true;

    return false;
}
```

---

## Ability Gating

### Chivalry (Sampire Only)

All Chivalry spells require active Sampire talisman:
- Cleanse by Fire
- Close Wounds
- Consecrate Weapon
- Dispel Evil
- Divine Fury
- Enemy of One
- Holy Light
- Noble Sacrifice
- Remove Curse
- Sacred Journey

### Necromancy (Sampire Only)

Necromancy spells gated to Sampire talisman.

### Bushido (Dexer Only)

Bushido abilities require active Dexer talisman.

### Weapon Special Moves (Dexer Only)

Special weapon abilities require active Dexer talisman.

---

## Talisman State Machine

```
┌─────────────┐
│   Active    │ ← Default, bonuses apply
└──────┬──────┘
       │ PvP engagement
       ▼
┌─────────────┐
│  Disabled   │ ← Timer running, no bonuses
└──────┬──────┘
       │ Unequip
       ▼
┌─────────────┐
│   Paused    │ ← Timer frozen
└──────┬──────┘
       │ Re-equip
       ▼
┌─────────────┐
│  Disabled   │ ← Timer resumes
└──────┬──────┘
       │ Timer expires
       ▼
┌─────────────┐
│   Active    │
└─────────────┘
```

---

## Crafting Requirements

### Materials

- **Relics**: Common, Uncommon, Rare, Legendary (dungeon drops)
- **Mage Stones**: Special dungeon mob drops
- **Crafting**: Tinkering + Blacksmithing

### Skill Requirements

| Tier | Tinkering | Blacksmithing |
|------|-----------|---------------|
| T3 | 65 | 65 |
| T2 | 85 | 85 |
| T1 | GM (100) | GM (100) |

---

## Binding Rules

- **Bound on Equip**: Cannot be traded after wearing
- **Character Bound**: Not transferable between characters
- **7-Day Gameplay Timer**: Pauses when offline

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_talisman_states | PvP disable tracking |
| s51a_talisman_timers | Gameplay timer tracking |
| s51a_player_templates | Active talisman/build info |

---

## Configuration

```json
{
  "combat.talisman_pvp_disable_minutes": 5,
  "talisman.dexer_damage_bonus": 0.15,
  "talisman.tamer_damage_bonus": 0.15,
  "talisman.sampire_leech_bonus": 0.10,
  "talisman.th_quality_bonus": 0.15,
  "talisman.gameplay_timer_days": 7
}
```

---

## Testing Matrix

### Timer Behavior

| Test | Initial State | Action | Expected |
|------|---------------|--------|----------|
| Fresh PvP | Active | Attack player | 5 min disable |
| Re-engage | 3 min left | Attack player | 5 min (extended) |
| Unequip | 3 min left | Unequip | Timer paused |
| Re-equip | 3 min paused | Equip | Timer resumes |
| Expire | 0 min left | Wait | Bonuses restore |

### Bonus Application

| Test | Context | Talisman State | Expected |
|------|---------|----------------|----------|
| PvM hit | Player → Monster | Active | +15% damage |
| PvM hit | Player → Monster | Disabled | Base damage |
| PvP hit | Player → Player | Active | Base damage |
| PvP hit | Player → Player | Disabled | Base damage |
