# CLAUDE.md - ModernUO Custom Shard Project Guide

This file provides context for Claude Code (AI assistant) when working on this ModernUO-based Ultima Online private server project.

## IMPORTANT: Read These First

| Document | Purpose | Authority |
|----------|---------|-----------|
| `INDEX.md` | **Topic lookup** - find any topic fast | Navigation |
| `Core/DESIGN_DECISIONS.md` | All locked design decisions (DO NOT change) | **TIER 1 - WINS CONFLICTS** |
| `ROADMAP.md` | Current implementation status | TIER 3 |
| `START_HERE.md` | Navigate the documentation folder | TIER 3 |

> **Rule**: If two documents disagree, check `Core/DESIGN_DECISIONS.md` - it is always correct.

## Documentation Structure

```
Documentation/
├── Core/                   ← TIER 1: Authoritative decisions
│   └── DESIGN_DECISIONS.md
├── Systems/                ← TIER 2: Implementation specs
│   ├── Combat/            (PVP_COMBAT.md, SPELL_SYSTEM.md)
│   ├── Factions/          (FACTION_OVERVIEW.md, SIEGE_BATTLES.md, SIEGE_REWARDS.md)
│   ├── Crafting/          (ORE_MATERIALS.md, RUNIC_CRAFTING.md, DUNGEON_COOKING.md)
│   ├── Progression/       (TALISMAN_SYSTEM.md, JEWELRY_SYSTEM.md, GLICKO_RATINGS.md)
│   ├── Economy/           (CURRENCIES.md)
│   └── Content/           (DUNGEONS.md, DAILY_CONTENT.md, TOURNAMENTS.md, NEW_PLAYER.md)
├── Archive/               ← Old docs (reference only)
└── INDEX.md, ROADMAP.md, START_HERE.md, CLAUDE.md
```

## Project Status

**Current State**: Pre-Alpha (NOT PLAYABLE)
- Design decisions: Complete (**18 sections** in DESIGN_DECISIONS.md)
- Database: SQLite working (PostgreSQL optional for production)
- Server: Runs successfully
- Faction System: Working with SQLite persistence
- Spell System: Complete and tested (51a-style)
- Crafting System: **DECIDED** - Option E-1 (full runic, PvE-only bonuses)
- PvP Combat: **DESIGNED** - ImaginNation-style formulas (see Section 17)
- Seasonal Ore: **DESIGNED** - 7-tier system with mining stones (see Section 18)
- **NEXT**: Implement PvP combat formulas, then Talisman system

**The owner does NOT code** - they use AI assistance for all development. Be thorough in explanations.

## Project Overview

This is a **custom Ultima Online shard** built on the **ModernUO** emulator framework. The project implements a "Sphere 51a-style" server with:
- Custom 3-faction system (guild-based)
- Sphere-style spell casting (immediate targeting, no damage interrupt)
- Talisman progression system (disabled in PvP)
- Relic-based economy (forces guild cooperation)

**Tech Stack:**
- **.NET 10.0** (Latest LTS)
- **C# 14** (Latest language features)
- **SQLite** for development, **PostgreSQL** optional for production
- **Cross-platform**: Windows, macOS, Linux (x64 & ARM64)

## Project Structure

```
51a-style-ModernUo-51alpha/
├── Projects/                    # C# source code (6 projects)
│   ├── Server/                  # Core server engine (261 files)
│   ├── Server.Tests/            # Server unit tests
│   ├── Application/             # Entry point & startup
│   ├── UOContent/               # Game content (3,557 files, 43MB)
│   │   ├── Accounting/          # Account management & security
│   │   ├── Commands/            # Player & admin commands
│   │   ├── Engines/             # Major game systems
│   │   │   ├── BulkOrders/      # Bulk order deeds (BODs)
│   │   │   ├── Craft/           # Crafting system
│   │   │   ├── Factions/        # Faction warfare system
│   │   │   ├── ConPVP/          # Competitive PvP tournaments
│   │   │   └── ...
│   │   ├── Items/               # All item definitions
│   │   ├── Mobiles/             # NPCs, creatures, players
│   │   ├── Multis/              # Housing & multi-tile structures
│   │   ├── Skills/              # Skill implementations
│   │   ├── Spells/              # Spell definitions & mechanics
│   │   ├── Sphere51a/           # CUSTOM: This shard's content
│   │   ├── Gumps/               # UI dialogs
│   │   └── Migrations/          # Serialization version schemas
│   ├── UOContent.Tests/         # Content unit tests
│   └── Logger/                  # Logging utility
├── Distribution/                # Runtime output directory
│   ├── Assemblies/              # Compiled DLLs
│   ├── Configuration/           # JSON config files
│   ├── Data/                    # Game data (spawns, items, etc.)
│   ├── Saves/                   # World persistence
│   ├── Logs/                    # Server logs
│   └── Backups/                 # Auto-backups
├── Documentation/               # Design & architecture docs
├── sql/                         # Database migrations & seed data
├── helm/                        # Kubernetes deployment
└── tests/                       # Integration & load tests
```

## Build Commands

```bash
# Windows (from project root)
publish.cmd [config] [os] [arch]
# Examples:
publish.cmd                      # Release, auto-detect OS/arch
publish.cmd Debug win x64        # Debug build for Windows x64
publish.cmd Release linux arm64  # Release for Linux ARM64

# Linux/macOS
./publish.sh [config] [os] [arch]

# Manual dotnet commands
dotnet restore --force-evaluate
dotnet build -c Release
dotnet publish -c Release -r win-x64
dotnet tool run ModernUOSchemaGenerator  # Generate migration schemas
```

**Build Configurations:** `Debug`, `Release`, `Analyze`

**Runtime Identifiers:** `win-x64`, `win-arm64`, `osx-x64`, `osx-arm64`, `linux-x64`, `linux-arm64`

## Running the Server

```bash
cd Distribution
./ModernUO.exe                   # Windows
./ModernUO                       # Linux/macOS
```

Default port: **2593** (configurable in `Distribution/Configuration/modernuo.json`)

## Key Configuration Files

| File | Purpose |
|------|---------|
| `Distribution/Configuration/modernuo.json` | Main server config (60+ settings) |
| `Distribution/Configuration/expansion.json` | Game expansion settings |
| `Distribution/Configuration/email-settings.json` | SMTP for in-game mail |
| `Distribution/Configuration/antimacro.json` | Anti-speedhack settings |
| `global.json` | .NET SDK version (10.0.100) |
| `Directory.Build.props` | Global MSBuild properties |
| `.editorconfig` | Code style (4-space indent, 125 char lines) |

## Coding Patterns

### 1. Serialization System (CRITICAL)
ModernUO uses **source-generated serialization** with migration versioning:

```csharp
[SerializationGenerator(version, false)]  // Version number for migrations
public partial class MyItem : Item
{
    [SerializableField(0)]
    private int _myField;

    // Migrations handle backward compatibility automatically
}
```

**Migration files** live in `Projects/*/Migrations/` as JSON schemas.

### 2. Entity Hierarchy
```
IEntity (IPoint3D, ISerializable)
    └── Entity (Serial, Location, Map)
            ├── Item (all items inherit from this)
            └── Mobile (all creatures/NPCs/players)
```

### 3. Common Attributes
```csharp
[SerializationGenerator(0, false)]  // Serialization version
[SerializableField(0)]              // Field to serialize
[Hue]                               // Color property
[PropertyObject]                    // Complex property marker
[CallPriority(100)]                 // Method execution order
[TypeAlias("OldName")]              // Backward compatibility alias
```

### 4. Command System
```csharp
public static class MyCommands
{
    [Usage("MyCommand <arg>")]
    [Description("Does something cool")]
    public static void MyCommand_OnCommand(CommandEventArgs e)
    {
        // Implementation
    }
}
```

### 5. Gump (UI) Pattern
```csharp
public class MyGump : Gump
{
    public MyGump() : base(50, 50)
    {
        AddBackground(0, 0, 300, 200, 9270);
        AddLabel(20, 20, 0, "Hello World");
        AddButton(20, 50, 4005, 4007, 1, GumpButtonType.Reply, 0);
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        // Handle button clicks
    }
}
```

## Custom Content Location

This shard's **custom content** is in `Projects/UOContent/Sphere51a/`:
- Custom faction systems
- Weapon quests
- Siege coins
- Shard-specific features

## Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test Projects/Server.Tests/
dotnet test Projects/UOContent.Tests/

# Test commands in-game (admin only)
[TestFactions          # Test faction system
[TestFactionDB         # Test faction database
```

Tests use **xUnit** framework.

## Database

### SQLite (Default - Development)
SQLite is used by default and requires no setup. Database file:
```
Distribution/Saves/sphere51a.db
```

### PostgreSQL (Optional - Production)
For high-load production, configure in `Distribution/Configuration/modernuo.json`:
```json
{
  "s51a.database.provider": "postgresql",
  "s51a.database.connectionString": "Host=localhost;Database=51alpha;Username=51alpha_app;Password=xxx"
}
```

## Important Directories for Content Creation

| Content Type | Location |
|--------------|----------|
| Items | `Projects/UOContent/Items/` |
| Mobiles/NPCs | `Projects/UOContent/Mobiles/` |
| Spells | `Projects/UOContent/Spells/` |
| Skills | `Projects/UOContent/Skills/` |
| Crafting recipes | `Projects/UOContent/Engines/Craft/` |
| Bulk orders | `Projects/UOContent/Engines/BulkOrders/` |
| Factions | `Projects/UOContent/Engines/Factions/` |
| Commands | `Projects/UOContent/Commands/` |
| Gumps (UI) | `Projects/UOContent/Gumps/` |
| Custom shard content | `Projects/UOContent/Sphere51a/` |
| Game data (JSON) | `Distribution/Data/` |
| Spawn definitions | `Distribution/Data/Spawns/` |

## Common Development Tasks

### Adding a New Item
1. Create class in `Projects/UOContent/Items/[Category]/`
2. Inherit from `Item` or appropriate base class
3. Add `[SerializationGenerator(0, false)]` attribute
4. Implement constructor with `Serial serial` for deserialization
5. Run `dotnet tool run ModernUOSchemaGenerator` after building

### Adding a New Mobile/NPC
1. Create class in `Projects/UOContent/Mobiles/[Category]/`
2. Inherit from `BaseCreature` or appropriate base
3. Set stats, skills, loot in constructor
4. Add AI type and fight mode

### Adding a New Command
1. Add static method with `[Usage]` and `[Description]` attributes
2. Method name must end with `_OnCommand`
3. Use `CommandEventArgs e` parameter

### Adding a New Spell
1. Create class in `Projects/UOContent/Spells/[School]/`
2. Inherit from appropriate `Spell` base class
3. Override `OnCast()` method

## Dependencies (NuGet)

Key packages:
- `Serilog` - Structured logging
- `Npgsql` - PostgreSQL driver
- `MailKit` - Email support
- `Argon2.Bindings` - Password hashing
- `ModernUO.Serialization.Generator` - Serialization source gen
- `CommunityToolkit.HighPerformance` - Performance utilities

## Code Style

- **4-space indentation** (no tabs)
- **125 character line limit**
- **UTF-8 encoding**
- StyleCop rules enforced (see `stylecop.json`)
- `var` preferred for obvious types
- Expression-bodied members encouraged

## Troubleshooting

### Build fails with serialization errors
Run: `dotnet tool run ModernUOSchemaGenerator`

### Migration version conflicts
Check `Projects/*/Migrations/` for schema files and increment version in `[SerializationGenerator(N)]`

### Server won't start
1. Check `Distribution/Logs/` for error logs
2. Verify `Distribution/Configuration/modernuo.json` is valid JSON
3. Ensure port 2593 is not in use

### Database connection issues
1. Verify PostgreSQL is running
2. Check connection string in `modernuo.json`
3. Run migrations from `sql/migrations/`

## Resources

- [ModernUO GitHub](https://github.com/modernuo/ModernUO)
- [UO Protocol Documentation](https://docs.polserver.com/packets/)
- Project docs in `Documentation/` folder
- Implementation docs: `IMPLEMENTATION_*.md` files in root

## Current Implementation Status

**See `ROADMAP.md` for full status.**

### Working Systems
- **Faction System** - SQLite persistence, admin commands, week 1 lockout
- **Spell System** - 51a-style (immediate targeting, no damage interrupt, 50% mana fizzle)
- **Database** - SQLite auto-setup, PostgreSQL optional

### Design Complete (Ready to Implement)
- **PvP Combat System** - ImaginNation-style formulas (DESIGN_DECISIONS.md Section 17)
- **Seasonal Ore System** - 7-tier armor, mining stones (DESIGN_DECISIONS.md Section 18)
- **Crafting Decision** - Option E-1 locked (full runic, PvE-only bonuses)
- **Talisman System** - Combat decisions unblocked this
- **Jewelry System** - Combat decisions unblocked this

### Not Started
- Siege system
- Tournament system
- Daily content
- Dungeons

### Next Steps
1. **Implement PvP combat formulas** - BaseWeapon.cs, BaseArmor.cs changes
2. **Implement 7-tier armor system** - Update ore AR bonuses (absolute values)
3. **Implement mining stones** - SharpeningStone, AccuracyStone (RNG on use)
4. **Implement 4 Talisman types** - Dexer, Tamer, Sampire, TH (each with T3/T2/T1)
5. **Implement Talisman mechanics** - PvP drop, skill reduction, ability gating
6. **Implement PvP Jewelry** - Ring/Bracelet/Earrings/Glasses (+2.5 stats each)

**Full implementation checklists in `Documentation/Systems/Combat/PVP_COMBAT.md`**

## Key Design Constraints

These are LOCKED. Do not change without explicit discussion:

1. **Talismans disabled in PvP** - drops to backpack on PvP flag, 5-min cooldown
2. **4 Talisman types × 3 tiers** - Dexer, Tamer, Sampire, TH (each has T3/T2/T1)
3. **Talisman-gated abilities** - Bushido/WeaponMoves (Dexer), Chiv/Necro (Sampire) disabled in PvP
4. **Talismans reduce other skills to 0** - Creates OSI 7-skill template builds
5. **Runic bonuses disabled in PvP** - only work vs monsters (Option E-1)
6. **Spells don't interrupt on damage** - Sphere 51a style
7. **Guild required for factions** - solo players make 1-member guilds
8. **7-day cooldowns** - on faction change and guild change
9. **Relics only from dungeons** - outside world gives gold, not relics
10. **PvP uses separate combat formulas** - ImaginNation-style (weaker AR absorption)
11. **7-tier armor system** - T1=25 to T7=65 AR for plate (absolute values, not bonuses)
12. **Mining stones** - Single item types with RNG on application
13. **Bound on equip** - talismans and jewelry cannot be traded after wearing
14. **PvP Jewelry** - Ring/Bracelet/Earrings/Glasses (+2.5 Str/Dex/Int each, +10 max)

## Custom Sphere51a Code Location

All custom code goes in `Projects/UOContent/Sphere51a/`:
```
Sphere51a/
├── Core/           # Initialization, config, database
├── Factions/       # Faction system (partially done)
├── Commands/       # Admin commands
├── Glicko/         # Rating system (partial)
├── Items/          # Custom items
├── Mobiles/        # Custom NPCs
└── Tests/          # Test commands
```

## When Implementing New Features

1. Check `Core/DESIGN_DECISIONS.md` for locked specs
2. Check relevant `Systems/[Category]/` doc for implementation details
3. Check `ROADMAP.md` for dependencies
4. Put new code in `Sphere51a/` folder
5. Use existing patterns from faction system
6. Add admin test commands for debugging

## Common Gotchas

- **SQLite works by default** - no database setup needed for development
- **Serialization versions** - increment when changing serialized fields
- **Run schema generator** after adding serializable classes
- **Check for null** - guilds, factions can be null
- **Runic bonuses PvE-only** - all runic properties disabled vs players (Option E-1)
