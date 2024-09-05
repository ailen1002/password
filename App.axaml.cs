using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using NLog;
using password.Data;
using password.Services;
using password.ViewModels;
using password.Views;

namespace password;

public class App : Application
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ConfigureLogging();
            var context = new MainDbContext();
            if (!context.Database.CanConnect())
            {
                context.EnsureDatabaseCreated();
            }
            var accountService = new AccountService(context); 
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(accountService),
            };
            Logger.Info("Application started successfully.");
        }

        base.OnFrameworkInitializationCompleted();
    }
    public void ChangeThemeVariant(ThemeVariant themeVariant)
    {
        // 切换到传入的主题
        RequestedThemeVariant = themeVariant;
    }

    private static void ConfigureLogging()
    {
        // 直接加载 NLog 配置文件
        LogManager.Setup().LoadConfigurationFromFile("NLog.config");
    }
}