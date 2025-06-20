using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Give Assistants access to OpenAI-hosted tools like Issue Interpreter and Knowledge Retrieval, or build your own tools using Function calling. 
    /// Usage of OpenAI-hosted tools comes at an additional fee — visit our help center article to learn more about how these tools are priced.
    /// </summary>
    /// <remarks>
    /// https://platform.openai.com/docs/Assistants/tools
    /// </remarks>
    public class AssistantRequest : AIRequest
    {
        /// <summary>
        /// The name of the AssistantObject. 
        /// The maximum length is 256 characters.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// The description of the AssistantObject. 
        /// The maximum length is 512 characters.
        /// </summary>
        [JsonProperty("description")] public string Description { get; set; }

        /// <summary>
        /// The System instructions that the AssistantObject uses. 
        /// The maximum length is 32768 characters.
        /// </summary>
        [JsonProperty("instructions")] public string Instructions { get; set; }

        /// <summary>
        /// A list of Tool enabled on the AssistantObject. 
        /// There can be a maximum of 128 tools per AssistantObject. 
        /// Tools can be of types code_interpreter, retrieval, or Function.
        /// </summary>
        /// <remarks>
        /// Default: []
        /// </remarks>
        [JsonProperty("tools")] public List<ToolCall> Tools { get; set; }

        /// <summary>
        /// A set of resources that are used by the assistant's tools.
        /// The resources are specific to the type of tool.
        /// For example, the code_interpreter tool requires a list of file IDs,
        /// while the file_search tool requires a list of vector store IDs.
        /// </summary>
        [JsonProperty("tool_resources")] public ToolResources ToolResources { get; set; }

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


        public class Builder : ModelRequestBuilder<Builder, AssistantRequest>
        {
            public Builder SetName(string name)
            {
                _req.Name = name;
                return this;
            }

            public Builder SetDescription(string description)
            {
                _req.Description = description;
                return this;
            }

            public Builder SetInstructions(string instructions)
            {
                _req.Instructions = instructions;
                return this;
            }

            public Builder SetTools(params ToolCall[] tools)
            {
                if (tools == null) return this;

                if (tools.Length > 128)
                {
                    throw new System.ArgumentException("The maximum number of tools is 128");
                }

                _req.Tools = new List<ToolCall>(tools);
                return this;
            }

            public Builder SetTools(IEnumerable<ToolCall> tools)
            {
                if (tools == null) return this;
                _req.Tools = new List<ToolCall>(tools);
                return this;
            }

            public Builder AddTool(ToolCall tool)
            {
                if (tool == null) return this;
                _req.Tools ??= new List<ToolCall>();
                _req.Tools.Add(tool);
                return this;
            }

            public Builder SetToolResources(ToolResources resources)
            {
                if (resources == null) return this;
                _req.ToolResources = resources;
                return this;
            }

            public Builder SetTemperature(float? temperature)
            {
                if (temperature == null) return this;
                if (temperature == AIDevKitConfig.TemperatureDefault) return this;
                _req.Temperature = temperature;
                return this;
            }

            public Builder SetTopP(float? topP)
            {
                if (topP == null) return this;
                if (topP == AIDevKitConfig.TopPDefault) return this;
                _req.TopP = topP;
                return this;
            }

            public Builder SetResponseFormat(ResponseFormat responseFormat)
            {
                if (responseFormat == null) return this;
                _req.ResponseFormat = responseFormat;
                return this;
            }

            public Builder SetFileSearchEnabled(bool enabled)
            {
                if (enabled)
                {
                    if (_req.Tools == null) _req.Tools = new List<ToolCall>();
                    if (!_req.Tools.Any(t => t.Type == ToolType.FileSearch))
                        _req.Tools.Add(new FileSearchCall());
                }
                else
                {
                    var tool = _req.Tools?.FirstOrDefault(t => t.Type == ToolType.FileSearch);
                    if (tool != null) _req.Tools.Remove(tool);
                }

                return this;
            }

            public Builder SetCodeInterpreterEnabled(bool enabled)
            {
                if (enabled)
                {
                    if (_req.Tools == null) _req.Tools = new List<ToolCall>();
                    if (!_req.Tools.Any(t => t.Type == ToolType.CodeInterpreter))
                        _req.Tools.Add(new CodeInterpreterCall());
                }
                else
                {
                    var tool = _req.Tools?.FirstOrDefault(t => t.Type == ToolType.CodeInterpreter);
                    if (tool != null) _req.Tools.Remove(tool);
                }

                return this;
            }
        }
    }
}