using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class TempVoice
    {
        internal static TempVoiceData OpenAI(string id, string name, VoiceGender gender, VoiceAge age, VoiceType type)
        {
            return new TempVoiceData
            {
                Id = id,
                Name = name,
                Gender = gender,
                Age = age,
                Category = VoiceCategory.Premade,
                Type = type,
                Language = SystemLanguage.English,
                Accent = "american",
                Api = Api.OpenAI,
                OwnedBy = Api.OpenAI.ToString(),
            };
        }

        internal static TempVoiceData Google(string id, string name, VoiceGender gender, VoiceAge age)
        {
            return new TempVoiceData
            {
                Id = id,
                Name = name,
                Gender = gender,
                Age = age,
                Category = VoiceCategory.Premade,
                Type = VoiceType.None,
                Language = SystemLanguage.English,
                Accent = "american",
                Api = Api.Google,
                OwnedBy = Api.Google.ToString(),
            };
        }
    }

    internal class TempVoiceData : IVoiceData
    {
        public Api Api { get; set; } //=> Api.OpenAI;
        public string OwnedBy { get; set; } //=> Api.OpenAI.ToString();
        public string PreviewPath => AIDevKitEditorPath.GetVoiceSampleFullPath(Api, Id);
        public bool IsCustom => false;
        public bool? IsFree => true;

        public string Id { get; set; }
        public string Name { get; set; }
        public UnixTime? CreatedAt { get; set; }
        public string Description { get; set; }
        public VoiceCategory? Category { get; set; }
        public VoiceGender? Gender { get; set; }
        public VoiceType? Type { get; set; }
        public VoiceAge? Age { get; set; }
        public SystemLanguage Language { get; set; }
        public string Accent { get; set; }
    }
}