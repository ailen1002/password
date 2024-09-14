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
                context.EnsureDatabaseCreated();
            }
            var accountService = new AccountService(context);
            var userService = new UserService(context);
            desktop.MainWindow = new Login
            {
                DataContext = new LoginViewModel(userService),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    public void ChangeThemeVariant(ThemeVariant themeVariant)
    {
        RequestedThemeVariant = themeVariant;
    }
}