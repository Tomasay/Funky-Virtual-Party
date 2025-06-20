using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Glitch9.IO.Json.Schema
{
    internal class JsonSchemaReader
    {
        public JsonSchema Read(JsonReader reader)
        {
            JObject obj = JObject.Load(reader);
            return ParseSchema(obj);
        }

        private JsonSchema ParseSchema(JObject obj)
        {
            string typeString = (string)obj["type"];
            JsonSchemaType jsonSchemaType;

            JsonSchema schema = new()
            {
                Description = (string)obj["description"],
                Format = (string)obj["format"]
            };

            if (obj["properties"] is JObject properties)
            {
                schema.Properties = new Dictionary<string, JsonSchema>();

                foreach (KeyValuePair<string, JToken> property in properties)
                {
                    schema.Properties.Add(property.Key, ParseSchema((JObject)property.Value));
                }
            }

            if (obj["enum"] is JArray enumArray)
            {
                schema.Enum = new List<string>();
                foreach (JToken token in enumArray)
                {
                    schema.Enum.Add(token.ToString());
                }

                jsonSchemaType = JsonSchemaType.Enum;
            }
            else
            {
                jsonSchemaType = JsonSchemaTypes.Parse(typeString);
            }

            schema.Type = jsonSchemaType;

            if (obj["required"] is JArray requiredArray)
            {
                schema.Required = new List<string>();
                foreach (JToken token in requiredArray)
                {
                    schema.Required.Add(token.ToString());
                }
            }

            return schema;
        }
    }
}