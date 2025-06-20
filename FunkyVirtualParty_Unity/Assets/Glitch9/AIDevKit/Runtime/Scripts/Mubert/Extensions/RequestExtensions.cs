using Cysharp.Threading.Tasks;

namespace Glitch9.AIDevKit.Mubert
{
    public static class RequestExtensions
    {
        public static async UniTask<GeneratedAudio> ExecuteAsync(this RecordTrackRequest request)
        {
            return await Mubert.DefaultInstance.TrackGeneration.CreateAsync(request);
        }

        public static async UniTask<GeneratedAudio> ExecuteAsync(this TextToMusicRequest request)
        {
            return await Mubert.DefaultInstance.TrackGeneration.CreateAsync(request);
        }

        public static async UniTask<GeneratedAudio> ExecuteAsync(this ImageToMusicRequest request)
        {
            return await Mubert.DefaultInstance.TrackGeneration.CreateAsync(request);
        }

        public static async UniTask<GeneratedAudio> ExecuteAsync(this TrackStatusRequest request)
        {
            return await Mubert.DefaultInstance.TrackGeneration.CreateAsync(request);
        }
    }
}