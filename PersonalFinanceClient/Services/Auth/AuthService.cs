// Services/Auth/AuthService.cs
using PersonalFinanceClient.Models.DTOs;
using PersonalFinanceClient.Services.Api;

namespace PersonalFinanceClient.Services.Auth
{
    public interface IAuthService
    {
        bool IsAuthenticated { get; }
        string CurrentUsername { get; }
        string UserRole { get; }
        Task<ApiResponse<AuthResponseDto>> LoginAsync(string usernameOrEmail, string password);
        Task<ApiResponse<UserProfileDto>> GetProfileAsync();
        Task<bool> TryAutoLoginAsync();
        Task LogoutAsync();
        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private AuthResponseDto _currentAuth;

        public bool IsAuthenticated => _currentAuth != null && !string.IsNullOrEmpty(_currentAuth.AccessToken);
        public string CurrentUsername => _currentAuth?.Username ?? string.Empty;
        public string UserRole => _currentAuth?.Role ?? "User";

        public AuthService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task InitializeAsync()
        {
            var token = await SecureStorage.GetAsync("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                _apiService.SetToken(token);

                var authJson = await SecureStorage.GetAsync("auth_data");
                if (!string.IsNullOrEmpty(authJson))
                {
                    _currentAuth = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthResponseDto>(authJson);
                }
            }
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
                await SaveAuthData(response.Data);
            }

            return response;
        }

        public async Task<ApiResponse<UserProfileDto>> GetProfileAsync()
        {
            return await _apiService.GetAsync<UserProfileDto>("/api/auth/profile");
        }

        public async Task<bool> TryAutoLoginAsync()
        {
            await InitializeAsync();
            return IsAuthenticated;
        }

        public async Task LogoutAsync()
        {
            if (IsAuthenticated)
            {
                await _apiService.PostAsync<bool>("/api/auth/logout", new { });
            }

            await ClearAuthData();
        }

        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync()
        {
            var refreshToken = await SecureStorage.GetAsync("refresh_token");
            if (string.IsNullOrEmpty(refreshToken))
            {
                return new ApiResponse<AuthResponseDto> { Success = false, Message = "Refresh token отсутствует" };
            }

            var request = new RefreshTokenRequestDto { RefreshToken = refreshToken };
            var response = await _apiService.PostAsync<AuthResponseDto>("/api/auth/refresh", request);

            if (response.Success && response.Data != null)
            {
                await SaveAuthData(response.Data);
            }

            return response;
        }

        private async Task SaveAuthData(AuthResponseDto authData)
        {
            _currentAuth = authData;
            _apiService.SetToken(authData.AccessToken);

            await SecureStorage.SetAsync("access_token", authData.AccessToken);
            await SecureStorage.SetAsync("refresh_token", authData.RefreshToken);
            await SecureStorage.SetAsync("auth_data", Newtonsoft.Json.JsonConvert.SerializeObject(authData));
        }

        private async Task ClearAuthData()
        {
            _currentAuth = null;
            _apiService.ClearToken();

            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
            SecureStorage.Remove("auth_data");
            await Task.CompletedTask;
        }
    }
}