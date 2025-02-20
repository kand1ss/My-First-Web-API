using Application.Extra;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<AccountValidator>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}