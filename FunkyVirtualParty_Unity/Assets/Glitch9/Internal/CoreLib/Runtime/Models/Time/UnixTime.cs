using System;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9
{
    /// <summary>
    /// A long value that acts as a DateTime object.
    /// (DateTime을 가장한 long 값)
    /// </summary>
    [Serializable, JsonConverter(typeof(UnixTimeJsonConverter))]
    public struct UnixTime : IComparable<UnixTime>, IEquatable<UnixTime>
    {
        public static readonly DateTime UnixEpoch = new(1970, 1, 1);
        public static UnixTime Empty => new();
        public static UnixTime Default => Now;
        public static UnixTime Now => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        public static UnixTime Today => new DateTimeOffset(DateTime.UtcNow.Date).ToUnixTimeSeconds();
        public static UnixTime MinValue => new(UnixEpoch);
        public static UnixTime MaxValue => new(long.MaxValue);
        public readonly bool IsEmpty => _value == 0;

        public long Value
        {
            get => _value;
            private set => _value = value;
        }

        [SerializeField] private long _value;

        public UnixTime(DateTime dateTime, bool toUniversalTime = true)
        {
            if (toUniversalTime)
            {
                _value = (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
            }
            else
            {
                _value = (long)(dateTime - UnixEpoch).TotalSeconds;
            }
        }

        public UnixTime(long unixTimestamp)
        {
            _value = unixTimestamp;
        }

        public UnixTime(int year, int month, int day, int hour, int minute, int second)
        {
            _value = (long)(new DateTime(year, month, day, hour, minute, second) - UnixEpoch).TotalSeconds;
        }

        public UnixTime(int year, int month, int day)
        {
            _value = (long)(new DateTime(year, month, day, 0, 0, 0) - UnixEpoch).TotalSeconds;
        }

        public UnixTime(string dateAsString)
        {
            _value = (long)(DateTime.Parse(dateAsString) - UnixEpoch).TotalSeconds;
        }

        public static explicit operator DateTime(UnixTime unixTime)
        {
            return UnixEpoch.AddSeconds(unixTime.Value);
        }

        public static implicit operator UnixTime(DateTime dateTime)
        {
            return new UnixTime(dateTime);
        }

        public static implicit operator UnixTime(long unixTimestamp)
        {
            return new UnixTime(unixTimestamp);
        }

        public static implicit operator long(UnixTime unixTime)
        {
            return unixTime.Value;
        }

        public readonly int Year => ((DateTime)this).Year;
        public readonly int Month => ((DateTime)this).Month;
        public readonly int Day => ((DateTime)this).Day;
        public readonly int Hour => ((DateTime)this).Hour;
        public readonly int Minute => ((DateTime)this).Minute;
        public readonly int Second => ((DateTime)this).Second;

        public UnixTime AddSeconds(long seconds)
        {
            return _value += seconds;
        }

        public readonly UnixTime AddMinutes(long minutes)
        {
            return _value + minutes * 60;
        }

        public readonly UnixTime AddHours(long hours)
        {
            return _value + hours * 3600;
        }

        public readonly UnixTime AddDays(long days)
        {
            return _value + days * 86400;
        }

        public readonly TimeSpan Add(UnixTime b)
        {
            return this + b;
        }

        public readonly TimeSpan Subtract(UnixTime b)
        {
            return this - b;
        }

        // 비교하는 것들
        public static bool operator ==(UnixTime a, UnixTime b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(UnixTime a, UnixTime b)
        {
            return a.Value != b.Value;
        }

        public static bool operator >(UnixTime a, UnixTime b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(UnixTime a, UnixTime b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(UnixTime a, UnixTime b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(UnixTime a, UnixTime b)
        {

            return a.Value <= b.Value;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is UnixTime time &&
                   _value == time.Value;
        }

        public readonly override int GetHashCode()
        {
            return -1937169414 + _value.GetHashCode();
        }

        public static TimeSpan operator +(UnixTime a, UnixTime b)
        {
            return TimeSpan.FromSeconds(a.Value + b.Value);
        }

        public static TimeSpan operator -(UnixTime a, UnixTime b)
        {
            return TimeSpan.FromSeconds(a.Value - b.Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public readonly string GetInspectorName(CultureInfo cultureInfo = null)
        {
            return ((DateTime)this).GetInspectorName(cultureInfo);
        }

        public readonly string ToString(string format)
        {
            return ((DateTime)this).ToString(format);
        }

        public readonly DateTime ToDateTime()
        {
            return (DateTime)this;
        }

        public readonly DateTime ToLocalTime()
        {
            return ((DateTime)this).ToLocalTime();
        }

        public readonly UnixTime Date => ((DateTime)this).Date;

        public readonly int CompareTo(UnixTime other)
        {
            return _value.CompareTo(other.Value);
        }

        public readonly int CompareTo(DateTime other)
        {
            return ((DateTime)this).CompareTo(other);
        }

        public readonly int CompareTo(long other)
        {
            return _value.CompareTo(other);
        }

        public static int Compare(UnixTime a, UnixTime b)
        {
            return a.CompareTo(b);
        }

        public readonly bool Equals(UnixTime other)
        {
            return _value == other.Value;
        }
    }

    internal class UnixTimeJsonConverter : JsonConverter<UnixTime>
    {
        public override UnixTime ReadJson(JsonReader reader, Type objectType, UnixTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                long unixTime = long.Parse((string)reader.Value ?? $"{UnixTime.MinValue}");
                return new UnixTime(unixTime);
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                // Directly pass the long value to UnixTime constructor
                if (reader.Value != null) return new UnixTime((long)reader.Value);
            }
            else if (reader.TokenType == JsonToken.Date)
            {
                // Convert the DateTime to UnixTime
                if (reader.Value != null)
                {
                    DateTime dateTime = (DateTime)reader.Value;
                    return new UnixTime(dateTime);
                }
            }
            else if (reader.TokenType == JsonToken.Float)
            {
                // Convert the double to UnixTime
                if (reader.Value != null)
                {
                    double unixTime = (double)reader.Value;
                    return new UnixTime((long)unixTime);
                }
            }
            else if (reader.TokenType == JsonToken.Null)
            {
                return UnixTime.MinValue;
            }

            throw new JsonException($"Failed to parse UnixTime from {reader.TokenType}");
        }

        public override void WriteJson(JsonWriter writer, UnixTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Value);
        }
    }
}
