using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Glitch9.ScriptableObjects;

namespace Glitch9.AIDevKit
{
    [JsonConverter(typeof(VoiceConverter))]
    public class Voice : AIDevKitAsset, IData
    {
        [SerializeField] private SystemLanguage language;
        [SerializeField] private VoiceGender gender;
        [SerializeField] private VoiceAge age;

        public override string Name => GetOrParseName();
        private string _name;

        private string GetOrParseName()
        {
            if (!string.IsNullOrEmpty(_name)) return _name;

            string nameName = displayName ?? id;
            if (string.IsNullOrEmpty(nameName)) nameName = "Unknown";
            string genderName = null;
            string ageName = null;
            string languageName = null;

            if (gender != VoiceGender.None) genderName = gender.GetInspectorName();
            if (age != VoiceAge.None) ageName = age.GetInspectorName();
            if (language != SystemLanguage.Unknown || language != SystemLanguage.Afrikaans) languageName = language.ToString();

            using (StringBuilderPool.Get(out var sb))
            {
                sb.Append(nameName);

                bool parenthesisStarted = false;

                if (genderName != null)
                {
                    sb.Append(" (");
                    sb.Append(genderName);
                    parenthesisStarted = true;
                }

                if (ageName != null)
                {
                    if (parenthesisStarted) sb.Append(", ");
                    else sb.Append(" (");
                    sb.Append(ageName);
                    parenthesisStarted = true;
                }

                if (languageName != null)
                {
                    if (parenthesisStarted) sb.Append(", ");
                    else sb.Append(" (");
                    sb.Append(languageName);
                    parenthesisStarted = true;
                }

                if (parenthesisStarted) sb.Append(")");
                _name = sb.ToString();
            }

            // AIDevKitDebug.Mark($"Parsed name: {_name}");

            return _name;
        }

        #region Updater

        /// <summary>
        /// Updates the core metadata of the model. Also initializes default prices if none are set.
        /// </summary>
        internal void UpdateData(
            string id = null,
            string name = null,
            Api? api = null,
            bool? custom = null,
            SystemLanguage? language = null,
            VoiceGender? gender = null,
            VoiceAge? age = null)
        {
            if (!string.IsNullOrEmpty(id)) this.id = id;
            if (!string.IsNullOrEmpty(name)) displayName = name;
            if (api != null) this.api = api.Value;
            if (custom != null) this.custom = custom.Value;
            if (language != null) this.language = language.Value;
            if (gender != null) this.gender = gender.Value;
            if (age != null) this.age = age.Value;

            this.SaveAsset();
        }

        #endregion

        #region Caching

        public static implicit operator Voice(string apiName) => Cache.Get(apiName);
        internal static class Cache
        {
            internal static readonly Dictionary<string, Voice> _cache = new();
            private static readonly HashSet<string> _tried = new();

            internal static Voice Get(string id)
            {
                if (string.IsNullOrEmpty(id)) return null;

                if (_cache.TryGetValue(id, out Voice voice)) return voice;

                if (VoiceLibrary.TryGetValue(id, out voice))
                {
                    _cache.AddOrUpdate(id, voice);
                    return voice;
                }

                if (_tried.Contains(id)) return null;
                _tried.Add(id);

                Debug.Log($"The {typeof(Voice).Name} {id} is not found in the '{typeof(VoiceLibrary).Name}.asset'.");

                return null;
            }
        }

        #endregion Caching
    }

    internal class VoiceConverter : JsonConverter<Voice>
    {
        public override void WriteJson(JsonWriter writer, Voice value, JsonSerializer serializer)
            => writer.WriteValue(value.Id);
        public override Voice ReadJson(JsonReader reader, Type objectType, Voice existingValue, bool hasExistingValue, JsonSerializer serializer)
            => reader.Value as string;
    }
}