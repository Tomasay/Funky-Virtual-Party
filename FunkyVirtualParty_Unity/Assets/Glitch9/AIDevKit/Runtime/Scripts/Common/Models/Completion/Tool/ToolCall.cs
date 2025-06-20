using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    public enum ToolType
    {
        [ApiEnum("none")] None,
        [ApiEnum("auto")] Auto,
        [ApiEnum("function")] Function,
        [ApiEnum("tool_calls")] ToolCalls,
        [ApiEnum("code_interpreter")] CodeInterpreter,
        [ApiEnum("file_search")] FileSearch,
    }

    public interface IToolCallReceiver
    {
        void OnReceiveToolCalls(ToolCall[] toolCalls);
    }

    public abstract class ToolCall
    {
        /// <summary>
        /// Required. The unique identifier of the tool.
        /// </summary>
        [JsonProperty("id")] public string Id { get; set; }

        /// <summary>
        /// Required. The type of the tool.
        /// </summary>
        [JsonProperty("type")] public ToolType Type { get; set; }
    }
}