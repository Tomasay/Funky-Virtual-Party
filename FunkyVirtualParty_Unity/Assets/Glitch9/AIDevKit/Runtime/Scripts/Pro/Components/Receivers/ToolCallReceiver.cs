using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Event Receiver (Tool Calls)")]
    public class ToolCallReceiver : MonoBehaviour, IToolCallReceiver
    {
        [SerializeField] private UnityEvent<ToolCall[]> onReceiveToolCalls;
        public void OnReceiveToolCalls(ToolCall[] toolCalls) => onReceiveToolCalls?.Invoke(toolCalls);
    }
}