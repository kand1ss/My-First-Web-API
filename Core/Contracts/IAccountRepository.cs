using Core.Models;

namespace Core.Contracts;

public interface IAccountRepository
{
    public Task CreateAsync(UserAccount userAccount, CancellationToken cancellationToken = default);
    public Task UpdateAsync(UserAccount userAccount, CancellationToken cancellationToken = default);
    public Task DeleteAsync(UserAccount userAccount, CancellationToken cancellationToken = default);
    
    public Task<UserAccount?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<UserAccount?> GetByGuidAsync(string guid, CancellationToken cancellationToken = default);
    public Task<UserAccount?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
    public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    public Task<IList<UserAccount>> GetAllAsync(CancellationToken cancellationToken = default);
    
    public Task<IList<Permission>> GetAllPermissionsByGuidAsync(string guid, CancellationToken cancellationToken = default);
}