using System.Collections.Generic;
using Flint.Converters;

namespace Flint.Core
{
    public static class ConversionRegistry
    {
        public static Dictionary<(string, string), IFileConverter> Register()
        {
            return new Dictionary<(string, string), IFileConverter>
            {
                { (".png", ".pdf"), new PngToPdfConverter() },
                { (".jpg", ".pdf"), new JpgToPdfConverter() },
                { (".png", ".jpg"), new PngToJpgConverter() },
                { (".jpg", ".png"), new JpgToPngConverter() },

                { (".docx", ".pdf"), new DocxToPdfConverter() },
                { (".docx", ".txt"), new DocxToTxtConverter() },
                { (".txt", ".pdf"), new TxtToPdfConverter() },

                { (".csv", ".xlsx"), new CsvToXlsxConverter() },
                { (".xlsx", ".csv"), new XlsxToCsvConverter() },

                { (".csv", ".json"), new CsvToJsonConverter() },
                { (".json", ".csv"), new JsonToCsvConverter() },
            };
        }
    }
}
