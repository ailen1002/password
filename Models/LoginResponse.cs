namespace password.Models

{
    public enum LoginResult
    {
        Success,
        InvalidUsername,
        IncorrectPassword
    }

    public class LoginResponse
    {
        public LoginResult Result { get; set; }
        public LoggedInUser? User { get; set; }
    }
}
