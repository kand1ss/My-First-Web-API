using System.ComponentModel.DataAnnotations;
using Core.Contracts;
using Application.DTO;
using Application.Exceptions;
using Application.Extra;
using Core.Models;

namespace Application.Services;

public class AuthService(
    IAccountRepository accountRepository, JWTService jwtService, AccountValidator accountValidator) 
    : IAuthService
{
    public async Task RegisterAsync(RegisterDTO registerData)
    {
        await accountValidator.Validate(registerData);

        var account = AccountFactory.Create(
            registerData.Login,
            registerData.Email,
            registerData.FirstName,
            registerData.LastName
        );
        
        account.PasswordHash = PasswordService.HashPassword(registerData.Password, account);
        account.CreatedUtc = DateTime.UtcNow;
        account.ExternalId = Guid.NewGuid();
        
        await accountRepository.CreateAsync(account);
    }

    public async Task<string> LoginAsync(LoginDTO loginData)
    {
        await accountValidator.Validate(loginData);

        var account = await accountRepository.GetByLoginAsync(loginData.Login);
        if(PasswordService.ValidatePassword(loginData.Password, account!))
            return jwtService.GenerateToken(account!);
            
        throw new ValidationException("Invalid password");
    }

    public async Task UpdateAccountAsync(string guid, UpdateDTO updateData)
    {
        var account = await accountRepository.GetByGuidAsync(guid);
        if (account == null)
            throw new AccountNotFoundException(guid);
        
        var login = updateData.Login;
        var email = updateData.Email;
        var password = updateData.Password;
        var firstName = updateData.FirstName;
        var lastName = updateData.LastName;
        
        await ValidateAndSetLogin(account, login);
        await ValidateAndSetEmail(account, email);
        
        if(!string.IsNullOrEmpty(password) && !PasswordService.ValidatePassword(password, account))
            account.PasswordHash = PasswordService.HashPassword(password, account);
        
        if(!string.IsNullOrEmpty(firstName))
            account.FirstName = firstName;
        if(!string.IsNullOrEmpty(lastName))
            account.LastName = lastName;
        
        account.ModifiedUtc = DateTime.UtcNow;

        await accountRepository.UpdateAsync(account);
    }

    private async Task ValidateAndSetEmail(UserAccount account, string? email)
    {
        if (account.Email != email && email != null)
        {
            await accountValidator.EnsureEmailDoesNotExist(email);
            account.Email = email;
        }
    }
    private async Task ValidateAndSetLogin(UserAccount account, string? login)
    {
        if (account.Login != login && login != null)
        {
            await accountValidator.EnsureLoginDoesNotExist(login);
            account.Login = login;
        }
    }

    
    public async Task DeleteAccountAsync(string guid)
    {
        var account = await accountRepository.GetByGuidAsync(guid);
        if(account == null)
            throw new AccountNotFoundException(guid);
        
        await accountRepository.DeleteAsync(account);
    }

    public async Task<AccountDTO> GetAccountByGuidAsync(string guid)
    {
        var account = await accountRepository.GetByGuidAsync(guid);
        if(account == null)
            throw new AccountNotFoundException(guid);

        return account.ToDTO();
    }

    public async Task<AccountDTO> GetAccountByLoginAsync(string login)
    {
        var account = await accountRepository.GetByLoginAsync(login);
        if (account == null)
            throw new AccountNotFoundException(login);
        
        return account.ToDTO();
    }

    public async Task<IList<AccountDTO>> GetAllAccountsAsync()
        => (await accountRepository.GetAllAsync()).ToDTOs().ToList();
}