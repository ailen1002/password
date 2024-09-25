﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Platform.Storage;
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
        private AccountInfo? _selectedAccount;
        private bool _isDarkMode;
        private IBrush _panelBackground = Brushes.White;
        private string _edit= string.Empty;
        private string _delete= string.Empty;
        private string _add= string.Empty;
        private string _languageButtonText= string.Empty;
        private string _import= string.Empty;
        private string _export= string.Empty;
        public ObservableCollection<AccountInfo> Accounts { get; set; }
        public AccountInfo? SelectedAccount
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
            if (Application.Current is not App app) return;
            
            var currentTheme = IsDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
            app.ChangeThemeVariant(currentTheme);
            PanelBackground = IsDarkMode? Brushes.DarkMagenta:Brushes.DodgerBlue;
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
        public string Import
        {
            get => _import;
            set => this.RaiseAndSetIfChanged(ref _import, value);
        }
        public string Export
        {
            get => _export;
            set => this.RaiseAndSetIfChanged(ref _export, value);
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
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
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
            Import = _localizationService.GetString("Import");
            Export = _localizationService.GetString("Export");
            // 订阅语言切换事件
            _localizationService.LanguageChanged += UpdateLocalizedTexts;
            
            ShowAddAccountWindowCommand = ReactiveCommand.Create(OpenAddAccountWindow);
            EditCommand = ReactiveCommand.Create(
                OpenEditAccountWindow, 
                this.WhenAnyValue(x => x.SelectedAccount)
                    .Select(account => account != null)
            );
            DeleteCommand = ReactiveCommand.Create(
                DeleteAccount, 
                this.WhenAnyValue(x => x.SelectedAccount)
                    .Select(account => account != null)
            );
            ChangeLanguageCommand = ReactiveCommand.Create(ChangeLanguage);
            ImportCommand = ReactiveCommand.Create(Import_click);
            ExportCommand = ReactiveCommand.Create(Export_click);
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
                DataContext = new AddAccountViewModel(_accountService, _localizationService)
            };

            addAccountWindow.Show();
            addAccountWindow.Closed += (sender, e) => LoadAccounts();
        }

        private void OpenEditAccountWindow()
        {
            var editAccountWindow = new EditAccount
            {
                DataContext = new EditAccountViewModel(_accountService, _localizationService, SelectedAccount)
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
            if (SelectedAccount != null) _accountService.DeleteAccount(SelectedAccount.Id);
            LoadAccounts();
        }
        private async void Import_click()
        {
            try
            {
                var options = new FilePickerOpenOptions
                {
                    Title = "选择要导入的文件",
                    AllowMultiple = false
                };

                var file = await OpenFilePickerAsync(options);

                if (file is null) return;
                var properties = await file.GetBasicPropertiesAsync();
                var filePath = file.Path;
                // 限制文件大小为 50MB
                if (properties.Size <= 1024 * 1024 * 50)
                {
                    // 使用 FileService 处理文件内容并导入到数据库
                    var fileService = new FileService();
  
                    var dataTable = fileService.ImportFile(filePath); // 确保该方法实现处理内容

                    _accountService.AddAccounts(dataTable); // 添加到数据库
                    
                    LoadAccounts();
                }
                else
                {
                    throw new Exception("文件超出 50MB 限制。");
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("错误", ex.Message, ButtonEnum.OkAbort, Icon.Error);
            }
        }

        
        private async Task<IStorageFile?> OpenFilePickerAsync(FilePickerOpenOptions options)
        {
            var currentWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Windows
                .FirstOrDefault(w => w.DataContext == this);
            if (currentWindow == null)
            {
                throw new InvalidOperationException("没有活动窗口");
            }
            
            var result = await currentWindow.StorageProvider.OpenFilePickerAsync(options);
            
            return result.Count > 0 ? result[0] : null;
        }
        
        private void Export_click()
        {
            // Todo 导出功能实现
        }
        // 切换语言
        private void ChangeLanguage()
        {
            var currentCulture = _localizationService.CurrentCulture.Name;

            _localizationService.ChangeCulture(currentCulture == "zh-CN" ? "en-US" : "zh-CN");
        }
        private void UpdateLocalizedTexts()
        {
            Edit = _localizationService.GetString("Edit");
            Delete = _localizationService.GetString("Delete");
            Add = _localizationService.GetString("Add");
            LanguageButtonText = _localizationService.GetString("LanguageButtonText");
            Import = _localizationService.GetString("Import");
            Export = _localizationService.GetString("Export");
        }
    }
}
