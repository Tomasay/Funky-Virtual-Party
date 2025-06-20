using System.Collections.Generic;
using Glitch9.IO.Json.Schema;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Configuration options for model generation and outputs.
    /// Not all parameters may be configurable for every model.
    /// </summary>
    public class GenerationConfig
    {
        /// <summary>
        /// Optional.
        /// The set of character sequences (up to 5) that will stop output generation.
        /// If specified, the API will stop at the first appearance of a stop sequence.
        /// The stop sequence will not be included as part of the response.
        /// </summary>
        [JsonProperty("stopSequences")] public string[] StopSequences { get; set; }

        /// <summary>
        /// Optional. Number of generated responses to return.
        /// Currently, this value can only be set to 1. If unset, this will default to 1.
        /// </summary>
        [JsonProperty("candidateCount")] public int CandidateCount { get; set; } = 1;

        /// <summary>
        /// Optional. The maximum number of tokens to include in a candidate.
        /// Note: The default value varies by model, see the Model.output_token_limit attribute of the Model returned from the getModel function.
        /// </summary>
        [JsonProperty("maxOutputTokens")] public int? MaxTokens { get; set; }

        /// <summary>
        /// Optional. Controls the randomness of the output.
        /// Note: The default value varies by model, see the Model.temperature attribute of the Model returned from the getModel function.
        /// Values can range from [0.0, 2.0].
        /// </summary>
        [JsonProperty("temperature")] public float? Temperature { get; set; }

        /// <summary>
        /// Optional. The maximum cumulative probability of tokens to consider when sampling.
        /// The model uses combined Top-k and nucleus sampling.
        /// Tokens are sorted based on their assigned probabilities so that only the most likely tokens are considered. Top-k sampling directly limits the maximum number of tokens to consider, while Nucleus sampling limits number of tokens based on the cumulative probability.
        /// Note: The default value varies by model, see the Model.top_p attribute of the Model returned from the getModel function.
        /// </summary>
        [JsonProperty("topP")] public float? TopP { get; set; }

        /// <summary>
        /// Optional. The maximum number of tokens to consider when sampling.
        /// Models use nucleus sampling or combined Top-k and nucleus sampling.Top-k sampling considers the set of topK most probable tokens. Models running with nucleus sampling don't allow topK setting.
        /// Note: The default value varies by model, see the Model.top_k attribute of the Model returned from the getModel function. Empty topK field in Model indicates the model doesn't apply top-k sampling and doesn't allow setting topK on requests.
        /// </summary>
        [JsonProperty("topK")] public int? TopK { get; set; }

        // Added 2025.03.30  

        /// <summary>
        /// Optional. MIME type of the generated candidate text.
        /// Supported MIME types are:
        /// - text/plain: (default) Text output.
        /// - application/json: JSON response in the response candidates.
        /// - text/x.enum: ENUM as a string response in the response candidates.
        /// Refer to the docs for a list of all supported text MIME types.
        /// Refer to the <see href="https://ai.google.dev/gemini-api/docs/file-prompting-strategies?_gl=1%2A10bjnrf%2A_up%2AMQ..%2A_ga%2ANjkzODk1OTc0LjE3NDMzMDY2ODM.%2A_ga_P1DBVKWT6V%2AMTc0MzMxODE4NC4yLjAuMTc0MzMxODE4NC4wLjAuMTQ4ODM2MjUwNg..#plain_text_formats">docs</see> for a list of all supported text MIME types.
        /// </summary>
        [JsonProperty("responseMimeType")] public string ResponseMimeType { get; set; } = "text/plain";

        /// <summary>
        /// Optional. Output schema of the generated candidate text. Schemas must be a subset of the OpenAPI schema and can be objects, primitives or arrays.
        /// If set, a compatible responseMimeType must also be set. Compatible MIME types: application/json: Schema for JSON response. Refer to the JSON text generation guide for more details.
        /// </summary>
        [JsonProperty("responseSchema")] public JsonSchema ResponseSchema { get; set; } = null;

        /// <summary>
        /// Optional. The requested modalities of the response.
        /// Represents the set of modalities that the model can return, and should be expected in the response.
        /// This is an exact match to the modalities of the response.
        /// A model may have multiple combinations of supported modalities.
        /// If the requested modalities do not match any of the supported combinations, an error will be returned.
        /// An empty list is equivalent to requesting only text.
        /// </summary>
        [JsonProperty("responseModalities")] public List<Modality> ResponseModalities { get; set; } = new();

        /// <summary>
        /// Optional. Seed used in decoding.
        /// If not set, the request uses a randomly generated seed.
        /// </summary>
        [JsonProperty("seed")] public int? Seed { get; set; } = null;

        /// <summary>
        /// Optional. Presence penalty applied to the next token's logprobs if the token has already been seen in the response.
        /// This penalty is binary on/off and not dependant on the number of times the token is used (after the first).
        /// Use frequencyPenalty for a penalty that increases with each use.
        /// A positive penalty will discourage the use of tokens that have already been used in the response, increasing the vocabulary.
        /// A negative penalty will encourage the use of tokens that have already been used in the response, decreasing the vocabulary.
        /// </summary>
        [JsonProperty("presencePenalty")] public float? PresencePenalty { get; set; }

        /// <summary>
        /// Optional. Frequency penalty applied to the next token's logprobs, multiplied by the number of times each token has been seen in the response so far.
        /// A positive penalty will discourage the use of tokens that have already been used, proportional to the number of times the token has been used: The more a token is used, the more difficult it is for the model to use that token again increasing the vocabulary of responses.
        /// Caution: A negative penalty will encourage the model to reuse tokens proportional to the number of times the token has been used. Small negative values will reduce the vocabulary of a response. Larger negative values will cause the model to start repeating a common token until it hits the maxOutputTokens limit.
        /// </summary>       
        [JsonProperty("frequencyPenalty")] public float? FrequencyPenalty { get; set; }

        /// <summary>
        /// Optional. If true, export the logprobs results in response.
        /// </summary>
        [JsonProperty("responseLogprobs")] public bool? ResponseLogprobs { get; set; }

        /// <summary>
        /// Optional. Only valid if responseLogprobs=True. This sets the number of top logprobs to return at each decoding step in the Candidate.logprobs_result.
        /// </summary>
        [JsonProperty("logprobs")] public int? Logprobs { get; set; }

        /// <summary>
        /// Optional. Enables enhanced civic answers. It may not be available for all models.
        /// </summary>
        [JsonProperty("enableEnhancedCivicAnswers")] public bool? EnableEnhancedCivicAnswers { get; set; }

        /// <summary>
        /// Optional. The speech generation config.
        /// </summary>
        [JsonProperty("speechConfig")] public SpeechConfig SpeechConfig { get; set; }

        /// <summary>
        /// Optional. If specified, the media resolution specified will be used.
        /// </summary>
        [JsonProperty("mediaResolution")] public MediaResolution? MediaResolution { get; set; } = null;

    }


    public class SpeechConfig
    {
        /// <summary>
        /// Optional. The configuration for the speaker to use.
        /// </summary>
        [JsonProperty("voiceConfig")] public VoiceConfig VoiceConfig { get; set; }
    }

    public class VoiceConfig
    {
        /// <summary>
        /// Optional. The configuration for the prebuilt voice to use.
        /// </summary>
        [JsonProperty("prebuiltVoiceConfig")] public PrebuiltVoiceConfig PrebuiltVoiceConfig { get; set; }
    }

    public class PrebuiltVoiceConfig
    {
        /// <summary>
        /// Optional. The name of the preset voice to use.
        /// </summary>
        [JsonProperty("voiceName")] public string VoiceName { get; set; }
    }

    public enum MediaResolution
    {
        /// <summary>
        /// Unspecified value.
        /// </summary>
        [ApiEnum("MEDIA_RESOLUTION_UNSPECIFIED")] Unspecified,

        /// <summary>
        /// Media resolution set to low (64 tokens).
        /// </summary>
        [ApiEnum("MEDIA_RESOLUTION_LOW")] Low,

        /// <summary>
        /// Media resolution set to medium (256 tokens).
        /// </summary>
        [ApiEnum("MEDIA_RESOLUTION_MEDIUM")] Medium,

        /// <summary>
        /// Media resolution set to high (zoomed reframing with 256 tokens).
        /// </summary>
        [ApiEnum("MEDIA_RESOLUTION_HIGH")] High,
    }
}