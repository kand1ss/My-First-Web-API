using System.Linq.Expressions;
using Core.Contracts;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using AppContext = Infrastructure.Contexts.AppContext;

namespace Infrastructure.Repositories;

public class AccountRepository(AppContext context, ILogger<AccountRepository> logger) 
    : IAccountRepository
{
    public async Task CreateAsync(UserAccount userAccount, CancellationToken cancellationToken = default)
    {
        await context.UserAccounts.AddAsync(userAccount, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserAccount userAccount, CancellationToken cancellationToken = default)
    {
        context.UserAccounts.Update(userAccount);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(UserAccount userAccount, CancellationToken cancellationToken = default)
    {
        context.UserAccounts.Remove(userAccount);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<UserAccount?> GetByGuidAsync(string guid, CancellationToken cancellationToken = default)
        => await context.UserAccounts.FirstOrDefaultAsync(x => x.ExternalId.ToString() == guid, cancellationToken);

    public async Task<UserAccount?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
        => await context.UserAccounts.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);

    public async Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await context.UserAccounts.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task<IEnumerable<UserAccount>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.UserAccounts.ToListAsync(cancellationToken);
}