using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// The per-line object of the batch input file
    /// </summary>
    public class BatchRequestInput
    {
        /// <summary>
        /// A developer-provided per-request id that will be used to match outputs to inputs. Must be unique for each request in a batch.
        /// </summary>
        [JsonProperty("custom_id")] public string CustomId { get; set; }

        /// <summary>
        /// The HTTP method to be used for the request. Currently only POST is supported.
        /// </summary>
        [JsonProperty("method")] public string Method { get; set; }

        /// <summary>
        /// The OpenAI API relative URL to be used for the request. Currently, /v1/chat/completions, /v1/embeddings, and /v1/completions are supported.
        /// </summary>
        [JsonProperty("url")] public string Url { get; set; }
    }
}