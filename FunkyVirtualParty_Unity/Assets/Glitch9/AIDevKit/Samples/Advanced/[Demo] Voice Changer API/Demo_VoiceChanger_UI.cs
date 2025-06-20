using System.Collections.Generic;
using System.Linq;
using Glitch9.AIDevKit.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class Demo_VoiceChanger_UI : MonoBehaviour
    {
        private static class Texts
        {
            internal const string kStartRecording = "Hold Tab to start recording";
            internal const string kStopRecording = "Release Tab to stop recording";
        }

        [SerializeField] private Text inscructionsText;
        [SerializeField] private VoiceChanger module;
        [SerializeField] private Dropdown voiceDropdown;
        [SerializeField] private Dropdown micDropdown;

        private Color _colorDefault = ExColor.amber;
        private Color _colorRecording = ExColor.aquamarine;
        private Dictionary<string, Voice> _voiceMap = new();

        private void Start()
        {
            // Null check all inspector fields
            if (module == null || voiceDropdown == null || micDropdown == null || inscructionsText == null)
            {
                Debug.LogError("Initialization failed: Inspector fields are not set");
                return;
            }

            List<Voice> voices = VoiceLibrary.GetVoicesByAPI(Api.ElevenLabs);
            if (voices.Count == 0)
            {
                Debug.LogError("No voices found for ElevenLabs API. Please check your voice library.");
                return;
            }

            foreach (var voice in voices)
            {
                _voiceMap.Add(voice.Name, voice);
            }

            micDropdown.AddOptions(DemoUtil.StringArrayToOptionData(Microphone.devices));
            voiceDropdown.AddOptions(DemoUtil.StringArrayToOptionData(_voiceMap.Keys.ToArray()));
        }

        private void Update()
        {
            if (module == null) return;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!module.IsRecording)
                {
                    module.Microphone = micDropdown.options[micDropdown.value].text;
                    module.StartRecording();
                    inscructionsText.text = Texts.kStopRecording;
                    inscructionsText.color = _colorRecording;
                }
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                if (module.IsRecording)
                {
                    StopRecording();
                    inscructionsText.text = Texts.kStartRecording;
                    inscructionsText.color = _colorDefault;
                }
            }
        }

        private async void StopRecording()
        {
            var voiceName = voiceDropdown.options[voiceDropdown.value].text;
            if (!_voiceMap.TryGetValue(voiceName, out var voice))
            {
                Debug.LogError($"Voice {voiceName} not found. Please check your voice library.");
                return;
            }

            var result = await module.StopRecording();

            if (result == null)
            {
                Debug.LogError("Voice change failed.");
                return;
            }

            AudioSource.PlayClipAtPoint(result, Vector3.zero);
        }
    }
}