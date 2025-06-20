using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    internal class AssistantEventDispatcher
    {
        private readonly AssistantController _controller;
        private readonly AssistantLogger _logger;
        private readonly SSEParser _sseParser;
        private readonly JsonSerializerSettings _jsonSettings;

        internal AssistantEventDispatcher(AssistantController controller, AssistantLogger logger)
        {
            _controller = controller;
            _logger = logger;

            if (controller.Client.SSEParser is SSEParser sseParser)
            {
                _sseParser = sseParser;
            }
            else
            {
                _sseParser = new SSEParser();
            }

            _jsonSettings = controller.Client.JsonSettings;
        }

        internal void OnReceiveStreamEvent(string sseText)
        {
            List<(SSEField field, string result)> sseFields = _sseParser.Parse(sseText);
            if (sseFields.Count == 0 || sseFields.Count < 2) return;

            List<(string eventAsString, string dataAsString)> eventAndData = AssistantUtil.PairEvents(sseFields, _logger);

            foreach ((string eventAsString, string dataAsString) in eventAndData)
            {
                HandleStreamEvent(eventAsString, dataAsString);
            }
        }

        private T Deserialize<T>(string data) where T : AIResponse
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(data, _jsonSettings);
            }
            catch (JsonException ex)
            {
                _logger.Error($"Failed to deserialize streamed event: {data}");
                _logger.Error(ex.Message);
                return null;
            }
        }

        private void HandleStreamEvent(string streamEvent, string data)
        {
            if (string.IsNullOrEmpty(streamEvent) || data == null) return;

            _logger.Stream(streamEvent, data);

            switch (streamEvent)
            {
                case AssistantEventStream.ThreadCreated:
                    _controller.OnThreadRetrieved(Deserialize<Thread>(data));
                    break;

                case AssistantEventStream.ThreadRunCreated:
                case AssistantEventStream.ThreadRunQueued:
                case AssistantEventStream.ThreadRunInProgress:
                case AssistantEventStream.ThreadRunRequiresAction:
                case AssistantEventStream.ThreadRunCompleted:
                case AssistantEventStream.ThreadRunCancelling:
                case AssistantEventStream.ThreadRunCancelled:
                case AssistantEventStream.ThreadRunFailed:
                case AssistantEventStream.ThreadRunIncomplete:
                case AssistantEventStream.ThreadRunExpired:
                    _controller.OnRunRetrieved(Deserialize<Run>(data));
                    break;

                case AssistantEventStream.ThreadRunStepCreated:
                case AssistantEventStream.ThreadRunStepInProgress:
                case AssistantEventStream.ThreadRunStepCompleted:
                case AssistantEventStream.ThreadRunStepCancelled:
                case AssistantEventStream.ThreadRunStepFailed:
                case AssistantEventStream.ThreadRunStepExpired:
                    _controller.OnRunStepRetrieved(Deserialize<RunStep>(data));
                    break;

                case AssistantEventStream.ThreadRunStepDelta:
                    RunStepDelta runStepDelta = JsonConvert.DeserializeObject<RunStepDelta>(data, _jsonSettings);
                    ToolCall[] toolCalls = runStepDelta?.Delta?.StepDetails?.ToolCalls;
                    _controller.OnToolCalls(toolCalls);
                    break;

                case AssistantEventStream.ThreadMessageDelta:
                    //AIDevKitDebug.Mark("ThreadMessageDelta");
                    ThreadMessageDelta messageDelta = JsonConvert.DeserializeObject<ThreadMessageDelta>(data, _jsonSettings);
                    string textDelta = messageDelta?.Delta?.Content;
                    if (textDelta != null) _controller.OnTextDelta(textDelta);
                    break;

                case AssistantEventStream.ThreadMessageCreated:
                    _controller.OnTextCreated();
                    ThreadMessage message = JsonConvert.DeserializeObject<ThreadMessage>(data, _jsonSettings);
                    _controller.OnMessageCreated(message);
                    break;

                case AssistantEventStream.ThreadMessageInProgress:
                    // Do nothing
                    break;

                case AssistantEventStream.ThreadMessageCompleted:
                    message = JsonConvert.DeserializeObject<ThreadMessage>(data, _jsonSettings);
                    _controller.OnMessageCompleted(message);
                    break;

                case AssistantEventStream.ThreadMessageIncomplete:
                    _controller.OnReceiveError("Message was incomplete.");
                    break;

                case AssistantEventStream.Error:
                    ErrorResponse errorResopnse = JsonConvert.DeserializeObject<ErrorResponse>(data, _jsonSettings);
                    _controller.OnReceiveError(errorResopnse);
                    break;

                case AssistantEventStream.Done:
                    _controller.OnStreamDone();
                    break;

                default:
                    _controller.OnReceiveError($"Unknown event:{streamEvent}\r\nData:{data}");
                    break;
            }
        }
    }
}