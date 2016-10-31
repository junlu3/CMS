using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.Utilities
{
    public class IDCardHelper
    {
        public static double GetAge(string idCard)
        {
            if (idCard.Length != 18)
            {
                throw new Exception("无效的身份证");
            }
            else
            {
                var year = int.Parse(idCard.Substring(6, 4));
                var month = int.Parse(idCard.Substring(10, 2));
                var day = int.Parse(idCard.Substring(12, 2));

                var span = DateTime.Now - (new DateTime(year, month, day));

                return Math.Round(span.TotalDays / 365d, 2);
            }
        }

        public static DateTime GetBirthDay(string idCard)
        {
            if (idCard.Length != 18)
            {
                throw new Exception("无效的身份证");
            }
            else
            {
                var year = int.Parse(idCard.Substring(6, 4));
                var month = int.Parse(idCard.Substring(10, 2));
                var day = int.Parse(idCard.Substring(12, 2));

                return new DateTime(year, month, day);
            }
        }
    }
}
