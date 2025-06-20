using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public interface IAssistantEventReceiver
    {
        void OnAssistantCreated(Assistant assistant);
        void OnAssistantRetrieved(Assistant assistant);
        void OnAssistantUpdated(Assistant assistant);
    }

    /// <summary>
    /// Renamed from AssistantObject to Assistant (2024.06.14)
    /// </summary>
    public class Assistant : AIResponse, IData
    {
        /// <summary>
        /// The name of the AssistantObject. The maximum length is 256 characters.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// The description of the AssistantObject. The maximum length is 512 characters.
        /// </summary>
        [JsonProperty("description")] public string Description { get; set; }

        /// <summary>
        /// The System instructions that the AssistantObject uses. The maximum length is 32768 characters.
        /// </summary>
        [JsonProperty("instructions")] public string Instructions { get; set; }

        /// <summary>
        /// A list of Tool enabled on the AssistantObject. There can be a maximum of 128 tools per AssistantObject. Tools can be of types code_interpreter, retrieval, or Function.
        /// </summary>
        [JsonProperty("tools")] public List<ToolCall> Tools { get; set; }

        /// <summary>
        /// [Legacy]
        /// A list of file IDs attached to this AssistantObject. There can be a maximum of 20 files attached to the AssistantObject. Files are ordered by their creation date in ascending order.
        /// </summary>
        /// [JsonProperty("file_ids")] public string[] FileIds { get; set; }

        /// <summary>
        /// What sampling temperature to use, between 0 and 2.
        /// Higher values like 0.8 will make the Output more random,
        /// while lower values like 0.2 will make it more focused and deterministic.
        /// We generally recommend altering this or top_p but not both.
        /// </summary>
        [JsonProperty("temperature")] public float? Temperature { get; set; }

        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling,
        /// where the model considers the results of the tokens with top_p probability mass.
        /// So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// We generally recommend altering this or temperature but not both.
        /// </summary>
        [JsonProperty("top_p")] public float? TopP { get; set; }

        /// <summary>
        /// Specifies the format that the model must output.
        /// Compatible with GPT-4o, GPT-4 Turbo, and all GPT-3.5 Turbo models since gpt-3.5-turbo-1106.
        /// Setting to { "type": "json_object" } enables JSON mode,
        /// which guarantees the message the model generates is valid JSON.
        /// </summary>
        /// <remarks>
        /// Important: when using JSON mode,
        /// you must also instruct the model to produce JSON yourself via a system or user message.
        /// Without this, the model may generate an unending stream of whitespace until the generation reaches the token limit,
        /// resulting in a long-running and seemingly "stuck" request.
        /// Also note that the message content may be partially cut off if finish_reason="length",
        /// which indicates the generation exceeded max_tokens or the conversation exceeded the max context length.
        /// </remarks>
        [JsonProperty("response_format")] public ResponseFormat ResponseFormat { get; set; }


        [JsonIgnore]
        public bool CodeInterpreterEnabled
        {
            get => Tools != null && Tools.Any(t => t.Type == ToolType.CodeInterpreter);
            set
            {
                Tools ??= new List<ToolCall>();
                if (value)
                {
                    if (!Tools.Any(t => t.Type == ToolType.CodeInterpreter))
                        Tools.Add(new CodeInterpreterCall());
                }
                else
                {
                    var tool = Tools.FirstOrDefault(t => t.Type == ToolType.CodeInterpreter);
                    if (tool != null) Tools.Remove(tool);
                }
            }
        }

        [JsonIgnore]
        public bool FileSearchEnabled
        {
            get => Tools != null && Tools.Any(t => t.Type == ToolType.FileSearch);
            set
            {
                Tools ??= new List<ToolCall>();
                if (value)
                {
                    if (!Tools.Any(t => t.Type == ToolType.FileSearch))
                        Tools.Add(new FileSearchCall());
                }
                else
                {
                    var tool = Tools.FirstOrDefault(t => t.Type == ToolType.FileSearch);
                    if (tool != null) Tools.Remove(tool);
                }
            }
        }
    }
}