namespace Core.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Author Author { get; set; } = new();
    public int AuthorId { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
}