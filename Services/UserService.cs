using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
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
        private int _decryptionCount = 0;
        private const int MaxDecryptionCount = 5;
        private string _encryptionKey = "";
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true, // 格式化输出
            PropertyNameCaseInsensitive = true, // 忽略大小写
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
            
            if (string.IsNullOrEmpty(_encryptionKey))
            {
                _encryptionKey = GenerateKey();
            }
            
            var loginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"));
            var encryptedLoginTime = Encrypt(loginTime, _encryptionKey);
            
            // 登录成功后，保存用户信息
            CurrentUser = new LoggedInUser
            {
                UserId = user.Id,
                UserName = user.UserName,
                PassWord = user.PassWordHash,
                Role = _encryptionKey,
                LoginTime = encryptedLoginTime
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
                var jsonData = File.ReadAllText(_userFilePath);
                CurrentUser = JsonSerializer.Deserialize<LoggedInUser>(jsonData, JsonOptions);
            }
            if (CurrentUser == null) return false;
            // 解密登录时间
            var decryptedLoginTime = Decrypt(CurrentUser.LoginTime, CurrentUser.Role);

            Console.WriteLine(decryptedLoginTime);

            if (!DateTime.TryParse(decryptedLoginTime, out var loginTime)) return false;
            
            Console.WriteLine((DateTime.Now - loginTime).TotalDays);
            // 检查登录时间是否在7天有效期内
            if ((DateTime.Now - loginTime).TotalDays <= 1)
            {
                return true; // 登录仍在有效期内
            }
            else
            {
                Logout(); // 超过有效期，自动登出
            }

            return false;
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
                LogService.Error(ex, "保存用户数据失败");
                return false;
            }
        }
        
        private static string GenerateKey(int keySize = 32) // 32 bytes for 256-bit AES
        {
            using var rng = RandomNumberGenerator.Create();
            var keyBytes = new byte[keySize];
            rng.GetBytes(keyBytes);
            return Convert.ToBase64String(keyBytes);
        }
        
        private static string Encrypt(string plainText, string key)
        {
            using var aesAlg = Aes.Create();
    
            // Get the exact key size needed for AES (32 bytes for AES-256)
            var keyBytes = Convert.FromBase64String(key); // Convert from Base64 string
            if (keyBytes.Length != 32) throw new ArgumentException("Key must be 256 bits for AES-256");

            aesAlg.Key = keyBytes; // Set the key for AES
            aesAlg.GenerateIV(); // Create a new IV

            using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();

            msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length); // Write IV to stream

            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using var swEncrypt = new StreamWriter(csEncrypt);

            swEncrypt.Write(plainText); // Write the plaintext to encrypt
            swEncrypt.Close();
            
            return Convert.ToBase64String(msEncrypt.ToArray()); // Return encrypted data as Base64
        }

        private string? Decrypt(string? cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(key)) return null;

            var fullCipher = Convert.FromBase64String(cipherText);

            using var aesAlg = Aes.Create();
            var iv = new byte[aesAlg.BlockSize / 8];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Array.Copy(fullCipher, iv, iv.Length); // Extract IV from the beginning
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            var keyBytes = Convert.FromBase64String(key); // Convert the Base64 key back to bytes
            if (keyBytes.Length != 32) throw new ArgumentException("Key must be 256 bits for AES-256");

            using var decryptor = aesAlg.CreateDecryptor(keyBytes, iv);
            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            OnDecrypt();
            return srDecrypt.ReadToEnd(); // Return decrypted text
        }

        private void OnDecrypt()
        {
            _decryptionCount++;
            if (_decryptionCount < MaxDecryptionCount) return;
            _encryptionKey = GenerateKey();
            // 重置计数器并更新密钥存储
            UpdateConfig(_encryptionKey);
            _decryptionCount = 0;
        }
        
        private async void UpdateConfig(string newKey)
        {
            // 读取当前配置文件内容
            CurrentUser = JsonSerializer.Deserialize<LoggedInUser>(await File.ReadAllTextAsync(_userFilePath));
            // 更新密钥
            if (CurrentUser != null) CurrentUser.Role = newKey;
            // 写回配置文件
            var jsonData = JsonSerializer.Serialize(CurrentUser, JsonOptions);;
            await File.WriteAllTextAsync(_userFilePath, jsonData);
        }
    }
}

