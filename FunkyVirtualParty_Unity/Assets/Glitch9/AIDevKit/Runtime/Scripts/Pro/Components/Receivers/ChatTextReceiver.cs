using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    /// <summary>
    /// Event handler for basic chat (non-streaming) responses.
    /// </summary>
    [AddComponentMenu("Chat Receiver (Text)")]
    public class ChatTextReceiver : ChatReceiver
    {
        /// <summary>
        /// Invoked when a message is successfully sent by the user.
        /// </summary>
        [Tooltip("Invoked when a message is successfully sent by the user.")]
        [SerializeField] UnityEvent<string> onSendMessage;

        /// <summary>
        /// Invoked when a response message is received from the AI.
        /// </summary>
        [Tooltip("Invoked when a response message is received from the AI.")]
        [SerializeField] UnityEvent<string> onReceiveMessage;

        public override void OnSendMessage(ChatMessage chatMessage) => onSendMessage?.Invoke(chatMessage.ToString());
        public override void OnReceiveResponse(ChatCompletion response) => onReceiveMessage?.Invoke(response.ToString());
    }
}