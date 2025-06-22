using System;
using System.Collections.Generic;
using System.Globalization;

namespace Glitch9
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startingDay /* sunday or monday */)
        {
            int diff = (7 + (date.DayOfWeek - startingDay)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        public static int GetWeekOfMonth(this DateTime date)
        {
            DateTime beginningOfMonth = new(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        public static IEnumerable<DateTime> GetDatesUntil(this DateTime startDate, DateTime endDate)
        {
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                yield return date;
            }
        }

        public static DateTime Today(int hour, int min, DateTime dayChangeStandardTime = default)
        {
            // validate hour and min
            if (hour < 0) hour = 0;
            else if (hour > 23) hour = 23;

            if (min < 0) min = 0;
            else if (min > 59) min = 59;

            DateTime result = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, min, 0);

            if (dayChangeStandardTime != default)
            {
                bool dayChange = result < dayChangeStandardTime;
                if (dayChange) result = result.AddDays(1);
            }
            return result;
        }

        public static string GetInspectorName(this DateTime dateTime, CultureInfo cultureInfo = null)
        {
            if (dateTime == default)
            {
                return "None";
            }

            cultureInfo ??= CultureInfo.CurrentCulture;
            // 영상 촬영을 위해 무조건 US 문화권으로 표시
            //cultureInfo = new CultureInfo("en-US");

            // 오늘이면 "오늘 HH:mm tt"로 표시
            if (dateTime.Date == DateTime.Today)
            {
                return $"Today {dateTime.ToString("t", cultureInfo)}";
            }

            // 어제면 "어제 HH:mm tt"로 표시
            if (dateTime.Date == DateTime.Today.AddDays(-1))
            {
                return $"Yesterday {dateTime.ToString("t", cultureInfo)}";
            }

            // 올해면 yyyy를 생략하고 7월 4일 7:30 PM 형식으로 표시
            // 현재 문화권에 따라 날짜 형식을 지정
            // 한국: 7월 4일 오후 7:30
            // 미국: July 4, 7:30 PM
            // 일본: 7月4日 19:30
            if (dateTime.Year == DateTime.Today.Year)
            {
                //return dateTime.ToString("m t", cultureInfo);
                //return $"{dateTime:M} {dateTime:t}";
                return $"{dateTime.ToString("M", cultureInfo)} {dateTime.ToString("t", cultureInfo)}";
            }

            // 그 외의 경우 yyyy-MM-dd로 표시 (시간 생략)
            return dateTime.ToString("D", cultureInfo);
        }
    }
}