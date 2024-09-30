using System.ComponentModel.DataAnnotations;

namespace password.Models;

public class User
{
    [Key]
    public int Id { get; init; }
    
    [Required] public string UserName { get; set; } = string.Empty;
    [Required] public string PassWordHash { get; set; } = string.Empty;
}