using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace NoCRM
{
    internal static class Program
    {
        public static AppSettings AppSettings { get; set; }
        
        private static void Main(string[] args)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////
            // Code to export all data to Excel. It will go to MyDocuments/All NoCRM Data.csv
            // Uncomment the following two lines to export the data.
            // var allNoCrmRecords = NoCrmCommunication.GetAllProspects().ToList();
            // ExcelWriting.ExportRecords(allNoCrmRecords.ToImmutableArray(), @"All NoCRM Data.csv");
            ////////////////////////////////////////////////////////////////////////////////////////////////
            
            ////////////////////////////////////////////////////////////////////////////////////////////////
            // var testPing = NoCrmCommunication.Ping();
            ////////////////////////////////////////////////////////////////////////////////////////////////

            ConfigureApp();
            ConfigureLogger();
            Log.Information("NoCRM Sync App started");
            Log.Information("=========================");
            var sync = new Sync();
            Log.Information("NoCRM Sync App shutting down");
            Log.Information("");
        }

        private static void ConfigureApp()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            ConfigurationBinder.Bind(configuration.GetSection("AppSettings"), AppSettings);
        }

        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}