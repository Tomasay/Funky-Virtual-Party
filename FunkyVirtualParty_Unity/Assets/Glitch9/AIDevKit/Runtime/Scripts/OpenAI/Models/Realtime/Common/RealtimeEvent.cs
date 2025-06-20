using System.Threading;
using Glitch9.IO.Networking.WebSocket;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    /// <summary>
    /// Handles all the different types of <c>Realtime API</c> events 
    /// that can be sent to or received from the OpenAI API.
    /// </summary>
    public class RealtimeEvent : IWebSocketMessage
    {
        public static class Request
        {
            /// <summary>Send this event to update the session’s default configuration.</summary>
            public const string SessionUpdate = "session.update";

            /// <summary>Send this event to append audio bytes to the input audio buffer.</summary>
            public const string InputAudioBufferAppend = "input_audio_buffer.append";

            /// <summary>Send this event to commit audio bytes to a user message.</summary>
            public const string InputAudioBufferCommit = "input_audio_buffer.commit";

            /// <summary>Send this event to clear the audio bytes in the buffer.</summary>
            public const string InputAudioBufferClear = "input_audio_buffer.clear";

            /// <summary>Send this event when adding an item to the conversation.</summary>
            public const string ConversationItemCreate = "conversation.item.create";
            /// <summary>Send this event when you want to truncate a previous assistant message’s audio.</summary>
            public const string ConversationItemTruncate = "conversation.item.truncate";

            /// <summary>Send this event when you want to remove any item from the conversation history.</summary>
            public const string ConversationItemDelete = "conversation.item.delete";

            /// <summary>Send this event to trigger a response generation.</summary>
            public const string ResponseCreate = "response.create";

            /// <summary>Send this event to cancel an in-progress response.</summary>
            public const string ResponseCancel = "response.cancel";
        }

        public static class Response
        {
            /// <summary>Returned when an error occurs.</summary>
            public const string Error = "error";

            /// <summary>Returned when a session is created. Emitted automatically when a new connection is established.</summary>
            public const string SessionCreated = "session.created";

            /// <summary>Returned when a session is updated.</summary>
            public const string SessionUpdated = "session.updated";

            /// <summary>Returned when an input audio buffer is committed, either by the client or automatically in server VAD mode.</summary>
            public const string InputAudioBufferCommitted = "input_audio_buffer.committed";

            /// <summary>Returned when the input audio buffer is cleared by the client.</summary>
            public const string InputAudioBufferCleared = "input_audio_buffer.cleared";

            /// <summary>Returned in server turn detection mode when speech is detected.</summary>
            public const string InputAudioBufferSpeechStarted = "input_audio_buffer.speech_started";

            /// <summary>Returned in server turn detection mode when speech stops.</summary>
            public const string InputAudioBufferSpeechStopped = "input_audio_buffer.speech_stopped";

            /// <summary>Returned when a conversation is created. Emitted right after session creation.</summary>
            public const string ConversationCreated = "conversation.created";

            /// <summary>Returned when input audio transcription is enabled and a transcription delta is received.</summary>
            public const string ConversationItemInputAudioTranscriptionDelta = "conversation.item.input_audio_transcription.delta";

            /// <summary>Returned when input audio transcription is enabled and a transcription succeeds.</summary>
            public const string ConversationItemInputAudioTranscriptionCompleted = "conversation.item.input_audio_transcription.completed";

            /// <summary>Returned when input audio transcription is configured, and a transcription request for a user message failed.</summary>
            public const string ConversationItemInputAudioTranscriptionFailed = "conversation.item.input_audio_transcription.failed";

            /// <summary>Returned when a conversation item is created.</summary>
            public const string ConversationItemCreated = "conversation.item.created";

            /// <summary>Returned when an earlier assistant audio message item is truncated by the client.</summary>
            public const string ConversationItemTruncated = "conversation.item.truncated";

            /// <summary>Returned when an item in the conversation is deleted.</summary>
            public const string ConversationItemDeleted = "conversation.item.deleted";

            /// <summary>Returned when a new Response is created. The first event of response creation, where the response is in an initial state of "in_progress".</summary>
            public const string ResponseCreated = "response.created";

            /// <summary>Returned when a Response is done streaming. Always emitted, no matter the final state.</summary>
            public const string ResponseDone = "response.done";

            /// <summary>Returned when a new Item is created during response generation.</summary>
            public const string ResponseOutputItemAdded = "response.output_item.added";

            /// <summary>Returned when an Item is done streaming. Also emitted when a Response is interrupted, incomplete, or cancelled.</summary>
            public const string ResponseOutputItemDone = "response.output_item.done";

            /// <summary>Returned when a new content part is added to an assistant message item during response generation.</summary>
            public const string ResponseContentPartAdded = "response.content_part.added";

            /// <summary>Returned when a content part is done streaming in an assistant message item. Also emitted when a Response is interrupted, incomplete, or cancelled.</summary>
            public const string ResponseContentPartDone = "response.content_part.done";

            /// <summary>Returned when the text value of a "text" content part is updated.</summary>
            public const string ResponseTextDelta = "response.text.delta";

            /// <summary>Returned when the text value of a "text" content part is done streaming. Also emitted when a Response is interrupted, incomplete, or cancelled.</summary>
            public const string ResponseTextDone = "response.text.done";

            /// <summary>Returned when the model-generated transcription of audio output is updated.</summary>
            public const string ResponseAudioTranscriptDelta = "response.audio_transcript.delta";

            /// <summary>Returned when the model-generated transcription of audio output is done streaming. Also emitted when a Response is interrupted, incomplete, or cancelled.</summary>
            public const string ResponseAudioTranscriptDone = "response.audio_transcript.done";

            /// <summary>Returned when the model-generated audio is updated.</summary>
            public const string ResponseAudioDelta = "response.audio.delta";

            /// <summary>Returned when the model-generated audio is done. Also emitted when a Response is interrupted, incomplete, or cancelled.</summary>
            public const string ResponseAudioDone = "response.audio.done";

            /// <summary>Returned when the model-generated function call arguments are updated.</summary>
            public const string ResponseFunctionCallArgumentsDelta = "response.function_call_arguments.delta";

            /// <summary>Returned when the model-generated function call arguments are done streaming. Also emitted when a Response is interrupted, incomplete, or cancelled.</summary>
            public const string ResponseFunctionCallArgumentsDone = "response.function_call_arguments.done";

            /// <summary>Emitted after every "response.done" event to indicate the updated rate limits.</summary>
            public const string RateLimitsUpdated = "rate_limits.updated";
        }

        [JsonIgnore] public CancellationTokenSource CancellationTokenSource { get; set; } = new();

        /// <summary>
        /// <para><c>Requests</c>: Optional client-generated ID used to identify this event.</para>
        /// <para><c>Responses</c>: The unique ID of the realtime event.</para>
        /// </summary>
        [JsonProperty("event_id")] public string EventId { get; set; }

        /// <summary>
        /// The event type.
        /// </summary>
        [JsonProperty("type")] public string Type { get; set; }

        /// <summary>
        /// The session resource.
        /// </summary>
        [JsonProperty("session")] public RealtimeSession Session { get; set; }

        /// <summary>
        /// <para><see cref="RealtimeEventId.ResponseCreate"/>: Configuration for the response.</para>
        /// </summary>
        [JsonProperty("response")] public RealtimeItem ResponseItem { get; set; }

        /// <summary>
        /// Details of the error.
        /// </summary>
        [JsonProperty("error")] public ErrorResponse Error { get; set; }

        /// <summary>
        /// The conversation resource.
        /// </summary>
        [JsonProperty("conversation")] public Conversation Conversation { get; set; }

        /// <summary>
        /// The item to add to the conversation.
        /// </summary>
        [JsonProperty("item")] public RealtimeItem Item { get; set; }

        /// <summary>
        /// The ID of the response.
        /// </summary>
        [JsonProperty("response_id")] public string ResponseId { get; set; }

        /// <summary>
        /// The ID of the item or the function call item.
        /// </summary>
        [JsonProperty("item_id")] public string ItemId { get; set; }

        /// <summary>
        /// The ID of the preceding item after which the new item will be inserted.
        /// </summary>
        [JsonProperty("previous_item_id")] public string PreviousItemId { get; set; }

        /// <summary>
        /// The index of the output item in the response.
        /// </summary>
        [JsonProperty("output_index")] public int? OutputIndex { get; set; }

        /// <summary>
        /// The index of the content part in the item's content array.
        /// </summary>
        [JsonProperty("content_index")] public int? ContentIndex { get; set; }

        /// <summary>
        /// The ID of the function call.
        /// </summary>
        [JsonProperty("call_id")] public string CallId { get; set; }

        /// <summary>
        /// <see cref="RealtimeEventId.ResponseAudioTranscriptDone"/>: The final transcript of the audio.
        /// </summary>
        [JsonProperty("transcript")] public string Transcript { get; set; }

        /// <summary>
        /// The final text content.
        /// </summary>
        [JsonProperty("text")] public string Text { get; set; }

        /// <summary>
        /// Base64-encoded audio bytes.
        /// </summary>
        /// <remarks>
        /// This property is only used when the event type is <see cref="RealtimeEventId.InputAudioBufferAppend"/>.
        /// </remarks>
        [JsonProperty("audio")] public string Audio { get; set; }

        /// <summary>
        /// <para><see cref="RealtimeEventId.ResponseContentPartAdded"/>: The content part that was added.</para>
        /// <para><see cref="RealtimeEventId.ResponseContentPartDone"/>: The content part that is done.</para>
        /// </summary>
        [JsonProperty("part")] public RealtimeItemContent Part { get; set; }

        /// <summary>
        /// The final arguments as a JSON string.
        /// </summary>
        [JsonProperty("arguments")] public string Arguments { get; set; }

        /// <summary>
        /// <para><see cref="RealtimeEventId.ResponseTextDelta"/>: The text delta.</para>
        /// <para><see cref="RealtimeEventId.ResponseAudioTranscriptDelta"/>: The transcript delta.</para>
        /// <para><see cref="RealtimeEventId.ResponseAudioDelta"/>: Base64-encoded audio data delta.</para>
        /// <para><see cref="RealtimeEventId.ResponseFunctionCallArgumentsDelta"/>: The arguments delta as a JSON string.</para>
        /// </summary>
        [JsonProperty("delta")] public string Delta { get; set; }

        /// <summary>
        /// List of rate limit information.
        /// </summary>
        [JsonProperty("rate_limits")] public RateLimit[] RateLimits { get; set; }

        /// <summary>
        /// Milliseconds since the session started when speech was detected.
        /// </summary>
        [JsonProperty("audio_start_ms")] public int? AudioStartMs { get; set; }

        /// <summary>
        /// Inclusive duration up to which audio is truncated, in milliseconds.
        /// </summary>
        [JsonProperty("audio_end_ms")] public int? AudioEndMs { get; set; }
    }
}
