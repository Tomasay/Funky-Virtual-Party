using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// The per-line object of the batch output and error files
    /// </summary>
    public class BatchRequestOutput
    {
        /// <summary>
        /// A developer-provided per-request id that will be used to match outputs to inputs.
        /// </summary>
        [JsonProperty("custom_id")] public string CustomId { get; set; }

        /// <summary>
        /// The response object
        /// </summary>
        [JsonProperty("response")] public object Response { get; set; }

        /// <summary>
        /// The error object
        /// </summary>
        [JsonProperty("error")] public AIDevKit.ErrorResponse Error { get; set; }
    }
}