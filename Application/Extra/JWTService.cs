using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Extra;

public class JWTService(IOptions<AuthSettings> authSettings)
{
    public string GenerateToken(UserAccount userAccount)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userAccount.ExternalId.ToString()),
            new(ClaimTypes.Name, userAccount.Login),
            new(ClaimTypes.Email, userAccount.Email),
        };

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.Add(authSettings.Value.TokenLifetime),
            claims: claims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Value.SecretKey)),
                SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}