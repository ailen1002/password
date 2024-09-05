using System.Collections.Generic;
using password.Models;

namespace password.Interfaces
{
    public interface IAccountService
    {
        // 添加新账户
        void AddAccount(AccountInfo accountInfo);

        // 查询所有账户
        List<AccountInfo> LoadAccounts();

        // 根据ID查询账户
        AccountInfo GetAccountById(int accountId);

        // 根据关键字查询账户
        List<AccountInfo> SearchAccounts(string keyword);

        // 更新现有账户
        void UpdateAccount(AccountInfo? accountInfo);

        // 删除账户
        void DeleteAccount(int accountId);
    }
}