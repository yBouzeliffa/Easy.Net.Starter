using Easy.Net.Starter.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Easy.Net.Starter.App
{
    public static class ApplicationRegistrator
    {
        public static TAppSettings RegisterAppSettings<TAppSettings>(this IServiceCollection services, IConfiguration configuration)
            where TAppSettings : AppSettings, new()
        {
            var configurationSettings = new TAppSettings();
            configuration.Bind(configurationSettings);
            services.AddSingleton(configurationSettings);

            Log.Logger.Information("Configuration registered");

            return configurationSettings;
        }

        public static IConfiguration? RegisterConfiguration()
        {
            try
            {
                DirectoryInfo currentDirectories = new DirectoryInfo(Directory.GetCurrentDirectory());
                var appsettings = currentDirectories.GetFiles("appsettings.json", SearchOption.AllDirectories).FirstOrDefault();

                ArgumentException.ThrowIfNullOrEmpty(appsettings?.FullName, nameof(appsettings));

                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                    .AddJsonFile(appsettings?.FullName);

                return configurationBuilder.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to compile the configuration");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static ServiceProvider RegisterServices(IServiceCollection services,
            string[] singletonServices,
            string[] scopedServices,
            string[] transientsServices,
            IDictionary<string, string> singletonsWithInterfaces,
            IDictionary<string, string> scopedsWithInterfaces,
            IDictionary<string, string> transientsWithInterfaces)
        {
            RegisterServicesByLifetime(services, singletonServices, ServiceLifetime.Singleton);
            RegisterServicesByLifetime(services, scopedServices, ServiceLifetime.Scoped);
            RegisterServicesByLifetime(services, transientsServices, ServiceLifetime.Transient);

            RegisterServicesWithInterfaces(services, singletonsWithInterfaces, ServiceLifetime.Singleton);
            RegisterServicesWithInterfaces(services, scopedsWithInterfaces, ServiceLifetime.Scoped);
            RegisterServicesWithInterfaces(services, transientsWithInterfaces, ServiceLifetime.Transient);
            return services.BuildServiceProvider();
        }

        private static void RegisterServicesByLifetime(IServiceCollection services, string[] serviceNames, ServiceLifetime lifetime)
        {
            foreach (var service in serviceNames)
            {
                var fullname = service.GetNamespaceFromClass();
                var assembly = service.GetAssemblyFromClass();
                if (fullname != null && assembly != null)
                {
                    Type type = assembly.GetType(fullname);

                    if (type == null)
                    {
                        Log.Logger.Information($"Type {fullname} non trouvé.");
                        continue;
                    }

                    switch (lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(type);
                            break;
                        case ServiceLifetime.Scoped:
                            services.AddScoped(type);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(type);
                            break;
                    }
                }
            }
        }

        private static void RegisterServicesWithInterfaces(IServiceCollection services, IDictionary<string, string> servicesWithInterfaces, ServiceLifetime lifetime)
        {
            foreach (var service in servicesWithInterfaces)
            {
                var interfaceName = service.Key;
                var implementationName = service.Value;

                try
                {
                    services.AddServicesByNames(interfaceName, implementationName, lifetime);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Erreur lors de l'enregistrement de {interfaceName} avec {implementationName} : {ex.Message}");
                }
            }
        }

        private static IServiceCollection AddServicesByNames(
            this IServiceCollection services,
            string interfaceFullName,
            string implementationFullName,
            ServiceLifetime lifetime)
        {
            var fullnameInterfaceFullName = interfaceFullName.GetNamespaceFromClass();
            var assemblyInterfaceFullName = interfaceFullName.GetAssemblyFromClass();
            Type interfaceType = assemblyInterfaceFullName.GetType(fullnameInterfaceFullName);

            var fullnameImplementationType = implementationFullName.GetNamespaceFromClass();
            var assemblyImplementationType = implementationFullName.GetAssemblyFromClass();
            Type implementationType = assemblyImplementationType.GetType(fullnameImplementationType);

            ArgumentException.ThrowIfNullOrEmpty(interfaceFullName);
            ArgumentNullException.ThrowIfNull(implementationType);

            if (!interfaceType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException($"La classe '{implementationFullName}' ne met pas en œuvre l'interface '{interfaceFullName}'.");
            }

            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(interfaceType, implementationType);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(interfaceType, implementationType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(interfaceType, implementationType);
                    break;
            }

            return services;
        }
    }
}
