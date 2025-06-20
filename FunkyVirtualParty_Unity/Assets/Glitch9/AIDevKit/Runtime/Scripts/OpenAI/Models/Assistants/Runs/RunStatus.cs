using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Enum representing the various statuses a Run can have during its lifecycle.
    /// </summary>
    public enum RunStatus
    {
        /// <summary>
        /// The default value when the Run has not been created yet.
        /// </summary>
        Null,

        /// <summary>
        /// When Runs are first created or when you complete the required_action, they are moved to a queued status.
        /// They should almost immediately move to in_progress.
        /// </summary>
        [ApiEnum("queued"), Message("Run is queued")]
        Queued,

        /// <summary>
        /// While in_progress, the Assistant uses the model and tools to perform steps.
        /// You can view progress being made by the Run by examining the Run Steps.
        /// </summary>
        [ApiEnum("in_progress"), Message("Run is in progress")]
        InProgress,

        /// <summary>
        /// The Run successfully completed! You can now view all Messages the Assistant added to the Thread,
        /// and all the steps the Run took. You can also continue the conversation by adding more user Messages
        /// to the Thread and creating another Run.
        /// </summary>
        [ApiEnum("completed"), Message("Run completed")]
        Completed,

        /// <summary>
        /// When using the Function calling tool, the Run will move to a required_action state once the model determines 
        /// the names and arguments of the functions to be called. You must then run those functions and submit the outputs 
        /// before the run proceeds. If the outputs are not provided before the expires_at timestamp passes 
        /// (roughly 10 mins past creation), the run will move to an expired status.
        /// </summary>
        [ApiEnum("requires_action"), Message("Run requires action")]
        RequiresAction,

        /// <summary>
        /// This happens when the function calling outputs were not submitted before expires_at and the run expires.
        /// Additionally, if the runs take too long to execute and go beyond the time stated in expires_at, 
        /// our systems will expire the run.
        /// </summary>
        [ApiEnum("expired"), Message("Run expired")]
        Expired,

        /// <summary>
        /// You can attempt to cancel an in_progress run using the Cancel Run endpoint.
        /// Once the attempt to cancel succeeds, the status of the Run moves to cancelled.
        /// Cancellation is attempted but not guaranteed.
        /// </summary>
        [ApiEnum("cancelling"), Message("Run is cancelling")]
        Cancelling,

        /// <summary>
        /// Run was successfully cancelled.
        /// </summary>
        [ApiEnum("cancelled"), Message("Run cancelled")]
        Cancelled,

        /// <summary>
        /// You can view the reason for the failure by looking at the last_error object in the Run.
        /// The timestamp for the failure will be recorded under failed_at.
        /// </summary>
        [ApiEnum("failed"), Message("Run failed")]
        Failed,

        /// <summary>
        /// Run ended due to max_prompt_tokens or max_completion_tokens reached.
        /// You can view the specific reason by looking at the incomplete_details object in the Run.
        /// </summary>
        [ApiEnum("incomplete"), Message("Run incomplete")]
        Incomplete
    }
}
