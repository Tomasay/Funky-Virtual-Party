using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glitch9
{
    public class MinMax<T> where T : IComparable<T>
    {
        public static implicit operator T(MinMax<T> ranged) => ranged.Value;

        [JsonIgnore] private T _value;

        public T Min { get; }
        public T Max { get; }
        public T Value
        {
            get => _value;
            set
            {
                if (value.CompareTo(Min) < 0) _value = Min;
                else if (value.CompareTo(Max) > 0) _value = Max;
                else _value = value;
            }
        }

        public MinMax() { }
        public MinMax(T value, T min, T max)
        {
            if (min.CompareTo(max) > 0)
            {
                throw new ArgumentException($"'{nameof(min)}' cannot be greater than '{nameof(max)}'.");
            }

            Min = min;
            Max = max;
            _value = min; // 초기화를 Min 값으로 설정
            Value = value; // This ensures the value is within the bounds
        }

        public MinMax(T min, T max) : this(min, min, max) { } // 기본값을 min으로 설정

        // equal check
        public static bool operator ==(MinMax<T> ranged1, MinMax<T> ranged2)
            => ranged1 is null && ranged2 is null
               || ranged1 is not null && ranged1.Equals(ranged2);

        public static bool operator !=(MinMax<T> ranged1, MinMax<T> ranged2)
            => !(ranged1 == ranged2);

        public static bool operator <(MinMax<T> ranged1, MinMax<T> ranged2) => ranged1.Value.CompareTo(ranged2.Value) < 0;
        public static bool operator >(MinMax<T> ranged1, MinMax<T> ranged2) => ranged1.Value.CompareTo(ranged2.Value) > 0;
        public static bool operator <=(MinMax<T> ranged1, MinMax<T> ranged2) => ranged1.Value.CompareTo(ranged2.Value) <= 0;
        public static bool operator >=(MinMax<T> ranged1, MinMax<T> ranged2) => ranged1.Value.CompareTo(ranged2.Value) >= 0;

        protected bool Equals(MinMax<T> other)
        {
            return EqualityComparer<T>.Default.Equals(_value, other._value) && EqualityComparer<T>.Default.Equals(Min, other.Min) && EqualityComparer<T>.Default.Equals(Max, other.Max);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is MinMax<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Min, Max);
        }
    }


    public static class MinMaxExtensions
    {
        public static MinMax<int> Add(this MinMax<int> ranged, int value)
        {
            int newValue = ranged.Value + value;
            return new MinMax<int>(ranged.Min, ranged.Max, newValue);
        }

        public static MinMax<float> Add(this MinMax<float> ranged, float value)
        {
            float newValue = ranged.Value + value;
            return new MinMax<float>(ranged.Min, ranged.Max, newValue);
        }

        public static MinMax<double> Add(this MinMax<double> ranged, double value)
        {
            double newValue = ranged.Value + value;
            return new MinMax<double>(ranged.Min, ranged.Max, newValue);
        }

        public static MinMax<int> Subtract(this MinMax<int> ranged, int value)
        {
            int newValue = ranged.Value - value;
            return new MinMax<int>(ranged.Min, ranged.Max, newValue);
        }

        public static MinMax<float> Subtract(this MinMax<float> ranged, float value)
        {
            float newValue = ranged.Value - value;
            return new MinMax<float>(ranged.Min, ranged.Max, newValue);
        }

        public static MinMax<double> Subtract(this MinMax<double> ranged, double value)
        {
            double newValue = ranged.Value - value;
            return new MinMax<double>(ranged.Min, ranged.Max, newValue);
        }

        public static MinMax<int> Multiply(this MinMax<int> ranged, int value)
        {
            int newValue = ranged.Value * value;
            return new MinMax<int>(ranged.Min, ranged.Max, newValue);
        }

        public static MinMax<float> Multiply(this MinMax<float> ranged, float value)
        {
            float newValue = ranged.Value * value;
            return new MinMax<float>(ranged.Min, ranged.Max, newValue);
        }

        public static MinMax<double> Multiply(this MinMax<double> ranged, double value)
        {
            double newValue = ranged.Value * value;
            return new MinMax<double>(ranged.Min, ranged.Max, newValue);
        }

        public static MinMax<int> Divide(this MinMax<int> ranged, int value)
        {
            int newValue = ranged.Value / value;
            return new MinMax<int>(ranged.Min, ranged.Max, newValue);
        }

        public static MinMax<float> Divide(this MinMax<float> ranged, float value)
        {
            float newValue = ranged.Value / value;
            return new MinMax<float>(ranged.Min, ranged.Max, newValue);
        }

        public static MinMax<double> Divide(this MinMax<double> ranged, double value)
        {
            double newValue = ranged.Value / value;
            return new MinMax<double>(ranged.Min, ranged.Max, newValue);
        }
    }
}