using Glitch9.AIDevKit.Components;
using UnityEngine;

namespace Glitch9.AIDevKit.Demo
{
    public class Demo_Chatbot : MonoBehaviour
    {
        private static class StatusTexts
        {
            internal const string INITIALIZING = "Chat Completion is initializing...";
            internal const string INITIALIZED = "Chat Completion is initialized";
            internal const string CHAT_CLEARED = "Chat has been cleared";
        }

        [SerializeField] private DemoChatView chatUI;
        [SerializeField] private Chatbot chatbot;

        private bool _isInitialized = false;

        private void Start() => Initialize();

        private void Initialize()
        {
            if (_isInitialized) return;

            // Null check all inspector fields
            if (chatbot == null || chatUI == null)
            {
                Debug.LogError("Initialization failed: Inspector fields are not set");
                return;
            }

            chatUI.SetStatus(DemoStatus.Loading, StatusTexts.INITIALIZING);
            chatUI.Initialize(chatbot);

            _isInitialized = true;
            chatUI.SetStatus(DemoStatus.Success, StatusTexts.INITIALIZED);
            chatUI.interactable = true;
        }

        public void ClearChat()
        {
            chatUI.Clear();
            chatUI.SetStatus(DemoStatus.Success, StatusTexts.CHAT_CLEARED);
        }
    }
}