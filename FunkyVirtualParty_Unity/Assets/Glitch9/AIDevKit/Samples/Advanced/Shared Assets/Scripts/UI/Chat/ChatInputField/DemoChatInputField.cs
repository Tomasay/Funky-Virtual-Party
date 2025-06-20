using Glitch9.IO.Files;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoChatInputField : MonoBehaviour
    {
        [SerializeField] private InputField userInput;
        [SerializeField] private DemoSendButton sendButton;
        [SerializeField] private DemoAttachButton attachButton;

        public bool interactable
        {
            get => _interactable;
            set
            {
                if (_interactable == value) return;
                _interactable = value;
                userInput.interactable = value;
                attachButton.interactable = value;
                if (!value) sendButton.interactable = false;
            }
        }
        private bool _interactable;

        public string text
        {
            get => userInput.text;
            set => userInput.text = value;
        }

        public void Initialize(Action onSend, Action<File<Texture2D>> onAttach)
        {
            // check null for inspector fields
            if (userInput == null || sendButton == null || attachButton == null)
            {
                Debug.LogError("Inspector fields are not set");
            }

            sendButton.onSend += onSend;
            attachButton.onAttach += onAttach;
            userInput.onValueChanged.AddListener(OnInputFieldValueChanged);
        }

        private void OnInputFieldValueChanged(string inputText)
        {
            sendButton.interactable = userInput.interactable && !string.IsNullOrWhiteSpace(inputText);
        }
    }
}