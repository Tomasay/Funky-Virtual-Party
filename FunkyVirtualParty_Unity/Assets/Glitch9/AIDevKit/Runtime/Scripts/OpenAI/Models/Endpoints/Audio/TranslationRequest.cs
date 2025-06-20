using Glitch9.IO.Files;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Translates audio into English.
    /// </summary>
    public class TranslationRequest : AIRequest
    {
        /// <summary>
        /// Required. 
        /// The audio file object (not file name) translate,
        /// in one of these formats: flac, mp3, mp4, mpeg, mpga, m4a, ogg, wav, or webm.
        /// </summary>
        [JsonProperty("file")] public File<AudioClip> File { get; set; }

        /// <summary>
        /// An optional text to guide the model's style or continue a previous audio segment.
        /// The prompt should be in English.
        /// </summary>
        [JsonProperty("prompt")] public string Prompt { get; set; }

        /// <summary>
        /// The sampling temperature, between 0 and 1.
        /// Higher values like 0.8 will make the output more random,
        /// while lower values like 0.2 will make it more focused and deterministic.
        /// If set to 0, the model will use log probability to automatically increase the temperature until certain thresholds are hit.
        /// </summary>
        [JsonProperty("temperature")] public float? Temperature { get; set; }

        public class Builder : ModelRequestBuilder<Builder, TranslationRequest>
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
        }
    }
}