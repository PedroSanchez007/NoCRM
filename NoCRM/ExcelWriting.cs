using System;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using CsvHelper;
namespace NoCRM
{
    public static class ExcelWriting
    {
        public static void ExportRecords<T>(ImmutableArray<T> records, string exportFileName)
        {
            var exportFilePath = Path.Combine(ExportDirectory, exportFileName);
            using var writer = new StreamWriter(exportFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
            csv.WriteRecords(records);
        }

        public static string ExportDirectory => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }
}