using System.ComponentModel.DataAnnotations;

namespace password.Models;

public class Users
{
    [Key]
    public int Id { get; set; }
    
    [Required] public string UserName { get; set; } = string.Empty;
    [Required] public string PasswordHash { get; set; } = string.Empty;
}