namespace Application.Extra;

public interface ICacheService
{
    bool TryGet<T>(string key, out T? value);
    void Put<T>(string key, T item, int absoluteExpirationMinutes, int slidingExpirationMinutes);
    void Remove(string key);
}