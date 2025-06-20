using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Glitch9.CoreLib.IO.Audio
{
    public static class WavUtil
    {
        private const int BLOCK_SIZE_16_BIT = 2;

        #region AudioClip Loaders

        public static AudioClip ToAudioClip(byte[] fileBytes, int offsetSamples = 0, string name = "decoded_audio_clip")
        {
            Debug.Log($"[WAV] Format: {BitConverter.ToUInt16(fileBytes, 20)} (1=PCM, 3=FLOAT)");
            Debug.Log($"[WAV] Bits: {BitConverter.ToUInt16(fileBytes, 34)}");

            int subchunk1 = BitConverter.ToInt32(fileBytes, 16);
            //ushort audioFormat = BitConverter.ToUInt16(fileBytes, 20);
            ushort channels = BitConverter.ToUInt16(fileBytes, 22);
            int sampleRate = BitConverter.ToInt32(fileBytes, 24);
            ushort bitDepth = BitConverter.ToUInt16(fileBytes, 34);

            int headerOffset = 16 + 4 + subchunk1 + 4;
            //int subchunk2 = BitConverter.ToInt32(fileBytes, headerOffset);

            float[] data = bitDepth switch
            {
                8 => Convert8BitByteArray(fileBytes, headerOffset),
                16 => Convert16BitByteArray(fileBytes, headerOffset),
                24 => Convert24BitByteArray(fileBytes, headerOffset),
                32 => Convert32BitByteArray(fileBytes, headerOffset),
                _ => throw new Exception($"Unsupported bit depth: {bitDepth}"),
            };

            if (data == null || data.Length == 0)
            {
                Debug.LogError("Failed to decode audio data: length is 0: " + fileBytes.Length);
                return null;
            }

            var audioClip = AudioClip.Create(name, data.Length, channels, sampleRate, false);
            audioClip.SetData(data, offsetSamples);
            return audioClip;
        }

        #endregion

        #region AudioClip Savers

        public static byte[] FromAudioClip(AudioClip clip) => FromAudioClip(clip, out _, false);

        public static byte[] FromAudioClip(AudioClip clip, out string filepath, bool saveAsFile = true, string dirname = "recordings")
        {
            using var stream = new MemoryStream();

            int fileSize = clip.samples * BLOCK_SIZE_16_BIT + 44;
            WriteFileHeader(stream, fileSize);
            WriteFileFormat(stream, clip.channels, clip.frequency, 16);
            WriteFileData(stream, clip);

            var bytes = stream.ToArray();
            Debug.AssertFormat(bytes.Length == fileSize, "Mismatch file size: {0} == {1}", bytes.Length, fileSize);

            if (saveAsFile)
            {
                filepath = Path.Combine(Application.persistentDataPath, dirname, DateTime.UtcNow.ToString("yyMMdd-HHmmss-fff") + ".wav");
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));
                File.WriteAllBytes(filepath, bytes);
            }
            else filepath = null;

            return bytes;
        }

        #endregion

        #region Bit Conversion Helpers

        internal static float[] Convert8BitByteArray(byte[] source, int offset)
        {
            int size = BitConverter.ToInt32(source, offset);
            offset += 4;
            var data = new float[size];
            for (int i = 0; i < size; i++) data[i] = (float)source[i + offset] / sbyte.MaxValue;
            return data;
        }

        internal static float[] Convert16BitByteArray(byte[] source, int offset)
        {
            int size = BitConverter.ToInt32(source, offset);
            offset += 4;
            int sampleCount = size / 2;
            var data = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
                data[i] = BitConverter.ToInt16(source, offset + i * 2) / (float)short.MaxValue;
            return data;
        }

        internal static float[] Convert24BitByteArray(byte[] source, int offset)
        {
            int size = BitConverter.ToInt32(source, offset);
            offset += 4;
            int sampleCount = size / 3;
            var data = new float[sampleCount];
            var temp = new byte[4];
            for (int i = 0; i < sampleCount; i++)
            {
                Buffer.BlockCopy(source, offset + i * 3, temp, 1, 3);
                data[i] = BitConverter.ToInt32(temp, 0) / (float)Int32.MaxValue;
            }
            return data;
        }

        internal static float[] Convert32BitByteArray(byte[] source, int offset)
        {
            int size = BitConverter.ToInt32(source, offset);
            offset += 4;
            int sampleCount = size / 4;
            var data = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
                data[i] = BitConverter.ToInt32(source, offset + i * 4) / (float)Int32.MaxValue;
            return data;
        }

        #endregion

        #region File Write Internals

        private static void WriteFileHeader(Stream stream, int fileSize)
        {
            stream.Write(Encoding.ASCII.GetBytes("RIFF"));
            stream.Write(BitConverter.GetBytes(fileSize - 8));
            stream.Write(Encoding.ASCII.GetBytes("WAVE"));
        }

        private static void WriteFileFormat(Stream stream, int channels, int sampleRate, ushort bitDepth)
        {
            stream.Write(Encoding.ASCII.GetBytes("fmt "));
            stream.Write(BitConverter.GetBytes(16)); // Subchunk1Size
            stream.Write(BitConverter.GetBytes((ushort)1)); // PCM
            stream.Write(BitConverter.GetBytes((ushort)channels));
            stream.Write(BitConverter.GetBytes(sampleRate));
            stream.Write(BitConverter.GetBytes(sampleRate * channels * bitDepth / 8)); // ByteRate
            stream.Write(BitConverter.GetBytes((ushort)(channels * bitDepth / 8))); // BlockAlign
            stream.Write(BitConverter.GetBytes(bitDepth));
        }

        private static void WriteFileData(Stream stream, AudioClip clip)
        {
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);
            byte[] data = ConvertFloatToInt16Bytes(samples);

            stream.Write(Encoding.ASCII.GetBytes("data"));
            stream.Write(BitConverter.GetBytes(data.Length));
            stream.Write(data);
        }

        private static byte[] ConvertFloatToInt16Bytes(float[] samples)
        {
            using var stream = new MemoryStream();
            foreach (float sample in samples)
            {
                short s = (short)(sample * short.MaxValue);
                stream.Write(BitConverter.GetBytes(s));
            }
            return stream.ToArray();
        }

        public static byte[] WriteFileHeader(byte[] bytes, int sampleRate, int channels, int bitsPerSample)
        {
            using var mem = new MemoryStream();
            using var writer = new BinaryWriter(mem);

            int byteRate = sampleRate * channels * bitsPerSample / 8;
            int blockAlign = channels * bitsPerSample / 8;
            int subchunk2Size = bytes.Length;

            // ChunkID
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + subchunk2Size); // ChunkSize
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

            // Subchunk1
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); // Subchunk1Size for PCM
            writer.Write((short)1); // AudioFormat = 1 (PCM)
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)blockAlign);
            writer.Write((short)bitsPerSample);

            // Subchunk2
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(subchunk2Size);
            writer.Write(bytes);

            return mem.ToArray();
        }


        #endregion

        public static byte[] EnsureFileHeader(byte[] bytes, SampleRate samplingRate)
        {
            if (HasFileHeader(bytes)) return bytes; // 이미 WAV 헤더가 있는 경우 
            return WriteFileHeader(bytes, (int)samplingRate, 1, 16); // WAV 헤더가 없는 경우 새로 작성  
        }

        public static bool HasFileHeader(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 44) return false; // 최소 WAV 헤더 크기

            // Check RIFF header
            if (!(bytes[0] == 'R' && bytes[1] == 'I' && bytes[2] == 'F' && bytes[3] == 'F'))
                return false;

            // Check WAVE format
            if (!(bytes[8] == 'W' && bytes[9] == 'A' && bytes[10] == 'V' && bytes[11] == 'E'))
                return false;

            // Check 'fmt ' subchunk
            if (!(bytes[12] == 'f' && bytes[13] == 'm' && bytes[14] == 't' && bytes[15] == ' '))
                return false;

            // Check 'data' subchunk exists somewhere in the file
            for (int i = 36; i < bytes.Length - 4; i++)
            {
                if (bytes[i] == 'd' && bytes[i + 1] == 'a' && bytes[i + 2] == 't' && bytes[i + 3] == 'a')
                    return true;
            }

            return false;
        }

    }
}
