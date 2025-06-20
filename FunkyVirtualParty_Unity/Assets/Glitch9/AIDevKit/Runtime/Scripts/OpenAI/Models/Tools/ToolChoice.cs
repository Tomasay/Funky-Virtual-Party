using Newtonsoft.Json;
using System;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;
using Glitch9.IO.Networking;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// This can be a String or an Object
    /// Specifies a tool the model should use.
    /// Use to force the model to call a specific tool.
    /// </summary>
    [JsonConverter(typeof(ToolChoiceConverter))]
    public class ToolChoice
    {
        public static implicit operator ToolChoice(ToolType type) => new(type);
        public static implicit operator ToolChoice(string functionName) => new(functionName);

        /// <summary>
        /// [Required] The type of the tool. 
        /// If type is function, the function name must be set
        /// </summary>
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("function")] public NameModel Function { get; set; }

        [JsonIgnore] public bool IsString { get; private set; }
        [JsonIgnore] public bool IsObject => !IsString;

        public ToolChoice(ToolType type)
        {
            if (type == ToolType.Function)
            {
                Debug.LogError("Use the other constructor for ToolType.Function");
                return;
            }

            IsString = type == ToolType.None || type == ToolType.Auto;
            Type = type.ToApiValue();
        }

        public ToolChoice(string functionName)
        {
            Type = ToolType.Function.ToApiValue();
            IsString = false;
            Function = new NameModel(functionName);// { Name = functionName };
        }
    }

    public class ToolChoiceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ResponseFormat) || objectType.IsEnum || objectType == typeof(string);
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ToolChoice toolChoice)
            {
                if (toolChoice.IsString)
                {
                    writer.WriteValue(toolChoice.Type);
                }
                else
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("type");
                    writer.WriteValue(toolChoice.Type);
                    if (toolChoice.Type == ToolType.Function.ToApiValue())
                    {
                        writer.WritePropertyName("function");
                        serializer.Serialize(writer, toolChoice.Function);
                    }
                    writer.WriteEndObject();
                }
                return;
            }

            throw new JsonSerializationException("value is not a ToolChoice");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new ToolChoice((string)reader.Value);
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                var toolChoice = new ToolChoice(ToolType.None);
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        var propertyName = (string)reader.Value;
                        reader.Read();
                        switch (propertyName)
                        {
                            case "type":
                                toolChoice.Type = (string)reader.Value;
                                break;
                            case "function":
                                toolChoice.Function = serializer.Deserialize<NameModel>(reader);
                                break;
                        }
                    }
                    else if (reader.TokenType == JsonToken.EndObject)
                    {
                        return toolChoice;
                    }
                }
            }

            throw new JsonSerializationException("Unexpected token or value when parsing ToolChoice");
        }
    }
}