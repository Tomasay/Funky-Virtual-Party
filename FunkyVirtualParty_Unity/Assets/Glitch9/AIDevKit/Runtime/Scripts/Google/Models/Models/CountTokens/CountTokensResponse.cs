using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Response from calling {@link GenerativeModel.countTokens}.
    /// </summary>
    public class CountTokensResponse
    {
        [JsonProperty("totalTokens")] public int? TotalTokens { get; set; }
    }
}