using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public sealed class CodeInterpreterResources
    {
        /// <summary>
        /// A list of file IDs made available to the code_interpreter tool. 
        /// There can be a maximum of 20 files associated with the tool.
        /// </summary>
        [JsonProperty("file_ids")] string[] FileIds { get; set; }
    }
}