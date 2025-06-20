using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// https://platform.openai.com/docs/api-reference/assistants-streaming/message-delta-object
    /// </summary>
    public class ThreadMessageDelta : AIResponse
    {
        /// <summary>
        /// The delta containing the fields that have changed on the Message.
        /// </summary>
        [JsonProperty("delta")] public ThreadMessage Delta { get; set; }
    }
}