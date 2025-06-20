using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    /// <summary>
    /// Simplifies implementation of HTTP operations for handling <see cref="Thread"/> for <see cref="AssistantController"/>.
    /// </summary>
    internal class ThreadProvider : AssistantProviderBase<Thread>
    {
        internal ThreadProvider(AssistantController controller, AssistantLogger logger) : base(controller, logger) { }

        protected override async UniTask<Thread> CreateInternalAsync(params object[] args)
        {
            return await Controller.Client.Beta.Threads.CreateAsync(new());
        }

        protected override async UniTask<Thread> RetrieveInternalAsync(string id, params object[] args)
        {
            if (!string.IsNullOrEmpty(id))
            {
                _logger.Info("Previous thread id found. Trying to retrieve the thread.");
                Thread newThread = await Controller.Client.Beta.Threads.RetrieveAsync(id);
                if (newThread != null) return newThread;
            }
            else
            {
                _logger.Info("Thread id doesn't exist.");
            }

            return null;
        }

        protected override async UniTask<bool> DeleteInternalAsync(string id, params object[] args)
        {
            return await Controller.Client.Beta.Threads.DeleteAsync(id);
        }

        protected override UniTask<Thread> UpdateInternalAsync(string id, params object[] args) => throw new NotSupportedCrudOperationException(this, Api.OpenAI, CRUDMethod.Update);
        protected override UniTask<Thread[]> ListInternalAsync(params object[] args) => throw new NotSupportedCrudOperationException(this, Api.OpenAI, CRUDMethod.Query);
    }
}