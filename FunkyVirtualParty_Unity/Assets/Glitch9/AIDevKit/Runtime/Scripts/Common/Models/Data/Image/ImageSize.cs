using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    public enum ImageSize
    {
        [ApiEnum("256x256", "256x256")] _256x256,
        [ApiEnum("512x512", "512x512")] _512x512,
        [ApiEnum("1024x1024", "1024x1024")] _1024x1024,
        [ApiEnum("1024x1792", "1024x1792")] _1024x1792,
        [ApiEnum("1792x1024", "1792x1024")] _1792x1024,
        [ApiEnum("1024x1536", "1024x1536")] _1024x1536,
        [ApiEnum("1536x1024", "1536x1024")] _1536x1024,
    }
}