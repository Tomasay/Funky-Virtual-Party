using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class FileRequest : AIRequest
    {
        [JsonProperty("file_id")] public string FileId { get; set; }
        public FileRequest(string fileId) => FileId = fileId;
    }
}