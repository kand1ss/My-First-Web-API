using Microsoft.AspNetCore.Authorization;

namespace Application.Permissions;

public class PermissionRequirements(string permission) : IAuthorizationRequirement
{
    public string Permission { get; set; } = permission;
}