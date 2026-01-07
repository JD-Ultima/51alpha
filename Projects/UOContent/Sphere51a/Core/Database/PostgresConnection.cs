// =====================================================
// 51ALPHA CORE - POSTGRESQL CONNECTION MANAGER
// =====================================================
// File: PostgresConnection.cs
// Created: 2025-12-14
// Phase: Phase 1 - Three-Faction System Foundation
// Documentation: PHASES_2-6_QUESTIONNAIRE_FILLED.md (P2-Q5: Database Schema)
// =====================================================

using System;
using Npgsql;

namespace Server.Sphere51a.Core.Database
{
    /// <summary>
    /// PostgreSQL connection manager with connection pooling.
    /// Provides centralized database access for all Sphere51a systems.
    /// </summary>
    public static class PostgresConnection
    {
        private static string _connectionString;
        private static bool _isConfigured = false;

        /// <summary>
        /// Configure the PostgreSQL connection string.
        /// Must be called during server initialization before any database access.
        /// </summary>
        /// <param name="connectionString">PostgreSQL connection string (e.g., "Host=localhost;Database=sphere51a;Username=s51a;Password=changeme")</param>
        public static void Configure(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

            _connectionString = connectionString;
            _isConfigured = true;

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine("[Sphere51a] PostgreSQL connection configured");
            Utility.PopColor();
        }

        /// <summary>
        /// Get a new PostgreSQL connection (already opened).
        /// Caller is responsible for disposing the connection (use 'using' statement).
        /// </summary>
        /// <returns>Opened NpgsqlConnection</returns>
        /// <exception cref="InvalidOperationException">Thrown if connection not configured</exception>
        public static NpgsqlConnection GetConnection()
        {
            if (!_isConfigured || string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException(
                    "PostgreSQL connection not configured. Call PostgresConnection.Configure() during server initialization.");
            }

            try
            {
                var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] PostgreSQL connection failed: {ex.Message}");
                Utility.PopColor();
                throw;
            }
        }

        /// <summary>
        /// Test database connectivity.
        /// </summary>
        /// <returns>True if connection successful, false otherwise</returns>
        public static bool TestConnection()
        {
            if (!_isConfigured)
            {
                Console.WriteLine("[Sphere51a] PostgreSQL connection not configured - skipping test");
                return false;
            }

            try
            {
                using var conn = GetConnection();
                using var cmd = new NpgsqlCommand("SELECT 1", conn);
                var result = cmd.ExecuteScalar();

                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine("[Sphere51a] PostgreSQL connection test: SUCCESS");
                Utility.PopColor();

                return true;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[Sphere51a] PostgreSQL connection test: FAILED - {ex.Message}");
                Utility.PopColor();

                return false;
            }
        }

        /// <summary>
        /// Check if PostgreSQL connection is configured.
        /// </summary>
        public static bool IsConfigured => _isConfigured;
    }
}
