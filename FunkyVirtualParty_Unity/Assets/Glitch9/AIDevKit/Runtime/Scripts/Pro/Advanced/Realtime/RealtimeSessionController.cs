using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Components;
using Glitch9.IO.Networking.WebSocket;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    public class RealtimeSessionController
    {
        private readonly WebSocketClient<RealtimeEvent> _client;
        private readonly RealtimeEventDispatcher _eventDispatcher;
        private readonly RealtimeEventProcessor _eventProcessor;

        public IWebSocket WebSocket => _client.WebSocket;
        public WebSocketState WebSocketState => WebSocket?.State ?? WebSocketState.None;
        public RealtimeAudioFormat InputFormat { get; set; } = RealtimeAudioFormat.PCM16;
        public RealtimeAudioFormat OutputFormat { get; set; } = RealtimeAudioFormat.PCM16;

        public Model RTMModel { get; set; }
        public Model STTModel { get; set; }
        public Voice Voice { get; set; } = OpenAIVoice.Alloy;
        public SystemLanguage SpokenLanguage { get; set; } = SystemLanguage.English;
        private RealtimeSession _session;
        private bool _inputTranscriptionEnabled = false;
        private RESTLogger Logger => OpenAI.DefaultInstance.Logger;

        /// <summary>
        /// Create a new RealtimeSessionController instance.
        /// </summary>
        public RealtimeSessionController(
            IStreamingAudioEventReceiver audioEventReceiver,
            IRealtimeEventReceiver realtimeEventReceiver = null,
            IWebSocketEventReceiver webSocketEventReceiver = null,
            IStreamingTextEventReceiver inputTranscriptionEventReceiver = null,
            IStreamingTextEventReceiver textEventReceiver = null,
            IStreamingTextEventReceiver transcriptEventReceiver = null,
            IToolCallReceiver toolCallReceiver = null,
            Model rtmModel = null,
            Model sttModel = null,
            SystemLanguage spokenLanguage = SystemLanguage.English,
            FunctionManager functionManager = null,
            bool autoManageWebSocketState = false)
        {
            if (audioEventReceiver == null) throw new ExArgumentNullException(nameof(audioEventReceiver));
            if (rtmModel == null) rtmModel = OpenAISettings.DefaultRTM;
            if (sttModel == null) sttModel = OpenAISettings.DefaultSTT;

            RTMModel = rtmModel;
            STTModel = sttModel;
            SpokenLanguage = spokenLanguage;
            _inputTranscriptionEnabled = inputTranscriptionEventReceiver != null;

            _eventDispatcher = new RealtimeEventDispatcher(
                controller: this,
                functionManager: functionManager,
                audioEventReceiver: audioEventReceiver,
                realtimeEventReceiver: realtimeEventReceiver,
                inputTranscriptionEventReceiver: inputTranscriptionEventReceiver,
                textEventReceiver: textEventReceiver,
                transcriptEventReceiver: transcriptEventReceiver,
                toolCallReceiver: toolCallReceiver
            );

            _client = new WebSocketClient<RealtimeEvent>(_eventDispatcher.OnRealtimeEventReceived, OpenAI.DefaultInstance.JsonSettings, autoManageWebSocketState);
            _eventProcessor = new RealtimeEventProcessor(_client);
            if (webSocketEventReceiver != null) WebSocket.OnStateChanged = webSocketEventReceiver.OnWebSocketStateChanged;
        }

        public async UniTask ConnectAsync()
        {
            if (_client == null)
            {
                Logger.Error(WebSocketStrings.ConnectionFailed_NullClient);
                return;
            }
            if (RTMModel == null) RTMModel = OpenAISettings.DefaultRTM;
            RESTHeader tempAuthHeader = new("Authorization", "Bearer " + OpenAISettings.Instance.GetApiKey());
            await _client.CreateWebSocketConnectionAsync(RealtimeUtil.ResolveRealtimeUrl(RTMModel), tempAuthHeader, OpenAI.RealtimeApiHeader);
            await _client.StartWebSocketListening();
        }

        public async UniTask ReconnectAsync(Model model = null)
        {
            if (_session == null)
            {
                Logger.Error(WebSocketStrings.ConnectionFailed_NullSession);
                return;
            }

            if (model != null) _session.Model = model;

            await CloseConnection(WebSocketStrings.Reconnecting);
            await ConnectAsync();
        }

        public async UniTask ConfigureSession(
            string instructions,
            Model model = null,
            Modality[] modalities = null,
            Voice voice = null,
            RealtimeAudioFormat inputAudioFormat = RealtimeAudioFormat.PCM16,
            RealtimeAudioFormat outputAudioFormat = RealtimeAudioFormat.PCM16,
            SpeechToTextOptions inputAudioTranscription = null,
            TurnDetection turnDetection = null,
            FunctionDeclaration[] tools = null,
            ToolChoice toolChoice = null,
            float temperature = 0.8f,
            int? maxOutputTokens = null)
        {
            if (model == null) model = OpenAISettings.DefaultRTM;
            if (voice == null) voice = OpenAIVoice.Alloy;

            modalities ??= new Modality[] { Modality.Text, Modality.Audio };
            if (_inputTranscriptionEnabled) inputAudioTranscription ??= new SpeechToTextOptions();

            turnDetection ??= new TurnDetection();

            _session = new RealtimeSession
            {
                Instructions = instructions,
                Model = model,
                Modalities = modalities,
                Voice = voice,
                InputAudioFormat = inputAudioFormat,
                OutputAudioFormat = outputAudioFormat,
                InputAudioTranscription = inputAudioTranscription,
                TurnDetection = turnDetection,
                Tools = tools,
                ToolChoice = toolChoice ?? new ToolChoice(ToolType.Auto),
                Temperature = temperature,
                MaxOutputTokens = maxOutputTokens
            };

            if (_inputTranscriptionEnabled)
            {
                _session.InputAudioTranscription = new() { Model = STTModel };
                if (SpokenLanguage != SystemLanguage.English) _session.InputAudioTranscription.Language = SpokenLanguage;
            }

            SetAudioFormats(_session);
            await _eventProcessor.UpdateSession(_session);
        }

        public async UniTask CloseConnection(string closeReason = null)
        {
            await _client.CloseWebSocketConnectionAsync(closeReason);
        }

        public async UniTask SendUserText(string text)
        {

            await _eventProcessor.CreateConversationItem(RealtimeItem.UserMessage(text));
            await _eventProcessor.CreateResponse();
        }

        public async UniTask SendUserAudioFile(string audioFilePath)
        {
            string Base64EncodedAudio = await RealtimeUtil.InputAudioToBase64EncodedAudio(InputFormat, audioFilePath);
            await SendUserBase64EncodedAudio(Base64EncodedAudio);
        }

        public async UniTask SendUserAudioClip(AudioClip audioClip)
        {
            string Base64EncodedAudio = RealtimeUtil.InputAudioToBase64EncodedAudio(InputFormat, audioClip);
            await SendUserBase64EncodedAudio(Base64EncodedAudio);
        }

        public async UniTask StreamUserAudioFiles(List<string> audioFilePaths)
        {
            foreach (var filePath in audioFilePaths)
            {
                string base64AudioData = await RealtimeUtil.InputAudioToBase64EncodedAudio(InputFormat, filePath);
                await _eventProcessor.AppendInputAudioBuffer(base64AudioData);
            }
        }

        public async UniTask StreamUserAudioClips(List<AudioClip> audioClips)
        {
            foreach (var audioClip in audioClips)
            {
                string base64AudioData = RealtimeUtil.InputAudioToBase64EncodedAudio(InputFormat, audioClip);
                await _eventProcessor.AppendInputAudioBuffer(base64AudioData);
            }
        }

        public async UniTask StreamUserAudioData(float[] audioData)
        {
            if (audioData.IsNullOrEmpty()) return;
            string base64AudioData = RealtimeUtil.InputAudioToBase64EncodedAudio(InputFormat, audioData);

            if (string.IsNullOrEmpty(base64AudioData)) return;

            try
            {
                // Send the base64 audio data in chunks via WebSocket
                await _eventProcessor.AppendInputAudioBuffer(base64AudioData);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return;
            }
        }

        public async UniTask CommitUserAudioStreamInput()
        {
            Logger.Info(WebSocketStrings.SpeakingEnded_CommittingAudioBuffer);

            try
            {
                // Commit the audio buffer after all files have been streamed
                await _eventProcessor.CommitInputAudioBuffer();
            }
            catch (Exception e)
            {
                Logger.Warning(e.Message);
                return;
            }

            // Trigger response creation
            await _eventProcessor.CreateResponse();
        }

        private async UniTask SendUserBase64EncodedAudio(string base64EncodedAudio)
        {
            if (base64EncodedAudio == null)
            {
                Logger.Error(WebSocketStrings.ReceivedAudio_NullOrEmpty);
                return;
            }
            await _eventProcessor.CreateConversationItem(RealtimeItem.UserAudio(base64EncodedAudio));
            await _eventProcessor.CreateResponse();
        }

        private void SetAudioFormats(RealtimeSession session)
        {
            if (session == null) return;
            if (session.InputAudioFormat != null) InputFormat = session.InputAudioFormat.Value;
            if (session.OutputAudioFormat != null) OutputFormat = session.OutputAudioFormat.Value;
        }

        public async UniTask SetModelAsync(Model model)
        {
            if (IsSessionNull()) return;
            _session.Model = model;
            await ReconnectAsync(model);
        }

        public async UniTask SetVoiceAsync(Voice voice)
        {
            if (IsSessionNull()) return;
            _session.Voice = voice;
            await _eventProcessor.UpdateSession(_session);
        }

        public async UniTask SetInstructionsAsync(string instructions)
        {
            if (IsSessionNull()) return;
            _session.Instructions = instructions;
            await _eventProcessor.UpdateSession(_session);
        }

        public async UniTask SetInputAudioTranscriptionEnabledAsync(bool enabled)
        {
            if (IsSessionNull()) return;

            if (enabled)
            {
                _session.InputAudioTranscription = new();
            }
            else
            {
                _session.InputAudioTranscription = null;
            }

            await _eventProcessor.UpdateSession(_session);
        }

        public async UniTask SetMaxOutputTokensAsync(int maxOutputTokens)
        {
            if (IsSessionNull()) return;
            _session.MaxOutputTokens = maxOutputTokens;
            await _eventProcessor.UpdateSession(_session);
        }

        public async UniTask SetTemperatureAsync(float temperature)
        {
            if (IsSessionNull()) return;
            _session.Temperature = temperature;
            await _eventProcessor.UpdateSession(_session);
        }

        private bool IsSessionNull()
        {
            bool isNull = _session == null;

            if (isNull)
            {
                Logger.Error(WebSocketStrings.UpdateFailed_NullSession);
                return true;
            }

            return false;
        }
    }
}