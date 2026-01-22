# ServUO Faction System Reference

**Authority**: REFERENCE - For implementation guidance
**Source**: E:\Test\ServUO-pub57
**Purpose**: Document ServUO's faction/siege mechanics for our custom implementation

---

## Overview

This document captures ServUO's faction system implementation as a reference for building our custom 51alpha faction system. Our system will differ significantly but this provides baseline mechanics to consider.

---

## 1. Siege Scoring System

### ServUO Implementation

ServUO does NOT use a "10,000 point battle victory" system. Instead, it uses:

**Kill Point Ranking System:**
- Players earn kill points by defeating faction enemies in PvP
- Points determine rank within faction hierarchy
- Formula: `award = Math.Max(victimState.KillPoints / 10, 1)` capped at 40 points
- Silver reward: `silver = award * 40`

**Death Consequences:**
- Defeated players lose kill points equal to attacker's award
- Minimum -6 kill points removes player from ranking

**Atrophy System:**
- Every 47 hours, inactive players lose 10% of kill points
- Points redistributed to active players

### Our 51alpha Design (Different)

We use a **battle-based scoring system**:

| Mechanic | Our Design |
|----------|------------|
| Victory Condition | 10,000 points |
| Battle Duration | 2-hour cycles |
| Scoring | Objective-based + kills |
| Rating System | Glicko-2 per player |

---

## 2. Defense Budget / Town Finance System

### ServUO Implementation

**File Locations:**
- `Core\Town.cs` (lines 58-170, 287-301)
- `Core\TownState.cs` (lines 136-157)
- `Gumps\FinanceGump.cs` (lines 113-175)
- `Gumps\SheriffGump.cs` (lines 49-76)

**Town Silver/Budget:**
```csharp
public int Silver { get; set; }  // Town treasury
public const int SilverCaptureBonus = 10000;  // Bonus for capturing
```

**Daily Income Calculation:**
```csharp
public int DailyIncome {
    get { return (10000 * (100 + this.m_State.Tax)) / 100; }
}
```
- Base income: 10,000 silver per day
- Modified by tax percentage (0-300% via FinanceGump)

**Upkeep System:**
```csharp
public int FinanceUpkeep {     // Vendor costs
    get {
        int upkeep = 0;
        for (int i = 0; i < vendorLists.Count; ++i)
            upkeep += vendorLists[i].Vendors.Count * vendorLists[i].Definition.Upkeep;
        return upkeep;
    }
}

public int SheriffUpkeep {     // Guard costs (DEFENSE BUDGET)
    get {
        int upkeep = 0;
        for (int i = 0; i < guardLists.Count; ++i)
            upkeep += guardLists[i].Guards.Count * guardLists[i].Definition.Upkeep;
        return upkeep;
    }
}

public int NetCashFlow {
    get { return this.DailyIncome - this.FinanceUpkeep - this.SheriffUpkeep; }
}
```

**Vendor Economics:**

| Vendor Type | Purchase Price | Daily Upkeep | Max Per Town |
|-------------|----------------|--------------|--------------|
| Bottle Vendor | 5,000 silver | 1,000 silver | 10 |
| Board Vendor | 3,000 silver | 500 silver | 10 |
| Ore Vendor | 3,000 silver | 500 silver | 10 |
| Reagent Vendor | 5,000 silver | 1,000 silver | 10 |
| Horse Breeder | 5,000 silver | 1,000 silver | 1 |

**Guard Types:**
- 10 guard types: Berserker, DeathKnight, Dragoon, Henchman, Knight, Mercenary, Necromancer, Paladin, Sorceress, Wizard
- Each has purchase price and daily upkeep cost

**Bankruptcy Handling:**
- If daily flow becomes negative, guards/vendors randomly deleted
- Prevents towns from going insolvent

### Our 51alpha Consideration

We should consider:
- Whether to have persistent town ownership between sieges
- If yes, what economic benefits (10% NPC discount decided)
- Guard/vendor system or simpler approach

---

## 3. Altar/Sigil Control Mechanics

### ServUO Implementation

**File Locations:**
- `Items\Sigil.cs` (lines 1-545)
- `Core\Faction.cs` (lines 973-1021)

**Monolith Types (Altar Equivalents):**
```
BaseMonolith - Abstract base class
├── TownMonolith - Return sigil here to capture town
└── StrongholdMonolith - Corruption altar at stronghold
```

**Sigil Corruption Timing:**
```csharp
public static readonly TimeSpan CorruptionGrace = TimeSpan.FromMinutes((Core.SE) ? 30.0 : 15.0);
public static readonly TimeSpan CorruptionPeriod = ((Core.SE) ? TimeSpan.FromHours(10.0) : TimeSpan.FromHours(24.0));
public static readonly TimeSpan ReturnPeriod = TimeSpan.FromHours(1.0);
public static readonly TimeSpan PurificationPeriod = TimeSpan.FromDays(3.0);
```

| Timer | SE Expansion | Pre-SE |
|-------|--------------|--------|
| Grace Period | 30 minutes | 15 minutes |
| Corruption Period | 10 hours | 24 hours |
| Return Period | 1 hour | 1 hour |
| Purification | 3 days | 3 days |

**ProcessTick() System (30-second intervals):**
1. Grace Period Logic: If sigil moved during grace window, reset corruption timer
2. Corruption Timeout: After CorruptionPeriod, mark as corrupted
3. Return Period: Corrupted sigil must return to town within 1 hour or resets
4. Purification: 3-day lock before next capture possible

**Sigil State Machine:**
```
Uncorrupted Sigil
    ↓ [Placed at Stronghold]
BeingCorrupted
    ├─ GraceStart timestamp set
    ├─ If moved during grace → reset
    └─ After CorruptionPeriod → IsCorrupted = true
        ↓ [Returned to Town within ReturnPeriod]
    Town Captured
        ├─ PurificationStart timestamp
        └─ After PurificationPeriod (3 days) → can be captured again
```

**Key State Variables:**
- `m_Corrupted` - Faction that owns the town
- `m_Corrupting` - Faction currently corrupting
- `m_CorruptionStart` - When corruption began
- `m_PurificationStart` - When purification began
- `m_GraceStart` - Grace period timestamp

### Our 51alpha Design (Different)

We use **2-hour siege battles** with:
- Scheduled siege times
- Point-based objectives (not sigil corruption)
- Immediate ownership transfer on victory
- No purification lockout

---

## 4. Town Control - Capture & Hold

### ServUO Capture Sequence

1. **Sigil Theft:** Player from attacking faction steals sigil from town
2. **Transport to Stronghold:** Bring sigil to enemy faction stronghold monolith
3. **Corruption Phase:** Hold sigil at stronghold for 10-24 hours
4. **Grace Period:** Can be interrupted if moved during grace window
5. **Return to Town:** Take corrupted sigil back to town monolith within 1 hour
6. **Ownership Transfer:**
   ```csharp
   this.m_Town.Capture(this.m_Corrupted);
   ```
7. **Purification Lock:** Town locked for 3 days before re-capture

**Town Capture Method:**
```csharp
public void Capture(Faction f) {
    if (this.m_State.Owner == f) return;

    if (this.m_State.Owner == null) {  // Unowned to owned
        this.LastIncome = DateTime.UtcNow;
        f.Silver += SilverCaptureBonus;  // 10,000 silver
    } else {  // Changing hands
        f.Silver += SilverCaptureBonus;
    }

    this.m_State.Owner = f;
    this.Sheriff = null;  // Clear officials
    this.Finance = null;

    // Delete all guards and vendors
}
```

**Town Control Benefits:**
- Daily income (10,000 base silver/day)
- NPC recruitment slots (guards & merchants)
- Tax rate control
- Finance/Sheriff leadership roles

### Our 51alpha Design

| Aspect | Our Design |
|--------|------------|
| Capture Method | Win 2-hour siege battle |
| Persistence | Yes, until next siege |
| Benefits | 10% NPC discount, faction banners |
| Lockout | None (sieges on schedule) |

---

## 5. Faction Rewards System

### ServUO Implementation

**Silver Currency:**
- Item ID: `0xEF0`
- Stackable, light weight (0.02 per piece)
- Distinct from gold

**Silver Earning:**
- Town income: 10,000 base silver/day
- Capture bonus: 10,000 silver
- Kill rewards: 40 × (victimPoints / 10) silver (capped at 40 × 40)

**Faction Equipment:**
- 20+ pre-defined faction artifacts with bonuses
- Examples:
  - PrimerOnArmsTalisman (10% attack)
  - ClaininsSpellbook (10% lower mana)
  - CrimsonCincture (+10 dex)
  - FeyLeggings (+3 all resists)
  - HeartOfTheLion (+5 all resists)

**Rank System:**
- Ranked by KillPoints within faction
- Max wearables increase with rank:
  - Commanders: 9 faction items
  - Regular members: Based on rank tier
- 21-day expiration on equipped faction items

**Stronghold Rune:**
- Teleportation to faction stronghold
- 30-minute cooldown (reduced by rank)
- Single use for non-admins

### Our 51alpha Design

We use **Siege Coins** currency:
- Earned from siege participation
- Spent on jewelry, clothing, mounts
- Different from gold/silver economy

---

## 6. Guard System

### ServUO Implementation

**Guard Types (10):**
| Type | Role |
|------|------|
| Berserker | Melee DPS |
| DeathKnight | Dark melee |
| Dragoon | Mounted |
| Henchman | Basic guard |
| Knight | Tank |
| Mercenary | Balanced |
| Necromancer | Magic |
| Paladin | Holy tank |
| Sorceress | Magic DPS |
| Wizard | Arcane |

**Guard Management:**
- Sheriff purchases via SheriffGump
- Each has purchase price + daily upkeep
- Guards defend town and stronghold
- Auto-deleted if town goes bankrupt

### Our 51alpha Consideration

- Do we want NPC guards during sieges?
- Or purely player-vs-player combat?

---

## 7. Trap System

### ServUO Implementation

**Trap Types:**
- ExplosionTrap
- GasTrap
- SawTrap
- SpikeTrap
- RemovalKit (to remove enemy traps)

**Limits:**
- Maximum 15 traps per faction
- Placed in controlled territories

### Our 51alpha Consideration

- Traps could add tactical depth to sieges
- Consider for future implementation

---

## 8. Key Files Reference

**Core Logic:**
```
Scripts/Services/Factions/Core/
├── Town.cs          - Town ownership, capture, finances
├── Faction.cs       - Faction scoring, death handling
├── PlayerState.cs   - Player rank/kill points
├── TownState.cs     - Town persistence
└── FactionItem.cs   - Equipment binding
```

**Items:**
```
Scripts/Services/Factions/Items/
├── Sigil.cs           - Town capture mechanic
├── Silver.cs          - Currency
└── Equipment/         - Faction gear
    ├── StrongholdRune.cs
    └── FactionEquipment.cs
```

**UI:**
```
Scripts/Services/Factions/Gumps/
├── FinanceGump.cs   - Town budget/vendor management
└── SheriffGump.cs   - Guard recruitment
```

**NPCs:**
```
Scripts/Services/Factions/Mobiles/
├── Guards/          - 10 guard types
└── Vendors/         - 5 vendor types
```

---

## 9. Implementation Notes for 51alpha
Investigate : https://uo.com/wiki/ultima-online-wiki/combat/virtue-versus-vice/
We want to implement the Alter Capture system for the town seiges.
This is what is being faught over and after x time or x points it finished.
Instead of guild focused it is faction focused.

### What We're Keeping (Conceptually)
- Town control benefits (10% discount, banners)
- Multiple factions competing
- Territory-based gameplay


### What We're Changing
- **Scoring:** Point-based siege battles vs kill-point ranking
- **Timing:** 2-hour scheduled battles vs persistent sigil corruption
- **Currency:** Siege Coins vs Silver
- **Capture:** Win battle vs sigil corruption
- **Lockout:** None vs 3-day purification

### Questions Resolved
1. ✓ Siege cycle timing: 2-hour battles
2. ✓ Town benefits: 10% NPC discount, banners
3. ✓ Victory condition: 10,000 points

### Questions Still Open
1. Do we want faction guards/vendors?
No
2. Trap system for sieges?
Yes
3. Leadership roles (Sheriff/Finance equivalent)?
No

---

## 10. Database Tables (ServUO Reference)

ServUO uses in-memory persistence with binary serialization. For our SQLite implementation:

```sql
-- Suggested tables based on ServUO structure
CREATE TABLE s51a_towns (
    town_id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    owner_faction_id INTEGER,
    last_siege_time DATETIME,
    silver_treasury INTEGER DEFAULT 0
);

CREATE TABLE s51a_siege_history (
    siege_id INTEGER PRIMARY KEY,
    town_id INTEGER NOT NULL,
    start_time DATETIME NOT NULL,
    end_time DATETIME,
    winner_faction_id INTEGER,
    final_scores TEXT  -- JSON array of faction scores
);

CREATE TABLE s51a_siege_participants (
    siege_id INTEGER NOT NULL,
    character_id INTEGER NOT NULL,
    faction_id INTEGER NOT NULL,
    points_earned INTEGER DEFAULT 0,
    kills INTEGER DEFAULT 0,
    deaths INTEGER DEFAULT 0,
    objectives_completed INTEGER DEFAULT 0,
    PRIMARY KEY (siege_id, character_id)
);
```

---

*This document serves as reference material. Our actual implementation will follow the designs in Core/DESIGN_DECISIONS.md and Systems/Factions/*.md*
