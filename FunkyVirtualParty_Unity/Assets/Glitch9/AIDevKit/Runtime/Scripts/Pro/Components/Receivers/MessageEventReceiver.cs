using Glitch9.AIDevKit.OpenAI;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Event Receiver (Message)")]
    public class MessageEventReceiver : MonoBehaviour, IThreadMessageEventReceiver
    {
        [SerializeField] private UnityEvent<ThreadMessage> onMessageCreated;
        [SerializeField] private UnityEvent<ThreadMessage> onMessageRetrieved;
        [SerializeField] private UnityEvent<ThreadMessage> onMessageCompleted;

        public void OnMessageCompleted(ThreadMessage message) => onMessageCompleted?.Invoke(message);
        public void OnMessageCreated(ThreadMessage message) => onMessageCreated?.Invoke(message);
        public void OnMessageRetrieved(ThreadMessage message) => onMessageRetrieved?.Invoke(message);
    }
}