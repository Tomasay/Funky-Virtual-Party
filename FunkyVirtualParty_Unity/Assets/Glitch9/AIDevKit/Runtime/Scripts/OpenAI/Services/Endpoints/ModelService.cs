using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Models Service: https://platform.openai.com/docs/api-reference/models
    /// </summary>
    public class ModelService : CRUDService<OpenAI, OpenAIModelData, object, CursorQuery>
    {
        private const string kEndpoint = "{ver}/models";
        public ModelService(OpenAI client) : base(kEndpoint, client) { }
    }
}