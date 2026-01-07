-- =====================================================
-- 51ALPHA COMPLETE SCHEMA - POSTGRESQL MIGRATION
-- =====================================================
-- Migration: 002_Complete_Schema.sql
-- Created: 2025-12-16
-- Phase: Sprint 1 - Complete Database Foundation
-- Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q5, P3-Q3)
-- Dependencies: 001_Factions_Schema.sql (must run first)
-- =====================================================
--
-- This migration adds the remaining 25 tables to complete
-- the 31-table schema defined in the questionnaire.
--
-- Table Organization (31 total):
-- - Core Config (3): s51a_config ✅, s51a_events ⏳, account_mapping ⏳
-- - Faction System (6): Already complete ✅ (001_Factions_Schema.sql)
-- - Siege System (5): Battle/territory control ⏳
-- - Daily Content (6): Quest/dungeon rotation ⏳
-- - Tournament (4): Bracket management ⏳
-- - Glicko (2): Rating calculations ⏳
-- - NPE (2): Tutorial progression ⏳
-- - Currency (1): Virtual currency ⏳
-- =====================================================

-- =====================================================
-- CORE CONFIG TABLES (2 additional)
-- =====================================================

-- TABLE: s51a_events (server-wide events)
CREATE TABLE IF NOT EXISTS s51a_events (
    event_id SERIAL PRIMARY KEY,
    event_type TEXT NOT NULL,
    event_name TEXT NOT NULL,
    event_data JSONB,
    start_time TIMESTAMPTZ NOT NULL,
    end_time TIMESTAMPTZ,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),

    CONSTRAINT check_event_times CHECK (end_time IS NULL OR end_time > start_time)
);

COMMENT ON TABLE s51a_events IS 'Server-wide events (sieges, tournaments, daily content rotation)';
COMMENT ON COLUMN s51a_events.event_type IS 'Event category (siege, tournament, daily_dungeon, etc.)';
COMMENT ON COLUMN s51a_events.event_name IS 'Human-readable event name';
COMMENT ON COLUMN s51a_events.event_data IS 'JSON payload with event-specific data';
COMMENT ON COLUMN s51a_events.is_active IS 'Whether event is currently active';

CREATE INDEX IF NOT EXISTS idx_events_active ON s51a_events(is_active, start_time DESC);
CREATE INDEX IF NOT EXISTS idx_events_type ON s51a_events(event_type, start_time DESC);

-- TABLE: account_mapping (ModernUO account → character tracking)
CREATE TABLE IF NOT EXISTS account_mapping (
    account_id TEXT NOT NULL,
    character_serial BIGINT NOT NULL,
    character_name TEXT NOT NULL,
    last_login TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW(),

    PRIMARY KEY (account_id, character_serial)
);

COMMENT ON TABLE account_mapping IS 'Maps ModernUO accounts to their characters (for account-bound features)';
COMMENT ON COLUMN account_mapping.account_id IS 'ModernUO Account.Username';
COMMENT ON COLUMN account_mapping.character_serial IS 'ModernUO Mobile.Serial';
COMMENT ON COLUMN account_mapping.character_name IS 'Character name (cached for queries)';

CREATE INDEX IF NOT EXISTS idx_account_characters ON account_mapping(account_id, last_login DESC);
CREATE INDEX IF NOT EXISTS idx_character_account ON account_mapping(character_serial);

-- =====================================================
-- SIEGE SYSTEM TABLES (5 tables)
-- =====================================================

-- TABLE: s51a_sieges (siege battle instances)
CREATE TABLE IF NOT EXISTS s51a_sieges (
    siege_id BIGSERIAL PRIMARY KEY,
    town_name TEXT NOT NULL REFERENCES s51a_town_control(town_name),
    attacking_faction_id INTEGER NOT NULL REFERENCES s51a_factions(faction_id),
    defending_faction_id INTEGER REFERENCES s51a_factions(faction_id),
    start_time TIMESTAMPTZ NOT NULL,
    end_time TIMESTAMPTZ,
    winner_faction_id INTEGER REFERENCES s51a_factions(faction_id),
    attacking_score INTEGER DEFAULT 0,
    defending_score INTEGER DEFAULT 0,
    status TEXT DEFAULT 'scheduled' CHECK (status IN ('scheduled', 'active', 'completed', 'cancelled')),
    created_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_sieges IS 'Siege battle instances for town control';
COMMENT ON COLUMN s51a_sieges.town_name IS 'Which siege town is being contested';
COMMENT ON COLUMN s51a_sieges.attacking_faction_id IS 'Faction initiating the siege';
COMMENT ON COLUMN s51a_sieges.defending_faction_id IS 'Faction defending (NULL if neutral)';
COMMENT ON COLUMN s51a_sieges.status IS 'Siege state (scheduled/active/completed/cancelled)';

CREATE INDEX IF NOT EXISTS idx_sieges_town ON s51a_sieges(town_name, start_time DESC);
CREATE INDEX IF NOT EXISTS idx_sieges_active ON s51a_sieges(status, start_time) WHERE status = 'active';

-- TABLE: s51a_siege_participants (player participation tracking)
CREATE TABLE IF NOT EXISTS s51a_siege_participants (
    siege_id BIGINT NOT NULL REFERENCES s51a_sieges(siege_id) ON DELETE CASCADE,
    account_id TEXT NOT NULL,
    faction_id INTEGER NOT NULL REFERENCES s51a_factions(faction_id),
    kills INTEGER DEFAULT 0,
    deaths INTEGER DEFAULT 0,
    damage_dealt BIGINT DEFAULT 0,
    healing_done BIGINT DEFAULT 0,
    points_earned INTEGER DEFAULT 0,
    joined_at TIMESTAMPTZ DEFAULT NOW(),

    PRIMARY KEY (siege_id, account_id)
);

COMMENT ON TABLE s51a_siege_participants IS 'Player participation and contributions in siege battles';
COMMENT ON COLUMN s51a_siege_participants.points_earned IS 'Faction points earned from this siege';

CREATE INDEX IF NOT EXISTS idx_siege_participants_account ON s51a_siege_participants(account_id, siege_id DESC);
CREATE INDEX IF NOT EXISTS idx_siege_participants_faction ON s51a_siege_participants(faction_id, siege_id DESC);

-- TABLE: s51a_siege_kills (PvP kill log during sieges)
CREATE TABLE IF NOT EXISTS s51a_siege_kills (
    kill_id BIGSERIAL PRIMARY KEY,
    siege_id BIGINT NOT NULL REFERENCES s51a_sieges(siege_id) ON DELETE CASCADE,
    killer_account_id TEXT NOT NULL,
    victim_account_id TEXT NOT NULL,
    killer_faction_id INTEGER NOT NULL REFERENCES s51a_factions(faction_id),
    victim_faction_id INTEGER NOT NULL REFERENCES s51a_factions(faction_id),
    timestamp TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_siege_kills IS 'PvP kill log during siege battles (for scoring and statistics)';

CREATE INDEX IF NOT EXISTS idx_siege_kills_siege ON s51a_siege_kills(siege_id, timestamp DESC);
CREATE INDEX IF NOT EXISTS idx_siege_kills_killer ON s51a_siege_kills(killer_account_id, timestamp DESC);

-- TABLE: s51a_siege_schedule (automated siege scheduling)
CREATE TABLE IF NOT EXISTS s51a_siege_schedule (
    schedule_id SERIAL PRIMARY KEY,
    town_name TEXT NOT NULL REFERENCES s51a_town_control(town_name),
    day_of_week INTEGER NOT NULL CHECK (day_of_week BETWEEN 0 AND 6),
    start_hour INTEGER NOT NULL CHECK (start_hour BETWEEN 0 AND 23),
    duration_minutes INTEGER NOT NULL DEFAULT 120,
    is_enabled BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT NOW(),

    UNIQUE (town_name, day_of_week, start_hour)
);

COMMENT ON TABLE s51a_siege_schedule IS 'Automated siege scheduling configuration';
COMMENT ON COLUMN s51a_siege_schedule.day_of_week IS 'Day of week (0=Sunday, 6=Saturday)';
COMMENT ON COLUMN s51a_siege_schedule.start_hour IS 'Hour of day (UTC) to start siege';
COMMENT ON COLUMN s51a_siege_schedule.duration_minutes IS 'Siege duration in minutes';

-- TABLE: s51a_siege_history (historical results)
CREATE TABLE IF NOT EXISTS s51a_siege_history (
    history_id BIGSERIAL PRIMARY KEY,
    town_name TEXT NOT NULL REFERENCES s51a_town_control(town_name),
    siege_date DATE NOT NULL,
    winner_faction_id INTEGER REFERENCES s51a_factions(faction_id),
    total_participants INTEGER DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_siege_history IS 'Historical record of siege outcomes (aggregated by date)';

CREATE INDEX IF NOT EXISTS idx_siege_history_town ON s51a_siege_history(town_name, siege_date DESC);

-- =====================================================
-- DAILY CONTENT SYSTEM TABLES (6 tables)
-- =====================================================

-- TABLE: s51a_dungeons (dungeon definitions)
CREATE TABLE IF NOT EXISTS s51a_dungeons (
    dungeon_id SERIAL PRIMARY KEY,
    dungeon_name TEXT NOT NULL UNIQUE,
    dungeon_location TEXT NOT NULL,
    min_level INTEGER DEFAULT 1,
    max_level INTEGER DEFAULT 100,
    loot_tier INTEGER DEFAULT 1 CHECK (loot_tier BETWEEN 1 AND 5),
    created_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_dungeons IS 'Dungeon definitions for daily rotation';
COMMENT ON COLUMN s51a_dungeons.loot_tier IS 'Loot quality (1=common, 5=legendary)';

-- TABLE: s51a_daily_dungeons (active dungeon rotation)
CREATE TABLE IF NOT EXISTS s51a_daily_dungeons (
    rotation_id SERIAL PRIMARY KEY,
    dungeon_id INTEGER NOT NULL REFERENCES s51a_dungeons(dungeon_id),
    rotation_date DATE NOT NULL,
    bonus_loot_multiplier NUMERIC(3,2) DEFAULT 1.0,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT NOW(),

    UNIQUE (rotation_date, dungeon_id)
);

COMMENT ON TABLE s51a_daily_dungeons IS 'Daily dungeon rotation schedule';
COMMENT ON COLUMN s51a_daily_dungeons.bonus_loot_multiplier IS 'Loot quality multiplier for this rotation (e.g., 1.5 = 50% better)';

CREATE INDEX IF NOT EXISTS idx_daily_dungeons_date ON s51a_daily_dungeons(rotation_date DESC) WHERE is_active = true;

-- TABLE: s51a_quests (quest definitions)
CREATE TABLE IF NOT EXISTS s51a_quests (
    quest_id SERIAL PRIMARY KEY,
    quest_name TEXT NOT NULL UNIQUE,
    quest_description TEXT,
    quest_type TEXT NOT NULL CHECK (quest_type IN ('kill', 'gather', 'explore', 'dungeon', 'pvp')),
    required_count INTEGER DEFAULT 1,
    reward_points INTEGER DEFAULT 0,
    reward_gold INTEGER DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_quests IS 'Quest definitions for daily/weekly rotation';
COMMENT ON COLUMN s51a_quests.quest_type IS 'Type of quest (kill/gather/explore/dungeon/pvp)';

-- TABLE: s51a_daily_quests (active quest rotation)
CREATE TABLE IF NOT EXISTS s51a_daily_quests (
    daily_quest_id SERIAL PRIMARY KEY,
    quest_id INTEGER NOT NULL REFERENCES s51a_quests(quest_id),
    rotation_date DATE NOT NULL,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT NOW(),

    UNIQUE (rotation_date, quest_id)
);

COMMENT ON TABLE s51a_daily_quests IS 'Daily quest rotation schedule';

CREATE INDEX IF NOT EXISTS idx_daily_quests_date ON s51a_daily_quests(rotation_date DESC) WHERE is_active = true;

-- TABLE: s51a_quest_progress (player quest completion tracking)
CREATE TABLE IF NOT EXISTS s51a_quest_progress (
    progress_id BIGSERIAL PRIMARY KEY,
    account_id TEXT NOT NULL,
    quest_id INTEGER NOT NULL REFERENCES s51a_quests(quest_id),
    current_count INTEGER DEFAULT 0,
    completed_at TIMESTAMPTZ,
    rotation_date DATE NOT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW(),

    UNIQUE (account_id, quest_id, rotation_date)
);

COMMENT ON TABLE s51a_quest_progress IS 'Player progress on daily quests';

CREATE INDEX IF NOT EXISTS idx_quest_progress_account ON s51a_quest_progress(account_id, rotation_date DESC);

-- TABLE: s51a_dungeon_completions (player dungeon completion tracking)
CREATE TABLE IF NOT EXISTS s51a_dungeon_completions (
    completion_id BIGSERIAL PRIMARY KEY,
    account_id TEXT NOT NULL,
    dungeon_id INTEGER NOT NULL REFERENCES s51a_dungeons(dungeon_id),
    completed_at TIMESTAMPTZ DEFAULT NOW(),
    rotation_date DATE NOT NULL,
    loot_earned TEXT,

    UNIQUE (account_id, dungeon_id, rotation_date)
);

COMMENT ON TABLE s51a_dungeon_completions IS 'Player dungeon completion tracking (one per day per dungeon)';

CREATE INDEX IF NOT EXISTS idx_dungeon_completions_account ON s51a_dungeon_completions(account_id, completed_at DESC);

-- =====================================================
-- TOURNAMENT SYSTEM TABLES (4 tables)
-- =====================================================

-- TABLE: s51a_tournaments (tournament instances)
CREATE TABLE IF NOT EXISTS s51a_tournaments (
    tournament_id SERIAL PRIMARY KEY,
    tournament_name TEXT NOT NULL,
    tournament_type TEXT NOT NULL CHECK (tournament_type IN ('single_elimination', 'double_elimination', 'round_robin')),
    start_time TIMESTAMPTZ NOT NULL,
    end_time TIMESTAMPTZ,
    max_participants INTEGER DEFAULT 64,
    current_participants INTEGER DEFAULT 0,
    status TEXT DEFAULT 'registration' CHECK (status IN ('registration', 'in_progress', 'completed', 'cancelled')),
    prize_pool_gold INTEGER DEFAULT 0,
    winner_account_id TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_tournaments IS 'Tournament instances (single/double elimination, round robin)';
COMMENT ON COLUMN s51a_tournaments.status IS 'Tournament state (registration/in_progress/completed/cancelled)';

CREATE INDEX IF NOT EXISTS idx_tournaments_status ON s51a_tournaments(status, start_time DESC);

-- TABLE: s51a_tournament_participants (registered players)
CREATE TABLE IF NOT EXISTS s51a_tournament_participants (
    tournament_id INTEGER NOT NULL REFERENCES s51a_tournaments(tournament_id) ON DELETE CASCADE,
    account_id TEXT NOT NULL,
    seed_position INTEGER,
    current_round INTEGER DEFAULT 0,
    is_eliminated BOOLEAN DEFAULT false,
    registered_at TIMESTAMPTZ DEFAULT NOW(),

    PRIMARY KEY (tournament_id, account_id)
);

COMMENT ON TABLE s51a_tournament_participants IS 'Tournament participant registration and status';

CREATE INDEX IF NOT EXISTS idx_tournament_participants_account ON s51a_tournament_participants(account_id, tournament_id DESC);

-- TABLE: s51a_tournament_matches (match results)
CREATE TABLE IF NOT EXISTS s51a_tournament_matches (
    match_id BIGSERIAL PRIMARY KEY,
    tournament_id INTEGER NOT NULL REFERENCES s51a_tournaments(tournament_id) ON DELETE CASCADE,
    round_number INTEGER NOT NULL,
    match_number INTEGER NOT NULL,
    player1_account_id TEXT NOT NULL,
    player2_account_id TEXT NOT NULL,
    winner_account_id TEXT,
    match_start_time TIMESTAMPTZ,
    match_end_time TIMESTAMPTZ,
    status TEXT DEFAULT 'pending' CHECK (status IN ('pending', 'in_progress', 'completed', 'forfeit')),
    created_at TIMESTAMPTZ DEFAULT NOW(),

    UNIQUE (tournament_id, round_number, match_number)
);

COMMENT ON TABLE s51a_tournament_matches IS 'Tournament match brackets and results';

CREATE INDEX IF NOT EXISTS idx_tournament_matches_tournament ON s51a_tournament_matches(tournament_id, round_number, match_number);
CREATE INDEX IF NOT EXISTS idx_tournament_matches_player ON s51a_tournament_matches(player1_account_id, created_at DESC);

-- TABLE: s51a_tournament_history (historical tournament results)
CREATE TABLE IF NOT EXISTS s51a_tournament_history (
    history_id BIGSERIAL PRIMARY KEY,
    tournament_id INTEGER NOT NULL REFERENCES s51a_tournaments(tournament_id),
    winner_account_id TEXT NOT NULL,
    runner_up_account_id TEXT,
    total_participants INTEGER DEFAULT 0,
    completed_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_tournament_history IS 'Historical tournament results (leaderboard)';

CREATE INDEX IF NOT EXISTS idx_tournament_history_winner ON s51a_tournament_history(winner_account_id, completed_at DESC);

-- =====================================================
-- GLICKO RATING SYSTEM TABLES (2 tables)
-- =====================================================

-- TABLE: s51a_glicko_ratings (player ratings)
CREATE TABLE IF NOT EXISTS s51a_glicko_ratings (
    account_id TEXT PRIMARY KEY,
    rating NUMERIC(8,2) DEFAULT 1500.0,
    rating_deviation NUMERIC(8,2) DEFAULT 350.0,
    volatility NUMERIC(8,4) DEFAULT 0.06,
    last_match_at TIMESTAMPTZ,
    total_matches INTEGER DEFAULT 0,
    wins INTEGER DEFAULT 0,
    losses INTEGER DEFAULT 0,
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_glicko_ratings IS 'Player Glicko-2 ratings for PvP skill tracking';
COMMENT ON COLUMN s51a_glicko_ratings.rating IS 'Glicko-2 rating (μ)';
COMMENT ON COLUMN s51a_glicko_ratings.rating_deviation IS 'Glicko-2 rating deviation (φ)';
COMMENT ON COLUMN s51a_glicko_ratings.volatility IS 'Glicko-2 volatility (σ)';

CREATE INDEX IF NOT EXISTS idx_glicko_leaderboard ON s51a_glicko_ratings(rating DESC);

-- TABLE: s51a_glicko_history (rating change history)
CREATE TABLE IF NOT EXISTS s51a_glicko_history (
    history_id BIGSERIAL PRIMARY KEY,
    account_id TEXT NOT NULL,
    match_id BIGINT,
    old_rating NUMERIC(8,2) NOT NULL,
    new_rating NUMERIC(8,2) NOT NULL,
    old_rd NUMERIC(8,2) NOT NULL,
    new_rd NUMERIC(8,2) NOT NULL,
    opponent_account_id TEXT,
    match_result TEXT CHECK (match_result IN ('win', 'loss', 'draw')),
    timestamp TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_glicko_history IS 'Historical record of Glicko-2 rating changes';

CREATE INDEX IF NOT EXISTS idx_glicko_history_account ON s51a_glicko_history(account_id, timestamp DESC);

-- =====================================================
-- NEW PLAYER EXPERIENCE (NPE) TABLES (2 tables)
-- =====================================================

-- TABLE: s51a_npe_checkpoints (tutorial checkpoint definitions)
CREATE TABLE IF NOT EXISTS s51a_npe_checkpoints (
    checkpoint_id SERIAL PRIMARY KEY,
    checkpoint_name TEXT NOT NULL UNIQUE,
    checkpoint_order INTEGER NOT NULL UNIQUE,
    reward_gold INTEGER DEFAULT 0,
    reward_points INTEGER DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_npe_checkpoints IS 'Tutorial checkpoint definitions (ordered progression)';

-- Seed tutorial checkpoints
INSERT INTO s51a_npe_checkpoints (checkpoint_name, checkpoint_order, reward_gold, reward_points)
VALUES
    ('Character Creation', 1, 100, 0),
    ('First Movement', 2, 50, 0),
    ('First Combat', 3, 100, 5),
    ('First Spell Cast', 4, 100, 5),
    ('First Crafting', 5, 100, 5),
    ('Join Guild', 6, 200, 10),
    ('Choose Faction', 7, 500, 20),
    ('Complete First Quest', 8, 500, 20),
    ('First PvP Kill', 9, 1000, 50),
    ('Tutorial Complete', 10, 2000, 100)
ON CONFLICT (checkpoint_name) DO NOTHING;

-- TABLE: s51a_npe_progress (player tutorial progress)
CREATE TABLE IF NOT EXISTS s51a_npe_progress (
    account_id TEXT NOT NULL,
    checkpoint_id INTEGER NOT NULL REFERENCES s51a_npe_checkpoints(checkpoint_id),
    completed_at TIMESTAMPTZ DEFAULT NOW(),

    PRIMARY KEY (account_id, checkpoint_id)
);

COMMENT ON TABLE s51a_npe_progress IS 'Player tutorial checkpoint completion tracking';

CREATE INDEX IF NOT EXISTS idx_npe_progress_account ON s51a_npe_progress(account_id, checkpoint_id);

-- =====================================================
-- CURRENCY SYSTEM TABLE (1 table)
-- =====================================================

-- TABLE: s51a_currency_log (virtual currency transaction log)
CREATE TABLE IF NOT EXISTS s51a_currency_log (
    transaction_id BIGSERIAL PRIMARY KEY,
    account_id TEXT NOT NULL,
    currency_type TEXT NOT NULL CHECK (currency_type IN ('gold', 'faction_points', 'tournament_tokens')),
    amount BIGINT NOT NULL,
    balance_after BIGINT NOT NULL,
    reason TEXT NOT NULL,
    metadata JSONB,
    timestamp TIMESTAMPTZ DEFAULT NOW()
);

COMMENT ON TABLE s51a_currency_log IS 'Virtual currency transaction audit log';
COMMENT ON COLUMN s51a_currency_log.amount IS 'Transaction amount (positive = earned, negative = spent)';
COMMENT ON COLUMN s51a_currency_log.balance_after IS 'Account balance after this transaction';
COMMENT ON COLUMN s51a_currency_log.metadata IS 'JSON payload with transaction-specific data';

CREATE INDEX IF NOT EXISTS idx_currency_log_account ON s51a_currency_log(account_id, timestamp DESC);
CREATE INDEX IF NOT EXISTS idx_currency_log_type ON s51a_currency_log(currency_type, timestamp DESC);

-- Partition by month for performance (optional, but recommended)
-- ALTER TABLE s51a_currency_log PARTITION BY RANGE (timestamp);

-- =====================================================
-- SCHEMA VERSION TRACKING
-- =====================================================
INSERT INTO s51a_config (key, value, created_at)
VALUES ('schema_version', '002', NOW())
ON CONFLICT (key) DO UPDATE SET value = '002', updated_at = NOW();

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================
-- Run these queries to verify schema setup:
--
-- SELECT key, value FROM s51a_config WHERE key = 'schema_version';
-- (Should return '002')
--
-- SELECT table_name FROM information_schema.tables
-- WHERE table_schema = 'public' AND table_name LIKE 's51a_%'
-- ORDER BY table_name;
-- (Should return 31 tables total)
--
-- SELECT COUNT(*) FROM s51a_npe_checkpoints;
-- (Should return 10 tutorial checkpoints)

-- =====================================================
-- END OF MIGRATION: 002_Complete_Schema.sql
-- =====================================================
