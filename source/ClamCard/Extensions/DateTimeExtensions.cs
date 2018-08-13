using System;
using System.Globalization;

namespace ClamCard.Extensions
{
    public static class DateTimeExtensions
    {
        public static int WeekOfYear(this DateTime date)
        {
            return DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
    }
}
