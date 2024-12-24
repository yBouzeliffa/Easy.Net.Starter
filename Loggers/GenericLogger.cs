using Serilog;
using Serilog.Events;

namespace Easy.Net.Starter.Loggers
{
    public class GenericLogger : IGenericLogger
    {
        private readonly ILogger _logger;
        public GenericLogger(ILogger logger)
        {
            _logger = logger;
        }
        public bool IsEnabled(LogEventLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }
        public void LogError(string message, params object?[] args)
        {
            _logger.Error(message, args);
        }
        public void LogError(Exception ex, string message, params object?[] args)
        {
            _logger?.Error(ex, message, args);
        }
        public void LogInformation(string message, params object?[] args)
        {
            _logger.Information(message, args);
        }
    }
}
