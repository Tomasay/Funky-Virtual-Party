using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using UnityEngine;

namespace Glitch9.IO.Networking.WebSocket
{
    public interface IWebSocketMessage
    {
        CancellationTokenSource CancellationTokenSource { get; set; }
    }

    public class WebSocketClient<TWebSocketMessage>
        where TWebSocketMessage : IWebSocketMessage
    {
        public string Name { get; private set; }
        public IWebSocket WebSocket => _webSocket;
        private IWebSocket _webSocket;
        private Action<TWebSocketMessage> _onReceiveMessage;
        private List<CancellationTokenSource> _cancellationTokenSources;
        private JsonSerializerSettings _jsonSettings;
        private WebSocketStateManager _webSocketStateManager;
        private DefaultLogger _logger;
        private bool _autoManage;
        private int _byteSize;

        private WebSocketClient() { }

        private void Initialize(string clientName, Action<TWebSocketMessage> onReceiveMessage, JsonSerializerSettings jsonSettings, bool autoManage, int byteSize)
        {
            Name = clientName;
#if UNITY_WEBGL && !UNITY_EDITOR
            _webSocket = new WebGLWebSocket();
#else
            _webSocket = new UnityWebSocket(new ClientWebSocket());
#endif
            _onReceiveMessage = onReceiveMessage;
            _jsonSettings = jsonSettings;
            _autoManage = autoManage;
            _byteSize = byteSize;
            _logger = new DefaultLogger(string.IsNullOrEmpty(clientName) ? "WebSocketClient" : clientName);
            _cancellationTokenSources = new List<CancellationTokenSource>();
        }

        public WebSocketClient(Action<TWebSocketMessage> onEventReceived, JsonSerializerSettings jsonSettings, bool autoManage = true, int byteSize = 1024)
        => Initialize(null, onEventReceived, jsonSettings, autoManage, byteSize);

        public WebSocketClient(string clientName, Action<TWebSocketMessage> onEventReceived, JsonSerializerSettings jsonSettings, bool autoManage = true, int byteSize = 1024)
        => Initialize(clientName, onEventReceived, jsonSettings, autoManage, byteSize);

        public UniTask<IWebSocket> CreateWebSocketConnectionAsync(string url, params RESTHeader[] headers)
        => CreateWebSocketConnectionAsync(url, null, headers);

        public async UniTask<IWebSocket> CreateWebSocketConnectionAsync(string url, CancellationTokenSource cancellationToken = null, params RESTHeader[] headers)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.Error("WebSocket URL is null or empty.");
                return null;
            }

            _logger.Info($"Creating WebSocket connection to {url}.");

            await _webSocket.ConnectAsync(url, HandleCancellationToken(cancellationToken), headers);

            if (_autoManage)
            {
                // Create WebSocketStateManager(MonoBehaviour) if it doesn't exist.
                // Create a GameObject in the top hierarchy and attach WebSocketStateManager to it.
                if (_webSocketStateManager == null)
                {
                    _logger.Info("Creating WebSocketStateManager.");

                    GameObject go = new GameObject("WebSocketStateManager");
                    _webSocketStateManager = go.AddComponent<WebSocketStateManager>();
                    _webSocketStateManager.Initialize(_webSocket);
                }
            }

            _logger.Info("WebSocket connection created successfully.");
            _logger.Info("Starting WebSocket listening.");

            // Start listening for WebSocket events.
            // await StartWebSocketListening();
            return _webSocket;
        }

        public async UniTask SendWebSocketEventAsync(TWebSocketMessage webSocketEvent, CancellationTokenSource cancellationToken = null)
        {
            //_logger.Info($"Sending WebSocket event: {webSocketEvent}.");

            string jsonString = JsonConvert.SerializeObject(webSocketEvent, _jsonSettings);

            //_logger.Info($"Sending JSON: {jsonString}");

            await _webSocket.SendAsync(jsonString, HandleCancellationToken(cancellationToken));
        }

        public async UniTask CloseWebSocketConnectionAsync(string closeReason = "No reason provided.", WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure, CancellationTokenSource cancellationToken = null)
        {
            _logger.Info($"Closing WebSocket connection. Reason: {closeReason}.");

            await _webSocket.CloseAsync(closeStatus, closeReason, HandleCancellationToken(cancellationToken));
        }

        public async UniTask StartWebSocketListening()
        {
            while (_webSocket?.State == WebSocketState.Open)
            {
                await ReceiveWebSocketEventAsync();
                await UniTask.Yield();  // Yield to the next frame.
            }
        }

        ~WebSocketClient()
        {
            Dispose();
        }

        public void Dispose()
        {
            CancelAllOperations();
            _cancellationTokenSources.Clear();
            _webSocket?.Dispose();
        }

        public void CancelAllOperations()
        {
            int count = _cancellationTokenSources.Count;
            _logger.Info($"Cancelling {count} WebSocket operations.");

            foreach (CancellationTokenSource cts in _cancellationTokenSources)
            {
                cts.Cancel();
            }
        }

        private async UniTask ReceiveWebSocketEventAsync()
        {
            _logger.Info("Receiving WebSocket event...");

            // 메시지 전체를 조립하기 위한 StringBuilder
            using (StringBuilderPool.Get(out var messageBuilder))
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[_byteSize]);

                try
                {
                    // 메시지 수신을 위한 루프
                    while (true)
                    {
                        WebSocketReceiveResult result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);

                        // 받은 데이터를 문자열로 변환하고 StringBuilder에 추가
                        string jsonString = System.Text.Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                        messageBuilder.Append(jsonString);

                        // EndOfMessage가 true일 때만 처리
                        if (result.EndOfMessage)
                        {
                            //_logger.Info("Received complete WebSocket message.");

                            // 텍스트 메시지 처리
                            if (result.MessageType == WebSocketMessageType.Text)
                            {
                                // 전체 메시지를 조립한 후 역직렬화 시도
                                string completeMessage = messageBuilder.ToString();

                                try
                                {
                                    TWebSocketMessage webSocketEvent = JsonConvert.DeserializeObject<TWebSocketMessage>(completeMessage, _jsonSettings);

                                    if (webSocketEvent == null)
                                    {
                                        _logger.Error($"Deserialization returned null. JSON: {completeMessage}");
                                        return;
                                    }

                                    if (_onReceiveMessage == null)
                                    {
                                        _logger.Error("OnEventReceived delegate is null.");
                                        return;
                                    }

                                    _onReceiveMessage.Invoke(webSocketEvent);
                                }
                                catch (JsonException jsonEx)
                                {
                                    _logger.Error($"JSON Deserialization Error: {jsonEx.Message}\nRaw: {completeMessage}");
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error($"Unhandled exception during message handling: {ex.Message}\n{ex.StackTrace}");
                                }
                            }
                            else if (result.MessageType == WebSocketMessageType.Binary)
                            {
                                _logger.Warning("Received binary message. This WebSocketClient only supports text messages.");
                            }
                            else if (result.MessageType == WebSocketMessageType.Close)
                            {
                                WebSocketCloseStatus closeStatus = result.CloseStatus ?? WebSocketCloseStatus.Empty;
                                string closeStatusDescription = string.IsNullOrEmpty(result.CloseStatusDescription)
                                    ? "Connection closed by server. No reason provided."
                                    : result.CloseStatusDescription;
                                await CloseWebSocketConnectionAsync(closeStatusDescription, closeStatus);
                                _logger.Info($"WebSocket closed: {closeStatus} - {closeStatusDescription}");
                                break;
                            }

                            // 메시지를 처리했으므로 StringBuilder 초기화
                            messageBuilder.Clear();
                        }
                    }
                }
                catch (WebSocketException wsEx)
                {
                    _logger.Error($"WebSocket Error: {wsEx.Message}");

                    // 연결을 닫을 수 있는 방법 추가
                    await CloseWebSocketConnectionAsync("WebSocket exception occurred.", WebSocketCloseStatus.InternalServerError);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Unexpected Error: {ex.Message}");
                }
            }
        }


        private CancellationToken HandleCancellationToken(CancellationTokenSource cancellationToken = null)
        {
            cancellationToken ??= new CancellationTokenSource();
            _cancellationTokenSources.Add(cancellationToken);
            return cancellationToken.Token;
        }
    }
}