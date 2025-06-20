using Cysharp.Threading.Tasks;
using System;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    public enum RunStatusCheckType
    {
        Response,
        TerminalStatus,
    }

    internal class RunPoller
    {
        private readonly int _runPollingDelay;
        private readonly int _runPollingInterval;
        private readonly int _runPollingTimeout;
        private readonly AssistantController _controller;
        private readonly AssistantLogger _logger;

        internal RunPoller(AssistantController controller, AssistantLogger logger, AssistantSettings settings)
        {
            _controller = controller;
            _logger = logger;
            _runPollingDelay = settings.RunPollingDelay;
            _runPollingInterval = settings.RunPollingInterval;
            _runPollingTimeout = settings.RunPollingTimeout;
        }

        internal async UniTask CreateNonStreamingResponseAsync()
        {
            if (_controller.AssistantStatus == AssistantStatus.PollingRun)
            {
                if (_controller.Run == null)
                {
                    _controller.CancelRunAsync().Forget();
                    throw new InvalidOperationException("Run object is null.");
                }

                if (_controller.Run.Status == RunStatus.Completed)
                {
                    await RetrieveLastAssistantMessage();
                    return;
                }

                if (_controller.Run.Status == RunStatus.RequiresAction)
                {
                    await _controller.HandleRequiredActionsAsync();
                    return;
                }

                if (_controller.Run.Status == RunStatus.Incomplete)
                {
                    throw new InvalidOperationException($"Run was not able to complete successfully: {_controller.Run.IncompleteDetails?.Reason ?? "No reason provided."}");
                }
            }

            _controller.UpdateStatus(AssistantStatus.PollingRun);
            Run run = await PollUntilConditionsAreMetAsync(_controller.Thread, _controller.Run, RunStatusCheckType.Response);

            if (run == null)
            {
                _controller.CancelRunAsync().Forget();
                throw new InvalidOperationException("Run object is null.");
            }

            if (run.Status == RunStatus.Incomplete)
            {
                throw new InvalidOperationException($"Run was not able to complete successfully: {_controller.Run.IncompleteDetails?.Reason ?? "No reason provided."}");
            }

            if (run.Status == RunStatus.Failed)
            {
                ErrorResponse error = run.LastError;
                string errorMessage = error?.ToString() ?? "No error provided.";
                _controller.CancelRunAsync().Forget();
                throw new InvalidOperationException($"Run operation failed: {errorMessage}");
            }

            if (run.Status != RunStatus.Completed && run.Status != RunStatus.RequiresAction)
            {
                _controller.CancelRunAsync().Forget();
                throw new InvalidOperationException($"Run operation failed with status: {run.Status}.");
            }

            _controller.Run = run;

            if (_controller.Run.Status == RunStatus.Completed)
            {
                await RetrieveLastAssistantMessage();
                return;
            }

            if (_controller.Run.Status == RunStatus.RequiresAction)
            {
                await _controller.HandleRequiredActionsAsync();
                return;
            }

            throw new InvalidOperationException($"Run operation failed to complete with status: {_controller.Run.Status}.");
        }

        internal async UniTask<Run> PollUntilConditionsAreMetAsync(Thread thread, Run run, RunStatusCheckType checkType)
        {
            if (thread == null || run == null) return null;

            int initialDelayMillis = _runPollingDelay * 1000; // How long to wait before checking the run status for the first time?
            int delayMillis = _runPollingInterval * 1000; // How long to wait between subsequent checks?
            int timeoutSec = _runPollingTimeout; // How long to wait before giving up?
            string runId = run.Id;

            TimeSpan maxWaitTime = TimeSpan.FromSeconds(timeoutSec);
            DateTime runTimeout = DateTime.Now.Add(maxWaitTime);

            await UniTask.Delay(initialDelayMillis);

            while (DateTime.Now < runTimeout)
            {
                run = await _controller.Client.Beta.Threads.Runs.Retrieve(thread.Id, runId);

                if (RunAndRunStatusNotNull(run))
                {
                    _controller.OnRunStatusChanged(run.Status!.Value);

                    if (checkType == RunStatusCheckType.Response)
                    {
                        if (run.Status.IsStatusType(RunStatusType.Success))
                        {
                            return run;
                        }

                        if (run.Status == RunStatus.Incomplete)
                        {
                            HandleIncompleteRun(run);
                            return run;
                        }

                        if (run.Status.IsStatusType(RunStatusType.Failure))
                        {
                            _logger.Error($"Operation failed with status: {run.Status}");
                            return run;
                        }
                    }
                    else if (checkType == RunStatusCheckType.TerminalStatus)
                    {
                        if (run.Status.IsStatusType(RunStatusType.Terminal))
                        {
                            return run;
                        }
                    }
                }

                await UniTask.Delay(delayMillis); // Wait for the next check
                delayMillis = Math.Min(delayMillis * 2, 30000); // Increase delay for the next iteration (Max 30 seconds)
            }

            _logger.Error($"Operation timed out after {timeoutSec} seconds.");
            return run;
        }

        private bool RunAndRunStatusNotNull(Run run)
        {
            if (run == null)
            {
                _logger.Info("Run object is null.");
                return false;
            }

            if (run.Status != null) return true;

            _logger.Info("Run status is null.");
            return false;
        }

        private void HandleIncompleteRun(Run currentRun)
        {
            string incompleteReason = currentRun.IncompleteDetails?.Reason;
            if (incompleteReason == null)
            {
                _logger.Error("Run operation is incomplete but the reason is not provided.");
                return;
            }
            _logger.Error($"Operation is incomplete: {incompleteReason}");
        }

        //private async UniTask RetrieveLastAssistantMessage()
        internal async UniTask RetrieveLastAssistantMessage()
        {
            _controller.UpdateStatus(AssistantStatus.HandlingResponse);
            QueryResponse<ThreadMessage> queryResponse = await _controller.Client.Beta.Threads.Messages.ListAsync(_controller.Thread.Id, new(1));
            ThreadMessage[] messages = queryResponse?.Data;
            ThrowIf.ListIsNullOrEmpty(messages);
            ThreadMessage lastMessage = messages[0];
            ThrowIf.ArgumentIsNull(lastMessage);
            if (lastMessage.Role != ChatRole.Assistant) throw new Exception("The last message is not from ChatRole.Assistant.");
            _controller.OnAssistantMessageCreated(lastMessage);
        }
    }
}