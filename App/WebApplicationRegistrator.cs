using Easy.Net.Starter.Extensions;
using Easy.Net.Starter.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;

namespace Easy.Net.Starter.App
{
    public static class WebApplicationRegistrator
    {
        private readonly static string CORS_API = "CORS_API";

        public static void RegisterSerilog(this ConfigureHostBuilder host, IConfiguration configuration)
        {
            try
            {
                var overrideWriteLogToFile = configuration["OverrideWriteLogToFile"];

                ArgumentException.ThrowIfNullOrEmpty(overrideWriteLogToFile, nameof(overrideWriteLogToFile));

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .WriteTo.File(overrideWriteLogToFile.Replace("{APP_NAME}", Process.GetCurrentProcess().ProcessName))
                    .CreateLogger();
                host.UseSerilog();

                Log.Logger.Information("Serilog registered");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to register Serilog", ex);
            }
        }

        public static void RegisterApiCapabilities(this IServiceCollection services, AppSettings appSettings)
        {
            try
            {
                services.AddControllers();
                services.AddLogging();
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen(options =>
                {
                    options.OperationFilter<AddApiKeyHeaderFilter>("X-API-KEY", appSettings.ApiKey.ApiPublicKey);
                    options.OperationFilter<AddReferrerHeaderFilter>("Referer", appSettings.AllowedOrigins.Split(',').ToArray().First());
                });

                services.AddHealthChecks();

                Log.Logger.Information("Api Capabilities registered");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to register the api capabilities");
                throw;
            }
        }

        public static void RegisterApplicationCapabilities(this WebApplication application, string apiName)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(apiName, nameof(apiName));

                application.UseHttpsRedirection();
                application.UseAuthentication();
                application.UseAuthorization();
                application.MapControllers();
                application.UseSwagger();
                application.UseSwaggerUI();
                application.MapHealthChecks("api/health");

                Log.Logger.Information("Application Capabilities registered");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to register the application capabilities");
                throw;
            }
        }

        public static void RegisterCors(this IServiceCollection services, AppSettings appSettings)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(appSettings.AllowedOrigins, nameof(appSettings.AllowedOrigins));
                ArgumentException.ThrowIfNullOrEmpty(appSettings.AllowedMethods, nameof(appSettings.AllowedMethods));
                ArgumentException.ThrowIfNullOrEmpty(appSettings.AllowedHeaders, nameof(appSettings.AllowedHeaders));

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
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to register the cors");
                throw;
            }
        }

        public static void UseAppCors(this WebApplication webApplication)
        {
            try
            {
                webApplication.UseCors(CORS_API);
            }
            catch (Exception ex)
            {

                Log.Logger.Error(ex, "Unable to use the cors");
                throw;
            }
        }

        public static void RegisterMiddlewares(this IApplicationBuilder application, string[] middlewareNames)
        {
            try
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
                            Log.Logger.Information($"Type {fullname} not found.");
                            continue;
                        }

                        application.UseMiddleware(type);

                        Log.Logger.Information($"Middleware {fullname} successfully registered.");
                    }
                }

                Log.Logger.Information("All middlewares have been registered..");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to register the middlewares");
                throw;
            }
        }
    }
}
