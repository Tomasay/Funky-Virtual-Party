using Glitch9.IO.Files;
using Glitch9.IO.Json.Schema;
using System;
using System.Collections.Generic;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Use a custom JsonConverter to handle serialization & deserialization 
    /// of this class for each API that has LLM models.
    /// </summary> 
    public abstract class CompletionRequestBase : AIRequest
    {
        /// <summary>
        /// Required if using OpenAI legacy models.
        /// Required for OpenRouter completion requests.
        /// The text prompt to complete
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Optional. The system message to use for the model.
        /// </summary>
        public string SystemInstruction { get; set; }

        /// <summary>
        /// Optional. Enable streaming of results.
        /// Defaults to false
        /// </summary>
        public bool? Stream { get; set; }

        /// <summary>
        /// Optional. Whether to include usage information in the response
        /// </summary>
        public StreamOptions StreamOptions { get; set; }

        /// <summary>
        /// Configuration options for model generation and outputs.
        /// Not all parameters may be configurable for every model.
        /// </summary>
        public ModelSettings ModelSettings { get; set; }

        /// <summary>
        /// An object specifying the format that the model must output.
        /// 
        /// Setting to { "type": "json_schema", "json_schema": { ...} }  
        /// enables Structured Outputs which ensures the model will match your supplied JSON schema.Learn more in the Structured Outputs guide.
        /// 
        /// Setting to { "type": "json_object" }
        /// enables the older JSON mode, which ensures the message the model generates is valid JSON.Using json_schema is preferred for models that support it.
        /// </summary>
        public ResponseFormat ResponseFormat { get; set; }

        /// <summary>
        /// Optional. Configuration for model reasoning/thinking tokens
        /// </summary>
        public ReasoningOptions ReasoningOptions { get; set; }

        /// <summary>
        /// Optional (OpenRouter-only). 
        /// Alternate list of models for routing overrides.
        /// </summary>
        public List<string> Models { get; set; }

        /// <summary>
        /// Optional (OpenRouter-only). 
        /// List of prompt transforms.
        /// </summary>
        public List<string> Transforms { get; set; }

        /// <summary>
        /// Optional (Ollama-only). 
        /// Controls how long the model will stay loaded into memory following the request.
        /// </summary>
        public string KeepAlive { get; set; }


        public List<IFile> AttachedFiles { get; set; }


        public class CompletionRequestBuilder<TBuilder, TRequest> : ModelRequestBuilder<TBuilder, TRequest>
            where TBuilder : CompletionRequestBuilder<TBuilder, TRequest>
            where TRequest : CompletionRequestBase
        {
            public TBuilder SetStream(bool stream = true)
            {
                _req.Stream = stream;
                return this as TBuilder;
            }

            public TBuilder IncludeUsage(bool includeUsage = true)
            {
                _req.StreamOptions ??= new StreamOptions();
                _req.StreamOptions.IncludeUsage = includeUsage;
                return this as TBuilder;
            }

            public TBuilder SetPrompt(string prompt)
            {
                if (string.IsNullOrEmpty(prompt)) return this as TBuilder;
                _req.Prompt = prompt;
                return this as TBuilder;
            }

            public TBuilder SetInstruction(string systemInstruction)
            {
                if (string.IsNullOrEmpty(systemInstruction)) return this as TBuilder;
                _req.SystemInstruction = systemInstruction;
                return this as TBuilder;
            }

            public TBuilder SetResponseFormat(ResponseFormat responseFormat)
            {
                if (responseFormat == null) return this as TBuilder;
                _req.ResponseFormat = responseFormat;
                return this as TBuilder;
            }

            public TBuilder SetJsonSchema(Type type)
            {
                if (type == null) return this as TBuilder;

                StrictJsonSchemaAttribute attribute = AttributeCache<StrictJsonSchemaAttribute>.Get(type);
                if (attribute == null) throw new ArgumentNullException(nameof(attribute), $"No {typeof(StrictJsonSchemaAttribute).Name} found for type '{type}'.");

                JsonSchema jsonSchema = JsonSchema.Create(type);
                StrictJsonSchema openAIJsonSchema = new(attribute, jsonSchema);

                return SetJsonSchema(openAIJsonSchema);
            }

            public TBuilder SetJsonSchema(StrictJsonSchema jsonSchema)
            {
                if (jsonSchema == null) return this as TBuilder;
                _req.ResponseFormat = new JsonSchemaFormat(jsonSchema);
                return this as TBuilder;
            }

            public TBuilder SetModelSettings(ModelSettings settings)
            {
                if (settings == null) return this as TBuilder;
                _req.ModelSettings = settings;
                return this as TBuilder;
            }

            public TBuilder SetModels(List<string> models)
            {
                if (models.IsNullOrEmpty()) return this as TBuilder;
                _req.Models = models;
                return this as TBuilder;
            }

            public TBuilder SetTransforms(List<string> transforms)
            {
                if (transforms.IsNullOrEmpty()) return this as TBuilder;
                _req.Transforms = transforms;
                return this as TBuilder;
            }

            public TBuilder SetReasoningOptions(ReasoningOptions options)
            {
                if (options == null) return this as TBuilder;
                _req.ReasoningOptions = options;
                return this as TBuilder;
            }

            public TBuilder SetReasoningEffort(ReasoningEffort effort)
            {
                _req.ReasoningOptions ??= new ReasoningOptions();
                _req.ReasoningOptions.Effort = effort;
                return this as TBuilder;
            }

            public TBuilder SetReasoningEffort(int maxTokens)
            {
                _req.ReasoningOptions ??= new ReasoningOptions();
                _req.ReasoningOptions.MaxTokens = maxTokens;
                return this as TBuilder;
            }

            public TBuilder ExcludeReasoning(bool exclude = true)
            {
                _req.ReasoningOptions ??= new ReasoningOptions();
                _req.ReasoningOptions.Exclude = exclude;
                return this as TBuilder;
            }

            public TBuilder SetAttachedFiles(List<IFile> files)
            {
                if (files.IsNullOrEmpty()) return this as TBuilder;
                _req.AttachedFiles = files;
                return this as TBuilder;
            }
        }
    }
}
