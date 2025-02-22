using Core.Contracts;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;
using AppContext = Infrastructure.Contexts.AppContext;

public class PermissionRepository(AppContext context, IMemoryCache cache) : IPermissionRepository
{
    public async Task<ICollection<Permission>> GetDefaultUserPermissionsAsync(
        CancellationToken cancellationToken = default)
    {
        string cacheKey = "permissions_defaultUserPermissions";

        if (!cache.TryGetValue(cacheKey, out ICollection<Permission> permissions) || permissions is null)
        {
            permissions = await context.Permissions
                .Where(p => p.Name == "Read")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(60))
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            cache.Set(cacheKey, permissions, cacheOptions);
        }
        
        return permissions;
    }
}