# New Player Experience

**Authority**: TIER 2 - Implementation Spec
**Related**: `Combat/PVP_COMBAT.md`, `Factions/FACTION_OVERVIEW.md`

---

## Overview

New players are protected during their first 14 days to learn game mechanics without PvP interference.

---

## Young Player Protection

| Protection | Details |
|------------|---------|
| Duration | 14 days |
| Cannot attack | Other players |
| Cannot be attacked | By players |
| Cannot join | Factions |
| Dungeon access | Blocked |

---

## Renouncing Young Status

Players can renounce early via:
- New Player Guide NPC
- Command: `[renounceyoung]`

**Warning**: Cannot be undone.

---

## Starter Islands

5 themed training islands accessed via Ferry NPC:

| Island | Skills Taught | Reward |
|--------|---------------|--------|
| Combat Basics | Swords, Tactics | GM Swords, GM Tactics |
| Magic Fundamentals | Magery, Eval Int | GM Magery, GM Eval Int |
| Healing Arts | Healing, Anatomy | GM Healing, GM Anatomy |
| Stealth & Cunning | Hiding, Stealth | GM Hiding, GM Stealth |
| Advanced Combat | Resist, Meditation | GM Resist, GM Meditation |

**Total time**: 2-3 hours for all islands.

---

## Quest Structure

Each island follows:

1. **NPC Introduction**
   - Explains skill purpose
   - Provides basic equipment

2. **Training Dummies/Targets**
   - Practice area
   - Safe skill gains

3. **Skill Challenges**
   - Practical tests
   - Guided progression

4. **Completion Reward**
   - GM skill granted
   - Next island unlocked

---

## New Player Guide NPC

**Location**: Britain Docks (Felucca)

### Services

| Service | Description |
|---------|-------------|
| Help Menu | Command reference |
| Wiki Link | External documentation |
| Renounce Young | Remove protection |
| Teleport | Training islands |
| FAQ | Common questions |

---

## Design Philosophy

### Quick PvP Readiness

Skills acquired quickly via quests, not grinding:
- 2-3 hours to PvP-ready character
- Combat skills from starter quests
- PvP success depends on player skill, not time investment

### Trade Skills

Trade skills still require traditional effort:
- Crafting (Blacksmithing, Tailoring, etc.)
- Gathering (Mining, Lumberjacking, etc.)
- Support (Tinkering, Inscription, etc.)

**Rationale**: Trade skills provide economy engagement, not combat advantage.

---

## Restrictions While Young

| Activity | Status |
|----------|--------|
| PvP combat | Blocked |
| Dungeon entry | Blocked |
| Faction join | Blocked |
| Guild join | Allowed |
| Trading | Allowed |
| Crafting | Allowed |
| PvM | Allowed (outside dungeons) |

---

## Database Tables

| Table | Purpose |
|-------|---------|
| s51a_young_players | NPE tracking |
| s51a_starter_quest_progress | Quest completion |
| s51a_skill_grants | Granted skills log |

---

## Configuration

```json
{
  "young.duration_days": 14,
  "young.dungeon_blocked": true,
  "young.pvp_blocked": true,
  "young.faction_blocked": true,
  "young.can_renounce": true,
  "starter.quest_skill_grant": 100.0,
  "starter.total_quests": 5
}
```

---

## FAQ

**Q: Can I skip the training islands?**
A: Yes, you can renounce young status and skip directly to normal gameplay.

**Q: Do I keep the skills if I renounce early?**
A: You keep any skills already granted from completed quests.

**Q: Can friends help me on training islands?**
A: Training islands are solo instances for learning purposes.

**Q: What happens after 14 days?**
A: Young status expires automatically. You can join factions and enter dungeons.
