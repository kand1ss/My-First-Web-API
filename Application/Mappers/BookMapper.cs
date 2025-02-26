using Application.DTO;
using Core.Contracts;
using Core.Models;

namespace Application.Extra;

public static class BookMapper
{
    public static BookDTO ToDTO(this Book book)
        => new(book.Title, book.AuthorId, book.PublishDate);
    
    public static IEnumerable<BookDTO> ToDTOs(this IEnumerable<Book> books)
        => books.Select(ToDTO);
}