using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class Batch : AIResponse
    {
        /// <summary>
        /// The OpenAI API endpoint used by the batch.
        /// </summary>
        [JsonProperty("endpoint")] public string Endpoint { get; set; }

        /// <summary>
        /// The errors that occurred during the batch.
        /// </summary>
        [JsonProperty("errors")] public BatchErrors Errors { get; set; }

        /// <summary>
        /// The ID of the input file for the batch.
        /// </summary>
        [JsonProperty("input_file_id")] public string InputFileId { get; set; }

        /// <summary>
        /// The time frame within which the batch should be processed.
        /// </summary>
        [JsonProperty("completion_window")] public string CompletionWindow { get; set; }

        /// <summary>
        /// The current status of the batch.
        /// </summary>
        [JsonProperty("status")] public string Status { get; set; }

        /// <summary>
        /// The ID of the file containing the outputs of successfully executed requests.
        /// </summary>
        [JsonProperty("output_file_id")] public string OutputFileId { get; set; }

        /// <summary>
        /// The ID of the file containing the outputs of requests with errors.
        /// </summary>
        [JsonProperty("error_file_id")] public string ErrorFileId { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch was created.
        /// </summary>
        [JsonProperty("in_progress_at")] public UnixTime? InProgressAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch started processing.
        /// </summary>
        [JsonProperty("expires_at")] public UnixTime? ExpiresAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch started finalizing.
        /// </summary>
        [JsonProperty("finalizing_at")] public UnixTime? FinalizingAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch was completed.
        /// </summary>
        [JsonProperty("completed_at")] public UnixTime? CompletedAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch failed.
        /// </summary>
        [JsonProperty("failed_at")] public UnixTime? FailedAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch expired.
        /// </summary>
        [JsonProperty("expired_at")] public UnixTime? ExpiredAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch started cancelling.
        /// </summary>
        [JsonProperty("cancelling_at")] public UnixTime? CancellingAt { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the batch was cancelled.
        /// </summary>
        [JsonProperty("cancelled_at")] public UnixTime? CancelledAt { get; set; }

        /// <summary>
        /// The request counts for different statuses within the batch.
        /// </summary>
        [JsonProperty("request_counts")] public RequestCounts RequestCounts { get; set; }
    }


    /// <summary>
    /// The errors that occurred during the batch.
    /// </summary>
    public class BatchErrors
    {
        /// <summary>
        /// The object type, which is always list.
        /// </summary>
        [JsonProperty("object")] public string Object { get; set; }

        /// <summary>
        /// The errors that occurred during the batch.
        /// </summary>
        [JsonProperty("data")] public BatchErrorData[] Data { get; set; }
    }


    public class BatchErrorData
    {
        /// <summary>
        /// An error code identifying the error type.
        /// </summary>
        [JsonProperty("code")] public string Code { get; set; }

        /// <summary>
        /// A human-readable message providing more details about the error.
        /// </summary>
        [JsonProperty("message")] public string Message { get; set; }

        /// <summary>
        /// The name of the parameter that caused the error, if applicable.
        /// </summary>
        [JsonProperty("param")] public string Param { get; set; }

        /// <summary>
        /// The line number of the input file where the error occurred, if applicable.
        /// </summary>
        [JsonProperty("line")] public int? Line { get; set; }
    }


    /// <summary>
    /// The request counts for different statuses within the batch.
    /// </summary>
    public class RequestCounts
    {
        /// <summary>
        /// Total number of requests in the batch.
        /// </summary>
        [JsonProperty("total")] public int? Total { get; set; }

        /// <summary>
        /// Number of requests that have been completed successfully.
        /// </summary>
        [JsonProperty("completed")] public int? Completed { get; set; }

        /// <summary>
        /// Number of requests that have failed.
        /// </summary>
        [JsonProperty("failed")] public int? Failed { get; set; }
    }
}