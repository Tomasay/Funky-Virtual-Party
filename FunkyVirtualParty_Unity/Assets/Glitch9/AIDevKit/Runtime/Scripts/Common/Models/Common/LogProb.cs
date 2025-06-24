using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    public class LogProb
    {
        /// <summary>
        /// Required. The bytes that were used to generate the log probability.
        /// </summary>
        [JsonProperty("bytes")] public List<int> Bytes { get; set; }

        /// <summary>
        /// Required. The log probability of the token.
        /// </summary>
        [JsonProperty("logprob")] public float Logprob { get; set; }

        /// <summary>
        /// Required. The token that was used to generate the log probability.
        /// </summary>
        [JsonProperty("token")] public string Token { get; set; }
    }
}