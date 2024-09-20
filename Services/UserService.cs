using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                LogService.Warn("Attempted to find user with an empty or null username.");
                return null;
            }

            try
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.UserName == userName);
        
                if (user == null)
                {
                    LogService.Info($"User with username '{userName}' not found.");
                    return null;
                }
        
                LogService.Debug($"User found: {user.UserName}");
                return user;
            }
            catch (Exception ex)
            {
                LogService.Error(ex, $"Error occurred while fetching user with username '{userName}'");
                return null;
            }
        }
    }
}

