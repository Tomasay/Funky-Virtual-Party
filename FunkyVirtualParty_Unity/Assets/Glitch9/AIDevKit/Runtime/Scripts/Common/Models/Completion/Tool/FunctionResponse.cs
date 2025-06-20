using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// A predicted <see cref="FunctionResponse"/> returned from the model that contains a string representing the <see cref="FunctionDeclaration.Name"/> with the arguments and their values.
    /// </summary>
    public class FunctionResponse
    {
        /// <summary>
        /// Required. The name of the function to call.
        /// Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 63.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Only appears in response. The function parameters and values in JSON object format.
        /// </summary>
        [JsonProperty("response")] public object Response { get; set; }
    }
}
