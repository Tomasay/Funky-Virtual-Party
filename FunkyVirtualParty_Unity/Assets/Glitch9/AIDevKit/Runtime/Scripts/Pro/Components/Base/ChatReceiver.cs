using UnityEngine;


namespace Glitch9.AIDevKit.Components
{
    public abstract class ChatReceiver : MonoBehaviour, IChatReceiver
    {
        public abstract void OnSendMessage(ChatMessage requestMessage);
        public abstract void OnReceiveResponse(ChatCompletion response);
    }

    public interface IChatReceiver
    {
        void OnSendMessage(ChatMessage requestMessage);
        void OnReceiveResponse(ChatCompletion response);
    }
}