using System;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using password.Interfaces;
using password.Models;

namespace password.ViewModels
{
    public class AddAccountViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;

        public string? AccountName { get; set; }
        public string? Account { get; set; }
        public string? Password { get; set; }

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public AddAccountViewModel(IAccountService accountService)
        {
            _accountService = accountService;

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
    }
}
