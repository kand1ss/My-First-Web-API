using Core.Contracts;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
using AppContext = Infrastructure.Contexts.AppContext;

public class PermissionRepository(AppContext context) : IPermissionRepository
{
    public async Task<ICollection<Permission>> GetDefaultUserPermissionsAsync(
        CancellationToken cancellationToken = default)
        => await context.Permissions.Where(p => p.Name == "Read").ToListAsync(cancellationToken);
}