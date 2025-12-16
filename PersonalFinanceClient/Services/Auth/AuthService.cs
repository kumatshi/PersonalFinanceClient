using PersonalFinanceClient.Models.DTOs;
using PersonalFinanceClient.Services.Api;

namespace PersonalFinanceClient.Services.Auth
{
    public class AuthService
    {
        private readonly ApiService _apiService;

        public AuthService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(string usernameOrEmail, string password)
        {
            var request = new LoginRequestDto
            {
                UsernameOrEmail = usernameOrEmail,
                Password = password
            };

            var response = await _apiService.PostAsync<AuthResponseDto>("/api/auth/login", request);

            if (response.Success && response.Data != null)
            {
                _apiService.SetToken(response.Data.AccessToken);

                await SecureStorage.SetAsync("access_token", response.Data.AccessToken);
                await SecureStorage.SetAsync("refresh_token", response.Data.RefreshToken);
                await SecureStorage.SetAsync("user_id", response.Data.UserId.ToString());
            }

            return response;
        }

        public async Task<bool> TryAutoLoginAsync()
        {
            var token = await SecureStorage.GetAsync("access_token");

            if (!string.IsNullOrEmpty(token))
            {
                _apiService.SetToken(token);
                return true;
            }

            return false;
        } 
        public async Task LogoutAsync()
        {
            SecureStorage.RemoveAll();
            _apiService.SetToken(string.Empty);
        }
    }
}