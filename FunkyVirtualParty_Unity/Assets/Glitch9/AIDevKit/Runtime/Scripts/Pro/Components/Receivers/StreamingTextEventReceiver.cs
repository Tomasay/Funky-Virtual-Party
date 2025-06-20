using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit
{
    [AddComponentMenu("Streaming Event Receiver (Text)")]
    public class StreamingTextEventReceiver : MonoBehaviour, IStreamingTextEventReceiver
    {
        [SerializeField] UnityEvent onReceiveTextStart;
        /// <summary>
        /// Invoked when a partial text is received during real-time transcription.
        /// </summary>
        [Tooltip("Invoked when a partial text is received during real-time transcription.")]
        [SerializeField] UnityEvent<string> onReceiveText;

        /// <summary>
        /// Invoked when the full text is finalized.
        /// </summary>
        [Tooltip("Invoked when the full text is finalized.")]
        [SerializeField] UnityEvent onReceiveTextDone;

        /// <summary>
        /// Invoked when text stream is interrupted.
        /// </summary>
        [Tooltip("Invoked when text stream is interrupted.")]
        [SerializeField] UnityEvent<string> onReceiveError;

        public void OnReceiveError(string errorMessage) => onReceiveError?.Invoke(errorMessage);
        public void OnReceiveText(string delta) => onReceiveText?.Invoke(delta);
        public void OnReceiveTextDone() => onReceiveTextDone?.Invoke();
        public void OnReceiveTextStart() => onReceiveTextStart?.Invoke();
    }
}