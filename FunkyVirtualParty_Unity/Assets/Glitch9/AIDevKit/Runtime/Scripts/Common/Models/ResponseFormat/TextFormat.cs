using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    public enum TextFormat
    {
        [ApiEnum("auto")] Auto,
        [ApiEnum("text")] Text, // also for Transcription
        [ApiEnum("json_object")] JsonObject,
        [ApiEnum("json_schema")] JsonSchema, // implemented 2025.04
    }
}