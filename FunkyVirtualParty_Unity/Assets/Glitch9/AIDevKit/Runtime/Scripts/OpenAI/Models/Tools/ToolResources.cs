using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// A set of resources that are made available to the assistant's tools in this thread. 
    /// The resources are specific to the type of tool. 
    /// For example, the code_interpreter tool requires a list of file IDs, 
    /// while the file_search tool requires a list of vector store IDs.
    /// </summary>
    public class ToolResources
    {
        [JsonProperty("code_interpreter")] public CodeInterpreterResources CodeInterpreter { get; set; }
        [JsonProperty("file_search")] public FileSearchResources FileSearch { get; set; }
    }
}