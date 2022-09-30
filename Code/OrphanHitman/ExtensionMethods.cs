using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OrphanHitman
{
    public static class ExtensionMethods
    {
        public static string TrimNumbers(this string s, bool allowDotComma = false)
        {
            if (!allowDotComma)
                s = Regex.Replace(s, "[^0-9]", "");
            else
                s = Regex.Replace(s, "[^.,0-9]", "");
            return s.Trim();
        }

        public static int GetInt(this string str)
        {
            if (str == null || str.Length < 1)
                return 0;

            str = str.Trim();

            try
            {
                if (str.Length >= 2 && str[0] == '-' && str[1] != '-')
                    return int.Parse("-" + str.TrimNumbers());
                else
                    return int.Parse(str.TrimNumbers());
            }
            catch
            {
                return 0;
            }
        }
    }
}
