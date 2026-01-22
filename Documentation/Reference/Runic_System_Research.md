# Runic System Research & Design Document

## How Runic Tools Work

### Runic Tool Types & Tiers

| Tool Type | Material Tiers | Source |
|-----------|---------------|--------|
| Runic Hammer (Blacksmith) | Dull Copper → Valorite (8 tiers) | Blacksmith BODs |
| Runic Sewing Kit (Tailor) | Spined → Barbed (3 tiers) | Tailor BODs |
| Runic Dovetail Saw (Carpentry) | Oak → Heartwood (4 tiers) | Heartwood Quests |
| Runic Fletcher Tool | Oak → Heartwood (4 tiers) | Fletching BODs |

### Runic Hammer Stats (Blacksmith)

| Ore | Min Props | Max Props | Min Intensity | Max Intensity | Charges |
|-----|-----------|-----------|---------------|---------------|---------|
| Dull Copper | 1 | 2 | 40% | 100% | 50 |
| Shadow Iron | 2 | 2 | 45% | 100% | 45 |
| Copper | 2 | 3 | 50% | 100% | 40 |
| Bronze | 3 | 3 | 55% | 100% | 35 |
| Golden | 3 | 4 | 60% | 100% | 30 |
| Agapite | 4 | 4 | 65% | 100% | 25 |
| Verite | 4 | 5 | 70% | 100% | 20 |
| **Valorite** | **5** | **5** | **85%** | **100%** | 15 |

### Runic Sewing Kit Stats (Tailor)

| Leather | Min Props | Max Props | Min Intensity | Max Intensity | Charges |
|---------|-----------|-----------|---------------|---------------|---------|
| Spined | 1 | 3 | 40% | 100% | 45 |
| Horned | 3 | 4 | 45% | 100% | 30 |
| Barbed | 4 | 5 | 50% | 100% | 15 |

### Runic Woodworking Stats (Carpentry/Fletching)

| Wood | Min Props | Max Props | Min Intensity | Max Intensity | Charges |
|------|-----------|-----------|---------------|---------------|---------|
| Oak | 1 | 2 | 1% | 50% | 45 |
| Ash | 2 | 3 | 35% | 75% | 35 |
| Yew | 3 | 3 | 40% | 90% | 25 |
| Heartwood | 4 | 4 | 50% | 100% | 15 |

---

## Properties Runic Tools Can Add

### Weapons (25 property slots)

- Hit Area Effects (Physical/Fire/Cold/Poison/Energy) - 2-50%
- Hit Spell Effects (Magic Arrow/Harm/Fireball/Lightning) - 2-50%
- Use Best Skill / Mage Weapon
- Weapon Damage +1-50%
- Defense Chance Increase 1-15%
- Hit Chance Increase 1-15%
- Swing Speed Increase 5-30%
- Spell Channeling (+1 FC penalty)
- Hit Leech (Hits/Mana/Stamina) - 2-50%
- Hit Lower Attack/Defense - 2-50%
- Resistances 1-15%
- Slayer (random)
- Elemental Damage Conversion

### Armor (20 property slots)

- Spell Channeling, Defense Chance 1-15%
- Lower Stat Req, Self Repair, Durability Bonus
- Mage Armor, Night Sight
- Regen Hits/Stam/Mana (1-2/1-3/1-2)
- Bonus Hits/Stam/Mana (1-5/1-8/1-8)
- Lower Mana Cost 1-8%, Lower Reg Cost 1-20%
- Luck 1-100, Reflect Physical 1-15%
- Elemental Resistances 1-15%

### Jewelry (24 property slots)

- All Elemental Resistances 1-15%
- Weapon Damage 1-25%, Defense/Attack Chance 1-15%
- Bonus Str/Dex/Int 1-8
- Enhance Potions 5-25%
- Cast Speed 1, Cast Recovery 1-3
- LMC 1-8%, LRC 1-20%, Luck 1-100
- Spell Damage 1-12%, Night Sight
- Skill Bonuses (up to 5 skills) 1-15

### Spellbooks (16 property slots)

- Bonus Int 1-8 (weighted 4x)
- Bonus Mana 1-8
- Cast Speed 1, Cast Recovery 1-3
- Spell Damage 1-12%
- Skill Bonuses (Magery, Meditation, Eval Int, Magic Resist) 1-15
- LRC 1-20%, LMC 1-8%
- Mana Regen 1-2
- Slayer (random)

---

## Item Property Caps (Official UO Reference)

| Property | Cap | Notes |
|----------|-----|-------|
| HCI (Hit Chance Increase) | 45% | Overcapping helps vs Hit Lower Attack |
| DCI (Defense Chance Increase) | 45% | 70% with armor refinements |
| LRC (Lower Reagent Cost) | 100% | |
| LMC (Lower Mana Cost) | 40% | 55% with inherent armor bonus |
| FC (Faster Casting) | 2 | 6 for Chivalry, 4 for Spellweaving |
| FCR (Faster Cast Recovery) | 6 | 8 max from items |
| SSI (Swing Speed Increase) | 60% | Or 1.25s weapon speed |
| SDI (Spell Damage Increase) | No PvM cap | 15% PvP cap |
| DI (Damage Increase) | 100% items | 300% total with slayers |

---

## Talisman Builds & Gear Requirements

### Talisman-Unique Properties (Official UO)

- Slayer bonuses (Bat, Bear, Beetle, Bird, Bovine, Flame, Ice, Mage, Vermin, Wolf) - +100% damage
- Killer bonuses (1-100% damage vs creature types) - stacks with slayers
- Protection bonuses (0-60% damage reduction)
- Crafting bonuses (1-30% success boost)
- Status removal (Curse, Damage, Ward, Wildfire)

### Key Build Types

#### 1. Sampire/Dexxer (Melee DPS)

**Must Have Stats:**
- 45% HCI (absolute minimum 40%)
- 35%+ SSI from gear
- 180+ Stamina
- 40% LMC
- 70 all resists (capped)

**DCI Strategy:**
- Option A: 45% DCI for defense
- Option B: 20% DCI to maximize Bushido blocking bonuses

**Essential Properties:**
- Hit Life Leech (sustainability)
- Weapon Damage Increase
- Stamina management

#### 2. Mage/Caster

**Must Have Stats:**
- 2 FC / 6 FCR (caps)
- 100% LRC
- 40% LMC
- 70 all resists

**Key Properties:**
- SDI (no PvM cap - stack heavily)
- Mana Regen 30+
- Medable armor OR Mage Armor property

#### 3. Tamer

**Key Stats:**
- 70 all resists
- LMC/LRC for healing
- Skill bonuses (Animal Taming, Animal Lore, Veterinary)

---

## 51Alpha Custom Design: Talisman Synergy

### Core Concept

**Talismans provide:**
- Skill unlocks (certain skills require talisman to use)
- Skill bonuses for unlocked abilities
- Build-defining special effects

**Slayer/Killer properties come from:**
- Special crafted weapons (NOT talismans)
- Requires high-tier runic tools + special materials

**Runic crafted gear provides:**
- The stat infrastructure (HCI/DCI/SSI/LMC/LRC/Resists)
- The foundation that talisman builds need to function
- Combat sustain properties (Hit Leech, Regen, etc.)

### Magical Loot Deprecation

**Magical loot from monsters becomes:**
- Starter/early game equipment
- Gold farming (NPC vendor sell)
- Crafting material salvage

**Magical loot restrictions:**
- 1-2 properties maximum
- 30-50% intensity cap (starter quality)
- No high-tier properties (slayers, hit effects)

---

## Build-Specific Weapon Research

### SAMPIRE Weapons

**Primary Weapons (Swordsmanship):**

| Weapon | Speed | Special Moves | Why Use It |
|--------|-------|---------------|------------|
| **Double Axe** | 3.25s (slow) | Double Strike / Whirlwind | Highest damage, best for bosses with Double Strike |
| **Radiant Scimitar** | 2.50s (fast) | Whirlwind / Bladeweave | Easy to craft, one-handed (can drink potions), fast swing |
| **Daisho** | 2.75s | Double Strike / Feint | Good balance, Double Strike for bosses |
| **Ornate Axe** | 3.50s | Disarm / Crushing Blow | High damage, good for single targets |

**Critical Weapon Properties for Sampire:**
1. **Hit Mana Leech** (MUST HAVE) - Sustain Lightning Strike spam
2. **Hit Stamina Leech** (MUST HAVE) - Maintain swing speed
3. **Swing Speed Increase** (Critical for Double Axe)
4. **Slayer** (when fighting specific monster types)
5. **Damage Increase**
6. **Hit Area** (for Whirlwind farming)

**Weapon Selection Logic:**
- Double Axe WITH SSI > Radiant Scimitar (more damage)
- Double Axe WITHOUT SSI < Radiant Scimitar (radiant's base speed wins)
- Radiant is easier to craft (only needs 2 good mods)
- Double Axe needs SSI + Mana Leech minimum to be effective

---

### DEXER Weapons

**Swordsmanship (Most Popular):**

| Weapon | Speed | Special Moves | Why Use It |
|--------|-------|---------------|------------|
| **Katana** | 2.50s | Double Strike / Armor Ignore | Fastest sword, best DPS over time |
| **Broadsword** | 3.25s | Crushing Blow / Armor Ignore | Cheap, good damage, Armor Ignore |
| **Longsword** | 3.50s | Concussion Blow / Armor Ignore | Higher damage than Broadsword |
| **Halberd** | 4.25s | Whirlwind / Concussion Blow | Hardest hitting sword weapon |
| **Viking Sword** | 3.25s | Crushing Blow / Paralyze | Good damage, Paralyze utility |

**Macing:**

| Weapon | Speed | Special Moves | Why Use It |
|--------|-------|---------------|------------|
| **War Hammer** | 3.75s | Whirlwind / Crushing Blow | High damage, popular PvM choice |
| **War Axe** | 3.00s | Crushing Blow / Armor Ignore | Fast for macing, Armor Ignore |
| **Maul** | 3.50s | Crushing Blow / Double Strike | Double Strike for bosses |
| **Hammer Pick** | 3.25s | Armor Ignore / Mortal Strike | Good PvP utility |

**Fencing:**

| Weapon | Speed | Special Moves | Why Use It |
|--------|-------|---------------|------------|
| **Kryss** | 2.00s | Armor Ignore / Infectious Strike | Fastest weapon, poison build |
| **Spear** | 2.75s | Armor Ignore / Paralyzing Blow | Reach weapon, good damage |
| **Leafblade** | 2.75s | Feint / Armor Ignore | SE weapon, good specials |
| **War Fork** | 2.50s | Bleed Attack / Disarm | Fast, utility moves |

**Recommended for Pure Dexer:**
- **PvM General:** Katana (speed) or Broadsword (damage/availability)
- **PvM Bosses:** Weapon with Armor Ignore
- **PvM AOE:** Weapon with Whirlwind (needs Bushido)

---

### ARCHER Weapons

| Weapon | Speed | Range | Special Moves | Why Use It |
|--------|-------|-------|---------------|------------|
| **Heavy Crossbow** | 5.00s | 8 | Moving Shot / Dismount | Highest raw damage |
| **Crossbow** | 4.00s | 8 | Concussion Blow / Mortal Strike | Good damage, Mortal Strike |
| **Composite Bow** | 4.00s | 10 | Armor Ignore / Moving Shot | Best special (AI), long range |
| **Yumi** | 3.25s | 10 | Armor Ignore / Double Shot | Fast, Double Shot for bosses |
| **Magical Shortbow** | 2.00s | 10 | Lightning Arrow / Psychic Attack | Fastest, no ammo (Lightning Arrow) |
| **Repeating Crossbow** | 2.75s | 8 | Double Strike / Moving Shot | Fast crossbow, Double Strike |
| **Elven Composite Longbow** | 4.00s | 10 | Serpent Arrow / Force Arrow | ONLY bow for poisoning |

**Recommended:**
- **PvM Single Target:** Composite Bow (Armor Ignore spam)
- **PvM General:** Yumi or Magical Shortbow
- **PvM Boss:** Heavy Crossbow (raw damage)
- **Poison Build:** Elven Composite Longbow (unique)

---

### Special Move Priority by Build

| Build | Priority Special Moves | Why |
|-------|----------------------|-----|
| **Sampire** | Whirlwind, Double Strike | AOE farming, boss damage stacking |
| **Dexer (Bushido)** | Whirlwind, Lightning Strike | Bushido bonus damage on Whirlwind |
| **Dexer (General)** | Armor Ignore, Crushing Blow | Single target damage |
| **Archer** | Armor Ignore, Moving Shot | Kiting + burst damage |
| **Ninja** | Armor Ignore, Shadow Strike | Burst from stealth |

---

### Weapon Property Priority by Build

| Property | Sampire | Dexer | Archer | Ninja |
|----------|---------|-------|--------|-------|
| Hit Mana Leech | **CRITICAL** | HIGH | MED | MED |
| Hit Stamina Leech | **CRITICAL** | HIGH | LOW | MED |
| Hit Life Leech | HIGH | HIGH | MED | MED |
| Swing Speed Increase | **CRITICAL** | HIGH | HIGH | MED |
| Damage Increase | HIGH | HIGH | HIGH | HIGH |
| Slayer | HIGH | HIGH | HIGH | HIGH |
| Hit Chance Increase | MED | HIGH | HIGH | HIGH |
| Hit Lower Defense | MED | MED | MED | LOW |
| Hit Area Effects | HIGH (farm) | MED | LOW | LOW |

---

## 51Alpha Design: Special Quest Weapons (Slayer Source)

### Core Principle

**In 51Alpha, Slayer/Killer properties come from SPECIAL QUEST WEAPONS with RANDOMIZED slayers, not talismans or random loot.**

This aligns with the 51Alpha design:
1. **Talismans** unlock skills and provide bonuses (define HOW you fight)
2. **Runic gear** provides stat foundation (HCI/SSI/Leech/Resists)
3. **Special Quest Weapons** provide the slayer damage multipliers (RANDOM slayer on craft)
4. **Cooking** defines where you can go (dungeon access + buffs)

### Special Quest Weapons

| Weapon | Base Type | Quest Required | Special Properties |
|--------|-----------|----------------|-------------------|
| **Blood Tentacle** | Kryss | Blood Tentacle Quest | Random Slayer + unique mechanics |
| **Wolf Bane** | Broad Sword | Wolf Bane Quest | Random Slayer + unique mechanics |
| **Diamond Katana** | Katana | Diamond Katana Quest | Random Slayer + unique mechanics |
| **Dwarven Battle Axe** | Battle Axe | Dwarven Axe Quest | Random Slayer + unique mechanics |
| **Hell's Halberd** | Halberd | Hell's Halberd Quest | Random Slayer + unique mechanics |
| **Black Widow** | Dagger/Kryss | Black Widow Quest | Random Slayer + unique mechanics |
| **Judgement Hammer** | War Hammer | Judgement Hammer Quest | Random Slayer + unique mechanics |

### Slayer System - RANDOMIZED

**On Craft, the weapon receives a RANDOM slayer type:**

| Slayer Type | Target Creatures | Damage Bonus |
|-------------|-----------------|--------------|
| Demon Slayer | Demons, Balrons, Imps | +100% |
| Undead Slayer | Liches, Skeletons, Vampires, Mummies | +100% |
| Reptile Slayer | Dragons, Drakes, Serpents, Lizardmen | +100% |
| Arachnid Slayer | Giant Spiders, Terathans, Scorpions | +100% |
| Elemental Slayer | All Elementals (Fire, Water, Earth, Air) | +100% |
| Repond Slayer | Orcs, Trolls, Ogres, Ettins, Cyclops | +100% |
| Fey Slayer | Pixies, Wisps, Treefellows | +100% |
| Beast Slayer | Wolves, Bears, Great Cats, Boars | +100% |

**Why Randomized:**
- Builds use specific weapon TYPES (Sampire uses fast weapons, Dexer uses Whirlwind weapons)
- Random slayer allows ANY build to be effective in ANY dungeon with the RIGHT slayer roll
- Creates crafting economy - players craft multiple weapons seeking specific slayers
- Encourages weapon diversity - collect different slayer versions of your preferred weapon

### Quest Requirements (Recipe-Locked)

**Each weapon REQUIRES completing a quest to unlock the recipe:**

| Weapon | Quest Name | Quest Type |
|--------|-----------|------------|
| **Blood Tentacle** | "The Crimson Tendril" | Investigation + Combat |
| **Wolf Bane** | "Hunter's Mark" | Tracking + Combat |
| **Diamond Katana** | "The Facet That Cuts" | Puzzle + Exploration |
| **Dwarven Battle Axe** | "Forge of the Ancients" | Crafting + Combat |
| **Hell's Halberd** | "Infernal Reach" | Combat + Ritual |
| **Black Widow** | "Silk and Venom" | Stealth + Combat |
| **Judgement Hammer** | "Weight of Justice" | Investigation + Combat |

**Recipe System:**
- Cannot craft the weapon without completing the quest
- Recipe is character-bound (not account-wide)
- Quest provides recipe scroll consumed on learning
- Once learned, can craft unlimited weapons of that type

### Weapon Unique Mechanics (Beyond Slayer)

**Blood Tentacle (Kryss):**
- Fast weapon speed (2.00s)
- Armor Ignore / Infectious Strike specials
- Unique: Enhanced poison application

**Wolf Bane (Broad Sword):**
- Balanced speed (3.25s) with good damage
- Crushing Blow / Armor Ignore specials
- Unique: Bonus damage vs pack creatures (stacks per nearby enemy)

**Diamond Katana:**
- Fast speed (2.50s)
- Double Strike / Armor Ignore specials
- Unique: Ignores % of physical resistance

**Dwarven Battle Axe:**
- High damage, slower speed (3.50s)
- Whirlwind / Double Strike specials
- Unique: Armor break stacks on target

**Hell's Halberd:**
- Reach weapon, AOE potential
- Whirlwind / Concussion Blow specials
- Unique: Bonus damage vs burning/enraged targets

**Black Widow:**
- Very fast speed
- Infectious Strike / Shadowstrike specials
- Unique: Stacking venom slows enemy attack speed

**Judgement Hammer:**
- High damage mace
- Crushing Blow / Armor Ignore specials
- Unique: Chance to stun, execute bonus vs low HP targets

### Integration with Talisman Builds

**System Boundaries (DO NOT VIOLATE):**
1. **Talismans define how you fight** - Playstyle modifiers only
2. **Weapons define what you can efficiently hunt** - Slayer bonuses only
3. **Cooking defines where you can go** - Access and buffs only
4. **No system replaces another** - Clear boundaries maintained

### Weapon + Build Synergy

| Build | Best Quest Weapon | Why |
|-------|-------------------|-----|
| **Dexer (Bushido)** | Diamond Katana | Fast katana speed, Armor Ignore, Bushido synergy |
| **Dexer (Bushido)** | Dwarven Battle Axe | Whirlwind for AOE with Bushido bonus |
| **Dexer (Bushido)** | Hell's Halberd | Reach + Whirlwind for Bushido AOE |
| **Sampire** | Blood Tentacle | Fast Kryss for life leech sustain |
| **Sampire** | Dwarven Battle Axe | High damage + Whirlwind for farming |
| **Archer** | Any (melee backup) | For when enemies get close |
| **Ninja** | Blood Tentacle | Fast, poison synergy |
| **Ninja** | Black Widow | Stealth synergy, venom stacking |

### Crafting Multiple Slayer Versions

**Expected Player Behavior:**
- Complete quest once per weapon type
- Craft multiple versions seeking different slayers
- Keep a "slayer set" for different dungeon content
- Trade/sell unwanted slayer rolls to other players

**Example Sampire Weapon Set:**
1. Blood Tentacle (Undead Slayer) - for Deceit
2. Blood Tentacle (Demon Slayer) - for Hythloth
3. Blood Tentacle (Reptile Slayer) - for Destard
4. Dwarven Battle Axe (Elemental Slayer) - for Shame/Fire/Ice

---

## Sources

- [UOGuide - Runic Tools](https://www.uoguide.com/Runic_Tools)
- [UO Wiki - Runic Bonuses](https://uo.com/wiki/ultima-online-wiki/items/runic-bonuses/)
- [UOGuide - Sampire](https://www.uoguide.com/Sampire)
- [UOGuide - Talismans](https://www.uoguide.com/Talismans)
- [UOGuide - Hit Chance Increase](https://www.uoguide.com/Hit_Chance_Increase)
- [UOGuide - Defense Chance Increase](https://www.uoguide.com/Defense_Chance_Increase)
- [UOGuide - Special Moves](https://www.uoguide.com/Special_Moves)
- [UOGuide - Archery](https://www.uoguide.com/Archery)
- [UO Wiki - Special Moves](https://uo.com/wiki/ultima-online-wiki/combat/special-moves/)
- [Stratics - Sampire Weapons](https://community.stratics.com/threads/sampire-weapons.255144/)
