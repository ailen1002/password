using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using password.Data;
using password.Interfaces;
using password.Models;

namespace password.Services
{
    public class UserService : IUserService
    {
        private readonly MainDbContext _context;
        private readonly string _userFilePath;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true, // 格式化输出
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // 使用 camelCase 命名策略
        };
        public LoggedInUser? CurrentUser { get; set; }
        
        public UserService(MainDbContext context)
        {
            const bool useUserDirectory = true;
            _context = context;

            // 根据需要选择路径
            var userDataPathProvider = new UserDataPathProvider(useUserDirectory);
            _userFilePath = userDataPathProvider.UserFilePath;
        }
        public void Register(string username, string password)
        {
            var user = new User
            {
                UserName = username,
                PassWordHash = HashPassword(password)
            };

            _context.User.Add(user);
            _context.SaveChanges();
        }
        private static string HashPassword(string password)
        {
            // 生成盐并哈希密码
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            var user = await GetUserByUserNameAsync(username);
            // 检查用户名是否存在
            if (user == null)
            {
                return new LoginResponse
                {
                    Result = LoginResult.InvalidUsername
                };
            }

            // 验证密码
            if (!BCrypt.Net.BCrypt.Verify(password, user.PassWordHash))
            {
                return new LoginResponse
                {
                    Result = LoginResult.IncorrectPassword
                };
            }

            // 登录成功后，保存用户信息
            CurrentUser = new LoggedInUser
            {
                UserId = user.Id,
                UserName = user.UserName,
                PassWord = user.PassWordHash,
                LoginTime = DateTime.Now
            };
            
            await SaveUserDataAsync(CurrentUser);
            
            return new LoginResponse
            {
                Result = LoginResult.Success,
                User = CurrentUser
            };
        }

        public void Logout()
        {
            CurrentUser = null;
            if (File.Exists(_userFilePath))
            {
                File.Delete(_userFilePath);
            }
        }

        public bool IsLoggedIn()
        {
            if (File.Exists(_userFilePath) && CurrentUser == null)
            {
                CurrentUser = JsonSerializer.Deserialize<LoggedInUser>(File.ReadAllText(_userFilePath));
            }
            return CurrentUser != null;
        }
        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.UserName == userName);
            return user;
        }
        public async Task<bool> SaveUserDataAsync(LoggedInUser user)
        {
            try
            {
                // 将用户数据序列化并保存到文件
                var jsonData = JsonSerializer.Serialize(user, JsonOptions);
                await File.WriteAllTextAsync(_userFilePath, jsonData);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

