using System;

namespace Glitch9.CoreLib.IO.Audio
{
    internal readonly struct PcmData
    {
        #region Public data

        public float[] Value { get; }
        public int Length { get; }
        public int Channels { get; }
        public int SampleRate { get; }

        #endregion

        #region Constructor

        private PcmData(float[] value, int channels, int sampleRate)
        {
            Value = value;
            Length = value.Length;
            Channels = channels;
            SampleRate = sampleRate;
        }

        #endregion

        #region Public Methods

        public static PcmData FromBytes(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            PcmHeader pcmHeader = PcmHeader.FromBytes(bytes);

            if (pcmHeader.BitDepth != 8 && pcmHeader.BitDepth != 16 && pcmHeader.BitDepth != 32)
                throw new ArgumentOutOfRangeException(nameof(pcmHeader.BitDepth), pcmHeader.BitDepth, "Supported values are: 8, 16, 32");

            float[] samples = new float[pcmHeader.AudioSampleCount];

            for (int i = 0; i < samples.Length; ++i)
            {
                int byteIndex = pcmHeader.AudioStartIndex + i * pcmHeader.AudioSampleSize;
                float rawSample = pcmHeader.BitDepth switch
                {
                    8 => bytes[byteIndex],
                    16 => BitConverter.ToInt16(bytes, byteIndex),
                    32 => BitConverter.ToInt32(bytes, byteIndex),
                    _ => throw new ArgumentOutOfRangeException(nameof(pcmHeader.BitDepth))
                };

                samples[i] = pcmHeader.NormalizeSample(rawSample);
            }

            return new PcmData(samples, pcmHeader.Channels, pcmHeader.SampleRate);
        }

        #endregion
    }
}