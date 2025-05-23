using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.StringExtensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        public static bool IsNullOrEmpty(this object thisValue) => thisValue == null || thisValue == DBNull.Value || string.IsNullOrWhiteSpace(thisValue.ToString());

    }
}
