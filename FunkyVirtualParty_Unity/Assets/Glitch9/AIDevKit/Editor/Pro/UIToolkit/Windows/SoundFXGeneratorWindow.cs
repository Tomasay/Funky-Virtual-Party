using System;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.Editor;
using Glitch9.IO.Files;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class SoundFXGeneratorWindow : AudioGeneratorWindowBase<SoundFXGeneratorWindow, SoundFXGeneratorSettings<SoundFXGeneratorWindow>>
    {
        protected double? Duration { get => Settings.Duration.Value; set => Settings.Duration.Value = value; }
        protected double? PromptInfluence { get => Settings.PromptInfluence.Value; set => Settings.PromptInfluence.Value = value; }

        protected override void DrawIMGUIGenerationSettings()
        {
            ExGUILayout.BoxedLabel("SoundFX doesn't have a model selection");
            DrawIMGUINSlider();
        }

        protected override void DrawIMGUIApiParameters()
        {
            Duration = ExGUILayout.NullableDoubleSlider(
                GUIContents.SoundFXDuration,
                Duration,
                3.0f,
                0.5f,
                10.0f
            );

            PromptInfluence = ExGUILayout.NullableDoubleSlider(
                GUIContents.PromptInfluence,
                PromptInfluence,
                0.5f,
                0.0f,
                1.0f
            );

            ElevenLabsFormat = ExGUILayout.NullableField(
                ElevenLabsFormat,
                ElevenLabsOutputFormat.MP3_44100_32,
                (v) => (ElevenLabsOutputFormat)EditorGUILayout.EnumPopup("Format", v)
            );
        }

        protected override void DesignGridViewBox(VisualElement box, File<AudioClip> file, int i)
        {
            // use .grid-view-box-title and .grid-view-box-description for styling 
            var title = new Label("Sound Effect");
            title.AddToClassList(GeneratorStyles.Class_GridViewBoxLabelTitle);
            box.Add(title);

            // description should be the prompt text
            var description = new Label(currentRecord?.Prompt ?? "No prompt available");
            description.AddToClassList(GeneratorStyles.Class_GridViewBoxLabelContent);
            box.Add(description);
        }

        protected override async UniTask GenerateContentAsync(string prompt)
        {
            GENSoundEffectTask task = prompt.GENSoundEffect()
                   .SetSender(windowName)
                   .SetOutputPath(AIDevKitSettings.OutputPath);

            if (Duration.HasValue)
            {
                task.SetDuration(Duration.Value);
            }

            if (PromptInfluence.HasValue)
            {
                task.SetPromptInfluence(PromptInfluence.Value);
            }

            if (ElevenLabsFormat.HasValue)
            {
                task.SetOutputFormat(ElevenLabsFormat.Value);
            }

            await task.ExecuteAsync();
        }

        protected override void OnClickUseThisPromptButton() => throw new NotImplementedException();
    }
}