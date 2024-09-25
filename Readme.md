### 1，使用avalonia模版创建一个MVVM程序。
### 2, 添加软件包支持
​    Avalonia
​    Avalonia.Controls.DataGird
​    Avalonia.Desktop
​    Avalonia.Fonts.Inter
​    Avalonia.ReactiveUI
​    Avalonia.Themes.Fluent
​    MessageBox.Avalonia
​    Microsoft.Data.Sqlite
​    Microsoft.EntityFrameworkCore
​    Microsoft.EntityFrameworkCore.Sqlite.Core
​    NLog
​    NLog.Extensions.Logging
​    NPOI

### 3, DataGird使用时候需要在App.axaml中添加

```c#
<Application.Styles>
    <StyleInclude Source="avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml" />
<Application.Styles>
```