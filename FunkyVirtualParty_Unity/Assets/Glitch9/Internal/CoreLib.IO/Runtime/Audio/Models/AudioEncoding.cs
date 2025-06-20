using System;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.CoreLib.IO.Audio
{
    public enum AudioEncoding
    {
        [ApiEnum("wav")] WAV, // OpenAI, PlayHT
        [ApiEnum("pcm")] PCM, // OpenAI, ElevenLabs
        [ApiEnum("mp3")] MP3, // OpenAI, PlayHT, ElevenLabs
        [ApiEnum("flac")] Flac, // OpenAI, PlayHT 
        [ApiEnum("opus")] Opus, // OpenAI, ElevenLabs
        [ApiEnum("aac")] AAC, // ElevenLabs
        [ApiEnum("ogg")] OGG, // PlayHT
        [ApiEnum("mulaw")] Mulaw, // PlayHT, ElevenLabs 
        [ApiEnum("ulaw")] ULaw, // ElevenLabs
        [ApiEnum("alaw")] ALaw,  // ElevenLabs
    }

    public static class AudioEncodingExtensions
    {
        public static MIMEType ToMIMEType(this AudioEncoding encoding)
        {
            return encoding switch
            {
                AudioEncoding.MP3 => MIMEType.MPEG,
                AudioEncoding.Opus => MIMEType.Opus,
                AudioEncoding.AAC => MIMEType.AAC,
                AudioEncoding.Flac => MIMEType.FLAC,
                AudioEncoding.WAV => MIMEType.WAV,
                AudioEncoding.PCM => MIMEType.PCM,
                AudioEncoding.OGG => MIMEType.OGG,
                AudioEncoding.Mulaw => MIMEType.MuLaw,
                _ => throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null)
            };
        }

        public static AudioEncoding ToAudioEncoding(this MIMEType mimeType, AudioEncoding defaultEncoding = AudioEncoding.MP3)
        {
            // Map MIME types to AudioEncoding
            return mimeType switch
            {
                MIMEType.MPEG => AudioEncoding.MP3,
                MIMEType.Opus => AudioEncoding.Opus,
                MIMEType.AAC => AudioEncoding.AAC,
                MIMEType.FLAC => AudioEncoding.Flac,
                MIMEType.WAV => AudioEncoding.WAV,
                MIMEType.PCM => AudioEncoding.PCM,
                MIMEType.OGG => AudioEncoding.OGG,
                MIMEType.MuLaw => AudioEncoding.Mulaw,
                _ => defaultEncoding // Default to MP3 if no match found
            };
        }
    }
}