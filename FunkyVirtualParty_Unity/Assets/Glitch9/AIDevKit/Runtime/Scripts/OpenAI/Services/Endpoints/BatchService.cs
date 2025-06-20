using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Batch Service: https://platform.openai.com/docs/api-reference/batch
    /// </summary>
    public class BatchService : CRUDService<OpenAI, Batch, BatchRequest, CursorQuery>
    {
        private const string kEndpoint = "{ver}/batches";
        private const string kCancelEndpoint = "{ver}/batches/{0}/cancel";

        public BatchService(OpenAI client) : base(kEndpoint, client) { }

        public async UniTask<Batch> CancelAsync(string objectId, RequestOptions options = null)
        {
            return await client.POSTCreateAsync<Batch>(kCancelEndpoint, this, options, PathParam.ID(objectId));
        }
    }
}