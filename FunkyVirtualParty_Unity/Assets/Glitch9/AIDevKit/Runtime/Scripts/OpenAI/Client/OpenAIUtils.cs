using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glitch9.AIDevKit.OpenAI
{
    internal static class OpenAIUtils
    {
        internal static Usage CreateImageUsage(string modelId, ImageSize size, ImageQuality quality, int count)
        {
            UsageType usageType = ResolveUsageType(size, quality);
            return UsageFactory.Create(modelId, usageType, count);
        }

        internal static ImageSize GetDefaultImageSize(Model model)
        {
            // all same size for now
            return ImageSize._1024x1024;
        }

        internal static ImageQuality GetDefaultImageQuality(Model model)
        {
            if (model == null) return ImageQuality.Standard;

            if (model.Id.Contains("dall-e-3"))
            {
                return ImageQuality.Standard;
            }

            if (model.Id.Contains("gpt"))
            {
                return ImageQuality.High;
            }

            return ImageQuality.Standard;
        }

        private static UsageType ResolveUsageType(ImageSize size, ImageQuality quality)
        {
            return quality switch
            {
                ImageQuality.Standard => size switch
                {
                    ImageSize._256x256 => UsageType.ImageSD256,
                    ImageSize._512x512 => UsageType.ImageSD512,
                    ImageSize._1024x1024 => UsageType.ImageSD1024,
                    ImageSize._1024x1792 => UsageType.ImageSD1792,
                    _ => throw new ArgumentOutOfRangeException(nameof(size), size, null),
                },
                ImageQuality.HighDefinition => size switch
                {
                    ImageSize._1024x1024 => UsageType.ImageHD1024,
                    ImageSize._1792x1024 => UsageType.ImageHD1792,
                    _ => throw new ArgumentOutOfRangeException(nameof(size), size, null),
                },
                ImageQuality.Low => size switch
                {
                    ImageSize._1024x1024 => UsageType.ImageLow1024,
                    ImageSize._1024x1536 => UsageType.ImageLow1536,
                    _ => throw new ArgumentOutOfRangeException(nameof(size), size, null),
                },
                ImageQuality.Medium => size switch
                {
                    ImageSize._1024x1024 => UsageType.ImageMedium1024,
                    ImageSize._1024x1536 => UsageType.ImageMedium1536,
                    _ => throw new ArgumentOutOfRangeException(nameof(size), size, null),
                },
                ImageQuality.High => size switch
                {
                    ImageSize._1024x1024 => UsageType.ImageHigh1024,
                    ImageSize._1024x1536 => UsageType.ImageHigh1536,
                    _ => throw new ArgumentOutOfRangeException(nameof(size), size, null),
                },
                _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, null),
            };
        }

        internal static JToken CreateThreadMessageContentJToken(List<ContentPartWrapper> partWrappers, JsonSerializer serializer)
        {
            AIDevKitDebug.Info("[OpenAIUtils] Creating ThreadMessage content JToken...");

            if (partWrappers.IsNullOrEmpty()) return null;

            var array = new JArray();

            foreach (var partWrapper in partWrappers)
            {
                if (partWrapper == null) continue;

                JToken token;
                // check if part is a string or object

                if (partWrapper.IsString)
                {
                    ContentPart textPart = new TextContentPart(partWrapper.ToString());
                    token = JToken.FromObject(textPart, serializer);
                }
                else
                {
                    var part = partWrapper.ToPart();
                    if (part == null)
                    {
                        AIDevKitDebug.Warning("[CreateThreadMessageContentJToken] Skipping part with null ContentPart.");
                        continue;
                    }
                    token = JToken.FromObject(part, serializer);
                }

                array.Add(token);
            }

            return array;
        }
    }
}