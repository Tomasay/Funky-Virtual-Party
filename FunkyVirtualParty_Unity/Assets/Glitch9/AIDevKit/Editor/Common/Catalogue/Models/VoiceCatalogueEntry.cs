using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    [JsonObject]
    public class VoiceCatalogueEntry : IVoiceData, ICatalogueEntry
    {
        // --- Core Properties ---
        [JsonProperty] public Api Api { get; internal set; }
        [JsonProperty] public string Id { get; internal set; }
        [JsonProperty] public string Name { get; internal set; }
        [JsonProperty] public string Description { get; internal set; }
        [JsonProperty] public string OwnedBy { get; internal set; }
        [JsonProperty] public UnixTime? CreatedAt { get; internal set; }
        [JsonProperty] public bool IsCustom { get; internal set; }
        [JsonProperty] public bool? IsFeatured { get; internal set; }
        [JsonProperty] public bool? IsFree { get; internal set; }
        [JsonProperty] public string PreviewUrl { get; internal set; }
        [JsonProperty] public string PreviewPath { get; internal set; }
        [JsonProperty] public string ImageUrl { get; internal set; }
        [JsonProperty] public string Accent { get; internal set; }
        [JsonProperty] public VoiceGender? Gender { get; internal set; }
        [JsonProperty] public VoiceType? Type { get; internal set; }
        [JsonProperty] public VoiceAge? Age { get; internal set; }
        [JsonProperty] public SystemLanguage Language { get; internal set; }
        [JsonProperty] public VoiceCategory? Category { get; set; }
        [JsonProperty] public string Descriptive { get; set; }
        [JsonProperty] public string Locale { get; set; }
        [JsonProperty] public List<string> AvailableForTiers { get; set; }
        [JsonIgnore]
        public bool IsDeprecated
        {
            get => _isDeprecated && Api != Api.Ollama;
            set => _isDeprecated = value;
        }
        [JsonProperty("IsDeprecated")] private bool _isDeprecated;
        [JsonConstructor] public VoiceCatalogueEntry() { }

        internal static VoiceCatalogueEntry Create(IVoiceData voiceData)
        {
            if (voiceData == null)
                throw new ArgumentNullException(nameof(voiceData), "Voice data cannot be null.");

            VoiceCatalogueEntry entry = new(voiceData);
            entry.Description = TextSplitter.SplitToParagraphs(entry.Description);

            return entry;
        }

        internal void Update(IVoiceData voiceData)
        {
            if (voiceData == null) throw new ArgumentNullException(nameof(voiceData), "Voice data cannot be null.");

            Api = voiceData.Api;
            Id = voiceData.Id;
            Name = voiceData.Name?.Trim();
            Description = voiceData.Description;
            OwnedBy = voiceData.OwnedBy;
            CreatedAt = voiceData.CreatedAt;
            IsCustom = voiceData.IsCustom;
            PreviewUrl = voiceData.PreviewUrl;
            PreviewPath = voiceData.PreviewPath;
            Accent = voiceData.Accent;
            Gender = voiceData.Gender;
            Type = voiceData.Type;
            Age = voiceData.Age;
            Language = voiceData.Language;
            IsFeatured = voiceData.IsFeatured;
            IsFree = voiceData.IsFree;
            Category = voiceData.Category;
            ImageUrl = voiceData.ImageUrl;
            Descriptive = voiceData.Descriptive;
            Locale = voiceData.Locale;
            AvailableForTiers = voiceData.AvailableForTiers;
        }

        private VoiceCatalogueEntry(IVoiceData voiceData) => Update(voiceData);

        public bool Equals(VoiceCatalogueEntry other) => other != null && Id == other.Id;
    }
}