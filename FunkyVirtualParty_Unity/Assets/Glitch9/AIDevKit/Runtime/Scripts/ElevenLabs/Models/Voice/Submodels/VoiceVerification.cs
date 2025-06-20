using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{
    /// <summary>
    /// Represents the overall voice verification state and history.
    /// </summary>
    public class VoiceVerification
    {
        /// <summary>
        /// Whether this voice requires verification.
        /// </summary>
        [JsonProperty("requires_verification")] public bool RequiresVerification { get; set; }

        /// <summary>
        /// Whether this voice has been successfully verified.
        /// </summary>
        [JsonProperty("is_verified")] public bool IsVerified { get; set; }

        /// <summary>
        /// A list of reasons why verification failed.
        /// </summary>
        [JsonProperty("verification_failures")] public List<string> VerificationFailures { get; set; }

        /// <summary>
        /// The number of times a verification attempt has been made.
        /// </summary>
        [JsonProperty("verification_attempts_count")] public int VerificationAttemptsCount { get; set; }

        /// <summary>
        /// The language used for verification attempts (e.g., \"en\", \"ko\").
        /// </summary>
        [JsonProperty("language")] public string Language { get; set; }

        /// <summary>
        /// A list of all verification attempts made for this voice.
        /// </summary>
        [JsonProperty("verification_attempts")] public List<VerificationAttempt> VerificationAttempts { get; set; }
    }

    /// <summary>
    /// Represents a single attempt to verify a voice.
    /// </summary>
    public class VerificationAttempt
    {
        /// <summary>
        /// The text used during the verification attempt.
        /// </summary>
        [JsonProperty("text")] public string Text { get; set; }

        /// <summary>
        /// The time the attempt was made, in Unix time.
        /// </summary>
        [JsonProperty("date_unix")] public long DateUnix { get; set; }

        /// <summary>
        /// Whether this verification attempt was accepted.
        /// </summary>
        [JsonProperty("accepted")] public bool Accepted { get; set; }

        /// <summary>
        /// The similarity score between the original and the attempt (higher is better).
        /// </summary>
        [JsonProperty("similarity")] public double Similarity { get; set; }

        /// <summary>
        /// The Levenshtein distance between expected and actual text (lower is better).
        /// </summary>
        [JsonProperty("levenshtein_distance")] public double LevenshteinDistance { get; set; }

        /// <summary>
        /// The recording used in this verification attempt.
        /// </summary>
        [JsonProperty("recording")] public Recording Recording { get; set; }
    }

    /// <summary>
    /// Represents a recording used during the voice verification process.
    /// </summary>
    public class Recording
    {
        /// <summary>
        /// The ID of the recording.
        /// </summary>
        [JsonProperty("recording_id")] public string RecordingId { get; set; }

        /// <summary>
        /// The MIME type of the recording (e.g., audio/wav, audio/mpeg).
        /// </summary>
        [JsonProperty("mime_type")] public string MimeType { get; set; }

        /// <summary>
        /// The size of the recording in bytes.
        /// </summary>
        [JsonProperty("size_bytes")] public long SizeBytes { get; set; }

        /// <summary>
        /// The upload date of the recording in Unix time.
        /// </summary>
        [JsonProperty("upload_date_unix")] public long UploadDateUnix { get; set; }

        /// <summary>
        /// The transcribed text from the recording.
        /// </summary>
        [JsonProperty("transcription")] public string Transcription { get; set; }
    }
}