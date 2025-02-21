namespace WebAPI;

public static class Extensions
{
    public static IServiceCollection AddAPIControllers(this IServiceCollection services)
    {
        services.AddControllers(opt =>
        {
            opt.Filters.Add<ExceptionFilter>();
        });
        return services;
    }
}