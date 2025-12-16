using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceClient.Services.Auth;

namespace PersonalFinanceClient.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string _usernameOrEmail = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isBusy = false;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (IsBusy) return;

            // Валидация
            if (string.IsNullOrWhiteSpace(UsernameOrEmail))
            {
                ErrorMessage = "Введите email или логин";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите пароль";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var response = await _authService.LoginAsync(UsernameOrEmail, Password);

                if (response.Success)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Успех!",
                        $"Добро пожаловать, {response.Data?.Username}!",
                        "OK");

                }
                else
                {
                    ErrorMessage = response.Message ?? "Ошибка входа";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}