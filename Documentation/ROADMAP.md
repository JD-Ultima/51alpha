# 51alpha Development Roadmap

**Last Updated**: 2025-01-16
**Status**: Pre-Alpha (Not Playable)
**Approach**: Solo developer using AI assistance
**Authority**: TIER 3 - Working document (see `Core/DESIGN_DECISIONS.md` for locked specs)

> **Looking for a topic?** → Use `INDEX.md`
> **Design questions?** → `Core/DESIGN_DECISIONS.md` is authoritative
> **Implementation details?** → Check `Systems/[Category]/` folders

---

## Quick Status

| Area | Status | Notes |
|------|--------|-------|
| Design Decisions | ✅ Complete | 18 sections locked in DESIGN_DECISIONS.md |
| Database Setup | ✅ SQLite Working | PostgreSQL optional for production |
| Server Runs | ✅ Verified | Server starts and runs |
| Faction System | ✅ Working | SQLite persistence, Week 1 lockout active |
| Spell System | ✅ Working | 51a-style tested and confirmed |
| Crafting System | ✅ **DECIDED** | Option E-1: Full runic, PvE-only bonuses |
| **PvP Combat** | ✅ **DESIGNED** | ImaginNation-style formulas, 7-tier armor |
| **Seasonal Ore** | ✅ **DESIGNED** | 7 tiers, seasonal rotation, mining stones |
| Talisman System | 🟡 Ready to implement | Combat decisions unblocked this |
| Jewelry System | 🟡 Ready to implement | Combat decisions unblocked this |

---

## Recent Design Decisions (LOCKED)

### PvP Combat Balance (Section 17)
- **ImaginNation-style formulas** for PvP (separate from PvM)
- **Hit Chance**: `0.69 × ((Skill + Tactics×2) / 300)` = 69% max
- **AR Absorption**: `Random(AR/4.3 to AR/2.4)` (weaker than PvM)
- **Parry**: 20% at GM with shield only
- **4 Talisman Types**: Dexer, Tamer, Sampire, Treasure Hunter (each with T1/T2/T3 tiers)
- **Talisman-gated abilities**: Bushido/Weapon Moves (Dexer), Chivalry/Necro (Sampire) - disabled in PvP when talisman drops

### 7-Tier Armor System (Section 18)
- **Plate AR**: T1=25, T2=30, T3=35, T4=45, T5=55, T6=60, T7=65
- **Leather**: 15 AR base
- **Seasonal rotation**: Ore names/hues change quarterly
- **Mining stones**: Sharpening (+dmg) and Accuracy (+hit) as rare drops

### Crafting Decision: Option E-1 (Section 16)
- **Full runic system** with all bonuses **PvE-only**
- Runic properties disabled vs players
- Base material bonuses still work in PvP

**See `Core/DESIGN_DECISIONS.md` for all 18 sections.**
**See `Systems/` folders for detailed implementation specs.**

---

## What's Actually Done

### Design Phase (Complete)
All major design decisions are locked. See `DESIGN_DECISIONS.md` for the full list.

### Code Written (Untested)
The following code exists in `Projects/UOContent/Sphere51a/` but has **not been verified to work**:

**Core Infrastructure:**
- `Core/S51aInitializer.cs` - System startup
- `Core/S51aConfig.cs` - Configuration
- `Core/Database/PostgresConnection.cs` - DB connection (needs PostgreSQL)
- `Core/Database/MigrationRunner.cs` - DB migrations
- `Core/S51aPvPContext.cs` - PvP detection
- `Core/S51aDamageTracker.cs` - Damage tracking
- `Core/S51aAuthenticationHooks.cs` - Auth hooks

**Faction System:**
- `Factions/FactionType.cs` - Enum (GoldenShield, Bridgefolk, LycaeumOrder)
- `Factions/S51aFaction.cs` - Faction definitions
- `Factions/S51aFactionSystem.cs` - Central manager
- `Factions/FactionRepository.cs` - PostgreSQL persistence
- `Factions/GuildFactionExtensions.cs` - Guild extensions
- `Factions/GuildFactionInfo.cs` - Guild-faction data
- `Factions/FactionVendorDiscount.cs` - 10% NPC discount
- `Factions/Commands/FactionCommands.cs` - Admin commands

**Other Systems (Stubs/Partial):**
- `Glicko/GlickoRating.cs` - Rating data structure
- `Glicko/GlickoCalculator.cs` - Rating calculations
- `Commands/RemoveAllFCCommand.cs` - Remove Faster Casting
- `Items/TestStone.cs` - Testing item
- `Items/DungeonIngredients.cs` - Dungeon drops
- `Mobiles/ExternalResourceCreatures.cs` - Outside world mobs

**Database Repositories (Need PostgreSQL):**
- `Core/Database/AccountRepository.cs`
- `Core/Database/SiegeRepository.cs`
- `Core/Database/TournamentRepository.cs`
- `Core/Database/DailyContentRepository.cs`
- `Core/Database/GlickoRepository.cs`
- `Core/Database/NPERepository.cs`

---

## Development Phases

### Phase 0: Foundation Verification ✅ COMPLETE
Database and core systems verified.

- [x] **0.1** ~~Install PostgreSQL locally~~ → Using SQLite instead (PostgreSQL optional for production)
- [x] **0.2** ~~Create database and user~~ → SQLite auto-creates `Distribution/Saves/sphere51a.db`
- [x] **0.3** Run the ModernUO server and check for errors → Server runs successfully
- [x] **0.4** Verify Sphere51a initializes (look for `[Sphere51a]` in console) → Confirmed
- [x] **0.5** Test faction commands work → Working (Week 1 lockout active by design)
- [x] **0.6** Document what works and what's broken → Faction system working, spell system working

### Phase 1: Core Systems Working ✅ MOSTLY COMPLETE
Base game runs with custom features.

- [x] **1.1** Fix any startup errors from Phase 0 → No errors
- [x] **1.2** Faction system fully functional (join/leave/cooldown) → Working with SQLite
- [ ] **1.3** Vendor discounts working (10% in home city) → Needs testing
- [x] **1.4** Basic admin commands working → Faction commands functional

### Phase 2: Spell System ✅ COMPLETE
Sphere 51a-style spell mechanics - tested and working.

- [x] **2.1** Immediate targeting (target cursor appears on cast)
- [x] **2.2** No damage interruption
- [x] **2.3** Free movement while casting
- [x] **2.4** 50% mana on fizzle
- [x] **2.5** Remove Faster Casting from all equipment
- [x] **2.6** Scroll bonuses (43% mana reduction, 0.5s faster)
- [x] **2.7** Protection spell has no FC penalty

### Phase 3: Talisman System
PvM progression gear that's disabled in PvP. 4 types × 3 tiers each.

- [ ] **3.1** 4 Talisman types (Dexer, Tamer, Sampire, TH) × 3 tiers (T3/T2/T1)
- [ ] **3.2** PvP disable mechanic (drops to backpack on flag, 5-min cooldown)
- [ ] **3.3** 7-day gameplay timers
- [ ] **3.4** Skill reduction mechanic (reduces other skills to 0)
- [ ] **3.5** Mage Stone crafting ingredient
- [ ] **3.6** Ability gating: Bushido/WeaponMoves (Dexer), Chivalry/Necro (Sampire)

### Phase 4: Jewelry System
Time-limited progression items from siege coins.

- [ ] **4.1** PvP jewelry: Ring, Bracelet, Earrings, Glasses (+2.5 Str/Dex/Int each, +10 max)
- [ ] **4.2** 7-day continuous timers (not gameplay)
- [ ] **4.3** Staggered renewal mechanic
- [ ] **4.4** Siege coin acquisition
- [ ] **4.5** Bound on equip
- [ ] **4.6** Separate crafter jewelry system

### Phase 5: Siege System
Faction warfare over towns.

- [ ] **5.1** Hourly siege rotation (Yew → Skara Brae → Jhelom)
- [ ] **5.2** Three-way faction battles
- [ ] **5.3** Town control persistence
- [ ] **5.4** Objectives and scoring
- [ ] **5.5** Rewards distribution

### Phase 6: Currency Systems
Multiple progression currencies.

- [ ] **6.1** Relics (dungeon drops, crafting materials)
- [ ] **6.2** Faction Points (account-bound, siege rewards)
- [ ] **6.3** Silver (character-bound, daily content)
- [ ] **6.4** Tournament Coins (PvP rewards)

### Phase 7: Daily Content
Repeatable content for engagement.

- [ ] **7.1** Daily bounty system (3 bounties/day)
- [ ] **7.2** Faction quest boss (daily spawn)
- [ ] **7.3** Silver vendor
- [ ] **7.4** Cosmetic rewards

### Phase 8: Tournament System
Competitive PvP with rankings.

- [ ] **8.1** Registration system
- [ ] **8.2** Bracket generation
- [ ] **8.3** Match processing
- [ ] **8.4** Glicko-2 rating integration
- [ ] **8.5** Rewards (Tournament Coins)

### Phase 9: Dungeons
PvM content with relic drops.

- [ ] **9.1** Easy dungeons (5-10 relics/hour)
- [ ] **9.2** Medium dungeons (10-15 relics/hour)
- [ ] **9.3** Hard dungeons (20-30 relics/hour)
- [ ] **9.4** Extreme dungeons (40-60 relics/hour)
- [ ] **9.5** Boss encounters
- [ ] **9.6** Weekly rotation (+20%/-20% loot)

### Phase 10: New Player Experience
Onboarding and protection.

- [ ] **10.1** Young player protection (14 days)
- [ ] **10.2** Training island content
- [ ] **10.3** Starter quests
- [ ] **10.4** New Player Guide NPC
- [ ] **10.5** Ferry to mainland

### Phase 11: Polish & Launch
Final testing and preparation.

- [ ] **11.1** Complete admin commands
- [ ] **11.2** Load testing
- [ ] **11.3** Security audit
- [ ] **11.4** Bug fixes
- [ ] **11.5** Documentation cleanup

---

## Database Setup

### Current: SQLite (Default)
SQLite is enabled by default and requires no setup. The database file is automatically created at:
```
Distribution/Saves/sphere51a.db
```

### Optional: PostgreSQL (Production)
For production with multiple servers or high load, you can switch to PostgreSQL:

1. Download PostgreSQL 15+ from https://www.postgresql.org/download/
2. Install with default settings
3. Create database:
```sql
CREATE DATABASE "51alpha";
CREATE USER "51alpha_app" WITH PASSWORD 'your_secure_password';
GRANT ALL PRIVILEGES ON DATABASE "51alpha" TO "51alpha_app";
```
4. Edit `Distribution/Configuration/modernuo.json`:
```json
{
  "s51a.database.provider": "postgresql",
  "s51a.database.connectionString": "Host=localhost;Database=51alpha;Username=51alpha_app;Password=your_secure_password"
}
```

---

## Key Design Decisions (Summary)

These are **LOCKED** - don't change without good reason.

### Factions
- **3 Factions**: The Golden Shield (Trinsic), The Bridgefolk (Vesper), The Lycaeum Order (Moonglow)
- **Guild Required**: Must be in a guild to join faction (1-member guilds OK)
- **7-day cooldown** on faction changes
- **10% NPC discount** in home city
- **Equal power** at launch - no faction-specific perks

### PvP Balance
- **Talismans DISABLED** in PvP (instantly unequipped)
- **Runic bonuses DISABLED** in PvP (base material stats only)
- **Jewelry ACTIVE** in PvP (provides some stats)
- **No enchanted/imbued items** - removed from game entirely
- **Skill-based combat** - gear provides minimal advantage

### Economy
- **Relics**: Core currency, only from dungeons
- **Bound on equip**: Talismans and jewelry cannot be traded after wearing
- **Tradeable**: Weapons, armor, housing, relics, gold
- **Guild treasury**: Gold/cheques only, protected from sieges

### Spells (51a Style)
- **Immediate targeting** - cursor appears when you cast
- **No damage interrupt** - getting hit doesn't cancel spells
- **Free movement** - walk while casting
- **50% mana on fizzle** - punishing but not devastating
- **No Faster Casting** - FC removed from all equipment
- **Scroll bonuses** - 43% cheaper mana, 0.5s faster for circle 3+

---

## Quick Commands Reference

### Build & Run
```bash
# Build the server
publish.cmd Release win x64

# Run the server
cd Distribution
ModernUO.exe
```

### In-Game Admin Commands (Planned)
```
[SetGuildFaction <guild> <faction>  - Set guild's faction
[GetGuildFaction <guild>            - Check guild's faction
[FactionStats                       - Show faction statistics
[ReloadFactions                     - Reload from database
[TestFactions                       - Run faction tests
[TestFactionDB                      - Test database connection
[RemoveAllFC                        - Remove Faster Casting from all items
```

---

## Files to Read

| Purpose | File | Authority |
|---------|------|-----------|
| **Topic lookup** | `INDEX.md` | Navigation |
| **All design decisions** | `Design/DESIGN_DECISIONS.md` | TIER 1 |
| PvP combat details | `Design/PVP_COMBAT_SYSTEM.md` | TIER 2 |
| Ore/crafting details | `Design/SEASONAL_ORE_SYSTEM.md` | TIER 2 |
| Talisman tech spec | `../specs/talisman-system.md` | TIER 2 |
| AI assistant context | `CLAUDE.md` | TIER 3 |
| This roadmap | `ROADMAP.md` | TIER 3 |

---

## Notes

- **SQLite works out of the box** - faction system uses SQLite by default
- **PostgreSQL is optional** - only needed for production with high load
- **Design is locked** - implementation details can be tuned, but core decisions shouldn't change
- **Crafting decision MADE** - Option E-1 (full runic, PvE-only bonuses)

## Next Steps

### Phase 2.5: PvP Combat Implementation
1. **Implement PvP combat formulas** - BaseWeapon.cs and BaseArmor.cs changes
2. **Implement 7-tier armor system** - Update ore AR bonuses
3. **Implement shield flat AR values** - Remove scaling, add faction shields
4. **Implement mining stones** - SharpeningStone and AccuracyStone items
5. **Remove deprecated items** - Weapons, shields, chainmail pieces

### Phase 3: Talisman System
6. **Implement 4 Talisman types** - Dexer, Tamer, Sampire, Treasure Hunter
7. **Implement T3/T2/T1 tiers** - Each type has 3 tiers, PvP disable, 7-day gameplay timers
8. **Implement skill reduction** - Talisman reduces other skills to 0
9. **Implement ability gating** - Bushido/WeaponMoves (Dexer), Chiv/Necro (Sampire)

### Phase 4: Jewelry System
10. **Implement PvP Jewelry** - Ring/Bracelet/Earrings/Glasses (+2.5 Str/Dex/Int each)
11. **Implement timers** - 7-day continuous, bound on equip

**Full implementation checklists in `Documentation/PVP_COMBAT_SYSTEM.md`**
