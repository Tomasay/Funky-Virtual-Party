using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit
{
    [AddComponentMenu("Event Receiver (WebSocket)")]
    public class WebSocketEventReceiver : MonoBehaviour, IWebSocketEventReceiver
    {
        /// <summary>
        /// Invoked when the WebSocket state changes.
        /// </summary>
        [Tooltip("Invoked when the WebSocket state changes.")]
        [SerializeField] private UnityEvent<WebSocketState> onWebSocketStateChanged;
        public void OnWebSocketStateChanged(WebSocketState state) => onWebSocketStateChanged?.Invoke(state);
    }
}