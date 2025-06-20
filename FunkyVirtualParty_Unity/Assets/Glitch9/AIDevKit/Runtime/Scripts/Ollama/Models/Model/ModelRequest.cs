using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Ollama
{
    // Create Model API
    public class CreateModelRequest : RequestBody
    {
        /// <summary>
        /// Required. Name of the model to create.
        /// </summary>
        [JsonProperty("model")] public Model Model { get; set; }

        /// <summary>
        /// Optional. Name of an existing model to create the new model from.
        /// </summary>
        [JsonProperty("from")] public string From { get; set; }

        /// <summary>
        /// Optional. Files to build the model from (filename to SHA256 hash).
        /// </summary>
        [JsonProperty("files")] public Dictionary<string, string> Files { get; set; }

        /// <summary>
        /// Optional. LORA adapters to apply (filename to SHA256 hash).
        /// </summary>
        [JsonProperty("adapters")] public Dictionary<string, string> Adapters { get; set; }

        /// <summary>
        /// Optional. Prompt template for the model.
        /// </summary>
        [JsonProperty("template")] public string Template { get; set; }

        /// <summary>
        /// Optional. License or licenses for the model.
        /// Can be a string or list of strings.
        /// </summary>
        [JsonProperty("license")] public StringOr<string> License { get; set; }

        /// <summary>
        /// Optional. System prompt for the model.
        /// </summary>
        [JsonProperty("system")] public string System { get; set; }

        /// <summary>
        /// Optional. Parameters to configure the model.
        /// </summary>
        [JsonProperty("parameters")] public ModelSettings Parameters { get; set; }

        /// <summary>
        /// Optional. A list of chat messages used to create a conversation.
        /// </summary>
        [JsonProperty("messages")] public List<ChatMessage> Messages { get; set; }

        /// <summary>
        /// Optional. If false, the response will be returned as a single object.
        /// </summary>
        [JsonProperty("stream")] public bool? Stream { get; set; }

        /// <summary>
        /// Optional. Type of quantization to apply to the model (e.g. q4_K_M).
        /// </summary>
        [JsonProperty("quantize")] public string Quantize { get; set; }
    }


    public class CopyModelRequest : RequestBody
    {
        /// <summary>
        /// Required. The id of the model to copy from.
        /// </summary>
        [JsonProperty("source")] public string SourceId { get; set; }

        /// <summary>
        /// Required. The id of the model to copy to.
        /// </summary>
        [JsonProperty("destination")] public string DestinationId { get; set; }
    }

    public class DeleteModelRequest : RequestBody
    {
        /// <summary>
        /// Required. The id of the model to delete.
        /// </summary>
        [JsonProperty("model")] public string ModelId { get; set; }
    }


    // Push/Pull Model API
    // Push: Upload a model to a remote model library (like ollama.ai) to share or back up.
    // Pull: Download a model from a remote model library to use locally.
    // Use this to move models between local development and remote hosting.
    public class PushPullModelRequest : RequestBody
    {
        /// <summary>
        /// Required. The name of the model to push or pull.
        /// For example: "llama3:latest" or "user/model:tag".
        /// </summary>
        [JsonProperty("model")] public Model Model { get; set; }

        /// <summary>
        /// Optional. Allow insecure connections to the model library.
        /// Only use this for local development or trusted networks.
        /// </summary>
        [JsonProperty("insecure")] public bool? Insecure { get; set; }

        /// <summary>
        /// Optional. If false, response will be returned as a single response object.
        /// If true or omitted, a stream of status messages is returned.
        /// </summary>
        [JsonProperty("stream")] public bool? Stream { get; set; }
    }
}