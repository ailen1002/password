using System;
using System.Data;
using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel; 

namespace password.Services;

public class FileService
{
    public DataTable ImportFile(Uri fileUri)
    {
        // 获取文件的绝对路径
        var filePath = fileUri.LocalPath;
        // 获取文件的扩展名并转换为小写
        var extension = Path.GetExtension(filePath).ToLower();
        // 根据文件扩展名调用不同的导入方法
        return extension switch
        {
            ".xls"  => ImportXls(filePath),
            ".xlsx" => ImportXlsx(filePath),
            ".csv"  => ImportCsv(filePath),
            _       => throw new NotSupportedException("Unsupported file format")
        };
    }

    private DataTable ImportXls(string filePath)
    {
        try
        {
            // 检查文件是否存在
            if (!File.Exists(filePath))
            {
                return null;
            }
            
            // 打开文件流用于读取
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // 创建 HSSFWorkbook 对象来处理 .xls 格式的文件
            var workbook = new HSSFWorkbook(stream);

            // 将工作簿转换为 DataTable
            return ExcelToDataTable(workbook);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"文件未找到: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"没有权限访问文件: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"读取 .xls 文件时发生错误: {ex.Message}");
        }
        
        return null;
    }


    private DataTable ImportXlsx(string filePath)
    {
        try
        {
            Console.WriteLine("开始读取 .xlsx 文件...");
        
            // 获取正确的文件路径
            Console.WriteLine($"文件路径: {filePath}");

            // 检查文件是否存在
            if (!File.Exists(filePath))
            {
                Console.WriteLine("文件不存在");
                return null;
            }

            // 打开文件流用于读取 .xlsx 文件
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            Console.WriteLine("文件流已打开，正在解析工作簿...");

            // 使用 XSSFWorkbook 解析 .xlsx 文件
            var workbook = new XSSFWorkbook(stream);
            Console.WriteLine("工作簿已成功读取。");

            // 将工作簿转换为 DataTable
            return ExcelToDataTable(workbook);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"文件未找到: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"没有权限访问文件: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"读取 .xlsx 文件时发生错误: {ex.Message}");
        }

        return null;
    }


    private DataTable ImportCsv(string filePath)
    {
        var dt = new DataTable();
    
        try
        {
            Console.WriteLine("开始读取 .csv 文件...");

            // 打开文件流用于读取 .csv 文件
            using var reader = new StreamReader(filePath);
            Console.WriteLine("文件流已打开，正在解析 .csv 文件内容...");

            bool isFirstRow = true;
        
            // 读取每一行数据
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
            
                // 跳过空行
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // 按逗号分割每一行的数据
                var values = line.Split(',');

                if (isFirstRow)
                {
                    // 第一行为表头，创建列
                    foreach (var header in values)
                    {
                        dt.Columns.Add(header.Trim());
                    }
                    isFirstRow = false;
                }
                else
                {
                    // 检查数据行长度是否匹配表头列数
                    if (values.Length != dt.Columns.Count)
                    {
                        throw new Exception("CSV 数据行和表头列数不匹配！");
                    }

                    // 添加数据行
                    var dataRow = dt.NewRow();
                    for (var i = 0; i < values.Length; i++)
                    {
                        dataRow[i] = values[i].Trim();
                    }
                    dt.Rows.Add(dataRow);
                }
            }

            Console.WriteLine("CSV 文件解析完成。");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"文件未找到: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"没有权限访问文件: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"读取 CSV 文件时发生错误: {ex.Message}");
        }

        return dt;
    }


    private DataTable ExcelToDataTable(IWorkbook workbook)
    {
        var sheet = workbook.GetSheetAt(0);
        var dt = new DataTable();
    
        // 读取表头
        var headerRow = sheet.GetRow(0);
        foreach (var cell in headerRow.Cells)
        {
            dt.Columns.Add(cell.ToString());
        }

        // 读取表格内容，跳过空行
        for (var i = 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);

            // 检查行是否为空
            if (row == null || row.Cells.All(c => c.CellType == CellType.Blank || string.IsNullOrWhiteSpace(c.ToString())))
            {
                continue;  // 跳过空行
            }

            var dataRow = dt.NewRow();

            for (var j = 0; j < row.Cells.Count; j++)
            {
                dataRow[j] = row.GetCell(j)?.ToString() ?? string.Empty;  // 处理空单元格
            }

            dt.Rows.Add(dataRow);
        }
        
        return dt;
    }
    
    private void PrintDataTable(DataTable dt)
    {
        if (dt == null || dt.Rows.Count == 0)
        {
            Console.WriteLine("DataTable 为空或没有数据。");
            return;
        }

        // 打印列名
        foreach (DataColumn column in dt.Columns)
        {
            Console.Write($"{column.ColumnName}\t");
        }
        Console.WriteLine(); // 换行

        // 打印行数据
        foreach (DataRow row in dt.Rows)
        {
            foreach (var item in row.ItemArray)
            {
                Console.Write($"{item}\t");
            }
            Console.WriteLine(); // 换行
        }
    }

}