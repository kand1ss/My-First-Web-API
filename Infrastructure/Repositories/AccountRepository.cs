using System.Linq.Expressions;
using Core.Contracts;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using AppContext = Infrastructure.Contexts.AppContext;

namespace Infrastructure.Repositories;

public class AccountRepository(AppContext context, IMemoryCache cache, ILogger<AccountRepository> logger) 
    : IAccountRepository
{
    private async Task<UserAccount?> GetFromCacheAsync(string cacheKey, Expression<Func<UserAccount, bool>> predicate,
        CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(cacheKey, out UserAccount? account))
            return account;
        
        account = await context.UserAccounts.
            FirstOrDefaultAsync(predicate, cancellationToken);

        if (account is null)
            return null;

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(15))
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            
        cache.Set(cacheKey, account, cacheOptions);
        logger.LogInformation("Account cached");
        
        return account;
    }

    private void RemoveFromCache(UserAccount account)
    {
        cache.Remove($"Account_{account.ExternalId}");
        cache.Remove($"Account_{account.Login}");
        cache.Remove($"Account_{account.Email}");
        
        logger.LogInformation("Account removed from cache");
    }
    
    
    
    public async Task CreateAsync(UserAccount userAccount, CancellationToken cancellationToken = default)
    {
        await context.UserAccounts.AddAsync(userAccount, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserAccount userAccount, CancellationToken cancellationToken = default)
    {
        context.UserAccounts.Update(userAccount);
        await context.SaveChangesAsync(cancellationToken);
        
        RemoveFromCache(userAccount);
    }

    public async Task DeleteAsync(UserAccount userAccount, CancellationToken cancellationToken = default)
    {
        RemoveFromCache(userAccount);
        
        context.UserAccounts.Remove(userAccount);
        await context.SaveChangesAsync(cancellationToken);
    }

    
    public async Task<UserAccount?> GetByGuidAsync(string guid, CancellationToken cancellationToken = default)
        => await GetFromCacheAsync($"Account_{guid}", a => a.ExternalId.ToString() == guid, cancellationToken);

    public async Task<UserAccount?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
        => await GetFromCacheAsync($"Account_{login}", a => a.Login == login, cancellationToken);

    public async Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await GetFromCacheAsync($"Account_{email}", a => a.Email == email, cancellationToken);

    public async Task<IList<UserAccount>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.UserAccounts.ToListAsync(cancellationToken);

    public async Task<IList<Permission>> GetAllPermissionsByGuidAsync(string guid, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"AccountPermissions_{guid}";

        if (!cache.TryGetValue(cacheKey, out IList<Permission>? permissions) || permissions is null)
        {
            var userAccount = await context.UserAccounts
                .Include(x => x.UserPermissions)
                .ThenInclude(x => x.Permission)
                .FirstOrDefaultAsync(a => a.ExternalId.ToString() == guid, cancellationToken);
            
            permissions = userAccount?.UserPermissions?.Select(p => p.Permission).ToList() ?? new List<Permission>();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            
            cache.Set(cacheKey, permissions, cacheOptions);
        }

        return permissions;
    }
}