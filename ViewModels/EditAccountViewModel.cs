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
    public class EditAccountViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;
        public AccountInfo AccountInfo { get; set; }
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public EditAccountViewModel(IAccountService accountService, AccountInfo account)
        {
            _accountService = accountService;
            AccountInfo = account;
            // 更新账户命令
            UpdateCommand = ReactiveCommand.Create(UpdateChanges);
            // 取消命令，关闭窗口
            CancelCommand = ReactiveCommand.Create(CloseWindow);
        }

        private void UpdateChanges()
        {
            _accountService.UpdateAccount(AccountInfo);
            
            // 关闭页面
            CloseWindow();
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