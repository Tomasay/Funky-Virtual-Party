using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class FileSearchCall : ToolCall
    {
        [JsonProperty("file_search")] public FileSearchTool FileSearch { get; set; }

        [JsonConstructor]
        public FileSearchCall()
        {
            Type = ToolType.FileSearch;
        }
    }
}