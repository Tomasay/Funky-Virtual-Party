
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Base class for <see cref="Run"/> and <see cref="RunStep"/>.
    /// </summary>
    public class RunBase : AIResponse
    {
        /// <summary>
        /// The ID of the AssistantObject used for execution of this run.
        /// </summary>
        [JsonProperty("assistant_id")] public string AssistantId { get; set; }

        /// <summary>
        /// The ID of the thread that was executed on as a part of this run.
        /// </summary>
        [JsonProperty("thread_id")] public string ThreadId { get; set; }

        /// <summary>
        /// The status of the run, which can be either Queued, In_Progress, Requires_Action, Cancelling, Cancelled, Failed, Completed, or Expired.
        /// </summary>
        [JsonProperty("status")] public RunStatus? Status { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run will expire.
        /// </summary>
        [JsonProperty("expires_at")] public UnixTime? ExpiresAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was started.
        /// </summary>
        [JsonProperty("started_at")] public UnixTime? StartedAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was cancelled.
        /// </summary>
        [JsonProperty("cancelled_at")] public UnixTime? CancelledAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run was Completed.
        /// </summary>
        [JsonProperty("completed_at")] public UnixTime? CompletedAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the run Failed.
        /// </summary>
        [JsonProperty("failed_at")] public UnixTime? FailedAt { get; set; }

        /// <summary>
        /// The last error associated with this run. Will be null if there are no errors.
        /// </summary>
        [JsonProperty("last_error")] public ErrorResponse LastError { get; set; }
    }
}