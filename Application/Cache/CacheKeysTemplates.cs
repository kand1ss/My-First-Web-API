namespace Application.Extra;

public static class CacheKeysTemplates
{
    public static string AccountKey(string login) => $"Account_{login}";
    public static string PermissionsKey(string key) => $"Permissions_{key}";
}