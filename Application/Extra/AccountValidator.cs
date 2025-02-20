using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Application.DTO;
using Core.Contracts;

namespace Application.Extra;

public class AccountValidator(IAccountRepository accountRepository)
{
    public async Task EnsureLoginDoesNotExist(string login, CancellationToken cancellationToken = default)
    {
        var accountByLogin = await accountRepository.GetByLoginAsync(login, cancellationToken);
        if(accountByLogin != null)
            throw new ValidationException("An account with this login already exists");
    }

    public async Task EnsureEmailDoesNotExist(string email, CancellationToken cancellationToken = default)
    {
        var accountByEmail = await accountRepository.GetByEmailAsync(email, cancellationToken);
        if(accountByEmail != null)
            throw new ValidationException("Email already exists");
    }
    
    public async Task Validate(RegisterDTO registerData, CancellationToken cancellationToken = default)
    {
        var login = registerData.Login;
        var email = registerData.Email;
        
        await EnsureLoginDoesNotExist(login, cancellationToken);
        await EnsureEmailDoesNotExist(email, cancellationToken);
    }

    public async Task Validate(LoginDTO accountDto, CancellationToken cancellationToken = default)
    {
        var accountByLogin = await accountRepository.GetByLoginAsync(accountDto.Login, cancellationToken);
        if (accountByLogin == null)
            throw new ValidationException("Invalid login");
    }
}