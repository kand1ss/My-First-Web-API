using Application.DTO;

namespace Application.Services;

public interface IAuthService
{
    public Task RegisterAsync(RegisterDTO registerData);
    public Task<string> LoginAsync(LoginDTO loginData);
    public Task UpdateAccountAsync(string guid, UpdateDTO updateData);
    public Task DeleteAccountAsync(string guid);
    
    public Task<AccountDTO> GetAccountByGuidAsync(string guid);
    public Task<AccountDTO> GetAccountByLoginAsync(string login);
    public Task<IList<AccountDTO>> GetAllAccountsAsync();
}