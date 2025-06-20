using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoChatView : MonoBehaviour
    {
        private static class StatusTexts
        {
            internal const string PROCESSING_MODERATION = "Processing moderation...";
            internal const string WAITING_FOR_RESPONSE = "Waiting for a response...";
            internal const string RESPONSE_RECEIVED = "Successfully received a response";
        }

        [SerializeField] private DemoStatusText statusText;
        [SerializeField] private ScrollRect chatScrollRect;
        [SerializeField] private DemoChatInputField chatInputField;
        [SerializeField] private DemoChatBox chatBoxPrefab;
        [SerializeField] private Transform chatBoxContainer;

        // image attachments
        [SerializeField] private DemoImagePrefab imagePrefab;
        [SerializeField] private Transform tempImageContainer;

        public bool interactable
        {
            get => chatInputField.interactable;
            set => chatInputField.interactable = value;
        }

        private List<ChatMessage> _chatList;
        private List<DemoChatBox> _chatBoxList;
        private List<File<Texture2D>> _imageFiles;

        private string _streamingText;
        private DemoChatBox _assistantChatBox;

        private IChatbot _chatbot;
        private bool _isInitialized = false;


        private void Start()
        {
            if (chatScrollRect == null || chatInputField == null ||
                tempImageContainer == null || chatBoxPrefab == null ||
                chatBoxContainer == null || imagePrefab == null ||
                statusText == null)
            {
                Debug.LogError("Chat UI inspector fields are not set.");
            }
        }

        public void Initialize(IChatbot chatbot)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _chatbot = chatbot;

            // Initialize chat input field
            chatInputField.Initialize(OnSendButtonClicked, OnAttachButtonClicked);
            chatInputField.interactable = false;

            _chatList = new List<ChatMessage>();
            _chatBoxList = new List<DemoChatBox>();
            _imageFiles = new List<File<Texture2D>>();
            _streamingText = string.Empty;

            List<ChatMessage> messages = chatbot.Messages;

            if (!messages.IsNullOrEmpty())
            {
                foreach (ChatMessage chat in messages)
                {
                    if (chat == null) continue;
                    if (chat.Role == ChatRole.User) AddUserChat(chat);
                    else if (chat.Role == ChatRole.Assistant) AddAssistantChat(chat);
                }
            }
        }

        public void SetStatus(DemoStatus status, string message)
        {
            Debug.Log(message);
            statusText.SetStatus(status, message);
        }

        private async void OnSendButtonClicked()
        {
            if (!_isInitialized)
            {
                OnError("DemoChatUI is not initialized");
                return;
            }

            string userMessage = chatInputField.text;
            if (string.IsNullOrEmpty(userMessage)) return;

            // Create input chat
            UserMessage inputChat = new(userMessage);

            if (_imageFiles.Count > 0) inputChat.AddAttachments(_imageFiles);

            chatInputField.text = string.Empty;
            _imageFiles.Clear();
            tempImageContainer.DestroyAllChildren();

            DemoChatBox userChatBox = AddUserChat(inputChat);
            if (userChatBox == null)
            {
                OnError("User chat box is null");
                return;
            }

            SetStatus(DemoStatus.Loading, StatusTexts.WAITING_FOR_RESPONSE);

            ChatMessage assistantChat;
            _assistantChatBox = StartAssistantChat();

            try
            {
                assistantChat = await _chatbot.EnterChatAsync(inputChat);

                if (!_chatbot.Stream)
                {
                    _chatList[_chatList.Count - 1] = inputChat;
                    userChatBox.SetMessage(inputChat);

                    SetStatus(DemoStatus.Success, StatusTexts.RESPONSE_RECEIVED);
                    OnAIResponseComplete(assistantChat);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                OnError(e.Message);

                assistantChat = new ResponseMessage(e.Message);

                SetStatus(DemoStatus.Success, StatusTexts.RESPONSE_RECEIVED);
                OnAIResponseComplete(assistantChat);
            }
        }


        private void OnAttachButtonClicked(File<Texture2D> image)
        {
            if (image == null)
            {
                Debug.LogError("Image file is null");
                return;
            }

            _imageFiles.Add(image);

            // Create image preview
            DemoImagePrefab imagePrefabInstance = Instantiate(imagePrefab, tempImageContainer);
            imagePrefabInstance.Initialize(image, OnImageRemoveButtonClicked).Forget();
        }

        private void OnImageRemoveButtonClicked(File<Texture2D> image)
        {
            if (image == null)
            {
                Debug.LogError("Image prefab is null");
                return;
            }

            _imageFiles.Remove(image);
            Destroy(imagePrefab.gameObject);
        }

        public void Clear()
        {
            chatBoxContainer.DestroyAllChildren();
            _chatList.Clear();
            _chatBoxList.Clear();
        }

        public DemoChatBox AddUserChat(ChatMessage userChat)
        {
            if (userChat == null)
            {
                Debug.LogError("User chat is null");
                return null;
            }

            DemoChatBox userChatBox = Instantiate(chatBoxPrefab, chatBoxContainer);
            userChatBox.SetMessage(userChat);

            _chatList.Add(userChat);
            _chatBoxList.Add(userChatBox);

            ScrollToBottom();
            return userChatBox;
        }

        public void AddAssistantChat(ChatMessage assistantChat)
        {
            if (assistantChat == null)
            {
                Debug.LogError("Assistant chat is null");
                return;
            }

            DemoChatBox assistantChatBox = Instantiate(chatBoxPrefab, chatBoxContainer);
            assistantChatBox.SetMessage(assistantChat);

            _chatList.Add(assistantChat);
            _chatBoxList.Add(assistantChatBox);

            ScrollToBottom();
        }

        private DemoChatBox StartAssistantChat()
        {
            DemoChatBox assistantChatBox = DemoChatBox.StartAssistantChatBox(chatBoxPrefab, chatBoxContainer);
            _chatBoxList.Add(assistantChatBox);

            ScrollToBottom();
            return assistantChatBox;
        }

        private void OnAIResponseComplete(ChatMessage assistantChat)
        {
            if (assistantChat == null)
            {
                Debug.LogError("Assistant chat is null");
                return;
            }

            if (_assistantChatBox != null)
            {
                _assistantChatBox.SetMessage(assistantChat);
                _assistantChatBox = null;
            }

            _chatList.Add(assistantChat);

            ScrollToBottom();
        }

        public void OnTextCreated()
        {
            // Do something
        }

        public void OnTextDelta(string textDelta)
        {
            //Debug.LogWarning($"Text delta: {textDelta}");
            _streamingText += textDelta;

            if (_assistantChatBox == null)
            {
                // Debug.LogError("Streaming box is null");
                // return;
                _assistantChatBox = StartAssistantChat();
            }

            _assistantChatBox.SetStreamingText(_streamingText);
        }

        public void OnStreamDone()
        {
            if (_assistantChatBox != null)
            {
                var finalizedChat = new ResponseMessage(_streamingText);
                _assistantChatBox.SetMessage(finalizedChat);
                _assistantChatBox = null;
            }

            _streamingText = string.Empty;
            SetStatus(DemoStatus.Success, StatusTexts.RESPONSE_RECEIVED);
        }

        public void OnError(string message)
        {
            Debug.LogError(message);
            SetStatus(DemoStatus.Error, message);
        }

        private void ScrollToBottom()
        {
            Canvas.ForceUpdateCanvases();
            chatScrollRect.verticalNormalizedPosition = 0;
        }
    }
}