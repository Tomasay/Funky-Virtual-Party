using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class FileCounts
    {
        /// <summary>
        /// The number of files that are currently being processed.
        /// </summary>
        [JsonProperty("in_progress")] public int? InProgress { get; set; }
        
        /// <summary>
        /// The number of files that have been successfully processed.
        /// </summary>
        [JsonProperty("completed")] public int? Completed { get; set; }

        /// <summary>
        /// The number of files that have failed to process.
        /// </summary>
        [JsonProperty("failed")] public int? Failed { get; set; }
        
        /// <summary>
        /// The number of files that were cancelled.
        /// </summary>
        [JsonProperty("cancelled")] public int? Cancelled { get; set; }

        /// <summary>
        /// The total number of files.
        /// </summary>
        [JsonProperty("total")] public int? Total { get; set; }
    }
}