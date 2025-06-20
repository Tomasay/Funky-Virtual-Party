using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs.Services
{
    public class SoundEffectService : CRUDServiceBase<ElevenLabs>
    {
        private const string kEndpoint = "v1/sound-generation";

        public SoundEffectService(ElevenLabs client)
            : base(client, false)
        {
        }

        /// <summary>
        /// Converts a text prompt into a sound effect.
        /// </summary>
        public async UniTask<GeneratedAudio> CreateAsync(SoundEffectRequest request)
        {
            RESTResponse res = await client.POSTCreateAsync(kEndpoint, this, request);
            if (res == null) return null;
            return new GeneratedAudio(res.AudioOutput, res.OutputPath);
        }
    }
}
