using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// https://platform.openai.com/docs/api-reference/assistants-streaming/run-step-delta-object
    /// </summary>
    public class RunStepDelta : AIResponse
    {
        /// <summary>
        /// The delta containing the fields that have changed on the run step.
        /// </summary>
        [JsonProperty("delta")] public RunStepDeltaValue Delta { get; set; }
    }

    public class RunStepDeltaValue
    {
        [JsonProperty("step_details")] public StepDetails StepDetails { get; set; }
    }
}