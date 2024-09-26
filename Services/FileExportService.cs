using System;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace password.Services
{
    public class FileExportService
    {
        public void Export(DataTable dt, string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            
            try
            {
                switch (extension)
                {
                    case ".xls":
                        ExportXls(dt, filePath);
                        break;
                    case ".xlsx":
                        ExportXlsx(dt, filePath);
                        break;
                    case ".csv":
                        ExportCsv(dt, filePath);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported file format: {extension}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting file: {ex.Message}");
                throw; // 可以根据需要选择是否抛出异常
            }
        }

        private void ExportXls(DataTable dt, string filePath)
        {
            try
            {
                var workbook = new HSSFWorkbook();
                SaveExcel(workbook, dt, filePath);
                Console.WriteLine($"Successfully exported to .xls at {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting .xls file: {ex.Message}");
                throw;
            }
        }

        private void ExportXlsx(DataTable dt, string filePath)
        {
            try
            {
                var workbook = new XSSFWorkbook();
                SaveExcel(workbook, dt, filePath);
                Console.WriteLine($"Successfully exported to .xlsx at {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting .xlsx file: {ex.Message}");
                throw;
            }
        }

        private void ExportCsv(DataTable dt, string filePath)
        {
            try
            {
                using var writer = new StreamWriter(filePath);
                
                // 写入表头
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    writer.Write(dt.Columns[i]);
                    if (i < dt.Columns.Count - 1) writer.Write(",");  // 分隔符
                }
                writer.WriteLine();
                
                // 写入数据行
                foreach (DataRow row in dt.Rows)
                {
                    for (var i = 0; i < dt.Columns.Count; i++)
                    {
                        writer.Write(row[i]?.ToString());
                        if (i < dt.Columns.Count - 1) writer.Write(",");  // 分隔符
                    }
                    writer.WriteLine();
                }

                Console.WriteLine($"Successfully exported to .csv at {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting .csv file: {ex.Message}");
                throw;
            }
        }

        private void SaveExcel(IWorkbook workbook, DataTable dt, string filePath)
        {
            try
            {
                var sheet = workbook.CreateSheet("Sheet1");

                // 创建表头
                var headerRow = sheet.CreateRow(0);
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    var cell = headerRow.CreateCell(i);
                    cell.SetCellValue(dt.Columns[i].ColumnName);
                }

                // 创建数据行
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var row = sheet.CreateRow(i + 1);
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(dt.Rows[i][j]?.ToString());
                    }
                }

                // 将文件写入
                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                workbook.Write(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving Excel file: {ex.Message}");
                throw;
            }
        }
    }
}