using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    /// <summary>
    /// Event handler for real-time assistant events.
    /// Provides UnityEvents that are triggered by the RealtimeEventReceiver.
    /// </summary>
    [AddComponentMenu("Streaming Event Receiver (PCM Audio)")]
    public class StreamingAudioEventReceiver : MonoBehaviour, IStreamingAudioEventReceiver
    {
        /// <summary>
        /// Invoked when a chunk of audio data (float array) is received during streaming.
        /// </summary>
        [Tooltip("Invoked when a chunk of audio data is received during streaming.")]
        [SerializeField] private UnityEvent<float[]> onReceiveAudio;

        /// <summary>
        /// Invoked when the full audio stream has been received and finalized.
        /// </summary>
        [Tooltip("Invoked when the full audio stream has been received and finalized.")]
        [SerializeField] private UnityEvent onReceiveAudioDone;

        public void OnReceiveAudio(float[] audioData) => onReceiveAudio?.Invoke(audioData);
        public void OnReceiveAudioDone() => onReceiveAudioDone?.Invoke();
    }
}
