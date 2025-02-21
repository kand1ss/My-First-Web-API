using System.ComponentModel.DataAnnotations;
using Application.DTO;
using Application.Extra;
using Core.Contracts;
using Core.Models;

namespace Application.Services;

public class BookService(IBookRepository bookRepository, IAuthorRepository authorRepository) : IBookService
{
    public async Task AddBook(BookDTO bookData)
    {
        var author = await authorRepository.GetByIdAsync(bookData.authorId);
        if (author is null)
            throw new ValidationException($"Author with id {bookData.authorId} not found");

        var book = BookFactory.Create(bookData);
        book.Author = author;
        
        await bookRepository.CreateAsync(book);
    }

    public async Task DeleteBook(int id)
    {
        var book = await bookRepository.GetByIdAsync(id);
        if(book is null)
            throw new ValidationException($"Book with id {id} not found");
        
        await bookRepository.DeleteAsync(book);
    }

    public async Task UpdateBook(int id, BookUpdateDTO bookUpdateData)
    {
        var book = await bookRepository.GetByIdAsync(id);
        if(book is null)
            throw new ValidationException($"Book with title {bookUpdateData.Title} not found");
        
        ValidateAndSet(book, bookUpdateData);
        
        book.ModifiedUtc = DateTime.UtcNow;
        await bookRepository.UpdateAsync(book);
    }

    private void ValidateAndSet(Book book, BookUpdateDTO bookDTO)
    {
        if(bookDTO.Title != null && book.Title != bookDTO.Title)
            book.Title = bookDTO.Title;
        if(bookDTO.PublishDate.HasValue && book.PublishDate != bookDTO.PublishDate.Value)
            book.PublishDate = bookDTO.PublishDate.Value;
        if(bookDTO.AuthorId.HasValue && book.AuthorId != bookDTO.AuthorId.Value)
            book.AuthorId = bookDTO.AuthorId.Value;
    }

    public async Task<BookDTO> GetBookById(int id)
    {
        var book = await bookRepository.GetByIdAsync(id);
        if(book is null)
            throw new ValidationException($"Book with id {id} not found");
        
        return book.ToDTO();
    }

    public async Task<IList<BookDTO>> GetAllBooks()
        => (await bookRepository.GetAllAsync()).ToDTOs().ToList();

    public async Task<BookDTO> GetBookByTitle(string title)
    {
        var book = await bookRepository.GetByTitleAsync(title);
        if(book is null)
            throw new ValidationException($"Book with title {title} not found");
        
        return book.ToDTO();
    }
}