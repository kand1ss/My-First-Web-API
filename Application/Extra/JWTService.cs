using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.DTO;
using Core.Contracts;
using Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Extra;

public class JWTService(IRefreshTokenRepository tokenRepository, IOptions<AuthSettings> authSettings)
{
    public async Task<TokensDTO> GenerateTokens(UserAccount userAccount)
    {
        var accessToken = GenerateAccessToken(userAccount);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenFromRepo = await tokenRepository.GetRefreshTokenByUserGuid(userAccount.ExternalId.ToString());
        
        await SaveRefreshToken(userAccount.ExternalId, refreshTokenFromRepo, refreshToken);

        return new TokensDTO(accessToken, refreshToken);
    }

    private async Task SaveRefreshToken(Guid userGuid, RefreshToken? refreshTokenFromRepo, string newRefreshToken)
    {
        var refreshTokenExpiration = DateTime.UtcNow.Add(authSettings.Value.RefreshTokenLifetime);
        
        if (refreshTokenFromRepo is null)
            await tokenRepository.AddRefreshToken(new RefreshToken
            {
                UserId = userGuid,
                Token = newRefreshToken,
                ExpiresAt = refreshTokenExpiration
            });
        else
        {
            refreshTokenFromRepo.Token = newRefreshToken;
            refreshTokenFromRepo.ExpiresAt = refreshTokenExpiration;
            await tokenRepository.UpdateRefreshToken(refreshTokenFromRepo);
        }
    }

    private string GenerateAccessToken(UserAccount userAccount)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userAccount.ExternalId.ToString()),
            new(ClaimTypes.Name, userAccount.Login),
        };

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.Add(authSettings.Value.AccessTokenLifetime),
            claims: claims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Value.SecretKey)),
                SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        RandomNumberGenerator.Fill(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}