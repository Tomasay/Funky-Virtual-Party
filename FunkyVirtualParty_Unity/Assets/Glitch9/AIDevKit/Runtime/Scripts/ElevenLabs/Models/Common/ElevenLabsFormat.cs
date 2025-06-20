namespace Glitch9.AIDevKit.ElevenLabs
{
    using Newtonsoft.Json;

    public abstract class ElevenLabsFormat
    {
        /// <summary>
        /// Required. The requested format of the transcript.
        /// </summary>
        [JsonProperty("format")] public string Format { get; set; }
    }

    public class DocxFormat : ElevenLabsFormat
    {
        [JsonProperty("include_speakers")] public bool? IncludeSpeakers { get; set; } = true;
        [JsonProperty("include_timestamps")] public bool? IncludeTimestamps { get; set; } = true;
        [JsonProperty("max_segment_chars")] public int? MaxSegmentChars { get; set; }
        [JsonProperty("max_segment_duration_s")] public double? MaxSegmentDurationS { get; set; }
        [JsonProperty("segment_on_silence_longer_than_s")] public double? SegmentOnSilenceLongerThanS { get; set; }
    }

    public class HtmlFormat : ElevenLabsFormat
    {
        [JsonProperty("include_speakers")] public bool? IncludeSpeakers { get; set; } = true;
        [JsonProperty("include_timestamps")] public bool? IncludeTimestamps { get; set; } = true;
        [JsonProperty("max_segment_chars")] public int? MaxSegmentChars { get; set; }
        [JsonProperty("max_segment_duration_s")] public double? MaxSegmentDurationS { get; set; }
        [JsonProperty("segment_on_silence_longer_than_s")] public double? SegmentOnSilenceLongerThanS { get; set; }
    }

    public class PdfFormat : ElevenLabsFormat
    {
        [JsonProperty("include_speakers")] public bool? IncludeSpeakers { get; set; } = true;
        [JsonProperty("include_timestamps")] public bool? IncludeTimestamps { get; set; } = true;
        [JsonProperty("max_segment_chars")] public int? MaxSegmentChars { get; set; }
        [JsonProperty("max_segment_duration_s")] public double? MaxSegmentDurationS { get; set; }
        [JsonProperty("segment_on_silence_longer_than_s")] public double? SegmentOnSilenceLongerThanS { get; set; }
    }

    public class SegmentedJsonFormat : ElevenLabsFormat
    {
        [JsonProperty("max_segment_chars")] public int? MaxSegmentChars { get; set; }
        [JsonProperty("max_segment_duration_s")] public double? MaxSegmentDurationS { get; set; }
        [JsonProperty("segment_on_silence_longer_than_s")] public double? SegmentOnSilenceLongerThanS { get; set; }
    }

    public class SrtFormat : ElevenLabsFormat
    {
        [JsonProperty("include_speakers")] public bool? IncludeSpeakers { get; set; } = false;
        [JsonProperty("include_timestamps")] public bool? IncludeTimestamps { get; set; } = true;
        [JsonProperty("max_characters_per_line")] public int? MaxCharactersPerLine { get; set; }
        [JsonProperty("max_segment_chars")] public int? MaxSegmentChars { get; set; }
        [JsonProperty("max_segment_duration_s")] public double? MaxSegmentDurationS { get; set; }
        [JsonProperty("segment_on_silence_longer_than_s")] public double? SegmentOnSilenceLongerThanS { get; set; }
    }

    public class TxtFormat : ElevenLabsFormat
    {
        [JsonProperty("include_speakers")] public bool? IncludeSpeakers { get; set; } = true;
        [JsonProperty("include_timestamps")] public bool? IncludeTimestamps { get; set; } = true;
        [JsonProperty("max_characters_per_line")] public int? MaxCharactersPerLine { get; set; }
        [JsonProperty("max_segment_chars")] public int? MaxSegmentChars { get; set; }
        [JsonProperty("max_segment_duration_s")] public double? MaxSegmentDurationS { get; set; }
        [JsonProperty("segment_on_silence_longer_than_s")] public double? SegmentOnSilenceLongerThanS { get; set; }
    }
}