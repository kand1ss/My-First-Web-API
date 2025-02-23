using Core.Contracts;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using AppContext = Infrastructure.Contexts.AppContext;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository(AppContext context) : IRefreshTokenRepository
{
    public async Task AddRefreshToken(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task RemoveRefreshToken(RefreshToken refreshToken)
    {
        context.RefreshTokens.Remove(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task UpdateRefreshToken(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenByUserGuid(string guid)
        => await context.RefreshTokens.FirstOrDefaultAsync(t => t.UserId.ToString() == guid);
    
    public async Task<RefreshToken?> GetRefreshTokenByToken(string token)
        => await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
}