using Glitch9.AIDevKit.Components;
using Glitch9.Editor;
using UnityEditor;

namespace Glitch9.AIDevKit.Editor
{
    [CustomEditor(typeof(SpeechToText))]
    public class SpeechToTextEditor : AudioInputComponentEditorBase
    {
        private SerializedProperty spokenLanguage;
        private SerializedProperty onTextGenerated;

        protected override void OnEnable()
        {
            base.OnEnable();
            spokenLanguage = serializedObject.FindProperty("spokenLanguage");
            onTextGenerated = serializedObject.FindProperty("onTextGenerated");
        }

        private void DrawGeneralSettings()
        {
            ExGUILayout.BeginSection("General Settings");
            {
                AIDevKitGUI.STTPopup(model, label: GUIContents.Model);
                EditorGUILayout.PropertyField(spokenLanguage, GUIContents.SpokenLanguage);
                EditorGUILayout.PropertyField(errorReceiver);
            }
            ExGUILayout.EndSection();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ExGUIPreset.UnityComponentLabel(AIDevKitIcons.SpeechToText, "Speech To Text", "A component that transcribes audio input into text.");
            EditorGUILayout.Space();

            DrawGeneralSettings();
            DrawRecordingSettings();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(onTextGenerated);

            serializedObject.ApplyModifiedProperties();
        }
    }
}