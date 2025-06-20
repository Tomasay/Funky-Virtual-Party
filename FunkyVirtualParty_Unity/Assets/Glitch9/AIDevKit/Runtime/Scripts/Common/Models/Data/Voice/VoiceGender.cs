using System;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    public enum VoiceGender
    {
        None, // prolly an error 
        [ApiEnum("Male", "Male")] Male,
        [ApiEnum("Female", "Female")] Female,
        [ApiEnum("Non Binary", "NonBinary")] NonBinary,
    }

    public class VoiceGenderConverter : JsonConverter<VoiceGender>
    {
        public static VoiceGender Parse(string voiceGenderAsString)
        {
            string enumString = voiceGenderAsString.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(enumString)) return VoiceGender.None;

            if (enumString.Contains("female")) return VoiceGender.Female; // 'female' contains 'male' becareful with this one
            if (enumString.Contains("male")) return VoiceGender.Male;

            return VoiceGender.NonBinary;
        }

        public override VoiceGender ReadJson(JsonReader reader, Type objectType, VoiceGender existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || reader.Value == null) return VoiceGender.None;

            string enumString = reader.Value.ToString().ToLowerInvariant();
            return Parse(enumString);
        }

        public override void WriteJson(JsonWriter writer, VoiceGender value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToApiValue());
        }
    }
}