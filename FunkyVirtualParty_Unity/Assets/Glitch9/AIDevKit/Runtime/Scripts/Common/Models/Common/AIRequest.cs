using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit
{
    public abstract class AIRequest : RequestBody
    {
        /// <summary>
        /// The AI model to use for the request.
        /// </summary>
        [JsonProperty("model")] public Model Model { get; set; }

        /// <summary>
        /// The number of responses to generate. 
        /// Defaults to 1.
        /// </summary>
        [JsonProperty("n")] public int? N { get; set; }

        /// <summary>
        /// Optional (OpenAI only).
        /// Custom metadata in key-value format (max 16 pairs).
        /// Keys: max 64 characters. Values: max 512 characters.
        /// Useful for storing additional structured data.
        /// </summary>
        [JsonProperty("metadata")] public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Optional (OpenAI only).
        /// A unique identifier for the end user.
        /// Helps OpenAI monitor and prevent abuse.
        /// </summary>
        [JsonProperty("user")] public string User { get; set; }

        public abstract class ModelRequestBuilder<TBuilder, TRequest> : RequestBodyBuilder<TBuilder, TRequest>
            where TBuilder : ModelRequestBuilder<TBuilder, TRequest>
            where TRequest : AIRequest
        {
            public virtual TBuilder SetUser(string user)
            {
                _req.User = user;
                return (TBuilder)this;
            }

            public TBuilder SetModel(Model model)
            {
                _req.Model = model;
                return (TBuilder)this;
            }

            public TBuilder SetMetadata(Dictionary<string, string> metadata)
            {
                if (metadata.IsNullOrEmpty()) return (TBuilder)this;
                _req.Metadata = metadata;
                return (TBuilder)this;
            }

            public TBuilder SetN(int n)
            {
                if (n < 2) return (TBuilder)this;
                _req.N = n;
                return (TBuilder)this;
            }

            // public virtual TBuilder SetResponseFormat(ResponseFormat responseFormat)
            // {
            //     _req.ResponseFormat = responseFormat;
            //     return (TBuilder)this;
            // }
        }
    }
}
