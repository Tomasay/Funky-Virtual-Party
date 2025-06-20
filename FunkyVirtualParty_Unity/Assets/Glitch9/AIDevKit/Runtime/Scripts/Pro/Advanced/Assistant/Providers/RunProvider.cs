using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    internal class RunProvider : AssistantProviderBase<Run>
    {
        internal RunProvider(AssistantController controller, AssistantLogger logger) : base(controller, logger) { }

        protected override async UniTask<Run> CreateInternalAsync(params object[] args)
        {
            RunRequest runRequest;

            if (args.Length == 1 && args[0] is RunRequest customRunRequest)
            {
                runRequest = Controller.PrepareRunRequest(customRunRequest);
            }
            else
            {
                runRequest = Controller.DefaultRunRequest;
                runRequest = Controller.PrepareRunRequest(runRequest);
            }

            //UnityEngine.Debug.LogError(runRequest.ToString());
            return await Controller.Client.Beta.Threads.Runs.CreateAsync(Controller.ThreadId, runRequest);
        }

        protected override async UniTask<Run> RetrieveInternalAsync(string id, params object[] args)
        {
            //RunObject run = await ValidateCurrentStatus();
            //if (run != null) return run;
            return await CreateInternalAsync(args);
        }

        protected override async UniTask<Run[]> ListInternalAsync(params object[] args)
        {
            QueryResponse<Run> queryResponse = await Controller.Client.Beta.Threads.Runs.ListAsync(Controller.ThreadId);
            return queryResponse?.Data;
        }

        /// <summary>
        /// Incomplete method.
        /// Do not use.
        /// </summary>
        /// <returns></returns>
        protected virtual async UniTask<Run> ValidateCurrentStatus()
        {
            QueryResponse<Run> queryResponse = await Controller.Client.Beta.Threads.Runs.ListAsync(Controller.ThreadId, new(1));
            Run[] runList = queryResponse?.Data;

            if (!runList.IsNullOrEmpty())
            {
                Run run = runList[0];
                if (run != null && run.Status != null)
                {

                }
            }
            return null;
        }

        protected override UniTask<Run> UpdateInternalAsync(string id, params object[] args) => throw new NotSupportedCrudOperationException(this, Api.OpenAI, CRUDMethod.Update);
        protected override UniTask<bool> DeleteInternalAsync(string id, params object[] args) => throw new NotSupportedCrudOperationException(this, Api.OpenAI, CRUDMethod.Delete);

    }
}