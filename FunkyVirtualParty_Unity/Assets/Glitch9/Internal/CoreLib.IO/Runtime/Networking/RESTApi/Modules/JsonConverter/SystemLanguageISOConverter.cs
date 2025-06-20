using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Custom <see cref="JsonConverter"/> for converting <see cref="SystemLanguage"/> to and from its ISO code.
    /// </summary>
    public class SystemLanguageISOConverter : JsonConverter<SystemLanguage>
    {
        public override void WriteJson(JsonWriter writer, SystemLanguage value, JsonSerializer serializer)
        {
            string isoCode = value.ToISOCode();
            writer.WriteValue(isoCode);
        }

        public override SystemLanguage ReadJson(JsonReader reader, Type objectType, SystemLanguage existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonSerializationException("Expected string value for language code");
            }

            string isoCode = (string)reader.Value;
            return LocaleUtil.ParseISOCode(isoCode);
        }
    }
}