// =====================================================
// 51ALPHA CORE - DATABASE MIGRATION RUNNER
// =====================================================
// File: MigrationRunner.cs
// Created: 2025-12-16
// Phase: Sprint 1 - Database Foundation
// Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P4-Q1: CI/CD Pipeline)
// =====================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Npgsql;

namespace Server.Sphere51a.Core.Database
{
    /// <summary>
    /// Database migration runner for executing PostgreSQL schema migrations.
    /// Automatically applies SQL migration files in order from Distribution/Data/Postgres/Migrations/
    /// </summary>
    public static class MigrationRunner
    {
        private const string MigrationTableName = "s51a_schema_migrations";

        /// <summary>
        /// Run all pending database migrations.
        /// Migration files must be named: ###_Name.sql (e.g., 001_Factions_Schema.sql)
        /// </summary>
        /// <param name="migrationDirectory">Directory containing .sql migration files</param>
        /// <returns>Number of migrations applied</returns>
        public static int RunMigrations(string migrationDirectory)
        {
            if (!PostgresConnection.IsConfigured)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine("[Sphere51a] PostgreSQL not configured - skipping migrations");
                Utility.PopColor();
                return 0;
            }

            if (!Directory.Exists(migrationDirectory))
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[Sphere51a] Migration directory not found: {migrationDirectory}");
                Utility.PopColor();
                return 0;
            }

            try
            {
                EnsureMigrationTable();
                var appliedMigrations = GetAppliedMigrations();
                var pendingMigrations = GetPendingMigrations(migrationDirectory, appliedMigrations);

                if (pendingMigrations.Count == 0)
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine("[Sphere51a] Database schema is up to date - no migrations to apply");
                    Utility.PopColor();
                    return 0;
                }

                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[Sphere51a] Found {pendingMigrations.Count} pending migration(s)");
                Utility.PopColor();

                var appliedCount = 0;
                foreach (var migration in pendingMigrations.OrderBy(m => m.Version))
                {
                    ApplyMigration(migration);
                    appliedCount++;
                }

                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[Sphere51a] Successfully applied {appliedCount} migration(s)");
                Utility.PopColor();

                return appliedCount;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Migration failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Ensure the migration tracking table exists.
        /// </summary>
        private static void EnsureMigrationTable()
        {
            using var conn = PostgresConnection.GetConnection();
            using var cmd = new NpgsqlCommand($@"
                CREATE TABLE IF NOT EXISTS {MigrationTableName} (
                    migration_version TEXT PRIMARY KEY,
                    migration_name TEXT NOT NULL,
                    applied_at TIMESTAMPTZ DEFAULT NOW()
                );
            ", conn);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Get list of already applied migrations from database.
        /// </summary>
        private static HashSet<string> GetAppliedMigrations()
        {
            using var conn = PostgresConnection.GetConnection();
            using var cmd = new NpgsqlCommand($"SELECT migration_version FROM {MigrationTableName}", conn);
            using var reader = cmd.ExecuteReader();

            var applied = new HashSet<string>();
            while (reader.Read())
            {
                applied.Add(reader.GetString(0));
            }

            return applied;
        }

        /// <summary>
        /// Get list of pending migrations from migration directory.
        /// </summary>
        private static List<MigrationFile> GetPendingMigrations(string directory, HashSet<string> applied)
        {
            var pending = new List<MigrationFile>();

            var sqlFiles = Directory.GetFiles(directory, "*.sql")
                .Select(Path.GetFileName)
                .OrderBy(f => f);

            foreach (var file in sqlFiles)
            {
                var migration = ParseMigrationFile(file);
                if (migration == null)
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[Sphere51a] Skipping invalid migration file: {file}");
                    Utility.PopColor();
                    continue;
                }

                if (!applied.Contains(migration.Version))
                {
                    migration.FilePath = Path.Combine(directory, file);
                    pending.Add(migration);
                }
            }

            return pending;
        }

        /// <summary>
        /// Parse migration file name to extract version and name.
        /// Expected format: ###_Name.sql (e.g., 001_Factions_Schema.sql)
        /// </summary>
        private static MigrationFile ParseMigrationFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !fileName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                return null;

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var parts = nameWithoutExtension.Split('_', 2);

            if (parts.Length != 2)
                return null;

            var version = parts[0];
            var name = parts[1];

            if (string.IsNullOrWhiteSpace(version) || !version.All(char.IsDigit))
                return null;

            return new MigrationFile
            {
                Version = version,
                Name = name,
                FileName = fileName
            };
        }

        /// <summary>
        /// Apply a single migration file to the database.
        /// </summary>
        private static void ApplyMigration(MigrationFile migration)
        {
            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine($"[Sphere51a] Applying migration {migration.Version}: {migration.Name}");
            Utility.PopColor();

            var sql = File.ReadAllText(migration.FilePath);

            using var conn = PostgresConnection.GetConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                // Execute migration SQL
                using (var cmd = new NpgsqlCommand(sql, conn, transaction))
                {
                    cmd.CommandTimeout = 300; // 5 minutes for large migrations
                    cmd.ExecuteNonQuery();
                }

                // Record migration as applied
                using (var cmd = new NpgsqlCommand($@"
                    INSERT INTO {MigrationTableName} (migration_version, migration_name, applied_at)
                    VALUES (@version, @name, NOW())
                ", conn, transaction))
                {
                    cmd.Parameters.AddWithValue("version", migration.Version);
                    cmd.Parameters.AddWithValue("name", migration.Name);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();

                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[Sphere51a] Migration {migration.Version} applied successfully");
                Utility.PopColor();
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] Migration {migration.Version} FAILED: {ex.Message}");
                Utility.PopColor();

                throw new Exception($"Migration {migration.Version} failed", ex);
            }
        }

        /// <summary>
        /// Get current schema version from database.
        /// </summary>
        public static string GetCurrentSchemaVersion()
        {
            if (!PostgresConnection.IsConfigured)
                return "Not configured";

            try
            {
                using var conn = PostgresConnection.GetConnection();
                using var cmd = new NpgsqlCommand($@"
                    SELECT migration_version
                    FROM {MigrationTableName}
                    ORDER BY applied_at DESC
                    LIMIT 1
                ", conn);

                var result = cmd.ExecuteScalar();
                return result?.ToString() ?? "None";
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Represents a migration file to be applied.
        /// </summary>
        private class MigrationFile
        {
            public string Version { get; set; }
            public string Name { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }
    }
}
