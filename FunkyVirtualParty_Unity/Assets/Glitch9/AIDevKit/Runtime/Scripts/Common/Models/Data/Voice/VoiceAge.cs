using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    public enum VoiceAge
    {
        None,
        [ApiEnum("Child", "child")] Child,
        [ApiEnum("Young", "young")] Young,
        [ApiEnum("Middle Aged", "middle_aged")] MiddleAged,
        [ApiEnum("Senior", "old")] Senior,
    }
}