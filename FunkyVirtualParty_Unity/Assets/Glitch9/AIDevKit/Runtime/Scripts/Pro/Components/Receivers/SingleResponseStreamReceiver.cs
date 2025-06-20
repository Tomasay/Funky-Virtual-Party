using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit
{
    [AddComponentMenu("Streaming Event Receiver (Single-Response)")]
    public class SingleResponseStreamReceiver : ChatCompletionStreamReceiver
    {
        [SerializeField] private UnityEvent<string> onReceiveText;
        [SerializeField] private UnityEvent<string> onReceiveRefusal;
        [SerializeField] private UnityEvent<ToolCall[]> onReceiveToolCalls;
        private readonly StringBuilder _sb = new();
        private readonly List<ToolCall> _toolCalls = new();

        protected override void OnReceiveChunk(ChatCompletionChunk chunk)
        {
            ChatDelta delta = chunk?.Value?.FirstDelta();
            if (delta == null) return;

            string deltaText = delta.Content;

            if (deltaText != null)
            {
                _sb.Append(deltaText);
                onReceiveText?.Invoke(deltaText);
            }

            if (!string.IsNullOrEmpty(delta.Refusal))
            {
                onReceiveRefusal?.Invoke(delta.Refusal);
            }

            if (delta.ToolCalls.IsNotNullOrEmpty())
            {
                _toolCalls.AddRange(delta.ToolCalls);
                onReceiveToolCalls?.Invoke(delta.ToolCalls);
                _onFunctionManagerCalls?.Invoke(delta.ToolCalls);
            }
        }

        protected override ChatCompletion CreateResult()
        {
            string streamedText = _sb.ToString();
            _sb.Clear();

            return ChatCompletionFactory.Create(
                streamedText,
                _toolCalls.ToArray(),
                _lastChunk?.Usage
            );
        }
    }
}