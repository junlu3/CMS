using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.Utilities
{
    public static class StringHelper
    {
        public static string Join(string[] array, string splitStr)
        {
            var returnValue = string.Empty;
            if (array != null)
            {
                foreach (var a in array)
                {
                    returnValue += a;
                    returnValue += splitStr;
                }
                if (array.Length > 0)
                {
                    returnValue = returnValue.Remove(returnValue.Length - splitStr.Length, splitStr.Length);
                }
            }
            return returnValue;
        }
    }
}
