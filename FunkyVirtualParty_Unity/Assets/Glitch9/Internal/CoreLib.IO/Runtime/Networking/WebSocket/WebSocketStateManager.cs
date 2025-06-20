using Cysharp.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using UnityEngine;

namespace Glitch9.IO.Networking.WebSocket
{
    /// <summary>
    /// Used when user chooses to automatically manage WebSocket connections.
    /// </summary>
    public class WebSocketStateManager : MonoBehaviour
    {
        private IWebSocket _webSocket;

        public void Initialize(IWebSocket webSocket)
        {
            _webSocket = webSocket;
        }

        private void OnApplicationQuit()
        {
            _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Application is closing.", CancellationToken.None).Forget();
        }

        private void OnDestroy()
        {
            _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocketStateManager GameObject is destroyed.", CancellationToken.None).Forget();
        }
    }
}