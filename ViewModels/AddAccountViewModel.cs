using System;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using password.Interfaces;
using password.Models;
using password.Services;

namespace password.ViewModels
{
    public class AddAccountViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;
        private readonly LocalizationService _localizationService;
        private string _accountNameLabel;
        private string _accountLabel;
        private string _passwordLabel;
        private string _confirm;
        private string _cancel;
        public string? AccountName { get; set; }
        public string? Account { get; set; }
        public string? Password { get; set; }

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public string AccountNameLabel
        {
            get => _accountNameLabel;
            set => this.RaiseAndSetIfChanged(ref _accountNameLabel, value);
        }
        public string AccountLabel
        {
            get => _accountLabel;
            set => this.RaiseAndSetIfChanged(ref _accountLabel, value);
        }
        public string PasswordLabel
        {
            get => _passwordLabel;
            set => this.RaiseAndSetIfChanged(ref _passwordLabel, value);
        }
        public string Confirm
        {
            get => _confirm;
            set => this.RaiseAndSetIfChanged(ref _confirm, value);
        }
        public string Cancel
        {
            get => _cancel;
            set => this.RaiseAndSetIfChanged(ref _cancel, value);
        }
        public AddAccountViewModel(IAccountService accountService,LocalizationService localizationService)
        {
            _accountService = accountService;
            _localizationService = localizationService;
            // 订阅语言切换事件
            _localizationService.LanguageChanged += UpdateLocalizedTexts;
            AccountNameLabel = _localizationService.GetString("AccountNameLabel");
            AccountLabel = _localizationService.GetString("AccountLabel");
            PasswordLabel = _localizationService.GetString("PasswordLabel");
            Confirm = _localizationService.GetString("Confirm");
            Cancel = _localizationService.GetString("Cancel");
            
            // 添加账户命令
            AddCommand = ReactiveCommand.Create(AddAccount);
            // 取消命令，关闭窗口
            CancelCommand = ReactiveCommand.Create(CloseWindow);
        }

        private void AddAccount()
        {
            if (!string.IsNullOrEmpty(AccountName) && !string.IsNullOrEmpty(Account) && !string.IsNullOrEmpty(Password))
            {
                // 创建一个新的 AccountInfo 实例
                var newAccount = new AccountInfo
                {
                    AccountName = AccountName,
                    Account = Account,
                    Password = Password
                };

                // 将新账户添加到数据库
                _accountService.AddAccount(newAccount);

                // 关闭添加窗口
                CloseWindow();
            }
            else
            {
                // Todo 弹窗提示输入正确的信息
            }
        }

        private void CloseWindow()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            
            var window = desktop.Windows.FirstOrDefault(w => w.DataContext == this);

            if (window == null) return;
            try
            {
                window.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to close window: {ex.Message}");
            }
        }
        private void UpdateLocalizedTexts()
        {
            AccountNameLabel = _localizationService.GetString("AccountNameLabel");
            AccountLabel = _localizationService.GetString("AccountLabel");
            PasswordLabel = _localizationService.GetString("PasswordLabel");
            Confirm = _localizationService.GetString("Confirm");
            Cancel = _localizationService.GetString("Cancel");
        }
    }
}
