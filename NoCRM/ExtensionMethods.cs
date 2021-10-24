using System;
using System.Globalization;

namespace NoCRM
{
    public static class ExtensionMethods
    {
        public static string BeforeSeparator(this string input)
        {
            if (input == null)
                return string.Empty;

            input = input.Trim();
            var index = input.LastIndexOf(Sync.Separator, StringComparison.InvariantCulture);
            return index == -1 ? string.Empty : input[..index].Trim();
        }
        
        public static int AfterSeparator(this string input)
        {
            if (input == null)
                return default;

            input = input.Trim();
            var index = input.LastIndexOf(Sync.Separator, StringComparison.InvariantCulture);
            if (index == -1) 
                return default;
            
            var numberText = input[(index + Sync.Separator.Length)..].Trim();
            if (!int.TryParse(numberText, NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
                return default;

            return number;
        }

        public static bool IsMatch(this string apple, string orange)
        {
            if (apple == null && orange == null ||
                apple == null && orange == string.Empty ||
                apple == string.Empty && orange == null)
                return true;
            
            if (apple == null || orange == null) 
                return false;
            
            if (orange.Contains("&amp"))
                return true;
            
            return
                string.Equals(apple.Trim(), orange.Trim(), StringComparison.CurrentCultureIgnoreCase);
        }
    }
}