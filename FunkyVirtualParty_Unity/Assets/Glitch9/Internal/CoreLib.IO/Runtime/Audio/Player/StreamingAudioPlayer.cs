using UnityEngine;
using System.Collections.Concurrent;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Glitch9.CoreLib.IO.Audio
{
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Glitch9/Audio/Streaming Audio Player")]
    public class StreamingAudioPlayer : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float volume = 1.0f;
        [SerializeField, Range(5, 20)] private int initialBufferSize = 5; // Number of chunks to buffer before starting playback
        [SerializeField] private SampleRate inputSampleRate = SampleRate.Hz16000; // Default sample rate 

        public UnityEvent onStreamingStarted; // Event to notify when streaming starts
        public UnityEvent onStreamingStopped; // Event to notify when streaming stops 

        private AudioSource _audioSource;
        private int _currentPosition = 0;
        private readonly ConcurrentQueue<float[]> _audioQueue = new(); // Queue for received audio data
        private List<float> _pushedAudioData; // Buffer for audio data pushed to the queue
        private float[] _currentAudioData; // Buffer for current audio data being played   
        private bool _stopAudioSourcecRequested = false; // Flag to indicate if stop is requested
        private bool _requiresResampling = false;
        private bool _hasStartedPlayback = false;

        public SampleRate InputSampleRate
        {
            get => inputSampleRate;
            set
            {
                inputSampleRate = value;
                _requiresResampling = AudioSettings.outputSampleRate != (int)value;
                Debug.Log($"Input sample rate set to {inputSampleRate}. Resampling required: {_requiresResampling}");
            }
        }

        void Awake()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
            ReConfigure();
            // _audioSource.Play(); // You need to play the audio source to start the audio filter

            int unitySampleRate = AudioSettings.outputSampleRate;
            int inputSampleRate = (int)this.inputSampleRate;
            _requiresResampling = unitySampleRate != inputSampleRate;
            Debug.Log($"Unity sample rate: {unitySampleRate}, Input sample rate: {inputSampleRate}. Resampling required: {_requiresResampling}");
        }

        /// <summary>
        /// Configure the audio streamer with new settings
        /// </summary>
        /// <param name="volume"></param>
        public void Configure(float volume, SampleRate inputSampleRate = SampleRate.Hz16000)
        {
            this.volume = volume;
            InputSampleRate = inputSampleRate; // Set the input sample rate
            ReConfigure();
        }

        private void ReConfigure()
        {
            if (volume < 0.0f || volume > 1.0f) volume = 1.0f;
            if (_audioSource != null) _audioSource.volume = volume;
        }

        /// <summary>
        /// Receive streamed audio data(delta) from the server (WebSockets)
        /// </summary>
        /// <param name="samples"></param>
        public void PushSamples(float[] samples)
        {
            if (samples == null || samples.Length == 0) return;

            // Resample the audio data to match Unity's sample rate
            if (_requiresResampling)
                samples = AudioProcessor.ResamplePCM16Linear(samples, (int)inputSampleRate, AudioSettings.outputSampleRate);

            const int minLength = 1024; // Minimum length for audio data to avoid crackling noise

            // Ensure the audio data is at least minLength to prevent crackling noise
            if (samples.Length < minLength)
            {
                Debug.LogWarning($"Received audio data length {samples.Length} is less than minimum required length {minLength}. Buffering samples.");

                _pushedAudioData ??= new();

                // Add the samples to the buffer
                _pushedAudioData.AddRange(samples);

                // check if we have enough data to push to the queue
                if (_pushedAudioData.Count >= minLength)
                {
                    // Convert the buffer to an array and clear it
                    samples = _pushedAudioData.ToArray();
                    _pushedAudioData.Clear();
                }
                else
                {
                    // Not enough data yet, just return
                    return;
                }
            }

            if (!_hasStartedPlayback && _audioQueue.Count >= initialBufferSize)
            {
                _audioSource.Play();
                _hasStartedPlayback = true;
            }

            _audioQueue.Enqueue(samples);
        }


        public float[] FetchCurrentAudioData()
        {
            return _currentAudioData;
        }

        public void PushStart()
        {
            onStreamingStarted?.Invoke();
        }

        public void PushDone()
        {
            onStreamingStopped?.Invoke();
        }

        private void Update()
        {
            if (_stopAudioSourcecRequested)
            {
                if (_audioSource.isPlaying)
                    _audioSource.Stop();

                _stopAudioSourcecRequested = false;
                _hasStartedPlayback = false; // Reset playback state
                Debug.Log("No more audio data available, stopping playback.");
            }
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            // If we don't have any current audio data, try to dequeue the next available audio data
            if (_currentAudioData == null || _currentAudioData.Length == 0)
            {
                if (_audioQueue.TryDequeue(out float[] nextAudioData))
                {
                    _currentAudioData = nextAudioData;
                    _currentPosition = 0; // Reset position for new data   
                }
                else
                {
                    // If there's no audio data available in the queue, fill with silence 
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = 0;
                    }
                    return;
                }
            }

            // 현재 오디오데이터가 Crackling Noise를 방지하기 위해 충분한 길이를 가지고 있는지 확인합니다.
            if (_currentAudioData.Length == 0 || _currentPosition >= _currentAudioData.Length)
            {
                // Fill with silence if no audio data is available
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = 0;
                }
                return;
            }

            // Fill the data array with audio samples from the current audio data
            int length = Mathf.Min(data.Length, _currentAudioData.Length - _currentPosition);

            for (int i = 0; i < length; i++)
                data[i] = _currentAudioData[_currentPosition + i];

            _currentPosition += length;

            // If we've finished the current audio data, dequeue the next buffer
            if (_currentPosition >= _currentAudioData.Length)
            {
                if (_audioQueue.TryDequeue(out float[] nextAudioData))
                {
                    _currentAudioData = nextAudioData;
                    _currentPosition = 0;
                }
                else
                {
                    // No more audio data available, reset current data
                    _currentAudioData = null;
                    _stopAudioSourcecRequested = true; // Request to stop the audio source
                }
            }
        }
    }
}
