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
        =>await context.Permissions
            .Where(p => p.Name == "Read")
            .AsNoTracking()
            .ToListAsync(cancellationToken);
}