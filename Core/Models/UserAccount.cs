namespace Core.Models;

public class UserAccount
{
    public int Id { get; set; }
    public Guid ExternalId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
    public ICollection<UserPermissions> UserPermissions { get; set; }
}