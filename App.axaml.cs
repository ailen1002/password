using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using password.Data;
using password.Services;
using password.ViewModels;
using password.Views;

namespace password;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var context = new MainDbContext();
            if (!context.Database.CanConnect())
            {
                LogService.Error("Database connection failed.");
                context.EnsureDatabaseCreated();
            }
            var accountService = new AccountService(context);
            var userService = new UserService(context);
            // 检查是否已登录
            if (userService.IsLoggedIn())
            {
                // 如果用户已登录，打开主窗口
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(accountService),
                };
            }
            else
            {
                // 用户未登录，显示登录窗口
                desktop.MainWindow = new Login
                {
                    DataContext = new LoginViewModel(userService, accountService),
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
    public void ChangeThemeVariant(ThemeVariant themeVariant)
    {
        RequestedThemeVariant = themeVariant;
    }
}