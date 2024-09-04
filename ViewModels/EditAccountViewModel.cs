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
    public class EditAccountViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;
        private readonly LocalizationService _localizationService;
        private string _accountNameLabel;
        private string _accountLabel;
        private string _passwordLabel;
        private string _generateButton;
        private bool _includeUppercase;
        private bool _includeLowercase;
        private bool _includeSpecialChar;
        private bool _includeNumbers;
        private string _upperCaseLetters;
        private string _lowerCaseLetters;
        private string _specialSymbol;
        private string _numbers;
        private string _passwordLength;
        private string _passwordLengthInput;
        private string _confirm;
        private string _cancel;
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
        public AccountInfo AccountInfo { get; set; }
        public ReactiveCommand<Unit, Unit> GeneratePasswordCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public EditAccountViewModel(IAccountService accountService, LocalizationService localizationService, AccountInfo account)
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
            AccountInfo = account;
            // 更新账户命令
            UpdateCommand = ReactiveCommand.Create(UpdateChanges);
            // 取消命令，关闭窗口
            CancelCommand = ReactiveCommand.Create(CloseWindow);
            // 生成密码命令
            GeneratePasswordCommand = ReactiveCommand.Create(Generate);
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
        private void Generate()
        {
            var passwordLength = int.TryParse(PasswordLengthInput, out var length) ? length : 0;
            // 根据用户选项生成密码
            var generatedPassword = GeneratePassword(IncludeUppercase, IncludeLowercase, IncludeSpecialChar, IncludeNumbers, passwordLength);

            // 将生成的密码显示在文本框中
            AccountInfo.Password = generatedPassword;
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