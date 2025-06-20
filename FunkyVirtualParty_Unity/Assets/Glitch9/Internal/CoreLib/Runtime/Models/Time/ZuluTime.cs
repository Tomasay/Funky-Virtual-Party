using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Glitch9
{
    [JsonConverter(typeof(ZuluTimeJsonConverter))]
    public readonly struct ZuluTime : IComparable<ZuluTime>, IEquatable<ZuluTime>
    {
        public static ZuluTime MinValue => new(DateTime.MinValue, 0);
        public DateTime DateTime { get; }
        public int Nanoseconds { get; }

        public ZuluTime(DateTime dateTime, int nanoseconds)
        {
            if (nanoseconds < 0 || nanoseconds > 999_999_999)
                throw new ArgumentOutOfRangeException(nameof(nanoseconds), "Nanoseconds must be between 0 and 999,999,999.");

            DateTime = dateTime;
            Nanoseconds = nanoseconds;
        }

        public override string ToString()
        {
            string dateTimeString = DateTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            string fractionalSeconds = Nanoseconds.ToString("D9").TrimEnd('0');

            return fractionalSeconds.Length > 0 ? $"{dateTimeString}.{fractionalSeconds}Z" : $"{dateTimeString}Z";
        }

        public static ZuluTime Parse(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            s = s.TrimEnd('Z');
            string dateTimePart = s;
            string nanosecondsPart = "0";

            int dotIndex = s.IndexOf('.');
            if (dotIndex >= 0)
            {
                dateTimePart = s.Substring(0, dotIndex);
                nanosecondsPart = s.Substring(dotIndex + 1);

                // 최대 9자리까지 자름
                if (nanosecondsPart.Length > 9)
                    nanosecondsPart = nanosecondsPart.Substring(0, 9);
            }


            DateTime dateTime = DateTime.ParseExact(dateTimePart, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            int nanoseconds = int.Parse(nanosecondsPart.PadRight(9, '0'));

            return new ZuluTime(dateTime, nanoseconds);
        }


        public int CompareTo(ZuluTime other)
        {
            int dateTimeComparison = DateTime.CompareTo(other.DateTime);
            if (dateTimeComparison != 0) return dateTimeComparison;
            return Nanoseconds.CompareTo(other.Nanoseconds);
        }

        public bool Equals(ZuluTime other)
        {
            return DateTime.Equals(other.DateTime) && Nanoseconds == other.Nanoseconds;
        }

        public override bool Equals(object obj)
        {
            return obj is ZuluTime other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DateTime.GetHashCode() * 397) ^ Nanoseconds;
            }
        }

        public static bool operator ==(ZuluTime left, ZuluTime right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ZuluTime left, ZuluTime right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(ZuluTime left, ZuluTime right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(ZuluTime left, ZuluTime right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(ZuluTime left, ZuluTime right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(ZuluTime left, ZuluTime right)
        {
            return left.CompareTo(right) >= 0;
        }

        public readonly ZuluTime AddSeconds(long seconds)
        {
            return new ZuluTime(DateTime.AddSeconds(seconds), Nanoseconds);
        }

        public readonly ZuluTime AddMinutes(long minutes)
        {
            return new ZuluTime(DateTime.AddMinutes(minutes), Nanoseconds);
        }

        public readonly ZuluTime AddHours(long hours)
        {
            return new ZuluTime(DateTime.AddHours(hours), Nanoseconds);
        }

        public readonly ZuluTime AddDays(long days)
        {
            return new ZuluTime(DateTime.AddDays(days), Nanoseconds);
        }

        public readonly TimeSpan Add(ZuluTime b)
        {
            return this + b;
        }

        public readonly TimeSpan Subtract(ZuluTime b)
        {
            return this - b;
        }

        public static TimeSpan operator +(ZuluTime a, ZuluTime b)
        {
            return a.DateTime - b.DateTime;
        }

        public static TimeSpan operator -(ZuluTime a, ZuluTime b)
        {
            return a.DateTime - b.DateTime;
        }
    }

    internal class ZuluTimeJsonConverter : JsonConverter<ZuluTime>
    {
        public override void WriteJson(JsonWriter writer, ZuluTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override ZuluTime ReadJson(JsonReader reader, Type objectType, ZuluTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default;

            if (reader.TokenType == JsonToken.String)
                return ZuluTime.Parse((string)reader.Value);

            if (reader.TokenType == JsonToken.Date)
                return new ZuluTime(((DateTime)reader.Value).ToUniversalTime(), 0);

            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing ZuluTime.");
        }

    }
}