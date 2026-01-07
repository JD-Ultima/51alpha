# QUICKSTART: Three-Faction System Deployment

**Estimated Time**: 15 minutes
**Prerequisites**: PostgreSQL 12+ installed and running

---

## STEP 1: DATABASE SETUP (5 minutes)

### 1.1 Create Database
```bash
# Connect to PostgreSQL as superuser
psql -U postgres

# Create database
CREATE DATABASE sphere51a;

# Exit psql
\q
```

### 1.2 Execute Schema Migration
```bash
# Navigate to project directory
cd "C:\Users\USER-002\Desktop\New Test\51a-style-ModernUo-51alpha\51a-style-ModernUo-51alpha"

# Execute migration (Windows PowerShell)
Get-Content "Distribution\Data\Postgres\Migrations\001_Factions_Schema.sql" | psql -U postgres -d sphere51a

# OR (Git Bash / WSL)
psql -U postgres -d sphere51a -f Distribution/Data/Postgres/Migrations/001_Factions_Schema.sql
```

### 1.3 Verify Schema
```bash
psql -U postgres -d sphere51a -c "\dt s51a_*"
```

**Expected Output**:
```
              List of relations
 Schema |          Name           | Type  |  Owner
--------+-------------------------+-------+----------
 public | s51a_config             | table | postgres
 public | s51a_faction_points     | table | postgres
 public | s51a_faction_points_log | table | postgres
 public | s51a_factions           | table | postgres
 public | s51a_guild_factions     | table | postgres
 public | s51a_seasons            | table | postgres
 public | s51a_town_control       | table | postgres
(7 rows)
```

### 1.4 Verify Faction Data Seeded
```bash
psql -U postgres -d sphere51a -c "SELECT * FROM s51a_factions;"
```

**Expected Output**:
```
 faction_id |   faction_name    | faction_hue | home_city
------------+-------------------+-------------+------------
          1 | The Golden Shield |        2721 | Trinsic
          2 | The Bridgefolk    |        2784 | Vesper
          3 | The Lycaeum Order |        2602 | Moonglow
(3 rows)
```

---

## STEP 2: CONFIGURE DATABASE CONNECTION (2 minutes)

### 2.1 Update Connection String
Edit: `Projects/UOContent/Sphere51a/Core/Database/PostgresConnection.cs`

**Find** (around line 15):
```csharp
private static string _connectionString = "Host=localhost;Database=sphere51a;Username=postgres;Password=your_password_here";
```

**Replace** with your PostgreSQL credentials:
```csharp
private static string _connectionString = "Host=localhost;Database=sphere51a;Username=postgres;Password=YOUR_ACTUAL_PASSWORD";
```

**Security Note**: For production, use environment variables:
```csharp
private static string _connectionString = Environment.GetEnvironmentVariable("SPHERE51A_DB_CONN")
    ?? "Host=localhost;Database=sphere51a;Username=postgres;Password=postgres";
```

---

## STEP 3: BUILD AND START SERVER (5 minutes)

### 3.1 Build Project
```bash
cd "C:\Users\USER-002\Desktop\New Test\51a-style-ModernUo-51alpha\51a-style-ModernUo-51alpha"
dotnet build Projects/UOContent/UOContent.csproj
```

**Expected Output**:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 3.2 Start Server
```bash
# Windows
.\modernuo.exe

# Linux/macOS
./modernuo
```

### 3.3 Verify Initialization
**Look for console output**:
```
========================================
=== Sphere51a System Initialization ===
========================================
[Sphere51a] Step 1/4: Loading configuration...
[Sphere51a] Server launch date: 2025-12-15 12:00:00 UTC
[Sphere51a] Week 1 lockout: Active (expires in 7 days)
[Sphere51a] Step 2/4: Initializing faction system...
[Sphere51a] Loading guild factions from database...
[Sphere51a] Faction Statistics:
  - The Golden Shield: 0 guild(s)
  - The Bridgefolk: 0 guild(s)
  - The Lycaeum Order: 0 guild(s)
[Sphere51a] Faction system initialized - 0 guild(s) loaded
[Sphere51a] Step 3/4: Initializing vendor discount system...
[Sphere51a] Vendor discount system initialized
[Sphere51a] Step 4/4: Registering test commands...
[Sphere51a] Faction test commands registered
========================================
=== Sphere51a Initialization Complete ===
========================================
```

---

## STEP 4: VERIFY FUNCTIONALITY (3 minutes)

### 4.1 Run Test Suite
**In-game** (as Admin character):
```
[TestFactions
```

**Expected Output**:
```
=== Running Faction System Tests ===
Test 1: Verify 3 factions loaded...
  PASS: 3 factions loaded
Test 2: Verify faction definitions...
  PASS: Faction definitions correct
Test 3: Test database connectivity...
  PASS: Database connection OK
Test 4: Verify faction system initialized...
  PASS: Faction system initialized
Test 5: Check Week 1 lockout status...
  PASS: Week 1 lockout check functional
Test 6: Test faction lookup methods...
  PASS: Faction lookup methods work

=== Test Results ===
Passed: 6/6
Failed: 0/6
All tests passed!
```

### 4.2 Test Database Connectivity
```
[TestFactionDB
```

**Expected Output**:
```
=== Testing Faction Database ===
Test 1: PostgreSQL connection...
  PASS: Connection successful
Test 2: Verify faction schema...
[Sphere51a] Faction database schema verified
  PASS: Schema verified
Test 3: Read faction data...
  GoldenShield: 0 guilds
  Bridgefolk: 0 guilds
  LycaeumOrder: 0 guilds
  Total: 0 guilds in factions
Database tests complete
```

### 4.3 Test Faction Join (Optional)
1. **Create test guild** (or use existing):
   ```
   [CreateGuild TestGuild
   ```

2. **Join faction** (as guild leader):
   ```
   [FactionJoin GoldenShield
   ```

3. **Verify**:
   ```
   [FactionInfo
   ```

   **Expected**:
   ```
   Faction Information:
   Current Faction: The Golden Shield
   Home City: Trinsic
   Joined: 2025-12-15 12:05:30 UTC
   Next Change Available: 2025-12-22 12:05:30 UTC (7 days)
   ```

---

## STEP 5: VERIFY PERSISTENCE (1 minute)

### 5.1 Check Database
```bash
psql -U postgres -d sphere51a -c "SELECT * FROM s51a_guild_factions;"
```

**Expected Output** (if guild joined):
```
 guild_serial | faction_id |       joined_at        |    last_change_at      | can_change_after
--------------+------------+------------------------+------------------------+-------------------
      1234567 |          1 | 2025-12-15 12:05:30... | 2025-12-15 12:05:30... | 2025-12-22 12:05:30...
(1 row)
```

### 5.2 Restart Server Test
1. **Restart server** (Ctrl+C, then restart)
2. **Check console** for:
   ```
   [Sphere51a] Faction system initialized - 1 guild(s) loaded
   ```
3. **In-game**, run:
   ```
   [FactionInfo
   ```
4. **Verify** faction assignment persisted

---

## TROUBLESHOOTING

### Error: "Failed to get guild faction: Npgsql.PostgresException"
**Cause**: Database connection failed

**Fix**:
```bash
# Test PostgreSQL running
pg_isready

# If not running (Windows):
net start postgresql-x64-14

# If not running (Linux):
sudo systemctl start postgresql
```

### Error: "Schema incomplete - found 0/7 tables"
**Cause**: SQL migration not executed

**Fix**: Re-run Step 1.2

### Error: "Week 1 lockout active"
**This is normal** - faction changes are locked for 7 days after first server launch.

**To bypass** (testing only):
1. Edit `s51a_config` table:
   ```sql
   UPDATE s51a_config
   SET value = '2025-01-01 00:00:00+00'
   WHERE key = 'server_launch_date';
   ```
2. Restart server

**Production**: DO NOT modify server launch date

### Error: "Cannot convert from 'string' to 'int'"
**Cause**: Old build artifacts

**Fix**:
```bash
dotnet clean Projects/UOContent/UOContent.csproj
dotnet build Projects/UOContent/UOContent.csproj --no-incremental
```

---

## POST-DEPLOYMENT CHECKLIST

- [ ] Database created and schema loaded
- [ ] 7 tables exist in `sphere51a` database
- [ ] 3 factions seeded in `s51a_factions`
- [ ] Connection string configured
- [ ] Server builds with 0 errors
- [ ] Server starts successfully
- [ ] Initialization messages appear in console
- [ ] `[TestFactions]` passes all 6 tests
- [ ] `[TestFactionDB]` passes all 3 tests
- [ ] Test guild can join faction
- [ ] Faction assignment persists after restart

---

## QUICK REFERENCE

### Player Commands
```
[FactionJoin GoldenShield    - Join The Golden Shield (Trinsic)
[FactionJoin Bridgefolk      - Join The Bridgefolk (Vesper)
[FactionJoin LycaeumOrder    - Join The Lycaeum Order (Moonglow)
[FactionLeave                - Leave current faction
[FactionInfo                 - Show faction status
```

### Admin Commands
```
[FactionReload               - Reload factions from database
[FactionStats                - Show faction statistics
[TestFactions                - Run test suite
[TestFactionDB               - Test database connectivity
```

### Database Queries
```sql
-- View all guild factions
SELECT gf.guild_serial, f.faction_name, gf.joined_at, gf.can_change_after
FROM s51a_guild_factions gf
JOIN s51a_factions f ON gf.faction_id = f.faction_id;

-- View faction statistics
SELECT f.faction_name, COUNT(gf.guild_serial) as guild_count
FROM s51a_factions f
LEFT JOIN s51a_guild_factions gf ON f.faction_id = gf.faction_id
GROUP BY f.faction_name;

-- Check server launch date
SELECT * FROM s51a_config WHERE key = 'server_launch_date';
```

---

## BACKUP RECOMMENDATIONS

### Daily Backup (Automated)
```bash
# Add to cron (Linux) or Task Scheduler (Windows)
pg_dump -U postgres -d sphere51a -t s51a_guild_factions > /backups/factions_$(date +\%Y\%m\%d).sql
```

### Pre-Maintenance Backup
```bash
pg_dump -U postgres -d sphere51a > /backups/sphere51a_full_backup.sql
```

---

## NEXT STEPS

1. **Monitor first 7 days**: Week 1 lockout prevents faction changes
2. **Test vendor discounts**: Have guilds test 10% discount in home cities
3. **Review logs**: Check for any PostgreSQL connection errors
4. **Plan Phase 2**: Review `IMPLEMENTATION_COMPLETE_Phase1_ThreeFactionSystem.md` for Phase 2 features

---

**Deployment Complete!**

For detailed implementation information, see: `IMPLEMENTATION_COMPLETE_Phase1_ThreeFactionSystem.md`
