using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using password.Models;
using password.Services;
using password.Views;
using ReactiveUI;

namespace password.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly AccountService _accountService;
        public ObservableCollection<AccountInfo> Accounts { get; set; }
        private AccountInfo _selectedAccount;
        public AccountInfo SelectedAccount
        {
            get => _selectedAccount;
            set => this.RaiseAndSetIfChanged(ref _selectedAccount, value);
        }
        public ReactiveCommand<Unit, Unit> ShowAddAccountWindowCommand { get; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    
        // 通过构造函数注入
        public MainWindowViewModel(AccountService accountService)
        {
            _accountService = accountService;
            Accounts = new ObservableCollection<AccountInfo>();
            
            ShowAddAccountWindowCommand = ReactiveCommand.Create(OpenAddAccountWindow);
            EditCommand = ReactiveCommand.Create(EditAccount,this.WhenAnyValue(x => x.SelectedAccount).Select(account => true));
            DeleteCommand = ReactiveCommand.Create(DeleteAccount,this.WhenAnyValue(x => x.SelectedAccount).Select(account => true));
    
            LoadAccounts();
        }
    
        private void LoadAccounts()
        {
            var accounts = _accountService.LoadAccounts();
            Accounts.Clear();
            foreach (var account in accounts)
            {
                Accounts.Add(account);
            }
        }
    
        private void OpenAddAccountWindow()
        {
            var addAccountWindow = new AddAccount
            {
                DataContext = new AddAccountViewModel(_accountService)
            };
    
            // 显示添加账户窗口
            addAccountWindow.Show();
    
            // 当窗口关闭时，重新加载账户
            addAccountWindow.Closed += (sender, e) => LoadAccounts();
        }
    
        private void EditAccount()
        {
            // Todo 实现编辑逻辑
        }
    
        private void DeleteAccount()
        {
            _accountService.DeleteAccount(SelectedAccount.Id);
            LoadAccounts(); // 刷新账户列表
        }
    }
}