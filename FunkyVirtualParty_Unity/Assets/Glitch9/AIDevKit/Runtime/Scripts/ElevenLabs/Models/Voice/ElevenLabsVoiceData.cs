using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class ElevenLabsVoiceData : IVoiceData
    {
        /// <summary>
        /// Required. The ID of the voice.
        /// </summary>
        [JsonProperty("voice_id")] public string Id { get; set; }

        /// <summary>
        /// Required. The name of the voice.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Required. The category of the voice.
        /// </summary>
        [JsonProperty("category")] public VoiceCategory? Category { get; set; }

        /// <summary>
        /// Optional. Labels associated with the voice.
        /// </summary>
        [JsonProperty("labels")] public Dictionary<string, string> Labels { get; set; }

        /// <summary>
        /// Optional. The tiers the voice is available for.
        /// </summary>
        [JsonProperty("available_for_tiers")] public List<string> AvailableForTiers { get; set; }

        /// <summary>
        /// Optional. The base model IDs for high-quality voices.
        /// </summary>
        [JsonProperty("high_quality_base_model_ids")] public List<string> HighQualityBaseModelIds { get; set; }

        /// <summary>
        /// Optional. List of samples associated with the voice.
        /// </summary>
        [JsonProperty("samples")] public List<VoiceSample> Samples { get; set; }

        /// <summary>
        /// Optional. Fine-tuning information for the voice.
        /// </summary>
        [JsonProperty("fine_tuning")] public FineTuning FineTuning { get; set; }

        /// <summary>
        /// Optional. The description of the voice.
        /// </summary>
        [JsonProperty("description")] public string Description { get; set; }

        /// <summary>
        /// Optional. The preview URL of the voice.
        /// </summary>
        [JsonProperty("preview_url")] public string PreviewUrl { get; set; }

        /// <summary>
        /// Optional. The settings of the voice.
        /// </summary> 
        [JsonProperty("settings")] public VoiceSettings Settings { get; set; }

        /// <summary>
        /// Optional. The sharing information of the voice.
        /// </summary>
        [JsonProperty("sharing")] public VoiceSharing Sharing { get; set; }

        /// <summary>
        /// Optional. The verified languages of the voice.
        /// </summary>
        [JsonProperty("verified_languages")] public List<VoiceLanguage> VerifiedLanguages { get; set; }

        /// <summary>
        /// Optional. The safety controls of the voice.
        /// </summary>
        [JsonProperty("safety_control")] public string SafetyControl { get; set; }

        /// <summary>
        /// Optional. The voice verification of the voice.
        /// </summary>
        [JsonProperty("voice_verification")] public VoiceVerification VoiceVerification { get; set; }

        /// <summary>
        /// Optional. The permission on the resource of the voice.
        /// </summary>
        [JsonProperty("permission_on_resource")] public string PermissionOnResource { get; set; }

        /// <summary>
        /// Optional. Whether the voice is owned by the user. 
        /// </summary>
        [JsonProperty("is_owner")] public bool? IsOwner { get; set; }

        /// <summary>
        /// Optional. Whether the voice is legacy.
        /// Defaults to false
        /// </summary>
        [JsonProperty("is_legacy")] public bool? IsLegacy { get; set; }

        /// <summary>
        /// Optional. Whether the voice is mixed.
        /// Defaults to false
        /// </summary>
        [JsonProperty("is_mixed")] public bool? IsMixed { get; set; }

        /// <summary>
        /// Optional. The creation time of the voice in Unix time.
        /// </summary>
        [JsonProperty("created_at_unix")] public UnixTime? CreatedAt { get; set; }


        // IVoiceData implementation
        [JsonIgnore] public Api Api => Api.ElevenLabs;
        [JsonIgnore] public string OwnedBy => VoiceCache.GetOwnedBy(this);
        [JsonIgnore] public VoiceGender? Gender => VoiceCache.GenGender(this);
        [JsonIgnore] public VoiceType? Type => VoiceCache.GetType(this);
        [JsonIgnore] public VoiceAge? Age => VoiceCache.GetAge(this);
        [JsonIgnore] public SystemLanguage Language => VoiceCache.GetLanguage(this);
        [JsonIgnore] public string Accent => VoiceCache.GetAccent(this);
        [JsonIgnore] public bool IsCustom => false;
        [JsonIgnore] public bool? IsFeatured => false;
        [JsonIgnore] public bool? IsFree => true;
    }
}