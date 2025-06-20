using Glitch9.AIDevKit.Components;
using Glitch9.Editor;
using UnityEditor;

namespace Glitch9.AIDevKit.Editor
{
    [CustomEditor(typeof(ImageGenerator))]
    public class ImageGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty model;
        private SerializedProperty outputPath;

        private SerializedProperty quality;
        private SerializedProperty size;
        private SerializedProperty style;

        private SerializedProperty aspectRatio;
        private SerializedProperty personGeneration;

        private SerializedProperty onTextureGenerated;
        private SerializedProperty errorReceiver;

        private void OnEnable()
        {
            model = serializedObject.FindProperty("model");
            outputPath = serializedObject.FindProperty("outputPath");
            quality = serializedObject.FindProperty("quality");
            size = serializedObject.FindProperty("size");
            style = serializedObject.FindProperty("style");
            aspectRatio = serializedObject.FindProperty("aspectRatio");
            personGeneration = serializedObject.FindProperty("personGeneration");
            onTextureGenerated = serializedObject.FindProperty("onTextureGenerated");
            errorReceiver = serializedObject.FindProperty("errorReceiver");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ExGUIPreset.UnityComponentLabel(AIDevKitIcons.Inpainting, "Image Generator", "A component that generates images from text input.");
            EditorGUILayout.Space();

            ExGUILayout.BeginSection("General Settings");
            {
                AIDevKitGUI.STTPopup(model, label: GUIContents.Model);
                AIDevKitGUI.OutputPathField(outputPath);
                EditorGUILayout.PropertyField(errorReceiver, GUIContents.ErrorReceiver);
            }
            ExGUILayout.EndSection();

            ExGUILayout.BeginSection("OpenAI Settings");
            {
                EditorGUILayout.PropertyField(quality, GUIContents.ImageQuality);
                EditorGUILayout.PropertyField(size, GUIContents.ImageSize);
                EditorGUILayout.PropertyField(style, GUIContents.ImageStyle);
            }
            ExGUILayout.EndSection();

            ExGUILayout.BeginSection("Google Settings");
            {
                EditorGUILayout.PropertyField(aspectRatio, GUIContents.AspectRatio);
                EditorGUILayout.PropertyField(personGeneration, GUIContents.PersonGeneration);
            }
            ExGUILayout.EndSection();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(onTextureGenerated);

            serializedObject.ApplyModifiedProperties();
        }
    }
}