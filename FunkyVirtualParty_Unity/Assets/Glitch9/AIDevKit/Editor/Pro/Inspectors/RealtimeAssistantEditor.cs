using Glitch9.AIDevKit.Components;
using Glitch9.AIDevKit.OpenAI.Realtime;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{

    [CustomEditor(typeof(RealtimeAssistant))]
    public class RealtimeAssistantEditor : UnityEditor.Editor
    {
        private SerializedProperty functionManager;

        // receivers  
        private SerializedProperty realtimeEventReceiver;
        private SerializedProperty webSocketEventReceiver;
        private SerializedProperty inputTranscriptionEventReceiver;
        private SerializedProperty textEventReceiver;
        private SerializedProperty transcriptEventReceiver;
        private SerializedProperty toolEventReceiver;

        private SerializedProperty rtmModel;
        private SerializedProperty sttModel;
        private SerializedProperty spokenLanguage;
        private SerializedProperty voice;
        private SerializedProperty instructions;

        private SerializedProperty inputAudioFormat;
        private SerializedProperty inputSampleRate;
        private SerializedProperty inputSampleDurationMs;
        private SerializedProperty silenceDurationMs;
        private SerializedProperty silenceThreshold;

        private SerializedProperty outputAudioFormat;
        private SerializedProperty outputAudioVolume;
        private SerializedProperty autoStart;

        private RealtimeAssistant _target;


        private void OnEnable()
        {
            rtmModel = serializedObject.FindProperty(nameof(rtmModel));
            sttModel = serializedObject.FindProperty(nameof(sttModel));
            voice = serializedObject.FindProperty(nameof(voice));

            spokenLanguage = serializedObject.FindProperty(nameof(spokenLanguage));

            instructions = serializedObject.FindProperty(nameof(instructions));

            inputAudioFormat = serializedObject.FindProperty(nameof(inputAudioFormat));
            inputSampleRate = serializedObject.FindProperty(nameof(inputSampleRate));
            inputSampleDurationMs = serializedObject.FindProperty(nameof(inputSampleDurationMs));
            silenceDurationMs = serializedObject.FindProperty(nameof(silenceDurationMs));
            silenceThreshold = serializedObject.FindProperty(nameof(silenceThreshold));

            outputAudioFormat = serializedObject.FindProperty(nameof(outputAudioFormat));
            outputAudioVolume = serializedObject.FindProperty(nameof(outputAudioVolume));

            functionManager = serializedObject.FindProperty(nameof(functionManager));
            autoStart = serializedObject.FindProperty(nameof(autoStart));

            realtimeEventReceiver = serializedObject.FindProperty(nameof(realtimeEventReceiver));
            webSocketEventReceiver = serializedObject.FindProperty(nameof(webSocketEventReceiver));
            inputTranscriptionEventReceiver = serializedObject.FindProperty(nameof(inputTranscriptionEventReceiver));
            textEventReceiver = serializedObject.FindProperty(nameof(textEventReceiver));
            transcriptEventReceiver = serializedObject.FindProperty(nameof(transcriptEventReceiver));
            toolEventReceiver = serializedObject.FindProperty(nameof(toolEventReceiver));

            _target = (RealtimeAssistant)serializedObject.targetObject;

            ResetInputSettingsToDefault();
        }

        private void SaveTarget()
        {
            EditorUtility.SetDirty(_target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void ResetInputSettingsToDefault()
        {
            _target.InputAudioFormat = RealtimeAudioFormat.PCM16;
            _target.InputSampleRate = SampleRate.Hz16000;
            _target.InputSampleDurationMs = 500;
            _target.SilenceDurationMs = 2500;
            _target.SilenceThreshold = 0.01f;
        }

        private void ResetOutputSettingsToDefault()
        {
            _target.OutputAudioFormat = RealtimeAudioFormat.PCM16;
            _target.OutputAudioVolume = 1.0f;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ExGUIPreset.UnityComponentLabel(AIDevKitIcons.Realtime, "Realtime Assistant", "Integrate OpenAI's realtime voice assistant into your project.");
            EditorGUILayout.Space();

            DrawGeneralSettings();

            DrawInputAudioTranscription();

            DrawInputAudioRecording();

            DrawOutputAudio();

            DrawEventReceivers();

            GUILayout.Space(10);

            AIDevKitGUI.Presets.OfficialApiDocButton(Api.OpenAI, "Realtime API", "https://platform.openai.com/docs/guides/realtime");

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGeneralSettings()
        {
            ExGUILayout.BeginSection("General Settings");
            {
                AIDevKitGUI.RTMPopup(rtmModel, Api.OpenAI, label: new GUIContent("Realtime Model"));
                AIDevKitGUI.VoicePopup(voice, Api.OpenAI, label: GUIContents.Voice);
                ExEditorGUI.ExpandableTextField("Instructions", instructions);
                EditorGUILayout.PropertyField(autoStart, new GUIContent("Auto Start", "Automatically start the assistant when the scene starts."));
            }
            ExGUILayout.EndSection();
        }

        private void DrawInputAudioTranscription()
        {
            ExGUILayout.BeginSection("Input Audio Transcription");
            {
                AIDevKitGUI.STTPopup(sttModel, Api.OpenAI, label: new GUIContent("Speech-to-Text Model"));
                EditorGUILayout.PropertyField(spokenLanguage, new GUIContent("Spoken Language", "The language of the spoken audio. Only used when input transcription is enabled."));
            }
            ExGUILayout.EndSection();
        }

        private void DrawInputAudioRecording()
        {
            ExGUILayout.BeginSection("Input Audio Recording", () =>
            {
                if (ExGUI.ResetButton())
                {
                    ResetInputSettingsToDefault();
                    SaveTarget();
                }
            });
            {
                EditorGUILayout.PropertyField(inputAudioFormat, new GUIContent("Input Audio Format"));
                EditorGUILayout.PropertyField(inputSampleRate, new GUIContent("Input Audio Sample Rate"));
                EditorGUILayout.PropertyField(inputSampleDurationMs, new GUIContent("Input Sample Duration (ms)"));
                EditorGUILayout.PropertyField(silenceDurationMs, new GUIContent("Silence Duration (ms)"));
                EditorGUILayout.PropertyField(silenceThreshold, new GUIContent("Silence Threshold"));
            }
            ExGUILayout.EndSection();
        }

        private void DrawOutputAudio()
        {
            ExGUILayout.BeginSection("Output Audio", () =>
            {
                if (ExGUI.ResetButton())
                {
                    ResetOutputSettingsToDefault();
                    SaveTarget();
                }
            });
            {
                EditorGUILayout.PropertyField(outputAudioFormat, new GUIContent("Output Audio Format"));
                EditorGUILayout.PropertyField(outputAudioVolume, new GUIContent("Output Audio Volume", "Volume of the output audio."));
            }
            ExGUILayout.EndSection();
        }

        private void DrawEventReceivers()
        {
            ExGUILayout.BeginSection("Event Managers & Receivers");
            {
                EditorGUILayout.PropertyField(functionManager, GUIContents.FunctionManager);
                EditorGUILayout.PropertyField(realtimeEventReceiver, GUIContents.RealtimeEventReceiver);
                EditorGUILayout.PropertyField(webSocketEventReceiver, GUIContents.WebSocketEventReceiver);
                EditorGUILayout.PropertyField(inputTranscriptionEventReceiver, new GUIContent("Input Transcription Event Receiver"));
                EditorGUILayout.PropertyField(textEventReceiver, new GUIContent("Text Event Receiver"));
                EditorGUILayout.PropertyField(transcriptEventReceiver, new GUIContent("Transcript Event Receiver"));
                EditorGUILayout.PropertyField(toolEventReceiver, GUIContents.ToolCallReceiver);
            }
            ExGUILayout.EndSection();
        }
    }
}