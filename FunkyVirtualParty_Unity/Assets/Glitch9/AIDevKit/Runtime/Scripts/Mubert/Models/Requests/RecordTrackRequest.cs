using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Mubert
{
    /// <summary>
    /// Request object for generating a music track based on playlist and other settings.
    /// Uses Mubert's RecordTrack method.
    /// </summary>
    public class RecordTrackRequest : MubertRequest<RecordTrackRequestParams>
    {
        /// <summary>
        /// Required. The method to call on the Mubert API.
        /// </summary>
        public override MubertMethod Method => MubertMethod.RecordTrack;
    }

    /// <summary>
    /// Parameters for the RecordTrack method.
    /// </summary>
    public class RecordTrackRequestParams : BaseRecordTrackRequestParams
    { 
        /// <summary>
        /// Required. The channel index in format like "0.2.3".
        /// </summary>
        [JsonProperty("playlist")] public string Playlist { get; set; } 
    } 
 
    public abstract class BaseRecordTrackRequestParams : MubertRequestParams
    {  
        /// <summary>
        /// Required. Duration of the track in seconds.
        /// </summary>
        [JsonProperty("duration")] public string Duration { get; set; }

        /// <summary>
        /// Optional. Audio format: mp3, wav, flac. Default is mp3.
        /// </summary>
        [JsonProperty("format")] public MubertFormat? Format { get; set; }

        /// <summary>
        /// Optional. Bitrate (kbps): 32, 96, 128, 192, 256, 320. Default is 128.
        /// </summary>
        [JsonProperty("bitrate")] public MubertBitrate? Bitrate { get; set; }

        /// <summary>
        /// Optional. Intensity of the arrangement: low, medium, high. Default is high.
        /// </summary>
        [JsonProperty("intensity")] public ArrangementIntensity? Intensity { get; set; }

        /// <summary>
        /// Optional. Composition mode: track, loop. Default is track.
        /// </summary>
        [JsonProperty("mode")] public CompositionMode? Mode { get; set; }
    }
}
