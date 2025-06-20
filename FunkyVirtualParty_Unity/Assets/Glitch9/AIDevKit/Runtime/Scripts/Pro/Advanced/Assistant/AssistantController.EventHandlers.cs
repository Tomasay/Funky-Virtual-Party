using System;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    public sealed partial class AssistantController
    {
        internal void OnAssistantCreated(Assistant newAssistant)
        {
            ThrowIf.ArgumentIsNull(newAssistant);
            Assistant = newAssistant;
            _router?.TriggerAssistantCreated(newAssistant);
        }

        internal void OnAssistantRetrieved(Assistant newAssistant)
        {
            ThrowIf.ArgumentIsNull(newAssistant);
            Assistant = newAssistant;
            _router?.TriggerAssistantRetrieved(newAssistant);
        }

        internal void OnAssistantUpdated(Assistant updatedAssistant)
        {
            ThrowIf.ArgumentIsNull(updatedAssistant);
            Assistant = updatedAssistant;
            _router?.TriggerAssistantUpdated(updatedAssistant);
        }

        internal void OnThreadCreated(Thread newThread)
        {
            ThrowIf.ArgumentIsNull(newThread);
            Thread = newThread;
            ThreadId = newThread.Id;
            _router?.TriggerThreadCreated(newThread);
            _threadIdList.Add(newThread.Id);
        }

        internal void OnThreadRetrieved(Thread newThread)
        {
            ThrowIf.ArgumentIsNull(newThread);
            Thread = newThread;
            ThreadId = newThread.Id;
            _router?.TriggerThreadRetrieved(newThread);
            if (!_threadIdList.Contains(newThread.Id)) _threadIdList.Add(newThread.Id);
        }

        internal void OnThreadUpdated(Thread updatedThread)
        {
            ThrowIf.ArgumentIsNull(updatedThread);
            Thread = updatedThread;
            ThreadId = updatedThread.Id;
            _router?.TriggerThreadUpdated(updatedThread);
            if (!_threadIdList.Contains(updatedThread.Id)) _threadIdList.Add(updatedThread.Id);
        }

        internal void OnRunCreated(Run newRun)
        {
            if (newRun == null) return;
            //ThrowIf.ArgumentIsNull(newRun);
            Run = newRun;
            _router?.TriggerRunCreated(newRun);

            RunStatus? newStatus = newRun?.Status;
            if (newStatus != null) HandleRunStatusChangeINTERNAL(newStatus.Value, newRun);
        }

        internal void OnRunRetrieved(Run newRun)
        {
            if (newRun == null) return;
            //ThrowIf.ArgumentIsNull(newRun);
            Run = newRun;
            _router?.TriggerRunRetrieved(newRun);

            RunStatus? newStatus = newRun?.Status;
            if (newStatus != null) HandleRunStatusChangeINTERNAL(newStatus.Value, newRun);
        }

        internal void OnRunUpdated(Run updatedRun)
        {
            ThrowIf.ArgumentIsNull(updatedRun);
            Run = updatedRun;
            _router?.TriggerRunUpdated(updatedRun);

            RunStatus? newStatus = updatedRun?.Status;
            if (newStatus != null) HandleRunStatusChangeINTERNAL(newStatus.Value, updatedRun);
        }

        internal void OnRunStepRetrieved(RunStep newRunStep)
        {
            ThrowIf.ArgumentIsNull(newRunStep);
            RunStep = newRunStep;
            _router?.TriggerRunStepRetrieved(newRunStep);

            RunStatus? newStatus = newRunStep?.Status;
            if (newStatus != null) HandleRunStatusChangeINTERNAL(newStatus.Value, newRunStep);
        }

        internal void OnMessageCreated(ThreadMessage newMessage)
        {
            ThrowIf.ArgumentIsNull(newMessage);

            if (newMessage.Role == ChatRole.User)
            {
                OnUserMessageCreated(newMessage);
            }
            else if (newMessage.Role == ChatRole.Assistant)
            {
                OnAssistantMessageCreated(newMessage);
            }
            else if (newMessage.Role == ChatRole.Tool)
            {
                OnToolMessageCreated(newMessage);
            }
            else
            {
                _logger.Warning("Unknown message role: " + newMessage.Role);
            }

            if (SaveThreadMessages)
            {
                ThrowIf.IsNullOrEmpty(ThreadId, "Thread Id");
                SavedMessages.Add(ThreadId, newMessage);
            }
        }

        internal void OnMessageRetrieved(ThreadMessage newMessage)
        {
            ThrowIf.ArgumentIsNull(newMessage);
            _router?.TriggerMessageRetrieved(newMessage);
        }

        internal void OnUserMessageCreated(ThreadMessage newMessage)
        {
            if (LastUserMessage == newMessage) throw new ArgumentException("This user message already exists.");
            LastUserMessage = newMessage;
            _router?.TriggerMessageCreated(newMessage);
        }

        internal void OnAssistantMessageCreated(ThreadMessage newMessage)
        {
            if (LastAssistantMessage == newMessage) throw new ArgumentException("This assistant message already exists.");
            LastAssistantMessage = newMessage;
            _router?.TriggerMessageCreated(newMessage);
        }

        internal void OnToolMessageCreated(ThreadMessage newMessage)
        {
            if (LastToolMessage == newMessage) throw new ArgumentException("This tool message already exists.");
            LastToolMessage = newMessage;
            _router?.TriggerMessageCreated(newMessage);
        }

        internal void OnTextCreated()
        {
            _router?.TriggerTextCreated();
        }

        internal void OnTextDelta(string textDelta)
        {
            _router?.TriggerTextDelta(textDelta);
        }

        internal void OnStreamDone()
        {
            _router?.TriggerStreamDone();
        }

        internal void OnToolCalls(ToolCall[] toolCalls)
        {
            _router?.TriggerToolCalls(toolCalls);
        }

        internal void OnRequiredAction(FunctionCall functionCall)
        {
            if (functionCall == null) throw new ArgumentNullException(nameof(functionCall));
            string id = functionCall.Id;
            if (!RequiredActions.Contains(id)) RequiredActions.Add(id);
            _router?.TriggerRequiredAction(functionCall);
        }

        internal void OnMessageCompleted(ThreadMessage completedMessage)
        {
            LastAssistantMessage = completedMessage;
            _newAssistantMessageCreated = true;
            _router?.TriggerMessageCompleted(completedMessage);
        }

        internal void OnReceiveError(ErrorResponse error)
        {
            string errorMessage = error?.ToString() ?? "Unknown error";
            OnReceiveError(errorMessage);
        }

        internal void OnReceiveError(string errorMessage)
        {
            _logger.Error(errorMessage);
            _router?.TriggerError(errorMessage);
        }

        internal void OnRunStatusChanged(RunStatus runStatus) => HandleRunStatusChangeINTERNAL(runStatus, null, null);
        private void HandleRunStatusChangeINTERNAL(RunStatus runStatus, RunStep newRunStep) => HandleRunStatusChangeINTERNAL(runStatus, null, newRunStep);
        private void HandleRunStatusChangeINTERNAL(RunStatus runStatus, Run newRun) => HandleRunStatusChangeINTERNAL(runStatus, newRun, null);
        private void HandleRunStatusChangeINTERNAL(RunStatus runStatus, Run newRun, RunStep newRunStep)
        {
            if (RunStatus == runStatus) return;
            //RunStatus = runStatus;

            switch (runStatus)
            {
                case RunStatus.Queued:
                case RunStatus.InProgress:
                case RunStatus.Cancelling:
                    _logger.RunStatusChange(RunStatus);
                    UpdateStatus(AssistantStatus.PollingRun);
                    break;

                case RunStatus.RequiresAction:
                    _logger.RunStatusChange(RunStatus);
                    OnToolCalls(newRun?.Tools);
                    UpdateStatus(AssistantStatus.RequiresAction);
                    break;

                case RunStatus.Completed:
                case RunStatus.Expired:
                case RunStatus.Cancelled:
                    _logger.RunStatusChange(RunStatus);
                    UpdateStatus(AssistantStatus.WaitingForInput);
                    break;

                case RunStatus.Failed:

                    ErrorResponse lastError = null;

                    if (newRun != null) lastError = newRun?.LastError;
                    else if (newRunStep != null) lastError = newRunStep?.LastError;

                    if (lastError != null)
                    {
                        _logger.RunFailed(lastError.Message);
                        OnReceiveError(lastError);
                    }

                    UpdateStatus(AssistantStatus.WaitingForInput);
                    break;

                case RunStatus.Incomplete:

                    if (newRun != null)
                    {
                        IncompleteDetails incompleteDetails = newRun?.IncompleteDetails;
                        if (incompleteDetails != null)
                        {
                            _logger.RunIncomplete(incompleteDetails.Reason);
                            OnReceiveError(incompleteDetails.Reason);
                        }
                    }

                    UpdateStatus(AssistantStatus.WaitingForInput);
                    break;

                default:
                    UpdateStatus(AssistantStatus.Initializing);
                    break;
            }

            _router?.TriggerRunStatusChanged(runStatus);
        }
    }
}