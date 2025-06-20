using System;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Speech To Text")]
    public class SpeechToText : AudioInputComponent<Transcript>
    {
        [SerializeField] private bool removeBackgroundNoise = true;
        [SerializeField] private SystemLanguage spokenLanguage = SystemLanguage.English;
        [SerializeField] private UnityEvent<string> onTextGenerated;

        /// <summary>
        /// Event listener for advanced users to handle the generated transcript.
        /// This event is triggered when a transcript is successfully generated from the recorded audio.
        /// </summary>
        public UnityEvent<Transcript> onTranscriptGenerated;

        public SystemLanguage SpokenLanguage { get => spokenLanguage; set => spokenLanguage = value; }
        public bool RemoveBackgroundNoise { get => removeBackgroundNoise; set => removeBackgroundNoise = value; }

        protected override async UniTask<Transcript> OnAudioRecorded(AudioClip recordedClip)
        {
            if (recordedClip == null)
            {
                OnError("No audio data recorded.");
                return null;
            }

            if (IsBusy)
            {
                OnError("SpeechToText is already processing another request.");
                return null;
            }

            if (AIDevKitDebug.kDebugMode.Value)
            {
                Debug.Log($"Transcribing with model: {model}");
                Debug.Log($"Transcribing with language: {spokenLanguage}");
            }

            IsBusy = true;
            try
            {
                Transcript transcript = await recordedClip
                    .GENTranscript()
                    .SetModel(model)
                    .SetLanguage(spokenLanguage)
                    .ExecuteAsync()
                    ?? throw new EmptyResponseException("Failed to generate transcript from audio.");

                onTextGenerated?.Invoke(transcript);
                onTranscriptGenerated?.Invoke(transcript);
                return transcript;
            }
            catch (Exception ex)
            {
                OnError(ex);
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}