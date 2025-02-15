using Easy.Net.Starter.Api.Authentications;
using Easy.Net.Starter.EntityFramework;
using Easy.Net.Starter.Models;
using Easy.Net.Starter.Security;
using Easy.Net.Starter.Services;
using Easy.Net.Starter.Startup.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;

namespace Easy.Net.Starter.Startup.Registrators
{
    internal static class DatabaseRegistrator
    {
        public static void ApplyChanges(this IServiceProvider services, StartupOptions options)
        {
            if (!options.UseDatabase)
                return;

            ArgumentNullException.ThrowIfNull(options.DatabaseContextType, nameof(options.DatabaseContextType));

            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService(options.DatabaseContextType) as DbContext;

            ArgumentNullException.ThrowIfNull(context, nameof(context));

            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while applying database migrations.");
                throw;
            }
        }

        public static void ConfigureDatabase<TAppSettings>(this WebApplicationBuilder builder, StartupOptions options, TAppSettings appSettings)
            where TAppSettings : AppSettings, new()
        {
            builder.Host.ConfigureServices((context, services) =>
            {
                services.ConfigureDatabase(options, appSettings);
            });
        }

        public static void ConfigureDatabase<TAppSettings>(this IServiceCollection services, StartupOptions options, TAppSettings appSettings)
            where TAppSettings : AppSettings, new()
        {
            if (options.UseDatabase && options.DatabaseContextType != null)
            {
                ArgumentException.ThrowIfNullOrEmpty(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL, nameof(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL));

                var connectionParts = ApplicationRegistrator.ParseConnectionString(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL);

                connectionParts.TryGetValue("Database", out var databaseName);

                ArgumentException.ThrowIfNullOrEmpty(databaseName, nameof(databaseName));

                var instanceManagerConnectionString = appSettings.ConnectionStrings.INSTANCE_MANAGER_POSTGRE_SQL.UpdateConnectionPassword(databaseName);
                var applicationConnectionString = appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL.UpdateConnectionPassword(databaseName);

                if (!instanceManagerConnectionString.TryDatabaseConnection())
                {
                    throw new InvalidOperationException("Database connection failed.");
                }

                var databaseManager = new DatabaseManager(instanceManagerConnectionString, connectionParts["Database"]);
                databaseManager.EnsureDatabaseExistsAsync().Wait();

                var method = typeof(ApplicationRegistrator).GetMethod(
                    options.UseDatabaseWithBuiltInUserConfiguration ? "RegisterDatabaseWithBaseDbContext" : "RegisterDatabase",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new[] { typeof(IServiceCollection), typeof(AppSettings) },
                    null
                );

                ArgumentNullException.ThrowIfNull(method, nameof(method));

                var genericMethod = method.MakeGenericMethod(options.DatabaseContextType);

                genericMethod.Invoke(null, new object[] { services, appSettings });
            }
            else if (options.UseDatabase)
            {
                throw new InvalidOperationException("DatabaseContextType must be specified when UseDatabase is true.");
            }

            services.AddScoped<IRsaService, RsaService>();

            if (options.UseDatabaseWithBuiltInUserConfiguration && options.UseJwtAuthentication)
            {
                services.AddScoped<BaseDbContext>(provider =>
                {
                    if (options.DatabaseContextType == null)
                        throw new InvalidOperationException("DatabaseContextType is not specified.");

                    var dbContextInstance = provider.GetRequiredService(options.DatabaseContextType);

                    return (BaseDbContext)dbContextInstance;
                });
                services.AddScoped<ITokenService, MyTokenService>();
                services.AddScoped<IUserService, UserService>();
                services.AddTransient<IClaimsTransformation, UserClaimsTransformation>();
            }
        }
    }
}
