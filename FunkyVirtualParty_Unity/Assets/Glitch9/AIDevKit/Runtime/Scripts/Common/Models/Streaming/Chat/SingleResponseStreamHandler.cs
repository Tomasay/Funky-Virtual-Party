using System;
using System.Collections.Generic;
using System.Text;

namespace Glitch9.AIDevKit
{
    public class SingleResponseStreamHandler : ChatCompletionStreamHandler
    {
        public Action<string> onReceiveText;
        public Action<string> onReceiveRefusal;
        public Action<ToolCall[]> onReceiveToolCalls;
        private readonly StringBuilder _sb = new();
        private readonly List<ToolCall> _toolCalls = new();

        public SingleResponseStreamHandler() { }
        public SingleResponseStreamHandler(
            Action onStart = null,
            Action<string> onReceiveText = null,
            Action<string> onReceiveRefusal = null,
            Action<ToolCall[]> onReceiveToolCalls = null,
            Action<string> onError = null,
            Action<ChatCompletion> onDone = null)
        {
            if (onStart != null) this.onStart += onStart;
            if (onReceiveText != null) this.onReceiveText += onReceiveText;
            if (onReceiveRefusal != null) this.onReceiveRefusal += onReceiveRefusal;
            if (onReceiveToolCalls != null) this.onReceiveToolCalls += onReceiveToolCalls;
            if (onError != null) this.onError += onError;
            if (onDone != null) this.onDone += onDone;
        }

        internal override void ProcessChunk(ChatCompletionChunk chunk)
        {
            ChatDelta delta = chunk?.Value?.FirstDelta();
            if (delta == null) return;

            string deltaText = delta.Content;

            if (deltaText != null)
            {
                _sb.Append(deltaText);
                onReceiveText?.Invoke(deltaText);
            }
            else
            {
                //AIDevKitDebug.Red("Received a chunk with no content.");
            }

            if (!string.IsNullOrEmpty(delta.Refusal))
            {
                onReceiveRefusal?.Invoke(delta.Refusal);
            }

            if (delta.ToolCalls.IsNotNullOrEmpty())
            {
                _toolCalls.AddRange(delta.ToolCalls);
                onReceiveToolCalls?.Invoke(delta.ToolCalls);
                _funtionManagerCallback?.Invoke(delta.ToolCalls);
            }
        }

        internal override ChatCompletion BuildFinalResult()
        {
            string streamedText = _sb.ToString();
            AIDevKitDebug.Mark($"SingleResponseStreamHandler: Received text: {streamedText}");
            _sb.Clear();

            if (_task.textProcessor != null)
            {
                if (_lastChunk == null || _lastChunk.Value == null)
                {
                    AIDevKitDebug.Error("Last chunk is null, cannot process text.");
                }
                else
                {
                    _lastChunk.Value.ProcessText(_task.textProcessor);
                }
            }

            return ChatCompletionFactory.Create(
                streamedText,
                _toolCalls.ToArray(),
                _lastChunk?.Usage
            );
        }
    }
}