using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.ElevenLabs
{
    /// <summary>
    /// Represents a verified language for a voice, including language, model, accent, and preview details.
    /// </summary>
    public class VoiceLanguage
    {
        /// <summary>
        /// The primary language code (e.g., \"en\", \"ko\").
        /// </summary>
        [JsonIgnore] public SystemLanguage Language { get; set; }

        [JsonProperty("language")]
        private string LanguageCode
        {
            set => Language = LocaleUtil.ParseISOCode(value);
        }

        /// <summary>
        /// The ID of the AI model associated with this language.
        /// </summary>
        [JsonProperty("model_id")] public string ModelId { get; set; }

        /// <summary>
        /// Optional. The accent of the voice (e.g., \"British\", \"American\").
        /// </summary>
        [JsonProperty("accent")] public string Accent { get; set; }

        /// <summary>
        /// Optional. The locale code of the voice (e.g., \"en-US\", \"ko-KR\").
        /// </summary>
        [JsonProperty("locale")] public string Locale { get; set; }

        /// <summary>
        /// Optional. A preview URL that demonstrates this voice in the specified language.
        /// </summary>
        [JsonProperty("preview_url")] public string PreviewUrl { get; set; }
    }
}