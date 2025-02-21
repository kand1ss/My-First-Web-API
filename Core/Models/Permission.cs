namespace Core.Models;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<UserPermissions> UserPermissions { get; set; }
}