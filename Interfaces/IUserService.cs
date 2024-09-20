using System.Threading.Tasks;
using password.Models;

namespace password.Interfaces;

public interface IUserService
{
    void Register(User user);
    Task<User?> GetUserByUserNameAsync(string userName);
}