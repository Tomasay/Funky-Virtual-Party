using Glitch9.IO.Json.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Structured representation of a function declaration as defined by the OpenAPI 3.03 specification.
    /// Included in this declaration are the function name and parameters.
    /// This FunctionDeclaration is a representation of a block of code that can be used as a Tool by the model and executed by the client.
    /// </summary>
    public class FunctionDeclaration
    {
        /// <summary>
        /// Required.
        /// The name of the function.
        /// Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 63.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Required.
        /// A brief description of the function.
        /// </summary>
        [JsonProperty("description")] public string Description { get; set; }

        /// <summary>
        /// Optional.
        /// Describes the parameters to this function.
        /// Reflects the Open API 3.03 Parameter Object string Key: the name of the parameter.
        /// Parameter names are case-sensitive.
        /// Schema Value: the Schema defining the type used for the parameter.
        /// </summary>
        [JsonProperty("parameters")] public JsonSchema Parameters { get; set; }

        /// <summary>
        /// Optional. Describes the output from this function in JSON Schema format. 
        /// Reflects the Open API 3.03 Response Object. 
        /// The Schema defines the type used for the response value of the function.
        /// 
        /// Only used by Google Generative AI.
        /// </summary>
        [JsonProperty("response")] public JsonSchema Response { get; set; }

        /// <summary>
        /// Only appears in response. The function parameters and values in JSON object format.
        /// </summary>
        [JsonProperty("args")] public string Arguments { get; set; }

        public FunctionDeclaration() { }
        public FunctionDeclaration(string name) => Name = name;

        public FunctionDeclaration(Type type, string name, string description = null)
        {
            Name = name;
            Description = description;
            Parameters = JsonSchema.Create(type);
        }

        public static FunctionDeclaration FromSchema<T>(string name, string description = null)
            where T : class
        {
            return new FunctionDeclaration()
            {
                Name = name,
                Description = description,
                Parameters = JsonSchema.Create<T>()
            };
        }
    }

    public class FunctionDeclarationConverter : JsonConverter<FunctionDeclaration>
    {
        public override FunctionDeclaration ReadJson(JsonReader reader, Type objectType, FunctionDeclaration existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonSerializationException("Expected StartObject token");
            }

            JObject jObject = JObject.Load(reader);
            FunctionDeclaration function = new()
            {
                Name = jObject["name"]?.ToString(),
                Description = jObject["description"]?.ToString(),
                Parameters = JsonSchema.Read(jObject["parameters"]?.CreateReader()),
            };

            return function;
        }

        public override void WriteJson(JsonWriter writer, FunctionDeclaration value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("name");
            writer.WriteValue(value.Name);

            if (!string.IsNullOrWhiteSpace(value.Description))
            {
                writer.WritePropertyName("description");
                writer.WriteValue(value.Description);
            }

            if (value.Parameters != null)
            {
                writer.WritePropertyName("parameters");
                value.Parameters.WriteTo(writer, TextCase.UpperCase);
            }

            writer.WriteEndObject();
        }
    }
}
