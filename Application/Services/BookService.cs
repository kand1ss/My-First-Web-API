using System.ComponentModel.DataAnnotations;
using Application.DTO;
using Application.Extra;
using Core.Contracts;
using Core.Models;

namespace Application.Services;

public class BookService(
    IBookRepository bookRepository, IAuthorRepository authorRepository, ICacheService cacheService) : IBookService
{
    private async Task<T> TryGetFromCache<T>(string cacheKey, Func<Task<T>> getFromDb) where T : class
    {
        if(cacheService.TryGet(cacheKey, out T? entity) && entity is not null)
            return entity;
        
        entity = await getFromDb();
        if(entity is null)
            throw new ValidationException($"{typeof(T).Name} not found.");
        
        return entity;
    }

    private async Task<Book> TryGetBookFromCache(int id)
        => await TryGetFromCache(CacheKeysTemplates.BookKey(id.ToString()), () => bookRepository.GetByIdAsync(id));
    private async Task<Author> TryGetAuthorFromCache(int id)
        => await TryGetFromCache(CacheKeysTemplates.AuthorKey(id.ToString()), () => authorRepository.GetByIdAsync(id));
    
    
    public async Task AddBook(BookDTO bookData)
    {
        var author = await TryGetAuthorFromCache(bookData.authorId);
        var book = BookFactory.Create(bookData);
        book.Author = author;
        
        await bookRepository.CreateAsync(book);

        var cacheKey = CacheKeysTemplates.BookKey(book.Id.ToString());
        cacheService.Put(cacheKey, book, 10, 5);
    }

    public async Task DeleteBook(int id)
    {
        var book = await TryGetBookFromCache(id);
        await bookRepository.DeleteAsync(book);
        
        var cacheKey = CacheKeysTemplates.BookKey(id.ToString());
        cacheService.Remove(cacheKey);
    }

    public async Task UpdateBook(int id, BookUpdateDTO bookUpdateData)
    {
        var book = await TryGetBookFromCache(id);
        
        ValidateAndSet(book, bookUpdateData);
        
        book.ModifiedUtc = DateTime.UtcNow;
        await bookRepository.UpdateAsync(book);
        
        var cacheKey = CacheKeysTemplates.BookKey(id.ToString());
        cacheService.Put(cacheKey, book, 10, 5);
    }

    private void ValidateAndSet(Book book, BookUpdateDTO bookDTO)
    {
        book.Title = bookDTO.Title ?? book.Title;
        book.PublishDate = bookDTO.PublishDate ?? book.PublishDate;
        book.AuthorId = bookDTO.AuthorId ?? book.AuthorId;
    }

    public async Task<BookDTO> GetBookById(int id)
    {
        var book = await TryGetBookFromCache(id);
        return book.ToDTO();
    }

    public async Task<IEnumerable<BookDTO>> GetAllBooks()
        => (await bookRepository.GetAllAsync()).ToDTOs();

    public async Task<BookDTO> GetBookByTitle(string title)
    {
        var book = await bookRepository.GetByTitleAsync(title);
        if(book is null)
            throw new ValidationException($"Book with title {title} not found");
        
        return book.ToDTO();
    }
}