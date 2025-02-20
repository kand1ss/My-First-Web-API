using Core.Contracts;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using AppContext = Infrastructure.Contexts.AppContext;

namespace Infrastructure.Repositories;

public class AuthorRepository(AppContext context) : IAuthorRepository
{
    public async Task CreateAsync(Author author, CancellationToken cancellationToken = default)
    {
        await context.Authors.AddAsync(author, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Author author, CancellationToken cancellationToken = default)
    {
        context.Authors.Update(author);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Author author, CancellationToken cancellationToken = default)
    {
        context.Authors.Remove(author);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Authors.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IList<Author>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Authors.ToListAsync(cancellationToken);
}