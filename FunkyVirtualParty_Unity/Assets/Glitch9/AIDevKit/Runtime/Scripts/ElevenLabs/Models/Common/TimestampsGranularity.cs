using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs
{
    /// <summary>
    /// The granularity of the timestamps in the transcription. 
    /// ‘word’ provides word-level timestamps and 
    /// ‘character’ provides character-level timestamps per word.
    /// </summary>
    public enum TimestampsGranularity
    {
        [ApiEnum("None", "none")] None,
        [ApiEnum("Word", "word")] Word,
        [ApiEnum("Character", "character")] Character
    }
}