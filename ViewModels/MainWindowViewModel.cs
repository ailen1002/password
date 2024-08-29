using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using password.Models;
using password.Views;
using ReactiveUI;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using password.Interfaces;

namespace password.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;
        private AccountInfo _selectedAccount;

        public ObservableCollection<AccountInfo> Accounts { get; set; }

        public AccountInfo SelectedAccount
        {
            get => _selectedAccount;
            set => this.RaiseAndSetIfChanged(ref _selectedAccount, value);
        }

        public ReactiveCommand<Unit, Unit> ShowAddAccountWindowCommand { get; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> ReloadCommand { get; }

        public MainWindowViewModel(IAccountService accountService)
        {
            _accountService = accountService;
            Accounts = new ObservableCollection<AccountInfo>();

            ShowAddAccountWindowCommand = ReactiveCommand.Create(OpenAddAccountWindow);
            EditCommand = ReactiveCommand.Create(OpenEditAccountWindow, this.WhenAnyValue(x => x.SelectedAccount).Select(account => account != null));
            DeleteCommand = ReactiveCommand.Create(DeleteAccount, this.WhenAnyValue(x => x.SelectedAccount).Select(account => account != null));
            ReloadCommand = ReactiveCommand.Create(Reload);
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

            addAccountWindow.Show();
            addAccountWindow.Closed += (sender, e) => LoadAccounts();
        }

        private void OpenEditAccountWindow()
        {
            var editAccountWindow = new EditAccount
            {
                DataContext = new EditAccountViewModel(_accountService, SelectedAccount)
            };

            editAccountWindow.Show();
            editAccountWindow.Closed += (sender, e) => LoadAccounts();
        }

        private async void DeleteAccount()
        {
            var result = await MessageBoxManager
                .GetMessageBoxStandard("删除账户", "确定删除此账户吗?", ButtonEnum.YesNo, Icon.Warning)
                .ShowAsync();

            if (result != ButtonResult.Yes) return;
            _accountService.DeleteAccount(SelectedAccount.Id);
            LoadAccounts();
        }
        
        private void Reload()
        {
            _accountService.LoadAccounts();
        }
    }
}
