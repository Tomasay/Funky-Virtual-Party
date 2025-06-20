

using System;
using Glitch9.CoreLib.IO.Audio;

namespace Glitch9.IO.Networking.RESTApi
{
    public class AudioStreamHandlerBuffer : BaseStreamHandlerBuffer<PcmAudioStreamHandler>
    {
        private Func<byte[], int, float[]> _converter;

        public AudioStreamHandlerBuffer(RESTClient client, PcmAudioStreamHandler audioStreamHandler, bool ignoreLogs) : base(client, audioStreamHandler, ignoreLogs)
        {
            AudioFormat audioFormat = audioStreamHandler.audioFormat;
            int offsetSample = audioStreamHandler.headerSize;

            if (audioFormat == null) throw new ArgumentNullException(nameof(audioFormat), "Audio format cannot be null.");

            if (audioFormat.Encoding == AudioEncoding.WAV)
            {
                if (audioFormat.BitDepth == BitDepth.Bit8)
                {
                    _converter = (fileBytes, size) => WavUtil.Convert8BitByteArray(fileBytes, offsetSample);
                }
                else if (audioFormat.BitDepth == BitDepth.Bit16)
                {
                    _converter = (fileBytes, size) => WavUtil.Convert16BitByteArray(fileBytes, offsetSample);
                }
                else if (audioFormat.BitDepth == BitDepth.Bit24)
                {
                    _converter = (fileBytes, size) => WavUtil.Convert24BitByteArray(fileBytes, offsetSample);
                }
                else if (audioFormat.BitDepth == BitDepth.Bit32)
                {
                    _converter = (fileBytes, size) => WavUtil.Convert32BitByteArray(fileBytes, offsetSample);
                }
                else
                {
                    throw new NotSupportedException($"WAV format with bitrate {audioFormat.Bitrate} is not supported.");
                }
            }
            else if (audioFormat.Encoding == AudioEncoding.PCM)
            {
                _converter = (data, size) => AudioProcessor.PCM16ToFloatArray(data);
            }
            else if (audioFormat.Encoding == AudioEncoding.ULaw || audioFormat.Encoding == AudioEncoding.Mulaw)
            {
                _converter = (data, size) => AudioProcessor.G711uLawToFloatArray(data);
            }
            else if (audioFormat.Encoding == AudioEncoding.ALaw)
            {
                _converter = (data, size) => AudioProcessor.G711aLawToFloatArray(data);
            }
            else
            {
                throw new NotSupportedException($"Audio encoding {audioFormat.Encoding} is not supported.");
            }
        }

        protected override bool ProcessData(byte[] streamedData, int dataLength)
        {
            float[] audioData = _converter(streamedData, dataLength);
            if (audioData == null || audioData.Length == 0) return false;

            _streamHandler.onStream?.Invoke(audioData);
            return true;
        }
    }
}