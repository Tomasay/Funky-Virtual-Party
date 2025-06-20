using Glitch9.AIDevKit.Components;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    [CustomEditor(typeof(TextToSpeech))]
    public class TextToSpeechEditor : UnityEditor.Editor
    {
        private SerializedProperty model;
        private SerializedProperty outputPath;
        private SerializedProperty voice;
        private SerializedProperty onAudioClipGenerated;
        private SerializedProperty errorReceiver;
        private SerializedProperty playOnGeneration;

        private void OnEnable()
        {
            model = serializedObject.FindProperty("model");
            outputPath = serializedObject.FindProperty("outputPath");
            voice = serializedObject.FindProperty("voice");
            onAudioClipGenerated = serializedObject.FindProperty("onAudioClipGenerated");
            errorReceiver = serializedObject.FindProperty("errorReceiver");
            playOnGeneration = serializedObject.FindProperty("playOnGeneration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ExGUIPreset.UnityComponentLabel(AIDevKitIcons.TextToSpeech, "Text To Speech", "A component that generates speech audio from text input.");
            EditorGUILayout.Space();

            AIDevKitGUI.TTSPopup(model, label: GUIContents.Model);

            Model m = model.stringValue;
            Api api = Api.OpenAI;
            if (m != null) api = m.Api;
            AIDevKitGUI.VoicePopup(voice, api, label: GUIContents.Voice);

            EditorGUILayout.PropertyField(playOnGeneration, new GUIContent("Play On Generation", "If enabled, the generated audio will play automatically after generation."));
            AIDevKitGUI.OutputPathField(outputPath);
            EditorGUILayout.PropertyField(errorReceiver);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(onAudioClipGenerated);

            serializedObject.ApplyModifiedProperties();
        }
    }
}