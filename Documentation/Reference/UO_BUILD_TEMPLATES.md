# UO Build Templates Reference

**Authority**: REFERENCE - For Talisman & Gear Design
**Purpose**: Document standard UO builds to design talisman tiers and runic gear interactions
**Related**: `Progression/TALISMAN_SYSTEM.md`, `Reference/RUNIC_PROPERTIES_COMPLETE.md`

---

## Design Goals

1. **Talismans define the role** - Unlock skill access and provide build identity
2. **Gear supports the role** - Runic properties enhance PvE effectiveness
3. **3 Tiers per build** - T1 (starter), T2 (intermediate), T3 (advanced)
4. **PvP Balance** - Talismans disabled in PvP, so PvP uses base 7-skill templates

---

## Build 1: DEXER (Pure Melee Warrior)

### Overview

The Dexer is a traditional melee warrior focusing on weapon skills, defense, and sustained damage output. Unlike the Sampire, Dexers don't rely on magic schools - they're pure physical combatants.

### Standard Dexer Skills (OSI 7-Skill Template)

| Skill | Points | Purpose |
|-------|--------|---------|
| Weapon Skill | 100-120 | Swords/Mace/Fencing - Primary damage |
| Tactics | 100-120 | +50% base damage at 100, +70% at 120 |
| Anatomy | 100-120 | +55% damage at 100, +65% at 120, healing bonus |
| Healing | 100 | Bandage self-healing |
| Parrying | 100-120 | Block chance with shield/weapon |
| Bushido | 80-120 | Weapon parrying, special moves, Honor system |
| Resist Spells | 80-100 | Defense vs magic |

**Alternative Skills:**
- Ninjitsu (instead of Bushido) for stealth-based combat
- Chivalry (light version) for self-buffs

### Stat Distribution

| Stat | Value | Reasoning |
|------|-------|-----------|
| STR | 100-125 | Primary damage stat |
| DEX | 80-125 | Swing speed, parry effectiveness |
| INT | 10-25 | Minimal, just for bandage/ability mana |

**Total**: 225 (base) + buffs

### Core Abilities (Bushido)

| Ability | Bushido Req | Mana | Effect |
|---------|-------------|------|--------|
| Honorable Execution | 25 | 0 | Finishing move, bonus vs low HP |
| Counter Attack | 40 | 5 | Auto-counter on block |
| Confidence | 25 | 10 | HP regen while active |
| Evasion | 60 | 10 | Dodge direct spell damage |
| Lightning Strike | 50 | 10 | +50% hit chance, critical damage |
| Momentum Strike | 70 | 10 | AOE damage on kill |

### Weapon Special Moves (Require Tactics + Weapon Skill)

**Primary Moves** (30 Tactics + 70 Weapon):
- Armor Ignore, Bleed Attack, Crushing Blow, Disarm, etc.

**Secondary Moves** (60 Tactics + 90 Weapon):
- Mortal Strike, Paralyzing Blow, Whirlwind Attack, etc.

**Samurai Empire Moves** (Require Bushido/Ninjitsu):
- Armor Pierce, Block, Defense Mastery, Dual Wield, Feint, Frenzied Whirlwind, Nerve Strike, Riding Swipe, Talon Strike

### Gear Priorities

| Slot | Priority Properties |
|------|---------------------|
| Weapon | Hit Chance Increase, Damage Increase, Swing Speed Increase, Hit Effects |
| Armor | Physical Resist 70+, Defense Chance Increase, Hit Point Increase |
| Jewelry | STR/DEX bonus, Hit Chance, Damage Increase |

### Key Runic Properties for Dexers (PvE)

**High Value:**
- WeaponDamage (+1-50%)
- AttackChance (+1-15%)
- DefendChance (+1-15%)
- WeaponSpeed (+5-30%)
- HitLeechHits (lifesteal)
- HitLeechStam (stamina sustain)

**Medium Value:**
- BonusStr, BonusDex
- RegenHits, RegenStam
- Slayers (vs specific monsters)

---

## Build 2: SAMPIRE (Samurai + Vampire Hybrid)

### Overview

The Sampire combines Bushido (Samurai) abilities with Necromancy's Vampiric Embrace to create a self-sustaining melee fighter. The 20% life drain on every hit plus Honor bonuses makes this the premier PvE solo farming build.

### Standard Sampire Skills (OSI 7-Skill Template)

| Skill | Points | Purpose |
|-------|--------|---------|
| Weapon Skill | 115-120 | Primary damage (Swords preferred) |
| Tactics | 100-120 | Base damage multiplier |
| Parrying | 115-120 | ~35% block chance with weapon |
| Bushido | 100-120 | Honor, Lightning Strike, weapon parry |
| Necromancy | 99+ | Vampiric Embrace (requires 99) |
| Chivalry | 65-75 | Enemy of One, Consecrate Weapon |
| Anatomy OR Resist | 80-100 | Damage bonus OR magic defense |

**Critical**: Must have exactly 99+ Necromancy (after item bonuses) to maintain Vampiric Embrace.

### Stat Distribution

| Stat | Value | Reasoning |
|------|-------|-----------|
| STR | 100-125 | Primary damage |
| DEX | 125 | Maximum swing speed |
| INT | 10-30 | Minimal (30 with Elf race) |

### Core Mechanics

**Vampiric Embrace** (Necromancy 99+):
- Transform into vampire form
- **20% life drain on every successful hit**
- Immune to lower-level poisons
- Fire weakness increased

**Honor System** (Bushido 50+):
- Activate on target before combat
- Build "Perfection" meter on consecutive hits
- Up to 100% bonus damage at max Perfection
- Resets if you miss or switch targets

**Consecrate Weapon** (Chivalry 15+):
- Weapon damage converts to target's weakest resistance
- Essential for fighting high-resist monsters

**Enemy of One** (Chivalry 45+):
- +50% damage vs one creature type
- Warning: Enemies deal double damage to you

### Gear Priorities

| Slot | Priority Properties |
|------|---------------------|
| Weapon | Fast swing (1.25s), Hit Mana Leech, Hit Stam Leech, Damage Increase |
| Armor | 70+ all resists, Damage Increase, Defense Chance Increase |
| Jewelry | STR/DEX bonus, Damage Increase, Lower Mana Cost |

**Target Stats:**
- 100% Damage Increase (total across all gear)
- All resistances 70+
- Defense Chance Increase: <20% OR >45%

### Key Runic Properties for Sampires (PvE)

**Critical:**
- HitLeechMana (sustain Lightning Strike spam)
- HitLeechStam (swing speed maintenance)
- WeaponDamage
- WeaponSpeed (reach 1.25s cap)

**High Value:**
- All Hit effects (synergize with high hit rate)
- Slayers
- ElementalDamage (for Consecrate Weapon variety)

---

## Build 3: TAMER (Pet-Based Combat)

### Overview

Tamers use controlled pets to deal damage while supporting from safety. The build requires heavy skill investment in pet-related skills, leaving limited room for personal combat abilities.

### Standard Tamer Skills (OSI 7-Skill Template)

| Skill | Points | Purpose |
|-------|--------|---------|
| Animal Taming | 110-120 | Tame stronger creatures |
| Animal Lore | 110-120 | Control/heal effectiveness |
| Veterinary | 100-120 | Heal and resurrect pets |
| Magery | 100-110 | Utility spells, travel, support |
| Meditation | 100 | Mana regeneration |
| Eval Intelligence | 100 | Spell damage (if offensive mage) |
| Wrestling OR Resist | 80-100 | Personal defense |

**Variations:**
- **Battle Tamer**: Add combat skills, lower Magery
- **Pure Tamer**: Max taming skills, support magic only
- **Disco Tamer**: Add Discordance for pet damage buff

### Stat Distribution

| Stat | Value | Reasoning |
|------|-------|-----------|
| STR | 90-100 | Survival |
| DEX | 25-40 | Minimal |
| INT | 100-125 | Mana pool for healing/support |

### Pet Progression by Taming Skill

| Skill | Tameable Creatures |
|-------|-------------------|
| 30-45 | Cows, Sheep |
| 45-60 | Hinds, Timber Wolves |
| 60-75 | Walrus, Polar Bears |
| 75-85 | Snow Leopards, Panthers |
| 85-100 | Great Harts, White Wolves |
| 100-110 | Ridgebacks, Bulls |
| 110+ | Greater Dragons, Cu Sidhe, etc. |

### Core Mechanics

**Taming Attempt:**
- Stand within 3 tiles of target
- Success based on Taming + Lore vs creature difficulty
- Can move up to 10 tiles once attempt begins

**Pet Control:**
- Control chance = (Taming + Lore) vs creature requirement
- Failed control = pet goes wild
- Stronger pets need higher combined skills

**Veterinary Healing:**
- Heal amount scales with Vet + Lore
- Fixed 2-second bandage timer (DEX irrelevant)
- 60/60 Vet/Lore to cure poison
- 80/80 Vet/Lore to resurrect bonded pets

### Gear Priorities

| Slot | Priority Properties |
|------|---------------------|
| Armor | Max resists (personal survival) |
| Jewelry | Skill bonuses (Taming, Lore, Vet), INT bonus |
| Weapon | Not combat-focused, maybe Mage Weapon |

**Important**: Avoid Reflect Physical Damage - it interrupts taming attempts!

### Key Runic Properties for Tamers (PvE)

**High Value:**
- BonusInt, BonusMana (mana pool)
- RegenMana (sustain healing)
- LowerManaCost (efficient spellcasting)
- All Resistances (survival)

**Skill Bonuses (if available):**
- Animal Taming +
- Animal Lore +
- Veterinary +

---

## Build 4: TREASURE HUNTER (Thief/Explorer Hybrid)

### Overview

The Treasure Hunter is a utility build focused on finding, decoding, and looting treasure maps and underwater salvage. It combines rogue skills (Lockpicking, Remove Trap) with exploration skills (Cartography, Mining/Fishing).

### Standard TH Skills (OSI 7-Skill Template)

| Skill | Points | Purpose |
|-------|--------|---------|
| Lockpicking | 100-120 | Open treasure chests |
| Remove Trap | 100 | Disarm chest traps safely |
| Cartography | 100 | Decode maps, increase dig radius |
| Mining OR Fishing | 100 | Dig up chests OR salvage MIBs |
| Magery | 80-100 | Travel, utility, emergency |
| Detect Hidden | 80-100 | Find hidden traps/objects |
| Stealth OR Combat | 80-100 | Escape or fight guardians |

**Variations:**
- **Land TH**: Mining focus, dungeon chests
- **Sea TH**: Fishing focus, Messages in Bottles, Ancient SOS
- **Combat TH**: Add weapon skill to fight guardians solo

### Stat Distribution

| Stat | Value | Reasoning |
|------|-------|-----------|
| STR | 90-100 | Carry capacity for loot |
| DEX | 80-100 | Lockpicking/trap success, stealth |
| INT | 40-60 | Magery for travel/utility |

### Core Mechanics

**Treasure Map Progression:**

| Map Level | Cartography to Decode | Guardian Difficulty |
|-----------|----------------------|---------------------|
| Level 1 | 0 | Easy |
| Level 2 | 41 | Moderate |
| Level 3 | 51 | Challenging |
| Level 4 | 61 | Hard |
| Level 5 | 71 | Very Hard |
| Level 6 | 81 | Elite |

**Dig Radius by Cartography:**
| Skill | Radius |
|-------|--------|
| 0-50 | 1 tile (precise) |
| 51-80 | 2 tiles |
| 81-99 | 3 tiles |
| 100+ | 4 tiles |

**Lockpicking Progression:**
- 30-55: Pick 30-skill tinker boxes
- 55-95: Pick 100-skill tinker boxes
- 95-120: Level 4+ treasure chests

**Remove Trap:**
- Must disarm trap BEFORE picking lock
- Failed disarm can trigger trap (spawns guardians, deals damage)
- No longer requires Detect Hidden (as of Publish 105)

### Fishing-Based TH (Messages in Bottles)

| Item | Fishing Skill | Contents |
|------|---------------|----------|
| MIB (basic) | 75 | SOS coordinates |
| Ancient MIB | 75 (1/25 chance) | Level 4 salvage, Leviathan chance |

**Special Catches:**
- Prized Fish: +8-12% INT for 150 seconds
- Truly Rare Fish: +8-12% STR for 150 seconds
- Wondrous Fish: +8-12% DEX for 150 seconds
- White Pearls: Imbuing ingredient
- Delicate Scales: Imbuing ingredient

### Gear Priorities

| Slot | Priority Properties |
|------|---------------------|
| Armor | Stealth-compatible (leather), resists |
| Jewelry | Skill bonuses (Lockpicking, Cartography), LRC |
| Tools | Durable lockpicks, shovels |

### Key Runic Properties for THs (PvE)

**High Value:**
- Luck (better loot quality)
- LowerManaCost (utility spells)
- All Resistances (guardian fights)
- BonusDex (lockpicking/trap)

**Skill Bonuses (if available):**
- Lockpicking +
- Cartography +
- Mining + (for land TH)

---

## Skill Summary by Build

### Skills GATED by Talisman (Require Talisman to Use)

| Build | Gated Skills | Reasoning |
|-------|--------------|-----------|
| **Dexer** | Bushido, Weapon Special Moves (SE+) | Samurai abilities |
| **Sampire** | Chivalry, Necromancy | Paladin + Death magic |
| **Tamer** | Animal Taming, Veterinary | Pet control |
| **Treasure Hunter** | Lockpicking, Remove Trap, Cartography | Rogue skills |

### Skills ALWAYS Available (No Talisman Required)

| Category | Skills |
|----------|--------|
| Combat Basics | Tactics, Anatomy, Healing, Parrying, Weapon Skills |
| Magic Basics | Magery, Eval Int, Meditation, Resist Spells |
| Utility | Mining, Fishing, Stealth, Hiding, Detect Hidden |

---

## Gear vs Talisman Responsibility

### What Talismans Should Provide

| Effect | Provided By |
|--------|-------------|
| Skill Access | Talisman UNLOCKS the skill |
| Build Identity | Talisman DEFINES the role |
| Stat Boosts | Talisman may provide base bonuses |
| Ability Gating | Only talisman wearers can use gated abilities |

### What Gear Should Provide

| Effect | Provided By |
|--------|-------------|
| Damage Enhancement | Runic weapon properties |
| Defense Enhancement | Runic armor properties |
| Sustain | Hit Leech effects, Regen |
| Utility | LMC, LRC, Luck |
| Resistances | Armor base + runic bonuses |

### Division Principle

> **Talisman = "What can I do?"**
> **Gear = "How well do I do it?"**

A Dexer without gear can still use Bushido abilities.
A non-Dexer with perfect Bushido gear still can't use Bushido abilities.

---

## Runic Property Value by Build

### Property Priority Matrix (PvE Only)

| Property | Dexer | Sampire | Tamer | TH |
|----------|-------|---------|-------|-----|
| WeaponDamage | HIGH | HIGH | LOW | MED |
| AttackChance | HIGH | HIGH | LOW | MED |
| DefendChance | HIGH | HIGH | MED | MED |
| WeaponSpeed | HIGH | CRITICAL | LOW | LOW |
| HitLeechHits | HIGH | CRITICAL | LOW | LOW |
| HitLeechMana | MED | CRITICAL | MED | MED |
| HitLeechStam | HIGH | CRITICAL | LOW | LOW |
| Slayers | HIGH | HIGH | LOW | MED |
| BonusStr | HIGH | HIGH | MED | MED |
| BonusDex | HIGH | HIGH | LOW | HIGH |
| BonusInt | LOW | LOW | HIGH | MED |
| BonusMana | LOW | LOW | HIGH | MED |
| RegenMana | LOW | LOW | HIGH | MED |
| All Resists | HIGH | HIGH | HIGH | HIGH |
| Luck | MED | MED | LOW | HIGH |
| LowerManaCost | LOW | MED | HIGH | HIGH |
| Skill Bonuses | - | - | HIGH | HIGH |

---

## Tiered Ability Effectiveness (Sphere 51a Custom)

This section documents how signature abilities scale with talisman tiers in our custom system. All tiers can USE abilities, but effectiveness scales with tier.

### Design Principle

> **All tiers get ACCESS to signature abilities**
> **Effectiveness SCALES with tier (+5% T1, +10% T2, +20% T3)**

This allows players to experience the build's core fantasy from T1, while T3 provides mastery-level performance.

---

### DEXER: Tiered Bushido Effectiveness

#### Signature Ability: Honor/Perfection System

In standard UO, Perfection builds up to 100% bonus damage through consecutive hits. Our tier system modifies the maximum Perfection bonus:

| Tier | Max Perfection Bonus | Effective Damage Ceiling |
|------|---------------------|-------------------------|
| T1 (Apprentice) | +25% | 1.25x damage at max |
| T2 (Journeyman) | +50% | 1.5x damage at max |
| T3 (Master) | +100% | 2.0x damage at max (full) |

#### Lightning Strike Scaling

Lightning Strike normally provides +50% hit chance. Our tier system:

| Tier | Hit Chance Bonus | Critical Hit Chance |
|------|-----------------|---------------------|
| T1 | +15% | 5% |
| T2 | +30% | 10% |
| T3 | +50% | 20% |

#### Evasion Scaling

Evasion provides spell dodge. Duration and effectiveness scale:

| Tier | Base Duration | Dodge Chance |
|------|--------------|--------------|
| T1 | 2 seconds | 15% |
| T2 | 4 seconds | 30% |
| T3 | 6 seconds | 50% |

#### Whirlwind/Momentum Strike AOE Scaling

| Tier | AOE Bonus Damage Cap | Max Targets |
|------|---------------------|-------------|
| T1 | +25 | 3 |
| T2 | +50 | 5 |
| T3 | +100 | 8 |

---

### DEXER (NINJA VARIANT): Tiered Ninjitsu Effectiveness

#### Signature Ability: Backstab/Death Strike

Ninjitsu damage abilities scale with tier:

| Tier | Backstab Damage Bonus | Death Strike Damage |
|------|----------------------|---------------------|
| T1 | +10% | Base × 1.1 |
| T2 | +25% | Base × 1.25 |
| T3 | +50% | Base × 1.5 |

#### Mirror Image Scaling

| Tier | Max Images | Image Duration |
|------|-----------|----------------|
| T1 | 1 | 30 seconds |
| T2 | 2 | 45 seconds |
| T3 | 4 | 60 seconds |

#### Shadow Jump Distance

| Tier | Max Jump Distance |
|------|------------------|
| T1 | 5 tiles |
| T2 | 10 tiles |
| T3 | 15 tiles |

---

### SAMPIRE: Tiered Vampiric/Chivalry Effectiveness

#### Signature Ability: Vampiric Embrace Life Drain

In standard UO, Vampiric Embrace drains 20% of damage dealt as HP. Our tier system:

| Tier | Life Drain % | Fire Weakness |
|------|-------------|---------------|
| T1 (Initiate) | +5% | +10% |
| T2 (Acolyte) | +10% | +15% |
| T3 (Master) | +20% | +25% (full) |

*Note: Lower tiers have reduced fire weakness as a benefit.*

#### Consecrate Weapon Scaling

Converts weapon damage to target's weakest resist:

| Tier | Conversion % | Duration |
|------|-------------|----------|
| T1 | 25% | 5 seconds |
| T2 | 50% | 10 seconds |
| T3 | 100% | 15 seconds |

#### Enemy of One Scaling

| Tier | Damage Bonus vs Target Type | Incoming Damage Increase |
|------|---------------------------|-------------------------|
| T1 | +15% | +25% |
| T2 | +30% | +50% |
| T3 | +50% | +100% (full penalty) |

*Note: Lower tiers have reduced incoming damage penalty.*

#### Curse Weapon (Pre-VE Alternative)

Available to all Sampire tiers as a mini life drain:

| Tier | Duration | Drain % |
|------|----------|---------|
| T1 | 10 seconds | +5% (stacks with VE) |
| T2 | 15 seconds | +8% |
| T3 | 20 seconds | +10% |

---

### TAMER: Tiered Pet Effectiveness

#### Signature Ability: Pet Damage Bonus

Talisman provides flat bonus to all pet damage:

| Tier | Pet Damage Bonus | Pet Resist Bonus |
|------|-----------------|------------------|
| T1 (Handler) | +5% | +5 all resists |
| T2 (Beastmaster) | +10% | +10 all resists |
| T3 (Legendary Tamer) | +20% | +15 all resists |

#### Control Chance Scaling

Affects ability to control difficult pets:

| Tier | Control Chance Bonus | Max Pet Slots |
|------|---------------------|---------------|
| T1 | +5% | 4 |
| T2 | +10% | 5 |
| T3 | +15% | 5 |

#### Veterinary Healing Effectiveness

| Tier | Heal Amount Bonus | Cure Success Bonus | Resurrect Success Bonus |
|------|------------------|-------------------|------------------------|
| T1 | +10% | +5% | +5% |
| T2 | +20% | +10% | +10% |
| T3 | +35% | +20% | +20% |

#### Tameable Creature Access

Higher tiers can tame stronger creatures at lower skill:

| Tier | Effective Taming Bonus | Unlocks |
|------|----------------------|---------|
| T1 | +5 | Basic pets (wolves, bears) |
| T2 | +10 | Advanced pets (drakes, nightmares) |
| T3 | +20 | Elite pets (greater dragons, Cu Sidhe) |

---

### TREASURE HUNTER: Tiered Rogue Effectiveness

#### Signature Ability: Luck Bonus

TH talismans provide stacking luck for better loot:

| Tier | Luck Bonus | Gold Find Bonus |
|------|-----------|-----------------|
| T1 (Apprentice) | +50 | +5% |
| T2 (Journeyman) | +100 | +10% |
| T3 (Master) | +200 | +20% |

#### Lockpicking Success Rate

| Tier | Success Bonus | Lockpick Durability |
|------|--------------|---------------------|
| T1 | +5% | Normal |
| T2 | +10% | +25% durability |
| T3 | +20% | +50% durability |

#### Cartography Dig Radius Bonus

Stacks with base Cartography skill:

| Tier | Bonus Radius | Map Decode Bonus |
|------|-------------|------------------|
| T1 | +0 | Can decode L1-L3 |
| T2 | +1 tile | Can decode L1-L5 |
| T3 | +2 tiles | Can decode all levels |

#### Remove Trap Success Scaling

| Tier | Success Bonus | Trap Damage Reduction (on fail) |
|------|--------------|-------------------------------|
| T1 | +5% | 10% |
| T2 | +10% | 25% |
| T3 | +20% | 50% |

---

### THIEF (T3 Variant): Tiered Stealing Effectiveness

#### Signature Ability: Stealing Success

| Tier | Steal Success Bonus | Max Steal Weight |
|------|---------------------|------------------|
| T3 (Master Thief) | +20% | +50 stones |

#### Snooping Detection Avoidance

| Tier | Detection Chance Reduction |
|------|---------------------------|
| T3 | -30% |

*Note: Thief is a T3-only specialization branching from Treasure Hunter.*

---

### Tier Comparison Summary

| Build | T1 Signature Effect | T2 Signature Effect | T3 Signature Effect |
|-------|--------------------|--------------------|---------------------|
| **Dexer** | 25% max Perfection | 50% max Perfection | 100% max Perfection |
| **Ninja** | +10% Backstab | +25% Backstab | +50% Backstab |
| **Sampire** | +5% Life Drain | +10% Life Drain | +20% Life Drain |
| **Tamer** | +5% Pet Damage | +10% Pet Damage | +20% Pet Damage |
| **TH** | +50 Luck | +100 Luck | +200 Luck |

---

## Next Steps: Talisman Tier Design

See `TALISMAN_TIER_DESIGN.md` for:
- T1/T2/T3 bonuses per build
- Skill unlock thresholds
- Stat allocation per tier
- Acquisition methods

---

## Sources

- UOGuide.com (Sampire, Bushido, Chivalry, Necromancy, etc.)
- UO community build guides
- OSI template standards

*This document serves as reference for designing our custom talisman system.*
