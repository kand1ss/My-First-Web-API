using Core.Models;

namespace Application.Extra;

public static class AccountFactory
{
    public static UserAccount Create(
        string login, string email, string? firstName, string? lastName)
        => new()
        {
            Login = login,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
        };
}