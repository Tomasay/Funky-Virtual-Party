using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    /// <summary>
    /// The content part that is done.
    /// </summary>
    public class RealtimeItemContent
    {
        /// <summary>
        /// The content type 
        /// </summary>
        [JsonProperty("type")] public RealtimeItemContentType? Type { get; set; }

        /// <summary>
        /// The text content (if type is "text").
        /// </summary>
        [JsonProperty("text")] public string Text { get; set; }

        /// <summary>
        /// Base64-encoded audio data (if type is "audio").
        /// </summary>
        [JsonProperty("audio")] public string Audio { get; set; }

        /// <summary>
        /// The transcript of the audio (if type is "audio").
        /// </summary>
        [JsonProperty("transcript")] public string Transcript { get; set; }


        public static RealtimeItemContent CreateInputText(string text)
        {
            return new RealtimeItemContent
            {
                Type = RealtimeItemContentType.InputText,
                Text = text
            };
        }

        public static RealtimeItemContent CreateInputAudio(string audio, string transcript)
        {
            return new RealtimeItemContent
            {
                Type = RealtimeItemContentType.InputAudio,
                Audio = audio,
                Transcript = transcript
            };
        }
    }
}
