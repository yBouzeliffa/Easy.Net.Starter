using Easy.Net.Starter.Models;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Easy.Net.Starter.Startup.Helpers
{
    public static class SerilogHelper
    {
        public static ArgsSection ExtractConfiguration(IConfiguration configuration)
        {
            var rollingIntervalString = configuration["Serilog:WriteTo:1:Args:rollingInterval"];
            var filesizeConfig = int.TryParse(configuration["Serilog:WriteTo:1:Args:fileSizeLimitBytes"], out var fileSizeLimit);
            var retainedFileCountLimitConfig = int.TryParse(configuration["Serilog:WriteTo:1:Args:retainedFileCountLimit"], out var retainedFileCount);
            var path = configuration["OverrideWriteLogToFile"];

            if (string.IsNullOrEmpty(rollingIntervalString) || string.IsNullOrEmpty(path) || !filesizeConfig || !retainedFileCountLimitConfig)
            {
                throw new InvalidOperationException("Unable to register Serilog");
            }

            return new ArgsSection
            {
                FileSizeLimitBytes = fileSizeLimit,
                RetainedFileCountLimit = retainedFileCount,
                RollingInterval = rollingIntervalString,
                Path = path.Replace("{APP_NAME}", Process.GetCurrentProcess().ProcessName)
            };
        }
    }
}
