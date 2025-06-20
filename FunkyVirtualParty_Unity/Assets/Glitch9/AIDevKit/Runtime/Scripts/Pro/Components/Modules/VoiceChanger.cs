using System;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Voice Changer")]
    public class VoiceChanger : AudioInputComponent<AudioClip>
    {
        [SerializeField] private bool removeBackgroundNoise = true;
        [SerializeField] private UnityEvent<AudioClip> onAudioClipGenerated;
        public bool RemoveBackgroundNoise { get => removeBackgroundNoise; set => removeBackgroundNoise = value; }

        protected override async UniTask<AudioClip> OnAudioRecorded(AudioClip recordedClip)
        {
            if (recordedClip == null)
            {
                OnError("No audio data recorded.");
                return null;
            }

            if (IsBusy)
            {
                OnError("VoiceChanger is already processing another request.");
                return null;
            }

            if (AIDevKitDebug.kDebugMode.Value)
            {
                Debug.Log($"Transcribing with model: {model}");
            }

            IsBusy = true;
            try
            {
                GeneratedAudio audio = await recordedClip
                    .GENVoiceChange()
                    .SetModel(model)
                    .RemoveBackgroundNoise(removeBackgroundNoise)
                    .ExecuteAsync()
                    ?? throw new EmptyResponseException("Failed to generate audio from voice change.");

                onAudioClipGenerated?.Invoke(audio);
                return audio;
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