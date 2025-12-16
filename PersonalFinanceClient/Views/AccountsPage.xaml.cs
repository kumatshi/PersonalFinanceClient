// Views/AccountsPage.xaml.cs
using PersonalFinanceClient.ViewModels;

namespace PersonalFinanceClient.Views
{
    public partial class AccountsPage : ContentPage
    {
        public AccountsPage(AccountsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as AccountsViewModel)?.LoadAccountsCommand?.Execute(null);
        }
    }
}