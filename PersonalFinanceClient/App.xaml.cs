using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceClient.Views;

namespace PersonalFinanceClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var loginPage = MauiProgram.CreateMauiApp().Services.GetRequiredService<LoginPage>();
            MainPage = new NavigationPage(loginPage);
        }
    }
}