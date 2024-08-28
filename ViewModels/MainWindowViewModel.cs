using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using password.Models;
using password.Services;
using password.Views;
using ReactiveUI;

namespace password.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly AccountService _accountService;
    public ObservableCollection<AccountInfo> Accounts { get; set; }

    public ReactiveCommand<Unit, Unit> ShowAddAccountWindowCommand { get; }
    public ReactiveCommand<AccountInfo, Unit> EditCommand { get; }
    public ReactiveCommand<AccountInfo, Unit> DeleteCommand { get; }

    // 通过构造函数注入
    public MainWindowViewModel(AccountService accountService)
    {
        _accountService = accountService;
        Accounts = new ObservableCollection<AccountInfo>();
        
        ShowAddAccountWindowCommand = ReactiveCommand.Create(OpenAddAccountWindow);
        EditCommand = ReactiveCommand.Create<AccountInfo>(EditAccount);
        DeleteCommand = ReactiveCommand.Create<AccountInfo>(DeleteAccount);

        LoadAccounts();
    }

    private void LoadAccounts()
    {
        var accounts = _accountService.LoadAccounts();
        Accounts.Clear();
        foreach (var account in accounts)
        {
            Accounts.Add(account);
            Console.WriteLine($"ID: {account.Id}, AccountName: {account.AccountName}, Account: {account.Account}, Password: {account.Password}, CreationDate: {account.CreationDate}");
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

    private void EditAccount(AccountInfo account)
    {
        // 实现编辑逻辑
    }

    private void DeleteAccount(AccountInfo account)
    {
        _accountService.DeleteAccount(account.Id);
        LoadAccounts(); // 刷新账户列表
    }
}