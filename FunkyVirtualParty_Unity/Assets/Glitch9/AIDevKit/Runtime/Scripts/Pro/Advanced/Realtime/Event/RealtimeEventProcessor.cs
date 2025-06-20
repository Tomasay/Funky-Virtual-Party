using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.WebSocket;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    public class RealtimeEventProcessor
    {
        private readonly Dictionary<int, RealtimeEvent> _eventCache = new();
        public int Count => _eventCache.Count;
        private WebSocketClient<RealtimeEvent> _webSocketClient;

        public RealtimeEventProcessor(WebSocketClient<RealtimeEvent> webSocketClient)
        {
            _webSocketClient = webSocketClient;
        }

        public async UniTask<int> UpdateSession(RealtimeSession session)
        {
            int eventCount = Count;

            RealtimeEvent realtimeEvent = new()
            {
                EventId = CreateEventId(eventCount),
                Type = RealtimeEvent.Request.SessionUpdate,
                Session = session
            };

            await _webSocketClient.SendWebSocketEventAsync(realtimeEvent, ProcessEvent(eventCount, realtimeEvent));
            return eventCount;
        }

        public async UniTask<int> AppendInputAudioBuffer(string base64EncodedAudioBytes)
        {
            int eventCount = Count;

            RealtimeEvent realtimeEvent = new()
            {
                EventId = CreateEventId(eventCount),
                Type = RealtimeEvent.Request.InputAudioBufferAppend,
                Audio = base64EncodedAudioBytes,
            };

            await _webSocketClient.SendWebSocketEventAsync(realtimeEvent, ProcessEvent(eventCount, realtimeEvent));
            return eventCount;
        }

        public UniTask<int> CommitInputAudioBuffer() => CreateEmptyEvent(RealtimeEvent.Request.InputAudioBufferCommit);
        public UniTask<int> ClearInputAudioBuffer() => CreateEmptyEvent(RealtimeEvent.Request.InputAudioBufferClear);

        public async UniTask<int> CreateConversationItem(RealtimeItem item, string previousItemId = null)
        {
            int eventCount = Count;

            RealtimeEvent realtimeEvent = new()
            {
                EventId = CreateEventId(eventCount),
                Type = RealtimeEvent.Request.ConversationItemCreate,
                Item = item,
                PreviousItemId = previousItemId
            };

            await _webSocketClient.SendWebSocketEventAsync(realtimeEvent, ProcessEvent(eventCount, realtimeEvent));
            return eventCount;
        }

        public async UniTask<int> TruncateConversationItem(string itemId, int contentIndex, int audioEndMs)
        {
            int eventCount = Count;

            RealtimeEvent realtimeEvent = new()
            {
                EventId = CreateEventId(eventCount),
                Type = RealtimeEvent.Request.ConversationItemTruncate,
                ItemId = itemId,
                ContentIndex = contentIndex,
                AudioEndMs = audioEndMs
            };

            await _webSocketClient.SendWebSocketEventAsync(realtimeEvent, ProcessEvent(eventCount, realtimeEvent));
            return eventCount;
        }

        public async UniTask<int> DeleteConversationItem(string itemId)
        {
            int eventCount = Count;

            RealtimeEvent realtimeEvent = new()
            {
                EventId = CreateEventId(eventCount),
                Type = RealtimeEvent.Request.ConversationItemDelete,
                ItemId = itemId
            };

            await _webSocketClient.SendWebSocketEventAsync(realtimeEvent, ProcessEvent(eventCount, realtimeEvent));
            return eventCount;
        }

        public async UniTask<int> CreateResponse()
        {
            int eventCount = Count;

            RealtimeEvent realtimeEvent = new()
            {
                EventId = CreateEventId(eventCount),
                Type = RealtimeEvent.Request.ResponseCreate,
            };

            await _webSocketClient.SendWebSocketEventAsync(realtimeEvent, ProcessEvent(eventCount, realtimeEvent));
            return eventCount;
        }

        public UniTask<int> CancelResponse() => CreateEmptyEvent(RealtimeEvent.Request.ResponseCancel);

        public void CancelEvent(int eventId)
        {
            if (_eventCache.TryGetValue(eventId, out RealtimeEvent realtimeEvent))
            {
                realtimeEvent.CancellationTokenSource.Cancel();
                _eventCache.Remove(eventId);
            }
            else
            {
                OpenAI.DefaultInstance.Logger.Warning($"Cannot cancel event with ID: {eventId}. Event not found.");
            }
        }

        private async UniTask<int> CreateEmptyEvent(string eventType)
        {
            int eventCount = Count;

            RealtimeEvent realtimeEvent = new()
            {
                EventId = CreateEventId(eventCount),
                Type = eventType,
            };

            await _webSocketClient.SendWebSocketEventAsync(realtimeEvent, ProcessEvent(eventCount, realtimeEvent));
            return eventCount;
        }

        // Id Format: "event_000"
        private string CreateEventId(int eventCount) => $"event_{eventCount:D3}";

        private CancellationTokenSource ProcessEvent(int eventCount, RealtimeEvent realtimeEvent)
        {
            _eventCache.AddOrUpdate(eventCount, realtimeEvent);
            return realtimeEvent.CancellationTokenSource;
        }
    }
}