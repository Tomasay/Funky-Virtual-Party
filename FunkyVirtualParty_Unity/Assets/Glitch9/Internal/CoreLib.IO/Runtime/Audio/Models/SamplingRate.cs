using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.CoreLib.IO.Audio
{
    public enum SampleRate
    {
        [ApiEnum("8KHz", "8000")] Hz8000 = 8000,
        [ApiEnum("11.025KHz", "11025")] Hz11025 = 11025,
        [ApiEnum("12KHz", "12000")] Hz12000 = 12000,
        [ApiEnum("16KHz", "16000")] Hz16000 = 16000,
        [ApiEnum("22.05KHz", "22050")] Hz22050 = 22050,
        [ApiEnum("24KHz", "24000")] Hz24000 = 24000,
        [ApiEnum("32KHz", "32000")] Hz32000 = 32000,
        [ApiEnum("44.1KHz", "44100")] Hz44100 = 44100,
        [ApiEnum("48KHz", "48000")] Hz48000 = 48000,
        [ApiEnum("88.2KHz", "88200")] Hz88200 = 88200,
        [ApiEnum("96KHz", "96000")] Hz96000 = 96000,
        [ApiEnum("176.4KHz", "176400")] Hz176400 = 176400,
        [ApiEnum("192KHz", "192000")] Hz192000 = 192000
    }
}