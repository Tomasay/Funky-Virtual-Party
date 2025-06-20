using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit.Components
{
    public abstract class AudioInputComponent<T> : AIModuleComponent
    {
        // Not allowing to change sample rate for now, any sample rate other than 16kHz oftens cause troubles and confuse beginners.
        // Had it here for the advanced users, but it seems there is more to lose than to gain by allowing this.
        // Many APIs don't allow sample rates other than 16kHz anyway, so it's better to keep it simple.
        // [SerializeField] private SampleRate inputSampleRate = SampleRate.Hz16000; 
        [SerializeField] protected string microphone;
        [SerializeField, Range(10, 300)] protected int recordingDuration = AIDevKitConfig.DefaultRecordingDururationInSec;
        // [SerializeField, Range(50, 5000)] private int detectionIntervalMs = 500;
        // [SerializeField, Range(500, 5000)] private int silenceDurationMs = 2500;
        // [SerializeField, Range(0.01f, 1.0f)] private float silenceThreshold = 0.01f;

        [SerializeField] private RecordingTrigger recordingTrigger = RecordingTrigger.Manual;
        [SerializeField] private KeyCodeTrigger keyCodeTrigger = new();
        [SerializeField] private VoiceDetectionSettings voiceDetectionSettings = new();
        [SerializeField] private bool saveRecordedAudio = false;
        [SerializeField] private string recordingPath;
        [SerializeField] private List<AIDevKitComponent> recordingBlockers;

        /// <summary>
        /// The maximum duration of the recording in seconds.
        /// Default is 30 seconds, if you need to speak longer, you can increase this value.
        /// </summary>
        public int RecordingDuration
        {
            get => recordingDuration;
            set
            {
                recordingDuration = value;
                if (Recorder != null) Recorder.RecordingLength = recordingDuration;
            }
        }

        /// <summary>
        /// The ID or name of the microphone device to use for recording.
        /// If not set, the default microphone will be used.
        /// </summary>
        public string Microphone
        {
            get => microphone;
            set
            {
                microphone = value;
                if (Recorder != null) Recorder.MicrophoneDeviceName = microphone;
            }
        }

        public RecordingTrigger RecordingTrigger
        {
            get => recordingTrigger;
            set
            {
                recordingTrigger = value;
                if (Recorder != null) Recorder.Trigger = recordingTrigger;
            }
        }

        public VoiceDetectionSettings VoiceDetectionSettings => voiceDetectionSettings;

        /// <summary>
        /// Disregard this property if SilenceDetectionEnabled is false.
        /// The interval in milliseconds at which the recorder checks for silence.
        /// Default is 500ms, you can adjust this value to control how often the recorder checks for silence.
        /// A lower value means more frequent checks, but may increase CPU usage.
        /// </summary>
        public KeyCodeTrigger KeyCodeTrigger => keyCodeTrigger;



        /// <summary>
        /// Whether the recorder is currently recording audio.
        /// This property will return true if the recorder is actively recording audio from the microphone.
        /// </summary>
        public bool IsRecording => Recorder?.IsRecording ?? false;

        public RealtimeAudioRecorder Recorder { get; private set; }

        private void Start()
        {
            Recorder = new RealtimeAudioRecorder(
                sampleRate: SampleRate.Hz16000,
                recordingLength: recordingDuration,
                microphoneDeviceName: microphone,
                recordingTrigger: recordingTrigger,
                keyCodeTrigger: keyCodeTrigger,
                voiceDetectionSettings: voiceDetectionSettings,
                onSpeakingBuffered: OnAudioRecorded,
                isRecordingAllowed: () => !IsRecordingBlocked()
            );
        }

        private void OnEnable()
        {
            if (recordingTrigger == RecordingTrigger.VoiceDetection)
            {
                StartRecording();
            }
        }

        private void Update() => Recorder?.OnUpdate();

        /// <summary>
        /// Start recording audio from the microphone
        /// </summary>
        public void StartRecording()
        {
            if (Recorder == null)
            {
                OnError("AudioRecorder is not initialized.");
                return;
            }

            if (string.IsNullOrWhiteSpace(microphone))
            {
                OnError("Microphone device is not set.");
                return;
            }

            Recorder.StartRecording(microphone);
        }

        /// <summary>
        /// Stop recording audio from the microphone and return the recorded audio clip, 
        /// then do the post-processing that derived classes need.
        /// If the recorder is not currently recording, it will return null.
        /// </summary> 
        public async UniTask<T> StopRecording()
        {
            if (IsRecording)
            {
                AudioClip recordedClip = await StopRecordingAsyncINTERNAL();

                if (recordedClip == null)
                {
                    OnError("Failed to stop recording. No audio data recorded.");
                    return default;
                }

                return await OnAudioRecorded(recordedClip);
            }
            else
            {
                OnError("Recorder is not currently recording.");
                return default;
            }
        }

        protected abstract UniTask<T> OnAudioRecorded(AudioClip recordedClip);
        protected async void OnAudioRecorded(float[] recordedSamples)
        {
            if (saveRecordedAudio)
            {
                byte[] audioBytes = AudioProcessor.FloatTo16BitPCM(recordedSamples);

                if (audioBytes == null || audioBytes.Length == 0)
                {
                    Debug.LogWarning("No audio data recorded. Please try again.");
                    return;
                }

                string filePath = FormatRecordingPath();
                if (AIDevKitDebug.kDebugMode.Value) Debug.Log($"Saving the recorded audio to {filePath}...");
                await AudioFileWriter.WriteFileAsync(audioBytes, filePath); // ✅ await UniTask version
            }

            OnAudioRecorded(recordedSamples.ToAudioClip()).Forget();
        }

        private bool IsRecordingBlocked()
        {
            if (recordingBlockers == null || recordingBlockers.Count == 0) return false;

            foreach (var blocker in recordingBlockers)
            {
                if (blocker != null && blocker.IsBusy)
                {
                    Debug.LogWarning($"Recording is blocked by {blocker.name} component.");
                    return true;
                }
            }

            return false;
        }

        private async UniTask<AudioClip> StopRecordingAsyncINTERNAL()
        {
            AudioClip recordedClip = Recorder?.StopRecording();

            if (recordedClip == null)
            {
                Debug.LogWarning("No audio data recorded. Please try again.");
                return null;
            }

            await UniTask.DelayFrame(2); // ✅ Allow time for data to flush

            float[] samples = new float[recordedClip.samples * recordedClip.channels];
            recordedClip.GetData(samples, 0);
            bool hasData = samples.Any(s => Mathf.Abs(s) > 0.001f);

            if (!hasData)
            {
                Debug.LogWarning("No audio data recorded. Please try again.");
                return null;
            }

            if (AIDevKitDebug.kDebugMode.Value)
            {
                Debug.Log($"Samples length: {samples.Length}, has data: {hasData}");
                Debug.Log("Trimming silence from the start and end of the audio clip...");
            }

            if (AIDevKitDebug.kDebugMode.Value)
            {
                Debug.Log($"Recording duration: {recordedClip.length} seconds");
                Debug.Log($"Recording frequency: {recordedClip.frequency} Hz");
                Debug.Log($"Recording channels: {recordedClip.channels}");
                Debug.Log($"Recording samples: {recordedClip.samples}");
            }

            // check if it's too short
            if (recordedClip.length < 0.5f)
            {
                Debug.LogWarning("Recording is too short. Please try again.");
                return null;
            }

            // play the recorded audio clip for testing purposes
            // Debug.Log($"Playing the recorded audio clip '{RecordedClip.name}'...");
            // AudioSource.PlayClipAtPoint(RecordedClip, Vector3.zero); 

            if (saveRecordedAudio)
            {
                string filePath = FormatRecordingPath();
                if (AIDevKitDebug.kDebugMode.Value) Debug.Log($"Saving the recorded audio to {filePath}...");
                await AudioFileWriter.WriteFileAsync(recordedClip, filePath); // ✅ await UniTask version
            }

            //File<AudioClip> recording = new(recordedClip, filePath.ToPersistentDataPath()); 
            return recordedClip;
        }

        private string FormatRecordingPath()
        {
            string fileName = AIDevKitConfig.RecordingFileNameFormat.Replace("{yyyyMMddHHmmss}", DateTime.Now.ToString("yyyyMMddHHmmss"));
            return $"{recordingPath}/{fileName}";
        }
    }
}