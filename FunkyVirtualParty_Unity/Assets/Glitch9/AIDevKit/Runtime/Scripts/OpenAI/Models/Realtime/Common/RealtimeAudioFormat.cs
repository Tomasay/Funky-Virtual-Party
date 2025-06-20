using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    public enum RealtimeAudioFormat
    {
        [ApiEnum("PCM16", "pcm16")] PCM16,
        [ApiEnum("G711 ULAW", "g711_ulaw")] G711_ULAW,
        [ApiEnum("G711 ALAW", "g711_alaw")] G711_ALAW
    }

    public static class RealtimeAudioFormatExtensions
    {
        /*
        raw 16 bit PCM audio at 24kHz, 1 channel, little-endian
        G.711 at 8kHz (both u-law and a-law)
        */

        public static int GetSampleRate(this RealtimeAudioFormat audioFormat)
        {
            switch (audioFormat)
            {
                case RealtimeAudioFormat.PCM16:
                    return 24000;
                case RealtimeAudioFormat.G711_ULAW:
                case RealtimeAudioFormat.G711_ALAW:
                    return 8000;
                default:
                    return 24000;
            }
        }

        public static int GetChannelCount(this RealtimeAudioFormat audioFormat)
        {
            switch (audioFormat)
            {
                case RealtimeAudioFormat.PCM16:
                    return 1;
                case RealtimeAudioFormat.G711_ULAW:
                case RealtimeAudioFormat.G711_ALAW:
                    return 1;
                default:
                    return 1;
            }
        }
    }
}
