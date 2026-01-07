-- =====================================================
-- 51ALPHA FACTION SYSTEM - POSTGRESQL SCHEMA
-- =====================================================
-- Migration: 001_Factions_Schema.sql
-- Created: 2025-12-14
-- Phase: Phase 1 - Three-Faction System Foundation
-- Documentation: PHASE1_VERIFICATION_REPORT_v3.md (Q1: Faction System)
--                PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q5: Database Schema)
-- =====================================================

-- =====================================================
-- CONFIG TABLE (Server-wide configuration)
-- =====================================================
CREATE TABLE IF NOT EXISTS s51a_config (
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_config IS 'Server-wide configuration key-value store';
COMMENT ON COLUMN s51a_config.key IS 'Configuration key (e.g., server_launch_date)';
COMMENT ON COLUMN s51a_config.value IS 'Configuration value (stored as TEXT, parsed by application)';

-- Insert server launch date (only if not already exists)
-- This timestamp is used for Week 1 faction change lockout
INSERT INTO s51a_config (key, value, created_at)
VALUES ('server_launch_date', NOW()::TEXT, NOW())
ON CONFLICT (key) DO NOTHING;

-- =====================================================
-- TABLE 1: s51a_factions (3 static factions)
-- =====================================================
CREATE TABLE IF NOT EXISTS s51a_factions (
    faction_id SERIAL PRIMARY KEY,
    faction_name TEXT NOT NULL UNIQUE,
    faction_hue INTEGER NOT NULL,
    home_city TEXT NOT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_factions IS 'Static faction definitions (The Golden Shield, The Bridgefolk, The Lycaeum Order)';
COMMENT ON COLUMN s51a_factions.faction_id IS 'Primary key (1=GoldenShield, 2=Bridgefolk, 3=LycaeumOrder)';
COMMENT ON COLUMN s51a_factions.faction_name IS 'Faction name (unique identifier)';
COMMENT ON COLUMN s51a_factions.faction_hue IS 'Guild name hue (2721, 2784, 2602 per Phase 1 spec)';
COMMENT ON COLUMN s51a_factions.home_city IS 'Home city for 10% NPC vendor discount';

-- Seed the 3 factions (locked per Phase 1 Q1 specification)
INSERT INTO s51a_factions (faction_id, faction_name, faction_hue, home_city)
VALUES
    (1, 'GoldenShield', 2721, 'Trinsic'),
    (2, 'Bridgefolk', 2784, 'Vesper'),
    (3, 'LycaeumOrder', 2602, 'Moonglow')
ON CONFLICT (faction_id) DO NOTHING;

-- =====================================================
-- TABLE 2: s51a_guild_factions (guild → faction binding)
-- =====================================================
CREATE TABLE IF NOT EXISTS s51a_guild_factions (
    guild_serial BIGINT PRIMARY KEY,
    faction_id INTEGER NOT NULL REFERENCES s51a_factions(faction_id) ON DELETE CASCADE,
    joined_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    last_change_at TIMESTAMPTZ,
    can_change_after TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW(),

    CONSTRAINT check_cooldown CHECK (can_change_after IS NULL OR can_change_after >= last_change_at)
);

COMMENT ON TABLE s51a_guild_factions IS 'Guild faction assignments with 7-day cooldown tracking';
COMMENT ON COLUMN s51a_guild_factions.guild_serial IS 'ModernUO Guild.Serial (unique guild identifier)';
COMMENT ON COLUMN s51a_guild_factions.faction_id IS 'Faction this guild belongs to';
COMMENT ON COLUMN s51a_guild_factions.joined_at IS 'First time this guild joined ANY faction';
COMMENT ON COLUMN s51a_guild_factions.last_change_at IS 'Last time this guild changed factions';
COMMENT ON COLUMN s51a_guild_factions.can_change_after IS 'Pre-calculated: last_change_at + 7 days (for efficient cooldown queries)';

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_guild_factions_faction ON s51a_guild_factions(faction_id);
CREATE INDEX IF NOT EXISTS idx_guild_factions_cooldown ON s51a_guild_factions(can_change_after) WHERE can_change_after IS NOT NULL;

-- =====================================================
-- TABLE 3: s51a_faction_points (account-bound points)
-- =====================================================
-- NOTE: This table is future-proofed for Phase 2+
-- Currently unused but schema is ready
CREATE TABLE IF NOT EXISTS s51a_faction_points (
    account_id TEXT PRIMARY KEY,
    faction_id INTEGER NOT NULL REFERENCES s51a_factions(faction_id) ON DELETE CASCADE,
    points INTEGER DEFAULT 0 CHECK (points >= 0),
    season_id INTEGER,
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_faction_points IS 'Account-bound faction points (future phase - siege/tournament rewards)';
COMMENT ON COLUMN s51a_faction_points.account_id IS 'ModernUO Account.Username (account-bound, not character-bound)';
COMMENT ON COLUMN s51a_faction_points.faction_id IS 'Faction these points belong to';
COMMENT ON COLUMN s51a_faction_points.points IS 'Current point total';
COMMENT ON COLUMN s51a_faction_points.season_id IS 'Season identifier (future: quarterly seasons)';

-- Indexes for leaderboard queries
CREATE INDEX IF NOT EXISTS idx_faction_points_faction ON s51a_faction_points(faction_id);
CREATE INDEX IF NOT EXISTS idx_faction_points_leaderboard ON s51a_faction_points(faction_id, points DESC);

-- =====================================================
-- TABLE 4: s51a_seasons (quarterly season results)
-- =====================================================
-- NOTE: This table is future-proofed for Phase 2+
-- Currently unused but schema is ready
CREATE TABLE IF NOT EXISTS s51a_seasons (
    season_id SERIAL PRIMARY KEY,
    season_name TEXT NOT NULL UNIQUE,
    start_date TIMESTAMPTZ NOT NULL,
    end_date TIMESTAMPTZ NOT NULL,
    winning_faction_id INTEGER REFERENCES s51a_factions(faction_id),
    created_at TIMESTAMPTZ DEFAULT NOW(),

    CONSTRAINT check_season_dates CHECK (end_date > start_date)
);

COMMENT ON TABLE s51a_seasons IS 'Quarterly season tracking and results (future phase)';
COMMENT ON COLUMN s51a_seasons.season_id IS 'Primary key (auto-incremented)';
COMMENT ON COLUMN s51a_seasons.season_name IS 'Season name (e.g., "Season 1 - Q1 2025")';
COMMENT ON COLUMN s51a_seasons.start_date IS 'Season start timestamp';
COMMENT ON COLUMN s51a_seasons.end_date IS 'Season end timestamp';
COMMENT ON COLUMN s51a_seasons.winning_faction_id IS 'Faction that won this season (highest points)';

-- =====================================================
-- TABLE 5: s51a_faction_points_log (audit trail)
-- =====================================================
-- NOTE: This table is future-proofed for Phase 2+
-- Currently unused but schema is ready
CREATE TABLE IF NOT EXISTS s51a_faction_points_log (
    log_id BIGSERIAL PRIMARY KEY,
    account_id TEXT NOT NULL,
    faction_id INTEGER NOT NULL REFERENCES s51a_factions(faction_id) ON DELETE CASCADE,
    points_delta INTEGER NOT NULL,
    reason TEXT NOT NULL,
    timestamp TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_faction_points_log IS 'Audit trail for all faction point changes';
COMMENT ON COLUMN s51a_faction_points_log.log_id IS 'Primary key (auto-incremented)';
COMMENT ON COLUMN s51a_faction_points_log.account_id IS 'Account that earned/lost points';
COMMENT ON COLUMN s51a_faction_points_log.faction_id IS 'Faction these points belong to';
COMMENT ON COLUMN s51a_faction_points_log.points_delta IS 'Point change (+10, -5, etc.)';
COMMENT ON COLUMN s51a_faction_points_log.reason IS 'Why points changed (siege_win, daily_quest, etc.)';

-- Indexes for audit queries
CREATE INDEX IF NOT EXISTS idx_faction_log_account ON s51a_faction_points_log(account_id, timestamp DESC);
CREATE INDEX IF NOT EXISTS idx_faction_log_faction ON s51a_faction_points_log(faction_id, timestamp DESC);

-- =====================================================
-- TABLE 6: s51a_town_control (faction ownership of siege towns)
-- =====================================================
-- NOTE: This table is future-proofed for Phase 2+
-- Currently unused but schema is ready
CREATE TABLE IF NOT EXISTS s51a_town_control (
    town_name TEXT PRIMARY KEY,
    controlling_faction_id INTEGER REFERENCES s51a_factions(faction_id),
    controlled_since TIMESTAMPTZ,
    last_siege_at TIMESTAMPTZ,
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_town_control IS 'Faction ownership of siege towns (Jhelom, Yew, Skara Brae)';
COMMENT ON COLUMN s51a_town_control.town_name IS 'Town name (primary key)';
COMMENT ON COLUMN s51a_town_control.controlling_faction_id IS 'Faction currently controlling this town (NULL if neutral)';
COMMENT ON COLUMN s51a_town_control.controlled_since IS 'Timestamp when current faction gained control';
COMMENT ON COLUMN s51a_town_control.last_siege_at IS 'Timestamp of most recent siege battle';

-- Seed siege towns (locked per Phase 1 Q6 specification)
INSERT INTO s51a_town_control (town_name, controlling_faction_id, controlled_since, last_siege_at)
VALUES
    ('Jhelom', NULL, NULL, NULL),
    ('Yew', NULL, NULL, NULL),
    ('SkaraBrae', NULL, NULL, NULL)
ON CONFLICT (town_name) DO NOTHING;

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================
-- Run these queries to verify schema setup:
--
-- SELECT * FROM s51a_factions;
-- (Should return 3 rows: GoldenShield, Bridgefolk, LycaeumOrder)
--
-- SELECT * FROM s51a_config WHERE key = 'server_launch_date';
-- (Should return server launch timestamp)
--
-- SELECT COUNT(*) FROM s51a_guild_factions;
-- (Should return 0 - no guilds assigned yet)
--
-- SELECT * FROM s51a_town_control;
-- (Should return 3 rows: Jhelom, Yew, SkaraBrae - all NULL faction)

-- =====================================================
-- SCHEMA VERSION TRACKING
-- =====================================================
INSERT INTO s51a_config (key, value, created_at)
VALUES ('schema_version', '001', NOW())
ON CONFLICT (key) DO UPDATE SET value = '001', updated_at = NOW();

-- =====================================================
-- END OF MIGRATION: 001_Factions_Schema.sql
-- =====================================================
