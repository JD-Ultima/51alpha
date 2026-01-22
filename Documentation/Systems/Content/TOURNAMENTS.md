# Tournament System

**Authority**: TIER 2 - Implementation Spec
**Related**: `Progression/GLICKO_RATINGS.md`, `Combat/PVP_COMBAT.md`

---

## Overview

Scheduled PvP tournaments with cosmetic rewards. Single elimination format with parallel arena matches.

---

## Schedule

| Day | Times (EST) |
|-----|-------------|
| Wednesday | 2 PM, 8 PM |
| Saturday | 2 PM, 8 PM |
| Sunday | 2 PM, 8 PM |

---

## Parameters

| Parameter | Value |
|-----------|-------|
| Registration | 15 minutes |
| Minimum Players | 6 |
| Match Duration | 5 minutes |
| Prep Time | 30 seconds |

---

## Format

### Structure

- Single elimination bracket
- Random seeding (no Glicko weight)
- Powers of 2 brackets (byes for odd counts)
- Best of 1 per round

### Match Flow

1. Registration period (15 min)
2. Bracket generation
3. Prep time (30 sec)
4. Combat (5 min max)
5. Winner advances
6. Repeat until champion

---

## Arenas

8 identical arenas for parallel matches:

| Feature | Specification |
|---------|---------------|
| Size | 20x20 tiles |
| Terrain | Flat, no obstacles |
| Resurrection | Shrine outside arena |
| Spectators | Balcony above |

---

## Rewards

### Tournament Coins

| Placement | Coins |
|-----------|-------|
| Participation | 50 |
| Per Win | 100 |
| Champion | 500 bonus |
| Runner-up | 250 bonus |

### Glicko Impact

| Match Type | Weight |
|------------|--------|
| Tournament | 1.5x |

Tournament matches count 50% more toward rating.

---

## Champion Perks

| Perk | Duration |
|------|----------|
| Title: "Champion" | 7 days |
| Trophy Item | Permanent |
| Leaderboard entry | Permanent |

---

## Tournament Coin Vendor

Cosmetic items only:

| Category | Examples |
|----------|----------|
| Dyes | Unique colors |
| Titles | "Gladiator", "Duelist" |
| Furniture | Trophies, banners |
| Emotes | Victory animations |

---

## Registration

### How to Register

1. Approach Tournament Stone
2. Use `[tournament register]`
3. Wait for bracket announcement
4. Teleported when match starts

### Disqualification

- Leaving arena bounds
- Logging out
- Timeout (5 min match limit)
- Using prohibited items

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_tournaments | Tournament events |
| s51a_tournament_participants | Registrations |
| s51a_tournament_matches | Match results |
| s51a_tournament_champions | Champion history |
| s51a_tournament_coins | Player balances |

---

## Configuration

```json
{
  "tournament.min_players": 6,
  "tournament.registration_minutes": 15,
  "tournament.match_duration_seconds": 300,
  "tournament.prep_seconds": 30,
  "tournament.coin_participation": 50,
  "tournament.coin_per_win": 100,
  "tournament.coin_champion": 500,
  "tournament.coin_runnerup": 250,
  "tournament.glicko_weight": 1.5
}
```
