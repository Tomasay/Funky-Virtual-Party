using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// The style of the generated images.
    /// Vivid causes the model to lean towards generating hyper-real and dramatic images.
    /// Natural causes the model to produce more natural, less hyper-real looking images.
    /// This param is only supported for <see cref="OpenAIModel.DallE3"/>.
    /// </summary>
    /// <remarks> Defaults to <see cref="ImageStyle.Vivid"/> </remarks>
    public enum ImageStyle
    {
        [ApiEnum("Vivid", "vivid")] Vivid,
        [ApiEnum("Natural", "natural")] Natural,
    }
}