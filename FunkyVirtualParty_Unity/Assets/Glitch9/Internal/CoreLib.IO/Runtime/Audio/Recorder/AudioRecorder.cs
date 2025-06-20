using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.CoreLib.IO.Audio
{
    public class AudioRecorder : AudioRecorderBase
    {
        public AudioRecorder(
            SampleRate sampleRate = SampleRate.Hz16000,
            int recordingLength = 30,
            string microphoneDeviceName = null,
            ILogger logger = null,
            bool debugMode = false) : base(sampleRate, recordingLength, microphoneDeviceName, logger, debugMode)
        { }

        public AudioClip StopRecording(bool playRecording = false)
        {
            if (!IsRecording)
            {
                _logger.Warning(Messages.kRecordingNotStarted);
                return null;
            }

            // Stop the recording
            Microphone.End(null);
            if (_debugMode) _logger.Info(Messages.kRecordingStopped);

            if (RecordingClip != null)
            {
                RecordingClip.TrimSilence();
                if (playRecording) PlayRecording();
                return RecordingClip;
            }

            return null;
        }
    }
}