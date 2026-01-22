# Virtue System Reference

**Authority**: REFERENCE - For Game Mechanic Design
**Purpose**: Document the 8 Virtues system and its integration with other systems
**Related**: `CHAMPION_SPAWNS.md`, `TALISMAN_TIER_DESIGN.md`, `PVP_DETECTION_IMPLEMENTATION.md`

---

## Sphere 51a Custom Rules

### Critical Rule: Virtues are PvM ONLY

**ALL Virtue abilities and bonuses are DISABLED when PvP flagged.**

This includes:
- Honor (Perfection damage bonus, monster non-aggression)
- Sacrifice (self-resurrection)
- Compassion (HP restoration boost)
- Justice (relic/siege coin bonus - repurposed)
- Valor (champion spawn activation)
- Spirituality (enhanced healing)
- Humility (content access)
- Honesty (merchant discounts)

**Implementation**: Use `S51aPvPCombatCheck.IsInPlayerCombat(mobile)` to disable virtue effects.

---

## The 8 Virtues Overview

| Virtue | How to Gain | Standard Benefit | Sphere 51a Modification |
|--------|-------------|------------------|------------------------|
| **Honor** | Honor then kill monsters at full HP | Perfection bonus, mob non-aggression | Keep as-is (PvM only) |
| **Justice** | Kill red/murderer players | Extra power scrolls | **Repurposed**: Extra relics + siege coins + Extra runic hammer |
| **Valor** | Kill champion spawn creatures | Activate/advance champion spawns | Keep as-is |
| **Sacrifice** | Donate fame | Self-resurrection | Keep (PvM only) |
| **Compassion** | Escort NPCs, resurrect others | HP restoration boost | Keep (PvM only) |
| **Spirituality** | Heal/resurrect other players | Enhanced healing | Keep (PvM only) |
| **Humility** | Kill evil-aligned monsters | Access to Humility Hunt | Keep if content exists |
| **Honesty** | Return "lost" items | NPC merchant discount | Keep (QoL) |

---

## Virtue Levels

Each virtue has 4 levels based on accumulated points:

| Level | Title | Points Required |
|-------|-------|-----------------|
| 0 | None | 0-3,999 |
| 1 | Seeker | 4,000-9,999 |
| 2 | Follower | 10,000-20,999 |
| 3 | Knight | 21,000+ (max varies by virtue) |

**Max Points by Virtue:**
- Honor: 20,000
- Sacrifice: 22,000
- All Others: 21,000

---

## Honor Virtue (Critical for Dexer/Sampire)

### How It Works

1. Player opens Virtue menu (double-click paperdoll symbol)
2. Target self OR a monster at full HP
3. If targeting **self**: Grants 25% damage bonus for short duration, monster non-aggression
4. If targeting **monster**: Initiates Honor/Perfection system

### Honor + Bushido Perfection

**This is CRITICAL for Dexer and Sampire builds.**

When a player with 50+ Bushido "honors" a monster:
- `HonorContext` is created between player and monster
- **Perfection meter** starts tracking consecutive hits
- Each consecutive hit increases Perfection
- Missing or switching targets resets Perfection
- At max Perfection: **+100% bonus damage** (with T3 talisman)

```
Honor Target (Virtue) + Bushido 50+ → Perfection System Active
                                    ↓
                        Consecutive Hits Build Damage Bonus
                                    ↓
                        Max Perfection = 100% bonus (T3)
```

### Tiered Perfection (Talisman Integration)

Per our talisman design, Perfection max bonus scales with tier:

| Tier | Max Perfection Bonus |
|------|---------------------|
| T1 | +25% |
| T2 | +50% |
| T3 | +100% (full) |

### Honor Duration by Level

| Level | Self-Buff Duration | Usage Cooldown |
|-------|-------------------|----------------|
| Seeker | 30 seconds | 5 minutes |
| Follower | 90 seconds | 5 minutes |
| Knight | 300 seconds | 5 minutes |

### Gaining Honor

- Honor a monster at FULL HP, then kill it
- Staying close to target grants more honor
- Letting target strike first grants more honor
- Using ranged attacks grants less honor

### Code Location

`Projects/UOContent/Engines/Virtues/Honor.cs`
`Projects/UOContent/Engines/Virtues/HonorContext.cs`

---

## Justice Virtue (REPURPOSED)

### Standard UO Function (REMOVED)

In standard UO, Justice provides extra power scrolls at champion spawns:
- Seeker: 60% chance of duplicate scroll
- Follower: 80% chance
- Knight: 100% chance

**Since we have no power scrolls, this is removed.**

### Sphere 51a Repurposed Function

Justice now provides:

| Level | Relic Drop Bonus | Siege Coin Bonus |
|-------|-----------------|------------------|
| Seeker | +5% from bosses | +5% per siege |
| Follower | +10% from bosses | +10% per siege |
| Knight | +15% from bosses | +15% per siege |

### Gaining Justice

- Kill a red (murderer) player in Felucca
- 5-minute cooldown between gains
- ~3 points per skill-capped murderer kill

### Code Location

`Projects/UOContent/Engines/Virtues/Justice.cs`

**Requires modification** to replace power scroll bonus with relic/siege coin/runic hammer bonus.

---

## Valor Virtue (Champion Spawn Activation)

### Function

Valor allows players to activate and advance Champion Spawns.

| Level | Capability |
|-------|-----------|
| None | Cannot activate spawns |
| Seeker | Can add candles to altars |
| Follower | Better candle advancement |
| Knight | Can activate spawns directly |

### Gaining Valor

- Kill creatures at Champion Spawns
- Defeating a champion grants significant Valor to all participants
- Can gain at Felucca, Ilshenar, and Tokuno spawns

### Code Location

`Projects/UOContent/Engines/Virtues/Valor.cs`

---

## Sacrifice Virtue (Self-Resurrection)

### Function

Sacrifice allows players to resurrect themselves.

| Level | Resurrection HP | Uses |
|-------|----------------|------|
| Seeker | Low HP | Limited |
| Follower | Medium HP | Moderate |
| Knight | High HP | Generous |

**PvM Only**: Cannot self-resurrect while PvP flagged.

### Gaining Sacrifice

- Donate fame by saying "I sacrifice" at shrine

### Code Location

`Projects/UOContent/Engines/Virtues/Sacrifice.cs`

---

## Compassion Virtue (Healing Boost)

### Function

Compassion enhances HP restoration when being resurrected.

| Level | HP on Resurrection |
|-------|-------------------|
| Seeker | ~40% |
| Follower | ~60% |
| Knight | ~80% |

### Gaining Compassion

- Complete NPC escort quests
- Resurrect other players
- Limited to 5 gains per day

### Code Location

`Projects/UOContent/Engines/Virtues/Compassion.cs`

---

## Spirituality Virtue (Enhanced Healing)

### Function

Spirituality enhances healing abilities when healing other players.

### Gaining Spirituality

- Use targeted heals/resurrections on other players

### Code Location

Not fully implemented in standard ModernUO - may need custom implementation.

---

## Humility Virtue (Content Access)

### Function

Humility provides access to the Humility Hunt content.

### Gaining Humility

- Kill evil-aligned monsters

### Code Location

`Projects/UOContent/Engines/Virtues/` (if implemented)

---

## Honesty Virtue (Merchant Discounts)

### Function

Honesty provides discounts from NPC merchants.

### Gaining Honesty

- Find and return "lost" items to NPCs

---

## Implementation: PvP Disable Check

### Core Check Method

Add to virtue ability usage:

```csharp
// In each virtue ability activation
if (S51aPvPCombatCheck.IsInPlayerCombat(mobile))
{
    mobile.SendMessage("You cannot use virtue abilities while in PvP combat.");
    return;
}
```

### Files to Modify

| File | Modification |
|------|--------------|
| `Honor.cs` | Check before EmbraceHonor, Honor target |
| `Sacrifice.cs` | Check before self-resurrection |
| `Compassion.cs` | Check before HP boost on resurrection |
| `Justice.cs` | Repurpose power scroll bonus to relics/siege coins |
| `HonorContext.cs` | Disable Perfection damage bonus in PvP |

### Perfection PvP Check

```csharp
// In damage calculation when Perfection is active
public double GetPerfectionDamageBonus(Mobile attacker, Mobile defender)
{
    // Disable Perfection bonus in PvP
    if (S51aPvPCombatCheck.IsPlayerTarget(defender))
    {
        return 0.0;
    }

    // Calculate normal Perfection bonus
    // Apply talisman tier cap (T1=25%, T2=50%, T3=100%)
    return CalculatePerfectionBonus(attacker);
}
```

---

## Virtue System Files in ModernUO

| File | Purpose |
|------|---------|
| `VirtueSystem.cs` | Core virtue persistence and tracking |
| `VirtueContext.cs` | Per-player virtue data |
| `VirtueGump.cs` | Virtue menu UI |
| `Honor.cs` | Honor virtue logic |
| `HonorContext.cs` | Honor/Perfection tracking |
| `Justice.cs` | Justice virtue logic |
| `Valor.cs` | Valor virtue logic |
| `Sacrifice.cs` | Sacrifice virtue logic |
| `Compassion.cs` | Compassion virtue logic |

---

## Configuration

```json
{
  "virtues.enabled": true,
  "virtues.pvp_disabled": true,
  "virtues.honor.perfection_t1_cap": 0.25,
  "virtues.honor.perfection_t2_cap": 0.50,
  "virtues.honor.perfection_t3_cap": 1.00,
  "virtues.justice.relic_bonus_seeker": 0.05,
  "virtues.justice.relic_bonus_follower": 0.10,
  "virtues.justice.relic_bonus_knight": 0.15,
  "virtues.justice.siege_coin_seeker": 5,
  "virtues.justice.siege_coin_follower": 10,
  "virtues.justice.siege_coin_knight": 15
}
```

---

## Integration with Other Systems

### Talisman System

- Honor/Perfection requires Dexer or Sampire talisman (for Bushido access)
- Perfection max bonus capped by talisman tier
- Talismans unequip on PvP, which also disables Perfection

### PvP Detection

- All virtue abilities use `S51aPvPCombatCheck.IsInPlayerCombat()`
- 2-minute window after PvP combat before abilities re-enable

### Champion Spawns

- Valor virtue tied to champion spawn activation
- Justice repurposed for relic/siege coin bonuses from champion kills

---

## Summary: What's PvM Only

| Virtue | Benefit | PvM Only? |
|--------|---------|-----------|
| Honor | Perfection damage bonus | YES |
| Honor | Monster non-aggression | YES |
| Sacrifice | Self-resurrection | YES |
| Compassion | HP restoration boost | YES |
| Spirituality | Enhanced healing | YES |
| Justice | Relic/siege coin bonus | YES |
| Valor | Spawn activation | N/A (PvM content) |
| Humility | Content access | N/A (PvM content) |
| Honesty | Merchant discount | NO (QoL, always active) |

---

*This document defines the Virtue system for Sphere 51a. Update DESIGN_DECISIONS.md after implementation.*
