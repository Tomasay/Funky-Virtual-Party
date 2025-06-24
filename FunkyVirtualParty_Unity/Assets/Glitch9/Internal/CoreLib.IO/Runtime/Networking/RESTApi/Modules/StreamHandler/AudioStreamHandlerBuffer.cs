using System;
using Glitch9.CoreLib.IO.Audio;
using UnityEngine;

namespace Glitch9.IO.Networking.RESTApi
{
    public class AudioStreamHandlerBuffer : BaseStreamHandlerBuffer<AudioStreamHandler>
    {
        private readonly Func<byte[], int, float[]> _converter;

        public AudioStreamHandlerBuffer(RESTClient client, AudioStreamHandler audioStreamHandler, bool ignoreLogs) : base(client, audioStreamHandler, ignoreLogs)
        {
            AudioFormat audioFormat = audioStreamHandler.AudioFormat;
            int offsetSample = audioFormat.HeaderSize;

            if (audioFormat == null) throw new ArgumentNullException(nameof(audioFormat), "Audio format cannot be null.");

            Debug.Log($"AudioStreamHandlerBuffer: Initializing with format {audioFormat.Encoding}, SampleRate: {audioFormat.SampleRate}, BitDepth: {audioFormat.BitDepth}, Bitrate: {audioFormat.Bitrate}");

            if (audioFormat.Encoding == AudioEncoding.WAV)
            {
                if (audioFormat.BitDepth == BitDepth.Bit8)
                {
                    _converter = (audioBytes, size) => WavUtil.Convert8BitByteArray(audioBytes, offsetSample);
                }
                else if (audioFormat.BitDepth == BitDepth.Bit16)
                {
                    _converter = (audioBytes, size) => WavUtil.Convert16BitByteArray(audioBytes, offsetSample);
                }
                else if (audioFormat.BitDepth == BitDepth.Bit24)
                {
                    _converter = (audioBytes, size) => WavUtil.Convert24BitByteArray(audioBytes, offsetSample);
                }
                else if (audioFormat.BitDepth == BitDepth.Bit32)
                {
                    _converter = (audioBytes, size) => WavUtil.Convert32BitByteArray(audioBytes, offsetSample);
                }
                else
                {
                    throw new NotSupportedException($"WAV format with bitrate {audioFormat.Bitrate} is not supported.");
                }
            }
            else if (audioFormat.Encoding == AudioEncoding.PCM)
            {
                _converter = (audioBytes, size) => AudioProcessor.PCM16ToFloatArray(audioBytes);
            }
            else if (audioFormat.Encoding == AudioEncoding.ULaw || audioFormat.Encoding == AudioEncoding.Mulaw)
            {
                _converter = (audioBytes, size) => AudioProcessor.G711uLawToFloatArray(audioBytes);
            }
            else if (audioFormat.Encoding == AudioEncoding.ALaw)
            {
                _converter = (audioBytes, size) => AudioProcessor.G711aLawToFloatArray(audioBytes);
            }
            else
            {
                throw new NotSupportedException($"Audio encoding {audioFormat.Encoding} is not supported.");
            }
        }

        protected override bool ProcessData(byte[] audioBytes, int dataLength)
        {
            float[] audioData = _converter(audioBytes, dataLength);
            if (audioData == null || audioData.Length == 0) return false;

            _streamHandler.onReceiveData?.Invoke(audioData);
            return true;
        }
    }
}