# Talisman Tier Design

**Authority**: REFERENCE - Draft for Review
**Purpose**: Design T1/T2/T3 talisman specifications for each build type
**Related**: `UO_BUILD_TEMPLATES.md`, `Progression/TALISMAN_SYSTEM.md`

---

## Design Philosophy

### Core Principles

1. **Talismans UNLOCK skills** - Without a talisman, gated skills cannot be trained or used
2. **Talismans define the 7-8 skill template** - Only these skills are available while wearing the talisman
3. **All other skills are ZEROED** - Non-template skills temporarily become 0 (revert on unequip)
4. **Tiers provide bonuses** - Higher tiers give better bonuses to key skills (T1=+5, T2=+10, T3=+20)
5. **PvP disabled** - Talisman unequips on PvP engagement, skills restore

### Tier Bonus Standard

| Tier | Skill Bonus | Stat Bonus | Acquisition |
|------|-------------|------------|-------------|
| T1 | +5 | +5 | Crafted (Easy) |
| T2 | +10 | +10 | Crafted (Medium) |
| T3 | +20 | +20 | Crafted (Hard) |

**Bonus Application**: Bonuses apply to skills that would benefit from 120+ effective skill. The goal is that a GM player (100 skill) gets the tier bonus on top.

### Tier Acquisition

| Tier | Crafting Skill | Relic Type | Relic Drop Rate |
|------|---------------|------------|-----------------|
| T1 | 65 Tinkering, 65 Blacksmithing | Common Relic | ~5% from dungeon mobs |
| T2 | 80 Tinkering, 80 Blacksmithing | Uncommon Relic | ~1% from dungeon mobs |
| T3 | 100 Tinkering (GM), 100 Blacksmithing (GM) | Rare Relic | ~0.1% from dungeon bosses |

**Duration**: 7-day TIME WORN (only counts while equipped), same for all tiers.

---

## DEXER TALISMAN

### Role Identity
Pure melee warrior with Bushido combat abilities and weapon special moves.

### Complete Skill Template (7 Skills)

When wearing a Dexer Talisman, ONLY these skills are available:

| Skill | Base Points | Purpose | Gets Tier Bonus? |
|-------|-------------|---------|------------------|
| **Bushido** | 100-120 | Honor, Lightning Strike, weapon parry | **YES** |
| Swordsmanship (or Mace/Fencing) | 100-120 | Primary weapon damage | No |
| Tactics | 100-120 | +50-70% base damage | No |
| Anatomy | 100-120 | +55-65% damage, healing bonus | No |
| Healing | 100 | Bandage self-healing | No |
| Parrying | 100-120 | Block chance with weapon | No |
| Resist Spells | 80-100 | Defense vs magic | No |

**Total Skill Points**: 680-820 (within 720 cap with Power Scrolls)

**All Other Skills**: Temporarily zeroed while talisman equipped.

### Gated Skills (Require This Talisman)
- **Bushido** - Cannot train or use without Dexer talisman

### Tier Specifications

#### T1 - Apprentice Samurai

| Property | Value |
|----------|-------|
| Bushido Bonus | +5 |
| Bonus STR | +5 |
| Bonus DEX | +5 |
| Bonus Damage | +5% |
| Special Moves | Primary only |

**Effective Bushido**: 100 trained + 5 = 105 effective

---

#### T2 - Journeyman Samurai

| Property | Value |
|----------|-------|
| Bushido Bonus | +10 |
| Bonus STR | +10 |
| Bonus DEX | +10 |
| Bonus Damage | +10% |
| Special Moves | Primary + Secondary |
| Swing Speed Bonus | +5% |

**Effective Bushido**: 100 trained + 10 = 110 effective

---

#### T3 - Master Samurai (TWO VARIANTS)

**T3-A: Bushido Master** (Sustained Combat)

| Property | Value |
|----------|-------|
| Bushido Bonus | +20 |
| Bonus STR | +20 |
| Bonus DEX | +20 |
| Bonus Damage | +20% |
| Special Moves | All |
| Swing Speed Bonus | +10% |
| Defense Chance Bonus | +5% |

**Effective Bushido**: 100 trained + 20 = 120 effective

---

**T3-B: Ninja Master** (Burst/Stealth Combat)

Different skill template - replaces Bushido with Ninjitsu:

| Skill | Base Points | Purpose | Gets Tier Bonus? |
|-------|-------------|---------|------------------|
| **Ninjitsu** | 100-120 | Stealth attacks, Animal Form | **YES** |
| Swordsmanship (or Fencing) | 100-120 | Primary weapon damage | No |
| Tactics | 100-120 | Base damage | No |
| Hiding | 100 | Enter stealth | No |
| Stealth | 100-120 | Move while hidden | No |
| Parrying | 100 | Defense | No |
| Poisoning | 80-100 | Infectious Strike | No |

| Property | Value |
|----------|-------|
| Ninjitsu Bonus | +20 |
| Bonus DEX | +20 |
| Bonus Damage | +20% (from stealth) |
| Special Moves | All Ninja moves |
| Stealth Bonus | +10 |

---

## SAMPIRE TALISMAN

### Role Identity
Vampiric warrior combining Necromancy life drain with Chivalry damage buffs.

### Complete Skill Template (7 Skills)

When wearing a Sampire Talisman, ONLY these skills are available:

| Skill | Base Points | Purpose | Gets Tier Bonus? |
|-------|-------------|---------|------------------|
| **Necromancy** | 99-120 | Vampiric Embrace (requires 99) | **YES** |
| **Chivalry** | 65-80 | Enemy of One, Consecrate Weapon | **YES** |
| **Bushido** | 100-120 | Honor, Lightning Strike | **YES** |
| Swordsmanship | 115-120 | Primary weapon damage | No |
| Tactics | 100-120 | Base damage multiplier | No |
| Parrying | 115-120 | ~35% block with weapon | No |
| Anatomy | 80-100 | Damage bonus | No |

**Total Skill Points**: 674-780 (within 720 cap with Power Scrolls)

**All Other Skills**: Temporarily zeroed while talisman equipped.

### Gated Skills (Require This Talisman)
- **Necromancy** - Cannot train or use without Sampire talisman
- **Chivalry** - Cannot train or use without Sampire talisman
- **Bushido** - Cannot train or use without Sampire OR Dexer talisman

### Key Ability: Vampiric Embrace (Tiered Effectiveness)

**Design**: All tiers can use Vampiric Embrace, but effectiveness scales with tier.

| Tier | Life Drain % | Fire Weakness | Notes |
|------|-------------|---------------|-------|
| T1 (Initiate) | +5% | +10% | Entry-level sustain |
| T2 (Disciple) | +10% | +15% | Moderate sustain |
| T3 (Master) | +20% | +25% | Full vampiric power |

*Note: Lower tiers have reduced fire weakness as a benefit.*

Standard UO requires 99 Necromancy for Vampiric Embrace. In our system, the talisman UNLOCKS the ability at any tier, but the drain % scales.

### Tier Specifications

#### T1 - Initiate of Shadows

| Property | Value |
|----------|-------|
| Necromancy Bonus | +5 |
| Chivalry Bonus | +5 |
| Bushido Bonus | +5 |
| Bonus STR | +5 |
| Bonus DEX | +5 |
| Life Leech Bonus | +5% |

---

#### T2 - Disciple of Blood

| Property | Value |
|----------|-------|
| Necromancy Bonus | +10 |
| Chivalry Bonus | +10 |
| Bushido Bonus | +10 |
| Bonus STR | +10 |
| Bonus DEX | +10 |
| Life Leech Bonus | +10% |
| Mana Leech Bonus | +5% |

---

#### T3 - Master of Blood

| Property | Value |
|----------|-------|
| Necromancy Bonus | +20 |
| Chivalry Bonus | +20 |
| Bushido Bonus | +20 |
| Bonus STR | +20 |
| Bonus DEX | +20 |
| Life Leech Bonus | +20% |
| Mana Leech Bonus | +10% |
| Damage Increase | +10% |

---

## TAMER TALISMAN

### Role Identity
Pet master who commands creatures to fight while providing support.

### Complete Skill Template (7 Skills)

When wearing a Tamer Talisman, ONLY these skills are available:

| Skill | Base Points | Purpose | Gets Tier Bonus? |
|-------|-------------|---------|------------------|
| **Animal Taming** | 110-120 | Tame stronger creatures | **YES** |
| **Veterinary** | 100-120 | Heal and resurrect pets | **YES** |
| Animal Lore | 110-120 | Control/heal effectiveness | **YES** (from talisman) |
| Magery | 100-110 | Utility spells, travel | No |
| Meditation | 100 | Mana regeneration | No |
| Eval Intelligence | 100 | Spell damage if needed | No |
| Wrestling | 80-100 | Personal defense | No |

**Total Skill Points**: 700-770 (within 720 cap with Power Scrolls)

**All Other Skills**: Temporarily zeroed while talisman equipped.

### Gated Skills (Require This Talisman)
- **Animal Taming** - Cannot train or use without Tamer talisman
- **Veterinary** - Cannot train or use without Tamer talisman

### Key Mechanics

**Pet Resurrection**: Requires 80 Veterinary + 80 Animal Lore
**Poison Cure**: Requires 60 Veterinary + 60 Animal Lore
**Elite Pets** (Greater Dragon, Cu Sidhe): Require 110+ Taming

### Tier Specifications

#### T1 - Beast Apprentice

| Property | Value |
|----------|-------|
| Animal Taming Bonus | +5 |
| Veterinary Bonus | +5 |
| Animal Lore Bonus | +5 |
| Pet Damage Bonus | +5% |
| Control Slots | 4 |

**Effective Taming**: 100 trained + 5 = 105 effective

---

#### T2 - Beast Handler

| Property | Value |
|----------|-------|
| Animal Taming Bonus | +10 |
| Veterinary Bonus | +10 |
| Animal Lore Bonus | +10 |
| Pet Damage Bonus | +10% |
| Pet Healing Bonus | +10% |
| Control Slots | 5 |

**Effective Taming**: 100 trained + 10 = 110 effective

---

#### T3 - Beast Master

| Property | Value |
|----------|-------|
| Animal Taming Bonus | +20 |
| Veterinary Bonus | +20 |
| Animal Lore Bonus | +20 |
| Pet Damage Bonus | +20% |
| Pet Healing Bonus | +20% |
| Control Slots | 5 |
| Bonus INT | +20 |

**Effective Taming**: 100 trained + 20 = 120 effective (can tame anything)

---

## TREASURE HUNTER / THIEF TALISMAN

### Role Identity
Explorer who finds and opens treasure through rogue skills (TH) OR steals from players (Thief).

### Complete Skill Template - Treasure Hunter (7 Skills)

When wearing a TH Talisman, ONLY these skills are available:

| Skill | Base Points | Purpose | Gets Tier Bonus? |
|-------|-------------|---------|------------------|
| **Lockpicking** | 100-120 | Open treasure chests | **YES** |
| **Remove Trap** | 100 | Disarm chest traps | **YES** |
| **Cartography** | 100 | Decode maps, dig radius | **YES** |
| Mining OR Fishing | 100 | Dig up chests / salvage | No |
| Magery | 80-100 | Travel, utility | No |
| Detect Hidden | 80-100 | Find hidden objects | No |
| Stealth | 80-100 | Escape guardians | No |

**Total Skill Points**: 640-720 (within cap)

**All Other Skills**: Temporarily zeroed while talisman equipped.

### Gated Skills (Require TH Talisman)
- **Lockpicking** - Cannot train or use without TH talisman
- **Remove Trap** - Cannot train or use without TH talisman
- **Cartography** - Cannot train or use without TH talisman

### Tier Specifications - Treasure Hunter

#### T1 - Novice Explorer

| Property | Value |
|----------|-------|
| Lockpicking Bonus | +5 |
| Remove Trap Bonus | +5 |
| Cartography Bonus | +5 |
| Bonus DEX | +5 |
| Luck Bonus | +50 |

---

#### T2 - Skilled Explorer

| Property | Value |
|----------|-------|
| Lockpicking Bonus | +10 |
| Remove Trap Bonus | +10 |
| Cartography Bonus | +10 |
| Bonus DEX | +10 |
| Luck Bonus | +100 |
| Mining/Fishing Bonus | +10 |

---

#### T3-A - Master Treasure Hunter

| Property | Value |
|----------|-------|
| Lockpicking Bonus | +20 |
| Remove Trap Bonus | +20 |
| Cartography Bonus | +20 |
| Bonus DEX | +20 |
| Luck Bonus | +200 |
| Mining/Fishing Bonus | +20 |
| Detect Hidden Bonus | +20 |

---

### Complete Skill Template - Thief (T3-B Only)

Different skill template - focused on player theft:

| Skill | Base Points | Purpose | Gets Tier Bonus? |
|-------|-------------|---------|------------------|
| **Stealing** | 100-120 | Steal from players | **YES** |
| **Snooping** | 100-120 | See player backpacks | **YES** |
| Hiding | 100 | Become hidden | No |
| Stealth | 100-120 | Move while hidden | No |
| Lockpicking | 80-100 | Open locked items | No |
| Detect Hidden | 80-100 | Find hidden players | No |
| Wrestling | 80-100 | Escape if caught | No |

### Gated Skills (Require Thief Talisman)
- **Stealing** - Cannot steal items of value without +15 or +20 bonus (T3 required)
- **Snooping** - Can use at any level but high-end requires bonus

#### T3-B - Master Thief

| Property | Value |
|----------|-------|
| Stealing Bonus | +20 |
| Snooping Bonus | +20 |
| Bonus DEX | +20 |
| Stealth Bonus | +20 |

**Note**: Stealing skill is gated such that stealing valuable weapons/items requires +15 or +20 effective skill, forcing use of T3 Thief Talisman for meaningful theft.

**PvP Behavior**: If the thief attacks or is attacked by another player, the talisman drops immediately.

---

## Summary Tables

### Complete Skill Templates by Talisman

| Talisman | Skill 1 | Skill 2 | Skill 3 | Skill 4 | Skill 5 | Skill 6 | Skill 7 |
|----------|---------|---------|---------|---------|---------|---------|---------|
| **Dexer** | Bushido* | Weapon | Tactics | Anatomy | Healing | Parrying | Resist |
| **Dexer (Ninja)** | Ninjitsu* | Weapon | Tactics | Hiding | Stealth | Parrying | Poisoning |
| **Sampire** | Necro* | Chivalry* | Bushido* | Weapon | Tactics | Parrying | Anatomy |
| **Tamer** | Taming* | Vet* | Lore | Magery | Med | Eval Int | Wrestling |
| **TH** | Lockpick* | RemoveTrap* | Carto* | Mining/Fish | Magery | Detect | Stealth |
| **Thief** | Stealing* | Snooping* | Hiding | Stealth | Lockpick | Detect | Wrestling |

*Starred skills = Gated (require talisman) and receive tier bonuses

### Skill Bonuses by Tier

| Talisman | T1 (+5) | T2 (+10) | T3 (+20) |
|----------|---------|----------|----------|
| Dexer | Bushido | Bushido | Bushido |
| Sampire | Necro, Chiv, Bushido | Necro, Chiv, Bushido | Necro, Chiv, Bushido |
| Tamer | Taming, Vet, Lore | Taming, Vet, Lore | Taming, Vet, Lore |
| TH | Lockpick, RemoveTrap, Carto | Lockpick, RemoveTrap, Carto | Lockpick, RemoveTrap, Carto |
| Thief | - | - | Stealing, Snooping |

### Stat Bonuses by Tier

| Talisman | T1 | T2 | T3 |
|----------|-----|-----|-----|
| Dexer | +5 STR, +5 DEX | +10 STR, +10 DEX | +20 STR, +20 DEX |
| Sampire | +5 STR, +5 DEX | +10 STR, +10 DEX | +20 STR, +20 DEX |
| Tamer | +5 Lore | +10 Lore | +20 Lore, +20 INT |
| TH | +5 DEX | +10 DEX | +20 DEX |
| Thief | - | - | +20 DEX |

---

## Tiered Ability Effectiveness

**Design Principle**: All tiers can ACCESS signature abilities, but effectiveness SCALES with tier.

This allows players to experience the build's core fantasy from T1, while T3 provides mastery-level performance.

### DEXER: Honor/Perfection System

| Tier | Max Perfection Bonus | Lightning Strike HCI | Evasion Duration |
|------|---------------------|---------------------|------------------|
| T1 | +25% damage | +15% | 2 seconds |
| T2 | +50% damage | +30% | 4 seconds |
| T3 | +100% damage (full) | +50% (full) | 6 seconds |

### DEXER (NINJA): Stealth Damage

| Tier | Backstab Bonus | Death Strike | Mirror Images |
|------|---------------|--------------|---------------|
| T1 | +10% | ×1.1 | 1 image |
| T2 | +25% | ×1.25 | 2 images |
| T3 | +50% | ×1.5 | 4 images |

### SAMPIRE: Vampiric/Chivalry

| Tier | Life Drain | Consecrate Weapon | Enemy of One |
|------|-----------|-------------------|--------------|
| T1 | +5% | 25% conversion | +15% dmg / +25% taken |
| T2 | +10% | 50% conversion | +30% dmg / +50% taken |
| T3 | +20% | 100% conversion | +50% dmg / +100% taken |

### TAMER: Pet Bonuses

| Tier | Pet Damage | Pet Resists | Heal Bonus | Control Bonus |
|------|-----------|-------------|-----------|---------------|
| T1 | +5% | +5 all | +10% | +5% |
| T2 | +10% | +10 all | +20% | +10% |
| T3 | +20% | +15 all | +35% | +15% |

### TREASURE HUNTER: Rogue Bonuses

| Tier | Luck | Lockpick Success | Dig Radius | Trap Damage Reduction |
|------|------|-----------------|------------|----------------------|
| T1 | +50 | +5% | +0 | 10% |
| T2 | +100 | +10% | +1 tile | 25% |
| T3 | +200 | +20% | +2 tiles | 50% |

### THIEF (T3 Only): Stealing

| Tier | Steal Success | Max Weight | Detection Reduction |
|------|--------------|-----------|---------------------|
| T3 | +20% | +50 stones | -30% |

---

## Always Available Skills

These skills are NEVER gated and work without any talisman:

| Category | Skills |
|----------|--------|
| Combat Basics | Tactics, Anatomy, Healing, Parrying, Wrestling |
| Weapon Skills | Swordsmanship, Mace Fighting, Fencing, Archery |
| Magic Basics | Magery, Eval Intelligence, Meditation, Resist Spells |
| Utility | Mining, Fishing, Hiding, Stealth, Detect Hidden |
| Crafting | Blacksmithing, Tailoring, Tinkering, Carpentry, etc. |
| Other | Spirit Speak, Musicianship, Discordance, Peacemaking, etc. |

**Note**: Spirit Speak is ALWAYS available per design decision.

---

## Questions Resolved

### 1. Bushido Availability
**Answer**: Bushido is GATED behind talismans. Both Dexer and Sampire talismans unlock Bushido.

### 2. Ninjitsu
**Answer**: T3 Dexer has TWO variants - Bushido Master (sustained) and Ninja Master (burst/stealth).

### 3. Spirit Speak
**Answer**: Spirit Speak is ALWAYS available (not gated).

### 4. Vampiric Embrace Access
**Answer**: Tiered effectiveness model - ALL tiers can use Vampiric Embrace, but the life drain % scales:
- T1: +5% life drain (entry-level sustain)
- T2: +10% life drain (moderate sustain)
- T3: +20% life drain (full vampiric power)

This allows T1 Sampires to experience the build's core fantasy from the start, while T3 provides mastery-level effectiveness. Lower tiers also have reduced fire weakness as a trade-off benefit.

### 5. Tamer Balance
**Answer**: Talismans unequip on PvP, tamers lose all taming skills and regain zeroed skills.

### 6. TH vs Thief
**Answer**: T3 has TWO variants - Master Treasure Hunter (PvE) and Master Thief (PvP stealing). Stealing valuable items requires the +20 bonus from T3 Thief.

---

## Implementation Notes

### Skill Zeroing on Equip

```csharp
void OnTalismanEquip(Mobile m, BaseTalisman talisman)
{
    foreach (var skill in m.Skills)
    {
        if (!talisman.IsTemplateSkill(skill.Name))
        {
            // Store original value, display as 0
            skill.StoredValue = skill.Base;
            skill.DisplayValue = 0;
        }
    }
    // Suppress skill change notifications
}
```

### Effective Skill Calculation

```csharp
double GetEffectiveSkill(Mobile m, SkillName skill)
{
    double trained = m.Skills[skill].Base;

    var talisman = m.FindEquipped<BaseTalisman>();
    if (talisman != null && talisman.GetsBonusFor(skill))
    {
        // Add tier bonus: T1=+5, T2=+10, T3=+20
        trained += talisman.GetSkillBonus(skill);
    }

    return Math.Min(trained, 120.0); // Cap at 120 effective
}
```

---

*This document is a draft for review. Final specifications will be added to `Core/DESIGN_DECISIONS.md` after approval.*
