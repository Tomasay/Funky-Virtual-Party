namespace Glitch9.IO.Networking.WebSocket
{
    public static class WebSocketStrings
    {
        // Names
        public const string WebSocketClient = "WebSocketClient";

        // Connection 
        public const string UrlIsNullOrEmpty = "WebSocket URL is null or empty.";
        public const string ConnectingTo = "Connecting to WebSocket server: {0}";
        public const string ConnectionEstablished = "WebSocket connection successfully established.";
        public const string ConnectionFailed = "Failed to connect to the WebSocket server.";
        public const string ConnectionFailed_NullClient = "WebSocket client is null. Failed to establish a WebSocket connection.";
        public const string ConnectionFailed_NullSession = "WebSocket session is null. Failed to establish a WebSocket connection.";
        public const string Reconnecting = "Reconnecting to WebSocket server...";
        public const string ConnectionClosedByServer = "WebSocket connection was closed by the server.";
        public const string ConnectionClosed = "WebSocket connection closed. Status: {0}, Reason: {1}";
        public const string ConnectionAborted = "WebSocket connection was aborted unexpectedly.";

        // Session
        public const string UpdateFailed_NullSession = "Failed to update WebSocket session. Session is null.";

        // Sending
        public const string SendingMessage = "Sending WebSocket message.";
        public const string SendFailed = "Failed to send WebSocket message.";
        public const string SendPayload = "Sending JSON payload: {0}";

        // Receiving
        public const string ReceivingMessage = "Receiving WebSocket message...";
        public const string ReceiveFailed = "Failed to receive WebSocket message.";
        public const string ReceivedBinaryMessage = "Received a binary message. Binary messages are not supported.";
        public const string ReceivedInvalidFormat = "Received message has an invalid format.";

        // Deserialization
        public const string DeserializationFailed = "Failed to deserialize WebSocket message.";
        public const string DeserializationReturnedNull = "Deserialization returned null. Raw JSON: {0}";

        // Listener
        public const string StartingListener = "Starting WebSocket receive loop.";
        public const string ListenerStopped = "WebSocket receive loop stopped.";
        public const string ListenerCrashed = "An exception occurred while processing WebSocket messages: {0}";

        // Cancellation
        public const string CancellingOperations = "Cancelling {0} pending WebSocket operations.";

        // General
        public const string UnexpectedError = "An unexpected error occurred: {0}";
        public const string WebSocketStateChanged = "WebSocket state changed: {0}";

        // Audio
        public const string SpeakingEnded_CommittingAudioBuffer = "Speaking ended. Committing audio buffer.";
        public const string SpeakingEnded_NoAudioBuffer = "Audio buffer is empty. No audio to commit.";
        public const string ReceivedAudio_NullOrEmpty = "Received audio data is null or empty. No audio to process.";
    }

}