using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    public class CorporaQueryRequest : GenerativeAIRequest
    {
        /// <summary>
        /// Required. Query string to perform semantic search.
        /// </summary>
        [JsonProperty("query")] public string Query { get; set; }


        /*Optional. Filter for Chunk and Document metadata. Each MetadataFilter object should correspond to a unique key. Multiple MetadataFilter objects are joined by logical "AND"s.
           
           Example query at document level: (year >= 2020 OR year < 2010) AND (genre = drama OR genre = action)
           
           MetadataFilter object list: metadataFilters = [ {key = "document.custom_metadata.year" conditions = [{int_value = 2020, operation = GREATER_EQUAL}, {int_value = 2010, operation = LESS}]}, {key = "document.custom_metadata.year" conditions = [{int_value = 2020, operation = GREATER_EQUAL}, {int_value = 2010, operation = LESS}]}, {key = "document.custom_metadata.genre" conditions = [{stringValue = "drama", operation = EQUAL}, {stringValue = "action", operation = EQUAL}]}]
           
           Example query at chunk level for a numeric range of values: (year > 2015 AND year <= 2020)
           
           MetadataFilter object list: metadataFilters = [ {key = "chunk.custom_metadata.year" conditions = [{int_value = 2015, operation = GREATER}]}, {key = "chunk.custom_metadata.year" conditions = [{int_value = 2020, operation = LESS_EQUAL}]}]
           
           Note: "AND"s for the same key are only supported for numeric values. String values only support "OR"s for the same key.*/

        [JsonProperty("metadataFilters")] public MetadataFilter[] MetadataFilters { get; set; }


        /// <summary>
        /// Optional. The maximum number of Chunks to return. The service may return fewer Chunks.
        /// If unspecified, at most 10 Chunks will be returned. The maximum specified result count is 100.
        /// </summary>
        [JsonProperty("resultsCount")] public int ResultsCount { get; set; }
    }
}