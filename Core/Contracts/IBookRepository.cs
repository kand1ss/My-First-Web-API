using Core.Models;

namespace Core.Contracts;

public interface IBookRepository
{
    public Task CreateAsync(Book book, CancellationToken cancellationToken = default);
    public Task UpdateAsync(Book book, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Book book, CancellationToken cancellationToken = default);
    
    public Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<Book?> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
    public Task<IList<Book>> GetAllAsync(CancellationToken cancellationToken = default);
}