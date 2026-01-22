# 51alpha Siege Coin Reward System

**Authority**: TIER 2 - Implementation Spec (see `DESIGN_DECISIONS.md` for authoritative decisions)

> **Note**: This document requires re-verification against DESIGN_DECISIONS.md for:
> - Jewelry slots (§7): Ring, Bracelet, Earrings, Glasses
> - Timer mechanics: 7-day continuous (not 180hr logged-in)
> - Some sections may be outdated drafts

## Document Metadata
- Version: v1.0.0
- Last Updated: 2025-12-27
- Primary System: PvP
- Secondary Systems: Economy, Progression, Items
- Status: Draft (NEEDS REVIEW)
- CHANGELOG Reference: [2.3.0]

## 1. Overview

The Siege Coin Reward System implements a performance-based PvP reward system that provides temporary stat boosts and faction-specific items through siege participation. This system is designed to reward active PvP engagement while maintaining strict separation between PvP and PvM power systems.

### Core Design Goals
- Reward participation and performance, not just victory
- Provide temporary, non-permanent power
- Keep PvP competitive without invalidating skill
- Prevent economy hoarding and power lock-in
- Hard-disable synergy with PvM talismans
- Fit cleanly into seasonal resets

## 2. System Classification

### Primary System: PvP
This system is primarily focused on faction sieges and PvP combat rewards.

### Secondary Systems: Economy, Progression, Items
- **Economy**: Introduces new currency (Siege Coins) and vendor system
- **Progression**: Provides temporary stat boosts that affect character progression
- **Items**: Adds new jewelry, clothing, mounts, and banners

## 3. Design Specifications

### 3.1 Siege Coins (Primary Currency)
- Non-tradable, character-bound currency
- Earned only from faction siege participation
- Persist across the season
- Wiped at season reset

### 3.2 Siege Coin Acquisition (Performance-Based)
Coins are awarded after each siege event, based on contribution score, not faction victory alone.

#### Contribution Score Sources
Each category has minimum thresholds to prevent AFK abuse.

**PvP Combat:**
- Damage dealt to enemy players
- Healing done to allied players
- Killing blows on enemies who dealt damage back
- Damage received while alive (discourages suicide zerging)

**Objective Play:**
- Time contesting objectives
- Successful captures / defenses
- Interrupting enemy captures

**Participation Integrity:**
- Time active in siege region
- Movement + action checks
- Diminishing returns for killing same target repeatedly
- No credit if victim dealt negligible damage back

**Victory Modifier:**
- Winning faction receives a small multiplier, not a massive payout
- Losing faction still earns meaningful coins
- **Result:** Losing well > Winning AFK

### 3.3 Siege Coin Vendor
A dedicated Faction Siege Vendor accessible only to faction members.

**Categories:**
- Jewelry
- Clothing
- Mounts & Banners

**All items:**
- Character-bound
- Time-limited
- Disabled while a talisman is equipped

### 3.4 Jewelry System (Temporary Stat Boosts)

**Authority**: See `DESIGN_DECISIONS.md` §7 for authoritative jewelry slots.

**Slots (4 pieces):**
- Ring
- Bracelet
- Earrings
- Glasses

**Stat Rules:**
- Each piece grants +2.5 Str, +2.5 Dex, +2.5 Int (all three stats)
- Full set bonus: +10 Str, +10 Dex, +10 Int (max)

**Duration:**
- 180 hours of logged-in time
- Timer starts on purchase
- Pauses when logged out
- Visible decay timer

**Restrictions:**
- Jewelry bonuses do NOT stack beyond +10
- Jewelry bonuses do NOT apply in PvM if talisman is equipped

### 3.5 Faction Clothing System (Temporary AR Boost)

**Clothing Slots (4 Total):**
- Faction Apron
- Faction Robe OR Sash
- Faction Doublet OR Tunic
- Faction Skirt OR Kilt

**Armor Rules:**
- Each piece grants +2.5 AR
- Max AR bonus: +10
- Counts as clothing, not armor
- Does NOT interfere with crafted armor balance

**Visual Rules:**
- Specifc to players faction hue
- Remains visually faction-identifiable via base pattern

**Duration:**
- 180 hours logged-in
- Same decay rules as jewelry

### 3.6 Other Purchasable Items

**Faction Mount:**
- Specific to players Faction-colored mount
- +10 Dexterity
- Dex bonus counts toward the +10 stat cap
- Does NOT stack beyond cap
- Disabled while talisman is equipped
- Same 180h decay system
- Purpose: Mobility advantage in sieges, tactical repositioning, visual prestige

**Faction Banner:**
- Cosmetic + social object
- Season # in the Banner name IE  "Bridgefolk season 1 banner" or "Bridgefolk S1 Banner"
- Can be placed in houses or carried during sieges
- Optional future mechanics: siege banner layering deed and minor morale buff, guild identification, siege scoring bonus
- No decay required (cosmetic)

### 3.7 Talisman Interaction (Critical Rule)

**Absolute Rule:**
If a talisman is equipped → ALL siege items are DISABLED

This includes:
- Jewelry stat bonuses
- Clothing AR bonuses
- Mount stat bonuses

**Behavior:**
- Items may remain equipped
- Bonuses dynamically turn off
- Bonuses instantly return when talisman is unequipped

**Result:**
PvM players must choose: Talisman power OR Siege power
- No PvM → PvP power stacking
- No farming PvM with siege bonuses

### 3.8 Anti-Abuse & Balance Safeguards

**Anti-Alt / Exploit Protection:**
- No siege coins if damage dealt < threshold
- No siege coins if victim did not fight back
- No siege coins for repeated kills of same character
- Diminishing returns per target per siege
- IP/account linkage already applied

**Seasonal Integration:**
- Siege coins wiped every season
- Siege items expire naturally or wiped
- Cosmetics / titles may persist
- System resets cleanly every quarter

## 4. Related Systems

- **PvP System** → Faction Warfare → Siege Coin Reward System
- **Economy System** → Currency Systems → Siege Coin Vendor
- **Progression System** → Stat Boosts → Temporary Stat System
- **Items System** → Special Items → Siege-Specific Items

**Dependency Graph:**
```
PvP System
├─ Faction Warfare
│  └─ Siege Coin Reward System
│     ├─ Contribution Scoring
│     ├─ Siege Coin Vendor
│     │  ├─ Jewelry System
│     │  ├─ Clothing System
│     │  ├─ Mount System
│     │  └─ Banner System
│     └─ Talisman Separation Logic
└─ Economy System
   └─ Siege Coin Currency
      └─ Vendor Integration
```

## 5. Implementation Requirements

### Code Targets:
- Projects/UOContent/Sphere51a/FactionSystems/SiegeCoinSystem.cs
- Projects/UOContent/Sphere51a/FactionSystems/SiegeCoinVendor.cs
- Projects/UOContent/Sphere51a/FactionSystems/SiegeContributionTracker.cs
- Projects/UOContent/Sphere51a/Items/SiegeJewelry.cs
- Projects/UOContent/Sphere51a/Items/SiegeClothing.cs
- Projects/UOContent/Sphere51a/Items/SiegeMount.cs

### Configuration:
- Siege coin acquisition rates and thresholds
- Contribution scoring weights
- Victory multiplier values
- Item decay timers (180h)
- Stat cap enforcement (125 max)
- Talisman separation logic

### Database:
- Player siege coin balances (account-wide)
- Contribution score tracking
- Item decay timers
- Seasonal reset logic

## 6. Cross-System Questions

### Q1: Balance Impact - Stat Stacking
**Question:** Does the +10 stat bonus from siege jewelry create unacceptable power gaps in PvP combat?
**Analysis:** The system includes multiple safeguards:
- 125 hard stat cap prevents runaway inflation
- Temporary nature (180h) prevents permanent advantage
- Requires active siege participation (not passive)
- Disabled with talismans (PvM/PvP separation)
- **Risk:** Medium - Requires monitoring and potential tuning

### Q2: Economy Impact - New Currency
**Question:** Does the introduction of siege coins create inflationary pressure on the economy?
**Analysis:** Mitigation factors:
- Non-tradable, character-bound currency
- Limited use (siege vendor only)
- Seasonal reset prevents hoarding
- **Risk:** Low - Isolated from main economy

### Q3: PvP Balance - Victory Modifier
**Question:** Does the victory multiplier create unfair advantages for winning factions?
**Analysis:** Design considerations:
- Small multiplier (not massive payout)
- Losing faction still earns meaningful coins
- Focus on performance, not just victory
- **Risk:** Low - Performance-based system

### Q4: System Conflict - Talisman Interaction
**Question:** Are there any edge cases where talisman separation logic might fail?
**Analysis:** Potential issues:
- Rapid equip/unequip scenarios
- Multi-client exploitation attempts
- Item decay during talisman switches
- **Risk:** Medium - Requires thorough testing

### Q5: Exploit Risk - Contribution Scoring
**Question:** Are the anti-AFK and anti-exploit measures sufficient?
**Analysis:** Current safeguards:
- Minimum damage thresholds
- Victim fight-back requirements
- Diminishing returns per target
- IP/account linkage
- **Risk:** Medium-High - PvP systems always attract exploit attempts

### Q6: Progression Impact - Temporary Boosts
**Question:** Do temporary stat boosts affect long-term character progression perceptions?
**Analysis:** Considerations:
- No permanent power gain
- Skill still primary factor
- Creates meaningful prep gameplay
- **Risk:** Low - Aligns with design goals

### Q7: Missing Information - Seasonal Reset Logic
**Question:** What should be the exact seasonal reset behavior for siege coins and items?
**Analysis:** Needs resolution:
- Full wipe vs partial carryover
- Cosmetic preservation rules
- Reset timing synchronization
- **Status:** TBD - Needs architectural decision

### Q8: Unknown Tuning - Acquisition Rates
**Question:** What are the optimal siege coin acquisition rates for balanced progression?
**Analysis:** Requires:
- Playtesting data
- Economy impact analysis
- PvP balance testing
- **Status:** TBD - Needs tuning phase

## 7. Risk Assessment

**Overall Risk:** High
**Justification:** Complex system with multiple dependencies, stat stacking implications, and PvP balance concerns. The system touches core combat mechanics, economy, and progression systems, requiring careful tuning and monitoring.

**Risk Breakdown:**
- **Implementation Complexity:** High (multiple subsystems)
- **Balance Risk:** High (stat stacking, PvP impact)
- **Exploit Risk:** High (PvP systems attract abuse)
- **Economy Risk:** Medium (new currency system)
- **Integration Risk:** Medium (cross-system dependencies)

## 8. Traceability

### Design Links:
- [Master Architecture - PvP Section](../../Systems/pvp.md)
- [Economy System - Currency Section](../../Systems/economy.md)
- [Progression System - Stat Boosts](../../Systems/progression.md)
- [Items System - Special Items](../../Systems/items.md)
- [CHANGELOG Entry - 2.3.0](../../../CHANGELOG.md#230---2025-12-27)

### Implementation Status:
- [Link to IMPLEMENTATION_STATUS.md](../../Implementation/Navigation/IMPLEMENTATION_STATUS.md)

### Known Conflicts:
- None identified

## 9. Approval

- [ ] Design Review Complete
- [ ] Architecture Review Complete
- [ ] PvP Balance Review Complete
- [ ] Economy Impact Review Complete
- [ ] Implementation Approved
- [ ] CHANGELOG Entry Created
- [ ] Traceability Established
- [ ] Cross-System Questions Documented

## 10. 51alpha Siege Power System — Revised With New Power Ceilings

This document supersedes the previous stat-cap assumptions.

### 10.1 Core Stat Rules (Authoritative)

**Base Stat Rules:**
- Natural stat cap: 100 STR / DEX / INT
- Stats trained normally via skills

**Absolute Maximums:**
- Hard cap with ALL items + buffs: 125 STR / 125 DEX / 125 INT
- No system, item, spell, potion, or exploit may exceed this value
- Any overflow is discarded
- This mirrors classic UO philosophy: You can stack systems, but you never break the ceiling.

### 10.2 Siege Jewelry System (Revised)

**Authority**: See `DESIGN_DECISIONS.md` §7 for authoritative jewelry slots.

**Jewelry Slots (4 pieces):**
- Ring
- Bracelet
- Earrings
- Glasses

**Jewelry Bonuses:**
- Each piece: +2.5 Str, +2.5 Dex, +2.5 Int
- Full set: +10 Str, +10 Dex, +10 Int (max)
- Subject to 125 hard stat cap

**Duration:**
- 7 days continuous timer (starts on first equip)
- Timer counts even when offline/unequipped
- After 7 days: Piece expires, needs replacement

### 10.3 Siege Clothing System (Unchanged, Clarified)

**Slots (4 Total):**
- Faction Apron
- Faction Robe or Sash
- Faction Doublet or Tunic
- Faction Skirt or Kilt

**AR Bonus:**
- +2.5 AR per piece
- +10 AR max
- Clothing-layer AR only
- Does not affect armor durability or crafting balance

**Duration:**
- 180 hours logged-in time

### 10.4 Buff & Consumable Stacking (Formalized)

All of the following stack together, subject to the 125 hard cap.

**Spell Buffs:**
| Spell | Bonus |
|-------|-------|
| Bless | +5 STR / +5 DEX / +5 INT |
| Minor Strength | +5 STR |
| Minor Agility | +5 DEX |
| Minor Cunning | +5 INT |

**Potions:**
| Potion | Bonus |
|--------|-------|
| Strength Potion | +5 STR |
| Greater Strength Potion | +10 STR |
| Agility Potion | +5 DEX |
| Greater Agility Potion | +10 DEX |

**Cooking System (New PvM Tie-In):**
🐟 **Special Intelligence Fish**
- Crafted via GM Cooking
- Provides: +5 INT (regular), +10 INT (exceptional/rare)
- Duration comparable to potion buffs
- Stacks with Bless, Jewelry, Siege items
- Subject to 125 INT hard cap

### 10.5 Mount Bonus (Clarified With New Caps)

**Faction Mount:**
- +10 DEX
- Counts as a stat bonus
- Fully stackable
- Subject to 125 DEX hard cap
- 180h logged-in duration
- Disabled if talisman equipped

### 10.6 Talisman Interaction (Unchanged, Reinforced)

**Absolute Rule:**
If a talisman is equipped, ALL siege bonuses are disabled.

**Buffs That Still Work With Talismans:**
- Spells (Bless, Minor buffs)
- Potions
- Food buffs (fish)

**Reason:**
- PvM remains viable
- Siege gear never bleeds into PvM dominance

### 10.7 Practical Max-Stat Reality (Why This Is Safe)

Even though 125 is the cap, realistic access looks like this:

**Example DEX Stack (Near Max):**
- Base DEX: 100
- Jewelry: +10
- Faction Mount: +10
- Bless: +5
- Greater Agility Potion: +10
- ➡ Total: 135 → capped at 125

**Requirements:**
- Siege participation
- Active consumable use
- Risk exposure
- No talisman

**Result:** You cannot maintain this permanently or casually.

### 10.8 PvP Balance Outcome

**What This Enables:**
✔ Burst windows of power
✔ Tactical preparation before sieges
✔ Rewarding coordinated play
✔ Meaningful consumable economy

**What It Prevents:**
❌ Permanent stat inflation
❌ Passive power stacking
❌ PvM → PvP abuse
❌ "Always-on" superiority

**Final Verdict:** Skill still decides fights — stats only tilt them.

### 10.9 Why This Is the Correct Evolution

This system now:
- Mirrors classic UO stat logic
- Respects Sphere-era stacking
- Allows high-end play without runaway power
- Creates meaningful prep gameplay
- Gives value to crafting, cooking, alchemy
- Preserves seasonal integrity

**Conclusion:** The 125 hard cap is the anchor that keeps everything sane. As long as that cap is enforced globally and ruthlessly, stacking is not a problem — it's a feature.
