using Easy.Net.Starter.App;
using Easy.Net.Starter.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Easy.Net.Starter.ProgramsEntries
{
    public static class ConsoleAppStarter
    {
        public static ServiceProvider Start(string[] args, string[] singletonServices, string[] scopedServices)
        {
            var builder = Host.CreateApplicationBuilder(args);

            var configuration = ApplicationRegistrator.RegisterConfiguration();

            var serviceProvider = builder.Services
                    .AddSingleton<IGenericLogger, GenericLogger>()
                    .AddServices(configuration, singletonServices, scopedServices);

            ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));

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
