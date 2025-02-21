namespace Core.Models;

public class UserPermissions
{
    public UserAccount Account { get; set; }
    public int UserId { get; set; }
    
    public Permission Permission { get; set; }
    public int PermissionId { get; set; }
}