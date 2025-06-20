using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Glitch9.AIDevKit.OpenAI
{
    public class CodeInterpreterTool
    {
        public string Input { get; set; }
        public CodeInterpreterOutput[] Outputs { get; set; }
    }

    public enum CodeInterpreterOutputType
    {
        [ApiEnum("logs")] Logs,
        [ApiEnum("image")] Image,
    }

    [JsonConverter(typeof(CodeInterpreterOutputConverter))]
    public abstract class CodeInterpreterOutput
    {
        [JsonProperty("index")] public int Index { get; set; }
        [JsonProperty("type")] public CodeInterpreterOutputType Type { get; set; }
    }

    public class CodeInterpreterLogOutput : CodeInterpreterOutput
    {
        [JsonProperty("logs")] public string Logs { get; set; }
        [JsonConstructor]
        public CodeInterpreterLogOutput()
        {
            Type = CodeInterpreterOutputType.Logs;
        }
    }

    public class CodeInterpreterImageOutput : CodeInterpreterOutput
    {
        [JsonProperty("image")] public FileRef Image { get; set; }
        [JsonConstructor]
        public CodeInterpreterImageOutput()
        {
            Type = CodeInterpreterOutputType.Image;
        }
    }

    public class CodeInterpreterOutputConverter : JsonConverter<CodeInterpreterOutput>
    {
        public override CodeInterpreterOutput ReadJson(JsonReader reader, Type objectType, CodeInterpreterOutput existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            CodeInterpreterOutputType type = jsonObject["type"]!.ToObject<CodeInterpreterOutputType>();

            CodeInterpreterOutput output = type switch
            {
                CodeInterpreterOutputType.Logs => new CodeInterpreterLogOutput(),
                CodeInterpreterOutputType.Image => new CodeInterpreterImageOutput(),
                _ => throw new ArgumentException("Unknown output type")
            };

            serializer.Populate(jsonObject.CreateReader(), output);
            return output;
        }

        public override void WriteJson(JsonWriter writer, CodeInterpreterOutput value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will not be written to JSON.");
        }

        public override bool CanWrite => false;
    }
}