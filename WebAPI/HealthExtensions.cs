using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace WebAPI;

public static class HealthExtensions
{
    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection"), name: "Database");

        return services;
    }

    public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions()
        {
            ResponseWriter = async (context, report) =>
            {
                var result = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                    }),
                    totalDuration = report.TotalDuration.TotalMilliseconds
                };
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(result);
            }
        });
        
        return app;
    }
}