
using Glitch9.IO.Networking.RESTApi;

using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// The fine_tuning.job object represents a fine-tuning job that has been created through the API.
    /// </summary>
    public class FineTuningJob : AIResponse
    {
        /// <summary>
        /// For fine-tuning jobs that have failed, this will contain more information on the cause of the failure.
        /// </summary>
        [JsonProperty("error")] public AIDevKit.ErrorResponse Error { get; set; }

        /// <summary>
        /// The name of the fine-tuned model that is being created.
        /// The value will be null if the fine-tuning job is still running.
        /// </summary>
        [JsonProperty("fine_tuned_model")] public string FineTunedModel { get; set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the fine-tuning job was finished.
        /// The value will be null if the fine-tuning job is still running.
        /// </summary>
        [JsonProperty("finished_at")] public UnixTime? FinishedAt { get; set; }

        /// <summary>
        /// The hyperparameters used for the fine-tuning job.
        /// See the fine-tuning guide for more details.
        /// </summary>
        [JsonProperty("hyperparameters")] public HyperParameters Hyperparameters { get; set; }

        /// <summary>
        /// The organization that owns the fine-tuning job. </summary>
        [JsonProperty("organization_id")] public string OrganizationId { get; set; }

        /// <summary>
        /// The compiled results file ID(s) for the fine-tuning job.
        /// You can retrieve the results with the Files API.
        /// </summary>
        [JsonProperty("result_files")] public string[] ResultFiles { get; set; }

        /// <summary>
        /// The current status of the fine-tuning job,
        /// which can be either validating_files, queued, running, succeeded, failed, or cancelled. 
        /// </summary>
        [JsonProperty("status")] public FineTuningJobStatus? Status { get; set; }

        /// <summary>
        /// The total number of billable tokens processed by this fine-tuning job.
        /// The value will be null if the fine-tuning job is still running.
        /// </summary>
        [JsonProperty("trained_tokens")] public int? TrainedTokens { get; set; }

        /// <summary>
        /// The file ID used for training.
        /// You can retrieve the training data with the Files API.
        /// </summary>
        [JsonProperty("training_file")] public string TrainingFile { get; set; }

        /// <summary>
        /// The file ID used for validation.
        /// You can retrieve the validation results with the Files API.
        /// </summary>
        [JsonProperty("validation_file")] public string ValidationFile { get; set; }
    }

    public enum FineTuningJobStatus
    {
        [ApiEnum("validating_files")]
        ValidatingFiles,
        [ApiEnum("queued")]
        Queued,
        [ApiEnum("running")]
        Running,
        [ApiEnum("succeeded")]
        Succeeded,
        [ApiEnum("failed")]
        Failed,
        [ApiEnum("cancelled")]
        Cancelled,
    }
}