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
        }
    }
}