using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glitch9.Collections
{
    [Serializable]
    [JsonConverter(typeof(SafeEnumKeyValueConverter))]
    public class SerializedKeyValuePair<TKey, TValue>
    {
        public TKey key;
        public TValue value;
    }

    internal class SafeEnumKeyValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(SerializedKeyValuePair<,>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObj = JObject.Load(reader);

            var keyProp = jObj["key"]?.ToString();
            var valueToken = jObj["value"];

            var keyType = objectType.GetGenericArguments()[0];
            var valueType = objectType.GetGenericArguments()[1];

            object parsedKey = null;

            try
            {
                if (keyType.IsEnum && Enum.TryParse(keyType, keyProp, out var enumValue))
                    parsedKey = enumValue;
            }
            catch { }

            var value = valueToken?.ToObject(valueType, serializer);

            var instance = Activator.CreateInstance(objectType);
            objectType.GetField("key").SetValue(instance, parsedKey);
            objectType.GetField("value").SetValue(instance, value);
            return instance;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var key = value.GetType().GetField("key")?.GetValue(value);
            var val = value.GetType().GetField("value")?.GetValue(value);

            writer.WriteStartObject();
            writer.WritePropertyName("key");
            serializer.Serialize(writer, key?.ToString());
            writer.WritePropertyName("value");
            serializer.Serialize(writer, val);
            writer.WriteEndObject();
        }
    }
}