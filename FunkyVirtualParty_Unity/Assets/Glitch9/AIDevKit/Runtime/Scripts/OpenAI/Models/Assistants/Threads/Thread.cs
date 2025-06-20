using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Thread object that assistants can interact with.
    /// <para>Renamed from ThreadObject to Thread (2024.06.14)</para>
    /// </summary>
    public class Thread : AIResponse { }

    public interface IThreadEventReceiver
    {
        void OnThreadCreated(Thread thread);
        void OnThreadRetrieved(Thread thread);
        void OnThreadUpdated(Thread thread);
    }

    public static class ThreadExtensions
    {
        public static async UniTask<ThreadMessage[]> GetMessagesAsync(this Thread thread, int limit = 20)
        {
            if (thread == null || string.IsNullOrEmpty(thread.Id))
            {
                OpenAI.DefaultInstance.Logger.Error("Thread or Thread ID is null or empty.");
                return null;
            }

            QueryResponse<ThreadMessage> queryResponse = await OpenAI.DefaultInstance.Beta.Threads.Messages.ListAsync(thread.Id, new(limit));
            return queryResponse?.Data;
        }
    }
}