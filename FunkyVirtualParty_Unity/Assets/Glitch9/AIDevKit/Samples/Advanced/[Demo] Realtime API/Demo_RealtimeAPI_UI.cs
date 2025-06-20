using System.Net.WebSockets;
using Glitch9.AIDevKit.Components;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.AIDevKit.OpenAI.Realtime;
using Glitch9.CoreLib.IO.Audio;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.Demo
{
    public class OpenAI_RealtimeAPI_UI : MonoBehaviour
    {
        enum BoxType
        {
            Logs,
            Transcript,
        }

        [SerializeField] private RealtimeAssistant realtimeAPI;
        //[SerializeField] private Dropdown modelDropdown;
        [SerializeField] private Dropdown voiceDropdown;
        [SerializeField] private Dropdown micDropdown;
        [SerializeField] private DemoLogBox stateMonitorBox;
        [SerializeField] private DemoLogBox transcriptBox;
        [SerializeField] private DemoLogBox textOutputBox;
        [SerializeField] private Button createConnectionButton;
        [SerializeField] private Button startSessionButton;
        [SerializeField] private Button startRecordingButton;
        [SerializeField] private Button stopRecordingButton;
        [SerializeField] private DemoToggleButton stateMonitorToggle;
        [SerializeField] private DemoToggleButton transcriptToggle;
        //[SerializeField] private DemoToggleButton textOutputToggle;
        [SerializeField] private Transform stateMonitorBoxObj;
        [SerializeField] private Transform transcriptBoxObj;
        [SerializeField] private Transform textOutputBoxObj;
        [SerializeField] private DemoStatusWidget webSocketStatusWidget;
        [SerializeField] private DemoStatusWidget recordingStatusWidget;
        [SerializeField] private DemoStatusWidget sessionStatusWidget;
        [SerializeField] private Slider inputAudioLevelSlider;
        [SerializeField] private Text instructionsText;

        private BoxType _currentBoxType = BoxType.Logs;
        private string[] _allVoices;

        private void GetAllVoices()
        {
            List<Voice> voices = VoiceLibrary.GetVoicesByAPI(Api.OpenAI);
            List<string> allVoices = new();
            foreach (var voice in voices)
            {
                allVoices.Add(voice);
            }

            _allVoices = allVoices.ToArray();
        }


        private void Start()
        {
            realtimeAPI.AudioRecorder.onStateChanged += OnAudioStreamRecorderStateChanged;
            realtimeAPI.AudioRecorder.onAudioLevelChanged += (level) => inputAudioLevelSlider.value = level;
            instructionsText.text = realtimeAPI.Instructions;

            //modelDropdown.ClearOptions();
            voiceDropdown.ClearOptions();

            //modelDropdown.AddOptions(EnumToOptionData<RealtimeModel>());
            voiceDropdown.AddOptions(DemoUtil.StringArrayToOptionData(_allVoices));
            micDropdown.AddOptions(DemoUtil.StringArrayToOptionData(Microphone.devices));

            //modelDropdown.onValueChanged.AddListener(OnModelDropdownValueChanged);
            voiceDropdown.onValueChanged.AddListener(OnVoiceDropdownValueChanged);
            micDropdown.onValueChanged.AddListener(OnMicDropdownValueChanged);

            createConnectionButton.onClick.AddListener(OnCreateConnectionButtonClicked);
            startSessionButton.onClick.AddListener(OnStartSessionButtonClicked);
            startRecordingButton.onClick.AddListener(OnStartRecordingButtonClicked);
            stopRecordingButton.onClick.AddListener(OnStopRecordingButtonClicked);

            stateMonitorToggle.onValueChanged += (isOn) => SetBoxType(BoxType.Logs, isOn);
            transcriptToggle.onValueChanged += (isOn) => SetBoxType(BoxType.Transcript, isOn);
            //textOutputToggle.onValueChanged += (isOn) => SetBoxType(BoxType.TextOutput, isOn);
        }

        private void SetBoxType(BoxType boxType, bool isOn)
        {
            if (_currentBoxType == boxType && isOn) return;
            _currentBoxType = boxType;

            switch (boxType)
            {
                case BoxType.Logs:
                    stateMonitorBoxObj.SetAsLastSibling();
                    transcriptToggle.Deselect();
                    //textOutputToggle.Deselect();
                    break;
                case BoxType.Transcript:
                    transcriptBoxObj.SetAsLastSibling();
                    stateMonitorToggle.Deselect();
                    //textOutputToggle.Deselect();
                    break;
                    // case BoxType.TextOutput:
                    //     textOutputBoxObj.SetAsLastSibling();
                    //     stateMonitorToggle.Deselect();
                    //     transcriptToggle.Deselect();
                    //     break;
            }
        }

        private void OnVoiceDropdownValueChanged(int value)
        {
            realtimeAPI.Voice = _allVoices[value];
        }

        private void OnMicDropdownValueChanged(int value)
        {
            realtimeAPI.SetMicrophone(Microphone.devices[value]);
        }

        private async void OnCreateConnectionButtonClicked()
        {
            webSocketStatusWidget.SetStatus(DemoStatusType.Processing, "Connecting...");

            await realtimeAPI.CreateConnection();

            webSocketStatusWidget.SetStatus(DemoStatusType.Positive, "Connected");
        }

        public void OnReceiveText(string text) => textOutputBox.OnTextDelta(text);
        public void OnReceiveTextDone(string text) => textOutputBox.OnTextDone($"<color=yellow>Assistant: </color>{text}");
        public void OnReceiveInputTranscriptionDelta(string transcription) => transcriptBox.OnTextDelta($"<color=grey>User: </color>{transcription}");
        public void OnReceiveInputTranscriptionCompleted(string transcription) => transcriptBox.OnTextDone($"<color=grey>User: </color>{transcription}");
        public void OnReceiveInputTranscriptionFailed(ErrorResponse error) => transcriptBox.OnTextDone($"<color=grey>User: </color>{error?.Message}");
        public void OnStartSessionButtonClicked() => realtimeAPI.ConfigureSession().Forget();
        public void OnStartRecordingButtonClicked() => realtimeAPI.StartRecording();
        public void OnStopRecordingButtonClicked() => realtimeAPI.StopRecording();


        public void OnWebSocketStateChanged(WebSocketState state)
        {
            //AIDevKitDebug.Log($"WebSocket State: {state}");
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