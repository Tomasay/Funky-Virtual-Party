using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Mubert
{
    /// <summary>
    /// Request object for getting all available music channels for streaming and generation.
    /// </summary>
    public class GetPlayMusicRequest : MubertRequest<MubertRequestParams>
    {
        /// <summary>
        /// The method name for this request.
        /// </summary>
        public override MubertMethod Method => MubertMethod.GetPlayMusic;
    }

    /// <summary>
    /// Response object for GetPlayMusic request.
    /// </summary>
    public class GetPlayMusicResponse : MubertResponse<GetPlayMusicData>
    {
    }

    /// <summary>
    /// The data payload containing the music channel structure.
    /// </summary>
    public class GetPlayMusicData
    {
        /// <summary>
        /// A list of music categories available for playback or generation.
        /// </summary>
        [JsonProperty("categories")] public List<MusicCategory> Categories { get; set; }
    }

    /// <summary>
    /// Represents a music category (e.g. Moods, Genres, etc.).
    /// </summary>
    public class MusicCategory
    {
        /// <summary>
        /// The ID of the category.
        /// </summary>
        [JsonProperty("category_id")] public int CategoryId { get; set; }

        /// <summary>
        /// The name of the category.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// The playlist index of the category (e.g. "0").
        /// </summary>
        [JsonProperty("playlist")] public string Playlist { get; set; }

        /// <summary>
        /// A list of groups under this category.
        /// </summary>
        [JsonProperty("groups")] public List<MusicGroup> Groups { get; set; }
    }

    /// <summary>
    /// Represents a group within a category (e.g. Calm inside Moods).
    /// </summary>
    public class MusicGroup
    {
        /// <summary>
        /// The ID of the group.
        /// </summary>
        [JsonProperty("group_id")] public int GroupId { get; set; }

        /// <summary>
        /// The name of the group.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// The playlist index of the group (e.g. "0.0").
        /// </summary>
        [JsonProperty("playlist")] public string Playlist { get; set; }

        /// <summary>
        /// A list of channels under this group.
        /// </summary>
        [JsonProperty("channels")] public List<MusicChannel> Channels { get; set; }
    }

    /// <summary>
    /// Represents an actual music channel that can be streamed or used for generation.
    /// </summary>
    public class MusicChannel
    {
        /// <summary>
        /// The ID of the channel.
        /// </summary>
        [JsonProperty("channel_id")] public int ChannelId { get; set; }

        /// <summary>
        /// The name of the channel (e.g. Acoustic).
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// The unique playlist index of the channel (e.g. "0.0.1").
        /// </summary>
        [JsonProperty("playlist")] public string Playlist { get; set; }

        /// <summary>
        /// An emoji representing the channel (if available).
        /// </summary>
        [JsonProperty("emoji")] public string Emoji { get; set; }

        /// <summary>
        /// A custom icon URL or identifier (if available).
        /// </summary>
        [JsonProperty("icon")] public string Icon { get; set; }

        /// <summary>
        /// Streaming info including the actual URL to stream this channel.
        /// </summary>
        [JsonProperty("stream")] public StreamInfo Stream { get; set; }
    }

    /// <summary>
    /// Contains the streaming URL for a channel.
    /// </summary>
    public class StreamInfo
    {
        /// <summary>
        /// The full stream URL for this music channel.
        /// </summary>
        [JsonProperty("url")] public string Url { get; set; }
    }
}
