namespace Easy.Net.Starter.App
{
    public partial class AppSettings
    {
        public string OverrideWriteLogToFile { get; set; }
        public LoggingSection Logging { get; set; }
        public SerilogSection Serilog { get; set; }
        public string AllowedOrigins { get; set; }
        public string AllowedHeaders { get; set; }
        public string AllowedMethods { get; set; }
        public string Version { get; set; }

        public ConnectionStringsSection ConnectionStrings { get; set; }
        public JwtSection Jwt { get; set; }
        public ApiKeySection ApiKey { get; set; }
        public PostgreSection Postgre { get; set; }
        public ServerSection Server { get; set; }
    }

    public class ServerSection
    {
        public string BackupFilePath { get; set; }
    }

    public class PostgreSection
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string BackupFilePath { get; set; }
    }

    public class LoggingSection
    {
        public LogLevelSection LogLevel { get; set; }
    }

    public class LogLevelSection
    {
        public string Default { get; set; }
        public string System { get; set; }
        public string Microsoft { get; set; }
    }

    public class SerilogSection
    {
        public List<string> Using { get; set; }
        public MinimumLevelSection MinimumLevel { get; set; }
        public List<WriteToSection> WriteTo { get; set; }
    }

    public class MinimumLevelSection
    {
        public string Default { get; set; }
        public Dictionary<string, string> Override { get; set; }
    }

    public class WriteToSection
    {
        public string Name { get; set; }
        public ArgsSection Args { get; set; }
    }

    public class ArgsSection
    {
        public string Path { get; set; }
        public string RollingInterval { get; set; }
        public long FileSizeLimitBytes { get; set; }
        public int RetainedFileCountLimit { get; set; }
    }

    public class ConnectionStringsSection
    {
        public string APPLICATION_POSTGRE_SQL { get; set; }
        public string INSTANCE_MANAGER_POSTGRE_SQL { get; set; }
    }

    public record JwtSection(string Issuer, string Audience, string Key, int MinutesDuration);

    public class ApiKeySection
    {
        public string ApiPrivateKey { get; set; }
        public string ApiPublicKey { get; set; }
        public string ApiDataKey { get; set; }
    }
}
