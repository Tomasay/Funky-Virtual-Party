using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Google.Services
{
    public class CachedContentService : CRUDService<GenerativeAI, CachedContent, CachedContentRequest, TokenQuery>
    {
        private const string kEndpoint = "{ver}/cachedContents";
        public CachedContentService(GenerativeAI client) : base(kEndpoint, client, IsBeta.CachedContents) { }
    }
}