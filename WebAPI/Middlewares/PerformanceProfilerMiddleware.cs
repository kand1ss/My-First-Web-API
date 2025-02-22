using System.Diagnostics;

namespace WebAPI;

public class PerformanceProfilerMiddleware(RequestDelegate requestDelegate, ILogger<PerformanceProfilerMiddleware> logger)
{
    public async Task Invoke(HttpContext httpContext)
    {
        var stopWatch = Stopwatch.StartNew();
        await requestDelegate(httpContext);
        stopWatch.Stop();
        
        var method = httpContext.Request.Method;
        var path = httpContext.Request.Path;
        var executionTime = stopWatch.ElapsedMilliseconds;

        logger.LogInformation($"{method} {path} took {executionTime} ms");
    }
}