using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;
using System;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Run: https://platform.openai.com/docs/api-reference/runs
    /// </summary>
    public class RunService : CRUDServiceBase<OpenAI>
    {
        private const string kEndpoint = "{ver}/threads/{0}/runs";
        private const string kEndpointWithId = "{ver}/threads/{0}/runs/{1}";
        private const string kCancelEndpoint = "{ver}/threads/{0}/runs/{1}/cancel";
        public RunStepService Steps { get; }

        public RunService(OpenAI client, params RESTHeader[] extraHeaders) : base(client, extraHeaders)
        {
            Steps = new RunStepService(client, extraHeaders);
        }

        public async UniTask<Run> CreateAsync(string threadId, RunRequest req)
        {
            return await client.POSTCreateAsync<RunRequest, Run>(kEndpoint, this, req, PathParam.ID(threadId));
        }

        public async UniTask<QueryResponse<Run>> ListAsync(string threadId, CursorQuery query = null, RequestOptions options = null)
        {
            return await client.GETListAsync<CursorQuery, Run>(kEndpoint, this, query, options, PathParam.ID(threadId));
        }

        public async UniTask<Run> Retrieve(string threadId, string runId, RequestOptions options = null)
        {
            return await client.GETRetrieveAsync<Run>(kEndpointWithId, this, options, PathParam.ID(threadId, runId));
        }

        public async UniTask<Run> UpdateAsync(string threadId, string runId, RunRequest req)
        {
            return await client.POSTUpdateAsync<RunRequest, Run>(kEndpointWithId, this, req, PathParam.ID(threadId, runId));
        }

        public async UniTask<Run> CancelAsync(string threadId, string runId, RequestOptions options = null)
        {
            return await client.POSTCreateAsync<Run>(kCancelEndpoint, this, options, PathParam.ID(threadId, runId));
        }

        public async UniTask<Run> SubmitToolOutputsAsync(string threadId, string runId, ToolOutputsRequest req)
        {
            try
            {
                const string CHILD_PATH = "submit_tool_outputs";
                ThrowIf.ArgumentIsNull(req);
                ThrowIf.IsNullOrEmpty(threadId, nameof(threadId));
                ThrowIf.IsNullOrEmpty(runId, nameof(runId));

                return await client.POSTCreateAsync<ToolOutputsRequest, Run>(kEndpointWithId, this, req, PathParam.ID(threadId, runId), PathParam.Child(CHILD_PATH));
            }
            catch (Exception e)
            {
                client.HandleException(e);
                return null;
            }
        }
    }

    public class RunStepService : CRUDServiceBase<OpenAI>
    {
        private const string kListEndpoint = "{ver}/threads/{0}/runs/{1}/steps";
        private const string kGetEndpoint = "{ver}/threads/{0}/runs/{1}/steps/{2}";
        public RunStepService(OpenAI client, params RESTHeader[] extraHeaders) : base(client, extraHeaders) { }
        public async UniTask<RunStep> RetrieveAsync(string threadId, string runId, string stepId, RequestOptions options = null)
        => await client.GETRetrieveAsync<RunStep>(kGetEndpoint, this, options, PathParam.ID(threadId, runId, stepId));
        public async UniTask<QueryResponse<RunStep>> ListAsync(string threadId, string runId, CursorQuery query = null, RequestOptions options = null)
        => await client.GETListAsync<CursorQuery, RunStep>(kListEndpoint, this, query, options, PathParam.ID(threadId, runId));
    }
}
