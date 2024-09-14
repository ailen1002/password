using password.Data;
using password.Interfaces;
using password.Models;

namespace password.Services
{
    public class UserService(MainDbContext context) : IUserService
    {
        public void Register(User user)
        {
            context.User.Add(user);
            context.SaveChanges();
        }
    }
}

