namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    /// <summary>
    /// Represents the stages of the Assistants API.
    /// </summary>
    public enum AssistantStatus
    {
        Initializing,
        WaitingForInput,
        PollingRun,
        RequiresAction,
        WaitingForSubmitToolOutputs,
        HandlingResponse,
    }

    internal static class AssistantStatusExtensions
    {
        internal static string GetStatusMessage(this AssistantStatus status)
        {
            return status switch
            {
                AssistantStatus.Initializing => $"Initializing the Assistants API.",
                AssistantStatus.WaitingForInput => "The Assistants API is ready.",
                AssistantStatus.PollingRun => "Waiting for the run to complete.",
                AssistantStatus.RequiresAction => "Waiting for all required actions to be handled.",
                AssistantStatus.WaitingForSubmitToolOutputs => "Waiting for the user to submit tool outputs.",
                AssistantStatus.HandlingResponse => "Retrieving the response.",
                _ => "Unknown stage."
            };
        }
    }
}