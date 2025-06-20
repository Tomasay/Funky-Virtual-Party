using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit.ElevenLabs
{
    internal static class VoiceCache
    {
        private static readonly Dictionary<string, string> _ownedByCache = new();
        private static readonly Dictionary<string, VoiceGender> _genderCache = new();
        private static readonly Dictionary<string, VoiceType> _typeCache = new();
        private static readonly Dictionary<string, VoiceAge> _ageCache = new();
        private static readonly Dictionary<string, SystemLanguage> _languageCache = new();

        internal static string GetOwnedBy(ElevenLabsVoiceData voice)
        {
            if (_ownedByCache.TryGetValue(voice.Id, out string cachedOwner))
            {
                return cachedOwner;
            }

            string ownedBy;

            if (voice.IsOwner == true)
            {
                ownedBy = "you";
            }
            else if (voice.Sharing != null && voice.Sharing.PublicOwnerId != null)
            {
                ownedBy = voice.Sharing.PublicOwnerId;
            }
            else
            {
                ownedBy = "ElevenLabs";
            }

            _ownedByCache[voice.Id] = ownedBy;
            return ownedBy;
        }

        internal static VoiceGender GenGender(ElevenLabsVoiceData voice)
        {
            if (_genderCache.TryGetValue(voice.Id, out VoiceGender cachedGender))
            {
                return cachedGender;
            }

            const string kLabelKey = "gender";
            VoiceGender gender = VoiceGender.None;

            if (!voice.Labels.IsNullOrEmpty())
            {
                string genderAsString = voice.Labels.GetValueOrDefault(kLabelKey, string.Empty).ToLowerInvariant();

                // parse enum
                if (genderAsString == "male")
                {
                    gender = VoiceGender.Male;
                }
                else if (genderAsString == "female")
                {
                    gender = VoiceGender.Female;
                }
                else if (genderAsString == "non binary")
                {
                    gender = VoiceGender.NonBinary;
                }
            }

            _genderCache[voice.Id] = gender;
            return gender;
        }

        internal static VoiceType GetType(ElevenLabsVoiceData voice)
        {
            if (_typeCache.TryGetValue(voice.Id, out VoiceType cachedType))
            {
                return cachedType;
            }

            const string kLabelKey = "use_case";
            VoiceType type = VoiceType.None;

            if (!voice.Labels.IsNullOrEmpty())
            {
                string typeAsString = voice.Labels.GetValueOrDefault(kLabelKey, string.Empty).ToLowerInvariant();

                // parse enum
                if (typeAsString == "characters")
                {
                    type = VoiceType.Characters;
                }
                else if (typeAsString == "narration")
                {
                    type = VoiceType.Narration;
                }
                else if (typeAsString == "news")
                {
                    type = VoiceType.News;
                }
                else if (typeAsString == "social media")
                {
                    type = VoiceType.SocialMedia;
                }
            }

            _typeCache[voice.Id] = type;
            return type;
        }

        internal static VoiceAge GetAge(ElevenLabsVoiceData voice)
        {
            if (_ageCache.TryGetValue(voice.Id, out VoiceAge cachedAge))
            {
                return cachedAge;
            }

            const string kLabelKey = "age";
            VoiceAge age = VoiceAge.None;

            if (!voice.Labels.IsNullOrEmpty())
            {
                string ageAsString = voice.Labels.GetValueOrDefault(kLabelKey, string.Empty).ToLowerInvariant();

                // parse enum
                if (ageAsString == "child")
                {
                    age = VoiceAge.Child;
                }
                else if (ageAsString == "young")
                {
                    age = VoiceAge.Young;
                }
                else if (ageAsString == "middle_aged")
                {
                    age = VoiceAge.MiddleAged;
                }
                else if (ageAsString == "old")
                {
                    age = VoiceAge.Senior;
                }
                else
                {
                    Debug.LogWarning($"Voice {voice.Id} has an unknown age label: {ageAsString}. Defaulting to Unknown.");
                }
            }
            else
            {
                Debug.LogWarning($"Voice {voice.Id} has no age label. Defaulting to Unknown.");
            }

            _ageCache[voice.Id] = age;
            return age;
        }

        internal static SystemLanguage GetLanguage(ElevenLabsVoiceData voice)
        {
            if (_languageCache.TryGetValue(voice.Id, out SystemLanguage cachedLanguage))
            {
                return cachedLanguage;
            }

            SystemLanguage language = SystemLanguage.English; // default to English

            if (voice.VoiceVerification != null && !string.IsNullOrWhiteSpace(voice.VoiceVerification.Language))
            {
                string languageCode = voice.VoiceVerification.Language;
                language = LocaleUtil.ParseISOCode(languageCode);
            }

            _languageCache[voice.Id] = language;
            return language;
        }

        internal static string GetAccent(ElevenLabsVoiceData voice)
        {
            const string kLabelKey = "accent";

            if (!voice.Labels.IsNullOrEmpty())
            {
                return voice.Labels.GetValueOrDefault(kLabelKey, string.Empty).ToLowerInvariant();
            }


            return null;
        }
    }
}