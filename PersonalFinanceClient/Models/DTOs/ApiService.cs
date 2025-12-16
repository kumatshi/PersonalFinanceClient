using System.Text;
using Newtonsoft.Json;
using PersonalFinanceClient.Models.DTOs;

namespace PersonalFinanceClient.Services.Api
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _baseUrl = "http://localhost:7068"; 
        private string _accessToken = string.Empty;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public void SetToken(string token)
        {
            _accessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
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
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(content)
                           ?? new ApiResponse<T> { Success = false, Message = "Ошибка десериализации" };
                }

                return new ApiResponse<T>
                {
                    Success = false,
                    Message = $"Ошибка {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = $"Исключение: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent)
                           ?? new ApiResponse<T> { Success = false, Message = "Ошибка десериализации" };
                }

                return new ApiResponse<T>
                {
                    Success = false,
                    Message = $"Ошибка {response.StatusCode}: {responseContent}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = $"Исключение: {ex.Message}"
                };
            }
        }
    }
}