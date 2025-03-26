using Easy.Net.Starter.Loggers;
using Easy.Net.Starter.Models;
using Easy.Net.Starter.Startup.Registrators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Easy.Net.Starter.Startup.Entries
{
    public static class ConsoleAppStarter
    {
        public static ServiceProvider? Start<TAppSettings>(string[] args, StartupOptions options)
            where TAppSettings : AppSettings, new()
        {
            try
            {
                var builder = Host.CreateApplicationBuilder(args);

                ArgumentNullException.ThrowIfNull(builder, nameof(builder));

                var configuration = ApplicationRegistrator.RegisterConfiguration();

                ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

                var appSettings = builder.Services.RegisterAppSettings<TAppSettings>(configuration);

                builder.Services.AddSerilogConsoleApp(configuration);

                builder.Services.ConfigureDatabase(options, appSettings);

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

                if (EF.IsDesignTime)
                {
                    logger.LogInformation("Design time detected. Skipping the application startup.");
                    return null;
                }

                serviceProvider.ApplyDbMigrationsChanges(options);

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
