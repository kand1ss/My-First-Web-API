namespace Core.Models;

public class Author
{
    public int? Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<Book> Books { get; set; } = new List<Book>();
    public DateTime CreatedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
}