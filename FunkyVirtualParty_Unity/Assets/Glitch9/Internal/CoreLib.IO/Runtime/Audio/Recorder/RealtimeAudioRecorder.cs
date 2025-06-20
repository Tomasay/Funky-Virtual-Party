using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.CoreLib.IO.Audio
{
    /// <summary>
    /// Used for both 'Recording Start Trigger' and 'Recording Stop Trigger'.
    /// This enum is used to define the conditions under which recording starts or stops.
    /// </summary>
    public enum RecordingTrigger
    {
        /// <summary>
        /// Starts when 'StartRecording()' is called via code.
        /// Stops when 'StopRecording()' is called via code.
        /// </summary>
        [InspectorName("Manual (Via Code)")]
        Manual,

        /// <summary>
        /// Starts when a specific key is pressed.
        /// Pressing the same key again stops the recording.
        /// </summary>
        [InspectorName("Key (Toggle)")]
        ToggleKey,

        /// <summary>
        /// Starts when a specific key is held down.
        /// Stops when the key is released.
        /// </summary>
        [InspectorName("Key (Hold)")]
        HoldKey,

        /// <summary>
        /// Automatically starts when sound is detected,
        /// and stops after a silence duration.
        /// </summary>
        [InspectorName("Voice Detection")]
        VoiceDetection
    }

    [Serializable]
    public class KeyCodeTrigger
    {
        // 키 조합
        public KeyCode key = KeyCode.Tilde; // ~, discord style 
        public bool useShift = false; // Shift 키 사용 여부
        public bool useCtrl = false; // Ctrl 키 사용 여부
        public bool useAlt = false; // Alt 키 사용 여부 

        private const int kDefaultHoldDurationMs = 300;
        [Range(100, 1000)] public int holdThresholdMs = kDefaultHoldDurationMs;

        private float _keyDownTime = -1f;
        private bool _isTriggered = false;

        public void OnHoldKey(Action onPressed, Action onReleased)
        {
            if (onPressed == null || onReleased == null)
            {
                Debug.LogError("onPressed and onReleased actions must not be null.");
                return;
            }

            bool modifierMatch =
                (!useShift || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
                (!useCtrl || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
                (!useAlt || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));

            if (Input.GetKeyDown(key) && modifierMatch)
            {
                _keyDownTime = Time.time;
                _isTriggered = false;
            }

            if (Input.GetKey(key) && modifierMatch && _keyDownTime >= 0f && !_isTriggered)
            {
                float heldDurationMs = (Time.time - _keyDownTime) * 1000f;
                if (heldDurationMs >= holdThresholdMs)
                {
                    _isTriggered = true;
                    Debug.Log($"Triggered after holding {heldDurationMs}ms");

                    onPressed?.Invoke();
                }
            }

            if (Input.GetKeyUp(key))
            {
                _keyDownTime = -1f;
                _isTriggered = false;

                Debug.Log($"Released key: {key}");
                onReleased?.Invoke();
            }
        }

        public void OnToggleKey(Action onPressed, Action onReleased)
        {
            if (onPressed == null || onReleased == null)
            {
                Debug.LogError("onPressed and onReleased actions must not be null.");
                return;
            }

            bool modifierMatch =
                (!useShift || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
                (!useCtrl || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
                (!useAlt || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));

            if (Input.GetKeyDown(key) && modifierMatch)
            {
                if (_isTriggered)
                {
                    _isTriggered = false;
                    Debug.Log($"Released key: {key}");
                    onReleased?.Invoke();
                }
                else
                {
                    _isTriggered = true;
                    Debug.Log($"Pressed key: {key}");
                    onPressed?.Invoke();
                }
            }
        }

        public void Reset()
        {
            _keyDownTime = -1f;
            _isTriggered = false;
        }

        public override string ToString()
        {
            var modifiers = new List<string>();
            if (useCtrl) modifiers.Add("Ctrl");
            if (useShift) modifiers.Add("Shift");
            if (useAlt) modifiers.Add("Alt");
            modifiers.Add(key.ToString());

            return string.Join(" + ", modifiers);
        }
    }

    [Serializable]
    public class VoiceDetectionSettings
    {
        public static class MaxValues
        {
            public const int DetectionIntervalMs = 10000; // 10 seconds
            public const int SilenceDurationMs = 300000; // 5 minutes
            public const float SilenceThreshold = 0.1f;
        }

        public static class MinValues
        {
            public const int DetectionIntervalMs = 50; // 0.05 seconds
            public const int SilenceDurationMs = 1000; // 1 second
            public const float SilenceThreshold = 0.005f;
        }

        [SerializeField, Range(MinValues.SilenceThreshold, MaxValues.SilenceThreshold)] private float silenceThreshold = 0.01f;
        [SerializeField, Range(MinValues.DetectionIntervalMs, MaxValues.DetectionIntervalMs)] private int detectionIntervalMs = 100;
        [SerializeField, Range(MinValues.SilenceDurationMs, MaxValues.SilenceDurationMs)] private int silenceDurationMs = 5000;

        public float SilenceThreshold
        {
            get => silenceThreshold;
            set
            {
                if (value < MinValues.SilenceThreshold || value > MaxValues.SilenceThreshold)
                {
                    Debug.LogWarning("Silence threshold must be between " + MinValues.SilenceThreshold + " and " + MaxValues.SilenceThreshold);
                    return;
                }
                silenceThreshold = value;
            }
        }

        public int DetectionIntervalMs
        {
            get => detectionIntervalMs;
            set
            {
                if (value < MinValues.DetectionIntervalMs || value > MaxValues.DetectionIntervalMs)
                {
                    Debug.LogWarning("Sample duration must be between " + MinValues.DetectionIntervalMs + " and " + MaxValues.DetectionIntervalMs);
                    return;
                }
                detectionIntervalMs = value;
            }
        }

        public int SilenceDurationMs
        {
            get => silenceDurationMs;
            set
            {
                if (value < MinValues.SilenceDurationMs || value > MaxValues.SilenceDurationMs)
                {
                    Debug.LogWarning("Silence duration must be between " + MinValues.SilenceDurationMs + " and " + MaxValues.SilenceDurationMs);
                    return;
                }
                silenceDurationMs = value;
            }
        }

        public bool Validate()
        {
            if (silenceThreshold < MinValues.SilenceThreshold || silenceThreshold > MaxValues.SilenceThreshold)
            {
                Debug.LogWarning("Silence threshold must be between " + MinValues.SilenceThreshold + " and " + MaxValues.SilenceThreshold);
                return false;
            }
            if (detectionIntervalMs < MinValues.DetectionIntervalMs || detectionIntervalMs > MaxValues.DetectionIntervalMs)
            {
                Debug.LogWarning("Sample duration must be between " + MinValues.DetectionIntervalMs + " and " + MaxValues.DetectionIntervalMs);
                return false;
            }
            if (silenceDurationMs < MinValues.SilenceDurationMs || silenceDurationMs > MaxValues.SilenceDurationMs)
            {
                Debug.LogWarning("Silence duration must be between " + MinValues.SilenceDurationMs + " and " + MaxValues.SilenceDurationMs);
                return false;
            }
            return true;
        }
    }

    public class RealtimeAudioRecorder : AudioRecorderBase
    {
        public enum State
        {
            NotInitialized,
            Idle,
            Speaking,
            Stopped,
        }

        #region Private Fields for Lazy Loading

        private KeyCodeTrigger _keyCodeTrigger; // Used for key-based triggers
        private VoiceDetectionSettings _voiceDetectionSettings;
        private State? _state;

        #endregion Private Fields for Lazy Loading 
        public float SilenceThreshold { get => _voiceDetectionSettings.SilenceThreshold; set => _voiceDetectionSettings.SilenceThreshold = value; }
        public int DetectionIntervalMs { get => _voiceDetectionSettings.DetectionIntervalMs; set => _voiceDetectionSettings.DetectionIntervalMs = value; }
        public int SilenceDurationMs { get => _voiceDetectionSettings.SilenceDurationMs; set => _voiceDetectionSettings.SilenceDurationMs = value; }

        public RecordingTrigger Trigger { get; set; }


#pragma warning disable IDE1006

        public State state
        {
            get
            {
                _state ??= State.NotInitialized;
                return _state.Value;
            }
            private set
            {
                _state = value;
                onStateChanged?.Invoke(_state.Value);
            }
        }

        public event Action<State> onStateChanged;
        public event Action<float> onAudioLevelChanged;

#pragma warning restore IDE1006

        private readonly Action _onRecordingStarted; // Callback for when recording starts
        private readonly Action<float[]> _onSpeakingChunk; // Callback for handling streaming data
        private readonly Action _onSpeakingEnded; // Callback for when speaking ends  
        private readonly Action<float[]> _onSpeakingBuffered; // Callback for handling buffered data
        private readonly Func<bool> _isRecordingAllowed; // Function to check if recording is allowed
        private int _lastSamplePosition = 0;
        private DateTime _lastInputTime;
        private readonly List<float> _buffer = new(); // for buffered mode
        private CancellationTokenSource _recordingCts;

        public RealtimeAudioRecorder(
            SampleRate sampleRate = SampleRate.Hz16000,
            int recordingLength = 30,
            string microphoneDeviceName = null,
            RecordingTrigger recordingTrigger = RecordingTrigger.Manual,
            KeyCodeTrigger keyCodeTrigger = null,
            VoiceDetectionSettings voiceDetectionSettings = null,
            Action onRecordingStarted = null,
            Action<float[]> onSpeakingChunk = null,
            Action onSpeakingEnded = null,
            Action<float[]> onSpeakingBuffered = null,
            Func<bool> isRecordingAllowed = null,
            ILogger logger = null,
            bool debugMode = false) : base(sampleRate, recordingLength, microphoneDeviceName, logger, debugMode)
        {
            // set properties
            Trigger = recordingTrigger;
            _keyCodeTrigger = keyCodeTrigger ?? new KeyCodeTrigger(); // 기본 키 설정

            if (voiceDetectionSettings != null)
            {
                if (!voiceDetectionSettings.Validate())
                {
                    throw new ArgumentException("Invalid silence detection settings provided.");
                }
                else
                {
                    _voiceDetectionSettings = voiceDetectionSettings;
                }
            }
            else
            {
                _voiceDetectionSettings = new();
            }

            // register callbacks
            _onRecordingStarted = onRecordingStarted;
            _onSpeakingChunk = onSpeakingChunk;
            _onSpeakingEnded = onSpeakingEnded;
            _onSpeakingBuffered = onSpeakingBuffered;

            // register predicates
            _isRecordingAllowed = isRecordingAllowed;

            // initialize states
            _lastInputTime = DateTime.Now;
        }

        public void OnUpdate()
        {
            if (state == State.NotInitialized) return;

            if (Trigger == RecordingTrigger.ToggleKey)
            {
                _keyCodeTrigger.OnHoldKey(
                    onPressed: () => StartRecording(),
                    onReleased: () => StopRecording());
            }
            else if (Trigger == RecordingTrigger.HoldKey)
            {
                _keyCodeTrigger.OnHoldKey(
                    onPressed: () => StartRecording(),
                    onReleased: () => StopRecording());
            }
        }

        protected override void OnAudioClipReceived(AudioClip clip)
        {
            base.OnAudioClipReceived(clip);
            if (clip == null) return;

            _lastInputTime = DateTime.Now;
            _recordingCts = new CancellationTokenSource();
            RunVoiceDetectionLoop().Forget();
        }

        public AudioClip StopRecording()
        {
            _recordingCts?.Cancel();
            Microphone.End(null);  // Stop the recording
            state = State.Stopped;
            if (_debugMode) _logger.Info(Messages.kRecordingStopped);

            if (RecordingClip != null)
            {
                RecordingClip.TrimSilence();
                return RecordingClip;
            }

            _logger.Warning("No audio data recorded. Please try again.");
            return null;
        }

        public void ResumeRecording()
        {
            if (CanRecord())
            {
                _logger.Warning(Messages.kRecordingAlreadyStarted);
                return;
            }

            if (_debugMode) _logger.Info("Resuming recording.");
            StartRecording();
        }

        private async UniTaskVoid RunVoiceDetectionLoop()
        {
            state = State.Idle;
            _lastSamplePosition = 0;

            while (CanRecord() && !_recordingCts.IsCancellationRequested)
            {
                await UniTask.Delay(DetectionIntervalMs);

                int currentSamplePosition = Microphone.GetPosition(MicrophoneDeviceName);
                int sampleLength = currentSamplePosition - _lastSamplePosition;
                if (sampleLength <= 0) continue; // 새로운 샘플이 없으면 건너뜀

                float[] audioData = new float[sampleLength];
                RecordingClip.GetData(audioData, _lastSamplePosition);
                _lastSamplePosition = currentSamplePosition;

                if (HasAudioInput(audioData))
                {
                    // 오디오 감지됨
                    if (state != State.Speaking)
                    {
                        if (_debugMode) _logger.Info("Speaking started.");
                        state = State.Speaking;
                        _onRecordingStarted?.Invoke();
                        _buffer.Clear(); // Speaking 시작 시 버퍼 초기화
                    }

                    _lastInputTime = DateTime.Now;

                    _onSpeakingChunk?.Invoke(audioData);

                    if (onAudioLevelChanged != null)
                    {
                        float audioLevel = CalcAudioLevel(audioData);
                        onAudioLevelChanged?.Invoke(audioLevel);
                    }

                    if (_onSpeakingBuffered != null)
                        _buffer.AddRange(audioData);
                }
                else
                {
                    onAudioLevelChanged?.Invoke(0);

                    // 무음 시간이 지나면 Speaking 종료 처리
                    if (Trigger == RecordingTrigger.VoiceDetection
                        && state == State.Speaking
                        && (DateTime.Now - _lastInputTime).TotalMilliseconds > SilenceDurationMs)
                    {
                        if (_debugMode) _logger.Info("Speaking ended. Switching to Idle.");
                        state = State.Idle;
                        _onSpeakingBuffered?.Invoke(_buffer.ToArray());
                        _onSpeakingEnded?.Invoke();
                        _buffer.Clear();
                    }
                }
            }
        }

        private bool CanRecord()
        {
            if (state != State.Speaking
                && state != State.Idle) return false;

            if (_isRecordingAllowed != null
                && !_isRecordingAllowed()) return false;

            return true; // Recording is allowed 
        }

        private bool HasAudioInput(float[] audioData)
        {
            foreach (float sample in audioData)
            {
                if (Math.Abs(sample) > SilenceThreshold)
                {
                    return true; // There is input
                }
            }
            return false; // No input detected
        }

        private float CalcAudioLevel(float[] audioData)
        {
            float sum = 0f;

            // 모든 오디오 샘플의 절대값을 합산
            foreach (float sample in audioData)
            {
                sum += Mathf.Abs(sample);
            }

            // 평균값을 구하고, 샘플 수로 나누어 정규화된 오디오 레벨 계산
            float average = sum / audioData.Length;

            // 오디오 레벨을 0에서 1 사이의 값으로 정규화
            return Mathf.Clamp01(average / SilenceThreshold);
        }
    }
}
