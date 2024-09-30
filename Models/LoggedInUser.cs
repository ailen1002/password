using System;

namespace password.Models;

public class LoggedInUser
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? PassWord { get; set; }
    
    public string? Role { get; set; }
    public DateTime LoginTime { get; set; }
}