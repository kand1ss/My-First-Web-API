using Core.Models;
using Application.DTO;

namespace Application.Extra;

public static class AccountMapper
{
    public static AccountDTO ToDTO(this UserAccount account)
        => new(account.Login, account.PasswordHash, account.Email, account.FirstName, account.LastName);

    public static IEnumerable<AccountDTO> ToDTOs(this IList<UserAccount> accounts)
        => accounts.Select(ToDTO);
}