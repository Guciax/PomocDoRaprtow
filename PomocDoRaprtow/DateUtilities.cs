using System;
using System.Globalization;

namespace PomocDoRaprtow
{
    public static class DateUtilities
    {
        public struct ShiftInfo
        {
            public ShiftInfo(int shiftNo, int month, int dayOfTheMonth)
            {
                ShiftNo = shiftNo;
                Month = month;
                DayOfTheMonth = dayOfTheMonth;
            }

            public int ShiftNo { get; }
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
                return new ShiftInfo(3, fixedDate.Month, fixedDate.Day);
            }
            if (inputDate.Hour >= 14) return new ShiftInfo(2, inputDate.Month, inputDate.Day);
            if (inputDate.Hour >= 6) return new ShiftInfo(1, inputDate.Month, inputDate.Day);
            return new ShiftInfo(3, inputDate.Month, inputDate.Day);
        }

        public static DateTime ParseExact(String date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
        }
    }
}