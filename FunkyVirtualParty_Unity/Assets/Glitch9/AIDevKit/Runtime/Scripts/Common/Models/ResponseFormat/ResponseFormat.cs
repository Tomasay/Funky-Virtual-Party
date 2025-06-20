using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    [JsonConverter(typeof(ResponseFormatConverter))]
    public class ResponseFormat
    {
        public static implicit operator ResponseFormat(Enum type) => new(type);
        public static implicit operator ResponseFormat(string type) => new(type);
        public static implicit operator string(ResponseFormat wrapper) => wrapper?.Type;

        /// <summary>
        /// The type of the format returned by the API.
        /// </summary>
        [JsonProperty("type")] public string Type { get; set; }

        [JsonConstructor] public ResponseFormat() => IsString = false;

        public ResponseFormat(Enum type)
        {
            IsString = type is not TextFormat;
            Type = type.ToApiValue();
        }

        public ResponseFormat(string type)
        {
            IsString = true;
            Type = type;
        }

        public bool IsString { get; protected set; }
        public bool IsObject => !IsString;

        public override string ToString() => Type;
        public override bool Equals(object obj) => obj is ResponseFormat format && Type == format.Type;
        public override int GetHashCode() => HashCode.Combine(Type);

        public static bool operator ==(ResponseFormat left, ResponseFormat right) => left?.Type == right?.Type;
        public static bool operator !=(ResponseFormat left, ResponseFormat right) => left?.Type != right?.Type;
    }

    public static class ResponseFormatExtensions
    {
        /// <summary>
        /// Converts the <see cref="ResponseFormat"/> to the specified enum type.
        /// </summary>
        /// <typeparam name="TFormatEnum">The enum type to convert to.</typeparam>
        /// <param name="format">The response format.</param>
        /// <returns>The corresponding enum value.</returns>
        public static TFormatEnum ToEnum<TFormatEnum>(this ResponseFormat format) where TFormatEnum : Enum
        {
            if (format != null) return ApiEnumUtil.Parse<TFormatEnum>(format!.Type);

            // Returning default value for each format type
            Type enumType = typeof(TFormatEnum);
            if (enumType == typeof(ImageFormat)) return (TFormatEnum)(object)ImageFormat.Url;
            if (enumType == typeof(AudioEncoding)) return (TFormatEnum)(object)AudioEncoding.MP3;
            if (enumType == typeof(TranscriptFormat)) return (TFormatEnum)(object)TranscriptFormat.Text;
            return (TFormatEnum)(object)TextFormat.Auto;
        }
    }

    /// <summary>
    /// Converts Format(Enum) or ResponseFormat (struct wrapper) in Json format to ResponseFormat(Enum).
    /// </summary>
    public class ResponseFormatConverter : JsonConverter
    {
        private static readonly string[] _stringTypes =
         {
            "auto",
            "none",
            // Add more if OpenAI adds more string types.
        };

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ResponseFormat) || objectType.IsEnum || objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                // Check the JSON token type to handle objects and primitives appropriately
                JToken token = JToken.Load(reader);
                if (token.Type == JTokenType.Object)
                {
                    return new ResponseFormat(token["type"]?.ToObject<string>());
                }
                if (token.Type == JTokenType.String)
                {
                    return new ResponseFormat(token.ToObject<string>());
                }
                throw new JsonSerializationException("Unexpected token type for ResponseFormat: " + token.Type);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in ReadJson: {ex.Message}");
                throw;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                if (value == null) return;

                if (value is ResponseFormat format)
                {
                    string typeAsString = format.Type;
                    bool isStringType = Array.IndexOf(_stringTypes, typeAsString) >= 0;

                    if (format.IsString || isStringType)
                    {
                        writer.WriteValue(typeAsString);
                    }
                    else
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("type");
                        writer.WriteValue(format.Type);

                        if (format is JsonSchemaFormat jsonSchemaFormat)
                        {
                            if (jsonSchemaFormat.JsonSchema == null)
                                throw new ArgumentNullException(nameof(jsonSchemaFormat.JsonSchema), "JsonSchema cannot be null.");

                            writer.WritePropertyName("json_schema");
                            serializer.Serialize(writer, jsonSchemaFormat.JsonSchema);
                        }

                        writer.WriteEndObject();
                    }
                }
                else
                {
                    throw new JsonSerializationException("Unexpected value type for ResponseFormat: " + value.GetType());
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in WriteJson: {ex.Message}");
                throw;
            }
        }
    }
}
