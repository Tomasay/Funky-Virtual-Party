using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// A single content embedding.
    /// </summary>
    public class ContentEmbedding
    {
        /// <summary>
        /// The embedding values.
        /// </summary>
        [JsonProperty("values")] public int[] Values { get; set; }
    }
}