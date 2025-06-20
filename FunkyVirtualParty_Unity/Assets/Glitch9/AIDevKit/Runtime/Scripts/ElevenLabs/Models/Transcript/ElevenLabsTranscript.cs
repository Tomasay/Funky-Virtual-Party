using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class ElevenLabsTranscript : Transcript
    {
        /// <summary>
        /// The detected language code (e.g. 'eng' for English).
        /// </summary>
        [JsonProperty("language_code")] public string LanguageCode { get; set; }
        [JsonIgnore] public override SystemLanguage Language => ElevenLabsUtil.GetLanguage(LanguageCode);

        /// <summary>
        /// The confidence score of the language detection (0 to 1).
        /// </summary>
        [JsonProperty("language_probability")] public double LanguageProbability { get; set; }

        /// <summary>
        /// The raw text of the transcription.
        /// </summary>
        [JsonProperty("text")] public override string Text { get; set; }

        /// <summary>
        /// List of words with their timing information.
        /// </summary>
        [JsonProperty("words")] public List<Word> Words { get; set; }

        /// <summary>
        /// Optional. Requested additional formats of the transcript.
        /// </summary>
        [JsonProperty("additional_formats")] public List<AdditionalFormat> AdditionalFormats { get; set; }
    }

    public class Word
    {
        /// <summary>
        /// The text of the word.
        /// </summary>
        [JsonProperty("text")] public string Text { get; set; }

        /// <summary>
        /// The type of the token (e.g., 'word', 'spacing').
        /// </summary>
        [JsonProperty("type")] public string Type { get; set; }

        /// <summary>
        /// The start time of the word in seconds.
        /// </summary>
        [JsonProperty("start")] public double Start { get; set; }

        /// <summary>
        /// The end time of the word in seconds.
        /// </summary>
        [JsonProperty("end")] public double End { get; set; }

        /// <summary>
        /// The ID of the speaker who spoke the word.
        /// </summary>
        [JsonProperty("speaker_id")] public string SpeakerId { get; set; }

        /// <summary>
        /// Optional. List of characters with their timing information.
        /// </summary>
        [JsonProperty("characters")] public List<Character> Characters { get; set; }
    }

    public class Character
    {
        /// <summary>
        /// The text of the character.
        /// </summary>
        [JsonProperty("text")] public string Text { get; set; }

        /// <summary>
        /// The start time of the character in seconds.
        /// </summary>
        [JsonProperty("start")] public double Start { get; set; }

        /// <summary>
        /// The end time of the character in seconds.
        /// </summary>
        [JsonProperty("end")] public double End { get; set; }
    }

    public class AdditionalFormat
    {
        /// <summary>
        /// The requested format of the transcript.
        /// </summary>
        [JsonProperty("requested_format")] public string RequestedFormat { get; set; }

        /// <summary>
        /// The file extension of the format.
        /// </summary>
        [JsonProperty("file_extension")] public string FileExtension { get; set; }

        /// <summary>
        /// The content type of the format.
        /// </summary>
        [JsonProperty("content_type")] public string ContentType { get; set; }

        /// <summary>
        /// Indicates if the content is base64 encoded.
        /// </summary>
        [JsonProperty("is_base64_encoded")] public bool IsBase64Encoded { get; set; }

        /// <summary>
        /// The content of the transcript in the specified format.
        /// </summary>
        [JsonProperty("content")] public string Content { get; set; }
    }
}