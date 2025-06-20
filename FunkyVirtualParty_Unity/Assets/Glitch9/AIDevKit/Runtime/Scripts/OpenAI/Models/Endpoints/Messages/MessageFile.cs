using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// A list of files attached to a message.
    /// </summary>
    public class MessageFile : AIResponse
    {
        /// <summary>
        /// The ID of the message that the File is attached to.
        /// </summary>
        [JsonProperty("message_id")] public string MessageId { get; set; }
    }
}