using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// The category of the voice.
    /// </summary>
    public enum VoiceCategory
    {
        None,
        [ApiEnum("Generated Voice", "generated")] Generated,
        [ApiEnum("Cloned Voice", "cloned")] Cloned,
        [ApiEnum("Premade Voice", "premade")] Premade,
        [ApiEnum("Professional Voice", "professional")] Professional,
        [ApiEnum("Famous Voice", "famous")] Famous,
        [ApiEnum("High Quality Voice", "high_quality")] HighQuality
    }
}