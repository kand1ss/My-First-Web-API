using Application.DTO;
using Core.Models;

namespace Application;

public static class BookFactory
{
    public static Book Create(BookDTO bookData)
        => new()
        {
            Title = bookData.Title,
            PublishDate = bookData.PublishDate,
            CreatedUtc = DateTime.UtcNow
        };
}