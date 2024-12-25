using Easy.Net.Starter.App;
using Easy.Net.Starter.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;

namespace Easy.Net.Starter.ProgramsEntries
{
    public static class ConsoleAppStarter
    {
        public static ServiceProvider Start<TAppSettings>(string[] args, StartupOptions options)
            where TAppSettings : AppSettings, new()
        {
            var builder = Host.CreateApplicationBuilder(args);

            var configuration = ApplicationRegistrator.RegisterConfiguration();

            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var appSettings = builder.Services.RegisterAppSettings<TAppSettings>(configuration);

            ArgumentException.ThrowIfNullOrEmpty(appSettings.OverrideWriteLogToFile, nameof(appSettings));

            builder.Services.AddSerilog(config =>
            {
                config
                    .ReadFrom.Configuration(configuration)
                    .WriteTo.File(appSettings.OverrideWriteLogToFile.Replace("{APP_NAME}", Process.GetCurrentProcess().ProcessName));
            });

            builder.Services.AddScoped<IGenericLogger, GenericLogger>();

            var serviceProvider = ApplicationRegistrator.RegisterServices(
                builder.Services,
                options.SingletonServices,
                options.ScopedServices,
                options.TransientServices,
                options.SingletonsWithInterfaces,
                options.ScopedWithInterfaces,
                options.TransientsWithInterfaces);

            var logger = serviceProvider.GetRequiredService<IGenericLogger>();

            logger.LogInformation("");
            logger.LogInformation("╔═══════════════════════════════════════════════════╗");
            logger.LogInformation("║          !! Application initialization !!         ║");
            logger.LogInformation("╚═══════════════════════════════════════════════════╝");
            logger.LogInformation("");

            logger.LogInformation("");
            logger.LogInformation("Version : {Version}", appSettings.Version);
            logger.LogInformation("");

            return serviceProvider;
        }
    }
}
