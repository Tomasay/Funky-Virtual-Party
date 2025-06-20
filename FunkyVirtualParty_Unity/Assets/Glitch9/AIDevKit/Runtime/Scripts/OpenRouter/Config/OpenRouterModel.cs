
// ReSharper disable All
namespace Glitch9.AIDevKit.OpenRouter
{
    public class OpenRouterModel
    {
        /// <remarks>
        /// Returns a maximum of 4096 tokens.
        /// </remarks>
        /// <remarks>
        /// A maximum of 200000 tokens can be processed at a time.
        /// </remarks>
        public const string Claude3_Sonnet = "anthropic/claude-3-sonnet";

        /// <remarks>
        /// Returns a maximum of 512 tokens.
        /// </remarks>
        /// <remarks>
        /// A maximum of 6144 tokens can be processed at a time.
        /// </remarks>
        public const string Goliath120b = "alpindale/goliath-120b";

    }

}
