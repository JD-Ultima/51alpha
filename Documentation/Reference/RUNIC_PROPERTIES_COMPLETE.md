# Complete Runic Properties Reference

**Authority**: REFERENCE - For PvM/PvP separation review
**Source**: ModernUO BaseRunicTool.cs, AOS.cs
**Purpose**: Document ALL runic properties for Option E-1 implementation

---

## Overview

This document lists EVERY property that runic tools can apply to weapons, armor, jewelry, and other items. Use this for reviewing which properties should be:
- **PvE-only** (disabled in PvP)
- **Always active** (works in both PvE and PvP)
- **Needs discussion** (edge cases)

---

## Decision Framework

Per DESIGN_DECISIONS.md Section 16 (Option E-1):
> "Full runic system kept. All runic bonuses ONLY work in PvE (against monsters). In PvP, runic items behave as base items."

### Categories:
1. **COMBAT POWER** - Damage, hit chance, defense → PvE-only
2. **UTILITY** - Quality of life, non-combat → Always active
3. **STATS** - Raw stat bonuses → Needs decision
4. **REGEN** - HP/Mana/Stam recovery → Needs decision

---

## Weapons - Primary Attributes (AosAttribute)

Applied to `weapon.Attributes`:

| # | Property | Effect | Min | Max | Category | Recommendation |
|---|----------|--------|-----|-----|----------|----------------|
| 0 | WeaponDamage | % damage increase | 1 | 50 | COMBAT | PvE-only |
| 1 | DefendChance | % parry bonus | 1 | 15 | COMBAT | PvE-only |
| 2 | CastSpeed | Faster casting (FC) | 1 | 1 | COMBAT | PvE-only |
| 3 | AttackChance | % hit accuracy | 1 | 15 | COMBAT | PvE-only |
| 4 | Luck | Affects loot/crits | 1 | 100 | UTILITY | Always active |
| 5 | WeaponSpeed | % swing speed | 5 | 30 | COMBAT | PvE-only |
| 6 | SpellChanneling | Cast while armed | 1 | 1 | UTILITY | Always active |
| 7 | ReflectPhysical | % damage reflect | 1 | 15 | COMBAT | PvE-only |
| 8 | EnhancePotions | % potion power | 5 | 25 | UTILITY | Always active |
| 9 | LowerManaCost | % mana reduction | 1 | 8 | UTILITY | Always active |
| 10 | LowerRegCost | % reagent reduction | 1 | 20 | UTILITY | Always active |
| 11 | CastRecovery | Faster cast recovery | 1 | 3 | COMBAT | PvE-only |
| 12 | BonusStr | +Strength | 1 | 8 | STATS | **DISCUSS** |
| 13 | BonusDex | +Dexterity | 1 | 8 | STATS | **DISCUSS** |
| 14 | BonusInt | +Intelligence | 1 | 8 | STATS | **DISCUSS** |
| 15 | BonusHits | +Max HP | 1 | 5 | STATS | **DISCUSS** |
| 16 | BonusStam | +Max Stamina | 1 | 8 | STATS | **DISCUSS** |
| 17 | BonusMana | +Max Mana | 1 | 8 | STATS | **DISCUSS** |
| 18 | RegenHits | HP per tick | 1 | 2 | REGEN | **DISCUSS** |
| 19 | RegenStam | Stamina per tick | 1 | 3 | REGEN | **DISCUSS** |
| 20 | RegenMana | Mana per tick | 1 | 2 | REGEN | **DISCUSS** |
| 21 | NightSight | Permanent vision | 1 | 1 | UTILITY | Always active |
| 22 | SpellDamage | % spell damage | 1 | 12 | COMBAT | PvE-only |

---

## Weapons - Secondary Attributes (AosWeaponAttribute)

Applied to `weapon.WeaponAttributes`:

### Hit Area Effects (AOE Damage)

| Property | Effect | Min | Max | Category | Recommendation |
|----------|--------|-----|-----|----------|----------------|
| HitPhysicalArea | AOE Physical | 2 | 50 | COMBAT | PvE-only |
| HitFireArea | AOE Fire | 2 | 50 | COMBAT | PvE-only |
| HitColdArea | AOE Cold | 2 | 50 | COMBAT | PvE-only |
| HitPoisonArea | AOE Poison | 2 | 50 | COMBAT | PvE-only |
| HitEnergyArea | AOE Energy | 2 | 50 | COMBAT | PvE-only |

### Hit Spell Effects

| Property | Effect | Min | Max | Category | Recommendation |
|----------|--------|-----|-----|----------|----------------|
| HitMagicArrow | Cast on hit | 2 | 50 | COMBAT | PvE-only |
| HitHarm | Cast on hit | 2 | 50 | COMBAT | PvE-only |
| HitFireball | Cast on hit | 2 | 50 | COMBAT | PvE-only |
| HitLightning | Cast on hit | 2 | 50 | COMBAT | PvE-only |
| HitDispel | Dispel on hit | 2 | 50 | COMBAT | PvE-only |

### Hit Leech Effects

| Property | Effect | Min | Max | Category | Recommendation |
|----------|--------|-----|-----|----------|----------------|
| HitLeechHits | Lifesteal % | 2 | 50 | COMBAT | PvE-only |
| HitLeechMana | Mana drain % | 2 | 50 | COMBAT | PvE-only |
| HitLeechStam | Stamina drain % | 2 | 50 | COMBAT | PvE-only |

### Hit Debuff Effects

| Property | Effect | Min | Max | Category | Recommendation |
|----------|--------|-----|-----|----------|----------------|
| HitLowerAttack | Debuff attack % | 2 | 50 | COMBAT | PvE-only |
| HitLowerDefend | Debuff defense % | 2 | 50 | COMBAT | PvE-only |

### Weapon Utility

| Property | Effect | Min | Max | Category | Recommendation |
|----------|--------|-----|-----|----------|----------------|
| UseBestSkill | Auto-use best skill | 1 | 1 | UTILITY | Always active |
| MageWeapon | Mage weapon mode | 1 | 10 | UTILITY | Always active |
| LowerStatReq | Reduce requirements | 10 | 100 | UTILITY | Always active |
| DurabilityBonus | Increase durability | 10 | 100 | UTILITY | Always active |

### Weapon Resistances (On Equip)

| Property | Effect | Min | Max | Category | Recommendation |
|----------|--------|-----|-----|----------|----------------|
| ResistPhysicalBonus | +Physical resist | 1 | 15 | COMBAT | PvE-only |
| ResistFireBonus | +Fire resist | 1 | 15 | COMBAT | PvE-only |
| ResistColdBonus | +Cold resist | 1 | 15 | COMBAT | PvE-only |
| ResistPoisonBonus | +Poison resist | 1 | 15 | COMBAT | PvE-only |
| ResistEnergyBonus | +Energy resist | 1 | 15 | COMBAT | PvE-only |

---

## Weapons - Slayer Types

**Category**: COMBAT - PvE-only

One slayer per weapon. Full list:

| Slayer Type | Effective Against |
|-------------|-------------------|
| Silver | Undead |
| OrcSlaying | Orcs |
| TrollSlaughter | Trolls |
| OgreTrashing | Ogres |
| Repond | Humanoids |
| DragonSlaying | Dragons |
| Terathan | Terathans |
| SnakesBane | Snakes |
| LizardmanSlaughter | Lizardmen |
| ReptilianDeath | All reptiles |
| DaemonDismissal | Daemons |
| GargoylesFoe | Gargoyles |
| BalronDamnation | Balrons |
| Exorcism | Undead (stronger) |
| Ophidian | Ophidians |
| SpidersDeath | Spiders |
| ScorpionsBane | Scorpions |
| ArachnidDoom | All arachnids |
| FlameDousing | Fire elementals |
| WaterDissipation | Water elementals |
| Vacuum | Air elementals |
| ElementalHealth | Earth elementals |
| EarthShatter | Earth elementals |
| BloodDrinking | Blood creatures |
| SummerWind | Snow creatures |
| ElementalBan | All elementals |
| Fey | Fey creatures |

---

## Weapons - Elemental Damage Split

**Category**: COMBAT - PvE-only

Applied to `weapon.AosElementDamages`:

| Element | Range | Notes |
|---------|-------|-------|
| Physical | Remainder | After other elements |
| Fire | 0-100% | Randomized |
| Cold | 0-100% | Randomized |
| Poison | 0-100% | Randomized |
| Energy | 0-100% | Randomized |

Total always equals 100%.

---

## Armor - Primary Attributes

Applied to `armor.Attributes`:

| # | Property | Effect | Min | Max | Category | Recommendation |
|---|----------|--------|-----|-----|----------|----------------|
| 0 | RegenHits | HP per tick | 1 | 2 | REGEN | **DISCUSS** |
| 1 | RegenStam | Stamina per tick | 1 | 3 | REGEN | **DISCUSS** |
| 2 | RegenMana | Mana per tick | 1 | 2 | REGEN | **DISCUSS** |
| 3 | DefendChance | % parry bonus | 1 | 15 | COMBAT | PvE-only |
| 4 | ReflectPhysical | % damage reflect | 1 | 15 | COMBAT | PvE-only |
| 5 | AttackChance | % hit bonus (shields) | 1 | 15 | COMBAT | PvE-only |
| 6 | CastSpeed | Faster casting (shields) | 1 | 1 | COMBAT | PvE-only |
| 7 | NightSight | Permanent vision | 1 | 1 | UTILITY | Always active |
| 8 | BonusHits | +Max HP | 1 | 5 | STATS | **DISCUSS** |
| 9 | BonusStam | +Max Stamina | 1 | 8 | STATS | **DISCUSS** |
| 10 | BonusMana | +Max Mana | 1 | 8 | STATS | **DISCUSS** |
| 11 | LowerManaCost | % mana reduction | 1 | 8 | UTILITY | Always active |
| 12 | LowerRegCost | % reagent reduction | 1 | 20 | UTILITY | Always active |
| 13 | Luck | Affects loot/crits | 1 | 100 | UTILITY | Always active |
| 14 | BonusStr | +Strength | 1 | 8 | STATS | **DISCUSS** |
| 15 | BonusDex | +Dexterity | 1 | 8 | STATS | **DISCUSS** |
| 16 | BonusInt | +Intelligence | 1 | 8 | STATS | **DISCUSS** |
| 17 | SpellDamage | % spell damage | 1 | 12 | COMBAT | PvE-only |

---

## Armor - Secondary Attributes

Applied to `armor.ArmorAttributes`:

| Property | Effect | Min | Max | Category | Recommendation |
|----------|--------|-----|-----|----------|----------------|
| LowerStatReq | Reduce requirements | 10 | 100 | UTILITY | Always active |
| SelfRepair | Auto-repair durability | 1 | 5 | UTILITY | Always active |
| DurabilityBonus | Increase max durability | 10 | 100 | UTILITY | Always active |
| MageArmor | Allow casting | 1 | 1 | UTILITY | Always active |

---

## Armor - Elemental Resistances

**Category**: COMBAT - PvE-only

| Resistance | Min | Max | Notes |
|------------|-----|-----|-------|
| Physical | 1 | 15 | Defense vs physical |
| Fire | 1 | 15 | Defense vs fire |
| Cold | 1 | 15 | Defense vs cold |
| Poison | 1 | 15 | Defense vs poison |
| Energy | 1 | 15 | Defense vs energy |

---

## Jewelry - Properties

Applied to jewelry items (rings, bracelets, earrings, necklaces):

### Resistances (5 types)

| Property | Min | Max | Category | Recommendation |
|----------|-----|-----|----------|----------------|
| Physical Resist | 1 | 15 | COMBAT | PvE-only |
| Fire Resist | 1 | 15 | COMBAT | PvE-only |
| Cold Resist | 1 | 15 | COMBAT | PvE-only |
| Poison Resist | 1 | 15 | COMBAT | PvE-only |
| Energy Resist | 1 | 15 | COMBAT | PvE-only |

### Combat Properties

| Property | Min | Max | Category | Recommendation |
|----------|-----|-----|----------|----------------|
| WeaponDamage | 1 | 25 | COMBAT | PvE-only |
| DefendChance | 1 | 15 | COMBAT | PvE-only |
| AttackChance | 1 | 15 | COMBAT | PvE-only |
| CastSpeed | 1 | 1 | COMBAT | PvE-only |
| CastRecovery | 1 | 3 | COMBAT | PvE-only |
| SpellDamage | 1 | 12 | COMBAT | PvE-only |

### Stat Bonuses

| Property | Min | Max | Category | Recommendation |
|----------|-----|-----|----------|----------------|
| BonusStr | 1 | 8 | STATS | **DISCUSS** |
| BonusDex | 1 | 8 | STATS | **DISCUSS** |
| BonusInt | 1 | 8 | STATS | **DISCUSS** |

### Utility Properties

| Property | Min | Max | Category | Recommendation |
|----------|-----|-----|----------|----------------|
| EnhancePotions | 5 | 25 | UTILITY | Always active |
| LowerManaCost | 1 | 8 | UTILITY | Always active |
| LowerRegCost | 1 | 20 | UTILITY | Always active |
| Luck | 1 | 100 | UTILITY | Always active |
| NightSight | 1 | 1 | UTILITY | Always active |

### Skill Bonuses

| Property | Min | Max | Category | Recommendation |
|----------|-----|-----|----------|----------------|
| Skill Bonus (x5) | 1 | 15 | COMBAT | PvE-only |

---

## Spellbooks - Properties

| Property | Min | Max | Category | Recommendation |
|----------|-----|-----|----------|----------------|
| BonusInt (x4) | 1 | 8 | STATS | **DISCUSS** |
| BonusMana | 1 | 8 | STATS | **DISCUSS** |
| CastSpeed | 1 | 1 | COMBAT | PvE-only |
| CastRecovery | 1 | 3 | COMBAT | PvE-only |
| SpellDamage | 1 | 12 | COMBAT | PvE-only |
| LowerRegCost | 1 | 20 | UTILITY | Always active |
| LowerManaCost | 1 | 8 | UTILITY | Always active |
| RegenMana | 1 | 2 | REGEN | **DISCUSS** |
| Skill Bonus (x4) | 1 | 15 | COMBAT | PvE-only |
| Slayer | Variable | Variable | COMBAT | PvE-only |

Skill bonuses limited to: Magery, Meditation, Eval Int, Magic Resist

---

## Hats - Properties

| Property | Min | Max | Category | Recommendation |
|----------|-----|-----|----------|----------------|
| All 5 Resistances | 1 | 15 | COMBAT | PvE-only |
| RegenHits | 1 | 2 | REGEN | **DISCUSS** |
| RegenStam | 1 | 3 | REGEN | **DISCUSS** |
| RegenMana | 1 | 2 | REGEN | **DISCUSS** |
| NightSight | 1 | 1 | UTILITY | Always active |
| BonusHits | 1 | 5 | STATS | **DISCUSS** |
| BonusStam | 1 | 8 | STATS | **DISCUSS** |
| BonusMana | 1 | 8 | STATS | **DISCUSS** |
| LowerManaCost | 1 | 8 | UTILITY | Always active |
| LowerRegCost | 1 | 20 | UTILITY | Always active |
| Luck | 1 | 100 | UTILITY | Always active |
| LowerStatReq | 10 | 100 | UTILITY | Always active |
| SelfRepair | 1 | 5 | UTILITY | Always active |
| DurabilityBonus | 10 | 100 | UTILITY | Always active |
| ReflectPhysical | 1 | 15 | COMBAT | PvE-only |

---

## Runic Intensity by Material

### Metal Resources (Ascending Power)

| Material | Attributes | Intensity (ML) | Intensity (Pre-ML) |
|----------|------------|----------------|-------------------|
| Iron | 0 | N/A | N/A |
| Dull Copper | 1-2 | 40-100% | 10-35% |
| Shadow Iron | 2 | 45-100% | 20-45% |
| Copper | 2-3 | 50-100% | 25-50% |
| Bronze | 3 | 55-100% | 30-65% |
| Gold | 3-4 | 60-100% | 35-75% |
| Agapite | 4 | 65-100% | 40-80% |
| Verite | 4-5 | 70-100% | 45-90% |
| Valorite | 5 | 85-100% | 50-100% |


### Leather Resources

| Material | Attributes | Intensity (ML) | Intensity (Pre-ML) |
|----------|------------|----------------|-------------------|
| Regular | 0 | N/A | N/A |
| Spined | 1-3 | 40-100% | 20-40% |
| Horned | 3-4 | 45-100% | 30-70% |
| Barbed | 4-5 | 50-100% | 40-100% |

We are going to use these three leather resources not for making armor.
These will be to craft 3 different tierd bags.
Each Tier will aloww more of specific items to be carried in them.
These will be weight reduction bags.
Spined Reagent bag (reduces reagent weight)
Spined Magic bag  (reduces scrolls and potion weight)
Spined Mining bag (reduces ore weight)
Spined Logging bag (reduces log weight)
Spined leather bag (reduced leather weight)
Horned will reduce weight even more
Barbed will reduce weight even more

Spined tailoring skill required 70
Horned tailoring skill required 80
Barbed tailoring skill required 100
---

## Summary: Properties Needing Discussion

### Stats (BonusStr, BonusDex, BonusInt, BonusHits, BonusStam, BonusMana)

**Arguments for PvE-only:**
- Directly affects combat effectiveness
- Str = damage, Dex = swing speed/defense, Int = mana pool
- Could create gear imbalance in PvP

**Arguments for Always Active:**
- Stats are "fundamental" not "special effects"
- Players expect equipped stats to work
- Easier to understand/explain

**Recommendation:** Need user decision

### Regen (RegenHits, RegenStam, RegenMana)

**Arguments for PvE-only:**
- Combat sustain advantage
- Could extend fights unfairly

**Arguments for Always Active:**
- Passive recovery, not burst
- Most regen values are small (1-3)
- Natural regen exists anyway

**Recommendation:** Need user decision
Decision actioned below @ line 435 +

---

## Implementation Notes

### Code Locations

The key files to modify for PvE-only checks:

**Weapons:**
- `Projects/UOContent/Items/Weapons/BaseWeapon.cs`
  - `ComputeDamage()` - line 2695
  - `OnHit()` - lines 1745-2019
  - `CheckHit()` - lines 1235-1366
  - Hit effects processing

**Armor:**
- `Projects/UOContent/Items/Armor/BaseArmor.cs`
  - Resistance calculations - lines 509-522
  - `AbsorbDamage()` - lines 1588-1704

**Damage Application:**
- `Projects/UOContent/Misc/AOS.cs`
  - Lines 28-241 for resistance application

### Check Pattern

```csharp
// Example pattern for disabling property in PvP
if (target is PlayerMobile)
{
    // Skip runic property - use base behavior only
    return baseValue;
}
else
{
    // Apply runic property against monsters
    return baseValue + runicBonus;
}
```

### Existing Hook

`Projects/UOContent/Sphere51a/Core/S51aPvPContext.cs` already has PvP detection:

```csharp
public static bool IsInPvPCombat(Mobile mobile)
{
    // Returns true if mobile is fighting another player
}
```

---

## Quick Reference Card

### Definitely PvE-Only

| Category | Properties |
|----------|------------|
| Damage | WeaponDamage, SpellDamage, Slayers, Elemental |
| Hit Effects | All HitXxx properties |
| Defense | DefendChance, AttackChance, ReflectPhysical |
| Resistances | All 5 resistance types |
| Speed | WeaponSpeed, CastSpeed, CastRecovery |
| Skill | All skill bonuses |
| Casting | SpellChanneling, MageArmor |
| Stats | BonusStr, BonusDex, BonusInt |
| HP/Pools | BonusHits, BonusStam, BonusMana |
| Regen | RegenHits, RegenStam, RegenMana |
| Resource | LowerManaCost, LowerRegCost, EnhancePotions |

### Definitely Always Active

| Category | Properties |
|----------|------------|
| Quality of Life | NightSight, Luck |
| Durability | SelfRepair, DurabilityBonus, LowerStatReq |
| Flexibility | UseBestSkill


## Remove from items
MageWeapon 

---

*This document lists all properties for review. Final PvE/PvP decisions should be recorded in Core/DESIGN_DECISIONS.md once made.*
