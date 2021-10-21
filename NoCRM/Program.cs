using System.Collections.Immutable;
using System.Linq;
using Serilog;

namespace NoCRM
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////
            // Code to export all data to Excel. It will go to MyDocuments/All NoCRM Data.csv
            // Uncomment the following two lines to export the data.
            // var allNoCrmRecords = NoCrmCommunication.GetAllProspects().ToList();
            // ExcelWriting.ExportRecords(allNoCrmRecords.ToImmutableArray(), @"All NoCRM Data.csv");
            ////////////////////////////////////////////////////////////////////////////////////////////////
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            Log.Information("NoCRM Sync App started");
            Log.Information("=========================");
            var sync = new Sync();
            Log.Information("NoCRM Sync App shutting down");
            Log.Information("");
        }
    }
}