using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// A <see cref="Document"/> is a collection of Chunks. A Corpus can have a maximum of 10,000 Documents.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Immutable. Identifier.
        /// The  <see cref="Document"/> resource name.
        /// The ID (name excluding the "corpora/*/documents/" prefix) can contain up to 40 characters that are lowercase alphanumeric or dashes (-).
        /// The ID cannot start or end with a dash.
        /// If the name is empty on create, a unique name will be derived from displayName along with a 12 character random suffix.
        /// Example: corpora/{corpus_id}/documents/my-awesome-doc-123a456b789c
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Optional.
        /// The human-readable display name for the  <see cref="Document"/>.
        /// The display name must be no more than 512 characters in length, including spaces.
        /// Example: "Semantic Retriever Documentation"
        /// </summary>
        [JsonProperty("displayName")] public string DisplayName { get; set; }

        /// <summary>
        /// Optional. User provided custom metadata stored as key-value pairs used for querying. A  <see cref="Document"/> can have a maximum of 20 CustomMetadata.
        /// </summary>
        [JsonProperty("customMetadata"), JsonConverter(typeof(CustomMetadataConverter))]
        public Dictionary<string, CustomMetadataValue> CustomMetadata { get; set; }

        /// <summary>
        /// Output only. The Timestamp of when the  <see cref="Document"/> was last updated.
        /// </summary>
        [JsonProperty("updateTime")] public ZuluTime? UpdateTime { get; set; }

        /// <summary>
        /// Output only. The Timestamp of when the  <see cref="Document"/> was created.
        /// </summary>
        [JsonProperty("createTime")] public ZuluTime? CreateTime { get; set; }
    }
}