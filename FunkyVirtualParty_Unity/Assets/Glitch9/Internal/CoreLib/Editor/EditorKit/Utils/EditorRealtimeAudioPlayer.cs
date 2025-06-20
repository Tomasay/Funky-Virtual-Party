using UnityEngine;

namespace Glitch9.Editor
{
    public class EditorRealtimeAudioPlayer
    {
        private readonly AudioClip clip;
        private const int sampleRate = 48000;
        private const int channels = 2;
        private const int bufferLength = 48000; // 1초

        private readonly float[] buffer = new float[bufferLength];
        private int writePosition = 0;

        public EditorRealtimeAudioPlayer()
        {
            clip = AudioClip.Create("RealtimeClip", bufferLength, channels, sampleRate, false);
        }

        public void Push(float[] samples)
        {
            int length = Mathf.Min(samples.Length, buffer.Length - writePosition);
            System.Array.Copy(samples, 0, buffer, writePosition, length);

            // write to AudioClip (if playing in Editor or PlayMode)
            clip.SetData(buffer, 0);
            writePosition = (writePosition + length) % buffer.Length;

            // If the buffer is full, reset the write position to avoid overflow
            if (writePosition >= buffer.Length)
            {
                writePosition = 0;
            }

            // Play the clip if not already playing
            // if (!AudioSettings.GetAudioConfiguration().dspBufferSize.Equals(bufferLength))
            // {
            //     AudioSettings.Reset(new AudioConfiguration
            //     {
            //         sampleRate = sampleRate,
            //         dspBufferSize = bufferLength,
            //         numRealVoices = 1,
            //         numVirtualVoices = 0,
            //         speakerMode = AudioSettings.speakerMode
            //     });
            // }

            EditorAudioPlayer.Play(clip);
        }
    }
}