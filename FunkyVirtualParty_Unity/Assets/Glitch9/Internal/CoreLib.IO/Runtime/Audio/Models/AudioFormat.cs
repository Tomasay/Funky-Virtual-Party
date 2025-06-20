namespace Glitch9.CoreLib.IO.Audio
{
    public class AudioFormat
    {
        public AudioEncoding Encoding { get; set; } = AudioEncoding.PCM;
        public SampleRate SampleRate { get; set; } = SampleRate.Hz44100;
        public Bitrate Bitrate { get; set; } = Bitrate.Kbps128;
        public BitDepth BitDepth { get; set; } = BitDepth.Bit16;
        public int Channels { get; set; } = 1; // Mono
    }
}