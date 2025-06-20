using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Params for calling {@link GenerativeModel.countTokens}
    /// </summary>
    public class CountTokensRequest : GenerativeAIRequest
    {
        [JsonProperty("contents")] public Content[] Contents { get; set; }
    }
}