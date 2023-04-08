using MD.PersianDateTime.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Common
{
    public static class ConvertDateTime
    {
        public static DateTime ConvertShamsiToMiladi(this string date)
        {
            PersianDateTime persianDateTime = PersianDateTime.Parse(date);
            return persianDateTime.ToDateTime();
        }

        public static string ConvertMiladiToShamsi(this DateTime? date, string format)
        {
            PersianDateTime persianDateTime = new PersianDateTime(date);
            return persianDateTime.ToString(format);
        }

        public static DateTimeResult CheckShamsiDate(this string date)
        {
            try
            {
                DateTime miladiDate = PersianDateTime.Parse(date).ToDateTime();
                return new DateTimeResult { IsShamsi = true, MiladiDate = miladiDate };
                
            }
            catch
            {
                return new DateTimeResult { IsShamsi = false };
            }
        }
    }
    public class DateTimeResult
    {
        public bool IsShamsi { get; set; }
        public DateTime? MiladiDate { get; set; }
    }
}
