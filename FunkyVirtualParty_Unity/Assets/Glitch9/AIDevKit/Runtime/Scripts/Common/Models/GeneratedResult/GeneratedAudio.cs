using System.Collections.Generic;
using System.Linq;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Represents a generated audio or a collection of generated audios.
    /// This class provides implicit conversions to AudioClip and Sprite types for easy usage.
    /// </summary>
    public class GeneratedAudio : GeneratedFile<AudioClip, File<AudioClip>>
    {
        public static implicit operator AudioClip(GeneratedAudio generatedAudio) => generatedAudio?.values?.FirstOrDefault();
        public static implicit operator AudioClip[](GeneratedAudio generatedAudio) => generatedAudio?.values;
        public static implicit operator File<AudioClip>(GeneratedAudio generatedAudio) => generatedAudio?.ToFiles()?.FirstOrDefault();
        public static implicit operator File<AudioClip>[](GeneratedAudio generatedAudio) => generatedAudio?.ToFiles();

        internal GeneratedAudio(AudioClip audioClip, string path, Usage usage = null) : base(audioClip, path, usage) { }
        internal GeneratedAudio(AudioClip[] audioClip, string[] paths, Usage usage = null) : base(audioClip, paths, usage) { }

        public override File<AudioClip>[] ToFiles()
        {
            List<File<AudioClip>> files = new(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                string path = Paths[i];
                AudioClip audioClip = values[i];
                if (audioClip == null) continue;
                files.Add(new File<AudioClip>(asset: audioClip, filePath: path, note: fileNote));
            }
            return files.ToArray();
        }
    }
}