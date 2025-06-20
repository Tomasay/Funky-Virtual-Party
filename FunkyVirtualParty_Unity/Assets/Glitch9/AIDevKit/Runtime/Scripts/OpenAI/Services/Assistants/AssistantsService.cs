using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    public partial class BetaService
    {
        /// <summary>
        /// Assistants: https: //platform.openai.com/docs/api-reference/assistants
        /// </summary>
        public class AssistantService : CRUDService<OpenAI, Assistant, AssistantRequest, CursorQuery>
        {
            private const string kEndpoint = "{ver}/assistants";
            public AssistantService(OpenAI client, params RESTHeader[] betaHeaders) : base(kEndpoint, client, betaHeaders) { }
        }
    }
}