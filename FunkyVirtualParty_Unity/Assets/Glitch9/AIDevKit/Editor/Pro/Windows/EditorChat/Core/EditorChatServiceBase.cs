using System;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Advanced.Chat;
using Glitch9.Editor;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal enum EditorChatState { Idle, WaitingForResponse, Streaming }
    internal abstract class EditorChatServiceBase
    {
        internal abstract bool IsInitialized { get; }
        internal EditorChatState CurrentState { get; set; } = EditorChatState.Idle;
        internal virtual string CurrentSessionId => CurrentSession?.Value?.Id;
        internal virtual Result<ChatSession> CurrentSession
        {
            get => _window.CurrentSession;
            set => _window.CurrentSession = value;
        }
        internal string StreamingText { get; set; } = string.Empty;
        protected EditorChatWindow _window;

        internal EditorChatServiceBase(EditorChatWindow window) => _window = window;

        internal bool SendRequest(ChatMessage inputMessage, Action<ResponseMessage> onSuccess = null)
        {
            if (inputMessage == null)
            {
                Debug.LogWarning("Chat message is null or empty. Cannot send request.");
                return false;
            }

            if (!ValidateState()) return false;

            PrepareRequest();
            SendRequestINTERNAL(inputMessage, onSuccess);

            return true;
        }


        internal bool SendEditRequest(int index, string editedContent)
        {
            if (string.IsNullOrWhiteSpace(editedContent))
            {
                Debug.LogWarning("Edited content is null or empty. Cannot send edit request.");
                return false;
            }

            if (!ValidateState()) return false;

            PrepareRequest();
            SendEditRequestINTERNAL(index, editedContent);

            return true;
        }

        protected abstract void SendRequestINTERNAL(ChatMessage inputMessage, Action<ResponseMessage> onSuccess);
        protected abstract void SendEditRequestINTERNAL(int index, string editedContent);

        internal void CancelRequest()
        {
            CancelRequestINTERNAL();
            FinalizeRequest();
        }

        protected abstract void CancelRequestINTERNAL();
        protected Func<UniTask<string>> CreateSessionIdDelegate;

        internal virtual void OnSetChatSession(string id) { }
        internal virtual void OnDeleteChatSession(string id) { }

        protected virtual void OnStreamStart()
        {
            StreamingText = string.Empty;
            CurrentState = EditorChatState.Streaming;
        }

        protected virtual void OnStreamDelta(string text)
        {
            // AIDevKitDebug.Log($"OnStreamDelta: {text}");
            if (string.IsNullOrEmpty(text)) return;
            StreamingText += text;

            _window.UpdateStreamingText(StreamingText);
        }

        protected virtual void OnStreamDone(ChatCompletion result)
        {
            if (string.IsNullOrEmpty(StreamingText)) return;

            ResponseMessage aiChat = new()
            {
                Content = StreamingText,
                Usage = result?.Usage,
            };

            OnReceiveResponse(aiChat);
            StreamingText = string.Empty;
        }

        internal void OnRequestFailed(string errorMessage = null)
        {
            if (EditorChatSettings.DebugMode) AIDevKitDebug.Error($"OnRequestFailed: {errorMessage}");
            _window.ShowErrorMessage(errorMessage ?? "An error occurred while processing the request.");
            CancelRequest();
        }

        protected void OnReceiveResponse(ResponseMessage responseMessage)
        {
            if (responseMessage != null)
            {
                _window.FinalizeResponse(responseMessage);

                if (EditorChatSettings.TextToSpeechEnabled)
                {
                    PlayVoiceAync(responseMessage.Content);
                    return;
                }
            }

            FinalizeRequest();

            // if (EditorChatSettings.TextToSpeechEnabled)
            // {
            //     AudioClip clip = await responseMessage.GetAudioClipAsync(null, MIMEType.WAV);

            //     if (clip != null)
            //     {
            //         EditorAudioPlayer.Play(clip);
            //     }
            //     else
            //     {
            //         Debug.LogWarning("OnReceiveResponse: AudioClip is null. Cannot play voice.");
            //     }
            // }

            // _window.RebuildUsageTracking();
            // FinalizeRequest();
        }

        private async void PlayVoiceAync(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            try
            {
                _window.ProgressBar?.ShowRequestProgressBar(RequestType.TTS, EditorChatSettings.TextToSpeechModel);

                GeneratedAudio audio = await text.GENSpeech()
                    .SetModel(EditorChatSettings.TextToSpeechModel)
                    .SetVoice(EditorChatSettings.TextToSpeechVoice)
                    .SetSender(EditorChatConfig.kSenderName)
                    .SetIgnoreLogs(!EditorChatSettings.DebugMode)
                    .ExecuteAsync()
                    ?? throw new EmptyResponseException("PlayVoiceAync: GeneratedAudio is null. Please check your Text-to-Speech settings.");

                FinalizeRequest();

                AudioClip clip = audio;
                Usage usage = audio.Usage;

                if (clip != null) EditorAudioPlayer.Play(clip);
                if (usage != null)
                {
                    Model ttsModel = EditorChatSettings.TextToSpeechModel;

                    if (ttsModel == null)
                    {
                        Debug.LogWarning("PlayVoiceAync: TTS model is null, cannot calculate usage cost.");
                        return;
                    }

                    // log model name
                    if (EditorChatSettings.DebugMode) AIDevKitDebug.Info($"PlayVoiceAync: Using TTS model '{ttsModel.SafeGetName()}' for text-to-speech.");

                    Currency cost = ttsModel.EstimatePrice(usage);
                    if (cost == null)
                    {
                        Debug.LogWarning("PlayVoiceAync: Model estimate price returned null, cannot calculate cost.");
                        return;
                    }

                    if (EditorChatSettings.DebugMode) AIDevKitDebug.Info($"PlayVoiceAync: Estimated cost for TTS usage is {cost.PriceInUsd} USD.");
                    _window.CurrentSession?.Value?.AddUsage(usage, cost);
                    _window.RebuildUsageTracking();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"PlayVoiceAync: {e.Message}\n{e.StackTrace}");
                FinalizeRequest();
            }
        }

        private bool ValidateState()
        {
            if (CurrentState != EditorChatState.Idle)
            {
                Debug.LogWarning("Chat service is not in idle state. Cannot send request.");
                _window.ProgressBar?.HideProgressBar();
                return false;
            }
            return true;
        }

        private void PrepareRequest()
        {
            CurrentState = EditorChatState.WaitingForResponse;
            _window.ProgressBar?.ShowRequestProgressBar(RequestType.LLM, EditorChatSettings.CurrentModel?.Id);
        }

        private void FinalizeRequest()
        {
            _window.ProgressBar?.HideProgressBar();
            CurrentState = EditorChatState.Idle;
        }
    }
}