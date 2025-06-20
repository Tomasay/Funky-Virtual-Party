using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit
{
    /// <summary>Base class for all responses from AI models.</summary>
    public abstract class AIResponse
    {
        [JsonProperty("id")] public string Id { get; set; }

        /// <summary>
        /// The object type
        /// </summary>
        [JsonProperty("object")] public string Object { get; set; }

        /// <summary>
        /// ID of the model.
        /// </summary>
        [JsonProperty("model")] public string Model { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) of when this object was created.
        /// </summary>
        [JsonProperty("created_at")] public UnixTime? CreatedAt { get; set; }
#pragma warning disable IDE1006
        [JsonProperty("created")] private UnixTime? _created { get => CreatedAt; set => CreatedAt = value; }
#pragma warning restore IDE1006

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object. 
        /// This can be useful for storing additional information about the object in a structured format. 
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonProperty("metadata")] public Dictionary<string, string> Metadata { get; set; }

        /// <summary> 
        /// How much tokens were used for the request.
        /// </summary>
        [JsonProperty("usage")]
        public Usage Usage
        {
            get => _usage;
            set
            {
                _usage = value;
                if (value != null)
                    value.modelId = Model;
            }
        }

        [JsonIgnore] private Usage _usage;
    }
}