using System;
using System.Reactive;
using password.Models;
using password.Services;
using password.Views;
using ReactiveUI;

namespace password.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private User? _user;
        private string _errorMessage;
        private bool _hasError;
        // Property for binding
        public User? User
        {
            get => _user;
            set => this.RaiseAndSetIfChanged(ref _user, value);
        }
        // ErrorMessage Property
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

        public LoginViewModel()
        {
            User = new User();
            ErrorMessage = "请输入正确信息";
            LoginCommand = ReactiveCommand.Create(Login);
            CancelCommand = ReactiveCommand.Create(Cancel);
            OpenRegisterCommand = ReactiveCommand.Create(Register);
        }
        private void Login()
        {
            if (User != null && (string.IsNullOrWhiteSpace(User.UserName) || string.IsNullOrWhiteSpace(User.PasswordHash)))
            {
                ErrorMessage = "Username or Password cannot be empty!";
                HasError = true;
            }
            else
            {
                ErrorMessage = string.Empty;
                HasError = false;
            }
        }
        private void Cancel()
        {
            // Todo Cancel 功能
        }

        private static void Register()
        {
            var registerWindow = new Register()
            {
                DataContext = new RegisterViewModel()
            };
            registerWindow.Show();
        }
    }
}
