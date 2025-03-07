using Core.Models;
using Application.DTO;

namespace Application.Extra;

public static class AccountMapper
{
    public static AccountDTO ToDTO(this UserAccount account)
        => new(account.Login, account.Email, account.FirstName, account.LastName);

    public static IEnumerable<AccountDTO> ToDTOs(this IEnumerable<UserAccount> accounts)
        => accounts.Select(ToDTO);
}