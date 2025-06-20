using UnityEngine;
using System.Collections.Concurrent;
using System;

namespace Glitch9.CoreLib.IO.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class RealtimeAudioPlayer : MonoBehaviour
    {
        [SerializeField] private float volume = 1.0f;
        private AudioSource _audioSource;
        private int _currentPosition = 0;
        private ConcurrentQueue<float[]> _audioQueue = new(); // Queue for received audio data
        private float[] currentAudioData; // Buffer for current audio data being played 

        void Awake()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
            ReConfigure();
            _audioSource.Play(); // You need to play the audio source to start the audio filter
        }

        /// <summary>
        /// Configure the audio streamer with new settings
        /// </summary>
        /// <param name="volume"></param>
        public void Configure(float volume)
        {
            this.volume = volume;
            ReConfigure();
        }

        private void ReConfigure()
        {
            if (volume < 0.0f || volume > 1.0f) volume = 1.0f;
            _audioSource.volume = volume;
        }

        /// <summary>
        /// Receive streamed audio data(delta) from the server (WebSockets)
        /// </summary>
        /// <param name="newData"></param>
        public void SetAudioData(float[] newData)
        {
            _audioQueue.Enqueue(newData);
        }

        public float[] GetAudioData()
        {
            return currentAudioData;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            // If we don't have any current audio data, try to dequeue the next available audio data
            if (currentAudioData == null || currentAudioData.Length == 0)
            {
                if (_audioQueue.TryDequeue(out float[] nextAudioData))
                {
                    currentAudioData = nextAudioData;
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

            // Fill the data array with audio samples from the current audio data
            int length = Mathf.Min(data.Length, currentAudioData.Length - _currentPosition);

            for (int i = 0; i < length; i++)
            {
                data[i] = currentAudioData[_currentPosition + i];
            }

            _currentPosition += length;

            // If we've finished the current audio data, dequeue the next buffer
            if (_currentPosition >= currentAudioData.Length)
            {
                if (_audioQueue.TryDequeue(out float[] nextAudioData))
                {
                    currentAudioData = nextAudioData;
                    _currentPosition = 0;
                }
                else
                {
                    // No more audio data available, reset current data
                    currentAudioData = null;
                }
            }
        }

        internal void PushSamples(float[] obj)
        {
            throw new NotImplementedException();
        }
    }
}
