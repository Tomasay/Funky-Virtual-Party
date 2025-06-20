using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    internal abstract class AssistantProviderBase<T> : CRUDProvider<T>
    {
        protected readonly AssistantController Controller;
        protected AssistantProviderBase(AssistantController controller, AssistantLogger logger) : base(logger: logger) => Controller = controller;
    }
}