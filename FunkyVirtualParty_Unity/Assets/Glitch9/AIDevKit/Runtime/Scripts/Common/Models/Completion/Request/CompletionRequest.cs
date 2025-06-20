using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Use a custom JsonConverter to handle serialization & deserialization 
    /// of this class for each API that has LLM models.
    /// </summary> 
    public class CompletionRequest : CompletionRequestBase
    {
        /// <summary>
        /// Optional (Ollama-only). The text after the model response.
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Optional (Ollama-only). 
        /// The prompt template to use (overrides what is defined in the Modelfile).
        /// </summary>
        [JsonProperty("template")] public string Template { get; set; }

        /// <summary>
        /// Optional (Ollama-only). 
        /// If true no formatting will be applied to the prompt.
        /// </summary>
        [JsonProperty("raw")] public bool? Raw { get; set; }

        /// <summary>
        /// Optional (Ollama-only). 
        /// The context parameter returned from a previous request to /generate.
        /// </summary>
        [JsonProperty("context")] public List<int> Context { get; set; }

        public class Builder : CompletionRequestBuilder<Builder, CompletionRequest>
        {
            /// <summary>
            /// Optional (Ollama-only). The text after the model response.
            /// </summary>
            public Builder SetSuffix(string suffix)
            {
                _req.Suffix = suffix;
                return this;
            }

            /// <summary>
            /// Optional (Ollama-only). The prompt template to use (overrides what is defined in the Modelfile).
            /// </summary>
            public Builder SetTemplate(string template)
            {
                _req.Template = template;
                return this;
            }

            /// <summary>
            /// Optional (Ollama-only). If true no formatting will be applied to the prompt.
            /// </summary>
            public Builder SetRaw(bool raw)
            {
                _req.Raw = raw;
                return this;
            }

            /// <summary>
            /// Optional (Ollama-only). The context parameter returned from a previous request to /generate.
            /// </summary>
            public Builder SetContext(List<int> context)
            {
                _req.Context = context;
                return this;
            }
        }
    }
}
