using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class VoiceSharing
    {
        /// <summary>
        /// Required. The status of the voice sharing.
        /// </summary>
        [JsonProperty("status")] public string Status { get; set; }

        /// <summary>
        /// Optional. The date of the voice sharing in Unix time.
        /// </summary>
        [JsonProperty("date_unix")] public UnixTime? DateUnix { get; set; }

        /// <summary>
        /// Optional. A list of whitelisted emails.
        /// </summary>
        [JsonProperty("whitelisted_emails")] public List<string> WhitelistedEmails { get; set; }

        /// <summary>
        /// Optional. The ID of the public owner.
        /// </summary>
        [JsonProperty("public_owner_id")] public string PublicOwnerId { get; set; }

        /// <summary>
        /// Optional. The ID of the original voice.
        /// </summary>
        [JsonProperty("original_voice_id")] public string OriginalVoiceId { get; set; }

        /// <summary>
        /// Optional. Whether financial rewards are enabled.
        /// </summary>
        [JsonProperty("financial_rewards_enabled")] public bool? FinancialRewardsEnabled { get; set; }

        /// <summary>
        /// Optional. Whether free users are allowed.
        /// </summary>
        [JsonProperty("free_users_allowed")] public bool? FreeUsersAllowed { get; set; }

        /// <summary>
        /// Optional. Whether live moderation is enabled.
        /// </summary>
        [JsonProperty("live_moderation_enabled")] public bool? LiveModerationEnabled { get; set; }

        /// <summary>
        /// Optional. The notice period of the voice sharing.
        /// </summary>
        [JsonProperty("notice_period")] public int? NoticePeriod { get; set; }

        /// <summary>
        /// Optional. Whether voice mixing is allowed.
        /// </summary>
        [JsonProperty("voice_mixing_allowed")] public bool? VoiceMixingAllowed { get; set; }

        /// <summary>
        /// Optional. Whether the voice is featured.
        /// </summary>
        [JsonProperty("featured")] public bool? Featured { get; set; }

        /// <summary>
        /// Optional. The category of the voice.
        /// </summary>
        [JsonProperty("category")] public VoiceCategory? Category { get; set; }

        /// <summary>
        /// Optional. Number of likes on the voice.
        /// </summary>
        [JsonProperty("liked_by_count")] public int? LikedByCount { get; set; }

        /// <summary>
        /// Optional. Number of clones on the voice.
        /// </summary>
        [JsonProperty("cloned_by_count")] public int? ClonedByCount { get; set; }

        /// <summary>
        /// Optional. The name of the voice.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Optional. Labels of the voice.
        /// </summary>
        [JsonProperty("labels")] public Dictionary<string, string> Labels { get; set; }

        /// <summary>
        /// Optional. The review status of the voice.
        /// </summary>
        [JsonProperty("review_status")] public string ReviewStatus { get; set; }

        /// <summary>
        /// Optional. Whether the voice is enabled in the library.
        /// </summary>
        [JsonProperty("enabled_in_library")] public bool? EnabledInLibrary { get; set; }

        /// <summary>
        /// Optional. The sample ID of the history item.
        /// </summary>
        [JsonProperty("history_item_sample_id")] public string HistoryItemSampleId { get; set; }

        /// <summary>
        /// Optional. The rate of the voice sharing.
        /// </summary>
        [JsonProperty("rate")] public double? Rate { get; set; }

        /// <summary>
        /// Optional. The date to disable the voice sharing.
        /// </summary>
        [JsonProperty("disable_at_unix")] public UnixTime? DisableAtUnix { get; set; }

        /// <summary>
        /// Optional. Whether the reader app is enabled.
        /// </summary>
        [JsonProperty("reader_app_enabled")] public bool? ReaderAppEnabled { get; set; }

        /// <summary>
        /// Optional. The image URL of the voice.
        /// </summary>
        [JsonProperty("image_url")] public string ImageUrl { get; set; }

        /// <summary>
        /// Optional. The ban reason of the voice.
        /// </summary>
        [JsonProperty("ban_reason")] public string BanReason { get; set; }

        /// <summary>
        /// Optional. The description of the voice.
        /// </summary>
        [JsonProperty("description")] public string Description { get; set; }

        /// <summary>
        /// Optional. The review message of the voice.
        /// </summary>
        [JsonProperty("review_message")] public string ReviewMessage { get; set; }

        /// <summary>
        /// Optional. Instagram username associated with the voice.
        /// </summary>
        [JsonProperty("instagram_username")] public string InstagramUsername { get; set; }

        /// <summary>
        /// Optional. Twitter/X username associated with the voice.
        /// </summary>
        [JsonProperty("twitter_username")] public string TwitterUsername { get; set; }

        /// <summary>
        /// Optional. YouTube username associated with the voice.
        /// </summary>
        [JsonProperty("youtube_username")] public string YoutubeUsername { get; set; }

        /// <summary>
        /// Optional. TikTok username associated with the voice.
        /// </summary>
        [JsonProperty("tiktok_username")] public string TiktokUsername { get; set; }

        /// <summary>
        /// Optional. Moderation check object.
        /// </summary>
        [JsonProperty("moderation_check")] public object ModerationCheck { get; set; }

        /// <summary>
        /// Optional. Reader restrictions applied on platforms.
        /// </summary>
        [JsonProperty("reader_restricted_on")] public List<object> ReaderRestrictedOn { get; set; }
    }
}