using Core.Models;

namespace Core.Contracts;

public interface IRefreshTokenRepository
{
    public Task AddRefreshToken(RefreshToken refreshToken);
    public Task RemoveRefreshToken(RefreshToken refreshToken);
    public Task UpdateRefreshToken(RefreshToken refreshToken);
    public Task<RefreshToken?> GetRefreshTokenByUserGuid(string userGuid);
    public Task<RefreshToken?> GetRefreshTokenByToken(string token);
}