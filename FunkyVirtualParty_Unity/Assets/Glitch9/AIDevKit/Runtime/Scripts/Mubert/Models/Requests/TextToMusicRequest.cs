using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Mubert
{
    public class TextToMusicRequest : MubertRequest<TextToMusicRequestParams>
    {
        public override MubertMethod Method => MubertMethod.TTMRecordTrack;
    }

    public class TextToMusicRequestParams : BaseRecordTrackRequestParams
    {  
        /// <summary>
        /// Required. Text prompt used to generate the music. Must be in English (max 200 characters).
        /// </summary>
        [JsonProperty("text")] public string Text { get; set; } 
    } 
}