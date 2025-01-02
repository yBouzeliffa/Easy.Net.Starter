using Easy.Net.Starter.Api.Authentications;
using Easy.Net.Starter.EntityFramework;
using Easy.Net.Starter.Models;
using Easy.Net.Starter.Services;
using Easy.Net.Starter.Startup.Helpers;
using Easy.Net.Starter.Startup.Registrators;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;
using System.Reflection;

namespace Easy.Net.Starter.Startup.Entries
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

                builder.CheckAspNetCoreEnvironment();

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
                    ArgumentException.ThrowIfNullOrEmpty(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL, nameof(appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL));

                    if (!appSettings.ConnectionStrings.APPLICATION_POSTGRE_SQL.TryDatabaseConnection())
                    {
                        throw new InvalidOperationException("Database connection failed.");
                    }

                    var method = typeof(ApplicationRegistrator).GetMethod(
                       options.UseDatabaseWithBuiltInUserConfiguration ? "RegisterDatabaseWithBaseDbContext" : "RegisterDatabase",
                        BindingFlags.Static | BindingFlags.Public,
                        null,
                        new[] { typeof(IServiceCollection), typeof(AppSettings) },
                        null
                    );

                    ArgumentNullException.ThrowIfNull(method, nameof(method));

                    var genericMethod = method.MakeGenericMethod(options.DatabaseContextType);

                    genericMethod.Invoke(null, new object[] { builder.Services, appSettings });
                }
                else if (options.UseDatabase)
                {
                    throw new InvalidOperationException("DatabaseContextType must be specified when UseDatabase is true.");
                }

                builder.Services.AddScoped<IRsaService, RsaService>();

                if (options.UseDatabaseWithBuiltInUserConfiguration && options.UseJwtAuthentication)
                {
                    builder.Services.AddScoped<BaseDbContext>(provider =>
                    {
                        if (options.DatabaseContextType == null)
                            throw new InvalidOperationException("DatabaseContextType is not specified.");

                        var dbContextInstance = provider.GetRequiredService(options.DatabaseContextType);

                        return (BaseDbContext)dbContextInstance;
                    });
                    builder.Services.AddScoped<ITokenService, MyTokenService>();
                    builder.Services.AddScoped<IUserService, UserService>();
                    builder.Services.AddTransient<IClaimsTransformation, UserClaimsTransformation>();
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

                if (options.UseJwtAuthentication)
                    builder.Services.ConfigureAuthentication(appSettings);

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
