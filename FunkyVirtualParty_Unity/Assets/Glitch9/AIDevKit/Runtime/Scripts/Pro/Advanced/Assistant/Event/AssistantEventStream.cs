namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    public class AssistantEventStream
    {
        /// <summary>
        /// Occurs when a new thread is created.
        /// <para>Data type is a <see cref="Thread"/>.</para>
        /// </summary>
        public const string ThreadCreated = "thread.created";

        /// <summary>
        /// Occurs when a new run is created.
        /// <para>Data type is a <see cref="Run"/>.</para>
        /// </summary>
        public const string ThreadRunCreated = "thread.run.created";

        /// <summary>
        /// Occurs when a run moves to a queued status.
        /// <para>Data type is a <see cref="Run"/>.</para>
        /// </summary>
        public const string ThreadRunQueued = "thread.run.queued";

        /// <summary>
        /// Occurs when a run moves to an in_progress status.
        /// <para>Data type is a <see cref="Run"/>.</para>
        /// </summary>
        public const string ThreadRunInProgress = "thread.run.in_progress";

        /// <summary>
        /// Occurs when a run moves to a requires_action status.
        /// <para>Data type is a <see cref="Run"/>.</para>
        /// </summary>
        public const string ThreadRunRequiresAction = "thread.run.requires_action";

        /// <summary>
        /// Occurs when a run is completed.
        /// <para>Data type is a <see cref="Run"/>.</para>
        /// </summary>
        public const string ThreadRunCompleted = "thread.run.completed";

        /// <summary>
        /// Occurs when a run ends with status incomplete.
        /// <para>Data type is a <see cref="Run"/>.</para>
        /// </summary>
        public const string ThreadRunIncomplete = "thread.run.incomplete";

        /// <summary>
        /// Occurs when a run fails.
        /// <para>Data type is a <see cref="Run"/>.</para>
        /// </summary>
        public const string ThreadRunFailed = "thread.run.failed";

        /// <summary>
        /// Occurs when a run moves to a cancelling status.
        /// <para>Data type is a <see cref="Run"/>.</para>
        /// </summary>
        public const string ThreadRunCancelling = "thread.run.cancelling";

        /// <summary>
        /// Occurs when a run is cancelled.
        /// <para>Data type is a <see cref="Run"/>.</para>
        /// </summary>
        public const string ThreadRunCancelled = "thread.run.cancelled";

        /// <summary>
        /// Occurs when a run expires.
        /// <para>Data type is a <see cref="Run"/>.</para>
        /// </summary>
        public const string ThreadRunExpired = "thread.run.expired";

        /// <summary>
        /// Occurs when a run step is created.
        /// <para>Data type is a <see cref="RunStep"/>.</para>
        /// </summary>
        public const string ThreadRunStepCreated = "thread.run.step.created";

        /// <summary>
        /// Occurs when a run step moves to an in_progress state.
        /// <para>Data type is a <see cref="RunStep"/>.</para>
        /// </summary>
        public const string ThreadRunStepInProgress = "thread.run.step.in_progress";

        /// <summary>
        /// Occurs when parts of a run step are being streamed.
        /// <para>Data type is a <see cref="RunStepDelta"/>.</para>
        /// </summary>
        public const string ThreadRunStepDelta = "thread.run.step.delta";

        /// <summary>
        /// Occurs when a run step is completed.
        /// <para>Data type is a <see cref="RunStep"/>.</para>
        /// </summary>
        public const string ThreadRunStepCompleted = "thread.run.step.completed";

        /// <summary>
        /// Occurs when a run step fails.
        /// <para>Data type is a <see cref="RunStep"/>.</para>
        /// </summary>
        public const string ThreadRunStepFailed = "thread.run.step.failed";

        /// <summary>
        /// Occurs when a run step is cancelled.
        /// <para>Data type is a <see cref="RunStep"/>.</para>
        /// </summary>
        public const string ThreadRunStepCancelled = "thread.run.step.cancelled";

        /// <summary>
        /// Occurs when a run step expires.
        /// <para>Data type is a <see cref="RunStep"/>.</para>
        /// </summary>
        public const string ThreadRunStepExpired = "thread.run.step.expired";

        /// <summary>
        /// Occurs when a message is created.
        /// <para>Data type is a <see cref="ChatMessage"/>.</para>
        /// </summary>
        public const string ThreadMessageCreated = "thread.message.created";

        /// <summary>
        /// Occurs when a message moves to an in_progress state.
        /// <para>Data type is a <see cref="ChatMessage"/>.</para>
        /// </summary>
        public const string ThreadMessageInProgress = "thread.message.in_progress";

        /// <summary>
        /// Occurs when parts of a Message are being streamed.
        /// <para>Data type is a <see cref="OpenAI.ThreadMessageDelta"/>.</para>
        /// </summary>
        public const string ThreadMessageDelta = "thread.message.delta";

        /// <summary>
        /// Occurs when a message is completed.
        /// <para>Data type is a <see cref="ChatMessage"/>.</para>
        /// </summary>
        public const string ThreadMessageCompleted = "thread.message.completed";

        /// <summary>
        /// Occurs when a message ends before it is completed.
        /// <para>Data type is a <see cref="ChatMessage"/>.</para>
        /// </summary>
        public const string ThreadMessageIncomplete = "thread.message.incomplete";

        /// <summary>
        /// Occurs when an error occurs. This can happen due to an internal server error or a timeout.
        /// <para>Data type is a <see cref="AIDevKit.ErrorResponse"/>.</para>
        /// </summary>
        public const string Error = "error";

        /// <summary>
        /// Occurs when a stream ends.
        /// <para>Data type is [DONE].
        /// </summary>
        public const string Done = "done";

    }
}