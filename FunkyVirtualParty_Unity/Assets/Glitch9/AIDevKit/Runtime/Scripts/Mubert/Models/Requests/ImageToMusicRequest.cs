using Glitch9.IO.Files;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Mubert
{
    /// <summary>
    /// Request object for Mubert's ITMRecordTrack method (Image-to-Music).
    /// </summary>
    public class ImageToMusicRequest : MubertRequest<ImageToMusicRequestParams>
    {
        public override MubertMethod Method => MubertMethod.ITMRecordTrack;

    }

    /// <summary>
    /// Parameters for ITMRecordTrack method.
    /// </summary>
    public class ImageToMusicRequestParams : BaseRecordTrackRequestParams
    {
        /// <summary>
        /// Required. The path to the image file to be used (jpeg/png, max 10MB).
        /// Should be used with multipart/form-data.
        /// </summary>
        [JsonProperty("file")] public IFile File { get; set; }
    }
}
