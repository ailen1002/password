using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace password.Models;

public class ModelAccount
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? AccountName { get; set; }

    [Required]
    public string? Account { get; set; }

    [Required]
    public string? Password { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string CreationDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}