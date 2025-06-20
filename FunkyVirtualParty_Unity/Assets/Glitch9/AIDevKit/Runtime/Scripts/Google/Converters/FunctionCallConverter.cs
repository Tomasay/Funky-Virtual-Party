using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glitch9.AIDevKit.Google
{
    public class FunctionCallConverter : JsonConverter<FunctionCall>
    {
        public override FunctionCall ReadJson(JsonReader reader, Type objectType, FunctionCall existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);

            string id = array["id"]?.ToString();
            string name = array["name"]?.ToString();
            string args = array["args"]?.ToString();

            return new FunctionCall(id, name, args);
        }

        public override void WriteJson(JsonWriter writer, FunctionCall value, JsonSerializer serializer)
        {
            var obj = new JObject
            {
                ["id"] = value.Id,
                ["name"] = value.Name,
                ["args"] = value.Args
            };

            obj.WriteTo(writer);
        }
    }
}