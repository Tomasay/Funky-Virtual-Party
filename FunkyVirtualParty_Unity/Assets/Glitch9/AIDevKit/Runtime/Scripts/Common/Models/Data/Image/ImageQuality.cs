using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// The quality of the image that will be generated.
    /// <see cref="ImageQuality.HighDefinition"/> creates images with finer details and greater consistency across the image.
    /// This param is only supported for <see cref="OpenAIModel.DallE3"/>.
    /// </summary>
    /// <remarks>Defaults to <see cref="ImageQuality.Standard"/> </remarks>
    public enum ImageQuality
    {
        [ApiEnum("Standard", "standard")] Standard,
        [ApiEnum("High Definition", "hd")] HighDefinition,
        [ApiEnum("Low", "low")] Low,
        [ApiEnum("Medium", "medium")] Medium,
        [ApiEnum("High", "high")] High,
    }
}