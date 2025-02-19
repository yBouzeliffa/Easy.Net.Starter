using Easy.Net.Starter.Exceptions;
using Easy.Net.Starter.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net;

namespace Easy.Net.Starter.Startup.Registrators
{
    public static class ExceptionRegistrator
    {
        public static void RegisterExceptionHandler(this WebApplication app, AppSettings appSettings)
        {
            try
            {
                if (app.Environment.IsDevelopment())
                {
                    app.UseApiDevelopmentExceptionHandler(appSettings.AllowedOrigins);
                }
                else
                {
                    app.UseApiReleaseExceptionHandler(appSettings.AllowedOrigins);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error while registering exception handler.");
                throw;
            }
        }

        private static void UseApiDevelopmentExceptionHandler(this WebApplication app, string urlConfig)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    context.Response.Headers.TryAdd("Access-Control-Allow-Origin", urlConfig);
                    context.Response.Headers.TryAdd("Access-Control-Allow-Headers", "access-control-allow-origin,authorization,content-type");

                    if (contextFeature != null)
                    {
                        if (contextFeature.Error is BusinessException businessException)
                        {
                            app.Logger.LogError("Dev Business Exception: {Error}", contextFeature.Error);
                            var devTechnicalExceptionResult = new ApiExceptionDto
                            {
                                ErrorText = businessException.ErrorText,
                                ErrorTitle = businessException.ErrorTitle,
                                ErrorCode = businessException.ErrorCode ?? "500",
                                StackTrace = businessException.StackTrace ?? string.Empty,
                                Message = businessException.Message,
                            };
                            context.Response.StatusCode = int.TryParse(businessException.ErrorCode, out int code) ? code : 500;
                            await context.Response.WriteAsync(devTechnicalExceptionResult.ToString());
                        }
                        else
                        {
                            app.Logger.LogError("Dev Technical Exception: {Error}", contextFeature.Error);
                            var devTechnicalExceptionResult = new ApiExceptionDto
                            {
                                ErrorTitle = $"Dev Technical Exception",
                                ErrorCode = contextFeature.Error.Message.Contains("401") || contextFeature.Error.Message.Contains("Hôte inconnu") ? "401" : "500",
                                StackTrace = contextFeature.Error.StackTrace ?? string.Empty,
                                Message = contextFeature.Error.Message
                            };
                            context.Response.StatusCode = int.TryParse(devTechnicalExceptionResult.ErrorCode, out int code) ? code : 500;
                            await context.Response.WriteAsync(devTechnicalExceptionResult.ToString());
                        }
                    }
                });
            });
        }

        private static void UseApiReleaseExceptionHandler(this WebApplication app, string urlConfig)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    context.Response.Headers.TryAdd("Access-Control-Allow-Origin", urlConfig);
                    context.Response.Headers.TryAdd("Access-Control-Allow-Headers", "access-control-allow-origin,authorization,content-type");

                    if (contextFeature != null)
                    {
                        if (contextFeature.Error is BusinessException businessException)
                        {
                            app.Logger.LogError("Release Business Exception :{Error}", contextFeature.Error);
                            var devTechnicalExceptionResult = new ApiExceptionDto
                            {
                                ErrorTitle = businessException.ErrorTitle,
                                ErrorText = businessException.ErrorText,
                                ErrorCode = businessException.ErrorCode ?? "500",
                            };
                            context.Response.StatusCode = int.TryParse(businessException.ErrorCode, out int code) ? code : 500;
                            await context.Response.WriteAsync(devTechnicalExceptionResult.ToString());
                        }
                        else
                        {
                            app.Logger.LogError("Release technical Exception: {Error}", contextFeature.Error);
                            var devTechnicalExceptionResult = new ApiExceptionDto
                            {
                                ErrorTitle = $"Oups une erreur est survenue",
                                ErrorText = "Un problème est survenu. Veuillez réessayer plus tard ou contacter votre administrateur",
                                ErrorCode = contextFeature.Error.Message.Contains("401") || contextFeature.Error.Message.Contains("Hôte inconnu") ? "401" : "500",
                            };
                            context.Response.StatusCode = int.TryParse(devTechnicalExceptionResult.ErrorCode, out int code) ? code : 500;
                            await context.Response.WriteAsync(devTechnicalExceptionResult.ToString());
                        }
                    }
                });
            });
        }
    }
}
