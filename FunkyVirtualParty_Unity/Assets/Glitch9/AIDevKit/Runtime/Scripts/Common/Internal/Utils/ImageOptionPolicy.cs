
namespace Glitch9.AIDevKit
{
    internal class ImageOptionPolicy
    {
        internal static ImageSize GetDefaultImageSize(Model model)
        {
            // all same size for now
            return ImageSize._1024x1024;
        }

        internal static ImageQuality GetDefaultImageQuality(Model model)
        {
            if (model == null) return ImageQuality.Standard;

            if (model.Id.Contains("dall-e-3"))
                return ImageQuality.Standard;

            if (model.Id.Contains("gpt"))
                return ImageQuality.High;

            return ImageQuality.Standard;
        }

        internal static bool IsImageSizeSupported(ImageSize size, Model model)
        {
            if (model == null) return true; // If no model is selected, enable all sizes
            AIDevKitConfig.SupportedImageSizes.TryGetValue(model.Id, out ImageSize[] supportedSizes);

            if (supportedSizes == null || supportedSizes.Length == 0)
            {
                return true; // If no specific sizes are defined for the model, enable all sizes
            }

            foreach (var supportedSize in supportedSizes)
            {
                if (supportedSize == size)
                {
                    return true; // The size is supported by the model
                }
            }

            return false; // The size is not supported by the model
        }

        internal static bool IsImageQualitySupported(ImageQuality quality, Model model)
        {
            if (model == null) return true; // If no model is selected, enable all qualities
            AIDevKitConfig.SupportedImageQualities.TryGetValue(model.Id, out ImageQuality[] supportedQualities);

            if (supportedQualities == null || supportedQualities.Length == 0)
            {
                return true; // If no specific qualities are defined for the model, enable all qualities
            }

            foreach (var supportedQuality in supportedQualities)
            {
                if (supportedQuality == quality)
                {
                    return true; // The quality is supported by the model
                }
            }

            return false; // The quality is not supported by the model
        }
    }
}