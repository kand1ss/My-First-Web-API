using System.ComponentModel.DataAnnotations;
using Core.Contracts;
using Application.DTO;
using Application.Exceptions;
using Application.Extra;
using Core.Models;

namespace Application.Services;

public class AuthService(
    IRefreshTokenRepository tokenRepository, IAccountRepository accountRepository, 
    IPermissionRepository permissionRepository, JWTService jwtService, AccountValidator accountValidator,
    ICacheService cacheService) 
    : IAuthService
{
    private async Task<UserAccount> GetAccountByGuid(string guid)
    {
        var account = await accountRepository.GetByGuidAsync(guid);
        if (account == null)
            throw new AccountNotFoundException(guid);
        
        return account;
    }
    private async Task<UserAccount> GetAccountByLogin(string login)
    {
        var account = await accountRepository.GetByLoginAsync(login);
        if (account == null)
            throw new AccountNotFoundException(login);
        
        return account;
    }
    private bool TryGetAccountFromCache(string login, out UserAccount? account)
    {
        var cacheKey = CacheKeysTemplates.AccountKey(login);
        return cacheService.TryGet(cacheKey, out account);
    }
    
    
    private static UserAccount InitializeAccount(AccountRegisterDTO registerData, ICollection<Permission> permissions)
    {
        var account = AccountFactory.Create(
            registerData.Login,
            registerData.Email,
            registerData.FirstName,
            registerData.LastName
        );
        
        account.PasswordHash = PasswordService.HashPassword(registerData.Password, account);
        account.CreatedUtc = DateTime.UtcNow;
        account.ExternalId = Guid.NewGuid();
        
        account.UserPermissions = permissions.Select(p => new UserPermissions
        {
            Account = account,
            PermissionId = p.Id
        }).ToList();
        
        return account;
    }
    private async Task<ICollection<Permission>> GetDefaultPermissions()
    {
        var permissionsCacheKey = CacheKeysTemplates.PermissionsKey("DefaultPermissions");
        if (!cacheService.TryGet<ICollection<Permission>>(permissionsCacheKey, out var defaultPermissions))
        {
            defaultPermissions = await permissionRepository.GetDefaultUserPermissionsAsync();
            cacheService.Put(permissionsCacheKey, defaultPermissions, 20, 10);
        }

        return defaultPermissions;
    }
    public async Task RegisterAsync(AccountRegisterDTO registerData)
    {
        await accountValidator.Validate(registerData);

        var defaultPermissions = await GetDefaultPermissions();
        var account = InitializeAccount(registerData, defaultPermissions);

        await accountRepository.CreateAsync(account);

        var accountCacheKey = CacheKeysTemplates.AccountKey(account.Login);
        cacheService.Put(accountCacheKey, account, 20, 5);
    }


    private async Task<TokensDTO> GenerateTokensIfPasswordValid(string passwordToValid, UserAccount account)
    {
        if (PasswordService.ValidatePassword(passwordToValid, account))
            return await jwtService.GenerateTokens(account);
        
        throw new ValidationException($"Invalid password for account \"{account.Login}\"");
    }
    public async Task<TokensDTO> LoginAsync(AccountLoginDTO loginData)
    {
        await accountValidator.Validate(loginData);
        
        if(TryGetAccountFromCache(loginData.Login, out UserAccount? accountFromCache))
            return await GenerateTokensIfPasswordValid(loginData.Password, accountFromCache);

        var account = await GetAccountByLogin(loginData.Login);
        return await GenerateTokensIfPasswordValid(loginData.Password, account);
    }


    public async Task<TokensDTO> LoginWithTokenAsync(string refreshToken)
    {
        var token = await tokenRepository.GetRefreshTokenByToken(refreshToken);
        if(token is null)
            throw new ValidationException("Refresh token is not found");
        
        if (token.ExpiresAt < DateTime.UtcNow)
            throw new ValidationException("The refresh token is an expired token");
        
        var account = await accountRepository.GetByGuidAsync(token.UserId.ToString());
        if(account is null)
            throw new ValidationException("Account by token does not exist");
        
        return await jwtService.GenerateTokens(account);
    }
    
    
    private async Task ValidateAndSetData(UserAccount accountToUpdate, AccountUpdateDTO updateData)
    {
        var newLogin = updateData.Login;
        var newEmail = updateData.Email;
        var newPassword = updateData.Password;
        var newFirstName = updateData.FirstName;
        var newLastName = updateData.LastName;

        var oldEmail = accountToUpdate.Email;
        var oldLogin = accountToUpdate.Login;
        
        if (newEmail != null && oldEmail != newEmail)
        {
            await accountValidator.EnsureEmailDoesNotExist(newEmail);
            accountToUpdate.Email = newEmail;
        }
        if (newLogin != null && oldLogin != newLogin)
        {
            await accountValidator.EnsureLoginDoesNotExist(newLogin);
            accountToUpdate.Login = newLogin;
        }
        if (!string.IsNullOrEmpty(newPassword))
            accountToUpdate.PasswordHash = PasswordService.HashPassword(newPassword, accountToUpdate);
        
        if(!string.IsNullOrEmpty(newFirstName))
            accountToUpdate.FirstName = newFirstName;
        if(!string.IsNullOrEmpty(newLastName))
            accountToUpdate.LastName = newLastName;
        
        accountToUpdate.ModifiedUtc = DateTime.UtcNow;
    }
    public async Task UpdateAccountAsync(string guid, AccountUpdateDTO updateData)
    {
        var account = await GetAccountByGuid(guid);
        var cacheKey = CacheKeysTemplates.AccountKey(account.Login);
        var oldLogin = account.Login;
        
        await ValidateAndSetData(account, updateData);
        await accountRepository.UpdateAsync(account);
        
        if(updateData.Login != oldLogin)
            cacheService.Remove(cacheKey);
        
        var newCacheKey = CacheKeysTemplates.AccountKey(account.Login);
        cacheService.Put(newCacheKey, account, 20, 10);
    }

    
    public async Task DeleteAccountAsync(string guid)
    {
        var account = await GetAccountByGuid(guid);
        cacheService.Remove(CacheKeysTemplates.AccountKey(account.Login));

        await accountRepository.DeleteAsync(account);
    }

    
    public async Task<AccountDTO> GetAccountByGuidAsync(string guid)
        => (await GetAccountByGuid(guid)).ToDTO();

    
    public async Task<AccountDTO> GetAccountByLoginAsync(string login)
    {
        if (TryGetAccountFromCache(login, out var accountFromCache))
            return accountFromCache.ToDTO();

        var account = await GetAccountByLogin(login);
        
        return account.ToDTO();
    }

    
    public async Task<IList<AccountDTO>> GetAllAccountsAsync()
        => (await accountRepository.GetAllAsync()).ToDTOs().ToList();
}