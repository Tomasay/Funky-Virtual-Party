using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    public partial class BetaService : CRUDServiceBase<OpenAI>
    {
        /// <summary>
        /// Build assistants that can call models and use tools to perform tasks.
        /// </summary>
        public AssistantService Assistants { get; }

        /// <summary>
        /// Create threads that assistants can interact with.
        /// </summary>
        public ThreadService Threads { get; }

        /// <summary>
        /// Create and manage vector stores.
        /// </summary>
        public VectorStoreService VectorStores { get; }

        /// <summary>
        /// Realtime API for creating and managing conversations.
        /// </summary>
        //public RealtimeService Realtime { get; }

        public BetaService(OpenAI client) : base(client, OpenAI.AssistantsApiHeader)
        {
            Assistants = new AssistantService(client, OpenAI.AssistantsApiHeader);
            Threads = new ThreadService(client, OpenAI.AssistantsApiHeader);
            VectorStores = new VectorStoreService(client, OpenAI.AssistantsApiHeader);
            //Realtime = new RealtimeService();
        }
    }
}