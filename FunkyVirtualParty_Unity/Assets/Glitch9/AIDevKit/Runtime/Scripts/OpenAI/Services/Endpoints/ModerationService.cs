using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Moderation Service: https://platform.openai.com/docs/api-reference/moderations
    /// </summary>
    public class ModerationService : CRUDServiceBase<OpenAI>
    {
        private const string kEndpoint = "{ver}/moderations";
        public ModerationService(OpenAI client) : base(client) { }

        /// <summary>
        /// Classifies if text is potentially harmful.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async UniTask<SafetyRating[]> CreateAsync(ModerationRequest req)
        {
            if (req.SafetySettings.IsNullOrEmpty()) return null;
            if (req.Model == null) req.Model = OpenAISettings.DefaultMOD;

            ModerationResponse res = await client.POSTCreateAsync<ModerationRequest, ModerationResponse>(kEndpoint, this, req);
            if (res == null) return null;

            if (res.IsFlagged(out List<SafetyRating> ratings)) return ratings.ToArray();
            return null;
        }
    }
}