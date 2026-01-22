# Currencies & Economy

**Authority**: TIER 2 - Implementation Spec
**Authoritative Source**: `Core/DESIGN_DECISIONS.md` §8, §11
**Related**: `Factions/SIEGE_REWARDS.md`, `Crafting/DUNGEON_COOKING.md`

---

## Overview

Multiple currency systems serve different purposes. Gold is the universal currency, while specialized currencies target specific content.

---

## Currency Types

### Gold (Universal)

| Property | Value |
|----------|-------|
| Type | Universal currency |
| Tradable | Yes |
| Sources | Monsters, quests, trade |
| Cap | None |

**Primary Uses:**
- NPC vendor purchases
- Player trading
- Repair costs
- Housing
- Skill training

### Silver (Daily Content)

| Property | Value |
|----------|-------|
| Type | Daily reward currency |
| Tradable | No (character-bound) |
| Sources | Daily bounties, sieges |
| Reset | Seasonal |

**Primary Uses:**
- Daily content vendors
- Minor consumables
- Quality-of-life items

### Siege Coins (Faction PvP)

| Property | Value |
|----------|-------|
| Type | PvP reward currency |
| Tradable | No (character-bound) |
| Sources | Siege participation |
| Reset | Seasonal |

**Primary Uses:**
- Siege jewelry (stat boosts)
- Siege clothing (AR boosts)
- Faction mounts
- Faction banners

See `Factions/SIEGE_REWARDS.md` for details.

### Relics (Crafting)

| Property | Value |
|----------|-------|
| Type | Crafting material |
| Tradable | Yes (until used) |
| Sources | Dungeons only |
| Tiers | Common, Uncommon, Rare, Legendary |

**Primary Uses:**
- Talisman crafting
- High-end equipment

**IMPORTANT**: Relics ONLY drop from dungeons. Outside world gives gold, not relics.

---

## Gold Economy

### Sources (Inflow)

| Source | Gold/Hour Estimate |
|--------|-------------------|
| Easy mobs | 500-1,000 |
| Medium mobs | 1,000-2,500 |
| Hard mobs | 2,500-5,000 |
| Dungeon bosses | 5,000-10,000 |
| Daily quests | 1,000-3,000 |
| Player trading | Variable |

### Sinks (Outflow)

| Sink | Purpose |
|------|---------|
| Repair costs | Durability maintenance |
| NPC vendors | Consumables, reagents |
| House taxes | Property maintenance |
| Skill training | New character progression |
| Crafting failures | Material loss |
| Dungeon cooking | Ingredient consumption |

---

## Relic Economy

### Drop Rates

| Relic Tier | Drop Source | Base Rate |
|------------|-------------|-----------|
| Common | All dungeon mobs | 5-10% |
| Uncommon | Mid-tier mobs | 1-3% |
| Rare | Bosses, champs | 0.5-1% |
| Legendary | Raid bosses only | 0.1% |

### Usage Requirements

| Item | Common | Uncommon | Rare | Legendary |
|------|--------|----------|------|-----------|
| T3 Talisman | 5 | - | - | - |
| T2 Talisman | 3 | 8 | - | - |
| T1 Talisman | - | 5 | 10 | 1 |

### Trade Rules

- Tradable until consumed
- Guild cooperation required for high-tier items
- Forces dungeon participation

---

## Resource Flow

### Dungeon Catalyst Resources

Per-dungeon requirements:
- 1 fishing aspect ingredient
- 1 humanoid monster drop
- 1 environmental reagent

**Economic Impact:**
- Revives low/mid-tier mob farming
- Encourages world exploration
- Prevents dungeon self-sufficiency

### Trade Specialization

**Opportunities:**
- Recipe knowledge trading
- Ingredient specialization
- Catalyst crafting services
- Buff optimization consulting

---

## Gold Sinks

### Active Sinks

| Sink | Trigger | Amount |
|------|---------|--------|
| Repair | Equipment damage | 10-50% item value |
| Reagents | Spell casting | 5-20g per cast |
| Arrows/Bolts | Ranged combat | 1-5g per shot |
| Food/Drink | Buffs | 10-50g |
| Housing | Weekly tax | 100-1,000g |

### Passive Sinks

| Sink | Trigger | Amount |
|------|---------|--------|
| Skill training | NPC trainers | 50-500g |
| Insurance | Item protection | % of item value |
| Travel | Runebook recall | Reagent cost |

### Failure Sinks

| Sink | Trigger | Loss |
|------|---------|------|
| Crafting failure | Low skill | Materials |
| Cooking failure | Wrong recipe | Ingredients |
| Experiment | Discovery attempts | Ingredients |

---

## Seasonal Resets

At season end:
- **Gold**: Preserved
- **Silver**: Wiped
- **Siege Coins**: Wiped
- **Relics**: Preserved

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_player_gold | Gold tracking |
| s51a_player_silver | Silver balances |
| s51a_siege_coins | Siege coin balances |
| s51a_relics | Relic inventory |
| s51a_economy_logs | Transaction history |

---

## Configuration

```json
{
  "economy.gold_drop_multiplier": 1.0,
  "economy.relic_drop_multiplier": 1.0,
  "economy.repair_cost_percent": 0.10,
  "economy.house_tax_weekly": 500,
  "economy.seasonal_silver_reset": true,
  "economy.seasonal_siege_coin_reset": true
}
```

---

## Anti-Inflation Measures

### Design Principles

1. **Limited Sources**: Gold comes from finite activities
2. **Continuous Sinks**: Repairs, reagents, fees
3. **Bound Currencies**: Silver/Siege Coins untradable
4. **Seasonal Resets**: Prevent currency hoarding
5. **Relic Scarcity**: High-tier items require effort

### Monitoring

- Track gold in circulation
- Monitor inflation indicators
- Adjust drop rates dynamically
- Balance new content gold rewards
