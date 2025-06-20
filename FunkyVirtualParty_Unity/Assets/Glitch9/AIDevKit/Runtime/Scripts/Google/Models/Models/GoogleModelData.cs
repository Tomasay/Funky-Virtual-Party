using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    public class GoogleModelData : IModelData
    {
        [JsonProperty("name")] public string Id { get; set; }

        /// <summary>
        /// The model's supported generation methods. 
        /// The corresponding API method names are defined as Pascal case strings, such as generateMessage and generateContent.
        /// </summary>
        [JsonProperty("supportedGenerationMethods")] public string[] SupportedGenerationMethods { get; set; }
        [JsonProperty("temperature")] public double Temperature { get; set; }
        [JsonProperty("topP")] public double TopP { get; set; }
        [JsonProperty("topK")] public int TopK { get; set; }


        // IGenAIModel implementation shared ------------------------------------------------------------------------------
        [JsonProperty("baseModelId")] public string BaseId { get; set; }
        [JsonProperty("displayName")] public string Name { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("version")] public string ModelVersion { get; set; }
        [JsonProperty("inputTokenLimit")] public int? InputTokenLimit { get; set; }
        [JsonProperty("outputTokenLimit")] public int? OutputTokenLimit { get; set; }


        // IModelData implementation -------------------------------------------------------------------------------- 
        [JsonIgnore] public Api Api => Api.Google;
        [JsonIgnore] public string Provider => Api.Google.ToString();
        [JsonIgnore] public string OwnedBy => "Google";
        [JsonIgnore] public bool? IsFineTuned => !string.IsNullOrEmpty(BaseId);
        [JsonIgnore] public ModelFeature? Feature => ResolveCapabilities(SupportedGenerationMethods, Id);

        /*

            Supported Generation Methods values and its matching ModelCapability:

            bidiGenerateContent     => LLM
            countMessageTokens      => LLM
            countTextTokens         => meanlingless
            countTokens             => meanlingless
            createCachedContent     => LLM
            createTunedModel        => Means the model is trainable
            createTunedTextModel    => Means the model is trainable
            embedContent            => EMB
            embedText               => EMB
            generateAnswer          => LLM
            generateContent         => LLM
            generateMessage         => LLM
            generateText            => LLM
            predict                 => IMG (이미지 생성)

       */

        // Google Model's creation date is sometimes included in the Description, but not always.

        internal static ModelFeature ResolveCapabilities(string[] supportedGenerationMethods, string id)
        {
            ModelFeature cap = ModelFeature.None;

            if (id.Contains("veo-")) cap |= ModelFeature.VideoGeneration;
            if (id.Contains("gemini-2.0-flash-live")) cap |= ModelFeature.SpeechGeneration | ModelFeature.SpeechRecognition;
            if (id.Contains("gemini")) cap |= ModelFeature.TextGeneration | ModelFeature.Streaming | ModelFeature.StructuredOutputs | ModelFeature.FunctionCalling;

            foreach (string method in supportedGenerationMethods)
            {
                if (method.Contains("bibiGenerate")
                | method.Contains("generateAnswer")
                | method.Contains("generateMessage")
                | method.Contains("generateContent")
                | method.Contains("generateText")) cap |= ModelFeature.TextGeneration;
                if (method.Contains("createCached")) cap |= ModelFeature.Caching;
                if (method.Contains("createTuned")) cap |= ModelFeature.FineTuning;
                if (method.Contains("embed")) cap |= ModelFeature.TextEmbedding;
                if (method.Contains("predict")) cap |= ModelFeature.ImageGeneration;
            }

            return cap;
        }
    }
}
