using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit
{
    [AddComponentMenu("Streaming Event Receiver (Multi-Response)")]
    public class MultiResponseStreamReceiver : ChatCompletionStreamReceiver
    {
        [SerializeField] private UnityEvent<ChatDelta[]> onReceiveDeltaChunk;
        private readonly List<StringBuilder> _sbList = new();
        private readonly List<ToolCall[]> _toolCalls = new();

        protected override void OnReceiveChunk(ChatCompletionChunk chunk)
        {
            ChatDelta[] deltaChunks = chunk.Value?.GetDeltaChunks();
            if (deltaChunks.IsNullOrEmpty()) return;

            onReceiveDeltaChunk?.Invoke(deltaChunks);

            for (int i = 0; i < deltaChunks.Length; i++)
            {
                ChatDelta delta = deltaChunks[i];
                if (delta == null) continue;

                if (i >= _sbList.Count) _sbList.Add(new StringBuilder());

                StringBuilder sb = _sbList[i];

                if (!string.IsNullOrEmpty(delta.Content))
                {
                    sb.Append(delta);
                }

                _toolCalls.Add(delta.ToolCalls);
                _onFunctionManagerCalls?.Invoke(delta.ToolCalls);
            }
        }

        protected override ChatCompletion CreateResult()
        {
            List<string> streamedTexts = new();

            for (int i = 0; i < _sbList.Count; i++)
            {
                StringBuilder sb = _sbList[i];
                streamedTexts.Add(sb.ToString());
                sb.Clear();
            }

            return ChatCompletionFactory.Create(
                streamedTexts,
                _toolCalls,
                _lastChunk?.Usage
            );
        }
    }
}