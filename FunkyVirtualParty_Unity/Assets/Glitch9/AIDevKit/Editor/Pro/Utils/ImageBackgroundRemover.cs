using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Pro
{
    internal static class ImageBackgroundRemover
    {
        internal static Texture2D RemoveBackground(Texture2D source, float threshold = 0.1f)
        {
            Texture2D tex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            tex.SetPixels(source.GetPixels());

            Color[] pixels = tex.GetPixels();
            Color bgColor = GetMostFrequentColor(pixels);

            for (int i = 0; i < pixels.Length; i++)
            {
                if (IsSimilar(pixels[i], bgColor, threshold))
                {
                    pixels[i] = new Color(0, 0, 0, 0); // 투명
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static bool IsSimilar(Color a, Color b, float threshold)
        {
            return Vector3.Distance(new Vector3(a.r, a.g, a.b), new Vector3(b.r, b.g, b.b)) < threshold;
        }

        private static Color GetMostFrequentColor(Color[] pixels)
        {
            Dictionary<Color32, int> histogram = new();

            foreach (var pixel in pixels)
            {
                Color32 c = pixel;
                if (histogram.ContainsKey(c))
                    histogram[c]++;
                else
                    histogram[c] = 1;
            }

            return histogram.OrderByDescending(kvp => kvp.Value).First().Key;
        }
    }
}