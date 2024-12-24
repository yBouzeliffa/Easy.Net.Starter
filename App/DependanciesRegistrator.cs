using Easy.Net.Starter.Extensions;
using Easy.Net.Starter.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;

namespace Easy.Net.Starter.App
{
    public static class DependanciesRegistrator
    {
        public static IConfiguration? RegisterConfiguration(EmbeddedRessource specificAppsettings)
        {
            try
            {
                var appsettingsFromMemory = EmbeddedRessource.Appsettings.GetDescription().FindAndGetFileFromEmbeddedResource("Globals.Tools");
                var appsettingsApplicationFromMemory = specificAppsettings.GetDescription().FindAndGetFileFromEmbeddedResource("Globals.Tools");

                if (appsettingsFromMemory == null || appsettingsApplicationFromMemory == null)
                {
                    throw new ArgumentNullException(nameof(appsettingsFromMemory));
                }

                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                    .AddJsonStream(appsettingsFromMemory)
                    .AddJsonStream(appsettingsApplicationFromMemory);

                return configurationBuilder.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to compile the configuration");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static ServiceProvider? AddServices(
            this IServiceCollection serviceCollection,
            IConfiguration? configuration,
            string[] requiredSingletonServices,
            string[] requiredScopedServices)
        {
            try
            {
                if (configuration != null)
                {
                    var configurationSettings = new AppSettings();
                    configuration.Bind(configurationSettings); // Bind appsettings info to data model
                    serviceCollection.AddSingleton(configurationSettings);

                    string appName = Process.GetCurrentProcess().ProcessName;

                    serviceCollection.AddSerilog(config =>
                    {
                        config
                            .ReadFrom.Configuration(configuration)
                            .WriteTo.File(configurationSettings.OverrideWriteLogToFile.Replace("{APP_NAME}", appName));
                    });

                    Log.Logger.Information("Configuration registered");
                }

                foreach (var service in requiredSingletonServices)
                {
                    var fullname = service.GetNamespaceFromClass();
                    if (fullname != null)
                    {
                        Type type = Type.GetType(fullname);

                        if (type == null)
                        {
                            Console.WriteLine($"Type {fullname} non trouvé.");
                            continue;
                        }

                        serviceCollection.AddSingleton(type);
                    }
                }

                foreach (var service in requiredScopedServices)
                {
                    var fullname = service.GetNamespaceFromClass();
                    if (fullname != null)
                    {
                        Type type = Type.GetType(fullname);

                        if (type == null)
                        {
                            Console.WriteLine($"Type {fullname} non trouvé.");
                            continue;
                        }

                        serviceCollection.AddTransient(type);
                    }
                }

                return serviceCollection.BuildServiceProvider();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to compile the services");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
