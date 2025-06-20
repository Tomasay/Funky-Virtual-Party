using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    public enum RealtimeItemContentType
    {
        [ApiEnum("InputText", "input_text")]
        InputText,

        [ApiEnum("InputAudio", "input_audio")]
        InputAudio,

        [ApiEnum("Text", "text")]
        Text,

        [ApiEnum("Audio", "audio")]
        Audio
    }
}
