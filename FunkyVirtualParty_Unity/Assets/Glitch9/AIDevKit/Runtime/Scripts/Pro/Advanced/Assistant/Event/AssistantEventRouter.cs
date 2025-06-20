
namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    internal class AssistantEventRouter
    {
        internal IAssistantEventReceiver assistantEventReceiver;
        internal IRunEventReceiver runEventReceiver;
        internal IThreadEventReceiver threadEventReceiver;
        internal IThreadMessageEventReceiver messageEventReceiver;
        internal IStreamingTextEventReceiver textEventReceiver;
        internal IToolCallReceiver toolCallReceiver;
        internal IRequiredActionListener requiredActionListener;

        internal AssistantEventRouter(AssistantSettings settings)
        {
            assistantEventReceiver = settings.AssistantEventReceiver;
            runEventReceiver = settings.RunEventReceiver;
            threadEventReceiver = settings.ThreadEventReceiver;
            messageEventReceiver = settings.MessageEventReceiver;
            textEventReceiver = settings.TextEventReceiver;
            toolCallReceiver = settings.ToolCallReceiver;
            requiredActionListener = settings.RequiredActionListener;
        }

        internal void TriggerAssistantCreated(Assistant assistant) => assistantEventReceiver?.OnAssistantCreated(assistant);
        internal void TriggerAssistantRetrieved(Assistant assistant) => assistantEventReceiver?.OnAssistantRetrieved(assistant);
        internal void TriggerAssistantUpdated(Assistant assistant) => assistantEventReceiver?.OnAssistantUpdated(assistant);
        internal void TriggerThreadCreated(Thread thread) => threadEventReceiver?.OnThreadCreated(thread);
        internal void TriggerThreadRetrieved(Thread thread) => threadEventReceiver?.OnThreadRetrieved(thread);
        internal void TriggerThreadUpdated(Thread thread) => threadEventReceiver?.OnThreadUpdated(thread);
        internal void TriggerRunCreated(Run run) => runEventReceiver?.OnRunCreated(run);
        internal void TriggerRunRetrieved(Run run) => runEventReceiver?.OnRunRetrieved(run);
        internal void TriggerRunUpdated(Run run) => runEventReceiver?.OnRunUpdated(run);
        internal void TriggerRunStepRetrieved(RunStep runStep) => runEventReceiver?.OnRunStepRetrieved(runStep);
        internal void TriggerRunStatusChanged(RunStatus runStatus) => runEventReceiver?.OnRunStatusChanged(runStatus);
        internal void TriggerTextCreated() => textEventReceiver?.OnReceiveTextStart();
        internal void TriggerTextDelta(string delta) => textEventReceiver?.OnReceiveText(delta);
        internal void TriggerStreamDone() => textEventReceiver?.OnReceiveTextDone();
        internal void TriggerToolCalls(ToolCall[] toolCalls) => toolCallReceiver?.OnReceiveToolCalls(toolCalls);
        internal void TriggerRequiredAction(FunctionCall functionCall) => requiredActionListener?.OnRequiredAction(functionCall);
        internal void TriggerMessageCreated(ThreadMessage message) => messageEventReceiver?.OnMessageCreated(message);
        internal void TriggerMessageRetrieved(ThreadMessage message) => messageEventReceiver?.OnMessageRetrieved(message);
        internal void TriggerMessageCompleted(ThreadMessage message) => messageEventReceiver?.OnMessageCompleted(message);
        internal void TriggerError(string errorMessage) => textEventReceiver?.OnReceiveError(errorMessage);
    }
}