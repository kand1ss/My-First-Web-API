using Application.DTO;

namespace Application.Services;

public interface IBookService
{
    Task AddBook(BookDTO bookData);
    Task DeleteBook(int id);
    Task UpdateBook(int id, BookUpdateDTO bookUpdateData);
    
    Task<BookDTO> GetBookById(int id);
    Task<IEnumerable<BookDTO>> GetAllBooks();
    Task<BookDTO> GetBookByTitle(string title);
}