using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace password.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void OnThemeToggle(object? sender, RoutedEventArgs e)
    {
        if (((ToggleButton)sender).IsChecked == true)
        {
            // 切换到 Dark 主题
            ((App)Application.Current).ChangeThemeVariant(ThemeVariant.Dark);
        }
        else
        {
            // 切换到 Light 主题
            ((App)Application.Current).ChangeThemeVariant(ThemeVariant.Light);
        }
    }
}