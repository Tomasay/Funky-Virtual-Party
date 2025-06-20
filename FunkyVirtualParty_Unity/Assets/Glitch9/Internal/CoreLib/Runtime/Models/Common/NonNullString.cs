using System;

namespace Glitch9
{
    public struct NonNullString
    {
        public NonNullString(string value) : this()
        {
            Value = value ?? "null";
        }

        public string Value
        {
            get;
            private set;
        }

        public static implicit operator NonNullString(string value)
        {
            return new NonNullString(value);
        }

        public static implicit operator string(NonNullString value)
        {
            return value.Value;
        }

        public static implicit operator NonNullString(Enum enumValue)
        {
            return new NonNullString(enumValue.ToString());
        }

        public static implicit operator NonNullString(int intValue)
        {
            return new NonNullString(intValue.ToString());
        }

        public override string ToString()
        {
            return Value;
        }
    }
}