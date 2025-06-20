using Glitch9.AIDevKit.OpenAI;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Event Receiver (Assistant)")]
    public class AssistantEventReceiver : MonoBehaviour, IAssistantEventReceiver
    {
        [SerializeField] private UnityEvent<Assistant> onAssistantCreated;
        [SerializeField] private UnityEvent<Assistant> onAssistantRetrieved;
        [SerializeField] private UnityEvent<Assistant> onAssistantUpdated;
        public void OnAssistantCreated(Assistant assistant) => onAssistantCreated?.Invoke(assistant);
        public void OnAssistantRetrieved(Assistant assistant) => onAssistantRetrieved?.Invoke(assistant);
        public void OnAssistantUpdated(Assistant assistant) => onAssistantUpdated?.Invoke(assistant);
    }
}