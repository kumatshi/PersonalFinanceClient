// Services/TransactionsService.cs
using PersonalFinanceClient.Models.DTOs;
using PersonalFinanceClient.Services.Api;

namespace PersonalFinanceClient.Services
{
    public interface ITransactionsService
    {
        Task<List<TransactionDto>> GetTransactionsAsync(int page = 1, int pageSize = 10);
        Task<TransactionDto> GetTransactionAsync(int id);
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto transaction);
        Task<TransactionDto> UpdateTransactionAsync(int id, UpdateTransactionDto transaction);
        Task<bool> DeleteTransactionAsync(int id);
        Task<List<TransactionDto>> GetTransactionsByAccountAsync(int accountId, int page = 1, int pageSize = 10);
    }

    public class TransactionsService : ITransactionsService
    {
        private readonly IApiService _apiService;

        public TransactionsService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<TransactionDto>> GetTransactionsAsync(int page = 1, int pageSize = 10)
        {
            var response = await _apiService.GetAsync<List<TransactionDto>>($"/api/Transactions?page={page}&pageSize={pageSize}");
            return response.Success ? response.Data ?? new List<TransactionDto>() : new List<TransactionDto>();
        }

        public async Task<TransactionDto> GetTransactionAsync(int id)
        {
            var response = await _apiService.GetAsync<TransactionDto>($"/api/Transactions/{id}");
            return response.Data;
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto transaction)
        {
            var response = await _apiService.PostAsync<TransactionDto>("/api/Transactions", transaction);
            return response.Data;
        }

        public async Task<TransactionDto> UpdateTransactionAsync(int id, UpdateTransactionDto transaction)
        {
            var response = await _apiService.PutAsync<TransactionDto>($"/api/Transactions/{id}", transaction);
            return response.Data;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var response = await _apiService.DeleteAsync<bool>($"/api/Transactions/{id}");
            return response.Success;
        }

        public async Task<List<TransactionDto>> GetTransactionsByAccountAsync(int accountId, int page = 1, int pageSize = 10)
        {
            var response = await _apiService.GetAsync<List<TransactionDto>>($"/api/Transactions/account/{accountId}?page={page}&pageSize={pageSize}");
            return response.Success ? response.Data ?? new List<TransactionDto>() : new List<TransactionDto>();
        }
    }
}