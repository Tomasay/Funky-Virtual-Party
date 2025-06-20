using Glitch9.IO.Files;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Google
{
    /// <summary>
    /// URI based data.
    /// </summary>
    public class GoogleFileData
    {
        /// <summary>
        /// Required. URI.
        /// </summary>
        [JsonProperty("fileUri")] public string Uri { get; set; }

        /// <summary>
        /// Optional.
        /// The IANA standard MIME type of the source data.
        /// </summary>
        [JsonProperty("mimeType")] public MIMEType? MimeType { get; set; }

        public GoogleFileData() { }
        public GoogleFileData(string fileUri) => Uri = fileUri;
    }
}