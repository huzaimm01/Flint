using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using ClosedXML.Excel;
using System.Drawing.Imaging;
using System.Drawing;
using Xceed.Words.NET;

namespace Flint.Converters
{
    public interface IFileConverter
    {
        void Convert(string sourcePath, string targetPath);
    }

    public class PngToPdfConverter : IFileConverter
    {
        public void Convert(string sourcePath, string targetPath)
        {
            using var doc = new PdfDocument();
            var page = doc.AddPage();
            using var gfx = XGraphics.FromPdfPage(page);
            using var img = XImage.FromFile(sourcePath);

            page.Width = img.PixelWidth * 72 / img.HorizontalResolution;
            page.Height = img.PixelHeight * 72 / img.VerticalResolution;
            gfx.DrawImage(img, 0, 0, page.Width, page.Height);

            doc.Save(targetPath);
            File.Delete(sourcePath);
        }
    }

    public class JpgToPdfConverter : PngToPdfConverter { }

    public class PngToJpgConverter : IFileConverter
    {
        public void Convert(string sourcePath, string targetPath)
        {
            using var img = System.Drawing.Image.FromFile(sourcePath);
            img.Save(targetPath, ImageFormat.Jpeg);
            File.Delete(sourcePath);
        }
    }

    public class JpgToPngConverter : IFileConverter
    {
        public void Convert(string sourcePath, string targetPath)
        {
            using var img = System.Drawing.Image.FromFile(sourcePath);
            img.Save(targetPath, ImageFormat.Png);
            File.Delete(sourcePath);
        }
    }

    public class DocxToPdfConverter : IFileConverter
    {
        public void Convert(string sourcePath, string targetPath)
        {
            // Placeholder: Use LibreOffice CLI or commercial library like Aspose for real implementation
            File.Copy(sourcePath, targetPath); // Simulate
        }
    }

    public class DocxToTxtConverter : IFileConverter
    {
        public void Convert(string sourcePath, string targetPath)
        {
            var doc = DocX.Load(sourcePath);
            File.WriteAllText(targetPath, doc.Text);
            File.Delete(sourcePath);
        }
    }

    public class TxtToPdfConverter : IFileConverter
    {
        public void Convert(string sourcePath, string targetPath)
        {
            var lines = File.ReadAllLines(sourcePath);
            using var doc = new PdfDocument();
            var page = doc.AddPage();
            using var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12);

            double y = 40;
            foreach (var line in lines)
            {
                gfx.DrawString(line, font, XBrushes.Black, new XRect(40, y, page.Width - 80, page.Height - 80));
                y += 20;
                if (y > page.Height - 40)
                    break;
            }

            doc.Save(targetPath);
            File.Delete(sourcePath);
        }
    }

    public class CsvToXlsxConverter : IFileConverter
    {
        public void Convert(string sourcePath, string targetPath)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");
            var lines = File.ReadAllLines(sourcePath);

            for (int i = 0; i < lines.Length; i++)
            {
                var cells = lines[i].Split(',');
                for (int j = 0; j < cells.Length; j++)
                {
                    worksheet.Cell(i + 1, j + 1).Value = cells[j];
                }
            }

            workbook.SaveAs(targetPath);
            File.Delete(sourcePath);
        }
    }

    public class XlsxToCsvConverter : IFileConverter
    {
        public void Convert(string sourcePath, string targetPath)
        {
            using var workbook = new XLWorkbook(sourcePath);
            var worksheet = workbook.Worksheet(1);
            using var writer = new StreamWriter(targetPath);

            foreach (var row in worksheet.RowsUsed())
            {
                var line = string.Join(",", row.Cells().Select(c => c.Value.ToString()));
                writer.WriteLine(line);
            }

            File.Delete(sourcePath);
        }
    }

    public class CsvToJsonConverter : IFileConverter
    {
        public void Convert(string sourcePath, string targetPath)
        {
            var lines = File.ReadAllLines(sourcePath);
            var headers = lines[0].Split(',');
            var list = new List<Dictionary<string, string>>();

            foreach (var line in lines.Skip(1))
            {
                var values = line.Split(',');
                var dict = new Dictionary<string, string>();
                for (int i = 0; i < headers.Length && i < values.Length; i++)
                    dict[headers[i]] = values[i];
                list.Add(dict);
            }

            var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(targetPath, json);
            File.Delete(sourcePath);
        }
    }

    public class JsonToCsvConverter : IFileConverter
    {
        public void Convert(string sourcePath, string targetPath)
        {
            var json = File.ReadAllText(sourcePath);
            var records = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);
            if (records == null || records.Count == 0) return;

            var headers = records[0].Keys;
            using var writer = new StreamWriter(targetPath);
            writer.WriteLine(string.Join(",", headers));

            foreach (var record in records)
                writer.WriteLine(string.Join(",", headers.Select(h => record.ContainsKey(h) ? record[h] : "")));

            File.Delete(sourcePath);
        }
    }
}
