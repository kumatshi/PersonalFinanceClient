using Microsoft.Extensions.Logging;
using PersonalFinanceClient.Services;
using PersonalFinanceClient.Services.Api;
using PersonalFinanceClient.Services.Auth;
using PersonalFinanceClient.ViewModels;
using PersonalFinanceClient.Views;

namespace PersonalFinanceClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Регистрация сервисов
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IAccountsService, AccountsService>();
            builder.Services.AddSingleton<ITransactionsService, TransactionsService>();
            builder.Services.AddTransient<AccountsViewModel>();

            // Регистрация ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MainViewModel>();

            // Регистрация страниц
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}