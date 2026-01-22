# Glicko-2 Rating System

**Authority**: TIER 2 - Implementation Spec
**Related**: `Factions/SIEGE_BATTLES.md`, `Combat/PVP_COMBAT.md`

---

## Overview

Glicko-2 provides accurate PvP skill measurement with confidence intervals. Used for matchmaking, leaderboards, and siege scoring.

---

## Rating Parameters

| Parameter | Default | Range | Description |
|-----------|---------|-------|-------------|
| Rating (μ) | 1500 | 100-4000 | Skill estimate |
| Deviation (RD) | 350 | 30-500 | Uncertainty |
| Volatility (σ) | 0.06 | 0.01-0.15 | Rating stability |
| Tau (τ) | 0.5 | System constant | Volatility change rate |

---

## Rating Meaning

| Rating | Skill Level |
|--------|-------------|
| < 1200 | Beginner |
| 1200-1400 | Below Average |
| 1400-1600 | Average |
| 1600-1800 | Above Average |
| 1800-2000 | Expert |
| > 2000 | Elite |

---

## Rating Period

- **Duration**: 3 days
- **Process**: Matches accumulate during period
- **Calculation**: Batch update at period end
- **Purpose**: Prevents rapid manipulation

### Why Batch Updates?

- More accurate skill estimation
- Prevents farming/manipulation
- Smoother rating progression
- Reduces server load

---

## Match Types

| Type | Weight | Notes |
|------|--------|-------|
| Siege Kill | 1.0 | Standard weight |
| Tournament | 1.5 | Higher stakes |
| Duel | 0.5 | Optional, lower stakes |

---

## Siege Point Weighting

Kill points in sieges are weighted by Glicko rating:

```
weight = 0.5 + (victim_rating / killer_rating) * 0.5
```

**Capped**: 0.5 to 2.0

### Examples

| Killer Rating | Victim Rating | Weight | Points (base 100) |
|---------------|---------------|--------|-------------------|
| 1500 | 1500 | 1.0 | 100 |
| 1500 | 2000 | 1.17 | 117 |
| 2000 | 1500 | 0.875 | 88 |
| 1500 | 1000 | 0.83 | 83 |
| 1000 | 2000 | 1.5 | 150 |

**Effect**: Killing higher-rated players rewards more points.

---

## Calculation Flow

### Per Rating Period

```csharp
public void ProcessRatingPeriod()
{
    foreach (var player in PlayersWithMatches)
    {
        var matches = GetMatchesThisPeriod(player);
        if (matches.Count == 0)
        {
            // Increase RD due to inactivity
            player.RD = Math.Min(player.RD * 1.05, 350);
            continue;
        }

        // Glicko-2 algorithm
        var newRating = CalculateNewRating(player, matches);
        player.Rating = newRating.Rating;
        player.RD = newRating.RD;
        player.Volatility = newRating.Volatility;
    }
}
```

### Inactivity Handling

- No matches in period: RD increases by 5%
- RD capped at 350 (starting value)
- Prevents locked-in ratings from inactive players

---

## Leaderboard Requirements

| Requirement | Value |
|-------------|-------|
| Minimum games | 10 |
| Maximum RD | 100 |
| Displayed | Top 100 |

**Provisional**: Players with RD > 100 show as "Provisional"

---

## Season Reset

At season end:
- **Rating**: Preserved (no reset)
- **RD**: Increased to 200 (more uncertainty)
- **Volatility**: Reset to 0.06

### Rationale

- Preserves skill recognition
- Allows rating shifts for returning players
- Encourages continued play

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_glicko_ratings | Current player ratings |
| s51a_glicko_matches | Match history |
| s51a_glicko_history | Rating snapshots |
| s51a_pvp_stats | Lifetime statistics |

---

## Configuration

```json
{
  "glicko.default_rating": 1500,
  "glicko.default_rd": 350,
  "glicko.default_volatility": 0.06,
  "glicko.tau": 0.5,
  "glicko.rating_period_days": 3,
  "glicko.min_games_leaderboard": 10,
  "glicko.max_rd_leaderboard": 100,
  "glicko.season_reset_rd": 200
}
```

---

## Glicko-2 Algorithm Reference

### Step 1: Convert to Glicko-2 Scale

```
μ = (Rating - 1500) / 173.7178
φ = RD / 173.7178
```

### Step 2: Calculate Expected Outcome

```
g(φ) = 1 / sqrt(1 + 3φ²/π²)
E(μ, μj, φj) = 1 / (1 + exp(-g(φj)(μ - μj)))
```

### Step 3: Compute Variance

```
v = [Σ g(φj)² × E × (1 - E)]⁻¹
```

### Step 4: Compute Delta

```
Δ = v × Σ g(φj) × (sj - E)
```

### Step 5: Update Volatility

Iterative algorithm to find new σ.

### Step 6: Update Rating and RD

```
φ' = 1 / sqrt(1/φ² + 1/v)
μ' = μ + φ'² × Σ g(φj) × (sj - E)
```

### Step 7: Convert Back

```
Rating' = 173.7178 × μ' + 1500
RD' = 173.7178 × φ'
```

---

## FAQ

**Q: Why Glicko-2 over Elo?**
A: Glicko-2 accounts for rating uncertainty (RD) and volatility, providing more accurate skill estimation.

**Q: Why batch updates?**
A: Single-match updates can be manipulated. Batch updates over 3 days provide more stable, accurate ratings.

**Q: Does losing to lower-rated players hurt more?**
A: Yes, the expected outcome is higher, so unexpected losses have larger impact.

**Q: Can I see my exact rating?**
A: Yes, ratings are visible. RD indicates confidence - lower RD means more certain rating.
