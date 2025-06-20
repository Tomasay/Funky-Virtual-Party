using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;
using System;
using System.Net.WebSockets;
using System.Threading;

namespace Glitch9.IO.Networking.WebSocket
{
    public interface IWebSocket : IDisposable
    {
        /// <summary>
        /// The WebSocket connection object.
        /// </summary>
        string SubProtocol { get; }

        /// <summary>
        /// The current state of the WebSocket connection.
        /// </summary>
        public WebSocketState State { get; }

        /// <summary>
        /// Connect to the WebSocket server with a <see cref="Uri"/> endpoint URL.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="headers"></param>
        UniTask ConnectAsync(Uri uri, CancellationToken cancellationToken, params RESTHeader[] headers);

        /// <summary>
        /// Connect to the WebSocket server with a <see cref="string"/> endpoint URL.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="headers"></param>
        UniTask ConnectAsync(string uri, CancellationToken cancellationToken, params RESTHeader[] headers);

        /// <summary>
        /// Close the WebSocket connection.
        /// </summary>
        /// <param name="closeStatus">
        /// The status code indicating the reason for closing the connection.
        /// </param>
        /// <param name="reason">
        /// The reason for closing the connection.
        /// </param>
        /// <param name="cancellationToken"></param>
        UniTask CloseAsync(WebSocketCloseStatus closeStatus, string reason, CancellationToken cancellationToken);

        /// <summary>
        /// Send a message to the WebSocket server.
        /// The message is most likely a JSON string.
        /// This is WebSocket equivalent of application/json.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="token"></param>
        UniTask SendAsync(string jsonString, CancellationToken token);

        /// <summary>
        /// Send a buffer to the WebSocket server.
        /// The buffer is binary data.
        /// This is WebSocket equivalent of multi-part/form-data.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="messageType"></param>
        /// <param name="endOfMessage"></param>
        /// <param name="token"></param>
        UniTask SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken token);

        /// <summary>
        /// Receive a response from the WebSocket server.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken"></param>
        UniTask<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);

        /// <summary>
        /// Event handler for when the WebSocket connection state changes.
        /// </summary>
        Action<WebSocketState> OnStateChanged { get; set; }
    }
}