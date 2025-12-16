using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceClient.Models.DTOs;
using PersonalFinanceClient.Services;
using PersonalFinanceClient.Services.Auth;

namespace PersonalFinanceClient.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IAccountsService _accountsService;
        private readonly ITransactionsService _transactionsService;

        [ObservableProperty]
        private UserProfileDto _userProfile;

        [ObservableProperty]
        private List<AccountDto> _accounts = new();

        [ObservableProperty]
        private List<TransactionDto> _recentTransactions = new();

        [ObservableProperty]
        private decimal _totalBalance;

        [ObservableProperty]
        private bool _isLoading;

        public MainViewModel(
            IAuthService authService,
            IAccountsService accountsService,
            ITransactionsService transactionsService)
        {
            _authService = authService;
            _accountsService = accountsService;
            _transactionsService = transactionsService;
        }

        [RelayCommand]
        private async Task LoadData()
        {
            if (IsLoading) return;

            IsLoading = true;

            try
            {
                // Загружаем профиль
                var profileResponse = await _authService.GetProfileAsync();
                if (profileResponse.Success)
                {
                    UserProfile = profileResponse.Data;
                }

                // Загружаем счета
                Accounts = await _accountsService.GetAccountsAsync();
                TotalBalance = Accounts.Sum(a => a.Balance);

                // Загружаем последние транзакции
                RecentTransactions = await _transactionsService.GetTransactionsAsync(1, 5);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось загрузить данные: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Выход",
                "Вы уверены, что хотите выйти?",
                "Да",
                "Нет");

            if (confirm)
            {
                await _authService.LogoutAsync();
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        [RelayCommand]
        private async Task GoToAccounts()
        {
            await Shell.Current.GoToAsync("AccountsPage");
        }

        [RelayCommand]
        private async Task GoToTransactions()
        {
            await Shell.Current.GoToAsync("TransactionsPage");
        }

        [RelayCommand]
        private async Task GoToAddTransaction()
        {
            await Shell.Current.DisplayAlert("Инфо", "Добавление транзакции", "OK");
        }
    }
}