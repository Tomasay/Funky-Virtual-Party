using System;
using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Glitch9.AIDevKit.ElevenLabs
{
    [JsonConverter(typeof(SharedVoiceConverter))]
    public class ElevenLabsSharedVoiceData : IVoiceData
    {
        /// <summary>
        /// Required. Public owner ID of the voice.
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Required. Voice ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Required. The date the voice was added, in Unix time.
        /// </summary>
        public UnixTime? CreatedAt { get; set; }

        /// <summary>
        /// Required. The name of the voice.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Required. Accent of the voice.
        /// </summary>
        public string Accent { get; set; }

        /// <summary>
        /// Required. Gender of the voice.
        /// </summary>
        public VoiceGender? Gender { get; set; }

        /// <summary>
        /// Required. Age associated with the voice.
        /// </summary>
        public VoiceAge? Age { get; set; }

        /// <summary>
        /// Required. Descriptive label for the voice.
        /// </summary>
        public string Descriptive { get; set; }

        /// <summary>
        /// Required. Use case for the voice.
        /// </summary>
        public VoiceType? Type { get; set; }

        /// <summary>
        /// Required. Category of the voice.
        /// </summary>
        public VoiceCategory? Category { get; set; }

        /// <summary>
        /// Required. Character usage count over the past year.
        /// </summary>
        public long UsageCharacterCount1Y { get; set; }

        /// <summary>
        /// Required. Character usage count over the past 7 days.
        /// </summary>
        public long UsageCharacterCount7D { get; set; }

        /// <summary>
        /// Required. Play API usage character count over the past year.
        /// </summary>
        public long PlayApiUsageCharacterCount1Y { get; set; }

        /// <summary>
        /// Required. Number of users who cloned this voice.
        /// </summary>
        public long ClonedByCount { get; set; }

        /// <summary>
        /// Required. Whether free users are allowed to use this voice.
        /// </summary>
        public bool FreeUsersAllowed { get; set; }

        /// <summary>
        /// Required. Whether live moderation is enabled for this voice.
        /// </summary>
        public bool LiveModerationEnabled { get; set; }

        /// <summary>
        /// Required. Whether the voice is featured.
        /// </summary>
        public bool Featured { get; set; }

        /// <summary>
        /// Required. The language of the voice.
        /// </summary>
        public SystemLanguage Language { get; set; } = SystemLanguage.English;

        /// <summary>
        /// Required. The locale of the voice.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Required. Description of the voice.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Required. URL to a preview audio sample of the voice.
        /// </summary>
        public string PreviewUrl { get; set; }

        /// <summary>
        /// Required. Playback rate of the voice.
        /// </summary>
        public int PlaybackRate { get; set; }

        /// <summary>
        /// Required. Verified languages of the voice.
        /// </summary>
        public List<VoiceLanguage> VerifiedLanguages { get; set; }

        /// <summary>
        /// Required. Notice period for the voice.
        /// </summary>
        public int NoticePeriod { get; set; }

        /// <summary>
        /// Required. Instagram username of the voice owner.
        /// </summary>
        public string InstagramUsername { get; set; }

        /// <summary>
        /// Required. Twitter username of the voice owner.
        /// </summary>
        public string TwitterUsername { get; set; }

        /// <summary>
        /// Required. YouTube username of the voice owner.
        /// </summary>
        public string YouTubeUsername { get; set; }

        /// <summary>
        /// Required. TikTok username of the voice owner.
        /// </summary>
        public string TikTokUsername { get; set; }

        /// <summary>
        /// Required. URL of the voice's image.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Required. Whether the voice is added by the user.
        /// </summary>
        public bool IsAddedByUser { get; set; }


        [JsonIgnore] public Api Api => Api.ElevenLabs;
        [JsonIgnore] public bool IsCustom => true;
        [JsonIgnore] public bool? IsFree => FreeUsersAllowed;
        [JsonIgnore] public bool? IsFeatured => Featured;
        [JsonIgnore]
        public string OwnedBy
        {
            get
            {
                if (_ownedBy == null)
                {
                    // check social media usernames for non-null or empty values
                    if (!string.IsNullOrEmpty(InstagramUsername)) _ownedBy = InstagramUsername;
                    else if (!string.IsNullOrEmpty(TwitterUsername)) _ownedBy = TwitterUsername;
                    else if (!string.IsNullOrEmpty(YouTubeUsername)) _ownedBy = YouTubeUsername;
                    else if (!string.IsNullOrEmpty(TikTokUsername)) _ownedBy = TikTokUsername;
                    else _ownedBy = "ElevenLabs User"; // default value if all usernames are null or empty
                }

                return _ownedBy;
            }
        }
        [JsonIgnore] private string _ownedBy;

        public bool Equals(ElevenLabsSharedVoiceData other) => other != null && Id == other.Id;
    }

    public class SharedVoiceConverter : JsonConverter<ElevenLabsSharedVoiceData>
    {
        public override ElevenLabsSharedVoiceData ReadJson(JsonReader reader, Type objectType, ElevenLabsSharedVoiceData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            var voice = new ElevenLabsSharedVoiceData
            {
                OwnerId = obj.GetString("public_owner_id"),
                Id = obj.GetString("voice_id"),
                CreatedAt = obj["date_unix"]?.ToObject<UnixTime>(),
                Name = obj.GetString("name"),
                Accent = obj.GetString("accent"),
                Gender = VoiceGenderConverter.Parse(obj.GetString("gender")),
                Age = obj.GetEnum("age", VoiceAge.None),
                Descriptive = obj.GetString("descriptive"),
                Type = VoiceTypeConverter.Parse(obj.GetString("use_case")),
                Category = obj.GetEnum("category", VoiceCategory.None),
                UsageCharacterCount1Y = obj.GetLong("usage_character_count_1y"),
                UsageCharacterCount7D = obj.GetLong("usage_character_count_7d"),
                PlayApiUsageCharacterCount1Y = obj.GetLong("play_api_usage_character_count_1y"),
                ClonedByCount = obj.GetLong("cloned_by_count"),
                FreeUsersAllowed = obj.GetBool("free_users_allowed"),
                LiveModerationEnabled = obj.GetBool("live_moderation_enabled"),
                Featured = obj.GetBool("featured"),
                Language = LocaleUtil.ParseISOCode(obj.GetString("language")),
                Locale = obj.GetString("locale"),
                Description = obj.GetString("description"),
                PreviewUrl = obj.GetString("preview_url"),
                PlaybackRate = obj.GetInt("rate"),
                VerifiedLanguages = obj["verified_languages"]?.ToObject<List<VoiceLanguage>>(),
                NoticePeriod = obj.GetInt("notice_period"),
                InstagramUsername = obj.GetString("instagram_username"),
                TwitterUsername = obj.GetString("twitter_username"),
                YouTubeUsername = obj.GetString("youtube_username"),
                TikTokUsername = obj.GetString("tiktok_username"),
                ImageUrl = obj.GetString("image_url"),
                IsAddedByUser = obj.GetBool("is_added_by_user")
            };
            return voice;
        }

        public override void WriteJson(JsonWriter writer, ElevenLabsSharedVoiceData value, JsonSerializer serializer)
        {
            var obj = new JObject
            {
                ["public_owner_id"] = value.OwnerId,
                ["voice_id"] = value.Id,
                ["date_unix"] = value.CreatedAt?.Value,
                ["name"] = value.Name,
                ["accent"] = value.Accent,
                ["gender"] = value.Gender?.ToString().ToLowerInvariant(),
                ["age"] = value.Age?.ToString().ToLowerInvariant(),
                ["descriptive"] = value.Descriptive,
                ["use_case"] = value.Type?.ToString().ToLowerInvariant(),
                ["category"] = value.Category.ToString().ToLowerInvariant(),
                ["usage_character_count_1y"] = value.UsageCharacterCount1Y,
                ["usage_character_count_7d"] = value.UsageCharacterCount7D,
                ["play_api_usage_character_count_1y"] = value.PlayApiUsageCharacterCount1Y,
                ["cloned_by_count"] = value.ClonedByCount,
                ["free_users_allowed"] = value.FreeUsersAllowed,
                ["live_moderation_enabled"] = value.LiveModerationEnabled,
                ["featured"] = value.Featured,
                ["language"] = LocaleUtil.ToISOCode(value.Language),
                ["locale"] = value.Locale,
                ["description"] = value.Description,
                ["preview_url"] = value.PreviewUrl,
                ["rate"] = value.PlaybackRate,
                ["verified_languages"] = JArray.FromObject(value.VerifiedLanguages ?? new List<VoiceLanguage>()),
                ["notice_period"] = value.NoticePeriod,
                ["instagram_username"] = value.InstagramUsername,
                ["twitter_username"] = value.TwitterUsername,
                ["youtube_username"] = value.YouTubeUsername,
                ["tiktok_username"] = value.TikTokUsername,
                ["image_url"] = value.ImageUrl,
                ["is_added_by_user"] = value.IsAddedByUser
            };

            obj.WriteTo(writer);
        }
    }
}