using UnityEngine;
using System.Collections.Generic;

namespace Glitch9.CoreLib.IO.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class StreamingAudioPlayer : MonoBehaviour
    {
        public int sampleRate = 44100;
        public int channels = 1;

        private AudioSource _audioSource;
        private AudioClip _streamClip;
        private Queue<float> _sampleQueue = new();
        private readonly object _lock = new();

        private const int kClipLengthSeconds = 10; // 내부 버퍼용 길이

        void Start()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
            _streamClip = AudioClip.Create("StreamClip", sampleRate * kClipLengthSeconds, channels, sampleRate, true, OnAudioRead, OnAudioSetPosition);
            _audioSource.clip = _streamClip;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        /// <summary> 
        /// Push PCM data from external source (float[-1~1] range)
        /// </summary>
        public void PushSamples(float[] samples)
        {
            lock (_lock)
            {
                foreach (var sample in samples)
                {
                    _sampleQueue.Enqueue(sample);
                }
            }
        }

        /// <summary> 
        /// Called when AudioClip requests samples
        /// </summary>
        private void OnAudioRead(float[] data)
        {
            lock (_lock)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = _sampleQueue.Count > 0 ? _sampleQueue.Dequeue() : 0f; // 부족하면 무음으로 채움
                }
            }
        }

        private void OnAudioSetPosition(int newPosition)
        {
            // Not used (can be implemented for seeking if needed)
        }
    }
}