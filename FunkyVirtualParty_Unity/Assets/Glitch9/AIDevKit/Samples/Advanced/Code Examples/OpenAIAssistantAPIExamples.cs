using System;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.OpenAI.Assistants;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit.Examples
{
    [RequireComponent(typeof(AudioSource))]
    public class OpenAIAssistantExample : MonoBehaviour
    {
        [Header("Assistant Settings")]
        public string assistantId = "your_assistant_id";
        public string assistantName = "lenovo_avatar";
        [TextArea] public string assistantInstruction = "You are a helpful assistant.";
        [Header("Input")][TextArea] public string userTextInput;
        [Header("Output")][TextArea] public string gptResponse;
        private AssistantController api;
        private AudioSource audioSource;

        private async void Start()
        {
            audioSource = GetComponent<AudioSource>();
            await InitializeAssistant();
        }

        /// <summary>
        /// Initializes the Assistant with preset options.
        /// </summary>
        public async UniTask InitializeAssistant()
        {
            var settings = new AssistantSettings
            {
                AssistantId = assistantId,
                // Name = assistantName,
                // Description = "An AI assistant example",
                // Instructions = assistantInstruction,
                // Model = OpenAIModel.GPT4o,
                // ResponseFormat = TextFormat.Text
            };

            api = new AssistantController(settings);
            await api.InitializeAsync();
            Debug.Log("<color=green>[Assistant Initialized]</color>");
        }

        /// <summary>
        /// Sends a text message to the assistant and plays back TTS response.
        /// </summary>
        // public async UniTask SendTextAsync(string inputText)
        // {
        //     if (string.IsNullOrWhiteSpace(inputText))
        //     {
        //         Debug.LogWarning("Input is empty.");
        //         return;
        //     }

        //     try
        //     {
        //         AssistantResult response = await api.RequestAsync(inputText);
        //         if (!response.IsSuccess)
        //         {
        //             Debug.LogWarning("Assistant response failed.");
        //             return;
        //         }

        //         gptResponse = response.GetResult();
        //         Debug.Log($"[Assistant Reply] {gptResponse}");

        //         await PlayTTSAsync(gptResponse);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError($"[Assistant Error] {e}");
        //     }
        // }

        /// <summary>
        /// Sends an AudioClip to the assistant.
        /// </summary>
        // public async UniTask SendAudioAsync(AudioClip clip)
        // {
        //     if (clip == null)
        //     {
        //         Debug.LogWarning("AudioClip is null.");
        //         return;
        //     }

        //     try
        //     {
        //         AssistantResult response = await api.RequestAsync(clip);
        //         if (!response.IsSuccess)
        //         {
        //             Debug.LogWarning("Assistant response failed.");
        //             return;
        //         }

        //         gptResponse = response.GetResult();
        //         Debug.Log($"[Assistant Reply] {gptResponse}");

        //         await PlayTTSAsync(gptResponse);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError($"[Assistant Error] {e}");
        //     }
        // }

        /// <summary>
        /// Sends an image with a prompt to the assistant and prints the result.
        /// </summary>
        // public async UniTask SendVisionRequestAsync(string imageUrl, string prompt = "What is this image?")
        // {
        //     if (string.IsNullOrWhiteSpace(imageUrl))
        //     {
        //         Debug.LogWarning("Image URL is empty.");
        //         return;
        //     }

        //     try
        //     {
        //         var request = new ThreadMessageRequest()
        //             .SetPrompt(prompt)
        //             .SetUploadFiles(new File<Texture2D>(imageUrl));

        //         var response = await api.RequestAsync(request);
        //         gptResponse = response.GetResult();

        //         Debug.Log($"[Vision Result] {gptResponse}");
        //         await PlayTTSAsync(gptResponse);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError($"[Vision Request Error] {e}");
        //     }
        // }

        /// <summary>
        /// Converts a text to speech and plays it.
        /// </summary>
        private async UniTask PlayTTSAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var ttsRequest = new SpeechRequest.Builder()
                .SetPrompt(text)
                .Build();

            var clip = await ttsRequest.ExecuteAsync();

            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        /// <summary>
        /// Restarts the current assistant thread.
        /// </summary>
        public async UniTask RestartThread()
        {
            await api.CreateThreadAsync();
            Debug.Log("<color=yellow>[Assistant Thread Restarted]</color>");
        }
    }
}

