namespace Glitch9.CoreLib.IO.Audio
{
    public class AudioFormat
    {
        public AudioEncoding Encoding { get; set; } = AudioEncoding.PCM;
        public SampleRate SampleRate { get; set; } = SampleRate.Hz44100;
        public Bitrate Bitrate { get; set; } = Bitrate.Kbps128;
        public BitDepth BitDepth { get; set; } = BitDepth.Bit16;
        public int Channels { get; set; } = 1; // Mono
        public int HeaderSize { get; set; } = 44; // Default WAV header size

        public static AudioFormat DefaultPCM(int channels = 1)
        {
            return new AudioFormat
            {
                Encoding = AudioEncoding.PCM,
                SampleRate = SampleRate.Hz44100,
                Bitrate = Bitrate.Kbps128, // PCM bitrate can vary, but 128kbps is common for 16-bit audio
                BitDepth = BitDepth.Bit16,
                Channels = channels,
                HeaderSize = 0 // PCM does not have a header like WAV
            };
        }

        public static AudioFormat DefaultWAV(int channels = 1)
        {
            return new AudioFormat
            {
                Encoding = AudioEncoding.WAV,
                SampleRate = SampleRate.Hz44100,
                Bitrate = Bitrate.Kbps1411, // CD quality
                BitDepth = BitDepth.Bit16,
                Channels = channels,
                HeaderSize = 44 // Standard WAV header size
            };
        }

        public static AudioFormat DefaultMP3(int channels = 1)
        {
            return new AudioFormat
            {
                Encoding = AudioEncoding.MP3,
                SampleRate = SampleRate.Hz44100,
                Bitrate = Bitrate.Kbps128, // Common bitrate for MP3
                BitDepth = BitDepth.Bit16, // MP3 is typically 16-bit
                Channels = channels,
                HeaderSize = 0 // MP3 does not have a fixed header size like WAV
            };
        }
    }
}