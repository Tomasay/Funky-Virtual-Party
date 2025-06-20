using System;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs
{
    /// <summary>
    /// The format of input audio. 
    /// Options are ‘pcm_s16le_16’ or ‘other’ For pcm_s16le_16, 
    /// the input audio must be 16-bit PCM at a 16kHz sample rate, 
    /// single channel (mono), and little-endian byte order.
    /// Latency will be lower than with passing an encoded waveform.
    /// </summary>
    public enum ElevenLabsInputFormat
    {
        [ApiEnum("PCM 16-bit 16kHz", "pcm_s16le_16")] PCM_16bit_16kHz,
        [ApiEnum("Other", "other")] Other,
    }

    /// <summary>
    /// Output format of the generated audio. 
    /// Formatted as codec_sample_rate_bitrate. 
    /// So an mp3 with 22.05kHz sample rate at 32kbs is represented as mp3_22050_32. 
    /// MP3 with 192kbps bitrate requires you to be subscribed to Creator tier or above. 
    /// PCM with 44.1kHz sample rate requires you to be subscribed to Pro tier or above. 
    /// Note that the μ-law format (sometimes written mu-law, often approximated as u-law) is commonly used for Twilio audio inputs.
    /// Default is mp3_44100_128.
    /// </summary>
    public enum ElevenLabsOutputFormat
    {
        // Unity does not support commneted formats
        [ApiEnum("MP3 22.05kHz 32kb", "mp3_22050_32")] MP3_22050_32,
        [ApiEnum("MP3 44.1kHz 32kb", "mp3_44100_32")] MP3_44100_32,
        [ApiEnum("MP3 44.1kHz 64kb", "mp3_44100_64")] MP3_44100_64,
        [ApiEnum("MP3 44.1kHz 96kb", "mp3_44100_96")] MP3_44100_96,
        [ApiEnum("MP3 44.1kHz 128kb", "mp3_44100_128")] MP3_44100_128,
        [ApiEnum("MP3 44.1kHz 192kb", "mp3_44100_192")] MP3_44100_192,
        [ApiEnum("PCM 8kHz", "pcm_8000")] PCM_8000,
        [ApiEnum("PCM 16kHz", "pcm_16000")] PCM_16000,
        [ApiEnum("PCM 22.05kHz", "pcm_22050")] PCM_22050,
        [ApiEnum("PCM 24kHz", "pcm_24000")] PCM_24000,
        [ApiEnum("PCM 44.1kHz", "pcm_44100")] PCM_44100,

        [ApiEnum("μ-law 8kHz", "ulaw_8000")] ULaw_8000,
        [ApiEnum("a-law 8kHz", "alaw_8000")] ALaw_8000,
        // [ApiEnum("Opus 48kHz 32kb", "opus_48000_32")] Opus_48000_32,
        // [ApiEnum("Opus 48kHz 64kb", "opus_48000_64")] Opus_48000_64,
        // [ApiEnum("Opus 48kHz 96kb", "opus_48000_96")] Opus_48000_96,
        // [ApiEnum("Opus 48kHz 128kb", "opus_48000_128")] Opus_48000_128,
        // [ApiEnum("Opus 48kHz 192kb", "opus_48000_192")] Opus_48000_192
    }

    public static class OutputFormatExtensions
    {
        public static AudioFormat ToAudioFormat(this ElevenLabsOutputFormat format)
        {
            return new AudioFormat()
            {
                Encoding = format.GetAudioEncoding(),
                Bitrate = format.GetBitrate(),
                SampleRate = format.GetSamplingRate()
            };
        }

        public static AudioEncoding GetAudioEncoding(this ElevenLabsOutputFormat format)
        {
            return format switch
            {
                ElevenLabsOutputFormat.MP3_22050_32 => AudioEncoding.MP3,
                ElevenLabsOutputFormat.MP3_44100_32 => AudioEncoding.MP3,
                ElevenLabsOutputFormat.MP3_44100_64 => AudioEncoding.MP3,
                ElevenLabsOutputFormat.MP3_44100_96 => AudioEncoding.MP3,
                ElevenLabsOutputFormat.MP3_44100_128 => AudioEncoding.MP3,
                ElevenLabsOutputFormat.MP3_44100_192 => AudioEncoding.MP3,
                ElevenLabsOutputFormat.PCM_8000 => AudioEncoding.PCM,
                ElevenLabsOutputFormat.PCM_16000 => AudioEncoding.PCM,
                ElevenLabsOutputFormat.PCM_22050 => AudioEncoding.PCM,
                ElevenLabsOutputFormat.PCM_24000 => AudioEncoding.PCM,
                ElevenLabsOutputFormat.PCM_44100 => AudioEncoding.PCM,
                ElevenLabsOutputFormat.ULaw_8000 => AudioEncoding.ULaw,
                ElevenLabsOutputFormat.ALaw_8000 => AudioEncoding.ALaw,
                // ElevenLabsOutputFormat.Opus_48000_32 => AudioEncoding.Opus,
                // ElevenLabsOutputFormat.Opus_48000_64 => AudioEncoding.Opus,
                // ElevenLabsOutputFormat.Opus_48000_96 => AudioEncoding.Opus,
                // ElevenLabsOutputFormat.Opus_48000_128 => AudioEncoding.Opus,
                // ElevenLabsOutputFormat.Opus_48000_192 => AudioEncoding.Opus,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        public static Bitrate GetBitrate(this ElevenLabsOutputFormat format)
        {
            return format switch
            {
                ElevenLabsOutputFormat.MP3_22050_32 => Bitrate.Kbps32,
                ElevenLabsOutputFormat.MP3_44100_32 => Bitrate.Kbps32,
                ElevenLabsOutputFormat.MP3_44100_64 => Bitrate.Kbps64,
                ElevenLabsOutputFormat.MP3_44100_96 => Bitrate.Kbps96,
                ElevenLabsOutputFormat.MP3_44100_128 => Bitrate.Kbps128,
                ElevenLabsOutputFormat.MP3_44100_192 => Bitrate.Kbps192,
                ElevenLabsOutputFormat.PCM_8000 => Bitrate.Kbps16,
                ElevenLabsOutputFormat.PCM_16000 => Bitrate.Kbps24,
                ElevenLabsOutputFormat.PCM_22050 => Bitrate.Kbps32,
                ElevenLabsOutputFormat.PCM_24000 => Bitrate.Kbps48,
                ElevenLabsOutputFormat.PCM_44100 => Bitrate.Kbps64,
                ElevenLabsOutputFormat.ULaw_8000 => Bitrate.Kbps16,
                ElevenLabsOutputFormat.ALaw_8000 => Bitrate.Kbps16,
                // ElevenLabsOutputFormat.Opus_48000_32 => Bitrate.Kbps32,
                // ElevenLabsOutputFormat.Opus_48000_64 => Bitrate.Kbps64,
                // ElevenLabsOutputFormat.Opus_48000_96 => Bitrate.Kbps96,
                // ElevenLabsOutputFormat.Opus_48000_128 => Bitrate.Kbps128,
                // ElevenLabsOutputFormat.Opus_48000_192 => Bitrate.Kbps192,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        public static SampleRate GetSamplingRate(this ElevenLabsOutputFormat format)
        {
            return format switch
            {
                ElevenLabsOutputFormat.MP3_22050_32 => SampleRate.Hz22050,
                ElevenLabsOutputFormat.MP3_44100_32 => SampleRate.Hz44100,
                ElevenLabsOutputFormat.MP3_44100_64 => SampleRate.Hz44100,
                ElevenLabsOutputFormat.MP3_44100_96 => SampleRate.Hz44100,
                ElevenLabsOutputFormat.MP3_44100_128 => SampleRate.Hz44100,
                ElevenLabsOutputFormat.MP3_44100_192 => SampleRate.Hz44100,
                ElevenLabsOutputFormat.PCM_8000 => SampleRate.Hz8000,
                ElevenLabsOutputFormat.PCM_16000 => SampleRate.Hz16000,
                ElevenLabsOutputFormat.PCM_22050 => SampleRate.Hz22050,
                ElevenLabsOutputFormat.PCM_24000 => SampleRate.Hz24000,
                ElevenLabsOutputFormat.PCM_44100 => SampleRate.Hz44100,
                ElevenLabsOutputFormat.ULaw_8000 => SampleRate.Hz8000,
                ElevenLabsOutputFormat.ALaw_8000 => SampleRate.Hz8000,
                // ElevenLabsOutputFormat.Opus_48000_32 => SamplingRate.Hz48000,
                // ElevenLabsOutputFormat.Opus_48000_64 => SamplingRate.Hz48000,
                // ElevenLabsOutputFormat.Opus_48000_96 => SamplingRate.Hz48000,
                // ElevenLabsOutputFormat.Opus_48000_128 => SamplingRate.Hz48000,
                // ElevenLabsOutputFormat.Opus_48000_192 => SamplingRate.Hz48000,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        public static string GetFileExtension(this ElevenLabsOutputFormat format)
        {
            return format switch
            {
                ElevenLabsOutputFormat.MP3_22050_32 => ".mp3",
                ElevenLabsOutputFormat.MP3_44100_32 => ".mp3",
                ElevenLabsOutputFormat.MP3_44100_64 => ".mp3",
                ElevenLabsOutputFormat.MP3_44100_96 => ".mp3",
                ElevenLabsOutputFormat.MP3_44100_128 => ".mp3",
                ElevenLabsOutputFormat.MP3_44100_192 => ".mp3",
                ElevenLabsOutputFormat.PCM_8000 => ".wav",
                ElevenLabsOutputFormat.PCM_16000 => ".wav",
                ElevenLabsOutputFormat.PCM_22050 => ".wav",
                ElevenLabsOutputFormat.PCM_24000 => ".wav",
                ElevenLabsOutputFormat.PCM_44100 => ".wav",
                ElevenLabsOutputFormat.ULaw_8000 => ".wav",
                ElevenLabsOutputFormat.ALaw_8000 => ".wav",
                // ElevenLabsOutputFormat.Opus_48000_32 => ".opus",
                // ElevenLabsOutputFormat.Opus_48000_64 => ".opus",
                // ElevenLabsOutputFormat.Opus_48000_96 => ".opus",
                // ElevenLabsOutputFormat.Opus_48000_128 => ".opus",
                // ElevenLabsOutputFormat.Opus_48000_192 => ".opus",
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }
    }
}