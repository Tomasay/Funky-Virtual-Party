using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class SpeechRequest : VoiceRequest
    {
        // Body Parameters ----------------------------------------------------------------

        /// <summary>
        /// Required. 
        /// The text that will get converted into speech.
        /// </summary>
        [JsonProperty("text")] public string Text { get; set; }

        /// <summary>
        /// Optional. 
        /// Language code (ISO 639-1) used to enforce a language for the model. 
        /// Currently only Turbo v2.5 and Flash v2.5 support language enforcement. 
        /// For other models, an error will be returned if language code is provided.
        /// </summary>
        [JsonProperty("language_code")] public string LanguageCode { get; set; }

        /// <summary>
        /// Optional. 
        /// Voice settings overriding stored settings for the given voice. 
        /// They are applied only on the given request.
        /// </summary>
        [JsonProperty("voice_settings")] public VoiceSettings VoiceSettings { get; set; }

        /// <summary>
        /// Optional. 
        /// A list of pronunciation dictionary locators (id, version_id) to be applied to the text. 
        /// They will be applied in order. 
        /// You may have up to 3 locators per request.
        /// </summary>
        [JsonProperty("pronunciation_dictionary_locators")] public List<PronunciationDictionaryLocator> PronunciationDictionaryLocators { get; set; }

        /// <summary>
        /// Optional. 
        /// If specified, our system will make a best effort to sample deterministically, 
        /// such that repeated requests with the same seed and parameters should return the same result. 
        /// Determinism is not guaranteed. Must be integer between 0 and 4294967295.
        /// </summary>
        [JsonProperty("seed")] public uint? Seed { get; set; }

        /// <summary>
        /// Optional. 
        /// The text that came before the text of the current request. 
        /// Can be used to improve the speech’s continuity when concatenating together multiple generations or to influence the speech’s continuity in the current generation.
        /// </summary>
        [JsonProperty("previous_text")] public string PreviousText { get; set; }

        /// <summary>
        /// Optional. The text that comes after the text of the current request. 
        /// Can be used to improve the speech’s continuity when concatenating together multiple generations or to influence the speech’s continuity in the current generation.
        /// </summary>
        [JsonProperty("next_text")] public string NextText { get; set; }

        /// <summary>
        /// Optional. A list of request_id of the samples that were generated before this generation. 
        /// Can be used to improve the speech’s continuity when splitting up a large task into multiple requests. 
        /// The results will be best when the same model is used across the generations. 
        /// In case both previous_text and previous_request_ids is send, previous_text will be ignored. 
        /// A maximum of 3 request_ids can be send.
        /// </summary>
        [JsonProperty("previous_request_ids")] public List<string> PreviousRequestIds { get; set; }

        /// <summary>
        /// Optional. A list of request_id of the samples that come after this generation. 
        /// next_request_ids is especially useful for maintaining the speech’s continuity when regenerating a sample that has had some audio quality issues. 
        /// For example, if you have generated 3 speech clips, and you want to improve clip 2, 
        /// passing the request id of clip 3 as a next_request_id (and that of clip 1 as a previous_request_id) will help maintain natural flow in the combined speech. 
        /// The results will be best when the same model is used across the generations. In case both next_text and next_request_ids is send, next_text will be ignored. 
        /// A maximum of 3 request_ids can be send.
        /// </summary>
        [JsonProperty("next_request_ids")] public List<string> NextRequestIds { get; set; }

        /// <summary>
        /// Optional. This parameter controls text normalization with three modes: ‘auto’, ‘on’, and ‘off’. 
        /// When set to ‘auto’, the system will automatically decide whether to apply text normalization (e.g., spelling out numbers). 
        /// With ‘on’, text normalization will always be applied, while with ‘off’, it will be skipped. 
        /// Cannot be turned on for ‘eleven_turbo_v2_5’ or ‘eleven_flash_v2_5’ models.
        /// Allowed values: auto, on, off.
        /// Defaults to 'auto'.
        /// </summary>
        [JsonProperty("apply_text_normalization")] public string ApplyTextNormalization { get; set; }

        /// <summary>
        /// Optional. This parameter controls language text normalization. 
        /// This helps with proper pronunciation of text in some supported languages. 
        /// WARNING: This parameter can heavily increase the latency of the request. Currently only supported for Japanese.
        /// Defaults to false.
        /// </summary>
        [JsonProperty("apply_language_text_normalization")] public bool? ApplyLanguageTextNormalization { get; set; }
    }

    public class PronunciationDictionaryLocator
    {
        /// <summary>
        /// Required.
        /// </summary>
        [JsonProperty("pronunciation_dictionary_id")] public string PronunciationDictionaryId { get; set; }
        [JsonProperty("version_id")] public string VersionId { get; set; }
    }
}