using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Glitch9.IO.Networking.WebSocket
{
    public class UnityWebSocket : IWebSocket
    {
        public ClientWebSocket Socket { get; }
        public Action<WebSocketState> OnStateChanged { get; set; }

        public string SubProtocol => Socket != null ? Socket.SubProtocol : string.Empty;
        public WebSocketState State => Socket != null ? Socket.State : WebSocketState.None;

        public UnityWebSocket(ClientWebSocket socket)
        {
            Socket = socket;
        }

        public async UniTask ConnectAsync(Uri uri, CancellationToken cancellationToken, params RESTHeader[] headers)
        {
            if (!headers.IsNullOrEmpty())
            {
                foreach (var header in headers)
                {
                    Socket.Options.SetRequestHeader(header.Name, header.Value);
                    // UnityEngine.Debug.Log($"Header: {header.Name} = {header.Value}");
                }
            }

            await Socket.ConnectAsync(uri, cancellationToken);
            OnStateChanged?.Invoke(Socket.State);
        }

        public UniTask ConnectAsync(string uri, CancellationToken cancellationToken, params RESTHeader[] headers)
        {
            return ConnectAsync(new Uri(uri), cancellationToken, headers);
        }

        public async UniTask CloseAsync(WebSocketCloseStatus closeCode, string reason, CancellationToken cancellationToken)
        {
            if (Socket.CloseStatus.HasValue) return;
            await Socket.CloseAsync(closeCode, reason, cancellationToken);
            OnStateChanged?.Invoke(Socket.State);
        }

        public async UniTask<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            var result = await Socket.ReceiveAsync(buffer, cancellationToken);

            if (Socket.State != WebSocketState.Open)
            {
                OnStateChanged?.Invoke(Socket.State); // 예: Aborted, Closed
            }

            return result;
        }

        public async UniTask SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            await Socket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
        }

        public async UniTask SendAsync(string message, CancellationToken token)
        {
            var dataToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await Socket.SendAsync(dataToSend, WebSocketMessageType.Text, true, token);
        }


        public void Dispose()
        {
            Socket.Dispose();
        }
    }
}