using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class VoiceSample
    {
        /// <summary>
        /// Required. The ID of the sample.
        /// </summary>
        [JsonProperty("sample_id")] public string SampleId { get; set; }

        /// <summary>
        /// Required. The name of the sample file.
        /// </summary>
        [JsonProperty("file_name")] public string FileName { get; set; }

        /// <summary>
        /// Required. The MIME type of the sample file.
        /// </summary>
        [JsonProperty("mime_type")] public string MimeType { get; set; }

        /// <summary>
        /// Required. The size of the sample file in bytes.
        /// </summary>
        [JsonProperty("size_bytes")] public long SizeBytes { get; set; }

        /// <summary>
        /// Required. The hash of the sample file.
        /// </summary>
        [JsonProperty("hash")] public string Hash { get; set; }

        /// <summary>
        /// Required. The duration of the sample in seconds.
        /// </summary>
        [JsonProperty("duration_secs")] public double DurationSecs { get; set; }

        /// <summary>
        /// Optional. Whether to remove background noise.
        /// </summary>
        /// <remarks>Default: false</remarks> 
        [JsonProperty("remove_background_noise")] public bool? RemoveBackgroundNoise { get; set; }

        /// <summary>
        /// Optional. Whether the sample has isolated audio.
        /// </summary>
        /// <remarks>Default: false</remarks>
        [JsonProperty("has_isolated_audio")] public bool? HasIsolatedAudio { get; set; }

        /// <summary>
        /// Optional. Whether the sample has isolated audio preview.
        /// </summary>
        /// <remarks>Default: false</remarks> 
        [JsonProperty("has_isolated_audio_preview")] public bool? HasIsolatedAudioPreview { get; set; }

        /// <summary>
        /// Optional. The speaker separation information.
        /// </summary>
        [JsonProperty("speaker_separation")] public SpeakerSeparation SpeakerSeparation { get; set; }
    }

    public class SpeakerSeparation
    {
        /// <summary>
        /// Required. The ID of the voice.
        /// </summary>
        [JsonProperty("voice_id")] public string VoiceId { get; set; }

        /// <summary>
        /// Required. The ID of the sample.
        /// </summary>
        [JsonProperty("sample_id")] public string SampleId { get; set; }

        /// <summary>
        /// Required. The status of the speaker separation.
        /// </summary>
        [JsonProperty("status")] public string Status { get; set; }

        /// <summary>
        /// Optional. The selected speaker IDs.
        /// </summary>
        [JsonProperty("selected_speaker_ids")] public List<string> SelectedSpeakerIds { get; set; }

        /// <summary>
        /// Optional. The speakers information.
        /// </summary>
        [JsonProperty("speakers")] public Dictionary<string, Speaker> Speakers { get; set; }

        /// <summary>
        /// Optional. The ID of the speaker.
        /// </summary>
        [JsonProperty("speaker_id")] public string SpeakerId { get; set; }
    }

    public class Speaker
    {
        /// <summary>
        /// Required. The ID of the speaker.
        /// </summary>
        [JsonProperty("speaker_id")] public string SpeakerId { get; set; }

        /// <summary>
        /// Required. The duration of the speaker in seconds.
        /// </summary>
        [JsonProperty("duration_secs")] public double? DurationSecs { get; set; }

        /// <summary>
        /// Optional. The utterances of the speaker.
        /// </summary>
        [JsonProperty("utterances")] public List<Utterances> Utterances { get; set; }
    }

    public class Utterances
    {
        /// <summary>
        /// The start time of the utterance in seconds.
        /// </summary>
        [JsonProperty("start")] public double? Start { get; set; }

        /// <summary>
        /// The end time of the utterance in seconds.
        /// </summary>
        [JsonProperty("end")] public double? End { get; set; }
    }
}