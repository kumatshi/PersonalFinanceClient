using System.Text;
using Newtonsoft.Json;
using PersonalFinanceClient.Models.DTOs;

namespace PersonalFinanceClient.Services.Api
{
    public interface IApiService
    {
        Task<bool> TestConnectionAsync();
        Task<ApiResponse<T>> GetAsync<T>(string endpoint);
        Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data);
        Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data);
        Task<ApiResponse<T>> DeleteAsync<T>(string endpoint);
        void SetToken(string token);
        void ClearToken();
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:7068";

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public void ClearToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/swagger");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            return await SendRequestAsync<T>(HttpMethod.Get, endpoint);
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
        {
            return await SendRequestAsync<T>(HttpMethod.Post, endpoint, data);
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
        {
            return await SendRequestAsync<T>(HttpMethod.Put, endpoint, data);
        }

        public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
        {
            return await SendRequestAsync<T>(HttpMethod.Delete, endpoint);
        }

        private async Task<ApiResponse<T>> SendRequestAsync<T>(HttpMethod method, string endpoint, object data = null)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(method, endpoint);

                if (data != null)
                {
                    var json = JsonConvert.SerializeObject(data);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(content)
                           ?? new ApiResponse<T> { Success = false, Message = "Ошибка десериализации" };
                }

                // Пробуем десериализовать как ErrorResponse
                try
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(content);
                    return new ApiResponse<T>
                    {
                        Success = false,
                        Message = error?.Message ?? $"Ошибка {response.StatusCode}"
                    };
                }
                catch
                {
                    return new ApiResponse<T>
                    {
                        Success = false,
                        Message = $"Ошибка {response.StatusCode}: {content}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = $"Сетевая ошибка: {ex.Message}"
                };
            }
        }
    }
}