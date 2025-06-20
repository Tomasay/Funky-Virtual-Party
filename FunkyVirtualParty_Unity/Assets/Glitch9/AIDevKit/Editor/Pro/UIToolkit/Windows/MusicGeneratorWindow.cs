using System;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Advanced.Lyria;
using Glitch9.Editor;
using Glitch9.IO.Files;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class MusicGeneratorWindow : AudioGeneratorWindowBase<MusicGeneratorWindow, MusicGeneratorSettings<MusicGeneratorWindow>>
    {
        // public MusicGenre Genre { get => Settings.Genre.Value; set => Settings.Genre.Value = value; }
        // public Mood Mood { get => Settings.Mood.Value; set => Settings.Mood.Value = value; }
        // public InstrumentHint Instrument { get => Settings.Instrument.Value; set => Settings.Instrument.Value = value; }

        private int Bpm { get => Settings.Bpm.Value; set => Settings.Bpm.Value = value; }
        private float Temperature { get => Settings.Temperature.Value; set => Settings.Temperature.Value = value; }
        private float Density { get => Settings.Density.Value; set => Settings.Density.Value = value; }
        private float Brightness { get => Settings.Brightness.Value; set => Settings.Brightness.Value = value; }
        private MusicScale Scale { get => Settings.Scale.Value; set => Settings.Scale.Value = value; }
        private LyriaSessionController _sessionController;
        private LyriaSessionController SessionController => _sessionController ??= new();
        private EditorRealtimeAudioPlayer _player;
        private EditorRealtimeAudioPlayer Player => _player ??= new EditorRealtimeAudioPlayer();

        protected string FormatPrompt(string prompt)
        {
            // const string kPromptFormat = "Generate a {genre} music track in {mood} style with {instrumentHint}: {prompt}";

            // return kPromptFormat
            //     .Replace("{genre}", Genre.FormatEnum())            // 예: Genre enum 값 사용
            //     .Replace("{mood}", Mood.FormatEnum())              // 예: Mood enum 값 사용
            //     .Replace("{instrumentHint}", Instrument.FormatEnum()) // 예: 악기 힌트 enum 사용
            //     .Replace("{prompt}", prompt)
            //     .Replace("  ", " ");

            return prompt; // 현재는 단순히 프롬프트를 그대로 반환
        }

        protected override void DrawIMGUIGenerationSettings()
        {

        }

        protected override void DrawIMGUIApiParameters()
        {



            Bpm = EditorGUILayout.IntSlider("BPM", Bpm, 60, 200);
            Temperature = EditorGUILayout.Slider("Temperature", Temperature, 0.0f, 3.0f);
            Density = EditorGUILayout.Slider("Density", Density, 0.0f, 1.0f);
            Brightness = EditorGUILayout.Slider("Brightness", Brightness, 0.0f, 1.0f);
            Scale = (MusicScale)EditorGUILayout.EnumPopup("Scale", Scale);

        }

        protected override async UniTask GenerateContentAsync(string prompt)
        {

            if (SessionController == null)
            {
                Debug.LogError("SessionController is not initialized.");
                return;
            }

            // skip formatting the prompt for now
            // prompt = FormatPrompt(prompt); 
            Debug.Log($"Generating music with prompt: {prompt}");

            // Start the generation process
            var generationConfig = new MusicGenerationConfig()
            {
                BPM = Bpm,
                Temperature = Temperature,
                Density = Density,
                Brightness = Brightness,
                Scale = Scale
            };

            await SessionController.ConnectAsync();
            if (SessionController.IsConnected)
            {
                IsGenerating = true;
                await SessionController.GenerateMusicAsync(prompt, OnReceiveAudio, generationConfig);
                // if (response != null)
                // {
                //     // Handle the response, e.g., save the generated music file
                //     Debug.Log("Music generation completed successfully.");
                //     // Save or process the generated music file here
                // }
                // else
                // {
                //     Debug.LogError("Music generation failed: No response received.");
                // }
            }
            else
            {
                Debug.LogError("Failed to connect to Lyria session.");
            }
        }

        private void OnReceiveAudio(float[] audioData)
        {

            if (audioData == null || audioData.Length == 0)
            {
                Debug.LogWarning("Received empty audio data.");
                return;
            }
            // Handle the received audio data, e.g., play it or save it
            Debug.Log($"Received audio data with length: {audioData.Length}");
            // You can implement audio playback or saving logic here

            if (Player == null)
            {
                Debug.LogError("Audio player is not initialized.");
                return;
            }

            try
            {
                Player.Push(audioData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to play audio: {ex.Message}");
            }
        }

        protected override void OnClickUseThisPromptButton()
        {
            throw new System.NotImplementedException();
        }

        protected override void DesignGridViewBox(VisualElement box, File<AudioClip> file, int i)
        {
        }
    }
}
