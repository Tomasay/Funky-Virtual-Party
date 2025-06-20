using Glitch9.AIDevKit.Components;
using Glitch9.Editor;
using UnityEditor;

namespace Glitch9.AIDevKit.Editor
{
    [CustomEditor(typeof(VoiceChanger))]
    public class VoiceChangerEditor : AudioInputComponentEditorBase
    {
        private SerializedProperty onAudioClipGenerated;
        private SerializedProperty outputPath;

        protected override void OnEnable()
        {
            base.OnEnable();
            onAudioClipGenerated = serializedObject.FindProperty(nameof(onAudioClipGenerated));
            outputPath = serializedObject.FindProperty(nameof(outputPath));
        }

        private void DrawGeneralSettings()
        {
            ExGUILayout.BeginSection("General Settings");
            {
                AIDevKitGUI.VCMPopup(model, label: GUIContents.Model);
                AIDevKitGUI.OutputPathField(outputPath);
                EditorGUILayout.PropertyField(errorReceiver);
            }
            ExGUILayout.EndSection();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ExGUIPreset.UnityComponentLabel(AIDevKitIcons.Voice, "Voice Changer", "A component that modifies audio input to change the voice characteristics.");
            EditorGUILayout.Space();

            DrawGeneralSettings();

            DrawRecordingSettings();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(onAudioClipGenerated);

            serializedObject.ApplyModifiedProperties();
        }
    }
}