using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Represents a verbose json transcription response returned by model, based on the provided input.
    /// </summary>
    public class OpenAITranscript : Transcript
    {
        /// <summary>
        /// The language of the input audio.
        /// </summary>
        [JsonProperty("language")] public override SystemLanguage Language { get; set; }

        /// <summary>
        /// The transcribed text.
        /// </summary>
        [JsonProperty("text")] public override string Text { get; set; }

        /// <summary>
        /// The duration of the input audio.
        /// </summary>
        [JsonProperty("duration")] public long Duration { get; set; }

        /// <summary>
        /// Extracted words and their corresponding timestamps.
        /// </summary>
        [JsonProperty("words")] public WordObject[] Words { get; set; }

        /// <summary>
        /// Segments of the transcribed text and their corresponding details.
        /// </summary>
        [JsonProperty("segments")] public SegmentObject[] Segments { get; set; }
    }

    public class WordObject
    {
        /// <summary>
        /// The text content of the word.
        /// </summary>
        [JsonProperty("word")] public string Word { get; set; }

        /// <summary>
        /// Start time of the word in seconds.
        /// </summary>
        [JsonProperty("start")] public float Start { get; set; }

        /// <summary>
        /// End time of the word in seconds.
        /// </summary>
        [JsonProperty("end")] public float End { get; set; }
    }

    public class SegmentObject
    {
        /// <summary>
        /// Unique identifier of the segment.
        /// </summary>
        [JsonProperty("id")] public int Id { get; set; }

        /// <summary>
        /// Seek offset of the segment.
        /// </summary>
        [JsonProperty("seek")] public int Seek { get; set; }

        /// <summary>
        /// Start time of the segment in seconds.
        /// </summary>
        [JsonProperty("start")] public float Start { get; set; }

        /// <summary>
        /// End time of the segment in seconds.
        /// </summary>
        [JsonProperty("end")] public float End { get; set; }

        /// <summary>
        /// Text content of the segment.
        /// </summary>
        [JsonProperty("text")] public string Text { get; set; }

        /// <summary>
        /// Array of token IDs for the text content.
        /// </summary>
        [JsonProperty("tokens")] public int[] Tokens { get; set; }

        /// <summary>
        /// Temperature parameter used for generating the segment.
        /// </summary>
        [JsonProperty("temperature")] public float Temperature { get; set; }

        /// <summary>
        /// Average logprob of the segment. If the value is lower than -1, consider the logprobs failed.
        /// </summary>
        [JsonProperty("avg_logprob")] public float AvgLogprob { get; set; }

        /// <summary>
        /// Compression ratio of the segment. If the value is greater than 2.4, consider the compression failed.
        /// </summary>
        [JsonProperty("compression_ratio")] public float CompressionRatio { get; set; }

        /// <summary>
        /// Probability of no speech in the segment. If the value is higher than 1.0 and the avg_logprob is below -1, consider this segment silent.
        /// </summary>
        [JsonProperty("no_speech_prob")] public float NoSpeechProb { get; set; }
    }
}