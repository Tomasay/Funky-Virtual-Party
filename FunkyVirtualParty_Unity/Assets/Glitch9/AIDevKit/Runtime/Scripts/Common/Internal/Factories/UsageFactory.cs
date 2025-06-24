using System;

namespace Glitch9.AIDevKit
{
    internal static class UsageFactory
    {
        internal static Usage Create(string modelId, UsageType type, int count)
        {
            return new Usage()
            {
                modelId = modelId,
                usages = { [type] = count }
            };
        }

        internal static Usage Empty() => new();
        internal static Usage Free() => new() { usages = { [UsageType.Free] = 0 } };
        internal static Usage PerMinute(double cost) => new() { usages = { [UsageType.PerMinute] = (int)cost } };
        internal static Usage PerCharacter(double cost) => new() { usages = { [UsageType.PerCharacter] = (int)cost } };
        internal static Usage Image(string modelId, ImageSize size, ImageQuality quality, int count)
        {
            UsageType usageType = ResolveImageUsageType(size, quality);
            return Create(modelId, usageType, count);
        }

        private static UsageType ResolveImageUsageType(ImageSize size, ImageQuality quality)
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
    }
}