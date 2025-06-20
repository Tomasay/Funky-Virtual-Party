using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.CoreLib.IO.Audio
{
    public enum Bitrate
    {
        [ApiEnum("16kbps", "16")] Kbps16 = 16,
        [ApiEnum("24kbps", "24")] Kbps24 = 24,
        [ApiEnum("32kbps", "32")] Kbps32 = 32,
        [ApiEnum("48kbps", "48")] Kbps48 = 48,
        [ApiEnum("64kbps", "64")] Kbps64 = 64,
        [ApiEnum("96kbps", "96")] Kbps96 = 96,
        [ApiEnum("128kbps", "128")] Kbps128 = 128,
        [ApiEnum("160kbps", "160")] Kbps160 = 160,
        [ApiEnum("192kbps", "192")] Kbps192 = 192,
        [ApiEnum("256kbps", "256")] Kbps256 = 256,
        [ApiEnum("320kbps", "320")] Kbps320 = 320
    }
}