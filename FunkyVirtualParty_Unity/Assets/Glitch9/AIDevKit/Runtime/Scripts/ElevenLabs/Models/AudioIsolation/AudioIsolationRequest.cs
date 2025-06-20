using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class AudioIsolationRequest : AudioRequest
    {
        /// <summary>
        /// Required. The audio file from which vocals/speech will be isolated from.
        /// </summary>
        [JsonProperty("audio")] public byte[] Audio { get; set; }

        /// <summary>
        /// Optional. The format of input audio. Options are ‘pcm_s16le_16’ or ‘other’
        /// For pcm_s16le_16, the input audio must be 16-bit PCM at a 16kHz sample rate, 
        /// single channel (mono), and little-endian byte order. Latency will be lower 
        /// than with passing an encoded waveform.
        /// </summary>
        [JsonProperty("file_format")] public ElevenLabsInputFormat InputFormat { get; set; }
    }
}