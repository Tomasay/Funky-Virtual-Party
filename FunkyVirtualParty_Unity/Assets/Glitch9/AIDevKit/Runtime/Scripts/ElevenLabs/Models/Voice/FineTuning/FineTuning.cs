using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{
    /// <summary>
    /// The state of the fine-tuning process.
    /// </summary>
    public enum FineTuningState
    {
        [ApiEnum("not_started")] NotStarted,
        [ApiEnum("queued")] Queued,
        [ApiEnum("fine_tuning")] FineTuning,
        [ApiEnum("fine_tuned")] FineTuned,
        [ApiEnum("failed")] Failed,
        [ApiEnum("delayed")] Delayed
    }

    /// <summary>
    /// Fine-tuning information for the voice.
    /// </summary>
    public class FineTuning
    {
        /// <summary>
        /// Whether the user is allowed to fine-tune the voice.
        /// </summary>
        [JsonProperty("is_allowed_to_fine_tune")]
        public bool IsAllowedToFineTune { get; set; }

        /// <summary>
        /// The state of the fine-tuning process for each model.
        /// </summary>
        [JsonProperty("state")]
        public Dictionary<string, FineTuningState> State { get; set; }

        /// <summary>
        /// List of verification failures in the fine-tuning process.
        /// </summary>
        [JsonProperty("verification_failures")]
        public List<string> VerificationFailures { get; set; }

        /// <summary>
        /// The number of verification attempts in the fine-tuning process.
        /// </summary>
        [JsonProperty("verification_attempts_count")]
        public int VerificationAttemptsCount { get; set; }

        /// <summary>
        /// Whether a manual verification was requested for the fine-tuning process.
        /// </summary>
        [JsonProperty("manual_verification_requested")]
        public bool ManualVerificationRequested { get; set; }

        /// <summary>
        /// Optional. The language of the fine-tuning process.
        /// </summary>
        [JsonProperty("language")]
        public string Language { get; set; }

        /// <summary>
        /// Optional. The progress of the fine-tuning process.
        /// </summary>
        [JsonProperty("progress")]
        public Dictionary<string, double?> Progress { get; set; }

        /// <summary>
        /// Optional. The message of the fine-tuning process.
        /// </summary>
        [JsonProperty("message")]
        public Dictionary<string, string> Message { get; set; }

        /// <summary>
        /// Optional. The duration of the dataset in seconds.
        /// </summary>
        [JsonProperty("dataset_duration_seconds")]
        public double? DatasetDurationSeconds { get; set; }

        /// <summary>
        /// Optional. The number of verification attempts.
        /// </summary>
        [JsonProperty("verification_attempts")]
        public List<VerificationAttempt> VerificationAttempts { get; set; }
    }


}