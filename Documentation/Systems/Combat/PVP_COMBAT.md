# PvP Combat System

**Status**: DESIGN COMPLETE - Ready for Implementation
**Last Updated**: 2025-01-15
**Purpose**: Define all PvP-specific combat formulas and values
**Authority**: TIER 2 - Implementation details for DESIGN_DECISIONS.md §17

> **Authoritative source**: `DESIGN_DECISIONS.md` Section 17
> **Related docs**: `SEASONAL_ORE_SYSTEM.md` (armor tiers), `specs/talisman-system.md` (ability gating)
> **Index**: See `Documentation/INDEX.md` for topic lookup

---

## Overview

This shard uses a **hybrid combat system** where PvP and PvM have separate formulas. This allows precise PvP balance without affecting existing PvM content.

**Design Philosophy:**
- PvP uses ImaginNation-style formulas (proven balanced)
- PvM keeps default ModernUO formulas (already balanced)
- Same gear works for both - formulas change based on context

---

## PvP vs PvM Formula Summary

| Mechanic | PvP Formula | PvM Formula |
|----------|-------------|-------------|
| **Hit Chance** | `0.69 × ((Skill + Tactics×2) / 300)` | Default AOS formula |
| **AR Absorption** | `Random(AR/4.3 to AR/2.4)` | `Random(AR/2 to AR)` |
| **Parry Chance** | 20% with shield at GM | 30% with shield at GM |
| **Parry Method** | Shield only | Shield or weapon |

---

## Hit Chance (PvP Only)

### Formula

```csharp
// Only applies when BOTH attacker AND defender are players
hitChance = 0.69 × ((WeaponSkill + (Tactics × 2)) / 300)
```

### Key Points

- **Tactics counts double** - Very important skill
- **No defender skill** affects hit chance
- **69% max at GM** (100 Weapon Skill, 100 Tactics)
- Simpler than stock UO - no attacker vs defender calculation

### Examples

| Weapon Skill | Tactics | Hit Chance |
|--------------|---------|------------|
| 100 | 100 | **69%** |
| 100 | 80 | **60%** |
| 80 | 100 | **64%** |
| 80 | 80 | **55%** |
| 100 | 0 | **23%** |

### Implementation

```csharp
public virtual bool CheckHit(Mobile attacker, Mobile defender)
{
    // PvP: Use ImaginNation formula
    if (attacker is PlayerMobile && defender is PlayerMobile)
    {
        var weapon = attacker.Weapon as BaseWeapon;
        var weaponSkill = attacker.Skills[weapon?.Skill ?? SkillName.Wrestling].Value;
        var tactics = attacker.Skills[SkillName.Tactics].Value;

        double hitChance = 0.69 * ((weaponSkill + (tactics * 2.0)) / 300.0);

        // Skill check for gain opportunity
        attacker.CheckSkill(weapon?.Skill ?? SkillName.Wrestling, hitChance);

        return Utility.RandomDouble() <= hitChance;
    }

    // PvM: Use existing default formula
    return CheckHitDefault(attacker, defender);
}
```

---

## Armor Absorption (PvP Only)

### Formula

```csharp
// Only applies when BOTH attacker AND defender are players
absorbed = Random(AR / 4.3, AR / 2.4)
damageTaken = incomingDamage - absorbed
```

### Key Points

- **Much weaker than PvM** - prevents invulnerability
- **Armor still matters** - higher AR = more absorption
- **Always some damage** - no 0-damage scenarios at reasonable AR

### Comparison to PvM

| AR | PvP Min Absorb | PvP Max Absorb | PvM Min Absorb | PvM Max Absorb |
|----|----------------|----------------|----------------|----------------|
| 40 | 9 | 17 | 20 | 40 |
| 60 | 14 | 25 | 30 | 60 |
| 80 | 19 | 33 | 40 | 80 |
| 100 | 23 | 42 | 50 | 100 |

### Implementation

```csharp
public virtual int AbsorbDamage(Mobile attacker, Mobile defender, int damage)
{
    int ar = GetArmorRating();
    int absorbed;

    // PvP: Use ImaginNation formula (weaker absorption)
    if (attacker is PlayerMobile && defender is PlayerMobile)
    {
        int min = (int)(ar / 4.3);
        int max = (int)(ar / 2.4);
        absorbed = Utility.RandomMinMax(min, max);
    }
    // PvM: Use default formula (stronger absorption)
    else
    {
        int min = ar / 2;
        int max = ar;
        absorbed = Utility.RandomMinMax(min, max);
    }

    return Math.Max(0, damage - absorbed);
}
```

---

## Parry System (PvP Only)

### Rules

- **Only shields can parry** in PvP (no weapon parrying)
- **20% parry chance** at GM Parry with shield
- **Successful parry = 100% block** (0 damage)
- Bushido does NOT affect PvP parry (requires Talisman)

### Formula

```csharp
// PvP parry (shield only, fixed 20% at GM)
if (attacker is PlayerMobile && defender is PlayerMobile)
{
    BaseShield shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

    if (shield == null)
        return false; // No weapon parrying in PvP

    double parry = defender.Skills[SkillName.Parry].Value;
    double chance = parry / 500.0; // 100 skill = 20%

    return Utility.RandomDouble() <= chance;
}
```

### Parry Chance by Skill

| Parry Skill | Chance |
|-------------|--------|
| 100 (GM) | 20% |
| 80 | 16% |
| 60 | 12% |
| 50 | 10% |

---

## Armor Tiers (PvP AR Values)

### Plate Armor by Tier

| Tier | Base AR | Mining Req | Rarity |
|------|---------|------------|--------|
| T1 (Iron) | 25 | 0 | Common |
| T2 | 30 | 55 | Uncommon |
| T3 | 35 | 65 | Uncommon |
| T4 | 45 | 75 | Rare |
| T5 | 55 | 85 | Rare |
| T6 | 60 | 95 | Very Rare |
| T7 | 65 | 99 | Extremely Rare |

### Tier Gaps

| Gap | AR Increase |
|-----|-------------|
| T1 → T2 | +5 |
| T2 → T3 | +5 |
| T3 → T4 | +10 |
| T4 → T5 | +10 |
| T5 → T6 | +5 |
| T6 → T7 | +5 |

### Tier Groupings

| Group | Tiers | AR Range | Notes |
|-------|-------|----------|-------|
| Low | T1-T2 | 25-30 | Entry level |
| Mid | T3-T4 | 35-45 | Big jump at T4 |
| High | T5-T6 | 55-60 | End game |
| Elite | T7 | 65 | Best in slot |

---

## Shield AR Values

### Flat Values (Not Scaling)

| Shield | Non-GM Parry | GM Parry (100) | Notes |
|--------|--------------|----------------|-------|
| Buckler | 1 | 5 | |
| Wooden Shield | 5 | 10 | |
| Heater Shield | 7 | 15 | |
| Order Shield | 10 | 20 | Faction: Lycaeum Order |
| Chaos Shield | 10 | 20 | Faction: Bridgefolk |
| Dupres Shield | 10 | 20 | Faction: Golden Shield |

**Note:** 
### Three Factions
| Faction | Home City | Hue | Color |
|---------|-----------|-----|-------|
| The Golden Shield | Trinsic | 2721 | Gold |
| The Bridgefolk | Vesper | 2784 | Blue |
| The Lycaeum Order | Moonglow | 2602 | Purple |

### Implementation

```csharp
public override int ArmorRating
{
    get
    {
        Mobile parent = Parent as Mobile;
        if (parent == null)
            return BaseArmorRating;

        double parry = parent.Skills[SkillName.Parry].Value;
        bool isGM = parry >= 100.0;

        // Return flat value based on GM status
        return isGM ? GMParryAR : NonGMParryAR;
    }
}
```

---

## Protection Spell

### Value

**Fixed +5 AR** (not variable)

### Implementation

```csharp
public class ProtectionSpell : MagerySpell
{
    private const int ArmorBonus = 5; // Fixed value

    public override void OnCast()
    {
        // Apply +5 AR via VirtualArmor
        target.VirtualArmor += ArmorBonus;
    }
}
```

---

## Sharpening Stones (Weapon Enhancement)

### Damage Bonuses (RNG on Application)

| Result | Flat Damage Bonus | Weapon Name Prefix |
|--------|-------------------|-------------------|
| Force | +6 | "a force [weapon]" |
| Power | +8 | "a power [weapon]" |
| Vanquishing | +10 | "a vanquishing [weapon]" |

### Rules

- **Single item type** - "Sharpening Stone" (not three separate stones)
- **RNG on application** - when used, rolls Force/Power/Vanq
- **Higher mining skill = better odds** for Power/Vanquishing results
- Bonus is **PERMANENT** (cannot be removed or replaced)
- **Rare mining drop** - found while mining ore
- Only ONE stone can be applied per weapon
- Applied via crafting menu (Blacksmithing)

### Acquisition

- **Mining only** - rare chance when mining any ore
- Higher tier ore = slightly better drop chance
- Cannot be crafted, only found

### RNG Weights by Mining Skill

| Mining Skill | Force | Power | Vanquishing |
|--------------|-------|-------|-------------|
| 0-49 | 80% | 18% | 2% |
| 50-79 | 70% | 25% | 5% |
| 80-99 | 55% | 35% | 10% |
| GM (100) | 40% | 40% | 20% |

*Exact percentages TBD during testing*

### Implementation

```csharp
public class SharpeningStone : Item
{
    public void ApplyTo(BaseWeapon weapon, Mobile from)
    {
        if (weapon.SharpeningBonus > 0)
        {
            from.SendMessage("This weapon already has a sharpening bonus.");
            return;
        }

        // RNG based on mining skill
        double miningSkill = from.Skills[SkillName.Mining].Value;
        int roll = Utility.Random(100);

        int bonus;
        string result;

        if (miningSkill >= 100 && roll < 20) // GM: 20% Vanq
        {
            bonus = 10;
            result = "vanquishing";
        }
        else if (miningSkill >= 80 && roll < 10) // 80+: 10% Vanq
        {
            bonus = 10;
            result = "vanquishing";
        }
        else if (roll < (40 + miningSkill * 0.2)) // Power chance scales
        {
            bonus = 8;
            result = "power";
        }
        else
        {
            bonus = 6;
            result = "force";
        }

        weapon.SharpeningBonus = bonus;
        from.SendMessage($"The stone enhances the weapon to {result} quality!");
        Delete(); // Consume the stone
    }
}
```

---

## Damage Calculation (Full Formula)

### PvP Damage Flow

```
1. Base Weapon Damage = Random(MinDamage, MaxDamage)
2. + Sharpening Stone Bonus (+6/+8/+10)
3. × Skill Multiplier (STR + Anatomy + Tactics + Lumberjacking)
4. = Gross Damage

5. Check Hit (69% at GM)
6. If Hit → Check Parry (20% at GM with shield)
7. If Not Parried → Apply AR Absorption (AR/4.3 to AR/2.4)
8. = Net Damage Taken
```

### Example Calculation

**Attacker:** Halberd (18-19) + Vanq Stone (+10) + Full Skills (+159%)

```
Base: 18.5 + 10 = 28.5
With Skills: 28.5 × 2.59 = 73.8 damage
```

**Defender:** T7 Plate (65) + Heater Shield (15) + Protection (5) = 85 AR

```
AR Absorption: Random(85/4.3, 85/2.4) = Random(20, 35) = ~27.5 avg
Damage Taken: 73.8 - 27.5 = 46.3 damage
```

---

## Full Combat Scenarios

### Max AR Builds by Tier

| Tier | Plate | +Heater (GM) | +Protection | **MAX AR** |
|------|-------|--------------|-------------|------------|
| T1 | 25 | 40 | 45 | **45** |
| T2 | 30 | 45 | 50 | **50** |
| T3 | 35 | 50 | 55 | **55** |
| T4 | 45 | 60 | 65 | **65** |
| T5 | 55 | 70 | 75 | **75** |
| T6 | 60 | 75 | 80 | **80** |
| T7 | 65 | 80 | 85 | **85** |

### Damage Absorption by Tier (PvP Formula)

| Tier | MAX AR | Min Absorb | Max Absorb | Avg Absorb |
|------|--------|------------|------------|------------|
| T1 | 45 | 10.5 | 18.8 | **15** |
| T2 | 50 | 11.6 | 20.8 | **16** |
| T3 | 55 | 12.8 | 22.9 | **18** |
| T4 | 65 | 15.1 | 27.1 | **21** |
| T5 | 75 | 17.4 | 31.3 | **24** |
| T6 | 80 | 18.6 | 33.3 | **26** |
| T7 | 85 | 19.8 | 35.4 | **28** |

### Damage Taken by Tier (vs 73.8 Halberd + Vanq + Skills)

| Tier | MAX AR | Avg Absorbed | **Avg Damage Taken** |
|------|--------|--------------|---------------------|
| T1 | 45 | 15 | **59 dmg** |
| T2 | 50 | 16 | **58 dmg** |
| T3 | 55 | 18 | **56 dmg** |
| T4 | 65 | 21 | **53 dmg** |
| T5 | 75 | 24 | **50 dmg** |
| T6 | 80 | 26 | **48 dmg** |
| T7 | 85 | 28 | **46 dmg** |

### Tier Advantage Summary

| Matchup | Damage Difference | % Advantage |
|---------|-------------------|-------------|
| T1 vs T7 | 13 damage | 22% less damage |
| T4 vs T7 | 7 damage | 13% less damage |
| T6 vs T7 | 2 damage | 4% less damage |

---

## Effective Combat Rate

### Per 10 Swings (GM Skills)

| Step | Calculation | Result |
|------|-------------|--------|
| Swings | 10 | 10 |
| × Hit Rate | × 69% | 6.9 hits |
| - Parried | × 80% (20% parry) | 5.5 damage events |
| × Damage per Hit | × 46 (vs T7) | **253 total damage** |

### Time to Kill (Assuming 100 HP)

| Defender Tier | Damage per Hit | Hits to Kill | Swings Needed |
|---------------|----------------|--------------|---------------|
| T1 (45 AR) | 59 | 2 | ~4 swings |
| T4 (65 AR) | 53 | 2 | ~4 swings |
| T7 (85 AR) | 46 | 3 | ~6 swings |

---

## Allowed Weapons

### Removed from Crafting/Loot

- Cutlass, Pike, WarFork, Pitchfork, Leafblade
- CompositeBow, WarMace
- All weapons from earlier removal list (see COMBAT_BALANCE_REFERENCE.md)

### Remaining Weapons

| Weapon | Damage | Speed | Skill |
|--------|--------|-------|-------|
| **SWORDS** |
| Broadsword | 14-15 | 3.25s | Swords |
| Longsword | 15-16 | 3.50s | Swords |
| Scimitar | 13-15 | 3.00s | Swords |
| **FENCING** |
| Kryss | 10-12 | 2.00s | Fencing |
| Dagger | 10-11 | 2.00s | Fencing |
| ShortSpear | 10-13 | 2.00s | Fencing |
| Spear | 13-15 | 2.75s | Fencing |
| **AXES** |
| Hatchet | 13-15 | 2.75s | Swords |
| Axe | 14-16 | 3.00s | Swords |
| ExecutionersAxe | 15-17 | 3.25s | Swords |
| Pickaxe | 13-15 | 3.00s | Swords |
| LargeBattleAxe | 16-17 | 3.75s | Swords |
| **MACING** |
| Club | 11-13 | 2.50s | Macing |
| Maul | 14-16 | 3.50s | Macing |
| HammerPick | 15-17 | 3.75s | Macing |
| WarHammer | 17-18 | 3.75s | Macing |
| QuarterStaff | 11-14 | 2.25s | Macing |
| WarAxe | 14-15 | 3.25s | Macing |
| **POLEARMS** |
| Bardiche | 17-18 | 3.75s | Swords |
| Halberd | 18-19 | 4.25s | Swords |
| **RANGED** |
| Bow | 15-19 | 4.25s | Archery |
| Crossbow | 18-22 | 4.50s | Archery |
| HeavyCrossbow | 19-24 | 5.00s | Archery |

---

## Quality & Crafting

### Exceptional Quality

- **NO AR BONUS** (cosmetic only)
- Displays crafter's mark
- Same AR as normal quality

### Ore Bonuses

- AR bonuses apply per tier (T1-T7)
- Same bonus for normal and exceptional
- Ore determines AR, not quality

### Weapon Ore Tier Benefits

Higher tier ore provides **TWO** benefits:

**1. Increased Durability:**
| Tier | Durability Bonus |
|------|------------------|
| T1 | Base |
| T2 | +10% |
| T3 | +20% |
| T4 | +35% |
| T5 | +50% |
| T6 | +70% |
| T7 | +100% |

**2. Better Stone RNG:**
When applying Accuracy or Sharpening stones, higher tier weapons get better odds for top-tier results.

| Weapon Tier | Force/Power/Vanq Odds | Accuracy Bonus Odds |
|-------------|----------------------|---------------------|
| T1-T3 | Standard mining skill odds | Standard odds |
| T4-T5 | +5% to higher tiers | +5% to higher tiers |
| T6-T7 | +10% to higher tiers | +10% to higher tiers |

**Note:** NO flat damage bonus per ore tier. Damage is from base weapon + sharpening stones only.

---

## Talisman-Gated Abilities (PvP)

### Overview

**Four Talisman Types** exist: Dexer, Tamer, Sampire, Treasure Hunter. Each has T1/T2/T3 tiers.

Certain skill-based abilities are **DISABLED in PvP** unless the player has the appropriate Talisman equipped. This creates meaningful gear choices and prevents skill bloat in PvP builds.

**IMPORTANT:** Bushido, Chivalry, Necromancy, and Weapon Special Moves are NOT talismans - they are **abilities GATED BY talismans**.

### Talisman Types and Gated Abilities

| Talisman Type | Build Archetype | Abilities Unlocked in PvP |
|---------------|-----------------|---------------------------|
| **Dexer** | Pure melee fighter | Bushido, Weapon Special Moves |
| **Tamer** | Animal tamer | Taming bonuses, pet control |
| **Sampire** | Paladin warrior | Chivalry, Necromancy |
| **Treasure Hunter** | Dungeon explorer | Lockpicking, cartography bonuses |

### Disabled Without Talisman (PvP Only)

| Ability | Required Talisman | What's Disabled |
|---------|-------------------|-----------------|
| **Bushido** | Dexer | All special moves, parry bonus |
| **Weapon Special Moves** | Dexer | Mortal Strike, etc. |
| **Chivalry** | Sampire | All paladin abilities |
| **Necromancy** | Sampire | All necro spells/forms |

### Key Mechanic: Talisman PvP Drop

When a player flags for PvP with a talisman equipped:
1. **Talisman drops to backpack instantly**
2. **ALL gated abilities are disabled** for that fight
3. **5-minute cooldown** before re-equipping
4. **PvM unaffected** - abilities work normally vs monsters

### Talisman Skill Reduction

When worn, talismans **reduce all OTHER skills to 0**. This creates OSI 7-skill style template builds where you commit to a specific playstyle.

### Rules

1. **PvP Check:** Only applies when `attacker is PlayerMobile && defender is PlayerMobile`
2. **Instant Disable:** Abilities fail immediately if Talisman not equipped (dropped on PvP flag)
3. **No Partial Effect:** Ability either works fully or not at all
4. **PvM Unaffected:** All abilities work normally vs monsters regardless of Talisman

### Behavior When Missing Talisman

```csharp
// Example: Bushido parry check in PvP
if (attacker is PlayerMobile && defender is PlayerMobile)
{
    // Check for Dexer Talisman (gates Bushido)
    if (!HasTalisman(defender, TalismanType.Dexer))
    {
        // Bushido parry bonus does not apply
        bushidoBonus = 0;
    }
}
```

### Message to Player

When a player attempts to use a Talisman-gated ability in PvP without the Talisman:

> "You need a [Talisman Type] Talisman equipped to use this ability in PvP."

### Design Intent

- **Prevents "do everything" builds** - Can't have melee + necro + chiv + bushido simultaneously
- **Creates specialization** - Pick your Talisman type, pick your playstyle
- **Skill templates like OSI** - Talisman reduces other skills to 0, forcing commitment
- **Encourages Talisman economy** - Players need to acquire/craft Talismans
- **Keeps PvM accessible** - New players can use all abilities vs monsters

### Talisman Tiers

Each of the 4 talisman types has 3 tiers:

| Tier | Name Pattern | Timer |
|------|--------------|-------|
| T3 | "Bloodthorn" | 7 days gameplay |
| T2 | "Bloodmage" | 7 days gameplay |
| T1 | "Bloodlord" | 7 days gameplay |

*See `specs/talisman-system.md` for full crafting requirements and stat blocks.*

---

## Implementation Checklist

### Code Changes Required

**Combat Formulas:**
1. [ ] **BaseWeapon.cs** - Add PvP hit chance formula
2. [ ] **BaseArmor.cs** - Add PvP absorption formula
3. [ ] **BaseWeapon.cs** - Add PvP parry logic (shield only, 20%)
4. [ ] **BaseShield.cs** - Change AR to flat values
5. [ ] **ProtectionSpell.cs** - Fix AR bonus to +5

**Armor & Crafting:**
6. [ ] **CraftResource.cs** - Update ore AR bonuses for 7 tiers
7. [ ] **BaseArmor.cs** - Remove exceptional AR bonus
8. [ ] **DefBlacksmithy.cs** - Remove chainmail/ringmail pieces (keep tunics)
9. [ ] **LootTables** - Remove disallowed weapons/armor
10. [ ] **CraftingMenus** - Remove disallowed weapons

**Mining Stones:**
11. [ ] **SharpeningStone.cs** - Create item (RNG on application: Force/Power/Vanq)
12. [ ] **AccuracyStone.cs** - Create item (RNG on application: 5 accuracy levels)
13. [ ] **Mining.cs** - Add stone drop chance to mining

**Talisman System:**
14. [ ] **BaseTalisman.cs** - Create base talisman class
15. [ ] **TalismanTypes** - Create Bushido/Chivalry/Necromancy/WeaponMaster
16. [ ] **Ability checks** - Add Talisman requirement for PvP abilities
17. [ ] **Bushido.cs** - Disable parry bonus in PvP without Talisman
18. [ ] **Chivalry spells** - Disable in PvP without Talisman
19. [ ] **Necromancy spells** - Disable in PvP without Talisman
20. [ ] **WeaponAbility.cs** - Disable special moves in PvP without Talisman

### Testing Commands

```
[TestPvPHit - Test hit chance formula
[TestPvPAbsorb - Test absorption formula
[TestPvPParry - Test parry system
[SetArmorTier <tier> - Set armor to specific tier
[GiveSharpStone - Give sharpening stone (single type)
[GiveAccuracyStone - Give accuracy stone (single type)
[GiveTalisman <type> - Give specific talisman type
[TestTalismanGate - Test ability gating in PvP
```

---

## Finalized Decisions Summary

All combat decisions locked as of 2025-01-15:

### Combat Formulas
| System | PvP | PvM |
|--------|-----|-----|
| **Hit Chance** | `0.69 × ((Skill + Tactics×2) / 300)` = 69% max | Default formula |
| **AR Absorption** | `Random(AR/4.3 to AR/2.4)` | `Random(AR/2 to AR)` |
| **Parry** | 20% at GM with shield only | Default |
| **Protection Spell** | Fixed +5 AR | Fixed +5 AR |

### Armor
| Decision | Value |
|----------|-------|
| **Exceptional Bonus** | None (cosmetic only) |
| **Plate AR Tiers** | T1=25, T2=30, T3=35, T4=45, T5=55, T6=60, T7=65 |
| **Leather Base AR** | 15 |
| **Chainmail/Ringmail** | Chest tunics only, use plate tiers |
| **Weapon Durability** | Increases with ore tier |

### Shields (Flat AR)
| Shield | Non-GM | GM |
|--------|--------|-----|
| Buckler | 1 | 5 |
| Wooden | 5 | 10 |
| Heater | 7 | 15 |
| Faction (Order/Chaos/TBD) | 10 | 20 |

### Stones (Mining Drops)
| Stone | Effect | Notes |
|-------|--------|-------|
| **Sharpening Stone** | RNG: Force(+6)/Power(+8)/Vanq(+10) | Single item type |
| **Accuracy Stone** | RNG: +5 to +25 Tactics, +2% to +10% hit | Single item type |
| Both | Permanent, one per weapon, can stack together | |

### Talisman Types (4 Types × 3 Tiers Each)
| Type | Build | Abilities Unlocked in PvP |
|------|-------|---------------------------|
| Dexer | Melee | Bushido, Weapon Special Moves |
| Tamer | Pets | Taming bonuses, pet control |
| Sampire | Paladin | Chivalry, Necromancy |
| Treasure Hunter | Explorer | Lockpicking, cartography |

**Note:** Talismans drop to backpack on PvP flag, disabling all gated abilities.

### Removed Items
- **Weapons:** Cutlass, Pike, WarFork, Pitchfork, Leafblade, CompositeBow, WarMace, and others
- **Shields:** Bronze Shield, Metal Shield, Wooden Kite Shield, Metal Kite Shield
- **Armor:** All chainmail/ringmail except chest tunics

### Runic Properties
- **PvE Only** - All runic bonuses disabled in PvP
- **No Regen** - Mana/HP/Stamina regen properties not allowed

---

## Change Log

| Date | Change | Reason |
|------|--------|--------|
| 2025-01-15 | Initial document | PvP combat system design |
| 2025-01-15 | Added ImaginNation formulas | Proven balanced system |
| 2025-01-15 | Separated PvP/PvM | Independent tuning |
| 2025-01-15 | Finalized armor tiers | T1-T7 with custom AR values |
| 2025-01-15 | Added shield flat values | Simpler balance |
| 2025-01-15 | Fixed parry at 20% | Faster combat |
| 2025-01-15 | Corrected sharpening stone | Single item with RNG on use |
| 2025-01-15 | Added Talisman-gated abilities | Prevent "do everything" builds |
| 2025-01-15 | Added leather AR (15) | Base armor value |
| 2025-01-15 | Chainmail/ringmail chest only | Simplify armor system |
| 2025-01-15 | Added accuracy stone system | From ImaginNation reference |
| 2025-01-15 | Created finalized decisions summary | Complete reference |
