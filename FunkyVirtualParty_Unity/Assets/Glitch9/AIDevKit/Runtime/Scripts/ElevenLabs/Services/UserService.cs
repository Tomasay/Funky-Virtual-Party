using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.ElevenLabs.Services
{
    public class UserService : CRUDServiceBase<ElevenLabs>
    {
        private const string kEndpoint = "v1/user";

        public UserService(ElevenLabs client) : base(client, false) { }

        /// <summary>
        /// Converts a text prompt into a sound effect.
        /// </summary>
        public async UniTask<UserSubscription> GetAsync(RequestOptions options = null)
        {
            UserSubscriptionResponse response = await client.GETRetrieveAsync<UserSubscriptionResponse>(kEndpoint, this, options);
            return response?.Subscription;
        }
    }
}
