using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class RunStep : RunBase
    {
        [JsonProperty("run_id")] public string RunId { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("expired_at")] public UnixTime? ExpiredAt { get; set; }
        [JsonProperty("step_details")] public StepDetails StepDetails { get; set; }
    }

    /// <summary>
    /// The details of the run step.
    /// </summary>
    public class StepDetails
    {
        /// <summary>
        /// Always message_creation.
        /// </summary>
        [JsonProperty("type")] public string Type { get; set; }

        /// <summary>
        /// Details of the message creation by the run step.
        /// </summary>
        [JsonProperty("message_creation")] public MessageInfo MessageCreation { get; set; }

        /// <summary>
        /// Details of the tool call.
        /// </summary>
        [JsonProperty("tool_calls")] public ToolCall[] ToolCalls { get; set; }
    }

    public class MessageInfo
    {
        [JsonProperty("message_id")] public string MessageId { get; set; }
    }
}