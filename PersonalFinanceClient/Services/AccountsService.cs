// Services/AccountsService.cs
using PersonalFinanceClient.Models.DTOs;
using PersonalFinanceClient.Services.Api;

namespace PersonalFinanceClient.Services
{
    public interface IAccountsService
    {
        Task<List<AccountDto>> GetAccountsAsync();
        Task<AccountDto> GetAccountAsync(int id);
        Task<AccountDto> CreateAccountAsync(CreateAccountDto account);
        Task<AccountDto> UpdateAccountAsync(int id, UpdateAccountDto account);
        Task<bool> DeleteAccountAsync(int id);
    }

    public class AccountsService : IAccountsService
    {
        private readonly IApiService _apiService;

        public AccountsService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<AccountDto>> GetAccountsAsync()
        {
            var response = await _apiService.GetAsync<List<AccountDto>>("/api/Accounts");
            return response.Success ? response.Data ?? new List<AccountDto>() : new List<AccountDto>();
        }

        public async Task<AccountDto> GetAccountAsync(int id)
        {
            var response = await _apiService.GetAsync<AccountDto>($"/api/Accounts/{id}");
            return response.Data;
        }

        public async Task<AccountDto> CreateAccountAsync(CreateAccountDto account)
        {
            var response = await _apiService.PostAsync<AccountDto>("/api/Accounts", account);
            return response.Data;
        }

        public async Task<AccountDto> UpdateAccountAsync(int id, UpdateAccountDto account)
        {
            var response = await _apiService.PutAsync<AccountDto>($"/api/Accounts/{id}", account);
            return response.Data;
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            var response = await _apiService.DeleteAsync<bool>($"/api/Accounts/{id}");
            return response.Success;
        }
    }
}