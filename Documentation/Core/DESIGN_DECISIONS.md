# 51alpha Design Decisions

**Status**: LOCKED (Do not change without strong justification)
**Last Updated**: 2025-01-15
**Source**: Phase 1 Verification Report v3
**Authority**: TIER 1 - This document is the final word on all design decisions

> **If any other document conflicts with this one, THIS DOCUMENT WINS.**
> See `Documentation/INDEX.md` for topic lookup and document hierarchy.

All decisions below have been finalized. Implementation details (specific numbers, tuning) can be adjusted, but the core design should not change.

---

## Table of Contents
1. [Factions](#1-factions)
2. [PvP Equipment](#2-pvp-equipment)
3. [Guilds](#3-guilds)
4. [Item Binding](#4-item-binding)
5. [Spells](#5-spells)
6. [Talismans](#6-talismans)
7. [Jewelry](#7-jewelry)
8. [Relics & Economy](#8-relics--economy)
9. [Sieges](#9-sieges)
10. [Dungeons](#10-dungeons)
11. [Daily Content](#11-daily-content)
12. [AFK Gathering](#12-afk-gathering)
13. [Mini-Games](#13-mini-games)
14. [Town Cryer](#14-town-cryer)
15. [Housing](#15-housing)
16. [Crafting](#16-crafting)
17. [PvP Combat Balance](#17-pvp-combat-balance-imaginNation-style) ← NEW
18. [Seasonal Ore System](#18-seasonal-ore-system) ← NEW

---

## 1. Factions

### Three Factions
| Faction | Home City | Hue | Color |
|---------|-----------|-----|-------|
| The Golden Shield | Trinsic | 2721 | Gold |
| The Bridgefolk | Vesper | 2784 | Blue |
| The Lycaeum Order | Moonglow | 2602 | Purple |

### Rules
- **Guild membership REQUIRED** to join a faction
- Solo players may create 1-member guilds to participate
- **7-day cooldown** on faction changes
- **Week 1 lockout** - no faction changes during server's first week
- **10% NPC vendor discount** in home city only
- All factions have **equal power** at launch
- No faction-specific perks beyond the discount
- No NPC affiliation changes based on faction
- No quest restrictions based on faction

### Siege Towns
All three factions can contest: **Jhelom, Yew, Skara Brae**

---

## 2. PvP Equipment

### What's ACTIVE in PvP
| Item Type | Active? | Notes |
|-----------|---------|-------|
| Base Armor (leather/plate) | YES | Base AR and material bonuses only |
| Normal Weapons | YES | Base damage and material bonuses only |
| Crafted Weapons | YES | Base stats only - **runic bonuses DISABLED** |
| Jewelry (all 4 pieces) | YES | Stats remain active |

### What's DISABLED in PvP
| Item Type | Active? | Notes |
|-----------|---------|-------|
| Talismans (T3/T2/T1) | NO | Instantly unequipped on PvP flag |
| **Runic Weapon Bonuses** | NO | Hit Life Leech, damage bonuses, procs = PvE only |
| **Runic Armor Bonuses** | NO | Resist bonuses, regen, etc. = PvE only |
| Enchanted Armor | NO | **REMOVED from game entirely** |
| Imbued Weapons | NO | **REMOVED from game entirely** |

### Runic Bonus PvP Behavior (Option E-1)
All runic-crafted properties are **PvE-only**:
- Hit Life Leech, Hit Mana Leech, Hit Stamina Leech → 0% vs players
- Weapon damage bonus → 0% vs players
- Hit spell procs (Fireball, Lightning, etc.) → Don't trigger vs players
- Armor resist bonuses from runic → Don't apply vs player damage
- Armor regen bonuses from runic → Don't apply during PvP

**Base material bonuses STILL WORK in PvP:**
- Valorite weapon = still best base damage
- Valorite armor = still best base AR and durability
- Only the magical runic properties are disabled

### Talisman PvP Behavior
- Player flags for PvP with talisman equipped → Instantly unequipped
- Cannot equip talisman while PvP flagged
- 5-minute disable timer after PvP engagement

### Design Goal
**Skill-based PvP** - gear provides minimal advantage, player skill matters most. Runic bonuses reward PvE grinding but don't create gear inequality in PvP.

---

## 3. Guilds

### Treasury System
- Guild treasury EXISTS (gold and cheques only)
- **All members** can contribute
- **Leader + Treasurer** can withdraw
- No withdrawal limits
- **Protected** from siege loss (cannot be looted)
- Transaction log visible to Leader/Officers

### Roles & Permissions
| Role | Permissions |
|------|-------------|
| **Guild Leader** | All permissions (create/disband, invite/remove, treasury, wars, etc.) |
| **Officers** | Invite/remove members, manage treasury |
| **Treasurer** | Dedicated bank management role |
| **PvMer** | Guild role tag (not PvP flagged except sieges) |
| **Crafter** | Guild role tag (not PvP flagged except sieges) |
| **Members** | View treasury balance, request withdrawals |

### Guild Mechanics
- **NO guild leveling** - Faction points system instead
- **NO guild-wide progression bonuses**
- One guild at a time per player
- **7-day cooldown** to leave and rejoin any guild
- Must have **0 members** to disband
- On disband: Treasury → Guild Leader's bank

---

## 4. Item Binding

### Binding Matrix
| Item Type | Initial | After Equip | Tradeable? | Vendorable? |
|-----------|---------|-------------|------------|-------------|
| Talismans (all tiers) | Unbound | **BOUND** | NO | NO |
| Jewelry (all pieces) | Unbound | **BOUND** | NO | NO |
| Relics | Tradeable | N/A | YES | NO |
| Gold | Tradeable | N/A | YES | YES |
| Housing Items | Unbound | Tradeable | YES | NO |
| Crafted Armor | Unbound | Tradeable | YES | YES |
| Crafted Weapons | Unbound | Tradeable | YES | YES |
| Mob Drops | Unbound | Tradeable | YES | YES |
| Boss Uniques | Unbound | Tradeable | YES | NO |
| Cosmetics/Titles | N/A | N/A | NO | NO |

### Binding Rules
- **Trigger**: Binding occurs on EQUIP only (not on craft/loot)
- **Consequence**: Bound items cannot be traded, vendored, or guild banked
- **NO unbinding** - once bound, always bound
- **NO guild gifting** of bound items
- **Character deletion**: Bound items are LOST permanently
- Merchants CAN buy unbound crafted items

### Design Goal
Progression items (talismans/jewelry) are personal achievements. Economy items (weapons/armor/housing) remain tradeable to support crafters and economy.

---

## 5. Spells

### 51a-Style Changes (vs. Standard ModernUO)
| Feature | Standard | 51alpha |
|---------|----------|---------|
| Target selection | After cast delay | **Immediately on cast** |
| Mana consumption | After fizzle check | **Before cast delay** |
| Fizzle cost | 0% mana | **50% mana lost** |
| Movement | Frozen during cast | **Free movement** |
| Damage interrupt | Yes | **NO** |
| Equip interrupt | Yes | **NO** |

### What CAN Interrupt Spells
- Casting another spell
- Toggling war mode (on/off)
- Using bandages
- Death

### What CANNOT Interrupt Spells
- Taking damage
- Equipping items
- Movement
- Using objects (except bandages)

### Faster Casting
- **FC removed** from all equipment in the game
- Protection spell has **no FC penalty**
- Use `[RemoveAllFC` command to clean existing items

### Scroll Bonuses
- **43% mana reduction** when casting from scroll
- **0.5 second faster** for circle 3+ spells
- Circle 1-2 scrolls: Same speed as memory

### Cast Times (with scrolls for circle 3+)
| Circle | Memory | Scroll |
|--------|--------|--------|
| 1st | 1.0s | 1.0s |
| 2nd | 1.25s | 1.25s |
| 3rd | 1.5s | **1.0s** |
| 4th | 1.75s | **1.25s** |
| 5th | 2.0s | **1.5s** |
| 6th | 2.25s | **1.75s** |
| 7th | 2.5s | **2.0s** |
| 8th | 2.75s | **2.25s** |

---

## 6. Talismans

### Four Talisman Types
| Type | Build Archetype | Key Abilities Unlocked |
|------|-----------------|------------------------|
| **Dexer** | Pure melee fighter | Bushido (PvP), Weapon Special Moves (PvP) |
| **Tamer** | Animal tamer | Taming bonuses, pet control |
| **Sampire** | Paladin warrior | Chivalry (PvP), Necromancy (PvP) |
| **Treasure Hunter** | Dungeon explorer | Lockpicking, cartography bonuses |

### Tiers (Each Type Has All 3)
| Tier | Name Pattern | Relic Cost | Mage Stones | Crafting Failure | Timer |
|------|--------------|------------|-------------|------------------|-------|
| T3 | "Bloodthorn" | 5 Common | 1 | 30% at 80 skill | 7 days gameplay |
| T2 | "Bloodmage" | 8 Uncommon + 3 Common | 3 | 40% at 100 skill | 7 days gameplay |
| T1 | "Bloodlord" | 10 Rare + 5 Uncommon + 1 Legendary | 5 | 50% at 120 skill | 7 days gameplay |

**Mage Stones:** Specific relic drops from dungeon mobs, used as talisman crafting ingredient.

### Key Mechanics
- **7-day GAMEPLAY timer** - only counts when equipped and playing
- **Pauses** when unequipped or offline
- **Disabled in PvP** - drops to backpack instantly on PvP flag
- **5-minute cooldown** after PvP engagement before re-equipping
- **Bound on equip** - cannot trade after wearing
- **Skill Reduction:** When worn, reduces all OTHER skills to 0 (creates OSI 7-skill style templates)

### Talisman-Gated Abilities (PvP Only)
These abilities are DISABLED in PvP unless the appropriate Talisman is equipped:

| Ability | Required Talisman | Notes |
|---------|-------------------|-------|
| Bushido | Dexer | Parry bonus, special moves |
| Weapon Special Moves | Dexer | Mortal Strike, etc. |
| Chivalry | Sampire | All paladin abilities |
| Necromancy | Sampire | All necro spells/forms |

**ALL talisman-gated abilities are disabled vs players when talisman drops.**

### Crafting Failure
- On failure: 70% of relics consumed, 30% recovered
- Higher skill = lower failure rate

---

## 7. Jewelry

### PvP Jewelry (From Siege Coins)
**4-Piece System:**
| Piece | Stats per Piece | Notes |
|-------|-----------------|-------|
| Ring | +2 Str, +2 Dex, +2 Int | Standard layer |
| Earrings | +2 Str, +2 Dex, +2 Int | Standard layer |
| Bracelet | +3 Str, +3 Dex, +3 Int | Standard layer |
| Glasses | +3 Str, +3 Dex, +3 Int | **NEW LAYER** (not helmet) |

**Full Set Bonus:** +10 Str, +10 Dex, +10 Int (max)
**Stat Cap:** 125 hard cap with jewelry

### Crafter Jewelry (Separate System)
Crafter jewelry exists separately for crafting skill bonuses.

### Timer Mechanics
- **7-day CONTINUOUS timer** - counts even when offline/unequipped
- Timer starts on **first equip**
- **Staggered renewal** - equip pieces on different days
- After 7 days: Piece expires, needs replacement

### Example Timeline
```
Day 1 (Mon): Equip Ring → Timer starts
Day 2 (Tue): Equip Earrings → Timer starts
Day 3 (Wed): Equip Bracelet → Timer starts
Day 4 (Thu): Equip Glasses → Timer starts
Day 8 (Mon): Ring expires → Get new one
Day 9 (Tue): Earrings expire → Get new one
```

### Costs
- Per piece: 2 Uncommon + 1 Rare relics
- Full set: 8 Uncommon + 4 Rare relics
- Weekly renewal: ~5 Uncommon + 2 Rare (staggered)

### Binding
- **Bound on equip** - cannot trade after wearing
- **Active in PvP** (unlike talismans)

---

## 8. Relics & Economy

### Relic Sources
- **ONLY from dungeons** - outside world drops gold/items, NOT relics
- Drop rates scale with dungeon difficulty

### Relic Tiers
| Tier | Drop Rate | Primary Use |
|------|-----------|-------------|
| Common | 2-10% | T3 talismans, filler |
| Uncommon | 0.2-15% | T2 talismans, jewelry |
| Rare | 0-8% (Hard/Extreme only) | T1 talismans, jewelry |
| Legendary | 0-2% (Extreme only) | T1 talismans (gate keeper) |

### Expected Yields
| Difficulty | Relics/Hour | Notes |
|------------|-------------|-------|
| Easy | 5-10 | Solo/duo content |
| Medium | 10-15 | Small group |
| Hard | 20-30 | Group required (4+) |
| Extreme | 40-60 | Raid content (8+) |

### Design Goal
**Scarcity creates guild dependency**. No single player can sustain talismans + jewelry + housing without guild support or specialization.

---

## 9. Sieges

### Schedule
- **2-hour rotation**: One city every 2 hours
- Cycle: Yew → Skara Brae → Jhelom → (repeat)
- Delays if tournament or PvM invasion active

### Mechanics
- **Three-way** faction battles (all factions can participate)
- **Victory condition**: 10,000 points
- Objectives: Altar control, kill scoring
- Glicko-2 rating integration for participants

### Town Control
- **Persistent** until recaptured (not time-limited)
- Town Cryer announces current ownership
- Benefits: 10% NPC discount, faction banners

### Deferred Details (Phase 2)
- Specific reward amounts
- Win conditions (damage vs. objectives)
- Tie-breaking rules
- Guard mechanics

---

## 10. Dungeons

### Difficulty Tiers
| Tier | Skill Req | Group Size | Relics/Hour |
|------|-----------|------------|-------------|
| Easy | 50+ | Solo/Duo | 5-10 |
| Medium | 70-80 | Small group | 10-15 |
| Hard | 100+ | 4+ players | 20-30 |
| Extreme | 120+ | 8+ players | 40-60 |

### Weekly Rotation
- One dungeon gets **+20%** loot
- One dungeon gets **-20%** loot
- Rotates weekly to encourage variety
- Total server relic output stays constant

### Loot Tables
Specific boss drops deferred to implementation phase. Will be built alongside dungeon content.

---

## 11. Daily Content

### Bounty System
- **3 daily bounties** (same for all players)
- Types: Monster kills, resource gathering, crafting
- Rewards: Silver currency

### Faction Quest
- Daily boss spawn at announced location
- HP scales with participants
- Drops relics (participation-based)

### Silver Vendor
- Accepts Silver currency
- Sells: Faction robes, dyes, house decorations
- Cosmetic items only (no progression advantage)

---

## 12. AFK Gathering

### Supported Skills
- Mining
- Lumberjacking
- Cotton gathering
- Fishing

### Mechanics
- **75% efficiency** vs. active gathering
- Works **everywhere** (PvP areas at player risk)
- **Unlimited session time**
- **Pauses on logout**

### Anti-AFK (Captcha)
- Captcha appears every **1-2 hours** (randomized)
- **30-second window** to answer
- If missed: Player flagged AFK, gathering stops
- Prevents botting/macro abuse

### PvP Risk
- Safe zones: Protected while AFK
- PvP areas: Vulnerable to attack

---

## 13. Mini-Games

### Texas Hold'em
- **Included at launch**
- PvP only (players compete for pot)
- **No-limit** format
- **Gold only** (no relics)
- **5% house take** (gold sink)
- **Optional** - no progression impact
- No level requirements
- No guild tournaments

---

## 14. Town Cryer

### Announcements
- Seasonal events ("New season started")
- Event availability
- Town ownership changes

### NOT Announced
- Server maintenance/downtime
- PvP kills
- Guild announcements

### Delivery
- **In-game chat messages**
- **Local scope** (only players in town hear it)
- NPC Town Cryer in: Britain, Vesper, Trinsic, Moonglow

### Control
- Automated (seasonal events)
- Admin-controlled (special announcements)

---

## 15. Housing

### Tiers
| Tier | Relic Cost | Notes |
|------|------------|-------|
| Starter | 25 | Entry level |
| T3 | 50 | Upgrade from Starter |
| T2 | 75 | Upgrade from T3 |
| T1 | 100 | Maximum tier |

### Mechanics
- **One-time cost** (no decay/maintenance)
- **Permanent** once placed
- **Salvage**: 25% relic recovery on demolish
- **Tradeable** (not bound)

### Upgrade Path
Total cost from Starter to T1: ~212.5 relics (after salvage returns)

---

## 16. Crafting

### Decision: Option E-1 (Full Runic, PvE-Only Bonuses)
**LOCKED**: Keep full runic crafting system, but all runic bonuses only work against monsters.

### Runic Crafting System
**Full runic system is ENABLED:**
- **7 ore tiers** with seasonal name/hue rotation (T1-T7)
- Runic hammers add 1-5 magical properties
- All standard UO runic properties available (Hit Life Leech, damage bonuses, etc.)

**Runic tools obtained from:**
- Bulk Order Deed (BOD) rewards (primary source)
- Rare mining drops
- High-end dungeon loot

### Runic Properties by Resource
| Resource | Properties | Min Intensity | Max Intensity |
|----------|------------|---------------|---------------|
| Dull Copper | 1-2 | 40% | 100% |
| Shadow Iron | 2 | 45% | 100% |
| Copper | 2-3 | 50% | 100% |
| Bronze | 3 | 55% | 100% |
| Gold | 3-4 | 60% | 100% |
| Agapite | 4 | 65% | 100% |
| Verite | 4-5 | 70% | 100% |
| Valorite | 5 | 85% | 100% |

### PvE vs PvP Behavior
| Property Type | vs Monsters | vs Players |
|---------------|-------------|------------|
| Hit Life Leech | Full effect | **0%** |
| Hit Mana/Stam Leech | Full effect | **0%** |
| Weapon Damage Bonus | Full effect | **0%** |
| Hit Spell Procs | Full effect | **Disabled** |
| Armor Resist Bonus | Full effect | **0%** |
| Armor Regen Bonus | Full effect | **0%** |
| Slayer Properties | Full effect | N/A (already PvE) |
| **Base Material Stats** | Full effect | **Full effect** |

### Sampire Build Sources
| Component | Source | Works in PvP? |
|-----------|--------|---------------|
| Hit Life Leech | Runic weapon (up to 50%) | NO - PvE only |
| High Damage | Valorite material bonus | YES |
| Vampiric Embrace | Necromancy skill | YES |
| Chivalry | Sampire Talisman gates it | NO - Talisman disabled |

### Skills
| Skill | Creates | Cap |
|-------|---------|-----|
| Tinkering | Jewelry, Talismans | 120 |
| Blacksmithing | Talismans, Jewelry, Weapons, Armor | 120 |
| Carpentry | Housing items | 120 |

### Skill Gain Formula
- **Linear gains** early (0-50)
- **Diminishing returns** mid/late (50-120)
- **Difficulty-scaled** (harder items = more gains)

### Timeline Targets
- Casual (1 hr/day): ~6 months per skill to 120
- Professional (guild focus): 3-4 weeks per skill
- No-life grinder (8 hrs/day for 1 year): All 3 skills maxed

### Crafting Failures
- Consume **70%** of relics
- Return **30%** of relics
- Higher skill = lower failure rate

---

## 17. PvP Combat Balance (ImaginNation-Style)

**LOCKED**: PvP uses separate formulas from PvM for independent balance tuning.

### Combat Formulas

| System | PvP Formula | PvM Formula |
|--------|-------------|-------------|
| **Hit Chance** | `0.69 × ((Skill + Tactics×2) / 300)` = 69% max | Default UO formula |
| **AR Absorption** | `Random(AR/4.3 to AR/2.4)` | `Random(AR/2 to AR)` |
| **Parry** | 20% at GM with shield only | Default |
| **Protection Spell** | Fixed +5 AR | Fixed +5 AR |

### 7-Tier Armor System

| Tier | Mining | Plate AR | Gap |
|------|--------|----------|-----|
| T1 (Iron) | 0 | 25 | - |
| T2 | 55 | 30 | +5 |
| T3 | 65 | 35 | +5 |
| T4 | 75 | 45 | +10 |
| T5 | 85 | 55 | +10 |
| T6 | 95 | 60 | +5 |
| T7 | 99 | 65 | +5 |

**Other Armor Base AR:**
- Leather: 15
- Bone: 25 (same as plate T1)
- Chainmail/Ringmail Tunic: 25 (chest only, other pieces removed)

### Shield AR (Flat Values)

| Shield | Non-GM Parry | GM Parry |
|--------|--------------|----------|
| Buckler | 1 | 5 |
| Wooden Shield | 5 | 10 |
| Heater Shield | 7 | 15 |
| Faction Shields | 10 | 20 |

**Removed Shields:** Bronze, Metal, Wooden Kite, Metal Kite

### Mining Stones (Rare Drops)

Two stone types found while mining. RNG determines bonus on application.

**Sharpening Stone:**
- Force (+6 dmg), Power (+8 dmg), Vanquishing (+10 dmg)
- Higher mining skill = better odds for Power/Vanq

**Accuracy Stone:**
- Accurate (+5 Tactics), Surpassingly (+10), Eminently (+15), Exceedingly (+20), Supremely (+25)
- Also adds +2% to +10% hit chance bonus

**Both stones:** Permanent, one per weapon type, can stack together on same weapon.

### Talisman-Gated Abilities

These abilities are DISABLED in PvP without the appropriate Talisman:

| Ability System | Required Talisman |
|----------------|-------------------|
| Bushido | Bushido Talisman |
| Chivalry | Chivalry Talisman |
| Necromancy | Necromancy Talisman |
| Weapon Special Moves | Weapon Master Talisman |

**PvM Unaffected:** All abilities work normally vs monsters regardless of Talisman.

### Removed Items

**Weapons:** Cutlass, Pike, WarFork, Pitchfork, Leafblade, CompositeBow, WarMace, and others
**Shields:** Bronze Shield, Metal Shield, Wooden Kite Shield, Metal Kite Shield
**Armor:** All chainmail/ringmail except chest tunics

### Exceptional Quality

- **NO AR bonus** - cosmetic only (crafter's mark)
- Ore tier determines AR, not quality

### Detailed Specification

Full implementation details in `Documentation/PVP_COMBAT_SYSTEM.md`

---

## 18. Seasonal Ore System

**LOCKED**: 7-tier ore system with seasonal rotation.

### Ore Tiers

Each season (3 months), ore names and hues change but mechanics stay the same.

| Tier | Mining Req | Properties | Intensity |
|------|------------|------------|-----------|
| T1 | 0 | 0 | - |
| T2 | 55 | 1-2 | 40-100% |
| T3 | 65 | 2 | 45-100% |
| T4 | 75 | 2-3 | 50-100% |
| T5 | 85 | 3-4 | 60-100% |
| T6 | 95 | 4 | 70-100% |
| T7 | 99 | 5 | 85-100% |

### Seasonal Rotation

| Season | Dates | Theme |
|--------|-------|-------|
| Spring | Mar 20 - Jun 20 | Growth, renewal |
| Summer | Jun 21 - Sep 21 | Heat, ocean |
| Fall | Sep 22 - Dec 20 | Harvest, earth |
| Winter | Dec 21 - Mar 19 | Ice, darkness |

**Iron (T1):** Always available, never changes.
**Season change:** Ore veins switch, existing items keep original name/hue.

### Detailed Specification

Full implementation details in `Documentation/SEASONAL_ORE_SYSTEM.md`

---

## Deferred Decisions

These are intentionally left for implementation phase:

1. **Crafting skill gain numbers** - Specific gains per item tuned during coding
2. **Siege reward amounts** - Balanced during testing
3. **Dungeon boss loot tables** - Built with dungeon content
4. **Specific town control benefits** - Finalized in Phase 2
5. **Creature-derived armor details** - Lich Bone, Dragon Scale, Titan Skin, Ophidian Silk (see SEASONAL_ORE_SYSTEM.md)

---

## Change Log

| Date | Change | Reason |
|------|--------|--------|
| 2025-01-13 | Initial consolidation | Documentation cleanup |
| 2025-01-14 | Added Option E-1 crafting decision | Full runic system, PvE-only bonuses |
| 2025-01-15 | Added Section 17: PvP Combat Balance | ImaginNation-style combat formulas |
| 2025-01-15 | Added Section 18: Seasonal Ore System | 7-tier ore with seasonal rotation |

---

*This document consolidates decisions from PHASE1_VERIFICATION_REPORT_v3.md and related economy specifications.*
