using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// The response from the model, including candidate completions.
    /// </summary>
    public class GenerateTextResponse
    {
        /// <summary>
        /// Candidate responses from the model.
        /// </summary>
        [JsonProperty("candidates")] public Candidate[] Candidates { get; set; }

        /// <summary>
        /// A set of content filtering metadata for the prompt and response text.
        /// <para>This indicates which SafetyCategory(s) blocked a candidate from this response,
        /// the lowest HarmProbability that triggered a block, and the HarmThreshold setting for that category.
        /// This indicates the smallest change to the SafetySettings that would be necessary to unblock at least 1 response.</para>
        /// <para>The blocking is configured by the SafetySettings in the request (or the default SafetySettings of the API).</para>
        /// </summary>
        [JsonProperty("filters")] public ContentFilter[] Filters { get; set; }

        /// <summary>
        /// Returns any safety feedback related to content filtering.
        /// </summary>
        [JsonProperty("safetyFeedback")] public SafetyFeedback[] SafetyFeedback { get; set; }
    }


    /// <summary>
    /// Content filtering metadata associated with processing a single request.
    /// ContentFilter contains a reason and an optional supporting string. The reason may be unspecified.
    /// </summary>
    public class ContentFilter
    {
        /// <summary>
        /// The reason content was blocked during request processing.
        /// </summary>
        [JsonProperty("reason")] public StopReason Reason { get; set; }

        /// <summary>
        /// A string that describes the filtering behavior in more detail.
        /// </summary>
        [JsonProperty("message")] public string Message { get; set; }
    }
}