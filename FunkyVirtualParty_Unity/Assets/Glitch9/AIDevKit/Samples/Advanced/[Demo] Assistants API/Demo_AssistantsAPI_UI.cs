using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.OpenAI.Assistants;
using Glitch9.AIDevKit.OpenAI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Glitch9.AIDevKit.Components;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoChatGPT_AssistantsAPI_UI : MonoBehaviour
    {
        private static class StatusTexts
        {
            internal const string INITIALIZING = "AssistantsAPI is initializing...";
            internal const string INITIALIZED = "AssistantsAPI is initialized";
        }

        [SerializeField] private Text threadInfoText;
        [SerializeField] private DemoThreadContainer threadContainer;
        [SerializeField] private DemoChatView chatUI;
        [SerializeField] private AssistantChatbot api;
        private bool _isInitialized = false;


        private void Start()
        {
            try
            {
                InitializeAsync().Forget();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private async UniTask InitializeAsync()
        {
            if (_isInitialized) return;

            // Null check all inspector fields
            if (threadInfoText == null || threadContainer == null || chatUI == null)
            {
                Debug.LogError("Initialization failed: Inspector fields are not set");
                return;
            }

            chatUI.SetStatus(DemoStatus.Loading, StatusTexts.INITIALIZING);
            chatUI.Initialize(api);

            // AssistantEventHandler handler = new();
            // handler.onTextCreated += (_, _) => chatUI.OnTextCreated();
            // handler.onTextDelta += (_, text) => chatUI.OnTextDelta(text);
            // handler.onThreadCreated += OnTreadCreated;
            // handler.onThreadRetrieved += SelectThread;
            // handler.onRunStatusChanged += OnRunStatusChanged;
            // handler.onStreamDone += (_, _) => chatUI.OnStreamDone();
            // handler.onError += chatUI.OnError;

            try
            {
                await api.InitializeAsync();
            }
            catch (Exception e)
            {
                chatUI.OnError($"Initialization failed: {e.Message}");
                Debug.LogError(e.StackTrace);
                return;
            }

            List<string> threadIds = api.Controller.GetThreadIds();

            threadContainer.Initialize(api, threadIds);
            threadContainer.onThreadSelect += OnThreadSelected;

            _isInitialized = true;

            chatUI.SetStatus(DemoStatus.Success, StatusTexts.INITIALIZED);
            chatUI.interactable = true;

            threadContainer.SelectThread(api.Controller.Thread);
        }

        private void SelectThread(object sender, Thread thread)
        {
            threadContainer.SelectThread(thread);
        }

        private void OnRunStatusChanged(object sender, RunStatus runStatus)
        {
            DemoStatus demoStatus = DemoStatus.Success;

            switch (runStatus)
            {
                case RunStatus.Queued:
                case RunStatus.InProgress:
                case RunStatus.Cancelling:
                    demoStatus = DemoStatus.Loading;
                    break;
                case RunStatus.RequiresAction:
                case RunStatus.Expired:
                    demoStatus = DemoStatus.Warning;
                    break;
                case RunStatus.Failed:
                case RunStatus.Incomplete:
                    demoStatus = DemoStatus.Error;
                    break;
            }

            chatUI.SetStatus(demoStatus, runStatus.GetMessage());
        }

        private void OnTreadCreated(object sender, Thread thread)
        {
            threadContainer.OnThreadCreated(thread);
        }

        private async void OnThreadSelected(Thread thread)
        {
            if (thread == null)
            {
                Debug.LogError("Selected thread is null");
                return;
            }
            Debug.Log($"Selected thread: {thread.Id}");
            threadInfoText.text = thread.Id;

            // Clear chat box ad chat list
            chatUI.Clear();


            try
            {
                // Get chat history
                ThreadMessage[] threadMessages = await thread.GetMessagesAsync();

                if (threadMessages.IsNullOrEmpty())
                {
                    Debug.Log($"Thread {thread.Id} has no messages");
                    return;
                }

                Debug.Log($"Thread {thread.Id} has {threadMessages.Length} messages");

                foreach (ThreadMessage threadMessage in threadMessages)
                {
                    if (threadMessage == null) continue;

                    ChatMessage localMessage = threadMessage.ToChatMessage();

                    if (localMessage.Role == ChatRole.User)
                    {
                        chatUI.AddUserChat(localMessage);
                    }
                    else
                    {
                        chatUI.AddAssistantChat(localMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}