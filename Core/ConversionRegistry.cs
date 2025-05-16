using System;
using System.Collections.Generic;
using System.IO;
using Flint.Converters;

namespace Flint.Core
{
    public static class ConverterManager
    {
        private static readonly Dictionary<(string, string), IFileConverter> converters = ConversionRegistry.Register();

        public static void Convert(string sourcePath, string targetPath)
        {
            string sourceExt = Path.GetExtension(sourcePath).ToLower();
            string targetExt = Path.GetExtension(targetPath).ToLower();

            if (converters.TryGetValue((sourceExt, targetExt), out var converter))
            {
                try
                {
                    converter.Convert(sourcePath, targetPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Conversion failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"No converter registered for: {sourceExt} -> {targetExt}");
            }
        }
    }
}
