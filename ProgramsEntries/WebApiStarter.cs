using Easy.Net.Starter.App;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;

namespace Easy.Net.Starter.ProgramsEntries
{
    public static class WebApiStarter
    {
        public static WebApplication Start<T, TAppSettings>(string[] args, StartupOptions options) where T : DbContext
            where TAppSettings : AppSettings, new()
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.RegisterSerilog(builder.Configuration);

            var appSettings = builder.Services.RegisterAppSettings<TAppSettings>(builder.Configuration);

            Log.Logger.Information("");
            Log.Logger.Information("╔═══════════════════════════════════════════════════╗");
            Log.Logger.Information("║              !! Api initialization !!             ║");
            Log.Logger.Information("╚═══════════════════════════════════════════════════╝");
            Log.Logger.Information("");

            Log.Logger.Information("");
            Log.Logger.Information($"Version : {appSettings.Version}");
            Log.Logger.Information("");

            if (options.UseDatabase)
                builder.Services.RegisterDatabase<T>(appSettings);

            if (options.UseSignalR)
                builder.Services.AddSignalR();

            builder.Services.RegisterApiCapabilities(appSettings);
            builder.Services.RegisterCors(appSettings);

            ApplicationRegistrator.RegisterServices(
                builder.Services,
                options.SingletonServices,
                options.ScopedServices,
                options.TransientServices,
                options.SingletonsWithInterfaces,
                options.ScopedWithInterfaces,
                options.TransientsWithInterfaces);

            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.RegisterApplicationCapabilities(Process.GetCurrentProcess().ProcessName);
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
    }
}
