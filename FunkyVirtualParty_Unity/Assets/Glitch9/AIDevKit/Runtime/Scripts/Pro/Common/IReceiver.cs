using System.Net.WebSockets;

namespace Glitch9.AIDevKit
{
    public interface IStreamingTextEventReceiver
    {
        /// <summary>
        /// Invoked when the text is received for the first time.
        /// </summary>
        void OnReceiveTextStart();

        /// <summary>
        /// Invoked when a partial text is received.
        /// </summary>
        void OnReceiveText(string delta);

        /// <summary>
        /// Invoked when the full text is finalized.
        /// </summary>
        void OnReceiveTextDone();

        /// <summary>
        /// Invoked when text stream is interrupted.
        /// </summary>
        void OnReceiveError(string errorMessage);
    }

    public interface IStreamingAudioEventReceiver
    {
        /// <summary>
        /// Invoked when a chunk of audio data (float array) is received during streaming.
        /// </summary>
        void OnReceiveAudio(float[] audioData);

        /// <summary>
        /// Invoked when the full audio stream has been received and finalized.
        /// </summary>
        void OnReceiveAudioDone();
    }

    public interface IWebSocketEventReceiver
    {
        /// <summary>
        /// Invoked when a message is received from the WebSocket.
        /// </summary>
        void OnWebSocketStateChanged(WebSocketState state);
    }
}