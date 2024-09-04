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
            set
            {
                this.RaiseAndSetIfChanged(ref _accountName, value);
                UpdateLastModified();
            }
        }

        [Required]
        public string Account
        {
            get => _account;
            set
            {
                this.RaiseAndSetIfChanged(ref _account, value);
                UpdateLastModified();
            }
        }

        [Required]
        public string Password
        {
            get => _password;
            set
            {
                this.RaiseAndSetIfChanged(ref _password, value);
                UpdateLastModified();
            }
        }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string CreationDate { get; set; }
        public DateTime? LastUpdated { get; set; } = DateTime.Now;

        private void UpdateLastModified()
        {
            LastUpdated = DateTime.Now;
            this.RaisePropertyChanged(nameof(LastUpdated));
        }
    }
}