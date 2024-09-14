using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using password.Interfaces;
using password.Models;
using password.Services;
using ReactiveUI;

namespace password.ViewModels;


public class RegisterViewModel : ReactiveObject
{
    private readonly IUserService _userService;
    private string _userName;
    private string _password;
    private string _confirmPassword;
    private string _errorMessage;
    private bool _hasError;

    public RegisterViewModel(IUserService userService)
    {
        _userService = userService;
        RegisterCommand = ReactiveCommand.Create(Register);
        CancelCommand = ReactiveCommand.Create(Cancel);
    }

    public string UserName
    {
        get => _userName;
        set => this.RaiseAndSetIfChanged(ref _userName, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public bool HasError
    {
        get => _hasError;
        set => this.RaiseAndSetIfChanged(ref _hasError, value);
    }

    public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    private void Register()
    {
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Username and Password are required.";
            HasError = true;
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            HasError = true;
            return;
        }
        HasError = false;
        ErrorMessage = string.Empty;
        var user = new User
        {
            UserName = UserName,
            PasswordHash = Password
        };
        _userService.Register(user);
        
        Cancel();
    }

    private void Cancel()
    {
        // 获取当前窗口并关闭
        if (Application.Current == null) return;
        var currentWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Windows
            .FirstOrDefault(w => w.DataContext == this);

        currentWindow?.Close();
    }
}