using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceClient.Models.DTOs;
using PersonalFinanceClient.Services;

namespace PersonalFinanceClient.ViewModels
{
    public partial class AccountsViewModel : ObservableObject
    {
        private readonly IAccountsService _accountsService;

        [ObservableProperty]
        private List<AccountDto> _accounts = new();

        [ObservableProperty]
        private bool _isLoading;

        public AccountsViewModel(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        [RelayCommand]
        private async Task LoadAccounts()
        {
            IsLoading = true;

            try
            {
                Accounts = await _accountsService.GetAccountsAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось загрузить счета: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task AddAccount()
        {
            await Shell.Current.DisplayAlert("Инфо", "Добавление счета", "OK");
        }

        [RelayCommand]
        private async Task EditAccount(AccountDto account)
        {
            if (account != null)
            {
                await Shell.Current.DisplayAlert("Редактирование", $"Редактируем счет: {account.Name}", "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteAccount(AccountDto account)
        {
            if (account != null)
            {
                bool confirm = await Shell.Current.DisplayAlert(
                    "Удаление",
                    $"Удалить счет {account.Name}?",
                    "Да",
                    "Нет");

                if (confirm)
                {
                    var success = await _accountsService.DeleteAccountAsync(account.Id);
                    if (success)
                    {
                        await LoadAccounts();
                    }
                }
            }
        }
    }
}