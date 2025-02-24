using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Extra;

public class MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger) : ICacheService
{
    public bool TryGet<T>(string key, out T? value)
    {
        var result = cache.TryGetValue(key, out value);
        if(result)
            logger.LogInformation($"Cache \"{key}\" has been retrieved.");
        
        return result;
    }

    public void Put<T>(string key, T item, int absoluteExpirationMinutes, int slidingExpirationMinutes)
    {
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(absoluteExpirationMinutes))
            .SetSlidingExpiration(TimeSpan.FromMinutes(slidingExpirationMinutes));
        
        cache.Set(key, item, cacheOptions);
        logger.LogInformation($"Cache \"{key}\" added to cache.");
    }

    public void Remove(string key)
    {
        cache.Remove(key);
        logger.LogInformation($"Cache \"{key}\" removed from cache.");
    }
}