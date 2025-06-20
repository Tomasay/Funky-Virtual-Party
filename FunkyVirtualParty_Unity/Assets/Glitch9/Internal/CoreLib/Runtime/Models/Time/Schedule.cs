using System;
using System.Collections.Generic;
using System.Text;

namespace Glitch9
{
    public enum ScheduleType
    {
        /// <summary>
        /// 월화수목금토일 중 선택
        /// Every 1 Week, Every 2 Weeks~
        /// </summary>
        Weekly,
        /// <summary>
        /// Every 1 Day (Everyday), Every 2 Days~
        /// </summary>
        Daily,
        /// <summary>
        /// Selected Days in the month (1~31)
        /// Every 1 Month, Every 2 Months~
        /// </summary>
        Monthly,
    }

    /// <summary>
    /// This is not thread-safe.
    /// Use ImmutableHashSet instead if you want to use this in a multi-threaded environment.
    /// </summary>
    public struct Schedule
    {
        /// <summary>
        /// Default
        /// </summary>
        public static Schedule Everyday => new(GetEveryday());
        private static IEnumerable<DayOfWeek> GetEveryday()
        {
            yield return DayOfWeek.Sunday;
            yield return DayOfWeek.Sunday;
            yield return DayOfWeek.Monday;
            yield return DayOfWeek.Tuesday;
            yield return DayOfWeek.Wednesday;
            yield return DayOfWeek.Thursday;
            yield return DayOfWeek.Friday;
            yield return DayOfWeek.Saturday;
        }

        public ScheduleType Type;

        private readonly HashSet<DayOfWeek> dayOfWeeks;
        private readonly HashSet<int> daysInMonth;

        /// <summary>
        /// Like every 2 days if interval is 2 and type is ScheduleType.Daily
        /// or every 2 weeks if interval is 2 and type is ScheduleType.Weekly
        /// </summary>
        public int Frequency;

        public Schedule(int interval = 1)
        {
            Type = ScheduleType.Daily;
            Frequency = interval;
            dayOfWeeks = new HashSet<DayOfWeek>();
            daysInMonth = new HashSet<int>();
        }

        public Schedule(IEnumerable<DayOfWeek> days, int interval = 1)
        {
            Type = ScheduleType.Weekly;
            Frequency = interval;
            dayOfWeeks = new HashSet<DayOfWeek>(days);
            daysInMonth = new HashSet<int>();
        }

        public Schedule(IEnumerable<int> days, int interval = 1)
        {
            Type = ScheduleType.Monthly;
            Frequency = interval;
            dayOfWeeks = new HashSet<DayOfWeek>();
            daysInMonth = new HashSet<int>(days);
        }

        public Schedule(long bitmask)
        {
            // Extract ScheduleType from the first 3 bits
            Type = (ScheduleType)(bitmask & 0b111);

            // Extract Frequency from the next 5 bits
            Frequency = (int)((bitmask >> 3) & 0b11111);

            dayOfWeeks = new HashSet<DayOfWeek>();
            daysInMonth = new HashSet<int>();

            if (Type == ScheduleType.Weekly)
            {
                // Decode days of the week from the bitmask (assuming bits 8-14 for weekly)
                for (int i = 0; i < 7; i++)
                {
                    if ((bitmask & (1L << (8 + i))) != 0)
                    {
                        dayOfWeeks.Add((DayOfWeek)i);
                    }
                }
            }
            else if (Type == ScheduleType.Monthly)
            {
                // Decode days in the month from the bitmask (assuming bits 15-45 for monthly)
                // Note: Adjusted starting bit position for monthly schedule decoding
                for (int i = 0; i < 31; i++)
                {
                    if ((bitmask & (1L << (15 + i))) != 0)
                    {
                        // Adjusted to add 1 to the day since daysInMonth should start from 1 to 31
                        daysInMonth.Add(i + 1);
                    }
                }
            }
            else
            {
                dayOfWeeks = new HashSet<DayOfWeek>();
                daysInMonth = new HashSet<int>();
            }
        }

        public void Set(params int[] days)
        {
            Type = ScheduleType.Monthly;

            if (days != null && days.Length > 0)
            {
                foreach (var day in days)
                {
                    daysInMonth.Add(day);
                }
            }
        }

        public void Remove(params int[] days)
        {
            Type = ScheduleType.Monthly;

            if (days != null && days.Length > 0)
            {
                foreach (var day in days)
                {
                    daysInMonth.Remove(day);
                }
            }
        }

        public readonly bool Contains(int dayInMonth)
        {
            return daysInMonth.Contains(dayInMonth);
        }

        public bool Toggle(int day)
        {
            if (daysInMonth.Contains(day))
            {
                Remove(day);
                return false;
            }
            else
            {
                Set(day);
                return true;
            }
        }

        public void Set(params DayOfWeek[] days)
        {
            Type = ScheduleType.Weekly;

            if (days != null && days.Length > 0)
            {
                foreach (var day in days)
                {
                    dayOfWeeks.Add(day);
                }
            }
        }

        public void Remove(params DayOfWeek[] days)
        {
            Type = ScheduleType.Weekly;

            if (days != null && days.Length > 0)
            {
                foreach (var day in days)
                {
                    dayOfWeeks.Remove(day);
                }
            }
        }

        public readonly bool Contains(DayOfWeek day)
        {
            return dayOfWeeks.Contains(day);
        }

        public bool Toggle(DayOfWeek day)
        {
            if (dayOfWeeks.Contains(day))
            {
                Remove(day);
                return false;
            }
            else
            {
                Set(day);
                return true;
            }
        }

        public readonly long ToBitMask()
        {
            long bitmask = 0;

            // Assuming ScheduleType is an enum with Daily = 0, Weekly = 1, Monthly = 2
            bitmask |= (long)Type; // First 3 bits for Type
            bitmask |= ((long)Frequency << 3); // Next 5 bits for Frequency

            if (Type == ScheduleType.Weekly)
            {
                foreach (var day in dayOfWeeks)
                {
                    bitmask |= (1L << (8 + (int)day)); // Encode days into bits 8-14
                }
            }
            // Implement similar logic for monthly schedules, acknowledging the 31-bit requirement

            return bitmask;
        }

        public static Schedule Create(string days)
        {
            Schedule scheduler = new();
            for (int i = 0; i < 7; i++)
            {
                int value = 1;
                if (days.Length > i)
                {
                    int.TryParse(days[i].ToString(), out value);
                }

                if (value == 1)
                {
                    scheduler.Set((DayOfWeek)i);
                }
            }

            return scheduler;
        }

        public readonly string GetDescription()
        {
            string unset = "Unset";

            if (Type == ScheduleType.Weekly)
            {
                if (dayOfWeeks.IsNullOrEmpty()) return unset;
                int count = dayOfWeeks.Count;

                if (count == 7) return "Everyday";
                else if (count == 2
                    && dayOfWeeks.Contains(DayOfWeek.Saturday)
                    && dayOfWeeks.Contains(DayOfWeek.Sunday))
                    return "Weekends";

                else if (count == 5
                    && dayOfWeeks.Contains(DayOfWeek.Monday)
                    && dayOfWeeks.Contains(DayOfWeek.Tuesday)
                    && dayOfWeeks.Contains(DayOfWeek.Wednesday)
                    && dayOfWeeks.Contains(DayOfWeek.Thursday)
                    && dayOfWeeks.Contains(DayOfWeek.Friday))
                    return "Weekdays";


                // it has to start from Sunday, and localized
                string[] dayofweek = new string[7] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

                string result = "";
                foreach (var day in dayOfWeeks)
                {
                    //TODO: Localize differently
                    //result += $"builtin.{dayofweek[(int)day].Localize()}, ";
                }

                result = result.Substring(0, result.Length - 2);
                return result;
            }
            else if (Type == ScheduleType.Daily)
            {
                if (Frequency == 1) return "Everyday";

                var sb = new StringBuilder();
                sb.Append("Every");
                sb.Append(Frequency);
                string day = Frequency == 1 ? "Day" : "Days";
                sb.Append(day);
                return sb.ToString();
            }
            else if (Type == ScheduleType.Monthly)
            {
                int count = daysInMonth.Count;
                if (count == 31) return "Everyday";

                var sb = new StringBuilder();
                sb.Append(count);
                sb.Append("Days ");
                sb.Append("Every");
                sb.Append(Frequency);
                string month = Frequency == 1 ? "Month" : "Months";
                sb.Append(month);
                return sb.ToString();
            }

            return unset;
        }
    }
}