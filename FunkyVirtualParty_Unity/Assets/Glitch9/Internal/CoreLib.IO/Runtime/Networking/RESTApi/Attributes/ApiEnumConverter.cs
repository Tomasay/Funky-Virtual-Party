using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Glitch9.IO.Networking.RESTApi
{
    /// <summary>
    /// Custom <see cref="JsonConverter"/> for API enum values.
    /// </summary>
    public class ApiEnumConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            Type enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;

            if (!enumType.IsEnum)
                throw new JsonSerializationException($"Type {enumType.Name} is not an enum.");

            string value = reader.Value?.ToString();
            if (string.IsNullOrEmpty(value)) return null;
            //throw new JsonSerializationException("Null or empty string value has been passed to ReadJson method.");

            foreach (var name in Enum.GetNames(enumType))
            {
                var field = enumType.GetField(name);
                if (field == null)
                    continue;

                var apiEnumAttribute = AttributeCache<ApiEnumAttribute>.Get(field);
                if (apiEnumAttribute != null)// && value.Equals(apiEnumAttribute.ApiName, StringComparison.OrdinalIgnoreCase))
                {
                    string parseKey = apiEnumAttribute.ParseKey;

                    if (!string.IsNullOrEmpty(parseKey) && value.Contains(parseKey, StringComparison.OrdinalIgnoreCase))
                    {
                        return field.GetValue(null);
                    }

                    string apiName = apiEnumAttribute.ApiName;

                    if (!string.IsNullOrEmpty(apiName) && value.Equals(apiName, StringComparison.OrdinalIgnoreCase))
                    {
                        return field.GetValue(null);
                    }
                }

                if (value.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return Enum.Parse(enumType, name);
                }
            }

            throw new JsonSerializationException($"Unknown enum value: {reader.Value}");

            // return 0; // return default value for the enum type if no match is found
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type enumType = (value?.GetType()) ?? throw new ArgumentNullException(nameof(value));
            string name = enumType.GetEnumName(value);
            //if (string.IsNullOrEmpty(name)) throw new JsonSerializationException("Invalid enum value");
            if (string.IsNullOrEmpty(name))
            {
                // write default enum name of the enum type
                writer.WriteValue(enumType.IsEnum ? enumType.GetEnumNames()[0] : "Unknown");
                return;
            }

            ApiEnumAttribute apiEnumAttribute = AttributeCache<ApiEnumAttribute>.Get(enumType.GetField(name));

            if (apiEnumAttribute != null)
            {
                writer.WriteValue(apiEnumAttribute.ApiName);
                return;
            }

            writer.WriteValue(name);
        }
    }
}