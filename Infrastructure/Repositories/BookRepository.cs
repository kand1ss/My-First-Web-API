using Core.Contracts;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using AppContext = Infrastructure.Contexts.AppContext;

namespace Infrastructure.Repositories;

public class BookRepository(AppContext context) : IBookRepository
{
    public async Task CreateAsync(Book book, CancellationToken cancellationToken = default)
    {
        await context.Books.AddAsync(book, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        context.Books.Update(book);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Book book, CancellationToken cancellationToken = default)
    {
        context.Books.Remove(book);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Books.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IList<Book>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Books.ToListAsync(cancellationToken);
}