using System;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    public class RealtimeItemBuffer<T>
    {
        private readonly SortedDictionary<(string itemId, int outputIndex, int contentIndex), T> _deltaBuffer = new();

        private string _currentItemId = null;
        private int _currentItemIndex = 0;
        private int _currentContentIndex = 0;
        private int _tempContentIndex = 0;
        private Action<T> _onProcessBufferDelta;
        private Action<T> _onCompleteBufferDelta;
        private DefaultLogger _logger;

        /* 
            How to use:
            - content index is just for the streamed response (delta)
            - disregard the 'event id' and 'response id', i'm not exactly sure what they are sent for, but it is just a uuid and each response has a different one

            Item hierarchy: Event => Item => Output => Content(delta)
        */

        // Constructor to initialize the buffer
        public RealtimeItemBuffer(
            string bufferName,
            Action<T> onProcessBuffer,
            Action<T> onCompleteBuffer)
        {
            if (!bufferName.EndsWith("Buffer")) bufferName += " Buffer";
            _logger = new DefaultLogger(bufferName);
            _onProcessBufferDelta = onProcessBuffer;
            _onCompleteBufferDelta = onCompleteBuffer;
        }

        public void AppendDeltaWithTempIndex(string itemId, int? itemIndex, T content)
        {
            if (TryProcessBufferDelta(itemId, itemIndex, _tempContentIndex, content))
            {
                _tempContentIndex++;
                return;
            }
            _onProcessBufferDelta?.Invoke(content);
        }

        public void AppendDelta(string itemId, int? itemIndex, int? contentIndex, T content)
        {
            if (TryProcessBufferDelta(itemId, itemIndex, contentIndex, content)) return;
            _onProcessBufferDelta?.Invoke(content);
        }

        public void CompleteDelta(string itemId, int? itemIndex, int? contentIndex, T content)
        {
            // 처리 중인 델타 아이템 완료
            //HandleBufferDelta();

            // 완료 처리
            _onCompleteBufferDelta?.Invoke(content);

            using (StringBuilderPool.Get(out var sb))
            {
                sb.AppendLine("Completing buffer delta: ");
                sb.AppendLine("Item ID: " + itemId);
                sb.AppendLine("Item Index: " + itemIndex);
                sb.AppendLine("Content Index: " + contentIndex);
                _logger.Info(sb.ToString());
            }

            _currentItemId = null;
            _currentItemIndex = 0;
            _currentContentIndex = 0;
            _tempContentIndex = 0;
        }


        private bool TryProcessBufferDelta(string itemId, int? itemIndex, int? contentIndex, T content)
        {
            if (!string.IsNullOrEmpty(itemId) && itemIndex.HasValue && contentIndex.HasValue)
            {
                _currentItemId ??= itemId;

                // itemIndex와 contentIndex가 둘다 0이면 그냥 콜백을 호출한다.
                if (itemIndex == 0 && contentIndex == 0)
                {
                    _onProcessBufferDelta?.Invoke(content);
                    return true;
                }

                var key = (itemId, itemIndex.Value, contentIndex.Value);
                _deltaBuffer[key] = content;

                using (StringBuilderPool.Get(out var sb))
                {
                    sb.AppendLine("Adding buffer delta: ");
                    sb.AppendLine("Item ID: " + itemId);
                    sb.AppendLine("Item ID: " + _currentItemId + "(Current)");
                    sb.AppendLine("Item Index: " + itemIndex);
                    sb.AppendLine("Item Index: " + _currentItemIndex + "(Current)");
                    sb.AppendLine("Content Index: " + contentIndex);
                    sb.AppendLine("Content Index: " + _currentContentIndex + "(Current)");
                    _logger.Info(sb.ToString());
                }

                HandleBufferDelta();
                return true;
            }
            else
            {
                if (string.IsNullOrEmpty(itemId))
                {
                    UnityEngine.Debug.LogError("Item ID is null or empty");
                }
                else if (!itemIndex.HasValue)
                {
                    UnityEngine.Debug.LogError("Item index is null");
                }
                else if (!contentIndex.HasValue)
                {
                    UnityEngine.Debug.LogError("Content index is null");
                }
                return false;
            }
        }

        private void HandleBufferDelta()
        {
            while (true)
            {
                var key = (_currentItemId, _currentItemIndex, _currentContentIndex);
                if (_deltaBuffer.TryGetValue(key, out var content))
                {
                    _onProcessBufferDelta?.Invoke(content);
                    _deltaBuffer.Remove(key);
                    _currentContentIndex++;
                }
                else
                {
                    _logger.Warning($"Buffer delta not found: {key}");
                    break;
                }
            }
        }
    }
}
