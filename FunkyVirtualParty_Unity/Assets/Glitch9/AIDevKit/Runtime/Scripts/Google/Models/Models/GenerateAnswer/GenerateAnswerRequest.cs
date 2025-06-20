using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// Generates a response from the model given an input GenerateContentRequest.
    /// Input capabilities differ between models, including tuned models.
    /// See the <see href="https://ai.google.dev/models/gemini">model guide</see>
    /// and <see href="https://ai.google.dev/docs/model_tuning_guidance">tuning guide</see> for details.
    /// </summary>
    public class GenerateAnswerRequest : GenerativeAIRequest
    {
        [JsonProperty("contents")] public List<Content> Contents { get; set; }
        [JsonProperty("answerStyle")] public AnswerStyle AnswerStyle { get; set; }
        [JsonProperty("safetySettings")] public List<SafetySetting> SafetySettings { get; set; }
        [JsonProperty("inlinePassages")] public GroundingPassages InlinePassages { get; set; }
        [JsonProperty("semanticRetriever")] public SemanticRetrieverConfig SemanticRetriever { get; set; }
        [JsonProperty("temperature")] public double Temperature { get; set; }
    }

    public enum AnswerStyle
    {
        /// <summary>
        /// Unspecified answer style.
        /// </summary>
        [ApiEnum("ANSWER_STYLE_UNSPECIFIED")] Unspecified,

        /// <summary>
        /// Succint but abstract style.
        /// </summary>
        [ApiEnum("ABSTRACTIVE")] Abstractive,

        /// <summary>
        /// Very brief and extractive style.
        /// </summary>
        [ApiEnum("EXTRACTIVE")] Extractive,

        /// <summary>
        /// Verbose style including extra details.
        /// The response may be formatted as a sentence, paragraph, multiple paragraphs, or bullet points, etc
        /// </summary>
        [ApiEnum("VERBOSE")] Verbose
    }

    /// <summary>
    /// A repeated list of passages.
    /// </summary>
    public class GroundingPassages
    {
        [JsonProperty("passages")] public GroundingPassage[] Passages { get; set; }
    }

    /// <summary>
    /// Passage included inline with a grounding configuration.
    /// </summary>
    public class GroundingPassage
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("content")] public Content Content { get; set; }
    }


    /// <summary>
    /// Configuration for retrieving grounding content from a Corpus or Document created using the Semantic Retriever API.
    /// </summary>
    public class SemanticRetrieverConfig
    {
        /// <summary>
        /// Required.
        /// Name of the resource for retrieval, e.g. corpora/123 or corpora/123/documents/abc.
        /// </summary>
        [JsonProperty("source")] public string Source { get; set; }

        /// <summary>
        /// Required.
        /// Query to use for similarity matching Chunks in the given resource.
        /// </summary>
        [JsonProperty("query")] public Content Query { get; set; }

        /// <summary>
        /// Optional.
        /// Filters for selecting Documents and/or Chunks from the resource.
        /// </summary>
        [JsonProperty("metadataFilters")] public List<MetadataFilter> MetadataFilters { get; set; }

        /// <summary>
        /// Optional.
        /// Maximum number of relevant Chunks to retrieve.
        /// </summary>
        [JsonProperty("maxChunksCount")] public int MaxChunksCount { get; set; }

        /// <summary>
        /// Optional.
        /// Minimum relevance score for retrieved relevant Chunks.
        /// </summary>
        [JsonProperty("minimumRelevanceScore")] public float MinimumRelevanceScore { get; set; }
    }

    /// <summary>
    /// User provided filter to limit retrieval based on Chunk or Document level metadata values.
    /// Example (genre = drama OR genre = action): key = "document.custom_metadata.genre" conditions = [{stringValue = "drama", operation = EQUAL}, {stringValue = "action", operation = EQUAL}]
    /// </summary>
    public class MetadataFilter
    {
        /// <summary>
        /// Required. The key of the metadata to filter on.
        /// </summary>
        [JsonProperty("key")] public string Key { get; set; }

        /// <summary>
        /// Required. The Conditions for the given key that will trigger this filter. Multiple Conditions are joined by logical ORs.
        /// </summary>
        [JsonProperty("conditions")] public List<Condition> Conditions { get; set; }
    }

    /// <summary>
    /// Filter condition applicable to a single key.
    /// </summary>
    public class Condition
    {
        /// <summary>
        /// Required.
        /// Operator applied to the given key-value pair to trigger the condition.
        /// </summary>
        [JsonProperty("operation")] public Operator Operation { get; set; }

        /// <summary>
        /// The string value to filter the metadata on.
        /// </summary>
        [JsonProperty("stringValue")] public string StringValue { get; set; }

        /// <summary>
        /// The numeric value to filter the metadata on.
        /// </summary>
        [JsonProperty("numericValue")] public float NumericValue { get; set; }
    }

    /// <summary>
    /// Defines the valid operators that can be applied to a key-value pair.
    /// </summary>
    public enum Operator
    {
        /// <summary>
        /// The default value. This value is unused.
        /// </summary>
        [ApiEnum("OPERATOR_UNSPECIFIED")] Unspecified,

        /// <summary>
        /// Supported by numeric.
        /// </summary>
        [ApiEnum("LESS")] Less,

        /// <summary>
        /// Supported by numeric.
        /// </summary>
        [ApiEnum("LESS_EQUAL")] LessEqual,

        /// <summary>
        /// Supported by numeric & string.
        /// </summary>
        [ApiEnum("EQUAL")] Equal,

        /// <summary>
        /// Supported by numeric.
        /// </summary>
        [ApiEnum("GREATER_EQUAL")] GreaterEqual,

        /// <summary>
        /// Supported by numeric.
        /// </summary>
        [ApiEnum("GREATER")] Greater,

        /// <summary>
        /// Supported by numeric & string.
        /// </summary>
        [ApiEnum("NOT_EQUAL")] NotEqual,

        /// <summary>
        /// Supported by string only when CustomMetadata value type for the given key has a stringListValue.
        /// </summary>
        [ApiEnum("INCLUDES")] Includes,

        /// <summary>
        /// Supported by string only when CustomMetadata value type for the given key has a stringListValue.
        /// </summary>
        [ApiEnum("EXCLUDES")] Excludes
    }
}