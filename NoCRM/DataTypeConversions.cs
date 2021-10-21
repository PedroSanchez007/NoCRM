using System;
using System.Globalization;
using Serilog;

namespace NoCRM
{
    public static class DataTypeConversions
    {
        public static DateTime ConvertToDateTime(this string input)
        {
            if (DateTime.TryParse(input, new CultureInfo("en-GB"), DateTimeStyles.AssumeLocal, out var convertedDate))
            {
                return convertedDate;
            }
            Log.Error($"Could not convert {input} to a date");
            return default;
        }
        public static int ConvertToInt(this string input)
        {
            if (Int32.TryParse(input, NumberStyles.Any, new CultureInfo("en-GB"), out var convertedInteger))
            {
                return convertedInteger;
            }
            Log.Error($"Could not convert {input} to an integer");
            return default;
        }
    }
}