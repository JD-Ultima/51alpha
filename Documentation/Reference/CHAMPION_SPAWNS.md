# Champion Spawns Reference

**Authority**: REFERENCE - For Content Design
**Purpose**: Document Champion Spawn system, locations, rewards, and Sphere 51a modifications
**Related**: `VIRTUE_SYSTEM.md`, `Economy/CURRENCIES.md`

---

## Overview

Champion Spawns are endgame PvM encounters where players kill progressively stronger monsters to summon and defeat a powerful champion boss. The system involves:

1. Activating a spawn altar (via Valor virtue or natural activation)
2. Killing waves of progressively harder monsters
3. Advancing through spawn levels (candles indicate progress)
4. Defeating the final Champion boss
5. Receiving rewards (artifacts, gold)

---

## Sphere 51a Modifications

### Rewards Modified

| Standard UO | Sphere 51a |
|-------------|------------|
| Power Scrolls (105, 110, 115, 120) | **REMOVED** - No power scrolls |
| Scrolls of Transcendence | Keep or remove (TBD) |
| Champion Artifacts | Keep |
| Gold (~250k) | Keep |
| Champion Skulls (for Harrower) | Keep if Harrower exists |

### Justice Virtue Integration

Justice virtue (gained by killing reds) now provides:
- Bonus artifacts from champion kills
- runic hammer (gets you a runic hammer)
- Bonus siege coins from siege events

See `VIRTUE_SYSTEM.md` for details.

---

## Champion Spawn Locations (ModernUO Default)

### Felucca Lost Lands (12 Spawns)

| # | Coordinates | Type | Eject Location |
|---|-------------|------|----------------|
| 1 | 5511, 2360 | Random | 5439, 2323 |
| 2 | 6038, 2401 | Random | 5988, 2340 |
| 3 | 5549, 2640 | Random | 5645, 2696 |
| 4 | 5636, 2916 | Random | 5721, 2949 |
| 5 | 6035, 2943 | Random | 6098, 2997 |
| 6 | 5265, 3171 | Random | 5314, 3232 |
| 7 | 5282, 3368 | Random | 5215, 3318 |
| 8 | 5207, 3637 | Random | 5263, 3687 |
| 9 | 5954, 3475 | Random | 6013, 3529 |
| 10 | 5982, 3882 | Random | 5929, 3820 |
| 11 | 5724, 3991 | Random | 5774, 4041 |
| 12 | 5559, 3757 | **Forest Lord (Fixed)** | 5513, 3878 |

### Felucca Dungeons (5 Spawns)

| Dungeon | Coordinates | Champion Type | Eject Location |
|---------|-------------|---------------|----------------|
| Deceit | 5179, 709 | Unholy Terror (Neira) | 4111, 432 |
| Despise | 5557, 827 | Vermin Horde (Barracoon) | 5580, 632 |
| Destard | 5259, 837 | Cold Blood (Rikktor) | 1176, 2637 |
| Fire | 5815, 1352 | Abyss (Semidar) | 2923, 3406 |
| Terathan Keep | 5190, 1607 | Arachnid (Mephitis) | 5482, 3161 |

---

## Champion Types

### Active in ModernUO

| Type | Champion | Theme | Monsters |
|------|----------|-------|----------|
| **Abyss** | Semidar | Demons | Mongbat → Imp → Gargoyle → Harpy → Fire Gargoyle → Daemon → Succubus |
| **Arachnid** | Mephitis | Spiders/Terathans | Scorpion → Giant Spider → Terathan Drone → Dread Spider → Poison Elemental |
| **Cold Blood** | Rikktor | Reptiles/Dragons | Lizardman → Giant Serpent → Lava Lizard → Drake → Dragon |
| **Forest Lord** | Lord Oaks | Nature | Pixie → Shadow Wisp → Kirin → Centaur → Unicorn → Ethereal Warrior |
| **Vermin Horde** | Barracoon | Rats/Vermin | Giant Rat → Slime → Dire Wolf → Ratman → Hell Hound → Silver Serpent |
| **Unholy Terror** | Neira | Undead | Ghoul → Shade → Bone Magi → Mummy → Lich → Rotting Corpse |
| **Sleeping Dragon** | Serado | Tokuno | Deathwatch Beetle → Lizardman → Kappa → Lesser Hiryu → Hiryu → Oni |
| **Glade** | Twaulo | ML Forest | Pixie → Shadow Wisp → Centaur → Dryad → Satyr → Cu Sidhe → Feral Treefellow |
| **Corrupt** | Ilhenir | ML Plague | Plague Spawn → Bogling → Plague Beast → Bog Thing → Interred Grizzle → Fetid Essence |

### Commented Out (Not Active)

- **Unliving** (Primeval Lich) - Stygian Abyss
- **Pit** (Abyssal Infernal) - Stygian Abyss
- **Valley** (Dragon Turtle) - Time of Legends
- **Khaldun** (Khal Ankur) - Event spawn

---

## Spawn Mechanics

### Level Progression

| Level Range | Kills per Advancement | Monster Difficulty |
|-------------|----------------------|-------------------|
| 1-5 | 256 kills | Easiest (Level 1 monsters) |
| 5-9 | 128 kills | Moderate (Level 2 monsters) |
| 9-13 | 64 kills | Hard (Level 3 monsters) |
| 13-16 | 32 kills | Hardest (Level 4 monsters) |
| 16 | Champion spawns | Boss fight |

### Candle System

- **White candles**: Each represents 20% progress toward next level
- **Red candles**: Each represents one full level advancement
- **16 red candles**: Champion spawns

### Constraints

- Cannot use Recall, Gate Travel, or Sacred Journey in spawn zones
- Cannot mark runes within the area
- Pet summoning restricted as spawn strengthens
- Light level decreases with spawn strength
- **Reset Timer**: If no 20% progress in 10 minutes, monsters reset

### Activation Methods

1. **Natural**: One random Felucca spawn activates on server restart
2. **Valor Virtue**: Knight of Valor can target altar to activate
3. **Candle Addition**: Players with Valor can add red candles to advance faster

---

## Champions

### Semidar (Abyss)

| Property | Value |
|----------|-------|
| Type | Daemon/Succubus |
| HP | ~30,000 |
| Attacks | Magic, Melee |
| Skull | Abyss Skull |

### Mephitis (Arachnid)

| Property | Value |
|----------|-------|
| Type | Giant Spider |
| HP | ~30,000 |
| Attacks | Poison, Melee |
| Skull | Arachnid Skull |

### Rikktor (Cold Blood)

| Property | Value |
|----------|-------|
| Type | Dragon |
| HP | ~30,000 |
| Attacks | Fire Breath, Melee |
| Skull | Cold Blood Skull |

### Lord Oaks (Forest Lord)

| Property | Value |
|----------|-------|
| Type | Ethereal Warrior |
| HP | ~30,000 |
| Special | Spawns Silvani (helper) |
| Skull | Forest Lord Skull |

### Barracoon (Vermin Horde)

| Property | Value |
|----------|-------|
| Type | Ratman |
| HP | ~30,000 |
| Attacks | Disease, Melee |
| Skull | Vermin Skull |

### Neira (Unholy Terror)

| Property | Value |
|----------|-------|
| Type | Lich/Necromancer |
| HP | ~30,000 |
| Attacks | Necromancy, Summons |
| Skull | Unholy Terror Skull |

### Serado (Sleeping Dragon)

| Property | Value |
|----------|-------|
| Type | Ninja |
| HP | ~30,000 |
| Attacks | Ninjitsu |
| Skull | Sleeping Dragon Skull |

### Twaulo (Glade)

| Property | Value |
|----------|-------|
| Type | Satyr/Nature |
| HP | ~30,000 |
| Attacks | Nature magic |
| Skull | Glade Skull |

### Ilhenir (Corrupt)

| Property | Value |
|----------|-------|
| Type | Plague creature |
| HP | ~30,000 |
| Attacks | Poison, Disease |
| Skull | Corrupt Skull |

---

## Rewards

### Artifact Drop Rates

When a champion dies, there's a 30% chance of artifact drop:

| Category | Chance | Examples |
|----------|--------|----------|
| Unique | 5% | Crown of Tal'Keesh, Shroud of Deceit, Light's Rampart |
| Shared | 10% | Detective Boots, Oblivion's Needle, Royal Guard items |
| Decorative | 15% | Lava Tile, Web, Spider Statue, Demon Skull |

### Gold Rewards

- ~250,000 gold scattered around champion death location
- Distributed via GoodiesTimer over 10 seconds

### Champion Skulls (Felucca Only)

Each champion drops a unique skull used to summon **The Harrower**:

| Champion | Skull Type |
|----------|-----------|
| Semidar | Abyss Skull |
| Mephitis | Arachnid Skull |
| Rikktor | Cold Blood Skull |
| Lord Oaks | Forest Lord Skull |
| Barracoon | Vermin Skull |
| Neira | Unholy Terror Skull |
| Serado | Sleeping Dragon Skull |
| Twaulo | Glade Skull |
| Ilhenir | Corrupt Skull |

### Valor Virtue Gain

All participants with looting rights gain 800 Valor points when champion dies.

### Power Scrolls (REMOVED IN SPHERE 51A)

Standard UO distributes 6 power scrolls per champion kill:
- 55% chance: 110 scroll
- 40% chance: 115 scroll
- 5% chance: 120 scroll

**These are REMOVED in our shard.** Consider replacing with:
- Relics (for talisman crafting) Yes
- Special crafting materials Yes

---

## Loot Distribution

### Damage-Based Rights

Loot distribution is based on damage contribution:
1. System calculates damage entries for all participants
2. Players with sufficient damage get "looting rights"
3. Artifacts and scrolls go to random players with rights
4. Gold scatters for anyone to pick up

### Justice Virtue Bonus (REPURPOSED)

In standard UO, a Justice protector gets duplicate power scrolls.

**Sphere 51a**: Repurpose to provide bonus relics/siege coins:

| Justice Level | Artifact Bonus | Siege Coin Bonus |
|---------------|-------------|------------------|
| Seeker | +5% drop chance | +5 coins |
| Follower | +10% drop chance | +10 coins |
| Knight | +15% drop chance | +15 coins |

---

## The Harrower

If all champion skulls are collected and placed on the Harrower altar, **The Harrower** can be summoned - the ultimate boss encounter.

### Harrower Rewards

- True Black dye
- Harrower artifacts
- Significant gold/loot

---

## Code Files

| File | Purpose |
|------|---------|
| `ChampionSpawn.cs` | Core spawn mechanics |
| `ChampionSpawnInfo.cs` | Spawn type definitions, monster tables |
| `ChampionCommands.cs` | Admin commands |
| `GenChamps.cs` | Spawn location generation |
| `GenChampEntry.cs` | Location entry definitions |
| `BaseChampion.cs` | Champion boss base class |
| `CannedEvilTimer.cs` | Spawn management timer |
| `ChampionAltar.cs` | Spawn altar item |
| `ChampionPlatform.cs` | Platform decoration |

### Individual Champions

| Champion | File |
|----------|------|
| Semidar | `Mobiles/Special/Semidar.cs` |
| Mephitis | `Mobiles/Special/Mephitis.cs` |
| Rikktor | `Mobiles/Special/Rikktor.cs` |
| Lord Oaks | `Mobiles/Special/LordOaks.cs` |
| Barracoon | `Mobiles/Special/Barracoon.cs` |
| Neira | `Mobiles/Special/Neira.cs` |
| Serado | `Mobiles/Special/Serado.cs` |
| Twaulo | `Mobiles/Monsters/ML/Special/Twaulo.cs` |
| Ilhenir | `Mobiles/Monsters/ML/Special/Ilhenir.cs` |
| Meraktus | `Mobiles/Monsters/ML/Special/Meraktus.cs` |

---

## Admin Commands

| Command | Access | Description |
|---------|--------|-------------|
| `[GenChamps` | Developer | Generate all champion spawns |

---

## Sphere 51a Implementation Notes

### Remove Power Scroll Distribution

In `BaseChampion.cs`, the `GivePowerScrolls()` method distributes power scrolls. Options:

1. **Remove entirely**: Comment out the power scroll distribution
2. **Replace with relics**: Give dungeon relics instead
3. **Replace with siege coins**: Award siege coins based on participation

### Add Relic/Siege Coin Rewards

```csharp
public void GiveRelicRewards()
{
    if (Map != Map.Felucca)
        return;

    var rights = GetLootingRights(DamageEntries, HitsMax);
    var toGive = new List<Mobile>();

    for (var i = rights.Count - 1; i >= 0; --i)
    {
        if (rights[i].m_HasRight)
            toGive.Add(rights[i].m_Mobile);
    }

    if (toGive.Count == 0)
        return;

    // Give Valor (keep this)
    foreach (var m in toGive)
    {
        if (m is PlayerMobile pm)
        {
            var gainedPath = false;
            VirtueSystem.Award(pm, VirtueName.Valor, 800, ref gainedPath);
            // Message handling...
        }
    }

    // Give relics instead of power scrolls
    toGive.Shuffle();
    for (var i = 0; i < 6; ++i)
    {
        var m = toGive[i % toGive.Count];

        // Create relic based on tier chance
        var relic = CreateRandomRelic();
        GiveRelicTo(m, relic);

        // Check Justice bonus
        if (m is PlayerMobile pm)
        {
            var prot = JusticeVirtue.GetProtector(pm);
            if (prot != null && CheckJusticeBonus(prot))
            {
                GiveBonusRelic(prot);
                GiveSiegeCoins(prot, GetJusticeSiegeCoinBonus(prot));
            }
        }
    }
}
```

### Modify Justice Virtue Check

```csharp
private int GetJusticeSiegeCoinBonus(PlayerMobile pm)
{
    return VirtueSystem.GetLevel(pm, VirtueName.Justice) switch
    {
        VirtueLevel.Seeker => 5,
        VirtueLevel.Follower => 10,
        VirtueLevel.Knight => 15,
        _ => 0
    };
}
```

---

## Configuration

```json
{
  "champion_spawns.enabled": true,
  "champion_spawns.power_scrolls_enabled": false,
  "champion_spawns.relics_enabled": true,
  "champion_spawns.relics_per_kill": 6,
  "champion_spawns.relic_common_chance": 0.70,
  "champion_spawns.relic_uncommon_chance": 0.25,
  "champion_spawns.relic_rare_chance": 0.05,
  "champion_spawns.gold_per_kill": 250000,
  "champion_spawns.artifact_chance": 0.30,
  "champion_spawns.skull_drop": true
}
```

---

## Open Questions

1. **Scrolls of Transcendence**: Keep or remove? (Provides instant skill gains)
2. **Harrower**: Enable or disable? (Requires all skulls)
3. **Additional Spawns**: Enable Ilshenar/Tokuno spawns?
4. **Spawn Frequency**: How often should spawns naturally activate?
5. **Relic Tier Distribution**: What % common/uncommon/rare from champions?

---

*This document defines Champion Spawns for Sphere 51a. Update after design decisions are finalized.*
