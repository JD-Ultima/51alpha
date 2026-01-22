# Faction Overview

**Authority**: TIER 2 - Implementation Spec
**Authoritative Source**: `Core/DESIGN_DECISIONS.md` §1
**Related**: `SIEGE_BATTLES.md`, `SIEGE_REWARDS.md`

---

## Overview

Three-faction guild warfare with territory control, sieges, and seasonal competition.

---

## The Three Factions

| ID | Faction | Home City | Hue | Color |
|----|---------|-----------|-----|-------|
| 1 | The Golden Shield | Trinsic | 2721 | Gold |
| 2 | The Bridgefolk | Vesper | 2784 | Blue |
| 3 | The Lycaeum Order | Moonglow | 2602 | Purple |

---

## Guild-Based Membership

**Core Rule**: Only guilds can join factions. Solo players can create 1-member guilds to participate.

| Requirement  | Details                                                     |
|--------------|-------------------------------------------------------------|
| Membership   | Must be in a guild (solo players may create 1-member guilds)|
| Assignment   | Guild leader chooses faction                                |
| Inheritance  | All members inherit faction                                 |
| Cooldown     | 7 days between faction changes                              |

---

## Member Types & In-Town PvP Flagging

This system mimics the UO **Chaos/Order** in-town flagging mechanics:

| Type      | In Town Attack | In Town Be Attacked | Outside Town | Earns Points |
|-----------|----------------|---------------------|--------------|--------------|
| Combatant | Enemy factions | By enemy factions   | Anyone       | Yes          |
| Peaceful  | No             | No                  | Anyone*      | No           |

**\*Outside guarded towns**: All players can attack and be attacked freely (standard Felucca rules).

### In-Town Flagging (Chaos/Order Style)

- Faction members are colored based on their faction (green = ally, orange = enemy)
- Enemy faction members can be freely attacked **inside guarded towns** without guard interference
- This allows faction warfare within town limits
- Non-faction players are NOT attackable by faction members in town

### Peaceful Members

Peaceful members can:
- Craft/supply faction
- Attend events as spectators
- Change to Combatant (24hr cooldown)

### Forced Combatant Flag

- Peaceful members who attack another player are **auto-flagged as Combatant** for 24 hours
- This prevents "peaceful" members from ganking and immediately returning to safety
- After 24 hours, they can choose to remain Combatant or return to Peaceful status

---

## Currency System

### Faction Points (Account-Bound)
- Earned from: Sieges, quests, bounties, tournaments
- Spent on: Faction rewards, seasonal rankings
- Reset: Soft reset each season (keep 10%)

### Siege Coins (Performance-Based)
- **Details**: See `SIEGE_REWARDS.md`
- Earned from: Siege participation based on contribution scoring
- Spent on: Temporary stat boosts, faction gear, mounts
- Reset: Full wipe each season

### Silver (Character-Bound)
- Earned from: Bounties, quests, VvV kills
- Spent on: Consumables, cosmetics, gambling
- Tradeable: Yes (in-game trading)

---

## Territory Control

### Siege Cities

| City       | Region  |
|------------|---------|
| Jhelom     | Felucca |
| Skara Brae | Felucca |
| Yew        | Felucca |
| Trinsic    | Felucca |

### Control Benefits

| Cities Controlled  | NPC Discount |
|--------------------|--------------|
| 1-3                | 10%          |
| 4 (All)            | 15%          |

Benefits apply at faction NPCs in controlled cities.

---

## Seasonal Competition

### Season Structure

| Duration | 90 days (quarterly) |
|----------|---------------------|
| Q1 | Jan 1 - Mar 31 |
| Q2 | Apr 1 - Jun 30 |
| Q3 | Jul 1 - Sep 30 |
| Q4 | Oct 1 - Dec 31 |

### Season End

1. Faction rankings finalized
2. Rewards distributed
3. Glicko ratings soft reset (RD increases)

### Underdog System

Trailing factions receive point bonuses:

| Position | Bonus |
|----------|-------|
| 1st      | 0%    |
| 2nd      | +2%   |
| 3rd      | +5%   |

Calculated weekly based on total faction points.

---

## Gumps

### Faction Status (`[factionstatus]`)
- Current faction and standing
- Personal stats (kills, deaths, points)
- Guild faction info

### Leaderboard
- Top 100 by Faction Points
- Top 100 by Glicko Rating
- Faction totals

---

## Database Tables

| Table                     | Purpose            |
|---------------------------|-------------------|
| s51a_factions             | Faction definitions |
| s51a_seasons              | Season tracking    |
| s51a_guild_factions       | Guild → faction mapping |
| s51a_player_combat_status | Combatant/peaceful |
| s51a_faction_points       | Point balances     |
| s51a_town_control         | City ownership     |

---

## Configuration

```json
{
  "faction.change_cooldown_days": 7,
  "town.single_city_discount": 0.10,
  "town.all_cities_discount": 0.15
}
```
