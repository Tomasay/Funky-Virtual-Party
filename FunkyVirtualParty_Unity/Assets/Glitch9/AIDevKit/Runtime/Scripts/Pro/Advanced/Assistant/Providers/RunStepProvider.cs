using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    internal class RunStepProvider : AssistantProviderBase<RunStep>
    {
        internal RunStepProvider(AssistantController controller, AssistantLogger logger) : base(controller, logger) { }

        protected override async UniTask<RunStep> RetrieveInternalAsync(string id, params object[] args)
        {
            return await Controller.Client.Beta.Threads.Runs.Steps.RetrieveAsync(Controller.ThreadId, Controller.RunId, id);
        }

        protected override async UniTask<RunStep[]> ListInternalAsync(params object[] args)
        {
            var queryResponse = await Controller.Client.Beta.Threads.Runs.Steps.ListAsync(Controller.ThreadId, Controller.RunId);
            return queryResponse?.Data;
        }

        protected override UniTask<RunStep> CreateInternalAsync(params object[] args) => throw new NotSupportedCrudOperationException(this, Api.OpenAI, CRUDMethod.Create);
        protected override UniTask<RunStep> UpdateInternalAsync(string id, params object[] args) => throw new NotSupportedCrudOperationException(this, Api.OpenAI, CRUDMethod.Update);
        protected override UniTask<bool> DeleteInternalAsync(string id, params object[] args) => throw new NotSupportedCrudOperationException(this, Api.OpenAI, CRUDMethod.Delete);
    }
}