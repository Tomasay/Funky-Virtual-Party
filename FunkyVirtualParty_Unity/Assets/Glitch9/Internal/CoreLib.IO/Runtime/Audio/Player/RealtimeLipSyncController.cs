using UnityEngine;

namespace Glitch9.CoreLib.IO.Audio
{
    public class RealtimeLipSyncController
    {
        public float GetRMS(float[] data)
        {
            float sumSquares = 0f;

            for (int i = 0; i < data.Length; i++)
            {
                float sample = data[i];
                sumSquares += sample * sample;
            }

            return Mathf.Sqrt(sumSquares / data.Length); // Typically 0-1
        }
    }
}