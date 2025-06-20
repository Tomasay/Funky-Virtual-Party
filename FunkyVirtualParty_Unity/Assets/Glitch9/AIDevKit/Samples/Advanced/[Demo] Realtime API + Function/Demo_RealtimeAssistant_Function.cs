using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Components;
using Glitch9.AIDevKit.OpenAI.Realtime;
using Glitch9.CoreLib.IO.Audio;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class Demo_RealtimeAssistant_Function : MonoBehaviour
    {
        [SerializeField] private RealtimeAssistant realtimeAPI;
        [SerializeField] private Dropdown micDropdown;
        [SerializeField] private DemoStatusWidget webSocketStatusWidget;
        [SerializeField] private DemoStatusWidget recordingStatusWidget;
        [SerializeField] private DemoStatusWidget sessionStatusWidget;
        [SerializeField] private Slider inputAudioLevelSlider;

        private void Start()
        {
            realtimeAPI.AudioRecorder.onStateChanged += OnAudioStreamRecorderStateChanged;
            realtimeAPI.AudioRecorder.onAudioLevelChanged += (level) => inputAudioLevelSlider.value = level;

            micDropdown.AddOptions(StringArrayToOptionData(Microphone.devices));
            micDropdown.onValueChanged.AddListener(OnMicDropdownValueChanged);

            // start the session right away
            InitializeRealtimeAsync();
        }

        private List<Dropdown.OptionData> StringArrayToOptionData(string[] values)
        {
            List<Dropdown.OptionData> optionData = new();
            foreach (string value in values)
            {
                optionData.Add(new Dropdown.OptionData(value));
            }
            return optionData;
        }

        private void OnMicDropdownValueChanged(int value)
        {
            realtimeAPI.SetMicrophone(Microphone.devices[value]);
        }

        private async void InitializeRealtimeAsync()
        {
            webSocketStatusWidget.SetStatus(DemoStatusType.Processing, "Connecting...");

            await realtimeAPI.CreateConnection();

            webSocketStatusWidget.SetStatus(DemoStatusType.Positive, "Connected");
        }


        public void OnWebSocketStateChanged(WebSocketState state)
        {
            switch (state)
            {
                case WebSocketState.Connecting:
                    webSocketStatusWidget.SetStatus(DemoStatusType.Processing, "Connecting...");
                    break;
                case WebSocketState.Open:
                    webSocketStatusWidget.SetStatus(DemoStatusType.Positive, "Connected");
                    break;
                case WebSocketState.CloseSent:
                    webSocketStatusWidget.SetStatus(DemoStatusType.Processing, "Closing Sent...");
                    break;
                case WebSocketState.CloseReceived:
                    webSocketStatusWidget.SetStatus(DemoStatusType.Processing, "Closing Received...");
                    break;
                case WebSocketState.Closed:
                    webSocketStatusWidget.SetStatus(DemoStatusType.Negative, "Closed");
                    break;
                case WebSocketState.Aborted:
                    webSocketStatusWidget.SetStatus(DemoStatusType.Negative, "Disconnected");
                    break;
            }
        }

        private void OnAudioStreamRecorderStateChanged(RealtimeAudioRecorder.State state)
        {
            switch (state)
            {
                case RealtimeAudioRecorder.State.NotInitialized:
                    recordingStatusWidget.SetStatus(DemoStatusType.Unavailable, "Not initialized");
                    break;
                case RealtimeAudioRecorder.State.Speaking:
                    recordingStatusWidget.SetStatus(DemoStatusType.Processing, "Recording...");
                    break;
                case RealtimeAudioRecorder.State.Stopped:
                    recordingStatusWidget.SetStatus(DemoStatusType.Negative, "Stopped");
                    break;
                case RealtimeAudioRecorder.State.Idle:
                    recordingStatusWidget.SetStatus(DemoStatusType.Positive, "Ready");
                    break;
            }
        }

        public void OnEventTypeChanged(string eventType)
        {
            switch (eventType)
            {
                case RealtimeEvent.Response.SessionCreated:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Session Created");
                    realtimeAPI.ConfigureSession().Forget();
                    realtimeAPI.StartRecording();
                    break;
                case RealtimeEvent.Response.SessionUpdated:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Session Updated");
                    break;
                case RealtimeEvent.Request.SessionUpdate:
                    sessionStatusWidget.SetStatus(DemoStatusType.Processing, "Updating Session...");
                    break;
                case RealtimeEvent.Response.InputAudioBufferCommitted:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Audio Buffer Committed");
                    break;
                case RealtimeEvent.Response.InputAudioBufferCleared:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Audio Buffer Cleared");
                    break;
                case RealtimeEvent.Response.InputAudioBufferSpeechStarted:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Speech Detected");
                    break;
                case RealtimeEvent.Response.InputAudioBufferSpeechStopped:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Speech Stopped");
                    break;
                case RealtimeEvent.Response.ConversationCreated:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Conversation Created");
                    break;
                case RealtimeEvent.Response.ConversationItemInputAudioTranscriptionCompleted:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Transcription Completed");
                    break;
                case RealtimeEvent.Response.ConversationItemInputAudioTranscriptionFailed:
                    sessionStatusWidget.SetStatus(DemoStatusType.Negative, "Transcription Failed");
                    break;
                case RealtimeEvent.Response.ConversationItemCreated:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Conversation Item Created");
                    break;
                case RealtimeEvent.Response.ConversationItemTruncated:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Assistant Audio Truncated");
                    break;
                case RealtimeEvent.Response.ConversationItemDeleted:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Conversation Item Deleted");
                    break;
                case RealtimeEvent.Response.ResponseCreated:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Response Created");
                    break;
                case RealtimeEvent.Response.ResponseDone:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Response Done");
                    break;
                case RealtimeEvent.Response.ResponseOutputItemAdded:
                    sessionStatusWidget.SetStatus(DemoStatusType.Processing, "Output Item Added...");
                    break;
                case RealtimeEvent.Response.ResponseOutputItemDone:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Output Item Done");
                    break;
                case RealtimeEvent.Response.ResponseContentPartAdded:
                    sessionStatusWidget.SetStatus(DemoStatusType.Processing, "Content Part Added...");
                    break;
                case RealtimeEvent.Response.ResponseContentPartDone:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Content Part Done");
                    break;
                case RealtimeEvent.Response.ResponseTextDelta:
                    sessionStatusWidget.SetStatus(DemoStatusType.Processing, "Text Updated...");
                    break;
                case RealtimeEvent.Response.ResponseTextDone:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Text Done");
                    break;
                case RealtimeEvent.Response.ResponseAudioTranscriptDelta:
                    sessionStatusWidget.SetStatus(DemoStatusType.Processing, "Audio Transcript Updated...");
                    break;
                case RealtimeEvent.Response.ResponseAudioTranscriptDone:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Audio Transcript Done");
                    break;
                case RealtimeEvent.Response.ResponseAudioDelta:
                    sessionStatusWidget.SetStatus(DemoStatusType.Processing, "Audio Updated...");
                    break;
                case RealtimeEvent.Response.ResponseAudioDone:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Audio Done");
                    break;
                case RealtimeEvent.Response.ResponseFunctionCallArgumentsDelta:
                    sessionStatusWidget.SetStatus(DemoStatusType.Processing, "Function Call Arguments Updated...");
                    break;
                case RealtimeEvent.Response.ResponseFunctionCallArgumentsDone:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Function Call Arguments Done");
                    break;
                case RealtimeEvent.Response.RateLimitsUpdated:
                    sessionStatusWidget.SetStatus(DemoStatusType.Positive, "Rate Limits Updated");
                    break;
                case RealtimeEvent.Response.Error:
                    sessionStatusWidget.SetStatus(DemoStatusType.Negative, "Error Occurred");
                    break;
                default:
                    sessionStatusWidget.SetStatus(DemoStatusType.Unavailable, "Unknown Event");
                    break;
            }
        }
    }
}