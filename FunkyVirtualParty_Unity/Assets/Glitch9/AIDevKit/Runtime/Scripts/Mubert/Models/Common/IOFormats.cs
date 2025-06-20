using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Mubert
{
    public enum MubertFormat
    {
        [ApiEnum("MP3", "mp3")] Mp3,
        [ApiEnum("WAV", "wav")] Wav,
        [ApiEnum("FLAC", "flac")] Flac
    }

    public enum MubertBitrate
    {
        [ApiEnum("32 kbps", "32")] Kbps32 = 32,
        [ApiEnum("96 kbps", "96")] Kbps96 = 96,
        [ApiEnum("128 kbps", "128")] Kbps128 = 128,
        [ApiEnum("192 kbps", "192")] Kbps192 = 192,
        [ApiEnum("256 kbps", "256")] Kbps256 = 256,
        [ApiEnum("320 kbps", "320")] Kbps320 = 320
    }

    /// <summary>
    /// The complexity of the arrangement, representing how many instruments play simultaneously.
    /// </summary>
    public enum ArrangementIntensity
    {
        /// <summary>
        /// Fewer instruments, more minimal and quiet arrangement.
        /// </summary>
        [ApiEnum("Low (Minimal / Sparse Sound)", "low")] Low,

        /// <summary>
        /// Balanced instrument layers and energy.
        /// </summary>
        [ApiEnum("Medium (Balanced / Moderate Sound)", "medium")] Medium,

        /// <summary>
        /// More instruments, dense and rich sound.
        /// </summary>
        [ApiEnum("High (Rich / Full Sound)", "high")] High
    }

    /// <summary>
    /// The composition type that defines the structure of the generated track.
    /// </summary>
    public enum CompositionMode
    {
        /// <summary>
        /// A standard track with intro, drops, breaks and outro.
        /// </summary>
        [ApiEnum("Track", "track")] Track,

        /// <summary>
        /// A seamless loopable track.
        /// </summary>
        [ApiEnum("Loop", "loop")] Loop,

        /// <summary>
        /// A short and complete musical piece best for up to 40 seconds.
        /// </summary>
        [ApiEnum("Jingle", "jingle")] Jingle,

        /// <summary>
        /// A DJ-style mix where tracks blend into each other.
        /// </summary>
        [ApiEnum("Mix", "mix")] Mix
    }
}
