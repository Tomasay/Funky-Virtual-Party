
using System.Collections.Generic;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public enum UploadPurpose
    {
        Unknown,

        [ApiEnum("assistants")]
        Assistants,

        [ApiEnum("assistants_output")]
        AssistantsOutput,

        [ApiEnum("fine-tune")] // it's not underscore, it's a hyphen
        FineTune,

        [ApiEnum("fine-tune-results")] // it's not underscore, it's a hyphen
        FineTuneResults,

        [ApiEnum("vision")]
        Vision,

        [ApiEnum("batch")]
        Batch,

        [ApiEnum("batch_output")]
        BatchOutput,
    }

    /// <summary>
    /// The File object represents a document that has been uploaded to OpenAI.
    /// </summary>
    public class OpenAIFile : IApiFile
    {
        /// <summary>
        /// The file identifier, which can be referenced in the API endpoints.
        /// </summary>
        [JsonProperty("id")] public string Id { get; set; }

        /// <summary>
        /// The object type, which is always file.
        /// </summary>
        [JsonProperty("object")] public string Object { get; set; }

        /// <summary>
        /// The size of the file, in bytes
        /// </summary>
        [JsonProperty("bytes")] public int ByteSize { get; set; }

        /// <summary>
        /// The name of the file
        /// </summary>
        [JsonProperty("filename")] public string Name { get; set; }

        /// <summary>
        /// The intended purpose of the file.
        /// Supported values are fine-tune, fine-tune-results, Assistants, and Assistants_Output
        /// </summary>
        [JsonProperty("purpose")] public UploadPurpose? Purpose { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) of when this object was created.
        /// </summary>
        [JsonProperty("created_at")] public UnixTime CreatedAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) of when this object will expire.
        /// </summary>
        [JsonProperty("expires_at")] public UnixTime ExpiresAt { get; set; }

        [JsonIgnore] public Api Api => Api.OpenAI;
        [JsonIgnore] public MIMEType MimeType => MIMETypeUtil.ParseFromPath(Name);

        public Metadata BuildMetadata()
        {
            Dictionary<string, string> metadata = new();
            if (Purpose != null) metadata.Add("Purpose", Purpose.ToString());
            return new Metadata(metadata);
        }
    }
}