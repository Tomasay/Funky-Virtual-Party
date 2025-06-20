using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glitch9.AIDevKit.OpenAI
{
    public class HttpResponse
    {
        /// <summary>
        /// The HTTP status code of the response
        /// </summary>
        [JsonProperty("status_code")] public int StatusCode { get; set; }

        /// <summary>
        /// An unique identifier for the OpenAI API request. Please include this request ID when contacting support.
        /// </summary>
        [JsonProperty("request_id")] public string RequestId { get; set; }

        /// <summary>
        /// The JSON body of the response
        /// </summary>
        [JsonProperty("body")] public JObject Body { get; set; }
    }
}