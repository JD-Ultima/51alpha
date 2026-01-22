# NEW FACTION SYSTEM REDESIGN - Implementation Plan

## Overview

This plan redesigns the 51alpha faction system to include player-level status choices (PvP/PvM/Crafter) with seasonal cooldowns, integrates it into the guild UI by replacing the Diplomacy tab with a Factions tab, and implements comprehensive leaderboards.

---

## Design Requirements Summary

### Player Status System
- **Three Status Types**: PvP, PvM, Crafter
- **Status Binding**: Hybrid approach - status persists but becomes inactive when not in factioned guild
- **Guild Faction Changes**: Keep existing player choices when guild changes faction
- **Season System**: Quarterly (3 months) - status change allowed during first week of each season
- **Confirmation**: Two-step confirmation when choosing status (warning about season lock)

### Leaderboard Tracking
- **PvP Board**: Faction kills/deaths, K/D ratio
- **PvM Board**: Boss kills, monster points
- **Crafter Board**: BOD completions, items crafted

### UI Changes
- **Remove**: Diplomacy tab
- **Add**: Factions tab with:
  - Leader controls (faction selection)
  - Player status selection
  - Leaderboard viewing
  - Faction statistics

### Performance Requirements
- Must scale to thousands of players
- Efficient database queries with proper indexing
- Write-through caching for fast lookups
- No server lag on frequent operations

---

## Architecture Design

### 1. Database Schema

#### Player Faction Status Table
```sql
CREATE TABLE s51a_player_faction_status (
    character_serial BIGINT PRIMARY KEY,
    account_id TEXT NOT NULL,
    character_name TEXT NOT NULL,

    -- Current guild relationship
    guild_serial BIGINT,
    current_faction_id INTEGER,

    -- Player's chosen status
    faction_status TEXT NOT NULL DEFAULT 'inactive',  -- 'inactive', 'pvp', 'pvm', 'crafter'
    status_chosen_at TIMESTAMPTZ,
    status_can_change_after TIMESTAMPTZ,  -- Pre-calculated cooldown

    -- Season tracking
    current_season_id INTEGER,

    -- Activity tracking
    last_login TIMESTAMPTZ,
    last_activity TIMESTAMPTZ,

    -- Metadata
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),

    FOREIGN KEY (current_faction_id) REFERENCES s51a_factions(faction_id) ON DELETE SET NULL,
    FOREIGN KEY (guild_serial) REFERENCES s51a_guild_factions(guild_serial) ON DELETE SET NULL,
    FOREIGN KEY (current_season_id) REFERENCES s51a_seasons(season_id)
);

-- Indexes for performance
CREATE INDEX idx_player_faction_guild ON s51a_player_faction_status(guild_serial);
CREATE INDEX idx_player_faction_active ON s51a_player_faction_status(current_faction_id, faction_status) WHERE faction_status != 'inactive';
CREATE INDEX idx_player_faction_account ON s51a_player_faction_status(account_id);
CREATE INDEX idx_player_faction_season ON s51a_player_faction_status(current_season_id, current_faction_id);
```

#### Season System Table
```sql
CREATE TABLE s51a_seasons (
    season_id SERIAL PRIMARY KEY,
    season_name TEXT NOT NULL,  -- e.g., "Q1 2026", "Winter 2026"
    season_number INTEGER NOT NULL,  -- Sequential: 1, 2, 3...
    start_date TIMESTAMPTZ NOT NULL,
    end_date TIMESTAMPTZ NOT NULL,
    status_change_window_end TIMESTAMPTZ NOT NULL,  -- start_date + 7 days
    is_active BOOLEAN DEFAULT FALSE,
    winning_faction_id INTEGER,
    created_at TIMESTAMPTZ DEFAULT NOW(),

    FOREIGN KEY (winning_faction_id) REFERENCES s51a_factions(faction_id)
);

-- Only one active season at a time
CREATE UNIQUE INDEX idx_season_active ON s51a_seasons(is_active) WHERE is_active = TRUE;
CREATE INDEX idx_season_dates ON s51a_seasons(start_date, end_date);
```

#### Leaderboard Stats Tables
```sql
-- PvP Leaderboard
CREATE TABLE s51a_pvp_stats (
    character_serial BIGINT PRIMARY KEY,
    faction_id INTEGER NOT NULL,
    season_id INTEGER NOT NULL,

    kills INTEGER DEFAULT 0,
    deaths INTEGER DEFAULT 0,
    assists INTEGER DEFAULT 0,
    points INTEGER DEFAULT 0,  -- Calculated: kills * 10 - deaths * 5

    last_kill_at TIMESTAMPTZ,
    longest_streak INTEGER DEFAULT 0,
    current_streak INTEGER DEFAULT 0,

    updated_at TIMESTAMPTZ DEFAULT NOW(),

    FOREIGN KEY (character_serial) REFERENCES s51a_player_faction_status(character_serial) ON DELETE CASCADE,
    FOREIGN KEY (faction_id) REFERENCES s51a_factions(faction_id),
    FOREIGN KEY (season_id) REFERENCES s51a_seasons(season_id)
);

CREATE INDEX idx_pvp_leaderboard ON s51a_pvp_stats(season_id, faction_id, points DESC);
CREATE INDEX idx_pvp_character ON s51a_pvp_stats(character_serial, season_id);

-- PvM Leaderboard
CREATE TABLE s51a_pvm_stats (
    character_serial BIGINT PRIMARY KEY,
    faction_id INTEGER NOT NULL,
    season_id INTEGER NOT NULL,

    boss_kills INTEGER DEFAULT 0,
    monster_points INTEGER DEFAULT 0,  -- Based on monster difficulty
    dungeon_completions INTEGER DEFAULT 0,
    points INTEGER DEFAULT 0,  -- Total score

    last_activity_at TIMESTAMPTZ,

    updated_at TIMESTAMPTZ DEFAULT NOW(),

    FOREIGN KEY (character_serial) REFERENCES s51a_player_faction_status(character_serial) ON DELETE CASCADE,
    FOREIGN KEY (faction_id) REFERENCES s51a_factions(faction_id),
    FOREIGN KEY (season_id) REFERENCES s51a_seasons(season_id)
);

CREATE INDEX idx_pvm_leaderboard ON s51a_pvm_stats(season_id, faction_id, points DESC);

-- Crafter Leaderboard
CREATE TABLE s51a_crafter_stats (
    character_serial BIGINT PRIMARY KEY,
    faction_id INTEGER NOT NULL,
    season_id INTEGER NOT NULL,

    bods_completed INTEGER DEFAULT 0,
    exceptional_items INTEGER DEFAULT 0,
    total_items_crafted INTEGER DEFAULT 0,
    points INTEGER DEFAULT 0,  -- BODs worth more

    last_activity_at TIMESTAMPTZ,

    updated_at TIMESTAMPTZ DEFAULT NOW(),

    FOREIGN KEY (character_serial) REFERENCES s51a_player_faction_status(character_serial) ON DELETE CASCADE,
    FOREIGN KEY (faction_id) REFERENCES s51a_factions(faction_id),
    FOREIGN KEY (season_id) REFERENCES s51a_seasons(season_id)
);

CREATE INDEX idx_crafter_leaderboard ON s51a_crafter_stats(season_id, faction_id, points DESC);

-- Activity Log (for auditing and anti-cheat)
CREATE TABLE s51a_faction_activity_log (
    log_id BIGSERIAL PRIMARY KEY,
    character_serial BIGINT NOT NULL,
    activity_type TEXT NOT NULL,  -- 'pvp_kill', 'boss_kill', 'bod_complete', 'status_change'
    points_awarded INTEGER,
    details JSONB,  -- Flexible storage for activity context
    timestamp TIMESTAMPTZ DEFAULT NOW(),

    FOREIGN KEY (character_serial) REFERENCES s51a_player_faction_status(character_serial) ON DELETE CASCADE
);

CREATE INDEX idx_activity_log_character ON s51a_faction_activity_log(character_serial, timestamp DESC);
CREATE INDEX idx_activity_log_type ON s51a_faction_activity_log(activity_type, timestamp DESC);
```

#### Config Updates
```sql
-- Add to s51a_config table
INSERT INTO s51a_config (config_key, config_value) VALUES
('current_season_id', '1'),
('season_duration_months', '3'),
('status_change_window_days', '7');
```

---

### 2. Caching Layer Architecture

#### PlayerFactionStatus Cache
```csharp
public class PlayerFactionStatus
{
    public Serial CharacterSerial { get; set; }
    public string AccountId { get; set; }
    public string CharacterName { get; set; }

    // Guild relationship
    public Serial? GuildSerial { get; set; }
    public S51aFaction CurrentFaction { get; set; }

    // Player choice
    public FactionStatusType Status { get; set; }  // Inactive, PvP, PvM, Crafter
    public DateTime? StatusChosenAt { get; set; }
    public DateTime? StatusCanChangeAfter { get; set; }

    // Season tracking
    public int? CurrentSeasonId { get; set; }

    // Activity
    public DateTime LastLogin { get; set; }
    public DateTime LastActivity { get; set; }

    // Computed properties
    public bool IsActive => Status != FactionStatusType.Inactive;
    public bool CanChangeStatus => !StatusCanChangeAfter.HasValue || DateTime.UtcNow >= StatusCanChangeAfter.Value;
    public TimeSpan? StatusChangeCooldown => StatusCanChangeAfter.HasValue ? StatusCanChangeAfter.Value - DateTime.UtcNow : null;
}

public enum FactionStatusType
{
    Inactive = 0,  // Not in factioned guild or chose to opt out
    PvP = 1,       // Flagged for PvP, on PvP leaderboard
    PvM = 2,       // Not flagged, on PvM leaderboard
    Crafter = 3    // Not flagged, on Crafter leaderboard
}
```

#### Cache Manager
```csharp
public static class S51aPlayerFactionSystem
{
    // Write-through cache (follows S51aFactionSystem pattern)
    private static readonly Dictionary<Serial, PlayerFactionStatus> _cache = new();
    private static readonly object _cacheLock = new();

    // Fast lookup without DB hit
    public static PlayerFactionStatus GetPlayerStatus(Mobile player)
    {
        lock (_cacheLock)
        {
            if (_cache.TryGetValue(player.Serial, out var status))
                return status;
        }

        // Lazy load on first access
        return LoadPlayerStatus(player.Serial);
    }

    // Update with write-through
    public static bool SetPlayerStatus(Mobile player, FactionStatusType newStatus, out string error)
    {
        // 1. Validation
        var current = GetPlayerStatus(player);
        if (!current.CanChangeStatus)
        {
            error = $"You cannot change your status until {current.StatusCanChangeAfter:yyyy-MM-dd}";
            return false;
        }

        // 2. Write to database
        if (!PlayerFactionRepository.UpdatePlayerStatus(player.Serial, newStatus))
        {
            error = "Database error";
            return false;
        }

        // 3. Reload from DB (ensures timestamps are accurate)
        var updated = PlayerFactionRepository.GetPlayerStatus(player.Serial);

        // 4. Update cache
        lock (_cacheLock)
        {
            _cache[player.Serial] = updated;
        }

        // 5. Update notoriety if PvP status
        if (newStatus == FactionStatusType.PvP || current.Status == FactionStatusType.PvP)
        {
            player.InvalidateProperties();
            player.Delta(MobileDelta.Noto);
        }

        error = null;
        return true;
    }

    // Initialize cache on server start
    public static void Initialize()
    {
        EventSink.WorldLoad += OnWorldLoad;
        EventSink.Login += OnPlayerLogin;
        EventSink.Logout += OnPlayerLogout;
    }

    private static void OnWorldLoad()
    {
        // Load only active players (last login within 30 days)
        var activePlayers = PlayerFactionRepository.GetActivePlayers(TimeSpan.FromDays(30));

        lock (_cacheLock)
        {
            foreach (var status in activePlayers)
            {
                _cache[status.CharacterSerial] = status;
            }
        }

        Console.WriteLine($"Loaded {activePlayers.Count} active player faction statuses into cache");
    }

    private static void OnPlayerLogin(Mobile player)
    {
        // Ensure player status exists
        var status = GetPlayerStatus(player);

        // Update last login
        PlayerFactionRepository.UpdateLastLogin(player.Serial);

        // Sync guild changes
        SyncGuildMembership(player);
    }

    // Periodic cache cleanup (every hour)
    private static void PeriodicCacheCleanup()
    {
        var cutoff = DateTime.UtcNow - TimeSpan.FromDays(30);

        lock (_cacheLock)
        {
            var toRemove = _cache
                .Where(kvp => kvp.Value.LastLogin < cutoff)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var serial in toRemove)
                _cache.Remove(serial);
        }
    }
}
```

---

### 3. Status Change Scenarios

#### Scenario 1: Player Chooses Status for First Time
```
Player in factioned guild → Opens Factions tab → Clicks status button →
Confirmation gump appears → Clicks Yes → Status set, cooldown calculated
```

**Logic**:
1. Check if player is in factioned guild
2. If not, show error: "You must be in a guild with a faction"
3. Show confirmation gump with season lock warning
4. On confirm: Set status, calculate `can_change_after` = end of current season + 7 days
5. Create leaderboard entry for appropriate board

#### Scenario 2: Player Leaves Guild
```
Player leaves guild → Status changes to 'inactive' → Stats persist → Not on leaderboards
```

**Logic**:
1. Hook `Guild.RemoveMember()`
2. Update `s51a_player_faction_status`: Set `status = 'inactive'`, `guild_serial = NULL`, `current_faction_id = NULL`
3. Leaderboard stats remain but player drops off leaderboards (filter: `status != 'inactive'`)
4. Cache updated

#### Scenario 3: Player Joins New Factioned Guild
```
Player joins guild with faction → Status reactivates to previous choice → Back on leaderboards
```

**Logic**:
1. Hook `Guild.AddMember()`
2. Check if guild has faction
3. If yes: Update `guild_serial`, `current_faction_id`, reactivate previous status
4. If status was 'inactive', prompt player to choose status
5. Leaderboard entry updated with new faction
6. Cache updated

#### Scenario 4: Guild Leader Changes Faction
```
Leader changes guild faction → All members keep their status choice →
Faction ID updated → Leaderboard entries moved to new faction
```

**Logic**:
1. Hook `S51aFactionSystem.ChangeFaction()`
2. Query all guild members from `s51a_player_faction_status`
3. Batch update: Set new `current_faction_id` for all members
4. Move leaderboard entries to new faction (keep stats)
5. Bulk cache update

#### Scenario 5: Season Change Window Opens
```
New season starts → First 7 days = status change allowed →
Players can switch between PvP/PvM/Crafter
```

**Logic**:
1. Scheduled job creates new season (cron or Timer)
2. Update all `status_can_change_after = NULL` for active players
3. After 7 days, recalculate cooldowns: `status_can_change_after = season_end_date + 7 days`
4. Notifications sent to online players

#### Scenario 6: Player Tries to Change Mid-Season
```
Player clicks status button → System checks cooldown → Shows error with date
```

**Logic**:
1. Check `status_can_change_after`
2. If future date: Show error "Cannot change until [date]"
3. Offer info about season system

---

### 4. UI Design - New Factions Tab

#### Tab Structure (Replaces Diplomacy)
```
[My Guild] [Guild Roster] [Factions]
```

#### Factions Tab Layout

**Top Section: Guild Faction Info**
- Guild's current faction (icon + name)
- Faction hue/color display
- Home city
- Member count in faction

**Middle Section: Player Status Selection** (3 buttons)
```
[====== Choose Your Status ======]

[PvP Combatant]
• Flagged for PvP against other factions
• Appears on PvP Leaderboard
• Earns points from kills
[SELECT] or [ACTIVE ✓]

[PvM Adventurer]
• Safe in towns (not flagged)
• Appears on PvM Leaderboard
• Earns points from bosses & monsters
[SELECT] or [ACTIVE ✓]

[Crafter / Supplier]
• Safe in towns (not flagged)
• Appears on Crafter Leaderboard
• Earns points from BODs & items
[SELECT] or [ACTIVE ✓]

Status Change Available: [Next Season - 2026-04-01] or [Now (Season Change Window)]
```

**Bottom Section: Leaderboards** (Tabs)
```
[PvP] [PvM] [Crafter]

Top 10 for [Your Faction]:
1. PlayerName - 1500 pts (150 kills, 30 deaths)
2. PlayerName2 - 1200 pts (...)
...

[View Full Leaderboard] button → Opens larger gump with all factions
```

**Leader-Only Section** (If player is guild leader)
```
[====== Guild Leader Controls ======]

Current Faction: [GoldenShield ▼]
Can change: [Yes] or [Cooldown: 3 days]

[Change Guild Faction] button
[View Faction Statistics] button
```

#### Confirmation Gump Design
```
╔════════════════════════════════════════╗
║   CONFIRM FACTION STATUS CHOICE        ║
╠════════════════════════════════════════╣
║                                        ║
║  You are choosing: [PvP Combatant]    ║
║                                        ║
║  ⚠️ IMPORTANT:                         ║
║  • You CANNOT change this choice       ║
║    until the next season               ║
║  • Next change available:              ║
║    2026-04-01 (Start of Q2)            ║
║                                        ║
║  Are you sure?                         ║
║                                        ║
║         [YES]        [NO]              ║
╚════════════════════════════════════════╝
```

---

### 5. Performance Optimizations

#### Database Indexing Strategy
```sql
-- Leaderboard queries (most frequent)
-- Query: SELECT TOP 100 WHERE season_id = X AND faction_id = Y ORDER BY points DESC
CREATE INDEX idx_pvp_leaderboard ON s51a_pvp_stats(season_id, faction_id, points DESC);
CREATE INDEX idx_pvm_leaderboard ON s51a_pvm_stats(season_id, faction_id, points DESC);
CREATE INDEX idx_crafter_leaderboard ON s51a_crafter_stats(season_id, faction_id, points DESC);

-- Player lookup (very frequent)
CREATE INDEX idx_player_faction_active ON s51a_player_faction_status(current_faction_id, faction_status)
    WHERE faction_status != 'inactive';

-- Guild operations (frequent on guild changes)
CREATE INDEX idx_player_faction_guild ON s51a_player_faction_status(guild_serial);
```

#### Cache Management
- **Hot Data**: Active players (logged in last 30 days) stay in cache
- **Cold Data**: Inactive players loaded on-demand, evicted after 1 hour
- **Write Pattern**: Write-through (database first, then cache)
- **Consistency**: Strong consistency (no stale reads)

#### Bulk Operations
```csharp
// When guild changes faction, update all members in single query
public static void UpdateGuildMembersFaction(Serial guildSerial, int newFactionId)
{
    using var conn = PostgresConnection.GetConnection();
    using var cmd = new NpgsqlCommand(@"
        UPDATE s51a_player_faction_status
        SET current_faction_id = @factionId, updated_at = NOW()
        WHERE guild_serial = @guildSerial AND faction_status != 'inactive'
    ", conn);

    cmd.Parameters.AddWithValue("@guildSerial", (long)guildSerial);
    cmd.Parameters.AddWithValue("@factionId", newFactionId);

    cmd.ExecuteNonQuery();
}
```

#### Leaderboard Caching
```csharp
// Cache top 100 for each faction/season combo
private static readonly Dictionary<(int seasonId, int factionId, string boardType), List<LeaderboardEntry>> _leaderboardCache = new();
private static DateTime _leaderboardCacheExpiry = DateTime.MinValue;

public static List<LeaderboardEntry> GetLeaderboard(int seasonId, int factionId, string boardType)
{
    var key = (seasonId, factionId, boardType);

    // Refresh every 5 minutes
    if (DateTime.UtcNow > _leaderboardCacheExpiry)
    {
        RefreshLeaderboardCache();
        _leaderboardCacheExpiry = DateTime.UtcNow.AddMinutes(5);
    }

    return _leaderboardCache.TryGetValue(key, out var cached) ? cached : new List<LeaderboardEntry>();
}
```

---

### 6. Critical Files to Modify/Create

#### New Files to Create

**Database**:
- `002_Player_Faction_Status_Schema.sql` - Player status tables
- `003_Season_System_Schema.sql` - Season tracking tables
- `004_Leaderboard_Stats_Schema.sql` - PvP/PvM/Crafter stats tables

**Core System**:
- `PlayerFactionStatus.cs` - Data model
- `S51aPlayerFactionSystem.cs` - Cache manager & business logic
- `PlayerFactionRepository.cs` - Database access layer
- `S51aSeasonSystem.cs` - Season management
- `FactionStatusType.cs` - Enum definition

**UI**:
- `GuildFactionsGump.cs` - New Factions tab (replaces DiplomacyGump)
- `FactionStatusChoiceGump.cs` - Status selection UI
- `FactionStatusConfirmGump.cs` - Confirmation dialog
- `FactionLeaderboardGump.cs` - Full leaderboard view

**Tracking**:
- `FactionPvPTracker.cs` - Hooks PlayerMobile.OnDeath for PvP kills
- `FactionPvMTracker.cs` - Hooks BaseCreature.OnDeath for PvM points
- `FactionCrafterTracker.cs` - Hooks BOD completion & crafting

**Extensions**:
- `MobileFactionExtensions.cs` - Extension methods for Mobile class

#### Files to Modify

**Guild.cs** (Projects\UOContent\Misc\Guild.cs):
- Hook `AddMember()` to sync player faction status
- Hook `RemoveMember()` to deactivate player status

**S51aFactionSystem.cs**:
- Hook `ChangeFaction()` to update all guild members

**BaseGuildGump.cs**:
- Remove Diplomacy button (button 3)
- Add Factions button (button 3)

**PlayerMobile.cs** (Optional - if using property reference):
- Add `public PlayerFactionStatus FactionStatus { get; set; }`

**Notoriety.cs**:
- Check PvP status for faction PvP flagging

---

### 7. Migration & Rollout Plan

#### Phase 1: Database Setup
1. Create new tables via migrations
2. Seed first season (Q1 2026)
3. Migrate existing guild faction data

#### Phase 2: Core System
1. Implement PlayerFactionStatus cache
2. Implement season system
3. Add guild hooks for member changes

#### Phase 3: UI
1. Create Factions tab gump
2. Add status selection/confirmation gumps
3. Remove Diplomacy tab

#### Phase 4: Tracking Systems
1. Implement PvP kill tracking
2. Implement PvM point tracking
3. Implement BOD/crafting tracking

#### Phase 5: Testing & Tuning
1. Load test with simulated player count
2. Optimize database queries
3. Tune cache refresh rates
4. Balance point values

---

### 8. Testing Scenarios

**Functional Tests**:
- [ ] Player chooses status for first time
- [ ] Confirmation gump shows correct season date
- [ ] Status change blocked mid-season
- [ ] Status change allowed during season change window
- [ ] Player leaves guild → status goes inactive
- [ ] Player rejoins guild → status reactivates
- [ ] Player joins different guild → status persists
- [ ] Guild changes faction → all members update
- [ ] PvP kill awards points correctly
- [ ] PvM monster kill awards points
- [ ] BOD completion awards points
- [ ] Leaderboards show correct rankings
- [ ] Leaderboards filter by faction correctly

**Performance Tests**:
- [ ] 1000 concurrent players - status lookups <5ms
- [ ] Guild faction change with 100 members <100ms
- [ ] Leaderboard query (top 100) <50ms
- [ ] Cache hit rate >95%
- [ ] Database connection pool handles load

**Edge Cases**:
- [ ] Player deleted → cascade deletes stats
- [ ] Guild disbanded → members go inactive
- [ ] Season transition → cooldowns reset correctly
- [ ] Server restart → cache rebuilds correctly
- [ ] Player has no guild → UI shows appropriate message

---

## Verification Steps

After implementation:

1. **Test Status Selection**:
   - Create test characters in factioned guilds
   - Verify each status type (PvP/PvM/Crafter) can be selected
   - Verify confirmation gump appears and blocks repeat changes

2. **Test Guild Operations**:
   - Have player leave guild → verify status goes inactive
   - Have player join new guild → verify status reactivates
   - Change guild faction → verify all members update

3. **Test Leaderboards**:
   - Generate PvP kills → verify points appear
   - Kill bosses → verify PvM points appear
   - Complete BODs → verify Crafter points appear
   - Check leaderboards show correct top 10

4. **Test Season System**:
   - Create test season with short duration
   - Verify status change blocked outside window
   - Verify status change allowed during first week

5. **Performance Test**:
   - Load 100+ NPCs as "players" with faction status
   - Measure cache lookup times
   - Measure leaderboard query times
   - Verify no server lag

---

## Summary

This redesign transforms the faction system from guild-only to player-centric with:
- **Player choice** persisting across guild changes
- **Seasonal cooldowns** preventing frequent switching
- **Three distinct playstyles** with separate leaderboards
- **Scalable architecture** using write-through caching
- **Clean UI integration** via new Factions tab

The system follows established patterns from the existing codebase (S51aFactionSystem, GlickoRepository) and maintains strong consistency while optimizing for read-heavy workloads (leaderboards).
