using System;
using System.Text.RegularExpressions;

namespace SKS_Klient
{
    static class RegexValidator
    {
        public static bool IsValidRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }
    }
}
