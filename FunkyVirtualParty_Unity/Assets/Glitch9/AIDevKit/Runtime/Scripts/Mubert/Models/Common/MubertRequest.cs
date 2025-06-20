using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Mubert
{
    public abstract class MubertRequest<T> : RequestBody
    {
        /// <summary>
        /// Required. The name of the method to call.
        /// </summary>
        [JsonProperty("method")] public abstract MubertMethod Method { get; }

        /// <summary>
        /// Required. The parameters for the method.
        /// </summary>
        [JsonProperty("params")] public T Params { get; set; }
    }

    public class MubertRequestParams
    {
        /// <summary>
        /// Required. The PAT (Personal Access Token) for the user.
        /// </summary>
        [JsonProperty("pat")] public string Pat { get; set; }
    }
}