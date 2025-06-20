using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI.Administration
{
    /// <summary>
    /// Renamed from AssistantObject to Assistant (2024.06.14)
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Optional. The Unix timestamp (in seconds) of when the project was archived or null.
        /// </summary>
        [JsonProperty("archived_at")] public UnixTime? ArchivedAt { get; set; }

        /// <summary>
        /// Required. The Unix timestamp (in seconds) of when the project was created.
        /// </summary>
        [JsonProperty("created_at")] public UnixTime CreatedAt { get; set; }

        /// <summary>
        /// Required. The identifier, which can be referenced in API endpoints
        /// </summary>
        [JsonProperty("id")] public string Id { get; set; }

        /// <summary>
        /// Required. The name of the project. This appears in reporting.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Required. The object type, which is always organization.project
        /// </summary>
        [JsonProperty("object")] public string Object { get; set; }

        /// <summary>
        /// Required. active or archived
        /// </summary>
        [JsonProperty("status")] public string Status { get; set; }
    }
}