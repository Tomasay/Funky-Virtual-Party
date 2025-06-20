using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Mubert
{
    public class MubertResponse<T>
    {
        /// <summary>
        /// Required. The name of the method.
        /// </summary>
        [JsonProperty("method")] public MubertMethod Method { get; set; }

        /// <summary>
        /// Required. The status of the request.
        /// </summary>
        [JsonProperty("status")] public int Status { get; set; }

        /// <summary>
        /// Required. The response data.
        /// </summary>
        [JsonProperty("data")] public T Data { get; set; }

        /// <summary>
        /// Required. The API version.
        /// </summary>
        [JsonProperty("api_ver")] public string ApiVersion { get; set; }
    }

}