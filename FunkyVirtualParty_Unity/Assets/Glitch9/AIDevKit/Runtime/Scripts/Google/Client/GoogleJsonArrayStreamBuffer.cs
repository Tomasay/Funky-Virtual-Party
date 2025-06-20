using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.Google
{
    public class GoogleJsonArrayStreamBuffer<T>
    {
        private readonly StringBuilder buffer = new();
        private readonly JsonSerializerSettings settings;
        private bool _bufferStarted = false;

        public GoogleJsonArrayStreamBuffer(JsonSerializerSettings settings = null)
        {
            this.settings = settings ?? new JsonSerializerSettings();
        }

        public IEnumerable<(T, bool)> Append(string chunk)
        {
            string trimmedChunk = chunk.Trim().TrimStart(',').TrimEnd(',');
            if (string.IsNullOrWhiteSpace(trimmedChunk)) yield break; // 빈 문자열은 무시  

            // trimmedChunk가 }로 끝나지 않는다면, incomplete JSON이므로 버퍼에 추가
            if (!trimmedChunk.EndsWith('}'))
            {
                _bufferStarted = true;
                buffer.Append(trimmedChunk);
                yield break;
            }

            if (_bufferStarted)
            {
                // 버퍼에 저장된 내용이 있다면, 현재 chunk와 합쳐서 처리
                trimmedChunk = buffer.ToString() + trimmedChunk;
                buffer.Clear();
                _bufferStarted = false;
            }

            bool isDone = false;

            if (!trimmedChunk.StartsWith('['))
            {
                trimmedChunk = $"[{chunk}";
            }

            if (trimmedChunk.EndsWith(']'))
            {
                isDone = true;
            }
            else
            {
                trimmedChunk = $"{trimmedChunk}]";
            }

            if (string.IsNullOrWhiteSpace(trimmedChunk) || trimmedChunk == "[]") yield break;

            var list = JsonConvert.DeserializeObject<List<T>>(trimmedChunk, settings);

            if (list.IsNullOrEmpty())
            {
                Debug.LogError($"Failed to parse response: {trimmedChunk}");
                yield break;
            }

            // foreach (var item in list)
            // {
            //     yield return (item, isDone);
            // }

            for (int i = 0; i < list.Count; i++)
            {
                bool isLast = isDone && (i == list.Count - 1);
                yield return (list[i], isLast);
            }
        }
    }
}