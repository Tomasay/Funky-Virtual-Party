using Glitch9.AIDevKit.Google;

namespace Glitch9.AIDevKit.Editor
{
    internal static class ImageModelUtil
    {
        internal static float ResolveAspectRatio(Model model, AspectRatio aspectRatio, ImageSize imageSize)
        {
            if (model == null) return 1f; // Default aspect ratio

            return model.Api switch
            {
                Api.Google => aspectRatio switch
                {
                    Google.AspectRatio.Square => 1f,
                    Google.AspectRatio.Portrait => 3f / 4f,
                    Google.AspectRatio.Landscape => 4f / 3f,
                    Google.AspectRatio.Vertical => 9f / 16f,
                    Google.AspectRatio.Horizontal => 16f / 9f,
                    _ => 1f
                },
                Api.OpenAI => imageSize switch
                {
                    ImageSize._256x256 => 1f,
                    ImageSize._512x512 => 1f,
                    ImageSize._1024x1024 => 1f,
                    ImageSize._1792x1024 => 1.75f,
                    ImageSize._1024x1792 => 0.56f,
                    _ => 1f
                },
                _ => 1f
            };
        }
    }
}