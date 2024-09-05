using System.ComponentModel.DataAnnotations;

namespace password.Models;

public class Users
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string PasswordHash  { get; set; }
}