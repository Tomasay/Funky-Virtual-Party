using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace Glitch9
{
    [Serializable, JsonConverter(typeof(CurrencyJsonConverter))]
    public class Currency : IComparable
    {
        [SerializeField] private double priceInUsd;
        [SerializeField] private CurrencyCode currencyCode;
        [SerializeField] private bool isEstimate;

        public double PriceInUsd
        {
            get => priceInUsd;
            set => priceInUsd = value;
        }

        public CurrencyCode CurrencyCode
        {
            get => currencyCode;
            set
            {
                if (currencyCode == value) return;
                currencyCode = value;

                if (_value == 0) return;
                ApplyRate();
            }
        }

        public bool IsEstimate
        {
            get => isEstimate;
            set => isEstimate = value;
        }

        public double Value
        {
            get
            {
                if (_value == null) ApplyRate();
                return _value ?? priceInUsd; // ✅ fallback
            }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_value == value) return;
                if (currencyCode == CurrencyCode.USD)
                {
                    priceInUsd = value;
                    _value = value;
                    return;
                }

                CurrencyExchange currencyConverter = new();
                priceInUsd = currencyConverter.Convert(value, currencyCode, CurrencyCode.USD);
                _value = value;
            }
        }
        private double? _value;

        public Currency()
        {
            CurrencyCode = CurrencyCode.USD;
        }

        public Currency(double priceInUsd, CurrencyCode currencyCode = CurrencyCode.USD, bool isEstimate = false)
        {
            this.priceInUsd = priceInUsd;
            this.currencyCode = currencyCode;
            this.isEstimate = isEstimate;
        }

        private void ApplyRate()
        {
            if (currencyCode == CurrencyCode.USD)
            {
                _value = priceInUsd;
                return;
            }

            CurrencyExchange currencyConverter = new();
            _value = currencyConverter.Convert(priceInUsd, CurrencyCode.USD, currencyCode);
        }

        // operators
        public static implicit operator double(Currency price) => price?.priceInUsd ?? 0;
        public static implicit operator Currency(double value) => new(value);
        public static implicit operator Currency(int value) => new(value);
        public static implicit operator Currency(float value) => new((double)value);
        public static implicit operator Currency(decimal value) => new((double)value);
        public static implicit operator (double value, bool isEstimate)(Currency price) => (price?.priceInUsd ?? 0, price?.isEstimate ?? false);

        public string ToString(string arg = null) => ToString(currencyCode, arg);
        public string ToString(CurrencyCode code, string arg = null)
        {
            string symbol = CurrencyUtil.GetSymbol(code);

            double convertedPrice;

            if (code == CurrencyCode.USD)
            {
                convertedPrice = priceInUsd;
            }
            else
            {
                CurrencyExchange currencyConverter = new();
                convertedPrice = currencyConverter.Convert(priceInUsd, CurrencyCode.USD, code);
            }
            arg ??= "0.00000000";
            string formattedPrice = convertedPrice.ToString(arg);

            return isEstimate ? $"{symbol}{formattedPrice} (estimate)" : $"{symbol}{formattedPrice}";
        }

        public int CompareTo(object obj) => obj is Currency price ? priceInUsd.CompareTo(price?.priceInUsd ?? 0) : priceInUsd.CompareTo(obj);

        public static Currency operator +(Currency a, Currency b) => new(a?.priceInUsd ?? 0 + b?.priceInUsd ?? 0);
        public static Currency operator -(Currency a, Currency b) => new(a?.priceInUsd ?? 0 - b?.priceInUsd ?? 0);
        public static Currency operator *(Currency a, Currency b) => new(a?.priceInUsd ?? 0 * b?.priceInUsd ?? 0);
        public static Currency operator /(Currency a, Currency b) => new(a?.priceInUsd ?? 0 / b?.priceInUsd ?? 0);
        public static Currency operator %(Currency a, Currency b) => new(a?.priceInUsd ?? 0 % b?.priceInUsd ?? 0);

        public static bool operator ==(Currency a, Currency b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return Math.Abs(a.priceInUsd - b.priceInUsd) < Tolerance.FLOAT && a.isEstimate == b.isEstimate;
        }

        public static bool operator !=(Currency a, Currency b) => !(a == b);
        public static bool operator >(Currency a, Currency b) => (a?.priceInUsd ?? 0) > (b?.priceInUsd ?? 0);
        public static bool operator <(Currency a, Currency b) => (a?.priceInUsd ?? 0) < (b?.priceInUsd ?? 0);
        public static bool operator >=(Currency a, Currency b) => (a?.priceInUsd ?? 0) >= (b?.priceInUsd ?? 0);
        public static bool operator <=(Currency a, Currency b) => (a?.priceInUsd ?? 0) <= (b?.priceInUsd ?? 0);

        public override bool Equals(object obj) => obj is Currency price && this == price;
        public override int GetHashCode() => HashCode.Combine(priceInUsd, isEstimate);

        #region double 
        public static Currency operator +(Currency a, double b) => new((a?.priceInUsd ?? 0) + b);
        public static Currency operator -(Currency a, double b) => new((a?.priceInUsd ?? 0) - b);
        public static Currency operator *(Currency a, double b) => new((a?.priceInUsd ?? 0) * b);
        public static Currency operator /(Currency a, double b) => new((a?.priceInUsd ?? 0) / b);
        public static Currency operator %(Currency a, double b) => new((a?.priceInUsd ?? 0) % b);

        public static bool operator ==(Currency a, double b) => Math.Abs((a?.priceInUsd ?? 0) - b) < Tolerance.FLOAT;
        public static bool operator !=(Currency a, double b) => Math.Abs((a?.priceInUsd ?? 0) - b) > Tolerance.FLOAT;
        public static bool operator >(Currency a, double b) => (a?.priceInUsd ?? 0) > b;
        public static bool operator <(Currency a, double b) => (a?.priceInUsd ?? 0) < b;
        public static bool operator >=(Currency a, double b) => (a?.priceInUsd ?? 0) >= b;
        public static bool operator <=(Currency a, double b) => (a?.priceInUsd ?? 0) <= b;

        public static Currency operator +(double a, Currency b) => new(a + (b?.priceInUsd ?? 0));
        public static Currency operator -(double a, Currency b) => new(a - (b?.priceInUsd ?? 0));
        public static Currency operator *(double a, Currency b) => new(a * (b?.priceInUsd ?? 0));
        public static Currency operator /(double a, Currency b) => new(a / (b?.priceInUsd ?? 0));
        public static Currency operator %(double a, Currency b) => new(a % (b?.priceInUsd ?? 0));

        public static bool operator ==(double a, Currency b) => Math.Abs(a - (b?.priceInUsd ?? 0)) < Tolerance.FLOAT;
        public static bool operator !=(double a, Currency b) => Math.Abs(a - (b?.priceInUsd ?? 0)) > Tolerance.FLOAT;
        public static bool operator >(double a, Currency b) => a > (b?.priceInUsd ?? 0);
        public static bool operator <(double a, Currency b) => a < (b?.priceInUsd ?? 0);
        public static bool operator >=(double a, Currency b) => a >= (b?.priceInUsd ?? 0);
        public static bool operator <=(double a, Currency b) => a <= (b?.priceInUsd ?? 0);
        #endregion

        #region int
        public static Currency operator +(Currency a, int b) => new((a?.priceInUsd ?? 0) + b);
        public static Currency operator -(Currency a, int b) => new((a?.priceInUsd ?? 0) - b);
        public static Currency operator *(Currency a, int b) => new((a?.priceInUsd ?? 0) * b);
        public static Currency operator /(Currency a, int b) => new((a?.priceInUsd ?? 0) / b);
        public static Currency operator %(Currency a, int b) => new((a?.priceInUsd ?? 0) % b);

        public static bool operator ==(Currency a, int b) => Math.Abs((a?.priceInUsd ?? 0) - b) < Tolerance.FLOAT;
        public static bool operator !=(Currency a, int b) => Math.Abs((a?.priceInUsd ?? 0) - b) > Tolerance.FLOAT;
        public static bool operator >(Currency a, int b) => (a?.priceInUsd ?? 0) > b;
        public static bool operator <(Currency a, int b) => (a?.priceInUsd ?? 0) < b;
        public static bool operator >=(Currency a, int b) => (a?.priceInUsd ?? 0) >= b;
        public static bool operator <=(Currency a, int b) => (a?.priceInUsd ?? 0) <= b;

        public static Currency operator +(int a, Currency b) => new(a + (b?.priceInUsd ?? 0));
        public static Currency operator -(int a, Currency b) => new(a - (b?.priceInUsd ?? 0));
        public static Currency operator *(int a, Currency b) => new(a * (b?.priceInUsd ?? 0));
        public static Currency operator /(int a, Currency b) => new(a / (b?.priceInUsd ?? 0));
        public static Currency operator %(int a, Currency b) => new(a % (b?.priceInUsd ?? 0));

        public static bool operator ==(int a, Currency b) => Math.Abs(a - (b?.priceInUsd ?? 0)) < Tolerance.FLOAT;
        public static bool operator !=(int a, Currency b) => Math.Abs(a - (b?.priceInUsd ?? 0)) > Tolerance.FLOAT;
        public static bool operator >(int a, Currency b) => a > (b?.priceInUsd ?? 0);
        public static bool operator <(int a, Currency b) => a < (b?.priceInUsd ?? 0);
        public static bool operator >=(int a, Currency b) => a >= (b?.priceInUsd ?? 0);
        public static bool operator <=(int a, Currency b) => a <= (b?.priceInUsd ?? 0);
        #endregion

        #region float
        public static Currency operator +(Currency a, float b) => new((a?.priceInUsd ?? 0) + (double)b);
        public static Currency operator -(Currency a, float b) => new((a?.priceInUsd ?? 0) - (double)b);
        public static Currency operator *(Currency a, float b) => new((a?.priceInUsd ?? 0) * (double)b);
        public static Currency operator /(Currency a, float b) => new((a?.priceInUsd ?? 0) / (double)b);
        public static Currency operator %(Currency a, float b) => new((a?.priceInUsd ?? 0) % (double)b);

        public static bool operator ==(Currency a, float b) => Math.Abs((a?.priceInUsd ?? 0) - (double)b) < Tolerance.FLOAT;
        public static bool operator !=(Currency a, float b) => Math.Abs((a?.priceInUsd ?? 0) - (double)b) > Tolerance.FLOAT;
        public static bool operator >(Currency a, float b) => (a?.priceInUsd ?? 0) > (double)b;
        public static bool operator <(Currency a, float b) => (a?.priceInUsd ?? 0) < (double)b;
        public static bool operator >=(Currency a, float b) => (a?.priceInUsd ?? 0) >= (double)b;
        public static bool operator <=(Currency a, float b) => (a?.priceInUsd ?? 0) <= (double)b;

        public static Currency operator +(float a, Currency b) => new((double)a + (b?.priceInUsd ?? 0));
        public static Currency operator -(float a, Currency b) => new((double)a - (b?.priceInUsd ?? 0));
        public static Currency operator *(float a, Currency b) => new((double)a * (b?.priceInUsd ?? 0));
        public static Currency operator /(float a, Currency b) => new((double)a / (b?.priceInUsd ?? 0));
        public static Currency operator %(float a, Currency b) => new((double)a % (b?.priceInUsd ?? 0));

        public static bool operator ==(float a, Currency b) => Math.Abs((double)a - (b?.priceInUsd ?? 0)) < Tolerance.FLOAT;
        public static bool operator !=(float a, Currency b) => Math.Abs((double)a - (b?.priceInUsd ?? 0)) > Tolerance.FLOAT;
        public static bool operator >(float a, Currency b) => (double)a > (b?.priceInUsd ?? 0);
        public static bool operator <(float a, Currency b) => (double)a < (b?.priceInUsd ?? 0);
        public static bool operator >=(float a, Currency b) => (double)a >= (b?.priceInUsd ?? 0);
        public static bool operator <=(float a, Currency b) => (double)a <= (b?.priceInUsd ?? 0);

        #endregion

        #region decimal
        public static Currency operator +(Currency a, decimal b) => new((a?.priceInUsd ?? 0) + (double)b);
        public static Currency operator -(Currency a, decimal b) => new((a?.priceInUsd ?? 0) - (double)b);
        public static Currency operator *(Currency a, decimal b) => new((a?.priceInUsd ?? 0) * (double)b);
        public static Currency operator /(Currency a, decimal b) => new((a?.priceInUsd ?? 0) / (double)b);
        public static Currency operator %(Currency a, decimal b) => new((a?.priceInUsd ?? 0) % (double)b);

        public static bool operator ==(Currency a, decimal b) => Math.Abs((a?.priceInUsd ?? 0) - (double)b) < Tolerance.FLOAT;
        public static bool operator !=(Currency a, decimal b) => Math.Abs((a?.priceInUsd ?? 0) - (double)b) > Tolerance.FLOAT;
        public static bool operator >(Currency a, decimal b) => (a?.priceInUsd ?? 0) > (double)b;
        public static bool operator <(Currency a, decimal b) => (a?.priceInUsd ?? 0) < (double)b;
        public static bool operator >=(Currency a, decimal b) => (a?.priceInUsd ?? 0) >= (double)b;
        public static bool operator <=(Currency a, decimal b) => (a?.priceInUsd ?? 0) <= (double)b;

        public static Currency operator +(decimal a, Currency b) => new((double)a + (b?.priceInUsd ?? 0));
        public static Currency operator -(decimal a, Currency b) => new((double)a - (b?.priceInUsd ?? 0));
        public static Currency operator *(decimal a, Currency b) => new((double)a * (b?.priceInUsd ?? 0));
        public static Currency operator /(decimal a, Currency b) => new((double)a / (b?.priceInUsd ?? 0));
        public static Currency operator %(decimal a, Currency b) => new((double)a % (b?.priceInUsd ?? 0));

        public static bool operator ==(decimal a, Currency b) => Math.Abs((double)a - (b?.priceInUsd ?? 0)) < Tolerance.FLOAT;
        public static bool operator !=(decimal a, Currency b) => Math.Abs((double)a - (b?.priceInUsd ?? 0)) > Tolerance.FLOAT;
        public static bool operator >(decimal a, Currency b) => (double)a > (b?.priceInUsd ?? 0);
        public static bool operator <(decimal a, Currency b) => (double)a < (b?.priceInUsd ?? 0);
        public static bool operator >=(decimal a, Currency b) => (double)a >= (b?.priceInUsd ?? 0);
        public static bool operator <=(decimal a, Currency b) => (double)a <= (b?.priceInUsd ?? 0);
        #endregion
    }

    // create a custom json converter
    public class CurrencyJsonConverter : JsonConverter<Currency>
    {
        public override void WriteJson(JsonWriter writer, Currency value, JsonSerializer serializer)
        {
            // write the double value and the is estimate flag
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(Currency.CurrencyCode));
            writer.WriteValue(value.CurrencyCode.ToString());
            writer.WritePropertyName(nameof(Currency.PriceInUsd));
            writer.WriteValue(value.PriceInUsd);
            writer.WritePropertyName(nameof(Currency.IsEstimate));
            writer.WriteValue(value.IsEstimate);
            writer.WriteEndObject();
        }

        public override Currency ReadJson(JsonReader reader, Type objectType, Currency existingvalue, bool hasExistingvalue, JsonSerializer serializer)
        {
            // null check
            if (reader.TokenType == JsonToken.Null) return new Currency();
            // read the double value and the is estimate flag
            JObject jObject = JObject.Load(reader);
            string currencyCodeAsString = jObject[nameof(Currency.CurrencyCode)]?.Value<string>();
            CurrencyCode currencyCode = CurrencyCode.USD;
            if (currencyCodeAsString != null) currencyCode = (CurrencyCode)Enum.Parse(typeof(CurrencyCode), currencyCodeAsString);
            double value = (jObject[nameof(Currency.PriceInUsd)] ?? 0f).Value<double>();
            bool isEstimate = (jObject[nameof(Currency.IsEstimate)] ?? false).Value<bool>();
            return new Currency(value, currencyCode, isEstimate);
        }
    }
}
