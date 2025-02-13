using Easy.Net.Starter.EntityFramework;
using Easy.Net.Starter.Extensions;
using Easy.Net.Starter.Models;
using Easy.Net.Starter.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Serilog;

namespace Easy.Net.Starter.Startup.Registrators
{
    public static class ApplicationRegistrator
    {
        public static TAppSettings RegisterAppSettings<TAppSettings>(
            this IServiceCollection services, IConfiguration configuration)
            where TAppSettings : AppSettings, new()
        {
            try
            {
                var configurationSettings = new TAppSettings();
                configuration.Bind(configurationSettings);
                services.AddSingleton(configurationSettings);

                // Also Register the appsettings for core api workload
                var appSettings = configurationSettings as AppSettings;
                configuration.Bind(appSettings);
                services.AddSingleton(appSettings);

                Log.Logger?.Information("Appsettings registered");

                return configurationSettings;
            }
            catch (Exception ex)
            {
                Log.Logger?.Error(ex, "Unable to register the configuration");
                throw;
            }
        }

        public static IConfiguration? RegisterConfiguration()
        {
            try
            {
                DirectoryInfo currentDirectories = new DirectoryInfo(Directory.GetCurrentDirectory());
                var appsettings = currentDirectories.GetFiles("appsettings.json", SearchOption.AllDirectories).FirstOrDefault();

                ArgumentNullException.ThrowIfNull(appsettings, nameof(appsettings));
                ArgumentException.ThrowIfNullOrEmpty(appsettings.FullName, nameof(appsettings));

                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                    .AddJsonFile(appsettings.FullName);

                return configurationBuilder.Build();
            }
            catch (Exception ex)
            {
                Log.Logger?.Error(ex, "Unable to compile the appsettings");
                throw;
            }
        }

        //Call by reflection
        public static void RegisterDatabase<T>(this IServiceCollection services, AppSettings appSettings) where T : DbContext
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL, nameof(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL));

                var applicationConnectionString = appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL.UpdateConnectionPassword();

                var connectionParts = ParseConnectionString(applicationConnectionString);

                connectionParts.TryGetValue("Host", out var host);
                connectionParts.TryGetValue("Database", out var database);
                connectionParts.TryGetValue("Port", out var port);

                Log.Logger.Information("[Database] Database server: {0}", host);
                Log.Logger.Information("[Database] Database: {0}", database);
                Log.Logger.Information("[Database] Port: {0}", port);
                Log.Logger.Information("");

                var dataSource = new NpgsqlDataSourceBuilder(applicationConnectionString)
                  .EnableDynamicJson()
                  .Build();

                services.AddDbContext<T>(options =>
                {
                    options
                        .UseNpgsql(dataSource)
                        .UseSnakeCaseNamingConvention();
                });

                Log.Logger.Information("Database registered");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to register the database");
                throw;
            }
        }

        //Call by reflection
        public static void RegisterDatabaseWithBaseDbContext<T>(this IServiceCollection services, AppSettings appSettings) where T : BaseDbContext
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL, nameof(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL));

                var applicationConnectionString = appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL.UpdateConnectionPassword();

                var connectionParts = ParseConnectionString(applicationConnectionString);

                connectionParts.TryGetValue("Host", out var host);
                connectionParts.TryGetValue("Database", out var database);
                connectionParts.TryGetValue("Port", out var port);

                Log.Logger.Information("[Database] Database server: {0}", host);
                Log.Logger.Information("[Database] Database: {0}", database);
                Log.Logger.Information("[Database] Port: {0}", port);
                Log.Logger.Information("");

                services.AddDbContext<T>(options =>
                {
                    options.UseNpgsql(applicationConnectionString).UseSnakeCaseNamingConvention();
                });

                Log.Logger.Information("Database registered");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to register the database");
                throw;
            }
        }

        public static ServiceProvider RegisterServices(this IServiceCollection services,
            string[] singletonServices,
            string[] scopedServices,
            string[] transientsServices,
            IDictionary<string, string> singletonsWithInterfaces,
            IDictionary<string, string> scopedsWithInterfaces,
            IDictionary<string, string> transientsWithInterfaces)
        {
            try
            {
                RegisterServicesByLifetime(services, singletonServices, ServiceLifetime.Singleton);
                RegisterServicesByLifetime(services, scopedServices, ServiceLifetime.Scoped);
                RegisterServicesByLifetime(services, transientsServices, ServiceLifetime.Transient);

                RegisterServicesWithInterfaces(services, singletonsWithInterfaces, ServiceLifetime.Singleton);
                RegisterServicesWithInterfaces(services, scopedsWithInterfaces, ServiceLifetime.Scoped);
                RegisterServicesWithInterfaces(services, transientsWithInterfaces, ServiceLifetime.Transient);
                return services.BuildServiceProvider();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to register the services");
                throw;
            }
        }

        private static void RegisterServicesByLifetime(
            IServiceCollection services, string[] serviceNames, ServiceLifetime lifetime)
        {
            try
            {
                foreach (var service in serviceNames)
                {
                    var fullname = service.GetNamespaceFromClass();
                    var assembly = service.GetAssemblyFromClass();

                    ArgumentException.ThrowIfNullOrEmpty(fullname, nameof(fullname));
                    ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

                    var type = assembly.GetType(fullname);

                    if (type is null)
                    {
                        Log.Logger.Information($"Type {fullname} not found.");
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
            catch (Exception ex)
            {
                var lifetimeString = lifetime.ToString();
                var serviceNamesString = string.Join(", ", serviceNames);
                Log.Logger.Error(ex, "Unable to register the services by lifetime: {lifetimeString} for services {serviceNamesString}", lifetimeString, serviceNamesString);
                throw;
            }

        }

        private static void RegisterServicesWithInterfaces(
            IServiceCollection services, IDictionary<string, string> servicesWithInterfaces, ServiceLifetime lifetime)
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
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(interfaceFullName, nameof(interfaceFullName));
                ArgumentException.ThrowIfNullOrEmpty(implementationFullName, nameof(implementationFullName));

                // Get interface and implementation full names (with namespaces)
                var fullnameInterfaceFullName = interfaceFullName.GetNamespaceFromClass();
                var fullnameImplementationType = implementationFullName.GetNamespaceFromClass();

                ArgumentException.ThrowIfNullOrEmpty(fullnameInterfaceFullName, "fullnameInterfaceFullName");
                ArgumentNullException.ThrowIfNull(fullnameImplementationType, "fullnameImplementationType");

                // Get the assembly of the interface and the implementation
                var assemblyInterfaceFullName = interfaceFullName.GetAssemblyFromClass();
                var assemblyImplementationType = implementationFullName.GetAssemblyFromClass();

                ArgumentNullException.ThrowIfNull(assemblyInterfaceFullName, "assemblyInterfaceFullName");
                ArgumentNullException.ThrowIfNull(assemblyImplementationType, "assemblyImplementationType");

                // Get the types of the interface and the implementation
                var interfaceType = assemblyInterfaceFullName.GetType(fullnameInterfaceFullName);
                var implementationType = assemblyImplementationType.GetType(fullnameImplementationType);

                ArgumentNullException.ThrowIfNull(interfaceType);
                ArgumentNullException.ThrowIfNull(implementationType);

                if (!interfaceType.IsAssignableFrom(implementationType))
                {
                    throw new ArgumentException($"The class '{implementationFullName}' does not implement the interface '{interfaceFullName}'.");
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
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to register the services by names");
                throw;
            }
        }

        public static Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(connectionString, nameof(connectionString));

                var parts = connectionString.Split(';')
                    .Select(part => part.Trim())
                    .Where(part => !string.IsNullOrEmpty(part))
                    .Select(part => part.Split('='))
                    .Where(split => split.Length == 2)
                    .ToDictionary(split => split[0], split => split[1]);

                return parts;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to parse the connection string");
                throw;
            }
        }
    }
}
