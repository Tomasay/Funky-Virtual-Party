using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    internal class AssistantLogger : RESTLogger
    {
        private const string TAG = "AssistantsAPI";
        private readonly AssistantController _controller;
        internal AssistantLogger(AssistantController controller) : base(TAG, AIDevKitSettings.LogLevel) => _controller = controller;
        private string GetStatusMessage() => $"(Assistant Status: {_controller.AssistantStatus})";
        public override void Info(string message) => Info(TAG, $"{message} {GetStatusMessage()}");
        public override void Warning(string message) => Warning(TAG, $"{message} {GetStatusMessage()}");
        public override void Error(string message) => Error(TAG, $"{message} {GetStatusMessage()}");
        public void RunStatusChange(RunStatus runStatus) => Info(TAG, $"Run is now <color=cyan>{runStatus}</color>");
        public void RunFailed(string failReason) => Error(TAG, $"Run failed with reason: {failReason}");
        public void RunIncomplete(string incompleteReason) => Error(TAG, $"Run incomplete with reason: {incompleteReason}");
    }
}