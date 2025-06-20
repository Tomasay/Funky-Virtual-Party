using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit.Components
{
    public abstract class ChatbotComponent : AIDevKitComponent, IChatbot
    {
        // Modules
        [SerializeField] protected FunctionManager functionManager;
        [SerializeField] protected SpeechToText sttModule;
        [SerializeField] protected TextToSpeech ttsModule;
        [SerializeField] protected ImageGenerator imageModule;
        [SerializeField] protected Moderator moderatorModule;

        // Receivers
        [SerializeField] protected ChatReceiver chatEventReceiver;
        [SerializeField] protected ToolCallReceiver toolCallReceiver;

        public abstract string Name { get; }
        public abstract List<ChatMessage> Messages { get; }
        public abstract Model Model { get; }
        public abstract bool Stream { get; }
        public abstract bool IsInitialized { get; }
        internal readonly ResponseTypeResolver responseTypeResolver = new();

        public void StartRecording()
        {
            if (sttModule == null)
            {
                OnError("SpeechToText module is not set.");
                return;
            }

            sttModule.StartRecording();
        }

        public async UniTask<ChatMessage> StopRecording()
        {
            if (sttModule == null)
            {
                OnError("SpeechToText module is not set.");
                return null;
            }

            try
            {
                Transcript transcript = await sttModule.StopRecording();
                if (transcript == null || string.IsNullOrWhiteSpace(transcript.Text))
                {
                    OnError("No speech detected or transcription failed.");
                    return null;
                }

                // Create a chat message from the transcript
                return await EnterChatAsync(transcript.Text);

            }
            catch (Exception ex)
            {
                OnError($"Error stopping recording: {ex.Message}");
                return null;
            }
        }

        public async UniTask<ChatMessage> EnterChatAsync(ChatMessage inputMessage)
        {
            if (inputMessage == null) return ResponseMessageFactory.ErrorMessage("Input message is null.");
            try
            {
                return await ProcessMessageRequestAsyncINTERNAL(inputMessage);
            }
            catch (Exception e)
            {
                OnError(e);
                return null;
            }
        }

        protected abstract UniTask<ChatMessage> SendMessageAsyncINTERNAL(ChatMessage inputMessage);
        protected async UniTask<ChatMessage> ProcessMessageRequestAsyncINTERNAL(ChatMessage inputMessage)
        {
            if (moderatorModule != null && inputMessage is UserMessage userMessage)
            {
                try
                {
                    // Moderate the input message
                    Moderation moderation = await moderatorModule.ModerateMessageAsync(userMessage, userMessage.AttachedFiles);

                    if (moderation != null && !moderation.IsEmpty)
                    {
                        if (moderation.IsFlagged)
                        {
                            OnError($"Message flagged by moderation: {moderation}");
                            return ResponseMessageFactory.FlaggedMessage(moderation);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnError($"Error in {nameof(SendMessageAsyncINTERNAL)}: {ex.Message}");
                    return null;
                }
            }

            if (imageModule != null)
            {
                try
                {
                    string textContent = inputMessage.Content;

                    if (!string.IsNullOrWhiteSpace(textContent))
                    {
                        ResponseType responseType = await responseTypeResolver.ResolveAsync(inputMessage.Content);

                        bool isRequestingImage = responseType == ResponseType.Image;

                        if (isRequestingImage)
                        {
                            // Generate image based on the input message
                            GeneratedImage image = await imageModule.GenerateImageAsync(textContent)
                                ?? throw new EmptyResponseException("Image generation returned null.");

                            return ResponseMessageFactory.FromImage(image);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnError($"Error in {nameof(SendMessageAsyncINTERNAL)}: {ex.Message}");
                    return null;
                }
            }

            ChatMessage response = await SendMessageAsyncINTERNAL(inputMessage);

            if (ttsModule != null)
            {
                string responseText = response?.Content;

                if (!string.IsNullOrWhiteSpace(responseText))
                {
                    try
                    {
                        // Convert response text to speech
                        GeneratedAudio speech = await ttsModule.GenerateSpeechAsync(responseText)
                            ?? throw new EmptyResponseException("TTS returned null.");

                        // if (_audioSource != null)
                        // {
                        //     // Play the generated audio clip
                        //     _audioSource.clip = speech;
                        //     _audioSource.Play();
                        // }
                    }
                    catch (Exception ex)
                    {
                        OnError($"Error in TTS conversion: {ex.Message}");
                    }
                }
            }

            return response;
        }
    }
}