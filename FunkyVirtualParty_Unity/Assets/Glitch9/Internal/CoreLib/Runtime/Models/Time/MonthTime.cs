using System;
using System.Collections.Generic;

namespace Glitch9
{
    public struct MonthTime
    {
        private int _year;
        private int _month;
        public int Year
        {
            get
            {
                if (_year < 2023 || _year > DateTime.Now.Year) _year = DateTime.Now.Year;
                return _year;
            }
            set => _year = value;
        }

        public int Month
        {
            get
            {
                if (_month == 0 || _month > 12) _month = DateTime.Now.Month;
                return _month;
            }
            set => _month = value;
        }

        public DateTime FirstDay
        {
            get
            {
                DateTime result = new(Year, Month, 1);
                return result;
            }
        }

        public DateTime ToDateTime() => new(Year, Month, 1);
        public static MonthTime Now => new(DateTime.Now.Year, DateTime.Now.Month);
        public MonthTime Previous => AddMonths(-1);
        public MonthTime Next => AddMonths(1);


        // AddMonths
        public MonthTime AddMonths(int months)
        {
            DateTime dateTime = new DateTime(Year, Month, 1);
            dateTime = dateTime.AddMonths(months);
            return new MonthTime(dateTime);
        }

        public MonthTime(int year, int month)
        {
            _year = year;
            _month = month;
        }

        public MonthTime(DateTime dateTime)
        {
            _year = dateTime.Year;
            _month = dateTime.Month;
        }

        public static bool operator ==(MonthTime left, MonthTime right)
        {
            if (ReferenceEquals(left, right))
                return true;

            return left.Year == right.Year && left.Month == right.Month;
        }

        // not equal
        public static bool operator !=(MonthTime left, MonthTime right)
        {
            if (ReferenceEquals(left, right))
                return false;

            return left.Year != right.Year || left.Month != right.Month;
        }

        // less than
        public static bool operator <(MonthTime left, MonthTime right)
        {
            if (left.Year < right.Year) return true;
            if (left.Year > right.Year) return false;
            return left.Month < right.Month;
        }

        // greater than
        public static bool operator >(MonthTime left, MonthTime right)
        {
            if (left.Year > right.Year) return true;
            if (left.Year < right.Year) return false;
            return left.Month > right.Month;
        }

        // less than or equal
        public static bool operator <=(MonthTime left, MonthTime right)
        {
            if (left.Year < right.Year) return true;
            if (left.Year > right.Year) return false;
            return left.Month <= right.Month;
        }

        // greater than or equal
        public static bool operator >=(MonthTime left, MonthTime right)
        {
            if (left.Year > right.Year) return true;
            if (left.Year < right.Year) return false;
            return left.Month >= right.Month;
        }

        public override bool Equals(object obj)
        {
            if (obj is MonthTime other)
            {
                return this.Year == other.Year && this.Month == other.Month;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Year * 100 + Month;
        }

        /// <summary>
        /// Firestore 서버에서 사용하는 포멧으로 반환합니다.
        /// </summary>
        public override string ToString()
        {
            return $"{Year:0000}-{Month:00}";
        }

        public IEnumerable<DateTime> GetAllDays()
        {
            DateTime startDate = new(Year, Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                yield return date;
            }
        }

        public List<DateTime> GetAllCalendarDays(CalendarStartDay startDay)
        {
            // 위에꺼랑 다른건 이건 캘린더에 표시되는 이전 달/다음 달의 날짜도 포함합니다.
            bool isSundayFirst = startDay == CalendarStartDay.Sunday; // otherwise Monday
            DateTime startDate = new(Year, Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            DayOfWeek startDayOfWeek = startDate.DayOfWeek;
            int thisMonthStartingIndex = isSundayFirst ? (int)startDayOfWeek : (int)startDayOfWeek - 1;
            int nextMonthStartingIndex = isSundayFirst ? 7 - (int)endDate.DayOfWeek : 6 - (int)endDate.DayOfWeek;

            DateTime firstDate = startDate.AddDays(-thisMonthStartingIndex);
            DateTime lastDate = endDate.AddDays(nextMonthStartingIndex);

            List<DateTime> result = new();
            for (DateTime date = firstDate; date <= lastDate; date = date.AddDays(1))
            {
                result.Add(date);
            }

            return result;
        }

        public DateTime GetStartDate()
        {
            return new DateTime(Year, Month, 1);
        }

        public DateTime GetEndDate()
        {
            return GetStartDate().AddMonths(1).AddDays(-1);
        }
    }
}