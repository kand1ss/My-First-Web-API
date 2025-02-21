using Application.DTO;
using Core.Contracts;
using Core.Models;

namespace Application.Extra;

public static class BookMapper
{
    public static BookDTO ToDTO(this Book book)
        => new(book.Title, book.AuthorId, book.PublishDate);
    
    public static ICollection<BookDTO> ToDTOs(this ICollection<Book> books)
        => books.Select(ToDTO).ToList();
}