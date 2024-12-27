using Easy.Net.Starter.App;
using Easy.Net.Starter.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;

namespace Easy.Net.Starter.ProgramsEntries
{
    public static class WebApiStarter
    {
        public static WebApplication Start<TAppSettings>(string[] args, StartupOptions options)
            where TAppSettings : AppSettings, new()
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.RegisterSerilog(builder.Configuration);

                var appSettings = builder.Services.RegisterAppSettings<TAppSettings>(builder.Configuration);

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

                if (options.UseDatabase && options.DatabaseContextType != null)
                {
                    var registerDatabaseMethod = typeof(ApplicationRegistrator)
                        .GetMethod("RegisterDatabase")
                        ?.MakeGenericMethod(options.DatabaseContextType);

                    if (registerDatabaseMethod != null)
                    {
                        registerDatabaseMethod.Invoke(null, [builder.Services, appSettings]);
                    }
                    else
                    {
                        throw new InvalidOperationException("RegisterDatabase method not found.");
                    }
                }
                else if (options.UseDatabase)
                {
                    throw new InvalidOperationException("DatabaseContextType must be specified when UseDatabase is true.");
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

                builder.Services.AddScoped<IRsaService, RsaService>();

                var app = builder.Build();

                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

                app.RegisterApplicationCapabilities(Process.GetCurrentProcess().ProcessName);

                if (options.UseHttpLoggerMiddleware)
                    app.UseMiddleware<HttpLoggerMiddleware>();

                app.RegisterMiddlewares(options.Middlewares);
                app.RegisterExceptionHandler(appSettings);
                app.UseAppCors();

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
