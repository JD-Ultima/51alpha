# Daily Content

**Authority**: TIER 2 - Implementation Spec
**Authoritative Source**: `Core/DESIGN_DECISIONS.md` §11
**Related**: `DUNGEONS.md`, `Factions/FACTION_OVERVIEW.md`

---

## Overview

Daily content provides repeatable objectives with meaningful rewards. Refreshes daily at midnight EST (5 AM UTC).

---

## Daily Bounties

### Overview

Three random tasks refresh daily. Available to all players regardless of faction.

### Bounty Types

| Type | Examples | Difficulty |
|------|----------|------------|
| Monster | Kill 20 Skeletons | Easy |
| Monster | Kill 5 Drakes | Hard |
| Resource | Mine 100 Iron Ore | Easy |
| Resource | Mine 25 Valorite Ore | Hard |
| Activity | Craft 50 Bandages | Easy |
| Activity | Craft 20 Potions | Medium |

### Rewards

| Difficulty | Faction Points | Silver |
|------------|----------------|--------|
| Easy | 100 | 25 |
| Medium | 150 | 50 |
| Hard | 200-250 | 75-100 |

### Tracking

Progress tracked automatically:
- Monster kills: Credited on death
- Resources: Credited on harvest
- Crafts: Credited on completion

Claim rewards at **Bounty Board NPC**.

---

## Faction Quest (Daily Boss)

### Overview

Daily boss spawn in rotating location. All factions compete for kill credit.

### Locations

| Region | Location |
|--------|----------|
| Swamp | Fens of the Dead |
| Swamp | Bog of Desolation |
| Desert | Scorched Sands |
| Desert | Sun Temple Ruins |

Rotation: Random selection daily.

### Boss Scaling

| Parameter | Value |
|-----------|-------|
| Base HP | 5,000 |
| HP per player | +500 |
| Maximum HP | 25,000 |
| Respawn | 2 hours after kill |

### Rewards

| Reward | Amount | Recipient |
|--------|--------|-----------|
| Faction Points | 500 | All participants |
| Bonus FP | 250 | Top damage dealer |
| Common Relic | 15% | Random participant |
| Uncommon Relic | 5% | Random participant |
| Rare Relic | 1% | Random participant |

**Participation requirement**: Minimum 1,000 damage dealt.

---

## Town Cryer Announcements

### Delivery Method
**Screen-based**: Messages appear within 15 tiles of Town Cryer location

### Locations
- Britain (near bank)
- 3 Faction Towns (near banks):
  - Yew
  - Skara Brae
  - Jhelom

### Announcement Types

| Type | Trigger |
|------|---------|
| Dungeon Bonus | Weekly rotation |
| Siege Starting | Battle begins |
| Tournament | Registration opens |
| Faction Quest | Boss spawns |
| Server Event | Admin triggered |

### Parameters

| Parameter | Value |
|-----------|-------|
| Range | 15 tiles |
| Cooldown | 10 minutes per player |
| Overlap | Prevented (one at a time) |

---

## Reset Schedule

| Content | Reset Time | Timezone |
|---------|------------|----------|
| Daily Bounties | 12:00 AM | EST |
| Faction Quest | 12:00 AM | EST |
| Weekly Bonus | Monday 12:00 AM | EST |

UTC equivalent: 5 AM UTC

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_bounty_definitions | Bounty task pool |
| s51a_daily_bounties | Daily selection |
| s51a_player_bounty_progress | Player tracking |
| s51a_faction_quest_locations | Quest spawn points |
| s51a_faction_quest_schedule | Daily location |
| s51a_faction_quest_completions | Completion log |

---

## Configuration

```json
{
  "bounty.reset_hour_utc": 5,
  "bounty.tasks_per_day": 3,

  "factionquest.boss_base_hp": 5000,
  "factionquest.boss_hp_per_player": 500,
  "factionquest.boss_max_hp": 25000,
  "factionquest.respawn_hours": 2,
  "factionquest.reward_faction_points": 500,
  "factionquest.relic_common_chance": 0.15,
  "factionquest.relic_uncommon_chance": 0.05,
  "factionquest.relic_rare_chance": 0.01
}
```
