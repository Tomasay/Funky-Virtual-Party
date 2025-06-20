using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class Expiration
    {
        /// <summary>
        /// Anchor timestamp after which the expiration policy applies. Supported anchors: last_active_at.
        /// </summary>
        [JsonProperty("anchor")] public string Anchor { get; set; }

        /// <summary>
        /// The number of days after the anchor time that the vector store will expire.
        /// </summary>
        [JsonProperty("days")] public int? Days { get; set; }
    }
}