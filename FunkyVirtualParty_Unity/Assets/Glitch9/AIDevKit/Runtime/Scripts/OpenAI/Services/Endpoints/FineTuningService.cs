using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Fine-tuning Service: https://platform.openai.com/docs/api-reference/fine-tuning
    /// </summary>
    public class FineTuningService
    {
        public FineTuningJobService Jobs { get; }
        public FineTuningService(OpenAI client) => Jobs = new FineTuningJobService(client);
    }

    public class FineTuningJobService : CRUDService<OpenAI, FineTuningJob, FineTuningRequest, CursorQuery>
    {
        private const string kEndpoint = "{ver}/fine_tuning/jobs";
        private const string kEventEndpoint = "{ver}/fine_tuning/jobs/{0}/events/{1}";
        private const string kCancelEndpoint = "{ver}/fine_tuning/jobs/{0}/cancel";
        private const string kResumeEndpoint = "{ver}/fine_tuning/jobs/{0}/resume";

        public FineTuningJobService(OpenAI client) : base(endpoint: kEndpoint, client: client) { }

        public async UniTask<FineTuningJob> CancelAsync(string objectId, RequestOptions options = null)
        {
            return await client.POSTCreateAsync<FineTuningJob>(kCancelEndpoint, this, options, PathParam.ID(objectId));
        }

        public async UniTask<FineTuningJob> ResumeAsync(string objectId, RequestOptions options = null)
        {
            return await client.POSTCreateAsync<FineTuningJob>(kResumeEndpoint, this, options, PathParam.ID(objectId));
        }

        public async UniTask<QueryResponse<FineTuningEvent>> ListEventsAsync(string fineTuningJobId, CursorQuery query = null, RequestOptions reqOptions = null)
        {
            return await client.GETListAsync<CursorQuery, FineTuningEvent>(kEventEndpoint, this, query, reqOptions, PathParam.ID(fineTuningJobId));
        }
    }
}