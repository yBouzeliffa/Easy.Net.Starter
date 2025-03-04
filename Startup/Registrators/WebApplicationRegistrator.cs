﻿using Easy.Net.Starter.Api.Swagger;
using Easy.Net.Starter.Extensions;
using Easy.Net.Starter.Models;
using Easy.Net.Starter.Startup.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Easy.Net.Starter.Startup.Registrators
{
    public static class WebApplicationRegistrator
    {
        private readonly static string CORS_API = "CORS_API";

        public static void CheckAspNetCoreEnvironment(this WebApplicationBuilder builder)
        {
            try
            {
                Log.Logger.Information("Environment: {EnvironmentName}", builder.Environment.EnvironmentName);
                if (builder.Environment.IsProduction() && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
                {
                    Log.Logger.Warning("Warning: ASPNETCORE_ENVIRONMENT is not defined on PATH ENVIRONMENT. Using default 'Production'.");
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to check the aspnet core environment");
                throw;
            }
        }

        public static void RegisterSerilog(this ConfigureHostBuilder host, IConfiguration configuration)
        {
            try
            {
                var args = SerilogHelper.ExtractConfiguration(configuration);

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .WriteTo.File(
                        args.Path,
                        rollingInterval: args.RollingInternalEnum,
                        fileSizeLimitBytes: args.FileSizeLimitBytes,
                        retainedFileCountLimit: args.RetainedFileCountLimit)
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
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer"
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new List<string>()
                        }
                    });
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

        public static void RegisterApplicationCapabilities(this WebApplication application)
        {
            try
            {
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
                        var type = assembly.GetType(fullname);

                        if (type == null)
                        {
                            Log.Logger.Information($"Type {fullname} not found.");
                            continue;
                        }

                        application.UseMiddleware(type);

                        Log.Logger.Information($"Middleware {fullname} successfully registered.");
                    }
                }

                Log.Logger.Information("Middlewares registered..");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to register the middlewares");
                throw;
            }
        }
    }
}
