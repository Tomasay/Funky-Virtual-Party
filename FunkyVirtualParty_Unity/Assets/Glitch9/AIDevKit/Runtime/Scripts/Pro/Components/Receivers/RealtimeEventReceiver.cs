using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    public interface IRealtimeEventReceiver
    {
        void OnEventTypeChanged(string eventType);

        // TODO: Add more events as needed
    }

    /// <summary>
    /// Event handler for real-time assistant events.
    /// Provides UnityEvents that are triggered by the RealtimeEventReceiver.
    /// </summary>
    [AddComponentMenu("Event Receiver (Realtime)")]
    public class RealtimeEventReceiver : MonoBehaviour, IRealtimeEventReceiver
    {
        /// <summary>
        /// Invoked when the event type changes.
        /// </summary>
        [Tooltip("Invoked when the event type changes.")]
        [SerializeField] private UnityEvent<string> onEventTypeChanged;

        public void OnEventTypeChanged(string eventType) => onEventTypeChanged?.Invoke(eventType);
    }
}
