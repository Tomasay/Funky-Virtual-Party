using UnityEngine;

namespace Glitch9.CoreLib.IO.Audio
{
    public class AudioRecorderBase
    {
        internal static class Messages
        {
            internal const string kNoMicrophone = "No microphone devices found. Please connect a microphone to your device.";
            internal const string kRecordingStarted = "Recording session started using {0} at {1} Hz for a duration of {2} seconds.";
            internal const string kRecordingStopped = "Recording session has been successfully terminated.";
            internal const string kRecordingFailed = "Unable to initiate recording session.";
            internal const string kRecordingAlreadyStarted = "Recording is already in progress.";
            internal const string kRecordingNotStarted = "No active recording session detected.";
            internal const string kNoRecordingFound = "No recording data available.";
            internal const string kPlayingRecording = "Playback of the recorded session has started.";
            internal const string kCannotPlayWhileRecording = "Playback is unavailable during an active recording session.";
            internal const string kCannotGetRecordingWhileRecording = "Recording data cannot be accessed while recording is in progress.";
            internal const string kCannotPlayNoRecording = "Playback failed: no available recording data found.";
        }

        public string MicrophoneDeviceName { get; set; }
        public SampleRate SampleRate { get; set; } = SampleRate.Hz16000;
        public int RecordingLength { get; set; } = 30; // seconds
        public bool IsRecording => Microphone.IsRecording(null);
        public AudioClip RecordingClip { get; private set; }
        protected readonly ILogger _logger;
        protected readonly bool _debugMode;

        public AudioRecorderBase(SampleRate sampleRate = SampleRate.Hz16000, int recordingLength = 30, string microphoneDeviceName = null, ILogger logger = null, bool debugMode = false)
        {
            SampleRate = sampleRate;
            RecordingLength = recordingLength;
            MicrophoneDeviceName = microphoneDeviceName;
            RecordingClip = null;
            _logger = logger ?? new DefaultLogger(nameof(AudioRecorder));
            _debugMode = debugMode;
        }

        public void StartRecording(string deviceName = null)
        {
            if (Microphone.devices.Length == 0)
            {
                _logger.Error(Messages.kNoMicrophone);
                return;
            }

            if (deviceName != null) MicrophoneDeviceName = deviceName;
            if (string.IsNullOrWhiteSpace(MicrophoneDeviceName)) MicrophoneDeviceName = Microphone.devices[0];

            if (IsRecording)
            {
                _logger.Warning(Messages.kRecordingAlreadyStarted);
                return;
            }

            int sampleRateAsInt = (int)SampleRate;
            if (sampleRateAsInt <= 1)
            {
                _logger.Warning("Invalid sample rate: " + sampleRateAsInt + ". Resetting to 16kHz.");
                SampleRate = SampleRate.Hz16000;
                sampleRateAsInt = (int)SampleRate;
            }

            // Start recording with the default microphone, with a length of 10 seconds and at a frequency of 44100 Hz
#if UNITY_WEBGL && !UNITY_EDITOR
            Microphone.onAudioClipReceived = clip => OnAudioClipReceived(clip);
            Microphone.Start(null, false, RecordingLength, sampleRateAsInt);
#else
            AudioClip clip = Microphone.Start(null, false, RecordingLength, sampleRateAsInt);
            OnAudioClipReceived(clip);
#endif

            if (_debugMode) _logger.Info(string.Format(Messages.kRecordingStarted, MicrophoneDeviceName, SampleRate, RecordingLength));
        }

        protected virtual void OnAudioClipReceived(AudioClip clip)
        {
            if (clip == null)
            {
                _logger.Error(Messages.kRecordingFailed);
                return;
            }
            RecordingClip = clip;
        }

        public AudioClip GetRecording()
        {
            if (IsRecording)
            {
                _logger.Warning(Messages.kCannotGetRecordingWhileRecording);
                return null;
            }

            if (RecordingClip == null)
            {
                _logger.Warning(Messages.kNoRecordingFound);
                return null;
            }

            return RecordingClip;
        }

        public void PlayRecording()
        {
            if (IsRecording)
            {
                _logger.Warning(Messages.kCannotPlayWhileRecording);
                return;
            }

            if (RecordingClip == null)
            {
                _logger.Warning(Messages.kCannotPlayNoRecording);
                return;
            }

            AudioSource.PlayClipAtPoint(RecordingClip, Vector3.zero);
            _logger.Info(Messages.kPlayingRecording);
        }
    }
}