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

        protected override void OnReceiveChunk(ChatCompletionChunk chunk)
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

                _toolCalls.Add(delta.ToolCalls);
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