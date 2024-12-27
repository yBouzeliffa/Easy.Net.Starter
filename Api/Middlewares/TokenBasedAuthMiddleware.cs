using Easy.Net.Starter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace Easy.Net.Starter.Api.Middlewares;

public class TokenBasedAuthMiddleware
{
    private const string BearerScheme = "Bearer";
    private readonly RequestDelegate next;
    private readonly ILogger<TokenBasedAuthMiddleware> logger;

    public TokenBasedAuthMiddleware(RequestDelegate next, ILogger<TokenBasedAuthMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService jwtTokenManager)
    {
        try
        {
            var authHeader = context.Request.Headers.ContainsKey(HeaderNames.Authorization)
              ? AuthenticationHeaderValue.Parse(context.Request.Headers[HeaderNames.Authorization])
              : null;

            if (context.User.Identity != null && context.User.Identity.IsAuthenticated && authHeader?.Scheme != BearerScheme) // User is Auhtenticated through Windows
            {
                var token = jwtTokenManager.GenerateToken(context.User.Identity.Name);
                context.Request.Headers[HeaderNames.Authorization] = $"{BearerScheme} {token}";
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occurred on token based auth middleware: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
        finally
        {
            await next(context);
        }
    }
}
