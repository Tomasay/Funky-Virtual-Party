using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Services
{
    /// <summary>
    /// Threads: https://platform.openai.com/docs/api-reference/threads
    /// </summary>
    public class ThreadService : CRUDService<OpenAI, Thread, ThreadRequest, CursorQuery>
    {
        private const string kEndpoint = "{ver}/threads";

        /// <summary>
        /// Create messages within threads
        /// </summary>
        public MessageService Messages { get; }

        /// <summary>
        /// Represents an execution run on a thread.
        /// </summary>
        public RunService Runs { get; }

        public ThreadService(OpenAI client, params RESTHeader[] extraHeaders) : base(kEndpoint, client, extraHeaders)
        {
            Messages = new MessageService(client, extraHeaders);
            Runs = new RunService(client, extraHeaders);
        }
    }
}