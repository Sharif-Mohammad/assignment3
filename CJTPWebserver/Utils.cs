using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CJTPWebserver
{
    public static class Utils
    {
        public static bool IsValidUnixTimestamp(string timestamp)
        {
            // Try to parse the string to a long
            if (long.TryParse(timestamp, out long unixTimestamp))
            {
                DateTimeOffset minDateTime = DateTimeOffset.FromUnixTimeSeconds(0); // 01 Jan 1970
                DateTimeOffset maxDateTime = DateTimeOffset.FromUnixTimeSeconds(2147483647); // ~19 Jan 2038

                try
                {
                    DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
                    return dateTime >= minDateTime && dateTime <= maxDateTime;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return false; 
                }
            }

            return false; 
        }

        public static bool IsValidPathWithIntId(string path)
        {
            string pattern = @"^/api/categories/\d+$";
            return Regex.IsMatch(path, pattern);
        }

        public static bool IsValidBasePath(string path)
        {
            string pattern = "/api/categories";
            return pattern == path;
        }

    }
}
