using Easy.Net.Starter.App;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;

namespace Easy.Net.Starter.Middlewares;

public class HttpLoggerMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate next = next;

    public async Task InvokeAsync(HttpContext context, ILogger<HttpLoggerMiddleware> logger)
    {
        var responseBody = string.Empty;
        string userTag = "Unknown";
        ConnectedUser user = null;
        try
        {
            if (context.Request.Method == "OPTIONS")
            {
                await next(context);
                return;
            }

            if (context.User.Identity.IsAuthenticated)
            {
                var identity = context.User.Identity as ClaimsIdentity;
                user = new ConnectedUser().FromClaims(identity);
                userTag = user.DisplayName;
            }

            logger.LogInformation("--- HTTP CALL BY {UserTag} || IS CALLING METHOD || [{RequestMethod}] {RequestPath}", userTag.TrimEnd(), context.Request.Method, context.Request.Path);
            Stream originalBody = context.Response.Body;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    await next(context);

                    memStream.Position = 0;
                    responseBody = new StreamReader(memStream).ReadToEnd();

                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);

                    stopwatch.Stop();
                    TimeSpan elapsed = stopwatch.Elapsed;
                    if (elapsed.TotalSeconds > 10)
                    {
                        logger.LogInformation("--- --- --- CALL END || [{RequestMethod}] {RequestPath} | Execution time: {ElapsedSeconds} seconds | LOW REQUEST, SHOULD BE CHECKED", context.Request.Method, context.Request.Path, elapsed.TotalSeconds);
                    }
                    else
                    {
                        logger.LogInformation("--- --- --- CALL END || [{RequestMethod}] {RequestPath} | Execution time: {ElapsedSeconds} seconds", context.Request.Method, context.Request.Path, elapsed.TotalSeconds);
                    }
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }

            if (context.Response.StatusCode < 200 || context.Response.StatusCode > 299)
            {
                if (string.IsNullOrEmpty(userTag) || string.IsNullOrEmpty(responseBody))
                {
                    logger.LogError("[{StatusCode}] for {UserTag} detected on [{RequestMethod}] {RequestPath}]", context.Response.StatusCode, userTag, context.Request.Method, context.Request.Path);
                }
                else
                {
                    logger.LogError("[{StatusCode}] for {UserTag} detected on [{RequestMethod}] {RequestPath}\n\t|> [{ResponseBody}]", context.Response.StatusCode, userTag, context.Request.Method, context.Request.Path, responseBody);
                }
            }
        }
        catch (Exception exception)
        {
            var sentryTraceId = Guid.NewGuid().ToString("N");
            logger.LogError("[Sentry_{SentryTraceId}] Unhandled exception detected on global HTTP middleware: {ErrorMessage}\n{StackTrace}\n\n[{ResponseBody}]", sentryTraceId, exception.Message, exception.StackTrace, responseBody);
            await next(context);
        }
    }
}
