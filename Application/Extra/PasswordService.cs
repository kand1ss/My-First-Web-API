using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Extra;

public static class PasswordService
{
    public static string HashPassword(string password, UserAccount userAccount)
    {
        var hashedPassword = new PasswordHasher<UserAccount>().HashPassword(userAccount, password);
        return hashedPassword;
    }

    public static bool ValidatePassword(string password, UserAccount userAccount)
    {
        var result = new PasswordHasher<UserAccount>().VerifyHashedPassword(userAccount, userAccount.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }
}