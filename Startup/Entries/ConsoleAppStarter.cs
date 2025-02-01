using Easy.Net.Starter.Loggers;
using Easy.Net.Starter.Models;
using Easy.Net.Starter.Startup.Registrators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;

namespace Easy.Net.Starter.Startup.Entries
{
    public static class ConsoleAppStarter
    {
        public static ServiceProvider Start<TAppSettings>(string[] args, StartupOptions options)
            where TAppSettings : AppSettings, new()
        {
            try
            {
                var builder = Host.CreateApplicationBuilder(args);

                ArgumentNullException.ThrowIfNull(builder, nameof(builder));

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

                if (options.UseDefaultGenericLogger)
                    builder.Services.AddScoped<IGenericLogger, GenericLogger>();

                var serviceProvider = builder.Services.RegisterServices(
                    options.SingletonServices,
                    options.ScopedServices,
                    options.TransientServices,
                    options.SingletonsWithInterfaces,
                    options.ScopedWithInterfaces,
                    options.TransientsWithInterfaces);

                ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));

                var logger = serviceProvider.GetRequiredService<IGenericLogger>();

                ArgumentNullException.ThrowIfNull(logger, nameof(logger));

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
            catch (Exception ex)
            {
                Console.WriteLine("Unable to start the application corretly");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
