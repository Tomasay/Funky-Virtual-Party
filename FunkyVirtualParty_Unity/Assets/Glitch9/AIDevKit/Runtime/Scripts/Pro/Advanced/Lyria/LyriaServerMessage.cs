using System;
using System.Collections.Generic;
using System.Threading;
using Glitch9.IO.Networking.WebSocket;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Advanced.Lyria
{
    [Serializable]
    public class LyriaServerMessage : IWebSocketMessage
    {
        [JsonProperty("server_content")] public LyriaServerContent ServerContent { get; set; }
        [JsonIgnore] public CancellationTokenSource CancellationTokenSource { get; set; } = new();
        [JsonIgnore] public bool HasData => ServerContent != null && ServerContent.AudioChunks != null && ServerContent.AudioChunks.Count > 0;

        public List<string> GetAudioChunksBase64()
        {
            if (ServerContent == null || ServerContent.AudioChunks == null)
                return new List<string>();

            var audioChunks = new List<string>();
            foreach (var chunk in ServerContent.AudioChunks)
            {
                if (chunk != null && !string.IsNullOrEmpty(chunk.Base64PcmData))
                {
                    audioChunks.Add(chunk.Base64PcmData);
                }
            }
            return audioChunks;
        }

        [Serializable]
        public class LyriaServerContent
        {
            [JsonProperty("audio_chunks")] public List<LyriaAudioChunk> AudioChunks;
        }

        [Serializable]
        public class LyriaAudioChunk
        {
            [JsonProperty("data")] public string Base64PcmData;
        }
    }
}