using Application.DTO;

namespace Application.Services;

public interface IAuthService
{
    public Task RegisterAsync(AccountRegisterDTO registerData);
    public Task<TokensDTO> LoginAsync(AccountLoginDTO accountLoginData);
    public Task<TokensDTO> LoginAsync(string refreshToken);
    public Task UpdateAccountAsync(string guid, AccountUpdateDTO updateData);
    public Task DeleteAccountAsync(string guid);
    
    public Task<AccountDTO> GetAccountByGuidAsync(string guid);
    public Task<AccountDTO> GetAccountByLoginAsync(string login);
    public Task<IList<AccountDTO>> GetAllAccountsAsync();
}