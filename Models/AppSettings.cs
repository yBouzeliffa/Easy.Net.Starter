using Serilog;

namespace Easy.Net.Starter.Models
{
    public partial class AppSettings
    {
        public string OverrideWriteLogToFile { get; set; } = string.Empty;
        public string EmailApiKey { get; set; } = string.Empty;
        public LoggingSection Logging { get; set; } = new();
        public SerilogSection Serilog { get; set; } = new();
        public string AllowedOrigins { get; set; } = string.Empty;
        public string AllowedHeaders { get; set; } = string.Empty;
        public string AllowedMethods { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string DatabaseBackupPath { get; set; } = string.Empty;
        public string ApiBaseUrl { get; set; } = string.Empty;
        public string MobileAppRedirection { get; set; } = string.Empty;

        public ConnectionStringsSection ConnectionStrings { get; set; } = new();
        public JwtSection Jwt { get; set; } = new("", "", "", 0);
        public ApiKeySection ApiKey { get; set; } = new();
        public ServerSection Server { get; set; } = new();
    }

    public class ServerSection
    {
        public string BackupFilePath { get; set; } = string.Empty;
    }

    public class PostgreSection
    {
        public string Host { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string BackupFilePath { get; set; } = string.Empty;
    }

    public class LoggingSection
    {
        public LogLevelSection LogLevel { get; set; } = new();
    }

    public class LogLevelSection
    {
        public string Default { get; set; } = string.Empty;
        public string System { get; set; } = string.Empty;
        public string Microsoft { get; set; } = string.Empty;
    }

    public class SerilogSection
    {
        public List<string> Using { get; set; } = new();
        public MinimumLevelSection MinimumLevel { get; set; } = new();
        public List<WriteToSection> WriteTo { get; set; } = new();
    }

    public class MinimumLevelSection
    {
        public string Default { get; set; } = string.Empty;
        public Dictionary<string, string> Override { get; set; } = new();
    }

    public class WriteToSection
    {
        public string Name { get; set; } = string.Empty;
        public ArgsSection Args { get; set; } = new();
    }

    public class ArgsSection
    {
        public string Path { get; set; } = string.Empty;
        public string RollingInterval { get; set; } = string.Empty;
        public long FileSizeLimitBytes { get; set; } = new();
        public int RetainedFileCountLimit { get; set; } = new();

        public RollingInterval RollingInternalEnum
        {
            get
            {
                if (Enum.TryParse(RollingInterval, out RollingInterval parsed))
                {
                    return parsed;
                }

                throw new InvalidOperationException("Unable to parse rolling interval");
            }
        }
    }

    public class ConnectionStringsSection
    {
        public string APPLICATION_POSTGRE_SQL { get; set; } = string.Empty;
        public string INSTANCE_MANAGER_POSTGRE_SQL { get; set; } = string.Empty;
    }

    public record JwtSection(string Issuer, string Audience, string Key, int MinutesDuration);

    public class ApiKeySection
    {
        public string ApiPrivateKey { get; set; } = string.Empty;
        public string ApiPublicKey { get; set; } = string.Empty;
        public string ApiDataKey { get; set; } = string.Empty;
    }
}
