using System.Linq;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.Editor;
using Glitch9.Editor.Collections;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public class AudioInputComponentEditorBase : UnityEditor.Editor
    {
        private const string kMicrophonePrefsKey = "AIDevKit.Microphone";
        private const string kFetchingDevicesKey = "FetchingMicrophoneList";
        private static EPrefsList<string> microphoneDevices;

        [InitializeOnLoadMethod]
        private static void OnEditorLoad()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            bool isFetchingDevices = SessionState.GetBool(kFetchingDevicesKey, false);

            if (isFetchingDevices && state == PlayModeStateChange.EnteredPlayMode)
            {
                microphoneDevices.Clear();
                microphoneDevices.AddRange(Microphone.devices);
                SessionState.EraseBool(kFetchingDevicesKey);
                EditorApplication.ExitPlaymode();
            }
        }

        protected SerializedProperty model;
        protected SerializedProperty recordingPath;
        protected SerializedProperty microphone;
        protected SerializedProperty removeBackgroundNoise;
        protected SerializedProperty errorReceiver;
        protected SerializedProperty recordingDuration;
        protected SerializedProperty recordingTrigger;
        protected SerializedProperty recordingBlockers;
        protected SerializedProperty saveRecordedAudio;

        // protected SerializedProperty voiceDetectionSettings;
        // protected SerializedProperty keyCodeTrigger;

        // inside voice detection settings
        protected SerializedProperty detectionIntervalMs;
        protected SerializedProperty silenceDurationMs;
        protected SerializedProperty silenceThreshold;

        // inside key code trigger 
        protected SerializedProperty key;
        protected SerializedProperty useShift;
        protected SerializedProperty useCtrl;
        protected SerializedProperty useAlt;
        protected SerializedProperty holdThresholdMs;

        protected virtual void OnEnable()
        {
            model = serializedObject.FindProperty(nameof(model));
            recordingPath = serializedObject.FindProperty(nameof(recordingPath));
            microphone = serializedObject.FindProperty(nameof(microphone));
            removeBackgroundNoise = serializedObject.FindProperty(nameof(removeBackgroundNoise));
            errorReceiver = serializedObject.FindProperty(nameof(errorReceiver));
            recordingDuration = serializedObject.FindProperty(nameof(recordingDuration));
            // detectionIntervalMs = serializedObject.FindProperty(nameof(detectionIntervalMs));
            // silenceDurationMs = serializedObject.FindProperty(nameof(silenceDurationMs));
            // silenceThreshold = serializedObject.FindProperty(nameof(silenceThreshold));
            // silenceDetectionEnabled = serializedObject.FindProperty(nameof(silenceDetectionEnabled)); 

            recordingTrigger = serializedObject.FindProperty(nameof(recordingTrigger));

            recordingBlockers = serializedObject.FindProperty(nameof(recordingBlockers));
            saveRecordedAudio = serializedObject.FindProperty(nameof(saveRecordedAudio));

            microphoneDevices = new EPrefsList<string>(kMicrophonePrefsKey);

            var voiceDetectionSettings = serializedObject.FindProperty("voiceDetectionSettings");
            if (voiceDetectionSettings != null)
            {
                detectionIntervalMs = voiceDetectionSettings.FindPropertyRelative("detectionIntervalMs");
                silenceDurationMs = voiceDetectionSettings.FindPropertyRelative("silenceDurationMs");
                silenceThreshold = voiceDetectionSettings.FindPropertyRelative("silenceThreshold");
            }

            var keyCodeTrigger = serializedObject.FindProperty("keyCodeTrigger");
            if (keyCodeTrigger != null)
            {
                key = keyCodeTrigger.FindPropertyRelative("key");
                useShift = keyCodeTrigger.FindPropertyRelative("useShift");
                useCtrl = keyCodeTrigger.FindPropertyRelative("useCtrl");
                useAlt = keyCodeTrigger.FindPropertyRelative("useAlt");
                holdThresholdMs = keyCodeTrigger.FindPropertyRelative("holdThresholdMs");
            }
        }

        private void DrawMicrophoneDevices()
        {
            if (microphoneDevices.Count == 0)
            {
                ExGUILayout.ErrorLabel("Microphone", "No microphones found.");
            }
            else
            {
                microphone.stringValue = ExGUILayout.TextPopup(
                    "Microphone",
                    microphone.stringValue,
                    microphoneDevices.ToArray()
                );
            }

            if (ExGUILayout.ButtonField("Fetch Microphone Devices"))
            {
                if (!EditorApplication.isPlaying)
                {
                    SessionState.SetBool(kFetchingDevicesKey, true);
                    EditorApplication.EnterPlaymode();
                }
            }
        }

        protected void DrawRecordingSettings()
        {
            ExGUILayout.BeginSection("Recording Settings");
            {
                DrawMicrophoneDevices();

                EditorGUILayout.PropertyField(recordingDuration, new GUIContent("Recording Duration (sec)",
                    "The duration of the recording in seconds."));

                EditorGUILayout.PropertyField(removeBackgroundNoise, new GUIContent("Remove Background Noise",
                    "If enabled, the recorder will attempt to remove background noise from the recording."));

                //save
                EditorGUILayout.PropertyField(saveRecordedAudio, new GUIContent("Save Recorded Audio",
                    "If enabled, the recorded audio will be saved to the specified path. " +
                    "If disabled, the recorded audio will not be saved."));

                if (saveRecordedAudio.boolValue)
                {
                    EditorGUI.indentLevel++;
                    AIDevKitGUI.OutputPathField(new GUIContent("Recordings Folder", "If not set, recordings won't be saved. " +
                        "This is useful for debugging or saving audio files for later use."), recordingPath);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(recordingTrigger, new GUIContent("Trigger", "The trigger to start recording. " +
                  "If set to KeyCode, the recording will start when the specified key is pressed. " +
                  "If set to Voice Detection, the recording will start when voice is detected."));

                if (recordingTrigger.enumValueIndex == (int)RecordingTrigger.VoiceDetection)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(detectionIntervalMs, new GUIContent("Detection Interval (ms)",
                        "The interval in milliseconds at which the recorder checks for silence."));
                    EditorGUILayout.PropertyField(silenceDurationMs, new GUIContent("Silence Duration (ms)",
                        "The duration in milliseconds of silence to detect before stopping the recording."));
                    EditorGUILayout.PropertyField(silenceThreshold, new GUIContent("Silence Threshold",
                        "The threshold for detecting silence. A lower value means more sensitive detection."));
                    EditorGUI.indentLevel--;
                }
                else if (recordingTrigger.enumValueIndex == (int)RecordingTrigger.ToggleKey)
                {
                    EditorGUI.indentLevel++;
                    ExEditorGUI.RecordingTriggerKeyPopup(new GUIContent("Key", "The key to toggle recording on and off."), key);
                    ExEditorGUI.ToggleSelector(new GUIContent("Modifiers",
                        "Select the modifier keys that must be held down to toggle recording."),
                        useShift, useCtrl, useAlt);
                    // EditorGUILayout.PropertyField(useShift, new GUIContent("Use Shift", "If true, the Shift key must be held down."));
                    // EditorGUILayout.PropertyField(useCtrl, new GUIContent("Use Ctrl", "If true, the Ctrl key must be held down."));
                    // EditorGUILayout.PropertyField(useAlt, new GUIContent("Use Alt", "If true, the Alt key must be held down."));
                    EditorGUI.indentLevel--;
                }
                else if (recordingTrigger.enumValueIndex == (int)RecordingTrigger.HoldKey)
                {
                    EditorGUI.indentLevel++;
                    ExEditorGUI.RecordingTriggerKeyPopup(new GUIContent("Key", "The key to hold down to start recording."), key);
                    ExEditorGUI.ToggleSelector(new GUIContent("Modifiers",
                        "Select the modifier keys that must be held down to toggle recording."),
                        useShift, useCtrl, useAlt);
                    EditorGUILayout.PropertyField(holdThresholdMs, new GUIContent("Hold Threshold (ms)",
                        "The duration in milliseconds the key must be held down to start recording."));
                    EditorGUI.indentLevel--;
                }
            }
            ExGUILayout.EndSection();

            EditorGUILayout.PropertyField(recordingBlockers, new GUIContent("Recording Blockers",
                "If any of these components are busy, recording will be blocked. " +
                "This is useful to prevent recording when other components are processing data."));
        }
    }
}