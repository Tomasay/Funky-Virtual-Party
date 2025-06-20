using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    /// <summary>
    /// Event handler for basic chat (non-streaming) responses.
    /// </summary>
    [AddComponentMenu("Chat Event Receiver (Single)")]
    public class SingleChatEventReceiver : ChatReceiver
    {
        /// <summary>
        /// Invoked when a message is successfully sent by the user.
        /// </summary>
        [Tooltip("Invoked when a message is successfully sent by the user.")]
        [SerializeField] UnityEvent<ChatMessage> onSendMessage;

        /// <summary>
        /// Invoked when a response message is received from the AI.
        /// </summary>
        [Tooltip("Invoked when a response message is received from the AI.")]
        [SerializeField] UnityEvent<ResponseMessage> onReceiveMessage;

        public override void OnSendMessage(ChatMessage chatMessage) => onSendMessage?.Invoke(chatMessage);
        public override void OnReceiveResponse(ChatCompletion response) => onReceiveMessage?.Invoke(response.FirstResponseMessage());
    }
}