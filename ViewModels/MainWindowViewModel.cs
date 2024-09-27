using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
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
        private readonly string _defaultExportPath;
        private AccountInfo? _selectedAccount;
        private bool _isDarkMode;
        private IBrush _panelBackground = Brushes.White;
        private bool _isAccountInfoVisible;
        private string _edit= string.Empty;
        private string _delete= string.Empty;
        private string _add= string.Empty;
        private string _languageButtonText= string.Empty;
        private string _import= string.Empty;
        private string _export= string.Empty;
        private string _UserName= string.Empty;
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
        public bool IsAccountInfoVisible
        {
            get => _isAccountInfoVisible;
            set => this.RaiseAndSetIfChanged(ref _isAccountInfoVisible, value);
        }
        public string UserName
        {
            get => _UserName;
            set => this.RaiseAndSetIfChanged(ref _UserName, value);
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
        public ReactiveCommand<Unit, Unit> ShowAccountInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
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
            _defaultExportPath = Path.Combine(AppContext.BaseDirectory, "TestRecord.csv");
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
            ShowAccountInfoCommand = ReactiveCommand.Create(ToggleAccountInfoDrawer);
            LogoutCommand = ReactiveCommand.Create(Logout);
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
        
        private async void Export_click()
        {
            try
            {
                var fileExportService = new FileExportService();
                var dt = GetDataTable(); // 获取要导出的数据
                var useDefault = false;
                string filePath;

                if (useDefault)
                {
                    filePath = _defaultExportPath;
                }
                else
                {
                    var customFilePath = await ShowSaveFileDialog();
                    Console.WriteLine(customFilePath);
                    if (string.IsNullOrEmpty(customFilePath))
                    {
                        return; // 如果用户取消，则返回
                    }
                    filePath = customFilePath;
                }

                // 调用导出服务
                fileExportService.Export(dt, filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"导出过程中发生错误: {ex.Message}");
                // 可以在这里添加更多的错误处理逻辑，例如显示错误消息给用户
            }
        }
        
        private async Task<string?> ShowSaveFileDialog()
        {
            var saveWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Windows
                .FirstOrDefault(w => w.DataContext == this);
            if (saveWindow == null)
            {
                throw new InvalidOperationException("没有活动窗口");
            }

            var savePickerOptions = new FilePickerSaveOptions
            {
                Title = "保存文件",
                SuggestedFileName = "TestRecord.csv",
                FileTypeChoices = new List<FilePickerFileType>
                {
                    new FilePickerFileType("CSV Files") { Patterns = new[] { "*.csv" } },
                    new FilePickerFileType("Excel 97-2003 Files") { Patterns = new[] { "*.xls" } },
                    new FilePickerFileType("Excel Files") { Patterns = new[] { "*.xlsx" } }
                }
            };

            try
            {
                Console.WriteLine("尝试打开文件保存对话框...");
                var result = await saveWindow.StorageProvider.SaveFilePickerAsync(savePickerOptions);
                Console.WriteLine("文件保存对话框调用完成");

                if (result == null)
                {
                    Console.WriteLine("文件对话框被取消。");
                    return null;
                }

                Console.WriteLine("文件保存路径: " + result.Path.LocalPath);
                return result.Path.LocalPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生异常: {ex.Message}");
                return null;
            }
        }
        
        private DataTable GetDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("AccountName", typeof(string));
            dt.Columns.Add("Account", typeof(string));
            dt.Columns.Add("Password", typeof(string));
            dt.Columns.Add("CreationDate", typeof(string));
            dt.Columns.Add("UpdateDate", typeof(string));
            foreach (var account in Accounts)
            {
                dt.Rows.Add(account.Id, account.AccountName, account.Account, account.Password, account.CreationDate,account.LastUpdated);
            }
            return dt;
        }
        private void ToggleAccountInfoDrawer()
        {
            IsAccountInfoVisible = !IsAccountInfoVisible;  // 切换抽屉的显示状态
        }

        private void Logout()
        {
            //Todo 实现用户注销功能
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
