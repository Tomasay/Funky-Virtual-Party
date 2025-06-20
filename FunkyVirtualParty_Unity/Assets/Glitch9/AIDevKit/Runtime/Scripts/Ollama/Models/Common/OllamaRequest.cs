using System;
using Glitch9.IO.Json.Schema;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Ollama
{
    public class OllamaRequest : RequestBody
    {
        /// <summary>
        /// Required. The model name.
        /// </summary>
        [JsonProperty("model")] public Model Model { get; set; }

        /// <summary>
        /// Optional. Additional model parameters listed in the documentation for the Modelfile such as temperature.
        /// </summary>
        [JsonProperty("options")] public ModelSettings Options { get; set; }

        /// <summary>
        /// Optional. Controls how long the model will stay loaded into memory following the request.
        /// </summary>
        [JsonProperty("keep_alive")] public string KeepAlive { get; set; }
    }

    public class OllamaTextRequest : OllamaRequest
    {
        /// <summary>
        /// Optional. The format to return a response in. Format can be json or a JSON schema.
        /// If it's null, it's the plain text response.
        /// </summary>
        [JsonProperty("format")] public ResponseFormat Format { get; set; }

        /// <summary>
        /// Optional. System message to override what is defined in the Modelfile.
        /// </summary>
        [JsonProperty("system")] public string System { get; set; }

        /// <summary>
        /// Optional. If false the response will be returned as a single response object.
        /// </summary>
        [JsonProperty("stream")] public bool Stream { get; set; } = false;

        public class OllamaTextRequestBuilder<TBuilder, TRequest> : RequestBodyBuilder<TBuilder, TRequest>
            where TBuilder : OllamaTextRequestBuilder<TBuilder, TRequest>
            where TRequest : OllamaTextRequest
        {
            public TBuilder SetFormat(TextFormat format) { _req.Format = format; return (TBuilder)(object)this; }
            public TBuilder SetSystem(string system) { _req.System = system; return (TBuilder)(object)this; }
            public TBuilder SetStream(bool stream) { _req.Stream = stream; return (TBuilder)(object)this; }

            public TBuilder SetJsonSchema(Type type)
            {
                if (type == null) return (TBuilder)(object)this;

                StrictJsonSchemaAttribute attribute = AttributeCache<StrictJsonSchemaAttribute>.Get(type);
                if (attribute == null) throw new ArgumentNullException(nameof(attribute), "No OpenAIJsonSchemaAttribute found for type '" + type + "'.");

                JsonSchema jsonSchema = JsonSchema.Create(type);
                StrictJsonSchema openAIJsonSchema = new(attribute, jsonSchema);

                return SetJsonSchema(openAIJsonSchema);
            }

            public TBuilder SetJsonSchema(StrictJsonSchema jsonSchema)
            {
                if (jsonSchema == null) return (TBuilder)(object)this;

                _req.Format = new JsonSchemaFormat(jsonSchema);
                return (TBuilder)(object)this;
            }
        }
    }
}