# Documentation Navigation

**Last Updated**: 2025-01-16

---

## Quick Start

**Looking for a specific topic?** → Use `INDEX.md` (alphabetical topic lookup)

**Need to know what's authoritative?** → `Core/DESIGN_DECISIONS.md` is TIER 1 (always wins)

---

## Primary Documents

| File | Purpose | Authority |
|------|---------|-----------|
| `INDEX.md` | **Topic lookup** - find any topic fast | Navigation |
| `Core/DESIGN_DECISIONS.md` | **All 18 locked design decisions** | TIER 1 (wins conflicts) |
| `ROADMAP.md` | Current status, what to work on next | TIER 3 |
| `CLAUDE.md` | Context for AI assistants | TIER 3 |

---

## Document Hierarchy

```
TIER 1: AUTHORITATIVE (If conflict, these win)
└── Core/DESIGN_DECISIONS.md ← 18 sections of locked decisions

TIER 2: IMPLEMENTATION SPECS (Detailed how-to)
├── Systems/Combat/
│   ├── PVP_COMBAT.md
│   └── SPELL_SYSTEM.md
├── Systems/Factions/
│   ├── FACTION_OVERVIEW.md
│   ├── SIEGE_BATTLES.md
│   └── SIEGE_REWARDS.md
├── Systems/Crafting/
│   ├── ORE_MATERIALS.md
│   ├── RUNIC_CRAFTING.md
│   └── DUNGEON_COOKING.md
├── Systems/Progression/
│   ├── TALISMAN_SYSTEM.md
│   ├── JEWELRY_SYSTEM.md
│   └── GLICKO_RATINGS.md
├── Systems/Economy/
│   └── CURRENCIES.md
└── Systems/Content/
    ├── DUNGEONS.md
    ├── DAILY_CONTENT.md
    ├── TOURNAMENTS.md
    └── NEW_PLAYER.md

TIER 3: WORKING DOCUMENTS (Status tracking)
├── ROADMAP.md
├── CLAUDE.md
└── START_HERE.md ← You are here

ARCHIVE: (Old files for reference)
└── Archive/
    ├── Design/   (original design docs)
    └── specs/    (original spec files)
```

---

## Folder Structure

```
Documentation/
├── INDEX.md                ← Topic lookup (use this!)
├── START_HERE.md           ← You are here
├── ROADMAP.md              ← Implementation status
├── CLAUDE.md               ← AI assistant context
│
├── Core/                   ← Authoritative decisions
│   └── DESIGN_DECISIONS.md ← TIER 1 AUTHORITY
│
├── Systems/                ← Implementation specs (TIER 2)
│   ├── Combat/
│   ├── Factions/
│   ├── Crafting/
│   ├── Progression/
│   ├── Economy/
│   └── Content/
│
├── Reference/              ← External system references
│   ├── SERVUO_FACTION_REFERENCE.md
│   └── RUNIC_PROPERTIES_COMPLETE.md
│
├── Archive/                ← Old documents (reference only)
│   ├── Design/
│   └── specs/
│
├── Implementation/         ← System architecture (may be outdated)
└── Development/            ← Dev-specific docs
```

---

## Quick Links by Topic

### Combat & PvP
- **Locked Decisions**: `Core/DESIGN_DECISIONS.md` Sections 2, 17
- **Implementation**: `Systems/Combat/PVP_COMBAT.md`
- **Spells**: `Systems/Combat/SPELL_SYSTEM.md`

### Armor & Crafting
- **Locked Decisions**: `Core/DESIGN_DECISIONS.md` Sections 16, 18
- **Ore System**: `Systems/Crafting/ORE_MATERIALS.md`
- **Runic**: `Systems/Crafting/RUNIC_CRAFTING.md`
- **Cooking**: `Systems/Crafting/DUNGEON_COOKING.md`

### Factions
- **Locked Decisions**: `Core/DESIGN_DECISIONS.md` Section 1, 9
- **Overview**: `Systems/Factions/FACTION_OVERVIEW.md`
- **Sieges**: `Systems/Factions/SIEGE_BATTLES.md`
- **Rewards**: `Systems/Factions/SIEGE_REWARDS.md`

### Talismans & Jewelry
- **Locked Decisions**: `Core/DESIGN_DECISIONS.md` Sections 6, 7
- **Talismans**: `Systems/Progression/TALISMAN_SYSTEM.md`
- **Jewelry**: `Systems/Progression/JEWELRY_SYSTEM.md`
- **Ratings**: `Systems/Progression/GLICKO_RATINGS.md`

### Economy
- **Locked Decisions**: `Core/DESIGN_DECISIONS.md` Sections 4, 8, 11
- **Currencies**: `Systems/Economy/CURRENCIES.md`

### Content
- **Dungeons**: `Systems/Content/DUNGEONS.md`
- **Daily Content**: `Systems/Content/DAILY_CONTENT.md`
- **Tournaments**: `Systems/Content/TOURNAMENTS.md`
- **New Players**: `Systems/Content/NEW_PLAYER.md`

---

## What Goes Where

### For New Design Decisions
1. Make decisions in conversation
2. Add summary to `Core/DESIGN_DECISIONS.md` (authoritative)
3. Create detailed spec in `Systems/[Category]/` if needed

### For Implementation Details
- Put in appropriate `Systems/[Category]/*.md` folder
- Reference back to DESIGN_DECISIONS.md section

### For Outdated Content
- Already moved to `Archive/`

---

## Archive Contents

Old design documents have been moved to `Archive/` for reference:

| Folder | Contents |
|--------|----------|
| `Archive/Design/` | Original design docs (factions.md, pvm.md, etc.) |
| `Archive/specs/` | Original spec files (talisman-system.md, spell-system.md) |

These are preserved for historical reference but should not be used for implementation.
