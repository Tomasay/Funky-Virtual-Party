using Glitch9.IO.Networking.RESTApi;
using UnityEngine;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class ElevenLabsQuery : Query
    {
        /// <summary>
        /// Optional.
        /// The next page token to use for pagination. Returned from the previous request.
        /// </summary>
        [QueryParameter("next_page_token")] public string NextPageToken { get; set; }

        /// <summary>
        /// Optional.
        /// Search term to filter voices by. Searches in name, description, labels, and category.
        /// </summary>
        [QueryParameter("search")] public string Search { get; set; }

        /// <summary>
        /// Optional.
        /// Which field to sort by. One of: 'created_at_unix', 'name'.
        /// </summary>
        [QueryParameter("sort")] public string Sort { get; set; }

        /// <summary>
        /// Optional.
        /// Which direction to sort the voices in. 'asc' or 'desc'.
        /// </summary>
        [QueryParameter("sort_direction")] public SortDirection? SortDirection { get; set; }

        /// <summary>
        /// Optional.
        /// Type of the voice to filter by. One of: 'personal', 'community', 'default', 'workspace'.
        /// </summary>
        [QueryParameter("voice_type")] public string VoiceType { get; set; }

        /// <summary>
        /// Optional.
        /// Category of the voice to filter by. One of: 'premade', 'cloned', 'generated', 'professional'.
        /// </summary>
        [QueryParameter("category")] public VoiceCategory? Category { get; set; }

        /// <summary>
        /// Optional.
        /// State of the voice’s fine tuning to filter by.
        /// One of: 'draft', 'not_verified', 'not_started', 'queued', 'fine_tuning', 'fine_tuned', 'failed', 'delayed'.
        /// </summary>
        [QueryParameter("fine_tuning_state")] public string FineTuningState { get; set; }

        /// <summary>
        /// Optional.
        /// Collection ID to filter voices by.
        /// </summary>
        [QueryParameter("collection_id")] public string CollectionId { get; set; }

        /// <summary>
        /// Optional.
        /// Whether to include the total count of voices found in the response.
        /// Defaults to true. May incur performance cost.
        /// </summary>
        [QueryParameter("include_total_count")] public bool? IncludeTotalCount { get; set; }

        /// <summary>
        /// Optional. Gender used for filtering
        /// </summary>
        [QueryParameter("gender")] public VoiceGender? Gender { get; set; }

        /// <summary>
        /// Optional. Accent used for filtering
        /// </summary>
        [QueryParameter("age")] public VoiceAge? Age { get; set; }

        /// <summary>
        /// Optional. Language used for filtering
        /// </summary>
        [QueryParameter("language")] public SystemLanguage? Language { get; set; }

        /// <summary>
        /// Optional. Accent used for filtering
        /// </summary>
        [QueryParameter("accent")] public string Accent { get; set; }

        /// <summary>
        /// Optional. Locale used for filtering
        /// </summary>
        [QueryParameter("locale")] public string Locale { get; set; }

        /// <summary>
        /// Optional. Use-case used for filtering
        /// </summary>
        [QueryParameter("use_case")] public VoiceType? Type { get; set; }

        /// <summary>
        /// Optional. Search term used for filtering
        /// </summary>
        [QueryParameter("descriptives")] public string SearchTerm { get; set; }

        /// <summary>
        /// Optional. Filter featured voices
        /// Defaults to false
        /// </summary>
        [QueryParameter("featured")] public bool? Featured { get; set; }

        /// <summary>
        /// Optional. Filter voices with a minimum notice period of the given number of days.
        /// </summary>
        [QueryParameter("min_notice_period_days")] public int? MinimumNoticePeriod { get; set; }

        /// <summary>
        /// Optional. Filter voices that are enabled for the reader app
        /// Defaults to false
        /// </summary>
        [QueryParameter("reader_app_enabled")] public bool? ReaderAppEnabled { get; set; }

        /// <summary>
        /// Optional. Filter voices by public owner ID
        /// </summary>
        [QueryParameter("owner_id")] public string OwnerId { get; set; }

        /// <summary>
        /// Optional. Defaults to 0
        /// </summary>
        [QueryParameter("page")] public int? Page { get; set; }

        public ElevenLabsQuery(int page, int pageSize)
        {
            Page = page;
            Size = pageSize;
        }
    }

    public enum SortDirection
    {
        [ApiEnum("asc")] Ascending,
        [ApiEnum("desc")] Descending
    }
}