using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    internal class MessageProvider : AssistantProviderBase<ThreadMessage>
    {
        internal MessageProvider(AssistantController controller, AssistantLogger logger) : base(controller, logger) { }
        protected override async UniTask<ThreadMessage> CreateInternalAsync(params object[] args)
        {
            return await Controller.Client.Beta.Threads.Messages.CreateAsync(Controller.Thread.Id, Controller.LastRequest);
        }

        protected override async UniTask<ThreadMessage> RetrieveInternalAsync(string id, params object[] args)
        {
            return await Controller.Client.Beta.Threads.Messages.RetrieveAsync(Controller.Thread.Id, id);
        }

        protected override async UniTask<ThreadMessage[]> ListInternalAsync(params object[] args)
        {
            QueryResponse<ThreadMessage> queryResponse = await Controller.Client.Beta.Threads.Messages.ListAsync(Controller.Thread.Id);
            return queryResponse?.Data;
        }

        protected override async UniTask<bool> DeleteInternalAsync(string id, params object[] args)
        {
            return await Controller.Client.Beta.Threads.Messages.DeleteAsync(Controller.Thread.Id, id);
        }

        protected override UniTask<ThreadMessage> UpdateInternalAsync(string id, params object[] args) => throw new NotSupportedCrudOperationException(this, Api.OpenAI, CRUDMethod.Update);
    }
}