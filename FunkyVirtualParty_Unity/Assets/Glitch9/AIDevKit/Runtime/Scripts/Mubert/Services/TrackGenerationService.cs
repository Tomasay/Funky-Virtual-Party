using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Mubert.Services
{
    public class TrackGenerationService : CRUDServiceBase<Mubert>
    {
        private const string kEndpoint = "v2/RecordTrack";

        public TrackGenerationService(Mubert client) : base(client, false)
        {
        }

        public async UniTask<GeneratedAudio> CreateAsync(RecordTrackRequest request)
        {
            TrackResponse response = await client.POSTCreateAsync<RecordTrackRequest, TrackResponse>(kEndpoint, this, request);
            return await MubertUtils.ConvertToGeneratedAudio(response?.Data?.Tasks);
        }

        public async UniTask<GeneratedAudio> CreateAsync(TextToMusicRequest request)
        {
            TrackResponse response = await client.POSTCreateAsync<TextToMusicRequest, TrackResponse>(kEndpoint, this, request);
            return await MubertUtils.ConvertToGeneratedAudio(response?.Data?.Tasks);
        }

        public async UniTask<GeneratedAudio> CreateAsync(ImageToMusicRequest request)
        {
            request.options.MIMEType = IO.Files.MIMEType.MultipartForm;
            TrackResponse response = await client.POSTCreateAsync<ImageToMusicRequest, TrackResponse>(kEndpoint, this, request);
            return await MubertUtils.ConvertToGeneratedAudio(response?.Data?.Tasks);
        }

        public async UniTask<GeneratedAudio> CreateAsync(TrackStatusRequest request)
        {
            TrackResponse response = await client.POSTCreateAsync<TrackStatusRequest, TrackResponse>(kEndpoint, this, request);
            return await MubertUtils.ConvertToGeneratedAudio(response?.Data?.Tasks);
        }
    }
}
