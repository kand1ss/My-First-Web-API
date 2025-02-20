using Core.Models;

namespace Core.Contracts;

public interface IAuthorRepository
{
    public Task CreateAsync(Author author, CancellationToken cancellationToken = default);
    public Task UpdateAsync(Author author, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Author author, CancellationToken cancellationToken = default);
    public Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<IList<Author>> GetAllAsync(CancellationToken cancellationToken = default);
}