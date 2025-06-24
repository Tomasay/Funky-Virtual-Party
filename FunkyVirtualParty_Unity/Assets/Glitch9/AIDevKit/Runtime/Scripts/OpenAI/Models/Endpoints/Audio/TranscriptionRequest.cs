using System.Collections.Generic;
using Glitch9.IO.Files;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Transcribes audio into the input language.
    /// </summary>
    /// <remarks>
    /// returns the transcription object or a verbose transcription object.
    /// </remarks>
    public class TranscriptionRequest : AIRequest
    {
        public class ChunkingStrategyWrapper
        {
            public class ServerVad
            {
                /// <summary>
                /// Required. Must be set to server_vad to enable manual chunking using server side VAD.
                /// </summary>
                [JsonProperty("type")] public string Type { get; set; }

                /// <summary>
                /// Optional. Defaults to 300
                /// Amount of audio to include before the VAD detected speech (in milliseconds).
                /// </summary>
                [JsonProperty("prefix_padding_ms")] public int? PrefixPaddingMs { get; set; }

                /// <summary>
                /// Optional. Defaults to 200
                /// Duration of silence to detect speech stop (in milliseconds). With shorter values the model will respond 
                /// more quickly, but may jump in on short pauses from the user.
                /// </summary>
                [JsonProperty("silence_duration_ms")] public int? SilenceDurationMs { get; set; }

                /// <summary>
                /// Optional. Defaults to 0.5
                /// Sensitivity threshold (0.0 to 1.0) for voice activity detection. A higher threshold will require louder 
                /// audio to activate the model, and thus might perform better in noisy environments.
                /// </summary>
                [JsonProperty("threshold")] public float? Threshold { get; set; }
            }

            /// <summary>
            /// Automatically set chunking parameters based on the audio. Must be set to "auto".
            /// </summary>
            [JsonProperty("chunking_strategy")] public string ChunkingStrategyType { get; set; } = "auto";

            /// <summary>
            /// Optional. If chunking_strategy is set to server_vad, 
            /// this object can be provided to tweak VAD detection parameters manually.
            /// </summary>
            [JsonProperty("server_vad")] public ServerVad ServerVadConfig { get; set; }
        }


        /// <summary>
        /// Required. 
        /// The audio file object (not file name) to transcribe,
        /// in one of these formats: flac, mp3, mp4, mpeg, mpga, m4a, ogg, wav, or webm.
        /// </summary>
        [JsonProperty("file")] public File<AudioClip> File { get; set; }

        /// <summary>
        /// The language of the input audio.
        /// Supplying the input language in ISO-639-1 format will improve accuracy and latency.
        /// </summary>
        [JsonProperty("language")] public SystemLanguage? Language { get; set; }

        /// <summary>
        /// An optional text to guide the model's style or continue a previous audio segment.
        /// The prompt should match the audio language.
        /// </summary>
        [JsonProperty("prompt")] public string Prompt { get; set; }

        /// <summary>
        /// The sampling temperature, between 0 and 1.
        /// Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.
        /// If set to 0, the model will use log probability to automatically increase the temperature until certain thresholds are hit.
        /// </summary>
        [JsonProperty("temperature")] public float? Temperature { get; set; }

        /// <summary>
        /// The timestamp granularities to populate for this transcription.
        /// response_format must be set verbose_json to use timestamp granularities.
        /// Either or both of these options are supported: word, or segment.
        /// </summary>
        /// <remarks>
        /// Note: There is no additional latency for segment timestamps, but generating word timestamps incurs additional latency.
        /// </remarks>
        [JsonProperty("timestamp_granularities")] public string[] TimestampGranularities { get; set; }

        /// <summary>
        /// Optional.
        /// Controls how the audio is cut into chunks. 
        /// When set to "auto", the server first normalizes loudness and then uses voice activity detection (VAD) to choose boundaries. 
        /// server_vad object can be provided to tweak VAD detection parameters manually.
        /// If unset, the audio is transcribed as a single block.
        /// </summary>
        [JsonProperty("chunking_strategy")] public ChunkingStrategyWrapper ChunkingStrategy { get; set; }

        /// <summary>
        /// Optional. Additional information to include in the transcription response. logprobs will return the log 
        /// probabilities of the tokens in the response to understand the model's confidence in the transcription. 
        /// logprobs only works with response_format set to json and only with the models gpt-4o-transcribe 
        /// and gpt-4o-mini-transcribe.
        /// </summary>
        [JsonProperty("include")] public List<string> Include { get; set; }

        /// <summary>
        /// Optional. Defaults to false
        /// If set to true, the model response data will be streamed to the client as it is generated using 
        /// server-sent events. See the Streaming section of the Speech-to-Text guide for more information.
        /// 
        /// Note: Streaming is not supported for the whisper-1 model and will be ignored.
        /// </summary>
        [JsonProperty("stream")] public bool? Stream { get; set; }

        public class Builder : ModelRequestBuilder<Builder, TranscriptionRequest>
        {
            public Builder SetFile(File<AudioClip> file)
            {
                _req.File = file;
                return this;
            }

            public Builder SetFile(AudioClip file)
            {
                _req.File = new(file, $"@{file.name}.mp3");
                return this;
            }

            public Builder SetLanguage(SystemLanguage language)
            {
                _req.Language = language;
                return this;
            }

            public Builder SetPrompt(string prompt)
            {
                _req.Prompt = prompt;
                return this;
            }

            public Builder SetTemperature(float temperature)
            {
                _req.Temperature = temperature;
                return this;
            }

            public Builder SetTimestampGranularities(string[] timestampGranularities)
            {
                _req.TimestampGranularities = timestampGranularities;
                return this;
            }

            public Builder SetChunkingStrategy(ChunkingStrategyWrapper chunkingStrategy)
            {
                _req.ChunkingStrategy = chunkingStrategy;
                return this;
            }

            public Builder SetInclude(List<string> include)
            {
                _req.Include = include;
                return this;
            }

            public Builder SetStream(bool stream)
            {
                _req.Stream = stream;
                return this;
            }
        }
    }
}