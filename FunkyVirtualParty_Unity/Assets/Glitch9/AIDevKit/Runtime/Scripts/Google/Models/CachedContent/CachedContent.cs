using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Google
{
    public class CachedContentRequest : GenerativeAIRequest { }

    /// <summary>
    /// Content that has been preprocessed and can be used in subsequent request to <see cref="ModelService"/>.
    /// Cached content can be only used with model it was created for.
    /// </summary>
    public class CachedContent
    {
        private const string CACHED_CONTENTS_NAME_PREFIX = "cachedContents/";
        [JsonProperty("contents")] public List<Content> Contents { get; set; } = new();
        [JsonProperty("tools")] public List<Tool> Tools { get; set; } = new();
        [JsonProperty("createTime")] public ZuluTime CreateTime { get; set; }
        [JsonProperty("updateTime")] public ZuluTime UpdateTime { get; set; }
        [JsonProperty("usageMetadata")] public Usage UsageMetadata { get; set; }
        [JsonProperty("expireTime")] public ZuluTime ExpireTime { get; set; }
        [JsonProperty("ttl")] public ZuluTime Ttl { get; set; }

        /// <summary>
        /// Optional. Identifier.
        /// The resource name referring to the cached content.
        /// Format: cachedContents/{id}
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name)) return null;
                return _name.Replace(CACHED_CONTENTS_NAME_PREFIX, "");
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                _name = $"{CACHED_CONTENTS_NAME_PREFIX}{value}";
            }
        }
        [JsonIgnore] private string _name;

        /// <summary>
        /// Optional. Immutable.
        /// The user-generated meaningful display name of the cached content.
        /// Maximum 128 Unicode characters.
        /// </summary>
        [JsonProperty("displayName")] public string DisplayName { get; set; }
        [JsonProperty("model")] public Model Model { get; set; }
        [JsonProperty("systemInstruction")] public Content SystemInstruction { get; set; }
        [JsonProperty("toolConfig")] public ToolConfig ToolConfig { get; set; }
    }
}