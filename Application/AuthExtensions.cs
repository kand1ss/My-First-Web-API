using System.Text;
using Application.Extra;
using Application.Permissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Application;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuthSettings>(configuration.GetSection(nameof(AuthSettings)));
        services.AddScoped<JWTService>();

        services.AddSingleton<IAuthorizationHandler, PermissionRequirementsHandler>();
        var authSettings = configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(Permissions.Permissions.Read, builder =>
                builder.Requirements.Add(new PermissionRequirements(Permissions.Permissions.Read)));
            opt.AddPolicy(Permissions.Permissions.Delete, builder =>
                builder.Requirements.Add(new PermissionRequirements(Permissions.Permissions.Delete)));
            opt.AddPolicy(Permissions.Permissions.Create, builder =>
                builder.Requirements.Add(new PermissionRequirements(Permissions.Permissions.Create)));
            opt.AddPolicy(Permissions.Permissions.Edit, builder =>
                builder.Requirements.Add(new PermissionRequirements(Permissions.Permissions.Edit)));
        });
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(authSettings.SecretKey))
            };

            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("accessToken"))
                        context.Token = context.Request.Cookies["accessToken"];

                    return Task.CompletedTask;
                }
            };
        });
        
        return services;
    }
}