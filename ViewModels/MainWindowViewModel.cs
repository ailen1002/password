using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using password.Models;
using password.Views;
using ReactiveUI;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using password.Interfaces;
using password.Services;

namespace password.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;
        private readonly LocalizationService _localizationService;
        private AccountInfo _selectedAccount;
        private bool _isDarkMode;
        private IBrush _panelBackground;
        private string _edit;
        private string _delete;
        private string _add;
        private string _languageButtonText;
        public ObservableCollection<AccountInfo> Accounts { get; set; }
        public AccountInfo SelectedAccount
        {
            get => _selectedAccount;
            set => this.RaiseAndSetIfChanged(ref _selectedAccount, value);
        }
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _isDarkMode, value);
                ChangeTheme();
            }
        }
        private void ChangeTheme()
        {
            var app = (App)Application.Current;
            if (IsDarkMode)
            {
                // Dark 主题背景颜色
                app.ChangeThemeVariant(ThemeVariant.Dark);
                PanelBackground = Brushes.DarkMagenta;
            }
            else
            {
                // Light 主题背景颜色
                app.ChangeThemeVariant(ThemeVariant.Light);
                PanelBackground = Brushes.DodgerBlue;
            }
        }
        public IBrush PanelBackground
        {
            get => _panelBackground;
            set => this.RaiseAndSetIfChanged(ref _panelBackground, value);
        }
        public string LanguageButtonText
        {
            get => _languageButtonText;
            set => this.RaiseAndSetIfChanged(ref _languageButtonText, value);
        }
        public string Edit
        {
            get => _edit;
            set => this.RaiseAndSetIfChanged(ref _edit, value);
        }
        public string Delete
        {
            get => _delete;
            set => this.RaiseAndSetIfChanged(ref _delete, value);
        }
        public string Add
        {
            get => _add;
            set => this.RaiseAndSetIfChanged(ref _add, value);
        }
        public ReactiveCommand<Unit, Unit> ShowAddAccountWindowCommand { get; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> ChangeLanguageCommand { get; }
        public MainWindowViewModel(IAccountService accountService)
        {
            _accountService = accountService;
            _localizationService = new LocalizationService();
            Accounts = new ObservableCollection<AccountInfo>();
            PanelBackground = Brushes.DodgerBlue;
            Edit = _localizationService.GetString("Edit");
            Delete = _localizationService.GetString("Delete");
            Add = _localizationService.GetString("Add");
            LanguageButtonText = _localizationService.GetString("LanguageButtonText");
            
            ShowAddAccountWindowCommand = ReactiveCommand.Create(OpenAddAccountWindow);
            EditCommand = ReactiveCommand.Create(OpenEditAccountWindow, this.WhenAnyValue(x => x.SelectedAccount).Select(account => account != null));
            DeleteCommand = ReactiveCommand.Create(DeleteAccount, this.WhenAnyValue(x => x.SelectedAccount).Select(account => account != null));
            ChangeLanguageCommand = ReactiveCommand.Create(ChangeLanguage);
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
        private void ChangeLanguage()
        {
            var currentCulture = _localizationService.CurrentCulture.Name;

            // 切换语言
            if (currentCulture == "zh-CN")
            {
                _localizationService.ChangeCulture("en-US");
            }
            else
            {
                _localizationService.ChangeCulture("zh-CN");
            }
            // 更新界面文本
            Edit = _localizationService.GetString("Edit");
            Delete = _localizationService.GetString("Delete");
            Add = _localizationService.GetString("Add");
            LanguageButtonText = _localizationService.GetString("LanguageButtonText");
        }
    }
}
