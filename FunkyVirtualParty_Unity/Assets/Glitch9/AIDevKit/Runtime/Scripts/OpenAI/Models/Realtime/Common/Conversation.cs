using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    /// <summary>
    /// A realtime Conversation consists of a list of Items.
    /// <para>
    /// By default, there is only one Conversation, and it gets created at the beginning of the Session. In the future, we may add support for additional conversations.
    /// </para>
    /// </summary>
    public class Conversation
    {
        /// <summary>
        /// The unique ID of the conversation.
        /// </summary>
        [JsonProperty("id")] public string Id { get; set; }

        /// <summary>
        /// The object type, must be "realtime.conversation".
        /// </summary>
        [JsonProperty("object")] public string Object { get; set; }

        public override string ToString()
        {
            return $"Conversation {Id}";
        }
    }
}
