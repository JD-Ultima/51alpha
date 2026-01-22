# Seasonal Ore & Crafting System

**Status**: DESIGN COMPLETE - Ready for Implementation
**Last Updated**: 2025-01-15
**Purpose**: Define the 7-tier ore system with seasonal rotation
**Authority**: TIER 2 - Implementation details for DESIGN_DECISIONS.md §16, §18

> **Authoritative source**: `DESIGN_DECISIONS.md` Sections 16 (Crafting) and 18 (Seasonal Ore)
> **Related docs**: `PVP_COMBAT_SYSTEM.md` (combat formulas, mining stones)
> **Index**: See `Documentation/INDEX.md` for topic lookup

---

## Overview

The ore system uses **7 tiers** (reduced from 9) to create meaningful gaps in armor rating. Each season (3 months), the ore names and hues change while keeping the same mechanical properties. Iron is always available regardless of season.

**Key Features:**
- 7 ore tiers with increasing AR bonuses
- Seasonal name/hue rotation (Spring, Summer, Fall, Winter)
- Small gaps in low-mid tiers, larger gaps in high tiers
- AR matters in PvP (significant but not overwhelming advantage)
- UO-centric ore names (traditional + some custom)

---

## The AR Formula (How Damage Reduction Works)

**IMPORTANT:** This shard uses SEPARATE formulas for PvP and PvM. See `PVP_COMBAT_SYSTEM.md` for full PvP details.

### PvP Formula (Player vs Player)

```
Damage Absorbed = Random(AR / 4.3, AR / 2.4)
Final Damage = Original Damage - Absorbed
```

**Weaker absorption** - prevents invulnerability, ensures combat is decisive.

### PvM Formula (Default - Unchanged)

```
Damage Absorbed = Random(AR / 2, AR)
Final Damage = Original Damage - Absorbed
```

**Stronger absorption** - keeps existing PvM balance intact.

### Comparison (60 AR Example)

| Context | Min Absorb | Max Absorb | Avg Absorb |
|---------|------------|------------|------------|
| PvP | 14 | 25 | 19 |
| PvM | 30 | 60 | 45 |

**Same armor, different effectiveness based on context.**

---

## Base Armor Values (PvP System)

**NOTE:** Plate armor uses custom base AR values for PvP balance. See tier table below.

| Armor Type      | PvP Base AR | Str Req | Meditation | Notes |
|-----------------|-------------|---------|------------|-------|
| Leather         | 15          | 15-25   | Full       | Creature-derived system |
| Studded Leather | 15          | 25-35   | Half       | Creature-derived system |
| Bone            | 25 (T1)     | 40-60   | None       | Same as plate base |
| Chainmail Tunic | 25 (T1)     | 60      | None       | **Chest only** - identical to plate |
| Ringmail Tunic  | 25 (T1)     | 40      | None       | **Chest only** - identical to plate |
| Platemail       | 25 (T1)     | 70-95   | None       | Uses 7-tier ore system |
| Dragon Scale    | 25 (T1)     | 60-75   | None       | Same as plate base (for now) |

### Chainmail & Ringmail Rules

**REMOVED:** All chainmail and ringmail pieces EXCEPT chest tunics.
- Chainmail Coif, Leggings, Gloves - REMOVED
- Ringmail Sleeves, Leggings, Gloves - REMOVED

**KEPT:** Chainmail Tunic and Ringmail Tunic only.
- Both use plate AR tier system (T1-T7)
- Lower STR requirement than plate chest
- Provides armor variety without balance complexity
- Can be crafted with colored ore for same AR bonuses as plate

### Leather/Special Armor

Leather, Studded, Bone, and Dragon Scale armors use the **Creature-Derived Armor system** (Lich Bone, Dragon Scale, Titan Skin, Ophidian Silk) - see end of this document.

---

## 7-Tier Ore System (Plate Armor)

### Tier Structure & AR Values (FINAL)

Custom AR values designed for PvP balance using ImaginNation-style formula.

| Tier | Mining | Plate AR | Gap  | Runic Props | Intensity  |
|------|--------|----------|------|-------------|------------|
| 1    | 0      | 25       | -    | 0           | -          |
| 2    | 55     | 30       | +5   | 1-2         | 40-100%    |
| 3    | 65     | 35       | +5   | 2           | 45-100%    |
| 4    | 75     | 45       | +10  | 2-3         | 50-100%    |
| 5    | 85     | 55       | +10  | 3-4         | 60-100%    |
| 6    | 95     | 60       | +5   | 4           | 70-100%    |
| 7    | 99     | 65       | +5   | 5           | 85-100%    |

**Gap Analysis:**
- T1→T2→T3: Small increments (+5, +5) = 10 total AR (entry progression)
- T3→T4→T5: Large increments (+10, +10) = 20 total AR (big jump to mid-game)
- T5→T6→T7: Small increments (+5, +5) = 10 total AR (elite refinement)

**Tier Groupings:**
- **Low (T1-T2):** Entry level, 25-30 AR
- **Mid (T3-T4):** Big jump at T4, 35-45 AR
- **High (T5-T6):** End game, 55-60 AR
- **Elite (T7):** Best in slot, 65 AR

### Max AR Builds (Plate + Shield + Protection)

| Tier | Plate | +Heater (GM) | +Protection | **MAX AR** |
|------|-------|--------------|-------------|------------|
| T1   | 25    | 40           | 45          | **45**     |
| T2   | 30    | 45           | 50          | **50**     |
| T3   | 35    | 50           | 55          | **55**     |
| T4   | 45    | 60           | 65          | **65**     |
| T5   | 55    | 70           | 75          | **75**     |
| T6   | 60    | 75           | 80          | **80**     |
| T7   | 65    | 80           | 85          | **85**     |

### PvP Damage Absorption by Tier

Using PvP formula: `Random(AR/4.3 to AR/2.4)`

| Tier | MAX AR | Min Absorb | Max Absorb | Avg Absorb |
|------|--------|------------|------------|------------|
| T1   | 45     | 10         | 19         | **15**     |
| T2   | 50     | 12         | 21         | **16**     |
| T3   | 55     | 13         | 23         | **18**     |
| T4   | 65     | 15         | 27         | **21**     |
| T5   | 75     | 17         | 31         | **24**     |
| T6   | 80     | 19         | 33         | **26**     |
| T7   | 85     | 20         | 35         | **28**     |

### Tier Advantage (vs 74 Damage Hit - Halberd + Vanq + Skills)

| Tier | Damage Taken | vs T1 Difference |
|------|--------------|------------------|
| T1   | 59 dmg       | -                |
| T2   | 58 dmg       | -1 dmg (2%)      |
| T3   | 56 dmg       | -3 dmg (5%)      |
| T4   | 53 dmg       | -6 dmg (10%)     |
| T5   | 50 dmg       | -9 dmg (15%)     |
| T6   | 48 dmg       | -11 dmg (19%)    |
| T7   | 46 dmg       | -13 dmg (22%)    |

**Design Intent:**
- Within same tier: Skill decides winner
- One tier group difference: Noticeable advantage
- T1 vs T7: ~22% damage reduction advantage

### Mining Success Rates

Reduced success rates to compensate for fewer ore types (7 instead of 9).

| Tier | Mining Skill | Base Find Rate | Notes                    |
|------|--------------|----------------|--------------------------|
| 1    | 0            | 100%           | Always found             |
| 2    | 55           | 30%            | Common colored ore       |
| 3    | 65           | 22%            | Uncommon colored ore     |
| 4    | 75           | 16%            | Mid-tier ore             |
| 5    | 85           | 10%            | Rare ore                 |
| 6    | 95           | 5%             | Very rare ore            |
| 7    | 99           | 2%             | Extremely rare ore       |

---

## Seasonal Rotation

Seasons rotate automatically every 3 months, synchronized with real-world seasons and faction season resets.

| Season | Real-World Dates       | Theme                    |
|--------|------------------------|--------------------------|
| Spring | March 20 - June 20     | Growth, renewal, fresh   |
| Summer | June 21 - September 21 | Heat, ocean, vibrant     |
| Fall   | September 22 - Dec 20  | Harvest, earth, twilight |
| Winter | December 21 - March 19 | Ice, darkness, cold      |

**When Season Changes:**
- All ore veins switch to new season's ore types
- Existing items KEEP their original name and hue
- Creates collectible seasonal variants over time

---

## Seasonal Ore Definitions (UO-Centric Names)

Using traditional UO ore names where possible, with some custom names for variety.

### Tier 1 (Iron) - All Seasons

Iron never changes - always available as baseline.

| Season | Name | Hue  |
|--------|------|------|
| All    | Iron | 0    |

---

### Spring Ores (March 20 - June 20)

Theme: Growth, renewal, fresh greens and soft colors

| Tier | Name         | Hue  | Color Description      |
|------|--------------|------|------------------------|
| 1    | Iron         | 0    | Gray (unchanged)       |
| 2    | Dull Copper  | 2419 | Dull copper tone       |
| 3    | Copper       | 2413 | Standard copper        |
| 4    | Bronze       | 2418 | Bronze metallic        |
| 5    | Verite       | 2207 | Green-tinted metal     |
| 6    | Agapite      | 2427 | Deep green             |
| 7    | Valorite     | 1281 | Rich purple-blue       |

---

### Summer Ores (June 21 - September 21)

Theme: Heat, ocean, warm tones

| Tier | Name         | Hue  | Color Description      |
|------|--------------|------|------------------------|
| 1    | Iron         | 0    | Gray (unchanged)       |
| 2    | Old Copper   | 1159 | Aged copper patina     |
| 3    | Rose         | 2948 | Rose gold tint         |
| 4    | Gold         | 2213 | Golden metallic        |
| 5    | Fire         | 2527 | Fiery orange-red       |
| 6    | Azure        | 1165 | Deep ocean blue        |
| 7    | Solarium     | 2720 | Radiant gold           |

---

### Fall Ores (September 22 - December 20)

Theme: Harvest, earth tones, twilight

| Tier | Name         | Hue  | Color Description      |
|------|--------------|------|------------------------|
| 1    | Iron         | 0    | Gray (unchanged)       |
| 2    | Shadow Iron  | 1904 | Dark iron              |
| 3    | Amethyst     | 1163 | Purple gemstone        |
| 4    | Blood Rock   | 1218 | Deep crimson           |
| 5    | Mytheril     | 1325 | Silvery-blue           |
| 6    | Dwarven      | 1940 | Dark metallic          |
| 7    | Daemon Steel | 1171 | Infernal dark metal    |

---

### Winter Ores (December 21 - March 19)

Theme: Ice, cold, darkness, silver

| Tier | Name         | Hue  | Color Description      |
|------|--------------|------|------------------------|
| 1    | Iron         | 0    | Gray (unchanged)       |
| 2    | Silver       | 1175 | Pure silver            |
| 3    | Frost        | 1152 | Light ice blue         |
| 4    | Ice          | 2513 | Frozen blue            |
| 5    | Glacial      | 1266 | Deep frozen blue       |
| 6    | Obsidian     | 1109 | Near-black volcanic    |
| 7    | Black Rock   | 1150 | Deepest black          |

---

## Complete Ore Reference Table

All seasons side-by-side for easy reference.

| Tier | Plate AR | Spring        | Summer       | Fall          | Winter        |
|------|----------|---------------|--------------|---------------|---------------|
| 1    | 25       | Iron          | Iron         | Iron          | Iron          |
| 2    | 30       | Dull Copper   | Old Copper   | Shadow Iron   | Silver        |
| 3    | 35       | Copper        | Rose         | Amethyst      | Frost         |
| 4    | 45       | Bronze        | Gold         | Blood Rock    | Ice           |
| 5    | 55       | Verite        | Fire         | Mytheril      | Glacial       |
| 6    | 60       | Agapite       | Azure        | Dwarven       | Obsidian      |
| 7    | 65       | Valorite      | Solarium     | Daemon Steel  | Black Rock    |

### Hue Reference

| Tier | Spring (Hue) | Summer (Hue) | Fall (Hue) | Winter (Hue) |
|------|--------------|--------------|------------|--------------|
| 1    | 0            | 0            | 0          | 0            |
| 2    | 2419         | 1159         | 1904       | 1175         |
| 3    | 2413         | 2948         | 1163       | 1152         |
| 4    | 2418         | 2213         | 1218       | 2513         |
| 5    | 2207         | 2527         | 1325       | 1266         |
| 6    | 2427         | 1165         | 1940       | 1109         |
| 7    | 1281         | 2720         | 1171       | 1150         |

---

## PvP & PvM Impact Summary

### PvP Balance

| Tier Group | AR Range (Plate) | vs 50 DMG Hit  | Skill Factor          |
|------------|------------------|----------------|-----------------------|
| Low (1-2)  | 40-44 AR         | Take 15-20 dmg | Skill dominant        |
| Mid (3-5)  | 48-58 AR         | Take 5-14 dmg  | Skill important       |
| High (6-7) | 64-76 AR         | Take 0-8 dmg   | Gear gives edge       |

**Cross-Group Fights:**
- Low vs Mid: Mid has advantage, Low can win with skill
- Mid vs High: High has noticeable advantage
- Low vs High: High dominates, Low needs exceptional play

### PvM Balance

| Content        | Recommended Tier | Why?                              |
|----------------|------------------|-----------------------------------|
| Easy dungeons  | Tier 1-3         | Low damage, good for new players  |
| Medium dungeons| Tier 3-5         | Moderate damage, gear helps       |
| Hard dungeons  | Tier 5-6         | High damage, good gear important  |
| Extreme/Raids  | Tier 6-7         | Extreme damage, best gear needed  |

---

## Factors That Affect Damage (Full Reference)

### Armor-Related
- **Ore Tier AR** - Absolute AR value by tier (T1=25 to T7=65 for plate)
- **Quality Bonus** - Exceptional is **cosmetic only** (no AR bonus)
- **Protection Spell** - Adds +5 AR (fixed)
- **Durability** - Damaged armor = reduced AR

### Resistance-Related (Elemental Damage)
- **Base Resistances** - From armor type
- **Resistance Cap** - 70% max for players
- **Elemental Damage Split** - Attacks can be mixed physical/elemental

### Combat-Related
- **Parry Skill** - Shield can block/reduce damage
- **Weapon Abilities** - Armor Ignore bypasses AR
- **Special Moves** - Various effects

### What Runic Bonuses Add (PvE Only)
- **Resist Bonuses** - +1-15% per element
- **Regen Bonuses** - HP/Mana/Stam regen
- **Luck** - Better loot drops
- **Lower Requirements** - Easier to equip
- **Self Repair** - Auto-repair durability

**Remember:** Runic bonuses are PvE-only per Option E-1 decision.

---

## Runic Tool System

### Runic Tool Properties by Tier

| Tier | Uses   | Properties | Intensity   |
|------|--------|------------|-------------|
| 2    | 25-50  | 1-2        | 40-100%     |
| 3    | 25-50  | 2          | 45-100%     |
| 4    | 30-55  | 2-3        | 50-100%     |
| 5    | 30-55  | 3-4        | 60-100%     |
| 6    | 35-60  | 4          | 70-100%     |
| 7    | 40-75  | 5          | 85-100%     |

### How to Obtain Runic Tools
- **Bulk Order Deeds (BODs)** - Primary source
- **Rare mining drops** - Small chance with Tier 5+ ore
- **High-end dungeon loot** - Boss drops

---

## Implementation Notes

### Season Detection Code

```csharp
public static Season GetCurrentSeason()
{
    var now = DateTime.UtcNow;
    var month = now.Month;
    var day = now.Day;

    // Spring: March 20 - June 20
    if ((month == 3 && day >= 20) || month == 4 || month == 5 || (month == 6 && day <= 20))
        return Season.Spring;

    // Summer: June 21 - September 21
    if ((month == 6 && day >= 21) || month == 7 || month == 8 || (month == 9 && day <= 21))
        return Season.Summer;

    // Fall: September 22 - December 20
    if ((month == 9 && day >= 22) || month == 10 || month == 11 || (month == 12 && day <= 20))
        return Season.Fall;

    // Winter: December 21 - March 19
    return Season.Winter;
}
```

---

## Questions Resolved

| Question                          | Answer                                         |
|-----------------------------------|------------------------------------------------|
| AR bonus additive?                | Yes, added to base armor AR                    |
| Gap structure?                    | +4/+4/+4/+6/+6/+12 for meaningful progression  |
| Mining skill requirements?        | 0, 55, 65, 75, 85, 95, 99                      |
| Season change mechanism?          | Automatic, synced with real seasons            |
| Existing items on season change?  | Keep original name and hue                     |
| Ore names?                        | UO-centric (traditional names where possible)  |
| Durability by tier?               | Higher tier = more durable (see PVP_COMBAT_SYSTEM.md) |

---

## Mining Stones (Rare Drops)

Two rare stone types can be found while mining. Both apply RNG bonuses on use.

### Drop Chance (Base)

| Mining Skill | Stone Drop Chance per Swing |
|--------------|----------------------------|
| 0-49 | 0.1% |
| 50-79 | 0.2% |
| 80-99 | 0.3% |
| GM (100) | 0.5% |

*Higher tier ore veins have slightly better drop rates (+0.1% per tier above T1)*

---

### 1. Sharpening Stone (Damage Enhancement)

**Single item type** - RNG determines Force/Power/Vanquishing on application.

| Result | Flat Damage Bonus | Weapon Name Prefix |
|--------|-------------------|-------------------|
| Force | +6 | "a force [weapon]" |
| Power | +8 | "a power [weapon]" |
| Vanquishing | +10 | "a vanquishing [weapon]" |

**RNG Weights by Mining Skill:**

| Mining Skill | Force | Power | Vanquishing |
|--------------|-------|-------|-------------|
| 0-49 | 80% | 18% | 2% |
| 50-79 | 70% | 25% | 5% |
| 80-99 | 55% | 35% | 10% |
| GM (100) | 40% | 40% | 20% |

**Rules:**
- Consumable (one-time use)
- Bonus is PERMANENT (cannot be removed)
- Only ONE stone per weapon
- Applied via Blacksmithing menu

---

### 2. Accuracy Stone (Hit Chance Enhancement)

**Single item type** - RNG determines accuracy level on application.

| Result | Tactics Bonus | Hit Chance Bonus | Weapon Name Prefix |
|--------|---------------|------------------|-------------------|
| Accurate | +5 | +2% | "accurate [weapon]" |
| Surpassingly | +10 | +4% | "surpassingly accurate [weapon]" |
| Eminently | +15 | +6% | "eminently accurate [weapon]" |
| Exceedingly | +20 | +8% | "exceedingly accurate [weapon]" |
| Supremely | +25 | +10% | "supremely accurate [weapon]" |

**RNG Weights by Mining Skill:**

| Mining Skill | Accurate | Surpassingly | Eminently | Exceedingly | Supremely |
|--------------|----------|--------------|-----------|-------------|-----------|
| 0-49 | 70% | 20% | 8% | 2% | 0% |
| 50-79 | 50% | 30% | 15% | 4% | 1% |
| 80-99 | 30% | 35% | 20% | 10% | 5% |
| GM (100) | 15% | 30% | 25% | 18% | 12% |

**Rules:**
- Consumable (one-time use)
- Bonus is PERMANENT (cannot be removed)
- Only ONE stone per weapon
- Applied via Blacksmithing menu
- Tactics bonus applies as skill modifier while weapon equipped

---

### Stacking Rules

- **Sharpening + Accuracy:** Both can be on same weapon
- **Multiple of same type:** NOT allowed
- Example: "a supremely accurate vanquishing halberd" = legal

---

## Weapon System (Hold)

Weapon bonuses will follow a similar tier structure. Will be designed after armor system is finalized, keeping in mind:
- Damage bonuses that complement AR differences
- Custom weapon designs
- PvP and PvM balance


##Aditionall Ideas to make leather ect more useable :
Creature-Derived Armor & Weapon Counter System

Design Document – v1.0

Purpose

Create a craft-driven armor identity system where:
Armor choice matters
Partial suits are viable
Weapon preparation counters armor
Crafting and hunting drive the economy
Visual customization exists without mechanical advantage
This system follows classic Ultima Online philosophy:
knowledge, preparation, and risk management over raw stats.

CORE DESIGN RULES

All special armor is crafted (no full armor drops)
Armor bonuses and vulnerabilities scale per piece
Weapon counters are crafted, not slayers
No Strength or Intelligence bonuses anywhere
Survivability uses HP / Mana / mitigation
No silver or legacy exceptions
Full suits grant bonuses only, never extra vulnerability
Cosmetic helmet variants use real UO items only
Visual choice ≠ mechanical advantage

ARMOR OVERVIEW (TAILORING)

Four creature-derived armor sets exist:

Armor Type	Theme	Crafted Via
Lich Bone	Mana sustain	Tailoring
Dragon Scale	Fire mitigation	Tailoring
Titan Skin	High HP endurance	Tailoring
Ophidian Silk	Large mana pool	Tailoring

Each armor piece:
Grants a per-piece bonus
Applies a per-piece vulnerability
Counts toward full suit bonuses

ARMOR PIECE SCALING
Per-Piece Vulnerability (Standardized)
+5% damage taken per matching piece
Capped at 25% total

Pieces Worn	Vulnerability
1	+5%
2	+10%
3	+15%
4	+20%
5+	+25% (cap)
ARMOR DETAILS
1. Lich Bone Armor
Per Piece Bonus
+3 Max Mana
Per Piece Vulnerability
+5% damage taken from Lichbane weapons
Full Suit Bonus (5+)
+10 Max Mana

2. Dragon Scale Armor
Per Piece Bonus
−5% Fire Spell damage taken
(Total cap −15%)
Per Piece Vulnerability
+5% damage taken from Dragonbane weapons
Full Suit Bonus
−5% additional Fire damage
(Still subject to cap)

3. Titan Skin Armor
Per Piece Bonus
+4 Max HP
Per Piece Vulnerability
+5% damage taken from Titanbane weapons
Full Suit Bonus
+15 Max HP

4. Ophidian Silk Armor
Per Piece Bonus
+5 Max Mana

Per Piece Vulnerability
+5% damage taken from Ophidianbane weapons
Full Suit Bonus
−10% mana cost

PARTIAL SUIT RULES
Any number of pieces may be mixed with normal armor
Bonuses and vulnerabilities apply only for pieces worn
No minimum set requirement
No “free gloves” exploits

Example:
2 Titan Skin pieces →
+8 HP
+10% damage taken from Titanbane weapons

WEAPON COUNTERS
(Tinkering / Blacksmithing)
Weapon counters are crafted bonuses, not slayers.
Key Rules
All four bonuses may exist on one weapon
Each bonus applies independently
No effect vs normal armor
Cannot stack the same bonus twice

Weapon Counter Bonuses
Bonus	Effect
Lichbane	+25% damage vs Lich Bone armor
Dragonbane	+25% damage vs Dragon Scale armor
Titanbane	+25% damage vs Titan Skin armor
Ophidianbane	+25% damage vs Ophidian Silk armor

Crafted via:
Blacksmithing → melee weapons
Tinkering → bows / crossbows / devices

DAMAGE CALCULATION
Final Damage =
Base Damage
× (Weapon Counter ? 1.25 : 1.0)
× (1 + Armor Piece Vulnerability %)

Example

Defender wears 3 Titan Skin pieces
Attacker uses Titanbane weapon

Damage × 1.25 × 1.15 = 1.44×

HELMET & HAT VARIANTS (COSMETIC SYSTEM)
Purpose

Allow players to wear real UO hats instead of Caps or Bone Helms
with identical stats, using extra creature materials.

CREATURE-BOUND CLOTH DROPS

Creatures drop a matching cloth used only for aesthetic variants.

Creature	Cloth Drop
Lich	Lichweave Cloth
Dragon	Dragonscale Cloth
Titan	Titanweft Cloth
Ophidian	Ophidian Silk Cloth

Drop Rules
5–10% chance
Stackable
Crafting-only

REAL UO HEAD ITEMS (NO CUSTOM ITEMS)
Valid UO Headwear
Cap
Skullcap
Bandana
Floppy Hat
Wide-Brim Hat
Tall Straw Hat
Feathered Hat
Tricorne Hat
Jester Hat
Wizard’s Hat
Bonnet
Kasa
Bone Helm
Orc Helm

HELMET VARIANT RULES
Counts as 1 armor piece
Same AR as Cap / Bone Helm
Same bonuses & vulnerabilities
Requires extra creature cloth
No mechanical advantage
Example tooltip:

Wizard’s Hat
(Counts as Lich Bone Armor)
AR: X
+3 Max Mana
+5% damage taken from Lichbane weapons

CRAFTING REQUIREMENTS
Standard Helm
Creature material
Leather
Variant Hat
Creature material
Leather
Creature-bound cloth
+10–15 Tailoring skill requirement

UI / UX REQUIREMENTS
Crafting gumps show variant grouping
Tooltips clearly state armor type
Piece count visible to player
Weapon bonuses listed explicitly

BALANCE SAFEGUARDS
Max armor vulnerability: 25%
Weapon counter bonus: 25% flat
No stat inflation
No hidden modifiers
No immunities

DESIGN PHILOSOPHY SUMMARY
Mixing armor is encouraged
Greed is punished
Knowledge wins fights
Crafting matters
Fashion is allowed, power is not
This system is modular, era-respectful, and PvP readable.
---

## Change Log

| Date       | Change                                    | Reason                          |
|------------|-------------------------------------------|---------------------------------|
| 2025-01-14 | Initial draft                             | Seasonal ore system design      |
| 2025-01-14 | Increased AR bonuses, UO names            | More meaningful PvP impact      |
| 2025-01-14 | Added AR formula documentation            | Clarity on damage mechanics     |
