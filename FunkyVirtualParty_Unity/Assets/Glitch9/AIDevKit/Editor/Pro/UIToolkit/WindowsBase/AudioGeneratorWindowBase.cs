using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.Editor.UIToolkit;
using Glitch9.IO.Files;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal abstract class AudioGeneratorWindowBase<TSelf, TSettings> : FileGeneratorWindowBase<TSelf, TSettings, File<AudioClip>>
        where TSelf : AudioGeneratorWindowBase<TSelf, TSettings>
        where TSettings : AudioGeneratorSettings<TSelf, TSettings>, new()
    {
        protected AudioEncoding? Encoding { get => Settings.Encoding.Value; set => Settings.Encoding.Value = value; }
        protected ElevenLabsOutputFormat? ElevenLabsFormat { get => Settings.ElevenLabsFormat.Value; set => Settings.ElevenLabsFormat.Value = value; }

        protected abstract void DesignGridViewBox(VisualElement box, File<AudioClip> file, int i);

        protected override VisualElement CreateGridViewItemINTERNAL(File<AudioClip> file, int i)
        {
            if (currentRecord == null || file == null || file.Asset == null)
            {
                return GeneratorUtil.CreateGridViewEmptyItem("No content available #2 (Error)");
            }

            VisualElement box = new();
            box.AddToClassList("preview-image");

            DesignGridViewBox(box, file, i); // Custom design for the grid view box

            file.EnsureAssetLoaded();
            AudioClip myClip = file.Asset;

            var previewPlayer = new AudioPlayerElement(myClip, myClip.length);
            box.Add(previewPlayer);

            return box;
        }
    }
}