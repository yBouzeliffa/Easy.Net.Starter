using Serilog.Events;

namespace Easy.Net.Starter.Loggers
{
    public interface IGenericLogger
    {
        void LogInformation(string message, params object?[] args);
        void LogError(string message, params object?[] args);
        void LogError(Exception ex, string message, params object?[] args);
        bool IsEnabled(LogEventLevel logLevel);
    }
}
