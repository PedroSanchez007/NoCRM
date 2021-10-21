using System;
using System.Globalization;

namespace NoCRM
{
    public static class ExtensionMethods
    {
        public static string Separator => "=>";
        
        public static string NowFormatted()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
        
        public static string BeforeSeparator(this string input)
        {
            if (input == null)
                return string.Empty;

            input = input.Trim();
            var index = input.IndexOf(Separator, StringComparison.InvariantCulture);
            return index == -1 ? string.Empty : input[..index].Trim();
        }
        
        public static int AfterSeparator(this string input)
        {
            if (input == null)
                return default;

            input = input.Trim();
            var index = input.IndexOf(Separator, StringComparison.InvariantCulture);
            if (index == -1) 
                return default;
            
            var numberText = input[(index + 1)..].Trim();
            if (!int.TryParse(numberText, NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
                return default;

            return number;
        }
    }
}