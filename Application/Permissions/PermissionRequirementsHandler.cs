using System.Security.Claims;
using Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Permissions;

public class PermissionRequirementsHandler(IServiceScopeFactory scopeFactory) : AuthorizationHandler<PermissionRequirements>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirements requirement)
    {
        var userGuid = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        using var scope = scopeFactory.CreateScope();
        
        var accountRepository = scope.ServiceProvider.GetService<IAccountRepository>();
        var permissions = await accountRepository.GetPermissionsByGuidAsync(userGuid);
        
        if (permissions.Any(x => x.Name == requirement.Permission))
            context.Succeed(requirement);
    }
}