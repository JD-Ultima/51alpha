# 51Alpha Shard - Weapons Reference Document

This document contains weapon damage tables, combat formulas, and the decision to adopt Imagine Nation weapon values.

**Status:** DECISION MADE - Adopting Imagine Nation weapon values for both PvP and PvM.

---

## CRITICAL DECISION: Weapon Damage Values

### Decision Summary

**51Alpha will use Imagine Nation's weapon damage values** for all weapons.

| Aspect | Decision |
|--------|----------|
| **Weapon Base Damage** | Use IN values (higher minimums, tighter ranges) |
| **PvP Combat** | IN-style formulas (already implemented) |
| **PvM Combat** | Accept damage buff, adjust monster HP |
| **Monster HP Adjustment** | **+60% HP to all monsters** |
| **Arms Lore** | Rework to show actual min/max damage numbers |

### Why This Change?

**Problem:** Current AOS damage values (11-13 katana, 18-19 halberd) are too low for the IN-style AR absorption formula. Result: most hits deal 0-10 damage in PvP.

**Solution:** IN values (13-24 katana, 25-52 halberd) work correctly with the absorption formula and provide meaningful damage.

---

## Combat Formula Reference

### Armor Absorption Formulas

| Context | Formula | Example (85 AR) |
|---------|---------|-----------------|
| **PvP** | `Random(AR/4.3 to AR/2.4)` | 20-35 absorbed |
| **PvM** | `Random(AR/2 to AR)` | 42-85 absorbed |

### Hit Chance Formula (PvP)

```
hitChance = 0.69 × ((WeaponSkill + Tactics×2) / 300)
```

**Max hit chance at GM:** 69%

### Damage Skill Multipliers

| Skill | At 100 | At 120 |
|-------|--------|--------|
| Tactics | +50% | +70% |
| Strength | +20% | +24% |
| Anatomy | +30% | +34% |
| Lumberjacking (axes) | +30% | +34% |
| **Total (non-axe)** | **~2x** | **~2.24x** |
| **Total (axe)** | **~2.3x** | **~2.62x** |

---

## Full Combat Analysis: Why IN Values Are Needed

### Current AOS Values vs T7 Armor (85 AR) in PvP

| Weapon | AOS Base | After Skills (2x) | - Absorption (20-35) | **Net Damage** |
|--------|----------|-------------------|----------------------|----------------|
| Katana | 11-13 | 22-26 | 22-26 minus 20-35 | **0-6 dmg** |
| Kryss | 10-12 | 20-24 | 20-24 minus 20-35 | **0-4 dmg** |
| Halberd | 18-19 | 36-38 | 36-38 minus 20-35 | **1-18 dmg** |

**Problem:** Most hits deal near-zero damage. Fights take forever.

### With Imagine Nation Values vs T7 Armor (85 AR) in PvP

| Weapon | IN Base | After Skills (2x) | - Absorption (20-35) | **Net Damage** |
|--------|---------|-------------------|----------------------|----------------|
| Katana | 13-24 | 26-48 | 26-48 minus 20-35 | **0-28 dmg** |
| Kryss | 11-18 | 22-36 | 22-36 minus 20-35 | **0-16 dmg** |
| Halberd | 25-52 | 50-104 | 50-104 minus 20-35 | **15-84 dmg** |

**Solution:** Weapons consistently deal meaningful damage. Two-handers hit hard as intended.

---

## Monster HP Adjustment Calculation

### Damage Increase by Weapon Type (AOS → IN)

| Weapon | AOS Avg | IN Avg | % Increase |
|--------|---------|--------|------------|
| **One-Handed** |
| Katana | 12 | 18.5 | +54% |
| Kryss | 11 | 14.5 | +32% |
| Broadsword | 14.5 | 17.5 | +21% |
| Scimitar | 14 | 18.5 | +32% |
| Longsword | 15.5 | 21.5 | +39% |
| Viking Sword | 16 | 23.5 | +47% |
| Mace | 13 | 22.5 | +73% |
| **Two-Handed** |
| Battle Axe | 16 | 30.5 | +91% |
| Halberd | 18.5 | 38.5 | +108% |
| War Hammer | 17.5 | 38 | +117% |
| Bardiche | 17.5 | 33.5 | +91% |
| **Ranged** |
| Bow | 17 | ~18 | +6% |
| Crossbow | 20 | ~20 | 0% |

### Category Averages

| Weapon Type | Average Damage Increase |
|-------------|------------------------|
| One-Handed | ~40% |
| Two-Handed | ~100% (double!) |
| Ranged | ~3% (negligible) |

### Recommended Monster HP Multiplier

| Approach | HP Multiplier | Effect |
|----------|---------------|--------|
| Conservative | +50% | 1H weapons feel strong, 2H trivialize content |
| **Balanced (CHOSEN)** | **+60%** | Good balance for mixed weapon usage |
| Aggressive | +80% | Normalizes 2H power, 1H feels weak |

**Decision: Apply +60% HP to all monsters**

This keeps:
- One-handed weapons feeling slightly stronger than before
- Two-handed weapons powerful but not trivializing content
- Room for player skill/gear to matter

---

## Damage Level Bonuses

### 51Alpha Damage Level System
| Level | Enum Value | Bonus |
|-------|------------|-------|
| Regular | 0 | +0 |
| Ruin | 1 | +1 |
| Might | 2 | +3 |
| Force | 3 | +5 |
| Power | 4 | +7 |
| Vanq | 5 | +9 |

*Damage levels tied to ore types:*
- Shadow Iron = Ruin
- Copper = Ruin + Surpassingly Accurate
- Bronze = Might + Surpassingly Accurate
- Gold = Force + Eminently Accurate
- Agapite = Power + Eminently Accurate
- Verite = Power + Exceedingly Accurate
- Valorite = Vanq + Supremely Accurate

### Imagine Nation Damage Level System (Sphere-style)
| Level | Enum Value | Bonus |
|-------|------------|-------|
| Regular | 0 | +0 |
| Ruin | 1 | +1.2 |
| Might | 2 | +3.4 |
| Force | 3 | +5.6 |
| Power | 4 | +7.8 |
| Vanq | 5 | +10 |

*Formula: `(2.2 * DamageLevel) - 1`*

---

## 51Alpha Target Weapon Values (Adopting IN)

### Table 1: Base Weapon Damage (Imagine Nation Style)

| Weapon | Skill | Min | Max | Avg | Regular | Force | Power | Vanq |
|--------|-------|-----|-----|-----|---------|-------|-------|------|
| **SWORDS** |
| Katana | Swords | 13 | 24 | 18.5 | 13-24 | 18-29 | 20-31 | 22-33 |
| Broadsword | Swords | 10 | 25 | 17.5 | 10-25 | 15-30 | 17-32 | 19-34 |
| Longsword | Swords | 13 | 30 | 21.5 | 13-30 | 18-35 | 20-37 | 22-39 |
| Viking Sword | Swords | 12 | 35 | 23.5 | 12-35 | 17-40 | 19-42 | 21-44 |
| Scimitar | Swords | 12 | 25 | 18.5 | 12-25 | 17-30 | 19-32 | 21-34 |
| Cutlass | Swords | 12 | 24 | 18 | 12-24 | 17-29 | 19-31 | 21-33 |
| **FENCING** |
| Kryss | Fencing | 11 | 18 | 14.5 | 11-18 | 16-23 | 18-25 | 20-27 |
| Short Spear | Fencing | 15 | 29 | 22 | 15-29 | 20-34 | 22-36 | 24-38 |
| Spear | Fencing | 15 | 34 | 24.5 | 15-34 | 20-39 | 22-41 | 24-43 |
| War Fork | Fencing | 16 | 36 | 26 | 16-36 | 21-41 | 23-43 | 25-45 |
| **MACES** |
| Mace | Macing | 12 | 33 | 22.5 | 12-33 | 17-38 | 19-40 | 21-42 |
| War Mace | Macing | 14 | 30 | 22 | 14-30 | 19-35 | 21-37 | 23-39 |
| War Hammer | Macing | 29 | 47 | 38 | 29-47 | 34-52 | 36-54 | 38-56 |
| Maul | Macing | 16 | 29 | 22.5 | 16-29 | 21-34 | 23-36 | 25-38 |
| Hammer Pick | Macing | 15 | 27 | 21 | 15-27 | 20-32 | 22-34 | 24-36 |
| **AXES** |
| Battle Axe | Swords | 25 | 36 | 30.5 | 25-36 | 30-41 | 32-43 | 34-45 |
| Double Axe | Swords | 20 | 32 | 26 | 20-32 | 25-37 | 27-39 | 29-41 |
| Executioner's Axe | Swords | 16 | 35 | 25.5 | 16-35 | 21-40 | 23-42 | 25-44 |
| Large Battle Axe | Swords | 20 | 40 | 30 | 20-40 | 25-45 | 27-47 | 29-49 |
| Two Handed Axe | Swords | 20 | 38 | 29 | 20-38 | 25-43 | 27-45 | 29-47 |
| Hatchet | Swords | 14 | 28 | 21 | 14-28 | 19-33 | 21-35 | 23-37 |
| **POLEARMS** |
| Halberd | Swords | 25 | 52 | 38.5 | 25-52 | 30-57 | 32-59 | 34-61 |
| Bardiche | Swords | 25 | 42 | 33.5 | 25-42 | 30-47 | 32-49 | 34-51 |
| **ARCHERY** |
| Bow | Archery | 15 | 19 | 17 | 15-19 | 20-24 | 22-26 | 24-28 |
| Crossbow | Archery | 18 | 22 | 20 | 18-22 | 23-27 | 25-29 | 27-31 |
| Heavy Crossbow | Archery | 19 | 24 | 21.5 | 19-24 | 24-29 | 26-31 | 28-33 |

---

### Table 2: Damage at 100 Stats / 100 Skills

**Multiplier:** ~2x base (non-axe), ~2.3x base (axe with Lumberjacking)

| Weapon | Skill | Regular | Force | Power | Vanq |
|--------|-------|---------|-------|-------|------|
| **SWORDS** |
| Katana | Swords | 26-48 | 36-58 | 40-62 | 44-66 |
| Broadsword | Swords | 20-50 | 30-60 | 34-64 | 38-68 |
| Longsword | Swords | 26-60 | 36-70 | 40-74 | 44-78 |
| Viking Sword | Swords | 24-70 | 34-80 | 38-84 | 42-88 |
| Scimitar | Swords | 24-50 | 34-60 | 38-64 | 42-68 |
| **FENCING** |
| Kryss | Fencing | 22-36 | 32-46 | 36-50 | 40-54 |
| Short Spear | Fencing | 30-58 | 40-68 | 44-72 | 48-76 |
| Spear | Fencing | 30-68 | 40-78 | 44-82 | 48-86 |
| **MACES** |
| Mace | Macing | 24-66 | 34-76 | 38-80 | 42-84 |
| War Mace | Macing | 28-60 | 38-70 | 42-74 | 46-78 |
| War Hammer | Macing | 58-94 | 68-104 | 72-108 | 76-112 |
| **AXES** (with 100 Lumberjacking ~2.3x) |
| Battle Axe | Swords | 58-83 | 69-94 | 74-99 | 78-104 |
| Halberd | Swords | 58-120 | 69-131 | 74-136 | 78-140 |

---

### Table 3: Damage at 120 Stats / 120 Skills

**Multiplier:** ~2.24x base (non-axe), ~2.62x base (axe with Lumberjacking)

| Weapon | Skill | Regular | Force | Power | Vanq |
|--------|-------|---------|-------|-------|------|
| **SWORDS** |
| Katana | Swords | 29-54 | 40-65 | 45-69 | 49-74 |
| Broadsword | Swords | 22-56 | 34-67 | 38-72 | 43-76 |
| Longsword | Swords | 29-67 | 40-78 | 45-83 | 49-87 |
| Viking Sword | Swords | 27-78 | 38-90 | 43-94 | 47-99 |
| Scimitar | Swords | 27-56 | 38-67 | 43-72 | 47-76 |
| **FENCING** |
| Kryss | Fencing | 25-40 | 36-52 | 40-56 | 45-60 |
| Short Spear | Fencing | 34-65 | 45-76 | 49-81 | 54-85 |
| Spear | Fencing | 34-76 | 45-87 | 49-92 | 54-96 |
| **MACES** |
| Mace | Macing | 27-74 | 38-85 | 43-90 | 47-94 |
| War Mace | Macing | 31-67 | 43-78 | 47-83 | 52-87 |
| War Hammer | Macing | 65-105 | 76-117 | 81-121 | 85-126 |
| **AXES** (with 120 Lumberjacking ~2.62x) |
| Battle Axe | Swords | 66-94 | 79-107 | 84-113 | 89-118 |
| Halberd | Swords | 66-136 | 79-149 | 84-155 | 89-160 |

---

## Damage vs Armor Scenarios (With IN Values)

### PvP: Player vs Player (T7 Armor = 85 AR)

**Absorption:** 20-35 damage absorbed per hit

| Weapon | Scaled Damage (100 skills) | After Absorption | Avg Net Damage |
|--------|---------------------------|------------------|----------------|
| Kryss | 22-36 | 0-16 | ~8 |
| Katana | 26-48 | 0-28 | ~14 |
| Broadsword | 20-50 | 0-30 | ~12 |
| Longsword | 26-60 | 0-40 | ~17 |
| War Hammer | 58-94 | 23-74 | ~49 |
| Halberd | 58-120 | 23-100 | ~55 |

### PvP: Time to Kill Analysis (100 HP target, 69% hit chance)

| Weapon | Avg Net Damage | Hits to Kill | Swings Needed | ~Time (3s swing) |
|--------|----------------|--------------|---------------|------------------|
| Kryss | 8 | 13 | 19 | ~57 seconds |
| Katana | 14 | 8 | 12 | ~36 seconds |
| Longsword | 17 | 6 | 9 | ~27 seconds |
| War Hammer | 49 | 3 | 5 | ~15 seconds |
| Halberd | 55 | 2 | 3 | ~9 seconds |

*Note: Does not account for parry (20%), healing, or running.*

---

## Arms Lore Rework Required

### Current Behavior
Arms Lore shows vague damage tiers ("This weapon might scratch someone") based on average damage divided into 6 buckets.

### Required Change
Arms Lore must show **actual min-max damage values**:

```
"This katana deals 13 to 24 damage."
"This vanquishing halberd deals 34 to 61 damage."
```

### Implementation Notes
- Display `MinDamage` and `MaxDamage` properties directly
- Include damage level bonus in displayed values
- Consider showing: base damage, damage level, and final range

---

## Implementation Checklist

### Weapon Value Changes
- [ ] Update all weapon files with IN MinDamage/MaxDamage values
- [ ] Verify damage level bonuses apply correctly
- [ ] Test damage calculations in PvP scenarios
- [ ] Test damage calculations in PvM scenarios

### Monster HP Adjustment
- [ ] Apply +60% HP multiplier to all BaseCreature
- [ ] Test dungeon difficulty with new values
- [ ] Adjust specific bosses if needed

### Arms Lore Rework
- [ ] Modify ArmsLore.cs to show actual damage numbers
- [ ] Display format: "This [weapon] deals X to Y damage"
- [ ] Include damage level bonus in displayed range
- [ ] Test with various weapon types and qualities

---

## Comparison: AOS vs IN vs Pre-AOS

| Weapon | AOS (Current) | IN (Target) | Pre-AOS (Old) |
|--------|---------------|-------------|---------------|
| Katana | 11-13 | 13-24 | 5-26 |
| Kryss | 10-12 | 11-18 | 3-28 |
| Broadsword | 14-15 | 10-25 | 5-29 |
| Halberd | 18-19 | 25-52 | 5-49 |
| Battle Axe | 15-17 | 25-36 | 6-38 |
| War Hammer | 17-18 | 29-47 | 8-36 |

**Key Insight:**
- AOS = Very tight ranges, low values
- IN = Medium ranges, higher minimums
- Pre-AOS = Wide ranges, low minimums

IN provides the best balance: consistent damage (higher floors) while still having meaningful variance.

---

## Change Log

| Date | Change | Reason |
|------|--------|--------|
| 2026-01-21 | Initial document | Combat reference needed |
| 2026-01-21 | Added IN comparison tables | Design comparison |
| 2026-01-21 | **DECISION: Adopt IN weapon values** | AOS values too low for IN absorption formula |
| 2026-01-21 | Added monster HP adjustment (+60%) | Compensate for weapon damage buff |
| 2026-01-21 | Added Arms Lore rework requirement | Show actual damage numbers |
| 2026-01-21 | Added full combat analysis | Document system interactions |

---

*Document Updated: 2026-01-21*
*Decision Status: APPROVED - Implement IN weapon values with +60% monster HP*
