using Easy.Net.Starter.Models;
using Easy.Net.Starter.Security;
using Easy.Net.Starter.Startup.Registrators;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Easy.Net.Starter.PostgreSQL
{
    public interface IDatabaseBackupService
    {
        Task CreateBackup();
    }
    public class DatabaseBackupService : IDatabaseBackupService
    {
        private readonly AppSettings appSettings;
        private readonly ILogger<DatabaseBackupService> logger;

        public DatabaseBackupService(AppSettings appSettings, ILogger<DatabaseBackupService> logger)
        {
            this.appSettings = appSettings;
            this.logger = logger;
        }

        public async Task CreateBackup()
        {
            await Task.Run(() =>
            {
                try
                {
                    logger.LogInformation("");
                    string timestamp = DateTime.UtcNow.ToString("yyyyMMdd");

                    var connectionStringParts = ApplicationRegistrator.ParseConnectionString(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL);

                    connectionStringParts.TryGetValue("Database", out var databaseName);
                    connectionStringParts.TryGetValue("Host", out var host);
                    connectionStringParts.TryGetValue("Username", out var username);

                    if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username))
                    {
                        logger.LogError("Database connection string is not valid.");
                        return;
                    }

                    var password = ConnectionStringsSecurityManager.GetDatabasePassword(databaseName);

                    var backupName = appSettings.DatabaseBackupPath.Replace("{DATABASE_NAME}", databaseName).Replace("{timestamp}", timestamp);

                    BackupPathHelper.EnsureDirectoryExistsForFile(backupName);

                    using var process = new Process();
                    process.StartInfo.FileName = PgDumpFinder.FindPgDumpPath();
                    process.StartInfo.Arguments = $"-h {host} -U {username} -Fc -f \"{backupName}\" {databaseName}";
                    process.StartInfo.EnvironmentVariables["PGPASSWORD"] = password;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    process.Start();
                    process.WaitForExit();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(error))
                    {
                        logger.LogError($"Error during backup: {error}");
                        return;
                    }

                    logger.LogInformation($"Database Backup created successfully!");
                }
                catch (Exception ex)
                {
                    logger.LogInformation("");
                    logger.LogInformation("");
                    logger.LogError(ex, ex.Message);
                    logger.LogInformation("");
                    logger.LogInformation("");
                }
            });
        }
    }
}
