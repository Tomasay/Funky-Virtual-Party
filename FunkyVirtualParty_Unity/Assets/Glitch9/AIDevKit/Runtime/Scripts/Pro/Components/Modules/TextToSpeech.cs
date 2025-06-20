using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Text To Speech")]
    public class TextToSpeech : AIModuleComponent
    {
        [SerializeField] private string voice;
        [SerializeField] private UnityEvent<AudioClip> onAudioClipGenerated;
        [SerializeField] private bool playOnGeneration = false;

        public Voice Voice { get => voice; set => voice = value; }
        public AudioSource AudioSource
        {
            get
            {
                if (_audioSource == null)
                {
                    _audioSource = GetComponent<AudioSource>();
                    if (_audioSource == null)
                        _audioSource = gameObject.AddComponent<AudioSource>();
                    _audioSource.playOnAwake = false; // Prevent auto-play on Awake
                }
                return _audioSource;
            }
        }
        private AudioSource _audioSource;

        public async void GenerateSpeech(string text) => await GenerateSpeechAsync(text);
        public UniTask<GeneratedAudio> GenerateSpeechAsync(string text) => GenerateSpeechAsync(text, null);
        public async UniTask<GeneratedAudio> GenerateSpeechAsync(string text, Voice voice)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                OnError("Prompt text cannot be null or empty.");
                return null;
            }

            if (IsBusy)
            {
                OnError("TextToSpeech is already processing another request.");
                return null;
            }

            IsBusy = true;
            try
            {
                if (voice == null) voice = this.voice;

                GeneratedAudio generatedAudio = await text.GENSpeech()
                    .SetModel(model)
                    .SetVoice(voice)
                    .SetOutputPath(outputPath)
                    .ExecuteAsync()
                    ?? throw new EmptyResponseException("Failed to generate audio from text.");

                onAudioClipGenerated?.Invoke(generatedAudio);

                if (playOnGeneration)
                {
                    AudioSource.clip = generatedAudio;
                    AudioSource.Play();
                }

                return generatedAudio;
            }
            catch (System.Exception ex)
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