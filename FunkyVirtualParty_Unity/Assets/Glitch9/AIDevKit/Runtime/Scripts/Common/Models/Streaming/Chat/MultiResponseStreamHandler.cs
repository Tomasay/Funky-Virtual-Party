using System;
using System.Collections.Generic;
using System.Text;

namespace Glitch9.AIDevKit
{
    public class MultiResponseStreamHandler : ChatCompletionStreamHandler
    {
        public Action<ChatDelta[]> onDeltaChunk;
        private readonly List<StringBuilder> _sbList = new();
        private readonly List<ToolCall[]> _toolCalls = new();

        public MultiResponseStreamHandler() { }
        public MultiResponseStreamHandler(
            Action onStart = null,
            Action<ChatDelta[]> onDeltaChunk = null,
            Action<string> onError = null,
            Action<ChatCompletion> onDone = null)
        {
            if (onStart != null) this.onStart += onStart;
            if (onDeltaChunk != null) this.onDeltaChunk += onDeltaChunk;
            if (onError != null) this.onError += onError;
            if (onDone != null) this.onDone += onDone;
        }

        internal override void ProcessChunk(ChatCompletionChunk chunk)
        {
            ChatDelta[] deltaChunks = chunk.Value?.GetDeltaChunks();
            if (deltaChunks.IsNullOrEmpty()) return;

            onDeltaChunk?.Invoke(deltaChunks);

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

                if (delta.ToolCalls.IsNotNullOrEmpty())
                {
                    _toolCalls.Add(delta.ToolCalls);
                    _funtionManagerCallback?.Invoke(delta.ToolCalls);
                }
            }
        }

        internal override ChatCompletion BuildFinalResult()
        {
            List<string> streamedTexts = new();

            for (int i = 0; i < _sbList.Count; i++)
            {
                StringBuilder sb = _sbList[i];
                string content = sb.ToString();

                if (_task.textProcessor != null)
                    content = _task.textProcessor(content);

                streamedTexts.Add(content);
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