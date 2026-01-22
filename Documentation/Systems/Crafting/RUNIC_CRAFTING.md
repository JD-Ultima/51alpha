# Runic Crafting System

**Authority**: TIER 2 - Implementation Spec
**Authoritative Source**: `Core/DESIGN_DECISIONS.md` §16
**Related**: `ORE_MATERIALS.md`, `Combat/PVP_COMBAT.md`

---

## Overview

Runic tools create items with magical properties. All runic bonuses are **PvE-only** per Option E-1 decision - properties are disabled against players.

---

## Core Rule: PvE Only

**CRITICAL**: All runic properties are disabled in PvP combat.

```csharp
// Runic bonuses check
if (defender is PlayerMobile)
{
    // Skip all runic property calculations
    return baseDamage;
}
```

This ensures:
- PvP balance based on skill, not gear grinding
- PvM remains engaging with powerful itemization
- No PvM→PvP power transfer

---

## Runic Tool Properties

### By Tier

| Tier | Uses | Properties | Intensity |
|------|------|------------|-----------|
| 2 | 25-50 | 1-2 | 40-100% |
| 3 | 25-50 | 2 | 45-100% |
| 4 | 30-55 | 2-3 | 50-100% |
| 5 | 30-55 | 3-4 | 60-100% |
| 6 | 35-60 | 4 | 70-100% |
| 7 | 40-75 | 5 | 85-100% |

### Property Types (PvE Only)

**Resistance Bonuses:**
- Physical Resist: +1-15%
- Fire Resist: +1-15%
- Cold Resist: +1-15%
- Poison Resist: +1-15%
- Energy Resist: +1-15%

**Regeneration Bonuses:**
- HP Regen: +1-5
- Mana Regen: +1-5
- Stamina Regen: +1-5

**Utility Bonuses:**
- Luck: +1-100
- Lower Requirements: 10-100%
- Self Repair: 1-5

---

## Obtaining Runic Tools

### Primary Source: Bulk Order Deeds

| BOD Quality | Runic Tier Possible |
|-------------|---------------------|
| Small Normal | T2 only |
| Small Exceptional | T2-T3 |
| Large Normal | T3-T4 |
| Large Exceptional | T4-T6 |
| Large Exceptional (colored) | T5-T7 |

### Secondary Sources

- **Rare mining drops**: Small chance with T5+ ore
- **Boss loot**: High-end dungeon bosses

---

## Exceptional Quality

**IMPORTANT**: Exceptional quality is **cosmetic only**.

- No AR bonus
- No damage bonus
- Visual prestige only
- Displays "exceptional" in item name

This prevents:
- GM smiths having combat advantage
- Gear grind requirements for PvP
- Balance issues from quality stacking

---

## Crafting Process

### Requirements

1. **Runic Tool**: Correct tier for desired properties
2. **Materials**: Appropriate ore/leather tier
3. **Skill**: GM Blacksmithing/Tailoring

### Result

Item receives:
- Base AR from material tier
- Random runic properties (based on tool)
- Properties disabled vs players

---

## Property Intensity

### Calculation

```
Final Intensity = Base Range × Tool Intensity Modifier
```

### Example (T5 Runic Tool)

- Intensity range: 60-100%
- Property: Fire Resist (max 15%)
- Result: 9-15% Fire Resist

---

## Blacksmithing vs Tailoring

### Blacksmithing Runics

- Metal armor (plate, chain, ring)
- Metal weapons
- Higher AR items possible

### Tailoring Runics

- Leather armor
- Studded leather
- Creature-derived armor
- Meditation-compatible

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_runic_tools | Tool tracking |
| s51a_runic_properties | Property definitions |
| s51a_item_properties | Applied properties |

---

## Configuration

```json
{
  "runic.pve_only": true,
  "runic.max_properties": 5,
  "runic.max_intensity": 100,
  "runic.exceptional_cosmetic_only": true
}
```

---

## FAQ

**Q: Why are runic bonuses PvE-only?**
A: To keep PvP skill-based. Players shouldn't need to grind PvM for competitive PvP gear.

**Q: Does exceptional quality give any bonus?**
A: No. Exceptional is cosmetic only - no AR or damage bonuses.

**Q: Can I use runic items in PvP?**
A: Yes, but the runic properties won't apply against players. Base AR/damage still works.

**Q: Do resistances work in PvP?**
A: Runic resist bonuses don't. Natural armor resistances may still apply (see PVP_COMBAT.md).
