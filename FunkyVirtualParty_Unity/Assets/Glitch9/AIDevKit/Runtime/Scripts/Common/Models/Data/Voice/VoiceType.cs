using System;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    [JsonConverter(typeof(VoiceTypeConverter))]
    public enum VoiceType
    {
        None,
        [ApiEnum("Characters", "characters_animation")] Characters,
        [ApiEnum("Narration", "narration")] Narration,
        [ApiEnum("News", "news")] News,
        [ApiEnum("Social Media", "social_media")] SocialMedia,
    }

    public class VoiceTypeConverter : JsonConverter<VoiceType>
    {
        public static VoiceType Parse(string voiceTypeAsString)
        {
            string enumString = voiceTypeAsString.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(enumString)) return VoiceType.None;

            if (enumString.Contains("character")) return VoiceType.Characters;
            if (enumString.Contains("narrat")) return VoiceType.Narration;
            if (enumString.Contains("news")) return VoiceType.News;
            if (enumString.Contains("social")) return VoiceType.SocialMedia;
            if (enumString.Contains("educational")) return VoiceType.SocialMedia;
            if (enumString.Contains("informative")) return VoiceType.SocialMedia;
            if (enumString.Contains("conversational")) return VoiceType.SocialMedia;

            return VoiceType.None;
        }

        public override VoiceType ReadJson(JsonReader reader, Type objectType, VoiceType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || reader.Value == null) return VoiceType.None;

            string enumString = reader.Value.ToString().ToLowerInvariant();
            return Parse(enumString);
        }

        public override void WriteJson(JsonWriter writer, VoiceType value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToSnakeCase());
        }
    }
}