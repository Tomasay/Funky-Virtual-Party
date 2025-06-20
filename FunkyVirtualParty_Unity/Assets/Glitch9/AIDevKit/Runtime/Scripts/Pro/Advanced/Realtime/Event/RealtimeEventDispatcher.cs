using System;
using System.Collections.Generic;
using Glitch9.AIDevKit.Components;
using Glitch9.AIDevKit.OpenAI.Assistants;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    internal class RealtimeEventDispatcher
    {
        private readonly RealtimeSessionController _controller;
        private readonly FunctionManager _functionManager;
        private readonly int _unityAudioSampleRate;
        private readonly int _unityAudioChannels;

        private readonly RealtimeItemBuffer<string> _audioBuffer;
        private readonly RealtimeItemBuffer<string> _textBuffer;
        private readonly RealtimeItemBuffer<string> _transcriptBuffer;
        private readonly RealtimeItemBuffer<string> _inputTranscriptionBuffer;

        private readonly IStreamingAudioEventReceiver _audioEventReceiver;
        private readonly IRealtimeEventReceiver _realtimeEventReceiver;
        private readonly IStreamingTextEventReceiver _inputTranscriptionEventReceiver;
        private readonly IStreamingTextEventReceiver _textEventReceiver;
        private readonly IStreamingTextEventReceiver _transcriptEventReceiver;
        private readonly IToolCallReceiver _toolCallReceiver;

        private readonly RESTLogger _logger;
        private readonly bool _audioEnabled;
        private readonly bool _textEnabled;
        private readonly bool _transcriptEnabled;
        private readonly bool _inputTranscriptionEnabled;
        private readonly bool _toolsEnabled;
        private string _lastEventType;

        internal RealtimeEventDispatcher(
            RealtimeSessionController controller,
            FunctionManager functionManager,
            IStreamingAudioEventReceiver audioEventReceiver,
            IRealtimeEventReceiver realtimeEventReceiver = null,
            IStreamingTextEventReceiver inputTranscriptionEventReceiver = null,
            IStreamingTextEventReceiver textEventReceiver = null,
            IStreamingTextEventReceiver transcriptEventReceiver = null,
            IToolCallReceiver toolCallReceiver = null,
            RESTLogger logger = null)
        {
            _audioEventReceiver = audioEventReceiver ?? throw new ArgumentNullException(nameof(audioEventReceiver));
            _realtimeEventReceiver = realtimeEventReceiver;
            _inputTranscriptionEventReceiver = inputTranscriptionEventReceiver;
            _textEventReceiver = textEventReceiver;
            _transcriptEventReceiver = transcriptEventReceiver;
            _toolCallReceiver = toolCallReceiver;

            AudioConfiguration config = AudioSettings.GetConfiguration();
            _unityAudioSampleRate = config.sampleRate;
            _unityAudioChannels = RealtimeUtil.GetChannelCount(config.speakerMode);

            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _functionManager = functionManager;
            _logger = logger ?? new RESTLogger("RealtimeEventReceiver", RESTLogLevel.All);

            _audioEnabled = _audioEventReceiver != null;
            _textEnabled = _textEventReceiver != null;
            _transcriptEnabled = _transcriptEventReceiver != null;
            _inputTranscriptionEnabled = _inputTranscriptionEventReceiver != null;
            _toolsEnabled = (_functionManager != null && !_functionManager.IsEmpty) || _toolCallReceiver != null;

            if (_audioEnabled) _audioBuffer = new RealtimeItemBuffer<string>("Audio", OnReceiveAudio, OnReceiveAudioDone);
            if (_textEnabled) _textBuffer = new RealtimeItemBuffer<string>("Text", OnReceiveText, OnReceiveTextDone);
            if (_transcriptEnabled) _transcriptBuffer = new RealtimeItemBuffer<string>("Transcript", OnReceiveTranscript, OnReceiveTranscriptDone);
            if (_inputTranscriptionEnabled) _inputTranscriptionBuffer = new RealtimeItemBuffer<string>("InputTranscription", OnReceiveInputTranscription, OnReceiveInputTranscriptionCompleted);
        }

        internal void OnRealtimeEventReceived(RealtimeEvent @event)
        {
            if (@event == null)
            {
                _logger.Error("Received null event.");
                return;
            }

            if (@event.Type == null)
            {
                _logger.Error("Received event with null type.");
                return;
            }

            if (string.IsNullOrEmpty(@event.EventId))
            {
                _logger.Error("Received event with null ID.");
                return;
            }

            _logger.ResVerbose(@event.EventId, $"Received event: {@event.Type}");

            if (_realtimeEventReceiver != null && _lastEventType != @event.Type)
            {
                _logger.ResVerbose(@event.EventId, $"Event Type changed: {_lastEventType} -> {@event.Type}");
                _realtimeEventReceiver?.OnEventTypeChanged(@event.Type);
            }

            _lastEventType = @event.Type;

            // Handle the event here.
            switch (@event.Type)
            {
                case RealtimeEvent.Response.Error:
                    OnReceiveError(@event.EventId, @event.Error);
                    break;
                case RealtimeEvent.Response.SessionCreated:
                    OnSessionCreated(@event.EventId, @event.Session);
                    break;
                case RealtimeEvent.Response.SessionUpdated:
                    OnSessionUpdated(@event.EventId, @event.Session);
                    break;
                case RealtimeEvent.Response.InputAudioBufferCommitted:
                    OnInputAudioBufferCommitted(@event.EventId, @event.PreviousItemId, @event.ItemId);
                    break;
                case RealtimeEvent.Response.InputAudioBufferCleared:
                    OnInputAudioBufferCleared(@event.EventId);
                    break;
                case RealtimeEvent.Response.InputAudioBufferSpeechStarted:
                    OnInputAudioBufferSpeechStarted(@event.EventId, @event.AudioStartMs, @event.ItemId);
                    break;
                case RealtimeEvent.Response.InputAudioBufferSpeechStopped:
                    OnInputAudioBufferSpeechStopped(@event.EventId, @event.AudioEndMs, @event.ItemId);
                    break;
                case RealtimeEvent.Response.ConversationCreated:
                    OnConversationCreated(@event.EventId, @event.Conversation);
                    break;
                case RealtimeEvent.Response.ConversationItemCreated:
                    OnConversationItemCreated(@event.EventId, @event.PreviousItemId, @event.Item);
                    break;
                case RealtimeEvent.Response.ConversationItemInputAudioTranscriptionDelta:
                    OnConversationItemInputAudioTranscriptionDelta(@event.EventId, @event.ItemId, @event.OutputIndex, @event.Delta);
                    break;
                case RealtimeEvent.Response.ConversationItemInputAudioTranscriptionCompleted:
                    OnConversationItemInputAudioTranscriptionCompleted(@event.EventId, @event.ContentIndex, @event.Transcript);
                    break;
                case RealtimeEvent.Response.ConversationItemInputAudioTranscriptionFailed:
                    OnConversationItemInputAudioTranscriptionFailed(@event.EventId, @event.ContentIndex, @event.Error);
                    break;
                case RealtimeEvent.Response.ConversationItemTruncated:
                    OnConversationItemTruncated(@event.EventId, @event.ItemId, @event.ContentIndex, @event.AudioEndMs);
                    break;
                case RealtimeEvent.Response.ConversationItemDeleted:
                    OnConversationItemDeleted(@event.EventId, @event.ItemId);
                    break;
                case RealtimeEvent.Response.ResponseCreated:
                    OnResponseCreated(@event.EventId, @event.ResponseItem);
                    break;
                case RealtimeEvent.Response.ResponseDone:
                    OnResponseDone(@event.EventId, @event.ResponseItem);
                    break;
                case RealtimeEvent.Response.ResponseOutputItemAdded:
                    OnResponseOutputItemAdded(@event.EventId, @event.ResponseId, @event.OutputIndex, @event.Item);
                    break;
                case RealtimeEvent.Response.ResponseOutputItemDone:
                    OnResponseOutputItemDone(@event.EventId, @event.ResponseId, @event.OutputIndex, @event.Item);
                    break;
                case RealtimeEvent.Response.ResponseContentPartAdded:
                    OnResponseContentPartAdded(@event.EventId, @event.ResponseId, @event.ItemId, @event.OutputIndex, @event.ContentIndex, @event.Part);
                    break;
                case RealtimeEvent.Response.ResponseContentPartDone:
                    OnResponseContentPartDone(@event.EventId, @event.ResponseId, @event.ItemId, @event.OutputIndex, @event.ContentIndex, @event.Part);
                    break;
                case RealtimeEvent.Response.ResponseTextDelta:
                    OnResponseTextDelta(@event.EventId, @event.ResponseId, @event.ItemId, @event.OutputIndex, @event.ContentIndex, @event.Delta);
                    break;
                case RealtimeEvent.Response.ResponseTextDone:
                    OnResponseTextDone(@event.EventId, @event.ResponseId, @event.ItemId, @event.OutputIndex, @event.ContentIndex, @event.Text);
                    break;
                case RealtimeEvent.Response.ResponseAudioTranscriptDelta:
                    OnResponseAudioTranscriptDelta(@event.EventId, @event.ResponseId, @event.ItemId, @event.OutputIndex, @event.ContentIndex, @event.Delta);
                    break;
                case RealtimeEvent.Response.ResponseAudioTranscriptDone:
                    OnResponseAudioTranscriptDone(@event.EventId, @event.ResponseId, @event.ItemId, @event.OutputIndex, @event.ContentIndex, @event.Transcript);
                    break;
                case RealtimeEvent.Response.ResponseAudioDelta:
                    OnResponseAudioDelta(@event.EventId, @event.ResponseId, @event.ItemId, @event.OutputIndex, @event.ContentIndex, @event.Delta);
                    break;
                case RealtimeEvent.Response.ResponseAudioDone:
                    OnResponseAudioDone(@event.EventId, @event.ResponseId, @event.ItemId, @event.OutputIndex, @event.ContentIndex);
                    break;
                case RealtimeEvent.Response.ResponseFunctionCallArgumentsDelta:
                    OnResponseFunctionCallArgumentsDelta(@event.EventId, @event.ResponseId, @event.ItemId, @event.OutputIndex, @event.CallId, @event.Delta);
                    break;
                case RealtimeEvent.Response.ResponseFunctionCallArgumentsDone:
                    OnResponseFunctionCallArgumentsDone(@event.EventId, @event.ResponseId, @event.ItemId, @event.OutputIndex, @event.CallId, @event.Arguments);
                    break;
                case RealtimeEvent.Response.RateLimitsUpdated:
                    OnRateLimitsUpdated(@event.EventId, @event.RateLimits);
                    break;
                default:
                    _logger.Warning($"Unknown event type: {@event.Type}");
                    break;
            }
        }

        /// <summary>
        /// Returned when an error occurs.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="error">Details of the error.</param>
        internal void OnReceiveError(string eventId, ErrorResponse error)
        {
            string errorMessage = error?.GetMessage() ?? "Unknown error";
            errorMessage = $"{eventId}: {errorMessage}";
            _logger.Error(errorMessage);
            _textEventReceiver?.OnReceiveError(errorMessage);
            _transcriptEventReceiver?.OnReceiveError(errorMessage);
        }

        /// <summary>
        /// Returned when a session is created. Emitted automatically when a new connection is established.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="session">The session resource.</param>
        internal void OnSessionCreated(string eventId, RealtimeSession session)
        {
            _logger.Info(eventId, "Session created.");
        }

        /// <summary>
        /// Returned when a session is updated.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="session">The updated session resource.</param>
        internal void OnSessionUpdated(string eventId, RealtimeSession session)
        {
            _logger.Info(eventId, "Session updated.");
        }

        /// <summary>
        /// Returned when a conversation is created. Emitted right after session creation.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="conversation">The conversation resource.</param>
        internal void OnConversationCreated(string eventId, Conversation conversation)
        {
            _logger.Info(eventId, "Conversation created.");
        }

        /// <summary>
        /// Returned when an input audio buffer is committed, either by the client or automatically in server VAD mode.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="previousItemId">The ID of the preceding item.</param>
        /// <param name="itemId">The ID of the new item.</param>
        internal void OnInputAudioBufferCommitted(string eventId, string previousItemId, string itemId)
        {
            using (StringBuilderPool.Get(out var sb))
            {
                sb.Append("Input audio buffer committed. ");
                if (!string.IsNullOrEmpty(previousItemId))
                {
                    sb.Append(previousItemId);
                    sb.Append(" -> ");
                }
                sb.Append(itemId);
                _logger.ResVerbose(eventId, sb.ToString());
            }
        }

        /// <summary>
        /// Returned when the input audio buffer is cleared by the client.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        internal void OnInputAudioBufferCleared(string eventId)
        {
            _logger.ResVerbose(eventId, "Input audio buffer cleared.");
        }

        /// <summary>
        /// Returned in server turn detection mode when speech is detected.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="audioStartMs">Milliseconds since the session started when speech was detected.</param>
        /// <param name="itemId">The ID of the new item.</param>
        internal void OnInputAudioBufferSpeechStarted(string eventId, int? audioStartMs, string itemId)
        {
            _logger.ResVerbose(eventId, $"Speech started at {audioStartMs}ms.({itemId})");
        }

        /// <summary>
        /// Returned in server turn detection mode when speech stops.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="audioEndMs">Milliseconds since the session started when speech stopped.</param>
        /// <param name="itemId">The ID of the new item.</param>
        internal void OnInputAudioBufferSpeechStopped(string eventId, int? audioEndMs, string itemId)
        {
            _logger.ResVerbose(eventId, $"Speech stopped at {audioEndMs}ms.({itemId})");
        }

        /// <summary>
        /// Returned when a conversation item is created.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="previousItemId">The ID of the preceding item.</param>
        /// <param name="item">The newly created conversation item.</param>
        internal void OnConversationItemCreated(string eventId, string previousItemId, RealtimeItem item)
        {
            _logger.Info(eventId, $"Conversation item created. {previousItemId} -> {item.Id}");
        }

        internal void OnConversationItemInputAudioTranscriptionDelta(string eventId, string itemId, int? contentIndex, string delta)
        {
            if (!_inputTranscriptionEnabled) return;
            _logger.Stream(eventId, $"Audio transcription delta: <color=white>{delta}</color>\nContent Index: {contentIndex}");
            _inputTranscriptionBuffer.AppendDelta(itemId, null, contentIndex, delta);
        }

        /// <summary>
        /// Returned when input audio transcription is enabled and a transcription succeeds.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="contentIndex">The index of the content part containing the audio.</param>
        /// <param name="transcription">The transcribed text.</param>
        internal void OnConversationItemInputAudioTranscriptionCompleted(string eventId, int? contentIndex, string transcription)
        {
            if (!_inputTranscriptionEnabled) return;
            _logger.Info(eventId, $"Audio transcription completed. {contentIndex} -> {transcription}");
            _inputTranscriptionBuffer.CompleteDelta(null, null, contentIndex, transcription);
        }

        /// <summary>
        /// Returned when input audio transcription is configured, and a transcription request for a user message failed.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="contentIndex">The index of the content part containing the audio.</param>
        /// <param name="error">Details of the transcription error.</param>
        internal void OnConversationItemInputAudioTranscriptionFailed(string eventId, int? contentIndex, ErrorResponse error)
        {
            if (!_inputTranscriptionEnabled) return;
            string errorMessage = error?.GetMessage() ?? "Unknown error";
            errorMessage = $"{eventId}: {errorMessage}";
            _logger.Error(eventId, $"Audio transcription failed. [{contentIndex}]");
            _transcriptEventReceiver.OnReceiveError(errorMessage);
        }

        /// <summary>
        /// Returned when an earlier assistant audio message item is truncated by the client.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="itemId">The ID of the assistant message item that was truncated.</param>
        /// <param name="contentIndex">The index of the content part that was truncated.</param>
        /// <param name="audioEndMs">The duration up to which the audio was truncated, in milliseconds.</param>
        internal void OnConversationItemTruncated(string eventId, string itemId, int? contentIndex, int? audioEndMs)
        {
            _logger.Info(eventId, $"Conversation item truncated at {audioEndMs}ms. ({itemId}[{contentIndex}])");
        }

        /// <summary>
        /// Returned when an item in the conversation is deleted.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="itemId">The ID of the item that was deleted.</param>
        internal void OnConversationItemDeleted(string eventId, string itemId)
        {
            _logger.Info(eventId, $"Conversation item deleted. ({itemId})");
        }

        /// <summary>
        /// Returned when a new Response is created.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="response">The response resource.</param>
        internal void OnResponseCreated(string eventId, RealtimeItem response)
        {
            _logger.Info(eventId, "Response created.");
        }

        /// <summary>
        /// Returned when a Response is done streaming. 
        /// Always emitted, no matter the final state.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="response">The response resource.</param>
        internal void OnResponseDone(string eventId, RealtimeItem item)
        {
            _logger.Info(eventId, "Response done.");

            if (!_toolsEnabled || item.Output.IsNullOrEmpty()) return;

            // AIDevKitDebug.Log($"Detected function call: {item.Output.Length}");
            List<ToolCall> functionCalls = new();
            bool hasToolEventReceiver = _toolCallReceiver != null;

            for (int i = 0; i < item.Output.Length; i++)
            {
                RealtimeItem output = item.Output[i];
                if (output == null || output.Type != RealtimeItemType.FunctionCall) continue;

                string callId = output.CallId;
                string functionName = output.Name;

                //AIDevKitDebug.Log($"Function call: {functionName}");
                if (string.IsNullOrEmpty(functionName)) continue;

                string arg = output.Arguments;
                _functionManager.ExecuteFunction(functionName, arg);

                if (hasToolEventReceiver)
                {
                    FunctionCall functionCall = FunctionCall.Response(callId, functionName, arg);
                    functionCalls.Add(functionCall);
                }
            }

            if (hasToolEventReceiver && functionCalls.Count > 0)
            {
                _toolCallReceiver.OnReceiveToolCalls(functionCalls.ToArray());
            }

            if (item.Usage != null)
            {
                _logger.Info($"Item usage: {item.Usage}");
            }
        }

        /// <summary>
        /// Returned when a new Item is created during response generation.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response to which the item belongs.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="item">The created item.</param>
        internal void OnResponseOutputItemAdded(string eventId, string responseId, int? itemIndex, RealtimeItem item)
        {
            if (!CheckItemStatus(eventId, item)) return;
            _logger.ResVerbose(eventId, $"Response output item added.\nResponse ID: {responseId}\nItem Index: {itemIndex}\nItem ID: {item.Id}");

            if (!item.Content.IsNullOrEmpty())
            {
                for (int i = 0; i < item.Content.Length; i++)
                {
                    switch (item.Content[i].Type)
                    {
                        case RealtimeItemContentType.Text:
                            _textBuffer.AppendDeltaWithTempIndex(item.Id, itemIndex, item.Content[i].Text);
                            break;
                        case RealtimeItemContentType.Audio:
                            if (_audioEnabled) _audioBuffer.AppendDeltaWithTempIndex(item.Id, itemIndex, item.Content[i].Audio);
                            if (_transcriptEnabled) _transcriptBuffer.AppendDeltaWithTempIndex(item.Id, itemIndex, item.Content[i].Transcript);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Returned when an Item is done streaming.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response to which the item belongs.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="item">The completed item.</param>
        internal void OnResponseOutputItemDone(string eventId, string responseId, int? itemIndex, RealtimeItem item)
        {
            if (!CheckItemStatus(eventId, item)) return;
            _logger.ResVerbose(eventId, $"Response output item done.\nResponse ID: {responseId}\nItem Index: {itemIndex}\nItem ID: {item.Id}");
        }

        /// <summary>
        /// Returned when a new content part is added to an assistant message item during response generation.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response.</param>
        /// <param name="itemId">The ID of the item to which the content part was added.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="contentIndex">The index of the content part in the item's content array.</param>
        /// <param name="part">The added content part.</param>
        internal void OnResponseContentPartAdded(string eventId, string responseId, string itemId, int? itemIndex, int? contentIndex, RealtimeItemContent part)
        {
            _logger.ResVerbose(eventId, $"Response content part added.\nResponse ID: {responseId}\nItem ID: {itemId}\nItem Index: {itemIndex}\nContent Index: {contentIndex}");

            switch (part.Type)
            {
                case RealtimeItemContentType.Text:
                    _textBuffer.AppendDelta(itemId, itemIndex, contentIndex, part.Text);
                    break;
                case RealtimeItemContentType.Audio:
                    if (_audioEnabled) _audioBuffer.AppendDelta(itemId, itemIndex, contentIndex, part.Audio);
                    if (_transcriptEnabled) _transcriptBuffer.AppendDelta(itemId, itemIndex, contentIndex, part.Transcript);
                    break;
            }
        }

        /// <summary>
        /// Returned when a content part is done streaming in an assistant message item.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response.</param>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="contentIndex">The index of the content part in the item's content array.</param>
        /// <param name="part">The completed content part.</param>
        internal void OnResponseContentPartDone(string eventId, string responseId, string itemId, int? itemIndex, int? contentIndex, RealtimeItemContent part)
        {
            _logger.ResVerbose(eventId, $"Response content part done.\nResponse ID: {responseId}\nItem ID: {itemId}\nItem Index: {itemIndex}\nContent Index: {contentIndex}");
        }

        /// <summary>
        /// Returned when the text value of a "text" content part is updated.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response.</param>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="contentIndex">The index of the content part in the item's content array.</param>
        /// <param name="delta">The text delta.</param>
        internal void OnResponseTextDelta(string eventId, string responseId, string itemId, int? itemIndex, int? contentIndex, string delta)
        {
            if (!_textEnabled) return;
            _logger.Stream(eventId, $"Text delta: <color=white>{delta}</color>\nResponse ID: {responseId}\nItem ID: {itemId}\nItem Index: {itemIndex}\nContent Index: {contentIndex}");
            _textBuffer.AppendDelta(itemId, itemIndex, contentIndex, delta);
        }

        /// <summary>
        /// Returned when the text value of a "text" content part is done streaming.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response.</param>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="contentIndex">The index of the content part in the item's content array.</param>
        /// <param name="text">The final text content.</param>
        internal void OnResponseTextDone(string eventId, string responseId, string itemId, int? itemIndex, int? contentIndex, string text)
        {
            if (!_textEnabled) return;
            _logger.Stream(eventId, $"Text done: <color=white>{text}</color>\nResponse ID: {responseId}\nItem ID: {itemId}\nItem Index: {itemIndex}\nContent Index: {contentIndex}");
            _textBuffer.CompleteDelta(itemId, itemIndex, contentIndex, text);
        }

        /// <summary>
        /// Returned when the model-generated transcription of audio output is updated.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response.</param>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="contentIndex">The index of the content part in the item's content array.</param>
        /// <param name="delta">The transcript delta.</param>
        internal void OnResponseAudioTranscriptDelta(string eventId, string responseId, string itemId, int? itemIndex, int? contentIndex, string delta)
        {
            if (!_transcriptEnabled) return;
            _logger.Stream(eventId, $"Transcript delta: <color=white>{delta}</color>\nResponse ID: {responseId}\nItem ID: {itemId}\nItem Index: {itemIndex}\nContent Index: {contentIndex}");
            _transcriptBuffer.AppendDelta(itemId, itemIndex, contentIndex, delta);
        }

        /// <summary>
        /// Returned when the model-generated transcription of audio output is done streaming.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response.</param>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="contentIndex">The index of the content part in the item's content array.</param>
        /// <param name="transcript">The final transcript of the audio.</param>
        internal void OnResponseAudioTranscriptDone(string eventId, string responseId, string itemId, int? itemIndex, int? contentIndex, string transcript)
        {
            if (!_transcriptEnabled) return;
            _logger.Stream(eventId, $"Transcript done: <color=white>{transcript}</color>\nResponse ID: {responseId}\nItem ID: {itemId}\nItem Index: {itemIndex}\nContent Index: {contentIndex}");
            _transcriptBuffer.CompleteDelta(itemId, itemIndex, contentIndex, transcript);
        }

        /// <summary>
        /// Returned when the model-generated audio is updated.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response.</param>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="contentIndex">The index of the content part in the item's content array.</param>
        /// <param name="delta">Base64-encoded audio data delta.</param>
        internal void OnResponseAudioDelta(string eventId, string responseId, string itemId, int? itemIndex, int? contentIndex, string delta)
        {
            if (!_audioEnabled) return;
            _logger.Stream(eventId, $"Audio delta: <color=white>{itemIndex}-{contentIndex}</color>\nResponse ID: {responseId}\nItem ID: {itemId}");
            _audioBuffer.AppendDelta(itemId, itemIndex, contentIndex, delta);
        }

        /// <summary>
        /// Returned when the model-generated audio is done.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response.</param>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="contentIndex">The index of the content part in the item's content array.</param>
        internal void OnResponseAudioDone(string eventId, string responseId, string itemId, int? itemIndex, int? contentIndex)
        {
            if (!_audioEnabled) return;
            _logger.Stream(eventId, $"Audio done: <color=white>{itemIndex}-{contentIndex}</color>\nResponse ID: {responseId}\nItem ID: {itemId}");
            _audioBuffer.CompleteDelta(itemId, itemIndex, contentIndex, null);
        }

        /// <summary>
        /// Returned when the model-generated function call arguments are updated.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response.</param>
        /// <param name="itemId">The ID of the function call item.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="callId">The ID of the function call.</param>
        /// <param name="delta">The arguments delta as a JSON string.</param>
        internal void OnResponseFunctionCallArgumentsDelta(string eventId, string responseId, string itemId, int? itemIndex, string callId, string delta)
        {
            //logger.Info($"Response function call arguments delta for {eventId}");
            _logger.Stream(eventId, $"Function call arguments delta: <color=white>{delta}</color>\nResponse ID: {responseId}\nItem ID: {itemId}\nItem Index: {itemIndex}\nCall ID: {callId}");
        }

        /// <summary>
        /// Returned when the model-generated function call arguments are done streaming. 
        /// Also emitted when a Response is interrupted, incomplete, or cancelled.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="responseId">The ID of the response.</param>
        /// <param name="itemId">The ID of the function call item.</param>
        /// <param name="outputIndex">The index of the output item in the response.</param>
        /// <param name="callId">The ID of the function call.</param>
        /// <param name="arguments">The final arguments as a JSON string.</param>
        internal void OnResponseFunctionCallArgumentsDone(string eventId, string responseId, string itemId, int? itemIndex, string callId, string arguments)
        {
            //logger.Info($"Response function call arguments done for {eventId}");
            _logger.Stream(eventId, $"Function call arguments done: <color=white>{arguments}</color>\nResponse ID: {responseId}\nItem ID: {itemId}\nItem Index: {itemIndex}\nCall ID: {callId}");
        }

        /// <summary>
        /// Emitted after every "response.done" event to indicate the updated rate limits.
        /// </summary>
        /// <param name="eventId">The unique ID of the event.</param>
        /// <param name="rateLimits">List of rate limit information.</param>
        internal void OnRateLimitsUpdated(string eventId, RateLimit[] rateLimits)
        {
            //logger.Info($"Rate limits updated for {eventId}");
            _logger.Info(eventId, "Rate limits updated.");
        }


        #region Utility Methods

        private void OnReceiveAudio(string base64EncodedAudio)
        {
            if (_audioEventReceiver == null || string.IsNullOrEmpty(base64EncodedAudio)) return;
            RealtimeAudioFormat outputFormat = _controller.OutputFormat;
            float[] audioData = RealtimeUtil.Base64EncodedAudioToAudioData(outputFormat, base64EncodedAudio);
            audioData = RealtimeUtil.ResampleAudio(audioData, outputFormat, _unityAudioSampleRate, _unityAudioChannels);
            _audioEventReceiver.OnReceiveAudio(audioData);
        }

        private void OnReceiveAudioDone(string _)
        {
            _audioEventReceiver?.OnReceiveAudioDone();
        }

        private void OnReceiveTranscript(string transcript) => _transcriptEventReceiver?.OnReceiveText(transcript);
        private void OnReceiveTranscriptDone(string transcript) => _transcriptEventReceiver?.OnReceiveTextDone();
        private void OnReceiveText(string text) => _textEventReceiver?.OnReceiveText(text);
        private void OnReceiveTextDone(string text) => _textEventReceiver?.OnReceiveTextDone();
        private void OnReceiveInputTranscription(string text) => _inputTranscriptionEventReceiver?.OnReceiveText(text);
        private void OnReceiveInputTranscriptionCompleted(string text) => _inputTranscriptionEventReceiver?.OnReceiveTextDone();

        private bool CheckItemStatus(string eventId, RealtimeItem item)
        {
            if (item == null || item.Status == null) return false;

            switch (item.Status)
            {
                case RealtimeItemStatus.Incomplete:
                case RealtimeItemStatus.Failed:
                    _logger.Error(eventId, $"Item Status: <color=yellow>{item.Status}</color>");
                    RealtimeItemStatusDetails details = item.StatusDetails;
                    if (details == null) return false;
                    string message = details.Error?.Message;
                    if (string.IsNullOrEmpty(message)) return false;
                    _logger.Error(message);
                    return false;
                case RealtimeItemStatus.Cancelled:
                    _logger.Warning(eventId, $"Item Status: <color=yellow>{item.Status}</color>");
                    return false;
                default:
                    _logger.Info(eventId, $"Item Status: <color=yellow>{item.Status}</color>");
                    return true;
            }
        }

        #endregion
    }
}