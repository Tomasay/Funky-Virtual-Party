using System;
using UnityEngine;

namespace Glitch9
{
    [Serializable]
    public struct ClockTime : IComparable
    {
        // 0 = 0:00
        // 130 = 1:30
        // 1820 = 18:20
        // 2400 => (invalid) => 0으로 처리

        public static ClockTime Now => DateTime.Now;
        public static ClockTime Zero => new(0);
        public static ClockTime Midnight => new(0);
        public static ClockTime Noon => new(12, 0);
        public static ClockTime Morning => new(6, 0);
        public static ClockTime Afternoon => new(12, 0);
        public static ClockTime Evening => new(18, 0);
        public static ClockTime Night => new(22, 0);
        public static ClockTime Dawn => new(4, 0);


        public int Value;
        public bool Immutable;

        [SerializeField] private int _hour;
        [SerializeField] private int _minute;

        private void UpdateValue()
        {
            Value = _hour * 100 + _minute;
        }

        public int Hour
        {
            readonly get => _hour;
            set => SetHour(value);
        }

        public int Minute
        {
            readonly get => _minute;
            set => SetMinute(value);
        }

        public readonly int TotalMinutes => Hour * 60 + Minute;

        private void SetHour(int hour)
        {
            if (hour > 23) _hour = hour - 24;
            else if (hour < 0) _hour = hour + 24;
            else _hour = hour;
            UpdateValue();
        }

        private void SetMinute(int minute)
        {
            if (minute > 59) _minute = minute - 60;
            else if (minute < 0) _minute = minute + 60;
            else _minute = minute;
            UpdateValue();
        }

        private void Validate()
        {
            if (_hour > 23) _hour = 0;
            if (_minute > 59) _minute = 0;
            UpdateValue();
        }

        public ClockTime(int time, bool immutable = false)
        {
            _hour = time / 100;
            _minute = time % 100;
            Value = 0;
            Immutable = immutable;
            Validate();
        }

        public ClockTime(int hour, int minute, bool immutable = false)
        {
            _hour = hour;
            _minute = minute;
            Value = 0;
            Immutable = immutable;
            Validate();
        }

        public ClockTime(ClockTime clockTime, bool immutable = false)
        {
            _hour = clockTime.Hour;
            _minute = clockTime.Minute;
            Value = 0;
            Immutable = immutable;
            Validate();
        }

        public ClockTime(DateTime dateTime, bool immutable = false)
        {
            _hour = dateTime.Hour;
            _minute = dateTime.Minute;
            Value = 0;
            Immutable = immutable;
            Validate();
        }

        public readonly DateTime ToDateTime()
        {
            var result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Hour, Minute, 0);
            if (Hour > 23) result = result.AddDays(1);
            if (result < DateTime.Now) result = result.AddDays(1);
            return result;
        }

        public ClockTime AddHours(int hours)
        {
            Hour += hours;
            return this;
        }

        public ClockTime AddMinutes(int minutes)
        {
            Minute += minutes;
            return this;
        }

        public readonly string ToServerString() => Value.ToString("0000");
        public readonly override string ToString() => $"{Hour.ToString("00")}:{Minute.ToString("00")} {((Hour < 12) ? "AM" : "PM")}";
        public readonly string ToString(string format)
        {
            if (format == "hh:mm") return ToString();
            else if (format == "hh:mm tt") return $"{Hour.ToString("00")}:{Minute.ToString("00")} {((Hour < 12) ? "AM" : "PM")}";
            else return ToString();
        }

        public static bool TryParse(string time, out ClockTime result)
        {
            if (int.TryParse(time, out int value))
            {
                result = new ClockTime(value);
                return true;
            }
            result = Zero;
            return false;
        }

        // 형변환
        public static explicit operator DateTime(ClockTime time)
        {
            return time.ToDateTime();
        }
        public static implicit operator int(ClockTime time)
        {
            return time.Value;
        }
        public static implicit operator ClockTime(int time) => new(time);
        public static implicit operator ClockTime(DateTime dateTime) => new(dateTime);

        // 비교
        public readonly bool Equals(ClockTime other)
        {
            return Value == other.Value;
        }
        public readonly override bool Equals(object obj)
        {
            return obj switch
            {
                null => false,
                ClockTime other => Equals(other),
                _ => false
            };
        }
        public readonly override int GetHashCode() => Value.GetHashCode();

        public readonly int CompareTo(object obj)
        {
            if (obj is null) return 1;
            if (obj is ClockTime other) return Value.CompareTo(other.Value);
            throw new ArgumentException("Object is not a ClockTime");
        }

        public static bool operator ==(ClockTime a, ClockTime b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(ClockTime a, ClockTime b)
        {
            return !a.Equals(b);
        }
        public static bool operator <(ClockTime a, ClockTime b)
        {
            return a.Value < b.Value;
        }
        public static bool operator >(ClockTime a, ClockTime b)
        {
            return a.Value > b.Value;
        }
        public static bool operator <=(ClockTime a, ClockTime b)
        {
            return a.Value <= b.Value;
        }
        public static bool operator >=(ClockTime a, ClockTime b)
        {
            return a.Value >= b.Value;
        }


        // 추가, 빼기
        public static TimeSpan operator -(ClockTime a, ClockTime b)
        {
            DateTime aDateTime = a.ToDateTime();
            DateTime bDateTime = b.ToDateTime();
            return aDateTime - bDateTime;
        }

        public static ClockTime operator +(ClockTime a, TimeSpan b)
        {
            DateTime aDateTime = a.ToDateTime();
            DateTime bDateTime = aDateTime + b;
            return new ClockTime(bDateTime);
        }
    }
}