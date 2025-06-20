using System;
using Glitch9.IO.Json.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    public class JsonSchemaFormat : ResponseFormat
    {
        [JsonProperty("json_schema")] public StrictJsonSchema JsonSchema { get; set; }

        public JsonSchemaFormat(StrictJsonSchema jsonSchema)
        {
            IsString = false;
            Type = "json_schema";
            JsonSchema = jsonSchema;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class StrictJsonSchemaAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Strict { get; set; }
        public StrictJsonSchemaAttribute(string name) => Name = name;
    }

    [JsonConverter(typeof(StrictJsonSchemaConverter))]
    public class StrictJsonSchema
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("strict")] public bool Strict { get; set; }
        [JsonProperty("schema")] public JsonSchema Schema { get; set; }

        public StrictJsonSchema() { }
        public StrictJsonSchema(StrictJsonSchemaAttribute attribute, JsonSchema schema)
        {
            Name = attribute.Name;
            Description = attribute.Description;
            Strict = attribute.Strict;
            Schema = schema;
        }
    }

    public class StrictJsonSchemaConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StrictJsonSchema);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                // Check the JSON token type to handle objects and primitives appropriately
                JToken token = JToken.Load(reader);
                if (token.Type == JTokenType.Object)
                {
                    var schema = new StrictJsonSchema
                    {
                        Name = token["name"]?.ToObject<string>(),
                        Description = token["description"]?.ToObject<string>(),
                        Strict = token["strict"]?.ToObject<bool>() ?? false,
                    };
                    if (token["schema"] != null)
                    {
                        JsonReader schemaReader = token["schema"].CreateReader();
                        schema.Schema = JsonSchema.Read(schemaReader);
                    }
                    return schema;
                }
                throw new JsonSerializationException("Unexpected token type for OpenAIJsonSchema: " + token.Type);
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

                if (value is StrictJsonSchema schema)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("name");
                    writer.WriteValue(schema.Name);

                    if (!string.IsNullOrEmpty(schema.Description))
                    {
                        writer.WritePropertyName("description");
                        writer.WriteValue(schema.Description);
                    }

                    writer.WritePropertyName("strict");
                    writer.WriteValue(schema.Strict);
                    writer.WritePropertyName("schema");
                    schema.Schema?.WriteTo(writer, TextCase.LowerCase);
                    writer.WriteEndObject();
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

