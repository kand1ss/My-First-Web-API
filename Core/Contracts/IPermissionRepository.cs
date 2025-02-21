using Core.Models;

namespace Core.Contracts;

public interface IPermissionRepository
{
    Task<ICollection<Permission>> GetDefaultUserPermissionsAsync(CancellationToken cancellationToken = default);
}