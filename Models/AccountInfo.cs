using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReactiveUI;

namespace password.Models
{
    public class AccountInfo : ReactiveObject
    {
        private string _accountName;
        private string _account;
        private string _password;

        [Key]
        public int Id { get; set; }

        [Required]
        public string AccountName
        {
            get => _accountName;
            set => this.RaiseAndSetIfChanged(ref _accountName, value);
        }

        [Required]
        public string Account
        {
            get => _account;
            set => this.RaiseAndSetIfChanged(ref _account, value);
        }

        [Required]
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string CreationDate { get; set; }
    }
}