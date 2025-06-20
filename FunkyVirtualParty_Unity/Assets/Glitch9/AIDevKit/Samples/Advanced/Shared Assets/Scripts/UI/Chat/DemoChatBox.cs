using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoChatBox : MonoBehaviour
    {
        private const string kUserName = "User";
        private const string kModeration = "Moderation";
        private const string kUsageFormat = "Input:{0} Output:{1}";
        private const string kUnknownUsage = "-";

        [SerializeField] private Text nameTxt;
        [SerializeField] private Text usageTxt;
        [SerializeField] private Text chatTxt;
        [SerializeField] private Transform imageContainer;

        private bool _imagesLoaded = false;
        private List<File<Texture2D>> _imageFiles;

        public static DemoChatBox StartAssistantChatBox(DemoChatBox prefab, Transform parent, bool showUsage = true)
        {
            DemoChatBox chatBox = Instantiate(prefab, parent);
            if (showUsage) chatBox.usageTxt.text = kUnknownUsage;
            else chatBox.usageTxt.gameObject.SetActive(false);
            chatBox.StartTyping();
            return chatBox;
        }

        public void SetMessage(ChatMessage message, bool showUsage = true)
        {
            StopTyping();

            chatTxt.text = message.Content;

            if (message is UserMessage userMessage)
            {
                nameTxt.text = kUserName;


                if (!userMessage.AttachedFiles.IsNullOrEmpty())
                {
                    _imageFiles = new List<File<Texture2D>>();

                    foreach (IFile f in userMessage.AttachedFiles)
                    {
                        if (f is not File<Texture2D> imgFile) continue;
                        _imageFiles.Add(imgFile);
                    }

                    if (_imageFiles.Count == 0) return;
                    LoadImagesAsync().Forget();
                }
            }
            else if (message is ResponseMessage assistantMessage)
            {
                if (showUsage)
                {
                    if (assistantMessage.Usage == null)
                    {
                        usageTxt.text = kUnknownUsage;
                    }
                    else
                    {
                        usageTxt.text = string.Format(
                            kUsageFormat,
                            assistantMessage.Usage.InputTokens ?? 0,
                            assistantMessage.Usage.OutputTokens ?? 0
                        );
                    }
                }
            }
        }

        public void SetModerationText(ChatRole role, string text)
        {
            StopTyping();
            nameTxt.text = role == ChatRole.User ? kUserName : kModeration;
            chatTxt.text = text;
            usageTxt.gameObject.SetActive(false);
        }

        private async UniTask LoadImagesAsync()
        {
            if (_imagesLoaded) return;
            _imagesLoaded = true;

            int index = 0;

            foreach (File<Texture2D> image in _imageFiles)
            {
                await DemoUtil.CreateImage(index, image, imageContainer);
                index++;
            }
        }

        public void SetStreamingText(string textDelta)
        {
            StopTyping();
            chatTxt.text = textDelta;
        }

        public void StartTyping()
        {
            chatTxt.text = "...";
            StartCoroutine(TypingRoutine());
        }

        private IEnumerator TypingRoutine()
        {
            // max 8 dots.
            string[] dots = new string[] { ".", "..", "...", "....", ".....", "......", ".......", "........" };
            int dotIndex = 0;

            while (true)
            {
                chatTxt.text = dots[dotIndex];
                dotIndex = (dotIndex + 1) % dots.Length;
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void StopTyping()
        {
            StopAllCoroutines();
            chatTxt.text = string.Empty;
        }
    }
}