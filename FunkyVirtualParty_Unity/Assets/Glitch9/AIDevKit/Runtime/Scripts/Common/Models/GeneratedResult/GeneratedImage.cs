using System;
using System.Collections.Generic;
using System.Linq;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Represents a generated image or a collection of generated images.
    /// This class provides implicit conversions to Texture2D and Sprite types for easy usage.
    /// </summary>
    public class GeneratedImage : GeneratedFile<Texture2D, File<Texture2D>>
    {
        public static implicit operator Texture2D(GeneratedImage generatedImage) => generatedImage?.values?.FirstOrDefault();
        public static implicit operator Texture2D[](GeneratedImage generatedImage) => generatedImage?.values;
        public static implicit operator Sprite(GeneratedImage generatedImage) => generatedImage?.values[0].ToSprite();
        public static implicit operator Sprite[](GeneratedImage generatedImage) => Array.ConvertAll(generatedImage?.values, t => t.ToSprite());
        public static implicit operator File<Texture2D>(GeneratedImage generatedImage) => generatedImage?.ToFiles()?.FirstOrDefault();
        public static implicit operator File<Texture2D>[](GeneratedImage generatedImage) => generatedImage?.ToFiles();

        internal GeneratedImage(Texture2D texture, string path, Usage usage = null) : base(texture, path, usage) { }
        internal GeneratedImage(Texture2D[] textures, string[] paths, Usage usage = null) : base(textures, paths, usage) { }

        public override File<Texture2D>[] ToFiles()
        {
            if (values.Length == 0) return null;
            List<File<Texture2D>> files = new(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                string path = Paths[i];
                Texture2D texture = values[i];
                if (texture == null) continue;
                files.Add(new File<Texture2D>(asset: texture, filePath: path, note: fileNote));
            }
            return files.ToArray();
        }
    }
}