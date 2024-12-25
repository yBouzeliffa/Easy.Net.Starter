using Easy.Net.Starter.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using Serilog;

namespace Easy.Net.Starter.App
{
    public static class WebApplicationRegistrator
    {
        public static void RegisterSerilog(this ConfigureHostBuilder host, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            host.UseSerilog();
        }

        public static void RegisterDatabase<T>(this IServiceCollection services, AppSettings appSettings) where T : DbContext
        {
            var connectionParts = ParseConnectionString(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL);

            Log.Logger.Information("[Database] Database server: {0}", connectionParts["Host"]);
            Log.Logger.Information("[Database] Database: {0}", connectionParts["Database"]);
            Log.Logger.Information("[Database] Port: {0}", connectionParts["Port"]);
            Log.Logger.Information("");

            services.AddDbContext<T>(options =>
            {
                options.UseNpgsql(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL).UseSnakeCaseNamingConvention();
            });

            Log.Logger.Information("Database registered");
        }

        public static void RegisterApiCapabilities(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddControllers();
            services.AddLogging();
            services.AddEndpointsApiExplorer();
            services.AddOpenApi();
            services.AddHealthChecks();

            Log.Logger.Information("Api Capabilities registered");
        }

        public static void RegisterApplicationCapabilities(this WebApplication application, string apiName)
        {
            application.UseHttpsRedirection();
            application.UseAuthentication();
            application.UseAuthorization();
            application.MapControllers();
            application.MapOpenApi();
            application.MapScalarApiReference(options =>
            {
                options
                    .WithTitle(apiName)
                    .WithTheme(ScalarTheme.Saturn);
            });

            Log.Logger.Information("Application Capabilities registered");
        }

        private readonly static string CORS_API = "CORS_API";
        public static void RegisterCors(this IServiceCollection services, AppSettings appSettings)
        {
            var allowedOrigins = appSettings.AllowedOrigins.Split(',');
            var allowedMethods = appSettings.AllowedMethods.Split(',');
            var allowedHeaders = appSettings.AllowedHeaders.Split(',');

            services.AddCors(options =>
            {
                options.AddPolicy(CORS_API,
                    builder => builder.WithMethods(allowedMethods).WithHeaders(allowedHeaders).WithOrigins(allowedOrigins));
            });

            Log.Logger.Information("Cors registered");
        }
        public static void UseAppCors(this WebApplication webApplication)
        {
            webApplication.UseCors(CORS_API);
        }

        public static void RegisterMiddlewares(this IApplicationBuilder application, string[] middlewareNames)
        {
            foreach (var name in middlewareNames)
            {
                var fullname = name.GetNamespaceFromClass();
                var assembly = name.GetAssemblyFromClass();

                if (fullname != null && assembly != null)
                {
                    Type type = assembly.GetType(fullname);

                    if (type == null)
                    {
                        Log.Logger.Information($"Type {fullname} non trouvé.");
                        continue;
                    }

                    application.UseMiddleware(type);
                    Log.Logger.Information($"Middleware {fullname} enregistré avec succès.");
                }
            }

            Log.Logger.Information("Tous les middlewares ont été enregistrés.");
        }

        private static Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            var parts = connectionString.Split(';')
                .Select(part => part.Trim())
                .Where(part => !string.IsNullOrEmpty(part))
                .Select(part => part.Split('='))
                .Where(split => split.Length == 2)
                .ToDictionary(split => split[0], split => split[1]);

            return parts;
        }
    }
}
