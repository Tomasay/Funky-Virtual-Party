using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// A <see cref="Corpus"/> is a collection of Documents. A project can create up to 5 corpora.
    /// </summary>
    public class Corpus
    {
        /// <summary>
        /// Immutable. Identifier.
        /// The <see cref="Corpus"/> resource name.
        /// The ID (name excluding the "corpora/" prefix) can contain up to 40 characters that are lowercase alphanumeric or dashes (-).
        /// The ID cannot start or end with a dash.
        /// If the name is empty on create, a unique name will be derived from displayName along with a 12 character random suffix.
        /// <para>Example: corpora/my-awesome-corpora-123a456b789c</para>
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Optional. The human-readable display name for the <see cref="Corpus"/>.
        /// The display name must be no more than 512 characters in length, including spaces.
        /// <para>Example: "Docs on Semantic Retriever"</para>
        /// </summary>
        [JsonProperty("displayName")] public string DisplayName { get; set; }

        /// <summary>
        /// Output only.
        /// The Timestamp of when the <see cref="Corpus"/> was created.
        /// </summary>
        [JsonProperty("createTime")] public ZuluTime? CreateTime { get; set; }

        /// <summary>
        /// Output only.
        /// The Timestamp of when the <see cref="Corpus"/> was last updated.
        /// </summary>
        [JsonProperty("updateTime")] public ZuluTime? UpdateTime { get; set; }
    }
}