using Easy.Net.Starter.App;
using Easy.Net.Starter.Loggers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;

namespace Easy.Net.Starter.ProgramsEntries
{
    public static class ConsoleAppStarter
    {
        public static ServiceProvider Start(string[] args, StartupOptions options)
        {
            var builder = Host.CreateApplicationBuilder(args);

            var configuration = ApplicationRegistrator.RegisterConfiguration();

            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var configurationSettings = new AppSettings();
            configuration.Bind(configurationSettings);
            builder.Services.AddSingleton(configurationSettings);
            builder.Services.AddSerilog(config =>
            {
                config
                    .ReadFrom.Configuration(configuration)
                    .WriteTo.File(configurationSettings.OverrideWriteLogToFile.Replace("{APP_NAME}", Process.GetCurrentProcess().ProcessName));
            });

            Log.Logger.Information("Configuration registered");

            builder.Services.AddScoped<IGenericLogger, GenericLogger>();

            var serviceProvider = ApplicationRegistrator.RegisterServices(
                builder.Services,
                options.SingletonServices,
                options.ScopedServices,
                options.TransientServices,
                options.SingletonsWithInterfaces,
                options.ScopedWithInterfaces,
                options.TransientsWithInterfaces);

            var appSettings = serviceProvider.GetRequiredService<AppSettings>();
            var logger = serviceProvider.GetRequiredService<IGenericLogger>();

            logger.LogInformation("");
            logger.LogInformation("╔═══════════════════════════════════════════════════╗");
            logger.LogInformation("║          !! Application initialization !!         ║");
            logger.LogInformation("╚═══════════════════════════════════════════════════╝");
            logger.LogInformation("");

            logger.LogInformation("");
            logger.LogInformation($"Version : {appSettings.Version}");
            logger.LogInformation("");

            return serviceProvider;
        }
    }
}
