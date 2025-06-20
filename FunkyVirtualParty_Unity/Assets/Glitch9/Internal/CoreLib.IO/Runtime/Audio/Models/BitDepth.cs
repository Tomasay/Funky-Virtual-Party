using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.CoreLib.IO.Audio
{
    public enum BitDepth
    {
        [ApiEnum("8bit", "8")] Bit8 = 8,
        [ApiEnum("16bit", "16")] Bit16 = 16,
        [ApiEnum("24bit", "24")] Bit24 = 24,
        [ApiEnum("32bit", "32")] Bit32 = 32,
        [ApiEnum("64bit", "64")] Bit64 = 64
    }
}