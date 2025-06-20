using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glitch9.AIDevKit.Google
{
    public class CustomMetadataValue
    {
        public string StringValue { get; set; }
        public List<string> StringListValue { get; set; }
        public float? NumericValue { get; set; }
    }

    public class CustomMetadataConverter : JsonConverter<Dictionary<string, CustomMetadataValue>>
    {
        public override Dictionary<string, CustomMetadataValue> ReadJson(JsonReader reader, Type objectType, Dictionary<string, CustomMetadataValue> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            var dict = new Dictionary<string, CustomMetadataValue>();

            foreach (var item in array)
            {
                var key = item["key"]?.ToString();
                if (string.IsNullOrEmpty(key)) continue;

                var value = new CustomMetadataValue();

                if (item["stringValue"] != null)
                    value.StringValue = item["stringValue"]!.ToString();

                if (item["numericValue"] != null)
                    value.NumericValue = item["numericValue"]!.Value<float>();

                if (item["stringListValue"]?["values"] is JArray list)
                    value.StringListValue = list.ToObject<List<string>>();

                dict[key] = value;
            }

            return dict;
        }

        public override void WriteJson(JsonWriter writer, Dictionary<string, CustomMetadataValue> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (var kvp in value)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("key");
                writer.WriteValue(kvp.Key);

                if (kvp.Value.StringValue != null)
                {
                    writer.WritePropertyName("stringValue");
                    writer.WriteValue(kvp.Value.StringValue);
                }
                else if (kvp.Value.NumericValue.HasValue)
                {
                    writer.WritePropertyName("numericValue");
                    writer.WriteValue(kvp.Value.NumericValue);
                }
                else if (kvp.Value.StringListValue != null)
                {
                    writer.WritePropertyName("stringListValue");
                    writer.WriteStartObject();
                    writer.WritePropertyName("values");
                    serializer.Serialize(writer, kvp.Value.StringListValue);
                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }
}