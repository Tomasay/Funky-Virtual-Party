using Newtonsoft.Json;

namespace Glitch9.AIDevKit.ElevenLabs
{
    public class VoiceChangerRequest : VoiceRequest
    {
        // Body Parameters ----------------------------------------------------------------

        /// <summary>
        /// Required. The audio file which holds the content and emotion that will control the generated speech.
        /// </summary>
        [JsonProperty("audio")] public byte[] Audio { get; set; }

        /// <summary>
        /// Optional. Voice settings overriding stored settings for the given voice.
        /// They are applied only on the given request. Needs to be send as a JSON encoded string.
        /// </summary>
        [JsonProperty("voice_settings")] public VoiceSettings VoiceSettings { get; set; }

        /// <summary>
        /// Optional. If specified, our system will make a best effort to sample deterministically,
        /// such that repeated requests with the same seed and parameters should return the same result.
        /// Determinism is not guaranteed. Must be integer between 0 and 4294967295.
        /// </summary>
        [JsonProperty("seed")] public uint? Seed { get; set; }

        /// <summary>
        /// Optional. If set, will remove the background noise from your audio input using our audio isolation model.
        /// Only applies to Voice Changer.
        /// Defaults to false
        /// </summary>
        [JsonProperty("remove_background_noise")] public bool? RemoveBackgroundNoise { get; set; }

        /// <summary>
        /// Optional. The format of input audio. Options are ‘pcm_s16le_16’ or ‘other’
        /// For pcm_s16le_16, the input audio must be 16-bit PCM at a 16kHz sample rate, single channel (mono),
        /// and little-endian byte order. Latency will be lower than with passing an encoded waveform.
        /// </summary>
        [JsonProperty("file_format")] public ElevenLabsInputFormat InputFormat { get; set; }
    }
}