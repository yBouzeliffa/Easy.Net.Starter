using Easy.Net.Starter.Models;
using Easy.Net.Starter.Services;
using Easy.Net.Starter.Startup.Registrators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Easy.Net.Starter.Startup.Entries
{
    public static class WebApiStarter
    {
        public static async Task<WebApplication?> StartAsync<TAppSettings>(string[] args, StartupOptions options)
            where TAppSettings : AppSettings, new()
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.RegisterSerilog(builder.Configuration);

                builder.CheckAspNetCoreEnvironment();

                var appSettings = builder.Services.RegisterAppSettings<TAppSettings>(
                    builder.Configuration,
                    options.UseEmailingService,
                    options.EmailProvider);

                ArgumentNullException.ThrowIfNull(appSettings, nameof(appSettings));
                ArgumentException.ThrowIfNullOrEmpty(appSettings.Version, nameof(appSettings.Version));

                Log.Logger.Information("");
                Log.Logger.Information("╔═══════════════════════════════════════════════════╗");
                Log.Logger.Information("║              !! Api initialization !!             ║");
                Log.Logger.Information("╚═══════════════════════════════════════════════════╝");
                Log.Logger.Information("");

                Log.Logger.Information("");
                Log.Logger.Information("Version : {Version}", appSettings.Version);
                Log.Logger.Information("");

                builder.ConfigureDatabase(options, appSettings);

                if (EF.IsDesignTime)
                {
                    Log.Logger.Information("Design time detected. Skipping the application startup.");
                    return null;
                }

                if (options.UseSignalR)
                    builder.Services.AddSignalR();

                builder.Services.RegisterApiCapabilities(appSettings);
                builder.Services.RegisterCors(appSettings);

                builder.Services.RegisterServices(
                    options.SingletonServices,
                    options.ScopedServices,
                    options.TransientServices,
                    options.SingletonsWithInterfaces,
                    options.ScopedWithInterfaces,
                    options.TransientsWithInterfaces);

                builder.Services.AddScoped<IUserContextService, UserContextService>();

                if (options.UseJwtAuthentication)
                    builder.Services.ConfigureAuthentication(appSettings);

                var app = builder.Build();

                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

                app.RegisterApplicationCapabilities();

                if (options.UseHttpLoggerMiddleware)
                    app.UseMiddleware<HttpLoggerMiddleware>();

                app.RegisterMiddlewares(options.Middlewares);
                app.RegisterExceptionHandler(appSettings);
                app.UseAppCors();

                app.Services.ApplyDbMigrationsChanges(options);

                Log.Logger.Information("");
                Log.Logger.Information("╔═══════════════════════════════════════════════════╗");
                Log.Logger.Information("║              Application starting ...             ║");
                Log.Logger.Information("╚═══════════════════════════════════════════════════╝");
                Log.Logger.Information("");

                return app;
            }
            catch (Exception ex)
            {
                Log.Logger?.Error(ex, "Unable to start the application");
                throw;
            }
        }
    }
}
