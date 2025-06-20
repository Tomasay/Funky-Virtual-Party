
using Glitch9.AIDevKit.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class Demo_SpeechToText_UI : MonoBehaviour
    {
        private static class Texts
        {
            internal const string kStartRecording = "Hold Tab to start recording";
            internal const string kStopRecording = "Release Tab to stop recording";
        }

        [SerializeField] private Text inscructionsText;
        [SerializeField] private Text transcriptText;
        [SerializeField] private SpeechToText module;
        [SerializeField] private Dropdown micDropdown;

        private Color _colorDefault = ExColor.amber;
        private Color _colorRecording = ExColor.aquamarine;

        private void Start()
        {
            // Null check all inspector fields
            if (module == null || transcriptText == null || micDropdown == null || inscructionsText == null)
            {
                Debug.LogError("Initialization failed: Inspector fields are not set");
                return;
            }

            micDropdown.AddOptions(DemoUtil.StringArrayToOptionData(Microphone.devices));
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
            var transcript = await module.StopRecording();
            if (transcript != null)
            {
                transcriptText.text = transcript;
                Debug.Log($"Transcript: {transcript}");
            }
        }
    }
}