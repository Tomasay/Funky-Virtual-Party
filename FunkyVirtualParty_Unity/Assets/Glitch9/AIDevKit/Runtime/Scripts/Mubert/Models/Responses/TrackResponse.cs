using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Mubert
{ 
    /// <summary>
    /// Response wrapper for RecordTrack method.
    /// </summary>
    public class TrackResponse : MubertResponse<TrackResponseParams>
    {
    }

    /// <summary>
    /// Contains the list of generated track tasks.
    /// </summary>
    public class TrackResponseParams
    {
        /// <summary>
        /// The list of track tasks created by the request.
        /// </summary>
        [JsonProperty("tasks")] public List<TrackTask> Tasks { get; set; }
    }

    /// <summary>
    /// Information about a single track generation task.
    /// </summary>
    public class TrackTask
    {
        /// <summary>
        /// The ID of the generation task.
        /// </summary>
        [JsonProperty("task_id")] public string TaskId { get; set; }

        /// <summary>
        /// The status code of the task (e.g., 1 = in progress, 2 = done).
        /// </summary>
        [JsonProperty("task_status_code")] public int TaskStatusCode { get; set; }

        /// <summary>
        /// The human-readable status message of the task.
        /// </summary>
        [JsonProperty("task_status_text")] public string TaskStatusText { get; set; }

        /// <summary>
        /// The download URL for the generated track (valid for 10 minutes).
        /// </summary>
        [JsonProperty("download_link")] public string DownloadUrl { get; set; }
    }
}
