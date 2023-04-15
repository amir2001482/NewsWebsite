using MD.PersianDateTime.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Common
{
    public static class DateTimeExtensions
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
        public static List<DateTime?> GetStartAndEndDateForSearch(this string searchText)
        {
            DateTime? StartMiladiDate = Convert.ToDateTime("01/01/01");
            DateTime? EndMiladiDate = Convert.ToDateTime("01/01/01");
            var dateTimeResult = searchText.CheckShamsiDate();
            if (dateTimeResult.IsShamsi)
            {
                StartMiladiDate = (DateTime)dateTimeResult.MiladiDate;
                if (searchText.Contains(":"))
                    EndMiladiDate = StartMiladiDate;
                else
                    EndMiladiDate = StartMiladiDate.Value.Date + new TimeSpan(23, 59, 59);
            }
            return new List<DateTime?>{ StartMiladiDate , EndMiladiDate};
        }
        public static bool IsLeapYear(this DateTime? date)
        {
            PersianDateTime persianDateTime = new PersianDateTime(date);
            return persianDateTime.IsLeapYear;
        }
    }
    public class DateTimeResult
    {
        public bool IsShamsi { get; set; }
        public DateTime? MiladiDate { get; set; }
        public string SearchText { get; set; }
    }
}
