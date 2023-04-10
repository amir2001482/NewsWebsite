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
        public static StartAndEndDate GetStartAndEndDateForSearch(string searchText)
        {
            DateTime? startMiladiDate = Convert.ToDateTime("01/01/01");
            DateTime? endMiladiDate = Convert.ToDateTime("01/01/01");
            var dateTimeResult = searchText.CheckShamsiDate();
            if (dateTimeResult.IsShamsi)
            {
                startMiladiDate = (DateTime)dateTimeResult.MiladiDate;
                if (searchText.Contains(":"))
                    endMiladiDate = startMiladiDate;
                else
                    endMiladiDate = startMiladiDate.Value.Date + new TimeSpan(23, 59, 59);
            }
            return new StartAndEndDate { EndMiladiDate = endMiladiDate, StartMiladiDate = startMiladiDate };
        }
    }
    public class DateTimeResult
    {
        public bool IsShamsi { get; set; }
        public DateTime? MiladiDate { get; set; }
    }
    public class StartAndEndDate
    {
        public DateTime? StartMiladiDate { get; set; }
        public DateTime? EndMiladiDate { get; set; }
    }
}
