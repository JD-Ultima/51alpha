# Siege Battles

**Authority**: TIER 2 - Implementation Spec
**Authoritative Source**: `Core/DESIGN_DECISIONS.md` §9
**Related**: `FACTION_OVERVIEW.md`, `SIEGE_REWARDS.md`

---

## Overview

Three-way faction battles over contested cities. Hourly rotation between siege towns.

---

## Battle Parameters

| Parameter      | Value                                      |
|----------------|--------------------------------------------|
| Duration       | 30 minutes                                 |
| Victory Score  | 10,000 points                              |
| Min Players    | 10 total (3+ per faction recommended)      |
| Global Cooldown | 30 minutes between any siege              |
| City Cooldown  | 2 hours per city                           |

---

## Triggers

Sieges start when:
1. Minimum player threshold met in siege city
2. City cooldown expired
3. Global cooldown expired
4. At least 2 factions represented

---

## Scoring

| Action                  | Points  | Silver |
|-------------------------|---------|--------|
| Kill (base)             | 100     | 10     |
| Kill (Glicko weighted)  | 50-200  | 5-20   |
| Sigil Capture           | 500     | 50     |
| Altar Control (per 30s) | 25      | -      |
| Trap Kill               | 50      | 5      |
| Turret Kill             | 75      | 7      |

### Glicko Weight Formula

```
weight = 0.5 + (victim_rating / killer_rating) * 0.5
```
Capped at 0.5-2.0

---

## Victory Conditions

1. **Score Limit**: First to 10,000 points
2. **Time Expiration**: Highest score at 30 minutes
3. **Forfeit**: All enemy factions leave

---

## Objectives

### Sigil
- Single capture point in city center
- 30 seconds to capture (uncontested)
- 45 seconds if contested (multiple factions)
- 500 points on capture

### Altars (3 per city)
- Control by proximity (most faction members nearby)
- 25 points per 30 seconds of control
- Enable resurrection at controlled altars

### Priests
- One per faction at spawn area
- Resurrect dead faction members
- Cannot be killed (non-targetable)

---

## Defenses

Each faction has a defense budget (5,000 silver default) per siege:

| Defense        | Cost | Effect                  |
|----------------|------|-------------------------|
| Alarm Trap     | 100  | Reveals hidden enemies  |
| Snare Trap     | 200  | Roots target 3s         |
| Smoke Trap     | 300  | AoE blind 5s            |
| Mana Drain Trap | 400  | Drains 50 mana          |
| Arrow Turret   | 500  | Auto-attacks enemies    |
| Magic Turret   | 750  | AoE damage              |

**Limits:**
- Traps: 10 max per faction, single-use
- Turrets: 3 max per faction, 500 HP each

---

## Glicko-2 Integration

PvP kills update Glicko ratings:

```csharp
public void ProcessKill(PlayerMobile killer, PlayerMobile victim)
{
    var killerRating = GlickoManager.GetRating(killer);
    var victimRating = GlickoManager.GetRating(victim);

    // Calculate point multiplier
    double weight = 0.5 + (victimRating.Rating / killerRating.Rating) * 0.5;
    weight = Math.Clamp(weight, 0.5, 2.0);

    int points = (int)(100 * weight);

    // Update ratings
    GlickoManager.RecordMatch(killer, victim);

    // Award points
    FactionPointManager.Award(killer, points, "Siege Kill");
}
```

### Rating Periods

- Rating period: 3 days
- Matches accumulate during period
- Batch update at period end
- Prevents farming by increasing volatility

---

## Siege Status Gump

Auto-opens during siege:
- Faction scores
- Objective states
- Time remaining
- Personal stats this siege

---

## Admin Controls

```
[siege start <city>]     - Force start siege
[siege stop]             - End active siege
[siege status]           - Show siege state
[siege toggles]          - Enable/disable sigil/altars
```

---

## Database Tables

| Table                     | Purpose            |
|---------------------------|-------------------|
| s51a_siege_battles        | Battle history     |
| s51a_siege_participants   | Per-player stats   |
| s51a_siege_kills          | Kill log           |

---

## Configuration

```json
{
  "siege.duration_minutes": 30,
  "siege.victory_score": 10000,
  "siege.global_cooldown_minutes": 30,
  "siege.city_cooldown_minutes": 120,
  "siege.min_total_players": 10,
  "siege.defense_budget_max": 5000,
  "siege.sigil_enabled": true,
  "siege.altars_enabled": true
}
```
