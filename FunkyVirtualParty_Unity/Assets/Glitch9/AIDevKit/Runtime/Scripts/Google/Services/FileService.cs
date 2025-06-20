using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Google.Services
{
    public class FileService : CRUDService<GenerativeAI, GoogleFile, object, TokenQuery>
    {
        private const string kEndpoint = "{ver}/files";
        public FileService(GenerativeAI client) : base(kEndpoint, client, IsBeta.Files) { }
    }
}