using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    public class ModalityConverter : JsonConverter<Modality>
    {
        public override Modality ReadJson(JsonReader reader, Type objectType, Modality existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return 0;

            string enumAsString = reader.Value.ToString();
            if (string.IsNullOrEmpty(enumAsString)) return 0;

            enumAsString = enumAsString.ToUpperInvariant();

            if (enumAsString == "MODALITY_UNSPECIFIED")
            {
                return 0;
            }

            if (Enum.TryParse(typeof(Modality), enumAsString, out var result))
            {
                return (Modality)result;
            }

            Debug.WriteLine($"Failed to parse Modality enum from string: {enumAsString}");

            return 0;
        }

        public override void WriteJson(JsonWriter writer, Modality value, JsonSerializer serializer)
        {
            string enumAsString;

            if (value == 0)
            {
                enumAsString = "MODALITY_UNSPECIFIED";
            }
            else
            {
                enumAsString = value.ToString().ToUpperInvariant();
            }

            writer.WriteValue(enumAsString);
        }
    }
}