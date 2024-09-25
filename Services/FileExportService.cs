using System;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel; 

namespace password.Services;

public class FileExportService
{
        public void Export(DataTable dt, string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        
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
                throw new NotSupportedException("Unsupported file format");
        }
    }

    private void ExportXls(DataTable dt, string filePath)
    {
        var workbook = new HSSFWorkbook();
        SaveExcel(workbook, dt, filePath);
    }

    private void ExportXlsx(DataTable dt, string filePath)
    {
        var workbook = new XSSFWorkbook();
        SaveExcel(workbook, dt, filePath);
    }

    private void ExportCsv(DataTable dt, string filePath)
    {
        using var writer = new StreamWriter(filePath);

        for (var i = 0; i < dt.Columns.Count; i++)
        {
            writer.Write(dt.Columns[i]);
            if (i < dt.Columns.Count - 1) writer.Write(",");
        }
        writer.WriteLine();
        
        foreach (DataRow row in dt.Rows)
        {
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                writer.Write(row[i]);
                if (i < dt.Columns.Count - 1) writer.Write(",");
            }
            writer.WriteLine();
        }
    }

    private void SaveExcel(IWorkbook workbook, DataTable dt, string filePath)
    {
        var sheet = workbook.CreateSheet("Sheet1");
        
        var headerRow = sheet.CreateRow(0);
        for (var i = 0; i < dt.Columns.Count; i++)
        {
            var cell = headerRow.CreateCell(i);
            cell.SetCellValue(dt.Columns[i].ColumnName);
        }
        
        for (var i = 0; i < dt.Rows.Count; i++)
        {
            var row = sheet.CreateRow(i + 1);
            for (var j = 0; j < dt.Columns.Count; j++)
            {
                row.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
            }
        }

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        workbook.Write(stream);
    }
}