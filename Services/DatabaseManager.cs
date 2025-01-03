using Npgsql;
using Serilog;

namespace Easy.Net.Starter.Services
{
    public class DatabaseManager
    {
        private readonly string _connectionString;
        private readonly string _databaseToCheck;

        public DatabaseManager(string instanceManagerConnectionString, string targetDatabaseName)
        {
            _connectionString = instanceManagerConnectionString;
            _databaseToCheck = targetDatabaseName;
        }

        public async Task EnsureDatabaseExistsAsync()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var checkDbQuery = $"SELECT 1 FROM pg_database WHERE datname = '{_databaseToCheck}';";
                using var checkCommand = new NpgsqlCommand(checkDbQuery, connection);
                var exists = await checkCommand.ExecuteScalarAsync() != null;

                if (exists)
                {
                    Log.Logger.Information("Database '{_databaseToCheck}' already exists.", _databaseToCheck);
                    return;
                }

                var createDbQuery = $"CREATE DATABASE \"{_databaseToCheck}\";";
                using var createCommand = new NpgsqlCommand(createDbQuery, connection);
                await createCommand.ExecuteNonQueryAsync();

                Log.Logger.Information("Database '{_databaseToCheck}' created successfully.", _databaseToCheck);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Error while ensuring database exists: {Message}", ex.Message);
                throw;
            }
        }
    }
}
