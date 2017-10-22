using System;
using System.Diagnostics;
using System.Globalization;

namespace PomocDoRaprtow
{
    public static class DateUtilities
    {
        public struct ShiftInfo
        {
            public ShiftInfo(int shiftNo, DateTime date)
            {
                ShiftNo = shiftNo;
                Date = date;
                Month = date.Month;
                DayOfTheMonth = date.Day;
            }

            public int ShiftNo { get; }
            public DateTime Date { get; }
            public int Month { get; }
            public int DayOfTheMonth { get; }
        }
        public static int GetRealWeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime FixedShiftDate(DateTime inputDate)
        {
            if (inputDate.Hour >= 22)
            {
                return inputDate.AddDays(1);

            }
            else return inputDate;
        }

        public static ShiftInfo DateToShiftInfo(DateTime inputDate)
        {
            if (inputDate.Hour >= 22)
            {
                var fixedDate = inputDate.AddDays(1);
                return new ShiftInfo(3, fixedDate);
            }
            if (inputDate.Hour >= 14) return new ShiftInfo(2, inputDate);
            if (inputDate.Hour >= 6) return new ShiftInfo(1, inputDate);
            return new ShiftInfo(3, inputDate);
        }

        public static DateTime ParseExact(string date)
        {
            try
            {
                if (date.Contains("-"))
                    return DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
                else
                    return DateTime.ParseExact(date, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
            }
            catch(Exception e)
            {
                Debug.WriteLine("Date error: "+date);
                return new DateTime(1900, 1, 1);
            }
        }

        public static DateTime ParseExactWithFraction(String date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
        }
    }
}