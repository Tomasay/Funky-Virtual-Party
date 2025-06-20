using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    public enum TranscriptFormat
    {
        [ApiEnum("text")] Text,
        [ApiEnum("json")] Json,
        [ApiEnum("srt")] Srt,
        [ApiEnum("verbose_json")] VerboseJson,
        [ApiEnum("vtt")] Vtt,
    }
}