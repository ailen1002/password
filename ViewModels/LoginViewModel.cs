using System;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using password.Interfaces;
using password.Views;
using ReactiveUI;

namespace password.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private string _userName = string.Empty;
        private string _passWord = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _hasError;

        public string UserName
        {
            get => _userName;
            set => this.RaiseAndSetIfChanged(ref _userName, value);
        }

        public string PassWord
        {
            get => _passWord;
            set => this.RaiseAndSetIfChanged(ref _passWord, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        // HasError Property
        public bool HasError
        {
            get => _hasError;
            set => this.RaiseAndSetIfChanged(ref _hasError, value);
        }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenRegisterCommand { get; }

        public LoginViewModel(IUserService userService, IAccountService accountService)
        {
            _userService = userService;
            _accountService = accountService;
            ErrorMessage = "请输入正确信息";
            LoginCommand = ReactiveCommand.Create(Login);
            CancelCommand = ReactiveCommand.Create(Cancel);
            OpenRegisterCommand = ReactiveCommand.Create(Register);
        }
        private async void Login()
        {
            var user = await _userService.GetUserByUserNameAsync(UserName);
            if (user == null)
            {
                await MessageBoxManager.GetMessageBoxStandard("错误", "用户名错误", ButtonEnum.Ok, Icon.Error).ShowAsync();
                return;
            }
            if (user.PasswordHash != PassWord)
            {
                await MessageBoxManager.GetMessageBoxStandard("错误", "登录密码错误", ButtonEnum.Ok, Icon.Error).ShowAsync();
            }
            else
            {
                var mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(_accountService)
                };
                mainWindow.Show();
                
                if (Application.Current != null)
                    (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow?.Close();
            }
        }
        private static void Cancel()
        {
            if (Application.Current == null) return;
            var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            mainWindow?.Close();
        }

        private void Register()
        {
            var registerWindow = new Register()
            {
                DataContext = new RegisterViewModel(_userService)
            };
            registerWindow.Show();
        }
    }
}
