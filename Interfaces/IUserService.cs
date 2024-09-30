using System.Threading.Tasks;
using password.Models;

namespace password.Interfaces;

public interface IUserService
{
    LoggedInUser? CurrentUser { get; set; }
    void Register(string username, string password);
    Task<LoginResponse> LoginAsync(string username, string password);
    void Logout();
    bool IsLoggedIn();
    Task<User?> GetUserByUserNameAsync(string userName);
    Task<bool> SaveUserDataAsync(LoggedInUser user);
}