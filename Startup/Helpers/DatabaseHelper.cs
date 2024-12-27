using Npgsql;
using Serilog;

namespace Easy.Net.Starter.Startup.Helpers
{
    internal static class DatabaseHelper
    {
        public static bool TryDatabaseConnection(this string connectionString)
        {
            try
            {
                using var connection = new NpgsqlConnection(connectionString);

                connection.Open();

                Log.Logger.Information("Database connection successful.");

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error while connecting to the database with connection: {ConnectionString}", connectionString);
                return false;
            }
        }

    }
}
