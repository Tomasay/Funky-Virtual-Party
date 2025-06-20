using Newtonsoft.Json;

namespace Glitch9.AIDevKit.Mubert
{
    /// <summary>
    /// Request object for checking the status of generated tracks.
    /// </summary>
    public class TrackStatusRequest : MubertRequest<MubertRequestParams>
    {
        /// <summary>
        /// The method name for the TrackStatus request.
        /// </summary>
        public override MubertMethod Method => MubertMethod.TrackStatus; 
    }
}
