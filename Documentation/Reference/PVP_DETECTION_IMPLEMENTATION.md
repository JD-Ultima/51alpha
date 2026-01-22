# PvP Detection Implementation Specification

**Authority**: REFERENCE - Technical Implementation Guide
**Purpose**: Define how to detect PvP context and disable runic/talisman bonuses
**Related**: `RUNIC_PROPERTIES_COMPLETE.md`, `Progression/TALISMAN_SYSTEM.md`

---

## Overview

This document specifies how to implement immediate PvP detection for disabling runic bonuses and talisman effects. The solution uses ModernUO's built-in aggressor flagging system which marks BOTH attacker and defender instantly.

---

## Design Requirements

1. **Immediate** - Bonuses disabled the moment PvP starts (no delay)
2. **Both Players** - Attacker AND defender both lose runic bonuses
3. **2-Minute Window** - Bonuses stay disabled for 2 minutes after last PvP action
4. **Target-Specific** - Damage bonuses only disabled vs players, not vs NPCs in same fight
5. **Performance** - Negligible server impact even with 1000+ players

---

## How ModernUO's PvP Flagging Works

### The Instant Flag Chain

When Player A attacks Player B:

```
T+0: Damage occurs
     ↓
     Mobile.Damage(amount, from)  [Mobile.cs:5930]
     ↓
     from.DoHarmful(this)  [Mobile.cs:8335]
     ↓
     target.AggressiveAction(this, isCriminal)  [Mobile.cs:3804]
     ↓
     BOTH PLAYERS FLAGGED INSTANTLY:
     ├─ A.Combatant = B
     ├─ B.Combatant = A
     ├─ A.Aggressed.Add(B)
     └─ B.Aggressors.Add(A)
```

### Flag Duration

| Flag | Duration | Purpose |
|------|----------|---------|
| `Combatant` | 1 minute | Active combat target |
| `Aggressors` list | 2 minutes | Who attacked me |
| `Aggressed` list | 2 minutes | Who I attacked |

### Key Properties

```csharp
// Mobile.cs properties
public Mobile Combatant { get; set; }           // Current combat target
public List<AggressorInfo> Aggressors { get; }  // Who attacked this mobile
public List<AggressorInfo> Aggressed { get; }   // Who this mobile attacked

// AggressorInfo.cs
public class AggressorInfo
{
    public Mobile Attacker { get; }
    public Mobile Defender { get; }
    public bool CriminalAggression { get; }
    public DateTime LastCombatTime { get; }
    public bool Expired => Core.Now > LastCombatTime + TimeSpan.FromMinutes(2.0);
}
```

---

## Implementation: Core Helper Method

Create a centralized PvP detection helper in `Sphere51a/Core/`:

### File: `S51aPvPCombatCheck.cs`

```csharp
using System;
using Server;
using Server.Mobiles;

namespace Server.Sphere51a.Core
{
    public static class S51aPvPCombatCheck
    {
        /// <summary>
        /// Check if mobile is currently in PvP combat (attacked by or attacking a player).
        /// Used for disabling runic regen bonuses and talisman effects.
        /// </summary>
        public static bool IsInPlayerCombat(Mobile m)
        {
            if (m == null || !m.Player)
                return false;

            // Check 1: Currently fighting a player
            if (m.Combatant is PlayerMobile)
                return true;

            // Check 2: A player attacked me recently (I'm the defender)
            var aggressors = m.Aggressors;
            for (int i = 0; i < aggressors.Count; i++)
            {
                var info = aggressors[i];
                if (!info.Expired && info.Attacker is PlayerMobile)
                    return true;
            }

            // Check 3: I attacked a player recently (I'm the attacker)
            var aggressed = m.Aggressed;
            for (int i = 0; i < aggressed.Count; i++)
            {
                var info = aggressed[i];
                if (!info.Expired && info.Defender is PlayerMobile)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if this specific target is a player (for damage/hit effect checks).
        /// Simpler check when we have the target reference.
        /// </summary>
        public static bool IsPlayerTarget(Mobile target)
        {
            return target is PlayerMobile;
        }

        /// <summary>
        /// Check if damage is coming from a player (for resistance checks).
        /// </summary>
        public static bool IsPlayerAttacker(Mobile attacker)
        {
            return attacker is PlayerMobile;
        }
    }
}
```

---

## Implementation: Runic Bonus Disabling

### 1. Damage Bonuses (Target-Specific)

**File**: `Projects/UOContent/Items/Weapons/BaseWeapon.cs`
**Location**: `ScaleDamageAOS()` method (~line 2567)

```csharp
// BEFORE (original):
var damageBonus = AosAttributes.GetValue(attacker, AosAttribute.WeaponDamage);

// AFTER (with PvP check):
var damageBonus = AosAttributes.GetValue(attacker, AosAttribute.WeaponDamage);

// Disable runic damage bonus vs players (Option E-1)
if (S51aPvPCombatCheck.IsPlayerTarget(defender))
{
    damageBonus = 0;
}
```

### 2. Hit Effects (Target-Specific)

**File**: `Projects/UOContent/Items/Weapons/BaseWeapon.cs`
**Location**: `OnHit()` method - hit effect processing

Wrap each hit effect in a player check:

```csharp
// Example for HitLeechHits
if (!S51aPvPCombatCheck.IsPlayerTarget(defender))
{
    // Apply hit leech effect - only vs monsters
    int lifeLeech = weapon.WeaponAttributes.HitLeechHits;
    if (lifeLeech > 0)
    {
        // ... leech logic
    }
}
```

**Hit Effects to Disable in PvP:**
- HitLeechHits (lifesteal)
- HitLeechMana (mana drain)
- HitLeechStam (stamina drain)
- HitLowerAttack (debuff attack)
- HitLowerDefend (debuff defense)
- HitMagicArrow, HitHarm, HitFireball, HitLightning (spell procs)
- HitDispel
- HitPhysicalArea, HitFireArea, HitColdArea, HitPoisonArea, HitEnergyArea (AOE)

### 3. Regeneration Bonuses (Context-Based)

**File**: `Projects/UOContent/Misc/RegenRates.cs`
**Location**: Each regen rate handler

```csharp
// In Mobile_HitsRegenRate():
private static TimeSpan Mobile_HitsRegenRate(Mobile from)
{
    int points = 0;

    // Only add runic regen if NOT in player combat
    if (!S51aPvPCombatCheck.IsInPlayerCombat(from))
    {
        points = AosAttributes.GetValue(from, AosAttribute.RegenHits);
    }

    // ... rest of calculation (non-runic sources still apply)
}

// Same pattern for Mobile_ManaRegenRate() and Mobile_StamRegenRate()
```

### 4. Stat Bonuses (Context-Based)

**File**: `Projects/UOContent/Misc/AOS.cs`
**Location**: `GetValue()` method or stat calculation

For BonusStr, BonusDex, BonusInt from runic items:

```csharp
// Option A: In GetValue() - affects all stat lookups
public static int GetValue(Mobile m, AosAttribute attribute)
{
    // For stat bonuses, check PvP context
    if (IsStatBonus(attribute) && S51aPvPCombatCheck.IsInPlayerCombat(m))
    {
        return 0; // Disable runic stat bonuses in PvP
    }

    // ... normal calculation
}

private static bool IsStatBonus(AosAttribute attr)
{
    return attr == AosAttribute.BonusStr ||
           attr == AosAttribute.BonusDex ||
           attr == AosAttribute.BonusInt ||
           attr == AosAttribute.BonusHits ||
           attr == AosAttribute.BonusStam ||
           attr == AosAttribute.BonusMana;
}
```

### 5. Resistance Bonuses (Attacker-Based)

**File**: `Projects/UOContent/Misc/AOS.cs`
**Location**: `Damage()` method (~line 120)

```csharp
// When calculating resistance reduction:
if (S51aPvPCombatCheck.IsPlayerAttacker(from))
{
    // Don't apply runic resistance bonuses vs player damage
    // Use base armor resistances only
}
```

---

## Implementation: Talisman Disabling

### Talisman Unequip on PvP

**File**: `Projects/UOContent/Sphere51a/Items/BaseTalisman51a.cs` (new or modified)

Hook into the aggressor system to unequip talisman:

```csharp
// Override OnAggressiveAction in PlayerMobile or hook via event
public override void AggressiveAction(Mobile aggressor, bool criminal)
{
    base.AggressiveAction(aggressor, criminal);

    // If aggressor is a player, unequip our talisman
    if (aggressor is PlayerMobile)
    {
        UnequipTalisman();
    }
}

// Also hook when WE attack a player
public override void DoHarmful(Mobile target, bool indirect = false)
{
    base.DoHarmful(target, indirect);

    if (target is PlayerMobile)
    {
        UnequipTalisman();
    }
}

private void UnequipTalisman()
{
    var talisman = FindItemOnLayer(Layer.Talisman) as BaseTalisman51a;
    if (talisman != null)
    {
        // Move to backpack
        AddToBackpack(talisman);
        SendLocalizedMessage(1234567); // "Your talisman has been unequipped due to PvP combat."

        // Start 5-minute cooldown
        talisman.PvPCooldownEnd = DateTime.UtcNow + TimeSpan.FromMinutes(5);
    }
}
```

---

## Properties Summary: What Gets Disabled

### Disabled in PvP (Runic Properties)

| Category | Properties | Check Method |
|----------|------------|--------------|
| **Damage** | WeaponDamage, SpellDamage | `IsPlayerTarget(defender)` |
| **Hit Effects** | All HitLeech*, HitLower*, HitSpell*, HitArea* | `IsPlayerTarget(defender)` |
| **Speed** | WeaponSpeed, CastSpeed, CastRecovery | `IsPlayerTarget(defender)` |
| **Regen** | RegenHits, RegenMana, RegenStam | `IsInPlayerCombat(mobile)` |
| **Stats** | BonusStr, BonusDex, BonusInt, BonusHits, BonusStam, BonusMana | `IsInPlayerCombat(mobile)` |
| **Resistances** | All 5 resistance bonuses | `IsPlayerAttacker(from)` |
| **Defense** | DefendChance, AttackChance, ReflectPhysical | `IsPlayerTarget(defender)` |
| **Slayers** | All slayer types | `IsPlayerTarget(defender)` |
| **Casting Utility** | SpellChanneling, MageArmor | QoL, not damage |
| **Resource** | LowerManaCost, LowerRegCost, EnhancePotions | Efficiency, not power |
| **Durability** | SelfRepair, DurabilityBonus, LowerStatReq | Item maintenance |

### Always Active (Not Runic Combat Bonuses)

| Category | Properties | Reasoning |
|----------|------------|-----------|
| **Quality of Life** | NightSight, Luck | Not combat power |
| **Flexibility** | UseBestSkill, MageWeapon | Convenience |

### Siege Jewelry (Always Active in PvP)

| Piece | Stats | Notes |
|-------|-------|-------|
| Ring | +2 Str/Dex/Int | Designed for PvP |
| Earrings | +2 Str/Dex/Int | Designed for PvP |
| Bracelet | +3 Str/Dex/Int | Designed for PvP |
| Glasses | +3 Str/Dex/Int | Designed for PvP |

Siege jewelry is NOT runic - it's specifically designed for PvP and always works.

---

## Performance Analysis

### Cost Per Check

| Operation | Cost | Frequency |
|-----------|------|-----------|
| `IsPlayerTarget(defender)` | ~10ns | Per attack hit |
| `IsInPlayerCombat(mobile)` | ~50-200ns | Per regen tick (7-11 sec) |
| `IsPlayerAttacker(from)` | ~10ns | Per damage received |

### With 1000 Concurrent Players

| Scenario | Checks/Second | Total CPU | Impact |
|----------|---------------|-----------|--------|
| Regen ticks | ~100-150 | ~20μs | Negligible |
| Combat hits | ~500 (estimate) | ~5μs | Negligible |
| **Total** | ~650 | **~25μs/sec** | **<0.001%** |

---

## File Locations Summary

### Files to Modify

| File | Changes |
|------|---------|
| `Projects/UOContent/Items/Weapons/BaseWeapon.cs` | Damage bonus, hit effects |
| `Projects/UOContent/Misc/RegenRates.cs` | Regen bonus checks |
| `Projects/UOContent/Misc/AOS.cs` | Stat bonuses, resistance checks |
| `Projects/UOContent/Mobiles/PlayerMobile.cs` | Talisman unequip hooks |

### Files to Create

| File | Purpose |
|------|---------|
| `Projects/UOContent/Sphere51a/Core/S51aPvPCombatCheck.cs` | Centralized PvP detection |

### Existing Files (Reference)

| File | Contains |
|------|----------|
| `Projects/Server/Mobiles/Mobile.cs` | Combatant, Aggressors, Aggressed |
| `Projects/Server/Mobiles/AggressorInfo.cs` | AggressorInfo class |

---

## Timeline Visualization

```
T+0.0s:  Player A attacks Player B
         ├─ DoHarmful() triggers
         ├─ AggressiveAction() called
         ├─ A.Combatant = B (instant)
         ├─ B.Combatant = A (instant)
         ├─ B.Aggressors.Add(A) (instant)
         ├─ A.Aggressed.Add(B) (instant)
         ├─ A's talisman unequipped (instant)
         ├─ B's talisman unequipped (instant)
         └─ Runic bonuses disabled for BOTH (instant)

T+0.1s:  A's damage calculated
         └─ damageBonus = 0 (vs player)

T+7.0s:  B's HP regen tick
         └─ RegenHits bonus = 0 (in player combat)

T+60s:   Combatant expires (if no further combat)
         └─ Still in PvP via Aggressors/Aggressed

T+120s:  Aggressors/Aggressed expire
         ├─ Runic bonuses re-enabled
         └─ Talisman can be re-equipped (if 5-min cooldown passed)
```

---

## Edge Cases

### Mixed Combat (Player + NPC)

**Scenario**: Player A fights NPC while Player B is nearby, then B attacks A.

**Behavior**:
- A's attacks vs NPC: Runic bonuses ACTIVE (target is NPC)
- B attacks A: Both flagged, A's runic bonuses vs B: DISABLED
- A's attacks vs NPC: Still ACTIVE (different target)
- A's regen: DISABLED (in player combat context)

### Pet Combat

**Scenario**: Player A's pet attacks Player B.

**Behavior**:
- Pet damage to B: Check if pet's owner is a player
- May need additional hook in pet combat code

### AOE Effects

**Scenario**: Player A uses AOE attack hitting both NPC and Player B.

**Behavior**:
- Damage to NPC: Runic bonuses ACTIVE
- Damage to B: Runic bonuses DISABLED
- Each target checked individually

---

## Testing Commands

```csharp
// Admin test commands for Sphere51a/Commands/

[Usage("TestPvPFlag")]
[Description("Check PvP flag status")]
public static void TestPvPFlag_OnCommand(CommandEventArgs e)
{
    var pm = e.Mobile as PlayerMobile;
    bool inPvP = S51aPvPCombatCheck.IsInPlayerCombat(pm);

    pm.SendMessage($"In Player Combat: {inPvP}");
    pm.SendMessage($"Combatant: {pm.Combatant?.Name ?? "None"}");
    pm.SendMessage($"Aggressors: {pm.Aggressors.Count}");
    pm.SendMessage($"Aggressed: {pm.Aggressed.Count}");
}
```

---

## Configuration

```json
{
  "pvp.runic_disabled": true,
  "pvp.context_duration_minutes": 2,
  "pvp.talisman_cooldown_minutes": 5,
  "pvp.siege_jewelry_always_active": true
}
```

---

*This document provides the technical specification for implementing PvE-only runic bonuses. Update DESIGN_DECISIONS.md Section 16 after implementation is complete.*
