using System;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.Editor;
using Glitch9.IO.Files;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class SpeechGeneratorWindow : AudioGeneratorWindowBase<SpeechGeneratorWindow, SpeechGeneratorSettings<SpeechGeneratorWindow>>
    {
        private Voice _voice;
        protected Voice Voice
        {
            get
            {
                if (_voice == null)
                {
                    if (Model != null)
                    {
                        if (Model.Api == Api.OpenAI)
                        {
                            _voice = OpenAISettings.DefaultVoice;
                        }
                        else if (Model.Api == Api.ElevenLabs)
                        {
                            _voice = ElevenLabsSettings.DefaultVoice;
                        }
                    }

                    if (_voice == null)
                        _voice = AIDevKitConfig.kDefault_OpenAI_Voice;

                    if (_voice == null)
                        ShowDialog.Error("No default voice found for the selected model. Please set a default voice in AIDevKit ");
                }
                return _voice;
            }
            set => _voice = value;
        }
        protected float? Speed { get => Settings.Speed.Value; set => Settings.Speed.Value = value; }
        protected uint? Seed { get => Settings.Seed.Value; set => Settings.Seed.Value = value; }

        protected override void DrawIMGUIGenerationSettings()
        {
            Model = AIDevKitGUI.TTSPopup(Model, label: GUIContents.Model, apiWidth: 80);
            DrawIMGUINSlider();
        }

        protected override void DrawIMGUIApiParameters()
        {
            if (Model == null) return;

            if (Model.Api == Api.OpenAI)
            {
                Voice = AIDevKitGUI.VoicePopup(Voice, Api.OpenAI, GUIContents.Voice, apiWidth: 80);
                Speed = ExGUILayout.NullableField("Speed", Speed, 1f, v => EditorGUILayout.Slider(v, 0.5f, 2.0f));
                Encoding = ExGUILayout.NullableField(
                    Encoding,
                    AudioEncoding.MP3,
                    (v) => ExGUILayout.EnumPopupEx(
                        label: "Format",
                        selected: v,
                        displayedOptions: AIDevKitConfig.AudioEncodingOptions)
                );
            }
            else if (Model.Api == Api.ElevenLabs)
            {
                Voice = AIDevKitGUI.VoicePopup(Voice, Api.ElevenLabs, GUIContents.Voice, apiWidth: 80);
                Speed = ExGUILayout.NullableField(Speed, 1f, v => EditorGUILayout.Slider("Speed", v, 0.5f, 2.0f));

                int seedAsInt = Seed.HasValue ? (int)Seed : 0;
                int? newSeed = ExGUILayout.NullableField(seedAsInt, 0, v => EditorGUILayout.IntField("Seed", v));

                if (newSeed == null && Seed.HasValue)
                {
                    Seed = null;
                }
                else if (newSeed != Seed)
                {
                    Seed = (uint)newSeed;
                }

                ElevenLabsFormat = ExGUILayout.NullableField(
                    ElevenLabsFormat,
                    ElevenLabsOutputFormat.MP3_44100_32,
                    (v) => (ElevenLabsOutputFormat)EditorGUILayout.EnumPopup("Format", v)
                );
            }
            else if (Model.Api == Api.Google)
            {
                Voice = AIDevKitGUI.VoicePopup(Voice, Api.Google, GUIContents.Voice, apiWidth: 80);
                GUILayout.Space(5);
                EditorGUILayout.HelpBox("Google's TTS is in 'Preview' stage and may not be available in all regions. " +
                                        "Please refer to the official documentation for more information.", MessageType.Info);

                if (GUILayout.Button("Open Google TTS Documentation"))
                {
                    Application.OpenURL("https://ai.google.dev/gemini-api/docs/speech-generation");
                }
            }
        }

        protected override void DesignGridViewBox(VisualElement box, File<AudioClip> file, int i)
        {
            // use .grid-view-box-title and .grid-view-box-description for styling
            string voiceName = file.Note;
            if (string.IsNullOrEmpty(voiceName)) voiceName = currentRecord?.GetMetadata("voice");
            if (string.IsNullOrEmpty(voiceName)) voiceName = "Unknown Voice";
            var title = new Label(voiceName);
            // note is the voice actor's name
            title.AddToClassList(GeneratorStyles.Class_GridViewBoxLabelTitle);
            box.Add(title);

            // description should be the prompt text
            var description = new Label(currentRecord?.Prompt ?? "No prompt available");
            description.AddToClassList(GeneratorStyles.Class_GridViewBoxLabelContent);
            box.Add(description);
        }

        protected override async UniTask GenerateContentAsync(string prompt)
        {
            GENSpeechTask task = prompt.GENSpeech()
                  .SetModel(Model)
                  .SetVoice(Voice)
                  .SetSender(windowName)
                  .SetOutputPath(AIDevKitSettings.OutputPath, Voice.Name);

            if (Speed.HasValue)
            {
                task.SetSpeed(Speed.Value);
            }

            if (Model.Api == Api.OpenAI)
            {
                if (Encoding.HasValue)
                {
                    task.SetEncoding(Encoding.Value);
                }
            }
            else if (Model.Api == Api.ElevenLabs)
            {
                if (Seed.HasValue)
                {
                    task.SetSeed(Seed.Value);
                }

                if (ElevenLabsFormat.HasValue)
                {
                    task.SetOutputFormat(ElevenLabsFormat.Value);
                }
            }

            await task.ExecuteAsync();
        }

        protected override void OnClickUseThisPromptButton() => throw new NotImplementedException();
    }
}