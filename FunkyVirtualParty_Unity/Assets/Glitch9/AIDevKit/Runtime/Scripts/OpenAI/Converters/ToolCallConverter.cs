using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Glitch9.AIDevKit.OpenAI
{
    internal class ToolCallConverter : JsonConverter<ToolCall>
    {
        public override void WriteJson(JsonWriter writer, ToolCall value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue(value.Type.ToApiValue());

            if (!string.IsNullOrWhiteSpace(value.Id))
            {
                writer.WritePropertyName("id");
                writer.WriteValue(value.Id);
            }

            if (value is FunctionCall functionToolCall)
            {
                writer.WritePropertyName("function");
                serializer.Serialize(writer, functionToolCall.Function);
            }

            writer.WriteEndObject();
        }

        public override ToolCall ReadJson(JsonReader reader, Type objectType, ToolCall existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonSerializationException("Expected StartObject token.");
            }

            JObject jObject = JObject.Load(reader);
            string typeAsString = jObject["type"]?.Value<string>();

            if (string.IsNullOrWhiteSpace(typeAsString))
            {
                throw new JsonSerializationException("Expected 'type' property.");
            }

            ToolType type = ApiEnumUtil.Parse<ToolType>(typeAsString);

            return type switch
            {
                ToolType.Function => CreateToolCall<FunctionCall>(jObject, serializer),
                ToolType.CodeInterpreter => CreateToolCall<CodeInterpreterCall>(jObject, serializer),
                ToolType.FileSearch => CreateToolCall<FileSearchCall>(jObject, serializer),
                _ => throw new JsonSerializationException($"Unknown tool type: {typeAsString}")
            };
        }

        private T CreateToolCall<T>(JObject reader, JsonSerializer serializer) where T : ToolCall
        {
            T tool = Activator.CreateInstance<T>();

            if (reader.TryGetValue("id", out JToken idToken))
            {
                tool.Id = idToken.Value<string>();
            }

            if (reader.TryGetValue("type", out JToken typeToken))
            {
                tool.Type = ApiEnumUtil.Parse<ToolType>(typeToken.Value<string>());
            }

            if (reader.TryGetValue("function", out JToken functionToken))
            {
                if (tool is FunctionCall functionToolCall)
                    functionToolCall.Function = functionToken.ToObject<FunctionDeclaration>(serializer);
            }
            else if (reader.TryGetValue("code_interpreter", out JToken codeInterpreterToken))
            {
                if (tool is CodeInterpreterCall codeInterpreterToolCall)
                    codeInterpreterToolCall.CodeInterpreter = codeInterpreterToken.ToObject<CodeInterpreterTool>(serializer);
            }
            else if (reader.TryGetValue("file_search", out JToken fileSearchToken))
            {
                if (tool is FileSearchCall fileSearchToolCall)
                    fileSearchToolCall.FileSearch = fileSearchToken.ToObject<FileSearchTool>(serializer);
            }

            return tool;
        }
    }
}