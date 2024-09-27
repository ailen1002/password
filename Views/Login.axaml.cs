using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using password.ViewModels;

namespace password.Views;

public partial class Login : Window
{
    public Login()
    {
        InitializeComponent();
    }
    // 监听键盘事件
    private void PasswordBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            (DataContext as LoginViewModel)?.LoginCommand.Execute().Subscribe();
        }
    }
}