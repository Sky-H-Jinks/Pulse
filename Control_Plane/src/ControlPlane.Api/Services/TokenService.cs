using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ControlPlane.Api.Database.Tables;
using Microsoft.IdentityModel.Tokens;

namespace ControlPlane.Api.Services;

public class TokenService
{
    private readonly string _key;
    private readonly string _issuer;

    public TokenService(string key, string issuer)
    {
        _key = key;
        _issuer = issuer;
    }

    public string CreateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("username", user.Username),
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _issuer,                      
            claims: claims,
            expires: DateTime.UtcNow.AddHours(ConfigService.GetConfigValue<int>(ConfigKeys.TokenExpirationHours)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}