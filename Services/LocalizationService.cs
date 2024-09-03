using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace password.Services;

public class LocalizationService
{
    private readonly Dictionary<string, ResourceManager> _resourceManagers;
    private ResourceManager _currentResourceManager;
    public CultureInfo CurrentCulture => CultureInfo.CurrentUICulture;
    public LocalizationService()
    {
        // 初始化资源管理器
        _resourceManagers = new Dictionary<string, ResourceManager>
        {
            { "zh-CN", new ResourceManager("password.Resources.zh-CN", typeof(LocalizationService).Assembly) },
            { "en-US", new ResourceManager("password.Resources.en-US", typeof(LocalizationService).Assembly) }
        };
        
        // 默认语言
        _currentResourceManager = _resourceManagers["zh-CN"];
    }
    public void ChangeCulture(string cultureName)
    {
        // 设置当前文化
        var culture = new CultureInfo(cultureName);
        CultureInfo.CurrentUICulture = culture;
        _currentResourceManager = _resourceManagers[cultureName];
    }

    public string GetString(string key)
    {
        return _currentResourceManager.GetString(key, CultureInfo.CurrentUICulture);
    }
}