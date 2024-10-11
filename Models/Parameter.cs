using System.ComponentModel.DataAnnotations;

namespace password.Models;

public class Parameter
{
    [Key]
    public int Id { get; init; }
    
    [Required] public int DecryptionCount { get; set; }
}