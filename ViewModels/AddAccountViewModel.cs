using System;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using NLog;
using ReactiveUI;
using password.Interfaces;
using password.Models;
using password.Services;

namespace password.ViewModels

{
    public class AddAccountViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly IAccountService _accountService;
        private readonly LocalizationService _localizationService;
        private string _accountNameLabel= string.Empty;
        private string _accountLabel= string.Empty;
        private string _passwordLabel= string.Empty;
        private string _generateButton= string.Empty;
        private string _confirm= string.Empty;
        private string _cancel= string.Empty;
        private bool _includeUppercase;
        private bool _includeLowercase;
        private bool _includeSpecialChar;
        private bool _includeNumbers;
        private string _upperCaseLetters= string.Empty;
        private string _lowerCaseLetters= string.Empty;
        private string _specialSymbol= string.Empty;
        private string _numbers= string.Empty;
        private string _passwordLength= string.Empty;
        private string _passwordLengthInput= string.Empty;
        private string _password= string.Empty;
        public string? AccountName { get; set; }
        public string? Account { get; set; }
        public ReactiveCommand<Unit, Unit> GeneratePasswordCommand { get; }
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
        public string GenerateButton
        {
            get => _generateButton;
            set => this.RaiseAndSetIfChanged(ref _generateButton, value);
        }
        public string UpperCaseLetters
        {
            get => _upperCaseLetters;
            set => this.RaiseAndSetIfChanged(ref _upperCaseLetters, value);
        }
        public string LowerCaseLetters
        {
            get => _lowerCaseLetters;
            set => this.RaiseAndSetIfChanged(ref _lowerCaseLetters, value);
        }
        public string SpecialSymbol
        {
            get => _specialSymbol;
            set => this.RaiseAndSetIfChanged(ref _specialSymbol, value);
        }
        public string Numbers
        {
            get => _numbers;
            set => this.RaiseAndSetIfChanged(ref _numbers, value);
        }
        public string PasswordLength
        {
            get => _passwordLength;
            set => this.RaiseAndSetIfChanged(ref _passwordLength, value);
        }
        public bool IncludeUppercase
        {
            get => _includeUppercase;
            set => this.RaiseAndSetIfChanged(ref _includeUppercase, value);
        }
        public bool IncludeLowercase
        {
            get => _includeLowercase;
            set => this.RaiseAndSetIfChanged(ref _includeLowercase, value);
        }
        public bool IncludeSpecialChar
        {
            get => _includeSpecialChar;
            set => this.RaiseAndSetIfChanged(ref _includeSpecialChar, value);
        }
        public bool IncludeNumbers
        {
            get => _includeNumbers;
            set => this.RaiseAndSetIfChanged(ref _includeNumbers, value);
        }
        public string PasswordLengthInput
        {
            get => _passwordLengthInput;
            set => this.RaiseAndSetIfChanged(ref _passwordLengthInput, value);
        }
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
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
            GenerateButton = _localizationService.GetString("GenerateButton");
            Confirm = _localizationService.GetString("Confirm");
            Cancel = _localizationService.GetString("Cancel");
            UpperCaseLetters = _localizationService.GetString("UpperCaseLetters");
            LowerCaseLetters = _localizationService.GetString("LowerCaseLetters");
            SpecialSymbol = _localizationService.GetString("SpecialSymbol");
            Numbers = _localizationService.GetString("Numbers");
            PasswordLength = _localizationService.GetString("PasswordLength");
            // 添加账户命令
            AddCommand = ReactiveCommand.Create(AddAccount);
            // 取消命令，关闭窗口
            CancelCommand = ReactiveCommand.Create(CloseWindow);
            // 生成密码命令
            GeneratePasswordCommand = ReactiveCommand.Create(Generate);
        }

        private void Generate()
        {
            var passwordLength = int.TryParse(PasswordLengthInput, out var length) ? length : 0;
            // 根据用户选项生成密码
            var generatedPassword = GeneratePassword(IncludeUppercase, IncludeLowercase, IncludeSpecialChar, IncludeNumbers, passwordLength);

            // 将生成的密码显示在文本框中
            Password = generatedPassword;
        }
        private static string GeneratePassword(bool includeUppercase, bool includeLowercase, bool includeSpecialChar, bool includeNumbers, int length)
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string specialChars = "!@#$%^&*()";
            const string numberChars = "0123456789";

            var allowedChars = "";
            if (includeUppercase) allowedChars += uppercaseChars;
            if (includeLowercase) allowedChars += lowercaseChars;
            if (includeSpecialChar) allowedChars += specialChars;
            if (includeNumbers) allowedChars += numberChars;

            if (string.IsNullOrEmpty(allowedChars) || length <= 0)
                return string.Empty;

            var random = new Random();
            return new string(Enumerable.Repeat(allowedChars, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
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
                Logger.Info("Window closed successfully.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to close window.");
            }
        }
        private void UpdateLocalizedTexts()
        {
            AccountNameLabel = _localizationService.GetString("AccountNameLabel");
            AccountLabel = _localizationService.GetString("AccountLabel");
            PasswordLabel = _localizationService.GetString("PasswordLabel");
            GenerateButton = _localizationService.GetString("GenerateButton");
            Confirm = _localizationService.GetString("Confirm");
            Cancel = _localizationService.GetString("Cancel");
            UpperCaseLetters = _localizationService.GetString("UpperCaseLetters");
            LowerCaseLetters = _localizationService.GetString("LowerCaseLetters");
            SpecialSymbol = _localizationService.GetString("SpecialSymbol");
            Numbers = _localizationService.GetString("Numbers");
            PasswordLength = _localizationService.GetString("PasswordLength");
        }
    }
}
