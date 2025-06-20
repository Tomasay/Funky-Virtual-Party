using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    public enum OpenAIServiceTier
    {
        [ApiEnum("Auto", "auto")] Auto,
        [ApiEnum("Default", "default")] Default,
        [ApiEnum("Flex", "flex")] Flex
    }
}