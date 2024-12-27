using Easy.Net.Starter.App;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Easy.Net.Starter.Services;

public interface ITokenService
{
    string GenerateToken(string Id);
}
public class MyTokenService : ITokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _secretKey;
    private readonly int _expirationMinutes;

    public MyTokenService(AppSettings appSettings)
    {
        _issuer = appSettings.Jwt.Issuer;
        _audience = appSettings.Jwt.Audience;
        _secretKey = appSettings.Jwt.Key;
        _expirationMinutes = appSettings.Jwt.MinutesDuration;
    }

    public string GenerateToken(string Id)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, _issuer),
            new Claim(JwtRegisteredClaimNames.Aud, _audience),
            new Claim(ClaimTypes.NameIdentifier, Id),
            // Vous pouvez ajouter d'autres claims personnalisés ici selon vos besoins
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }
}
