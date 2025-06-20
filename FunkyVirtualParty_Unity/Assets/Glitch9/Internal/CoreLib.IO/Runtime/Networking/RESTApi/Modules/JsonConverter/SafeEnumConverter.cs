using System;
using Newtonsoft.Json;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Custom <see cref="JsonConverter"/> for safely converting enum values.
    /// This converter will return the default value of the enum if the JSON value does not match any enum names.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SafeEnumConverter<T> : JsonConverter where T : struct, Enum
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(T);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String && Enum.TryParse<T>((string)reader.Value, out var result))
            {
                return result;
            }

            return default(T); // fallback
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}