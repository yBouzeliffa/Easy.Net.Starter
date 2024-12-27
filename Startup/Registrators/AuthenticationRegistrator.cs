using Easy.Net.Starter.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Easy.Net.Starter.Startup.Registrators
{
    public static class AuthenticationRegistrator
    {
        public static void ConfigureAuthentication(this IServiceCollection services, AppSettings appConfig)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Else use IIS scheme
                    .AddNegotiate()
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = appConfig.Jwt.Issuer,
                            ValidAudience = appConfig.Jwt.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfig.Jwt.Key))
                        };
                    });
        }
    }
}
