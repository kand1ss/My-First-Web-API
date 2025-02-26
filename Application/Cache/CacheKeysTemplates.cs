namespace Application.Extra;

public static class CacheKeysTemplates
{
    public static string AccountKey(string login) => $"Account_{login}";
    public static string PermissionsKey(string key) => $"Permissions_{key}";
    public static string BookKey(string key) => $"Book_{key}";
    public static string AuthorKey(string key) => $"Author_{key}";
}