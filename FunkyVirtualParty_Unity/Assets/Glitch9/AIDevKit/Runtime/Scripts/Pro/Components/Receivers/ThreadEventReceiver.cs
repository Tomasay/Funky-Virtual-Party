using Glitch9.AIDevKit.OpenAI;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Event Receiver (Thread)")]
    public class ThreadEventReceiver : MonoBehaviour, IThreadEventReceiver
    {
        [SerializeField] private UnityEvent<Thread> onThreadCreated;
        [SerializeField] private UnityEvent<Thread> onThreadRetrieved;
        [SerializeField] private UnityEvent<Thread> onThreadUpdated;

        public void OnThreadCreated(Thread thread) => onThreadCreated?.Invoke(thread);
        public void OnThreadRetrieved(Thread thread) => onThreadRetrieved?.Invoke(thread);
        public void OnThreadUpdated(Thread thread) => onThreadUpdated?.Invoke(thread);
    }
}