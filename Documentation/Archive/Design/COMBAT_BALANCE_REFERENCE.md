# Combat Balance Reference Document

**Status**: REFERENCE - Complete Combat Data for Balance Decisions
**Last Updated**: 2025-01-14
**Purpose**: Comprehensive reference for all combat mechanics, weapon damage, armor values, and balance calculations

---

## Table of Contents

1. [Damage Formula](#damage-formula)
2. [Weapon Damage Tables](#weapon-damage-tables)
3. [Armor Values](#armor-values)
4. [Shield Values](#shield-values)
5. [Damage Modifiers](#damage-modifiers)
6. [Parry System](#parry-system)
7. [Zero Damage Thresholds](#zero-damage-thresholds)
8. [Balance Analysis](#balance-analysis)
9. [Code Locations](#code-locations)

---

## Damage Formula

### The Core AR Absorption Formula

**Location:** `Projects/UOContent/Items/Weapons/BaseWeapon.cs` (lines 1635-1703)

```
Damage Absorbed = Random value between (AR/2) and AR
Final Damage = Original Damage - Absorbed
If Final Damage < 0, Final Damage = 0
```

**Key Points:**
- This is a **random roll each hit** - not a flat reduction
- Minimum absorption is always AR/2
- Maximum absorption is AR
- Average absorption is 75% of AR (midpoint of AR/2 to AR)

### Example Calculations

| AR Value | Min Absorbed | Max Absorbed | Avg Absorbed |
|----------|--------------|--------------|--------------|
| 20 | 10 | 20 | 15 |
| 40 | 20 | 40 | 30 |
| 60 | 30 | 60 | 45 |
| 80 | 40 | 80 | 60 |
| 100 | 50 | 100 | 75 |

### When Does Damage = 0?

| Scenario | Condition |
|----------|-----------|
| **Sometimes 0** | AR ≥ Incoming Damage (50% chance when AR = damage) |
| **Usually 0** | AR ≥ 1.5x Incoming Damage |
| **Always 0** | AR ≥ 2x Incoming Damage (minimum absorb covers full hit) |

**Critical Insight:** If AR/2 ≥ incoming damage, the target ALWAYS takes 0 damage.

---

## Weapon Damage Tables

### Damage Ranges by Category

All damage values are base AOS (Age of Shadows) damage before any modifiers.

#### SWORDS (Swordsmanship Skill)

| Weapon | Min | Max | Avg | Speed | STR | Hands | Primary Ability | Secondary Ability |
|--------|-----|-----|-----|-------|-----|-------|-----------------|-------------------|
| Kryss | 10 | 12 | 11 | 2.00s | 10 | 1H | Armor Ignore | Infectious Strike |
| Cutlass | 11 | 13 | 12 | 2.50s | 25 | 1H | Bleed Attack | Shadow Strike |
| Wakizashi | 11 | 13 | 12 | 2.50s | 20 | 1H | Frenzied Whirlwind | Double Strike |
| Bokuto | 9 | 11 | 10 | 2.00s | 20 | 1H | Feint | Nerve Strike |
| GlassSword | 11 | 15 | 13 | 2.75s | 20 | 1H | Bleed Attack | Mortal Strike |
| CrescentBlade | 11 | 14 | 12.5 | 2.50s | 55 | 1H | Double Strike | Mortal Strike |
| Scimitar | 13 | 15 | 14 | 3.00s | 25 | 1H | Double Strike | Paralyzing Blow |
| Katana | 11 | 13 | 12 | 2.50s | 25 | 1H | Double Strike | Armor Ignore |
| RadiantScimitar | 12 | 14 | 13 | 2.50s | 20 | 1H | Whirlwind Attack | Bladeweave |
| ElvenMachete | 13 | 15 | 14 | 2.75s | 20 | 1H | Defense Mastery | Bladeweave |
| Broadsword | 14 | 15 | 14.5 | 3.25s | 30 | 1H | Crushing Blow | Armor Ignore |
| Longsword | 15 | 16 | 15.5 | 3.50s | 35 | 1H | Armor Ignore | Concussion Blow |
| VikingSword | 15 | 17 | 16 | 3.75s | 40 | 1H | Crushing Blow | Paralyzing Blow |
| BoneHarvester | 13 | 15 | 14 | 3.00s | 25 | 1H | Paralyzing Blow | Mortal Strike |
| BloodBlade | 10 | 12 | 11 | 2.00s | 10 | 1H | Bleed Attack | Paralyzing Blow |
| DreadSword | 14 | 18 | 16 | 3.50s | 35 | 1H | Crushing Blow | Concussion Blow |
| Lance | 17 | 18 | 17.5 | 4.50s | 95 | 1H | Dismount | Concussion Blow |
| Daisho | 13 | 15 | 14 | 2.75s | 40 | 2H | Feint | Double Strike |
| NoDachi | 16 | 18 | 17 | 3.50s | 40 | 2H | Crushing Blow | Riding Swipe |
| RuneBlade | 15 | 17 | 16 | 3.00s | 30 | 2H | Disarm | Bladeweave |

#### AXES (Swordsmanship Skill + Lumberjacking Bonus)

| Weapon | Min | Max | Avg | Speed | STR | Hands | Primary Ability | Secondary Ability |
|--------|-----|-----|-----|-------|-----|-------|-----------------|-------------------|
| Hatchet | 13 | 15 | 14 | 2.75s | 20 | 1H | Armor Ignore | Disarm |
| Axe | 14 | 16 | 15 | 3.00s | 35 | 1H | Crushing Blow | Dismount |
| DoubleAxe | 15 | 17 | 16 | 3.25s | 45 | 2H | Double Strike | Whirlwind Attack |
| ExecutionersAxe | 15 | 17 | 16 | 3.25s | 40 | 1H | Bleed Attack | Mortal Strike |
| Pickaxe | 13 | 15 | 14 | 3.00s | 50 | 1H | Double Strike | Disarm |
| WarAxe | 14 | 15 | 14.5 | 3.25s | 35 | 1H | Armor Ignore | Bleed Attack |
| BattleAxe | 15 | 17 | 16 | 3.50s | 35 | 2H | Bleed Attack | Concussion Blow |
| TwoHandedAxe | 16 | 17 | 16.5 | 3.50s | 40 | 2H | Double Strike | Shadow Strike |
| LargeBattleAxe | 16 | 17 | 16.5 | 3.75s | 80 | 2H | Whirlwind Attack | Bleed Attack |
| OrnateAxe | 18 | 20 | 19 | 3.50s | 45 | 2H | Disarm | Crushing Blow |
| GuardianAxe | 18 | 20 | 19 | 3.50s | 45 | 2H | Disarm | Crushing Blow |
| HeavyOrnateAxe | 18 | 20 | 19 | 3.50s | 45 | 2H | Disarm | Crushing Blow |
| SingingAxe | 18 | 20 | 19 | 3.50s | 45 | 2H | Disarm | Crushing Blow |
| ThunderingAxe | 18 | 20 | 19 | 3.50s | 45 | 2H | Disarm | Crushing Blow |
| DualShortAxes | 14 | 17 | 15.5 | 3.00s | 35 | 2H | Double Strike | Infectious Strike |

#### MACES/BASHING (Macing Skill)

| Weapon | Min | Max | Avg | Speed | STR | Hands | Primary Ability | Secondary Ability |
|--------|-----|-----|-----|-------|-----|-------|-----------------|-------------------|
| MagicWand | 9 | 11 | 10 | 2.75s | 5 | 1H | Dismount | Disarm |
| Club | 11 | 13 | 12 | 2.50s | 40 | 1H | Shadow Strike | Dismount |
| Nunchaku | 11 | 13 | 12 | 2.50s | 15 | 1H | Block | Feint |
| Tessen | 10 | 12 | 11 | 2.00s | 10 | 2H | Feint | Block |
| Mace | 12 | 14 | 13 | 2.75s | 45 | 1H | Concussion Blow | Disarm |
| DiscMace | 11 | 15 | 13 | 2.75s | 45 | 1H | Armor Ignore | Disarm |
| Maul | 14 | 16 | 15 | 3.50s | 45 | 1H | Crushing Blow | Concussion Blow |
| Scepter | 14 | 17 | 15.5 | 3.50s | 40 | 1H | Crushing Blow | Mortal Strike |
| HammerPick | 15 | 17 | 16 | 3.75s | 45 | 1H | Armor Ignore | Mortal Strike |
| WarMace | 16 | 17 | 16.5 | 4.00s | 80 | 1H | Crushing Blow | Mortal Strike |
| WarHammer | 17 | 18 | 17.5 | 3.75s | 95 | 2H | Whirlwind Attack | Crushing Blow |
| Tetsubo | 12 | 14 | 13 | 2.50s | 35 | 2H | Frenzied Whirlwind | Crushing Blow |

#### STAVES (Macing Skill)

| Weapon | Min | Max | Avg | Speed | STR | Hands | Primary Ability | Secondary Ability |
|--------|-----|-----|-----|-------|-----|-------|-----------------|-------------------|
| QuarterStaff | 11 | 14 | 12.5 | 2.25s | 30 | 2H | Double Strike | Concussion Blow |
| GnarledStaff | 15 | 17 | 16 | 3.25s | 20 | 2H | Concussion Blow | Force of Nature |
| BlackStaff | 13 | 16 | 14.5 | 2.75s | 35 | 2H | Whirlwind Attack | Paralyzing Blow |

#### POLEARMS (Swordsmanship Skill)

| Weapon | Min | Max | Avg | Speed | STR | Hands | Primary Ability | Secondary Ability |
|--------|-----|-----|-----|-------|-----|-------|-----------------|-------------------|
| Scythe | 15 | 18 | 16.5 | 3.50s | 45 | 2H | Bleed Attack | Paralyzing Blow |
| Bardiche | 17 | 18 | 17.5 | 3.75s | 45 | 2H | Paralyzing Blow | Dismount |
| Halberd | 18 | 19 | 18.5 | 4.25s | 95 | 2H | Whirlwind Attack | Concussion Blow |

#### FENCING - PIERCING (Fencing Skill)

| Weapon | Min | Max | Avg | Speed | STR | Hands | Primary Ability | Secondary Ability |
|--------|-----|-----|-----|-------|-----|-------|-----------------|-------------------|
| Dagger | 10 | 11 | 10.5 | 2.00s | 10 | 1H | Infectious Strike | Shadow Strike |
| ButcherKnife | 9 | 11 | 10 | 2.25s | 5 | 1H | Infectious Strike | Disarm |
| SkinningKnife | 9 | 11 | 10 | 2.25s | 5 | 1H | Shadow Strike | Disarm |
| Cleaver | 11 | 13 | 12 | 2.50s | 10 | 1H | Bleed Attack | Infectious Strike |
| AssassinSpike | 10 | 12 | 11 | 2.00s | 15 | 1H | Infectious Strike | Shadow Strike |
| Leafblade | 13 | 15 | 14 | 2.75s | 20 | 1H | Feint | Armor Ignore |
| ElvenSpellblade | 12 | 14 | 13 | 2.50s | 35 | 2H | Psychic Attack | Bleed Attack |
| Sai | 9 | 11 | 10 | 2.00s | 15 | 2H | Block | Armor Pierce |
| Kama | 9 | 11 | 10 | 2.00s | 15 | 2H | Whirlwind Attack | Defense Mastery |
| Tekagi | 10 | 12 | 11 | 2.00s | 10 | 2H | Dual Wield | Talon Strike |
| Lajatang | 16 | 18 | 17 | 3.50s | 65 | 2H | Defense Mastery | Frenzied Whirlwind |
| WarCleaver | 9 | 11 | 10 | 2.25s | 15 | 1H | Disarm | Bladeweave |

#### FENCING - SPEARS (Fencing Skill)

| Weapon | Min | Max | Avg | Speed | STR | Hands | Primary Ability | Secondary Ability |
|--------|-----|-----|-----|-------|-----|-------|-----------------|-------------------|
| ShortSpear | 10 | 13 | 11.5 | 2.00s | 40 | 1H | Shadow Strike | Mortal Strike |
| Pitchfork | 13 | 14 | 13.5 | 2.50s | 55 | 2H | Bleed Attack | Dismount |
| Spear | 13 | 15 | 14 | 2.75s | 50 | 2H | Armor Ignore | Paralyzing Blow |
| Pike | 14 | 16 | 15 | 3.00s | 50 | 2H | Paralyzing Blow | Infectious Strike |
| WarFork | 12 | 13 | 12.5 | 2.50s | 45 | 2H | Bleed Attack | Disarm |

#### RANGED - ARCHERY (Archery Skill)

| Weapon | Min | Max | Avg | Speed | STR | Hands | Range | Primary Ability | Secondary Ability |
|--------|-----|-----|-----|-------|-----|-------|-------|-----------------|-------------------|
| Bow | 15 | 19 | 17 | 4.25s | 30 | 2H | 10 | Paralyzing Blow | Mortal Strike |
| CompositeBow | 13 | 17 | 15 | 4.00s | 45 | 2H | 10 | Armor Ignore | Moving Shot |
| Crossbow | 18 | 22 | 20 | 4.50s | 35 | 2H | 8 | Concussion Blow | Mortal Strike |
| HeavyCrossbow | 19 | 24 | 21.5 | 5.00s | 80 | 2H | 8 | Moving Shot | Dismount |
| Yumi | 16 | 20 | 18 | 4.50s | 35 | 2H | 10 | Armor Pierce | Double Shot |
| ElvenCompositeLongbow | 12 | 16 | 14 | 4.00s | 45 | 2H | 10 | Force Arrow | Serpent Arrow |
| MagicalShortbow | 9 | 13 | 11 | 3.00s | 45 | 2H | 10 | Light Arrow | Psychic Attack |
| RepeatingCrossbow | 8 | 12 | 10 | 2.75s | 30 | 2H | 7 | Double Strike | Moving Shot |

### Weapon Damage Summary by Tier

| Damage Tier | Avg Damage | Example Weapons |
|-------------|------------|-----------------|
| **Very Low (9-11)** | 10 | Dagger, ButcherKnife, Sai, Kama, MagicWand |
| **Low (10-13)** | 11.5 | Kryss, Cutlass, Katana, Club, AssassinSpike |
| **Low-Mid (12-15)** | 13.5 | Scimitar, Mace, Spear, QuarterStaff |
| **Mid (14-16)** | 15 | Broadsword, Axe, Maul, BattleAxe |
| **Mid-High (15-17)** | 16 | Longsword, DoubleAxe, NoDachi, Scythe |
| **High (16-18)** | 17 | VikingSword, Lance, WarHammer, Bardiche |
| **Very High (18-20)** | 19 | OrnateAxe, Halberd, Crossbow |
| **Extreme (19-24)** | 21.5 | HeavyCrossbow |

---

## Armor Values

### Base Armor AR by Type

These are the **base AR values** before any ore/material bonuses.

| Armor Type | Base AR | Phys Resist | Fire | Cold | Poison | Energy | Str Req | Meditation |
|------------|---------|-------------|------|------|--------|--------|---------|------------|
| Leather | 13 | 2% | 4% | 3% | 3% | 3% | 15-25 | Full |
| Studded Leather | 16 | 2% | 4% | 3% | 3% | 4% | 25-35 | Half |
| Ringmail | 22 | 3% | 1% | 3% | 3% | 5% | 20-40 | None |
| Chainmail | 28 | 4% | 4% | 4% | 1% | 2% | 20-60 | None |
| Bone | 30 | 3% | 3% | 4% | 2% | 4% | 40-60 | None |
| Platemail | 40 | 5% | 3% | 2% | 3% | 2% | 70-95 | None |
| Dragon | 40 | 3% | 3% | 3% | 3% | 3% | 60-75 | None |

### Armor AR by Piece (Platemail Example)

| Piece | Base AR | % of Total |
|-------|---------|------------|
| Helm | 6-7 | ~15% |
| Gorget | 3-4 | ~7% |
| Arms | 5-6 | ~14% |
| Gloves | 3-4 | ~7% |
| Chest | 12-15 | ~35% |
| Legs | 8-10 | ~22% |
| **TOTAL** | **40** | 100% |

### Quality Bonuses

| Quality | AR Bonus |
|---------|----------|
| Normal | +0 |
| Exceptional | +8 |
| Magic (Protection +1) | +5 |
| Magic (Protection +2) | +10 |
| Magic (Protection +3) | +15 |

### Body Part Hit Chances

When attacked, a random armor piece is hit based on these weights:

| Body Part | Hit Chance | Typical AR (Plate) |
|-----------|------------|-------------------|
| Chest | 35% | 12-15 |
| Legs | 22% | 8-10 |
| Head | 15% | 6-7 |
| Arms | 14% | 5-6 |
| Neck | 7% | 3-4 |
| Hands | 7% | 3-4 |

---

## Shield Values

### All Shields with AR and Parry Data

| Shield | Base AR | Effective AR* | STR Req | Weight | Durability | Resist Bonus |
|--------|---------|---------------|---------|--------|------------|--------------|
| Buckler | 7 | 4.5 | 20 | 5.0 | 40-50 | +1 Poison |
| Wooden Shield | 8 | 5.0 | 20 | 5.0 | 20-25 | +1 Energy |
| Bronze Shield | 10 | 6.0 | 35 | 6.0 | 25-30 | +1 Cold |
| Metal Shield | 11 | 6.5 | 45 | 6.0 | 50-65 | +1 Fire |
| Wooden Kite Shield | 12 | 7.0 | 20 | 5.0 | 50-65 | +1 Energy |
| Metal Kite Shield | 16 | 9.0 | 45 | 7.0 | 45-60 | +1 Energy |
| Heater Shield | 23 | 12.5 | 90 | 8.0 | 50-65 | +1 Fire |
| Order Shield | 30 | 16.0 | 95 | 7.0 | 100-125 | +1 Physical |
| Chaos Shield | 32 | 17.0 | 95 | 4.0 | 100-125 | +1 Physical |

*Effective AR at 100 Parry skill using formula: `(ParrySkill × ArmorBase) / 200 + 1`

### Shield AR Formula

**Location:** `Projects/UOContent/Items/Shields/BaseShield.cs`

```csharp
public override int ArmorRating => (int)(ParrySkill * ArmorBase / 200.0 + 1.0);
```

| Parry Skill | Buckler (7) | Heater (23) | Chaos (32) |
|-------------|-------------|-------------|------------|
| 0 | 1 | 1 | 1 |
| 50 | 2.75 | 6.75 | 9 |
| 100 | 4.5 | 12.5 | 17 |
| 120 (GM+) | 5.2 | 14.8 | 20.2 |

### Shield Damage Blocking (On Parry)

When a parry succeeds with a shield:
- **vs Melee:** Blocks up to `ArmorBase / 2` damage
- **vs Archery:** Blocks up to `ArmorBase` damage

| Shield | vs Melee Block | vs Archery Block |
|--------|----------------|------------------|
| Buckler | 3.5 | 7 |
| Wooden Shield | 4 | 8 |
| Bronze Shield | 5 | 10 |
| Metal Shield | 5.5 | 11 |
| Wooden Kite | 6 | 12 |
| Metal Kite | 8 | 16 |
| Heater Shield | 11.5 | 23 |
| Order Shield | 15 | 30 |
| Chaos Shield | 16 | 32 |

---

## Damage Modifiers

### Offensive Damage Bonuses (AOS System)

**Location:** `Projects/UOContent/Items/Weapons/BaseWeapon.cs` (lines 2535-2603)

#### Base Damage Formula

```
Final Damage = Base Weapon Damage × (1 + All Bonuses)
```

#### Skill-Based Bonuses

| Skill | Formula | At 100 Skill | At GM (120) |
|-------|---------|--------------|-------------|
| **Strength** | `(STR / 100) × 0.3` + 0.05 if STR≥100 | +35% | +41% |
| **Anatomy** | `(Skill / 100) × 0.5` + 0.05 if ≥100 | +55% | +65% |
| **Tactics** | `(Skill / 100) × 0.625` + 0.0625 if ≥100 | +68.75% | +81.25% |
| **Lumberjacking** (axes only) | `(Skill / 100) × 0.2` + 0.1 if ≥100 | +30% | +34% |

#### Cumulative Damage Bonus Example

**Fully skilled melee character (100 in all):**
- Strength 100: +35%
- Anatomy 100: +55%
- Tactics 100: +68.75%
- **Total: +158.75%** (2.59x multiplier)

**With Lumberjacking (axes):**
- Add +30% for axes
- **Total: +188.75%** (2.89x multiplier)

#### Equipment & Spell Bonuses

| Source | Bonus | Cap |
|--------|-------|-----|
| Weapon Damage Attribute (runic) | +1% to +50% per item | +100% total |
| Divine Fury Spell | +10% | - |
| Horrific Beast Form | +25% | - |
| Slayer Property (vs creature type) | +100% | - |
| Talisman Killer | Variable | - |
| Honor Virtue | +25% | - |
| Enemy of One | +50% (PvE only) | - |

**CRITICAL: Total damage multiplier is CAPPED at 3.0x (300%)**

### Complete Damage Calculation Example

**Scenario:** Player with 100 STR, 100 Anatomy, 100 Tactics using a Halberd (18-19 damage) with +25% Weapon Damage attribute

1. Base damage: 18.5 (average)
2. Strength bonus: +35%
3. Anatomy bonus: +55%
4. Tactics bonus: +68.75%
5. Weapon attribute: +25%
6. **Total bonus: +183.75%**
7. **Final damage: 18.5 × 2.8375 = 52.5 damage**

---

## Parry System

### Parry Chance Calculation

**Location:** `Projects/UOContent/Items/Weapons/BaseWeapon.cs` (lines 1503-1586)

#### With Shield

```
Base Chance = (Parry - BushidoNonRacial) / 400
If Parry ≥ 100: Add +5% bonus
```

| Parry Skill | Base Chance | With GM Bonus |
|-------------|-------------|---------------|
| 60 | 15% | 15% |
| 80 | 20% | 20% |
| 100 | 25% | 30% |
| 120 | 30% | 35% |

#### With Weapon (No Shield)

**One-Handed Weapon:**
```
Divisor = 48000 / (Parry × Bushido)
```

**Two-Handed Weapon:**
```
Divisor = 41140 / (Parry × Bushido)
```

#### Dexterity Penalty

If DEX < 80:
```
Parry Chance = Parry Chance × (20 + DEX) / 100
```

| DEX | Multiplier |
|-----|------------|
| 80+ | 100% (no penalty) |
| 60 | 80% |
| 40 | 60% |
| 20 | 40% |

### Parry Result

**Successful Parry = 100% damage block (0 damage taken)**

This is binary - either full block or no block at all.

---

## Zero Damage Thresholds

### Critical Analysis

For damage to be reduced to 0, the AR absorption must equal or exceed incoming damage.

#### Minimum AR to SOMETIMES Block (50% chance)

AR must equal incoming damage.

| Incoming Damage | AR Needed |
|-----------------|-----------|
| 10 | 10 |
| 20 | 20 |
| 30 | 30 |
| 40 | 40 |
| 50 | 50 |
| 60 | 60 |

#### Minimum AR to ALWAYS Block (100% guarantee)

AR/2 must equal or exceed incoming damage, so AR must be 2× damage.

| Incoming Damage | AR Needed |
|-----------------|-----------|
| 10 | 20 |
| 20 | 40 |
| 30 | 60 |
| 40 | 80 |
| 50 | 100 |
| 60 | 120 |

### Real Combat Scenarios

#### Base Weapon Damage (No Skills)

| Weapon | Avg Damage | AR to Sometimes Block | AR to Always Block |
|--------|------------|----------------------|-------------------|
| Dagger | 10.5 | 11 | 21 |
| Katana | 12 | 12 | 24 |
| Broadsword | 14.5 | 15 | 29 |
| Halberd | 18.5 | 19 | 37 |
| HeavyCrossbow | 21.5 | 22 | 43 |

#### Skilled Attacker (100 STR/Anatomy/Tactics = +159% bonus)

| Weapon | Base | Skilled Damage | AR to Sometimes Block | AR to Always Block |
|--------|------|----------------|----------------------|-------------------|
| Dagger | 10.5 | 27 | 27 | 54 |
| Katana | 12 | 31 | 31 | 62 |
| Broadsword | 14.5 | 38 | 38 | 76 |
| Halberd | 18.5 | 48 | 48 | 96 |
| HeavyCrossbow | 21.5 | 56 | 56 | 112 |

#### Max Damage Build (+200% capped at 3.0x)

| Weapon | Base | Max Damage | AR to Sometimes Block | AR to Always Block |
|--------|------|------------|----------------------|-------------------|
| Dagger | 10.5 | 31.5 | 32 | 63 |
| Katana | 12 | 36 | 36 | 72 |
| Broadsword | 14.5 | 43.5 | 44 | 87 |
| Halberd | 18.5 | 55.5 | 56 | 111 |
| HeavyCrossbow | 21.5 | 64.5 | 65 | 129 |

---

## Balance Analysis

### The Problem with High AR Bonuses

#### Original Proposed T7 Armor (+36 AR)

| Armor Build | Total AR | Min Absorb | Max Absorb |
|-------------|----------|------------|------------|
| T7 Plate Only | 76 | 38 | 76 |
| T7 Plate + Exceptional | 84 | 42 | 84 |
| T7 Plate + Exceptional + Heater | 96 | 48 | 96 |

**Result:** 96 AR minimum absorbs 48 damage - this blocks ALL skilled weapon attacks.

#### What This Means for PvP

| Attack Type | Skilled Damage | vs 96 AR (Min Absorb 48) | Damage Taken |
|-------------|----------------|--------------------------|--------------|
| Dagger | 27 | 27 < 48 | **Always 0** |
| Katana | 31 | 31 < 48 | **Always 0** |
| Broadsword | 38 | 38 < 48 | **Always 0** |
| Halberd | 48 | 48 = 48 | **0 (50% chance)** |
| HeavyCrossbow | 56 | 48-96 | **0-8 damage** |

**Conclusion:** T7 armor with shield makes most weapons deal 0 damage.

### Damage Gap Analysis

The difference between T1 and T7 should be significant but not create invulnerability.

#### Current Gap (T1 +0 vs T7 +36)

| Armor | Total AR | vs 38 Damage (Broadsword skilled) |
|-------|----------|-----------------------------------|
| T1 Plate | 40 | Takes 18-38 dmg (avg 28) |
| T7 Plate | 76 | Takes 0-0 dmg (avg 0) |

**Gap: 28 damage average difference - T7 is invulnerable**

### Maximum AR Recommendations

To prevent 0-damage scenarios, AR should not exceed 2× the lowest viable weapon damage.

**Lowest skilled weapon damage:** ~27 (Dagger with full skills)
**Maximum AR to allow damage:** 53 (27 × 2 - 1)

With Exceptional (+8) and Shield (+12), base armor should be ~33 max.
**T7 Plate should be no higher than:** 40 (base) + 18 (T7 bonus) = 58 AR

---

## Proposed Balance Options

### Option A: Reduce AR Bonuses (Recommended)

| Tier | Original | Proposed | Reasoning |
|------|----------|----------|-----------|
| 1 | +0 | +0 | Baseline |
| 2 | +4 | +2 | Minor upgrade |
| 3 | +8 | +4 | Noticeable |
| 4 | +12 | +6 | Mid-tier |
| 5 | +18 | +9 | High-tier |
| 6 | +24 | +12 | Very high |
| 7 | +36 | +18 | Elite |

**Result:** T7 Plate = 58 AR, with Exceptional + Heater = 78 AR
- Min absorb: 39
- Skilled broadsword (38): Sometimes does 0, usually 1-10 damage
- Light weapons still deal reduced but meaningful damage

### Option B: Add Weapon Ore Bonuses

Match armor progression with weapon damage bonuses.

| Tier | Armor AR Bonus | Weapon Damage Bonus |
|------|----------------|---------------------|
| 1 | +0 | +0% |
| 2 | +4 | +5% |
| 3 | +8 | +10% |
| 4 | +12 | +15% |
| 5 | +18 | +22% |
| 6 | +24 | +30% |
| 7 | +36 | +45% |

**Result:** T7 weapons do +45% more damage, helping overcome T7 armor.

### Option C: Modify the AR Formula

Use diminishing returns instead of linear absorption.

**Current:** `Absorbed = Random(AR/2, AR)`
**Proposed:** `Absorbed = Random(AR/3, AR/1.5)`

This reduces minimum absorption, ensuring damage always gets through.

### Option D: Hybrid Approach

1. Reduce AR bonuses (Option A values)
2. Add small weapon damage bonuses (+0% to +15%)
3. Keep formula unchanged

---

## Code Locations

### Key Files for Combat Balance

| System | File Path | Key Lines |
|--------|-----------|-----------|
| Damage Calculation | `Projects/UOContent/Items/Weapons/BaseWeapon.cs` | 2535-2603 |
| AR Absorption | `Projects/UOContent/Items/Weapons/BaseWeapon.cs` | 1635-1703 |
| Parry System | `Projects/UOContent/Items/Weapons/BaseWeapon.cs` | 1503-1586 |
| Hit Chance | `Projects/UOContent/Items/Weapons/BaseWeapon.cs` | 1235-1366 |
| Shield AR | `Projects/UOContent/Items/Shields/BaseShield.cs` | ArmorRating property |
| Base Armor | `Projects/UOContent/Items/Armor/BaseArmor.cs` | ArmorBase values |
| Weapon Damage | Individual weapon files in `Projects/UOContent/Items/Weapons/` | MinDamage, MaxDamage |

### Weapon Ability Effects

| Ability | Effect | Location |
|---------|--------|----------|
| Armor Ignore | Bypasses AR completely | `WeaponAbility.cs` |
| Crushing Blow | +50% damage | `WeaponAbility.cs` |
| Mortal Strike | Prevents healing | `WeaponAbility.cs` |
| Paralyzing Blow | Stuns target | `WeaponAbility.cs` |
| Double Strike | Two attacks | `WeaponAbility.cs` |

---

## Special Considerations

### Armor Ignore Ability

Some weapons have **Armor Ignore** as a special ability, which completely bypasses AR calculation. This is the counter to heavy armor builds.

**Weapons with Armor Ignore:**
- Kryss (Secondary)
- Katana (Secondary)
- Longsword (Primary)
- Broadsword (Secondary)
- Hatchet (Primary)
- WarAxe (Primary)
- HammerPick (Primary)
- Spear (Primary)
- Leafblade (Secondary)
- CompositeBow (Primary)

### Critical Hits / Special Moves

Weapon abilities can multiply damage beyond the 3.0x cap in specific circumstances. These should be factored into balance but represent exceptional events, not baseline combat.

### PvE vs PvP Considerations

- **PvE:** High AR is desirable to reduce potion usage and allow soloing harder content
- **PvP:** High AR creates stalemates and makes light weapons unviable

**Option E-1 Decision:** Runic bonuses are PvE-only, meaning resistances and other runic properties don't apply in PvP. However, base AR from ore type DOES apply in PvP.

---

## Summary Tables

### Quick Reference: Weapon Damage Tiers

| Tier | Damage Range | Example Weapons |
|------|--------------|-----------------|
| 1 (Lowest) | 9-11 | Dagger, Sai, Kama |
| 2 | 11-13 | Katana, Club, Cutlass |
| 3 | 13-15 | Scimitar, Mace, Spear |
| 4 | 14-16 | Broadsword, Axe, Maul |
| 5 | 15-17 | Longsword, DoubleAxe, BattleAxe |
| 6 | 16-18 | VikingSword, NoDachi, Lance |
| 7 (Highest) | 18-24 | Halberd, OrnateAxe, HeavyCrossbow |

### Quick Reference: Armor AR Tiers

| Tier | Leather | Studded | Chain | Plate |
|------|---------|---------|-------|-------|
| T1 (Iron) | 13 | 16 | 28 | 40 |
| T2 (+2) | 15 | 18 | 30 | 42 |
| T3 (+4) | 17 | 20 | 32 | 44 |
| T4 (+6) | 19 | 22 | 34 | 46 |
| T5 (+9) | 22 | 25 | 37 | 49 |
| T6 (+12) | 25 | 28 | 40 | 52 |
| T7 (+18) | 31 | 34 | 46 | 58 |

*Using Option A reduced bonuses*

### Quick Reference: Shield AR

| Shield | Base AR | At 100 Parry |
|--------|---------|--------------|
| Buckler | 7 | 4.5 |
| Wooden | 8 | 5 |
| Bronze | 10 | 6 |
| Metal | 11 | 6.5 |
| Wooden Kite | 12 | 7 |
| Metal Kite | 16 | 9 |
| Heater | 23 | 12.5 |




**When you read this, this needs to be added into the system as well : ## Weapon Accuracy Levels in ImagineNation**

The project has a `WeaponAccuracyLevel` enum with these values that correspond to your titles:

```csharp
public enum WeaponAccuracyLevel
{
    Regular,        // No title
    Accurate,       // "Accurate"  
    Surpassingly,   // "Surpassingly accurate"
    Eminently,      // "Eminently accurate" 
    Exceedingly,    // "Exceedingly accurate"
    Supremely       // "Supremely accurate"
}
```

## How They Work

### 1. __Tactics Skill Bonus__

From `BaseWeapon.cs`, accuracy levels apply a __direct tactics skill bonus__:

```csharp
m_SkillMod = new DefaultSkillMod(AccuracySkill, true, (int)m_AccuracyLevel * 5);
```

- __Accurate__: +5 Tactics
- __Surpassingly__: +10 Tactics
- __Eminently__: +15 Tactics
- __Exceedingly__: +20 Tactics
- __Supremely__: +25 Tactics

### 2. __Hit Chance Bonus__

From `GetHitChanceBonus()`:

```csharp
switch (m_AccuracyLevel)
{
    case WeaponAccuracyLevel.Accurate: bonus += 02; break;
    case WeaponAccuracyLevel.Surpassingly: bonus += 04; break;
    case WeaponAccuracyLevel.Eminently: bonus += 06; break;
    case WeaponAccuracyLevel.Exceedingly: bonus += 08; break;
    case WeaponAccuracyLevel.Supremely: bonus += 10; break;
}
```

### 3. __Weapon Naming__

From `Sphere.cs`, weapons display these titles:

```csharp
case WeaponAccuracyLevel.Surpassingly:
    level = "Surpassingly accurate";
    break;
case WeaponAccuracyLevel.Eminently:
    level = "Eminently accurate";
    break;
case WeaponAccuracyLevel.Exceedingly:
    level = "Exceedingly accurate";
    break;
```

## How They're Applied

Weapons get these accuracy levels through:
 __Upgrade Stone__ (player can upgrade existing weapons)
 Accuracy Stone found through mining when used on an existing weapon it has a random RNG to place a +5% - +25% onto that weapon.
 Permanently. (Need to incorporate this into C:\Users\USER-002\Desktop\New Test\51a-style-ModernUo-51alpha\Documentation\SEASONAL_ORE_SYSTEM.md    and figure out a good %drop chance )
 Just like the Sharpening Stone.
 Sharpening Stone is found through mining when used on an existing weapon is has a random RNG to place Force, Power or Vanq onto that weapon Permanently. (Need to incorporate this into C:\Users\USER-002\Desktop\New Test\51a-style-ModernUo-51alpha\Documentation\SEASONAL_ORE_SYSTEM.md    and figure out a good %drop chance )

## PvP Combat Formula

The hit chance formula uses both weapon skill AND tactics:

```csharp
hitChance = (m_SphereHitPercetage) * ((atkSkill.Value + (attacker.Skills.Tactics.Value * 2)) / 300)
```

Where `m_SphereHitPercetage = 0.69` (69% base max hit chance).

## Summary

__Yes, these titles exist in ImagineNation!__ They provide:

- Direct tactics skill bonuses (+5 to +25)
- Hit chance bonuses (+2% to +10%)
- Display in weapon names ("Eminently accurate longsword")


---

## Change Log

| Date | Change | Reason |
|------|--------|--------|
| 2025-01-14 | Initial document creation | Combat balance reference needed |
| 2025-01-14 | Added all weapon damage tables | Complete weapon data for balance |
| 2025-01-14 | Added shield AR and parry data | Shield impact on combat |
| 2025-01-14 | Added zero damage threshold analysis | Identified T7 invulnerability issue |
| 2025-01-14 | Added balance options A-D | Proposed solutions for review |

---

## Next Steps

1. **Decide on balance approach** (Option A, B, C, D, or hybrid)
2. **Finalize ore AR bonuses** based on chosen approach
3. **Consider weapon ore damage bonuses** if using Option B or D
4. **Update SEASONAL_ORE_SYSTEM.md** with final values
5. **Design weapon system** with balance data in mind
