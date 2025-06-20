using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.ElevenLabs
{
    /// <summary>
    /// Represents a voice synthesis model with supported capabilities and metadata.
    /// </summary>
    public class ElevenLabsModelData : IModelData
    {
        /// <summary>
        /// The unique identifier of the model.
        /// </summary>
        [JsonProperty("model_id")] public string Id { get; set; }

        /// <summary>
        /// The name of the model.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Whether the model can be finetuned.
        /// </summary>
        [JsonProperty("can_be_finetuned")] public bool? IsTrainable { get; set; }

        /// <summary>
        /// Whether the model can do text-to-speech.
        /// </summary>
        [JsonProperty("can_do_text_to_speech")] public bool CanDoTextToSpeech { get; set; }

        /// <summary>
        /// Whether the model can do voice conversion.
        /// </summary>
        [JsonProperty("can_do_voice_conversion")] public bool CanDoVoiceConversion { get; set; }

        /// <summary>
        /// Whether the model can use style.
        /// </summary>
        [JsonProperty("can_use_style")] public bool CanUseStyle { get; set; }

        /// <summary>
        /// Whether the model can use speaker boost.
        /// </summary>
        [JsonProperty("can_use_speaker_boost")] public bool CanUseSpeakerBoost { get; set; }

        /// <summary>
        /// Whether the model serves pro voices.
        /// </summary>
        [JsonProperty("serves_pro_voices")] public bool ServesProVoices { get; set; }

        /// <summary>
        /// The cost factor for the model.
        /// </summary>
        [JsonProperty("token_cost_factor")] public double TokenCostFactor { get; set; }

        /// <summary>
        /// The description of the model.
        /// </summary>
        [JsonProperty("description")] public string Description { get; set; }

        /// <summary>
        /// Whether the model requires alpha access.
        /// </summary>
        [JsonProperty("requires_alpha_access")] public bool RequiresAlphaAccess { get; set; }

        /// <summary>
        /// The maximum number of characters that can be requested by a free user.
        /// </summary>
        [JsonProperty("max_characters_request_free_user")] public int MaxCharactersRequestFreeUser { get; set; }

        /// <summary>
        /// The maximum number of characters that can be requested by a subscribed user.
        /// </summary>
        [JsonProperty("max_characters_request_subscribed_user")] public int MaxCharactersRequestSubscribedUser { get; set; }

        /// <summary>
        /// The maximum length of text that can be requested for this model.
        /// </summary>
        [JsonProperty("maximum_text_length_per_request")] public int MaximumTextLengthPerRequest { get; set; }

        /// <summary>
        /// The languages supported by the model.
        /// </summary>
        [JsonProperty("languages")] public List<ModelLanguage> Languages { get; set; }

        /// <summary>
        /// The rates for the model.
        /// </summary>
        [JsonProperty("model_rates")] public ModelRates ModelRates { get; set; }

        /// <summary>
        /// The concurrency group for the model. Allowed values: 'standard', 'turbo'
        /// </summary>
        [JsonProperty("concurrency_group")] public string ConcurrencyGroup { get; set; }


        // --- IGenAIModel implementation ----------------------------------------------------------------
        [JsonIgnore] public Api Api => Api.ElevenLabs;
        [JsonIgnore] public string Provider => Api.ElevenLabs.ToString();
        [JsonIgnore] public string OwnedBy => "ElevenLabs";
        [JsonIgnore] public string BaseId => string.Empty;  // there is no base model for ElevenLabs models
        [JsonIgnore] public bool IsFineTuned => false;      // there is no fine-tuning for ElevenLabs models
        [JsonIgnore] public double? PricePerCharacter => ModelRates?.CharacterCostMultiplier; // the cost per character (TTS models)
    }

    /// <summary>
    /// Represents a language supported by a model.
    /// </summary>
    public class ModelLanguage
    {
        /// <summary>
        /// The unique identifier of the language.
        /// </summary>
        [JsonProperty("language_id")] public string LanguageId { get; set; }

        /// <summary>
        /// The name of the language.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }
    }

    /// <summary>
    /// Represents the cost multiplier for a model.
    /// </summary>
    public class ModelRates
    {
        /// <summary>
        /// The cost multiplier for characters.
        /// </summary>
        [JsonProperty("character_cost_multiplier")] public double CharacterCostMultiplier { get; set; }
    }
}
