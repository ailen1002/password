using password.Models;

namespace password.Interfaces;

public interface IUserService
{
    void Register(User user);
}