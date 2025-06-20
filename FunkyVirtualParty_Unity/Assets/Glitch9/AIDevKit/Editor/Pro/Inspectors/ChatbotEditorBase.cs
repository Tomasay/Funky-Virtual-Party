using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public abstract class ChatbotEditorBase : UnityEditor.Editor
    {
        private SerializedProperty errorReceiver;

        // Modules
        private SerializedProperty functionManager;
        private SerializedProperty sttModule;
        private SerializedProperty ttsModule;
        private SerializedProperty imageModule;

        // Receivers
        private SerializedProperty chatEventReceiver;
        private SerializedProperty toolCallReceiver;



        protected virtual void OnEnable()
        {
            errorReceiver = serializedObject.FindProperty(nameof(errorReceiver));

            // Modules
            functionManager = serializedObject.FindProperty(nameof(functionManager));
            sttModule = serializedObject.FindProperty(nameof(sttModule));
            ttsModule = serializedObject.FindProperty(nameof(ttsModule));
            imageModule = serializedObject.FindProperty(nameof(imageModule));

            // Receivers
            chatEventReceiver = serializedObject.FindProperty(nameof(chatEventReceiver));
            toolCallReceiver = serializedObject.FindProperty(nameof(toolCallReceiver));
        }

        protected abstract void DrawUnityComponentLabel();

        protected virtual void DrawExtraSettings()
        {
            // This method can be overridden in derived classes to add extra settings.
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawUnityComponentLabel();
            EditorGUILayout.Space();

            DrawInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawInspectorGUI()
        {
            DrawGeneralSettings();

            DrawChatbotProfile();

            DrawAdvancedOptions();

            DrawModuleSettings();

            DrawEventReceivers();

            DrawExtraSettings();
        }


        protected abstract void DrawGeneralSettings();
        protected abstract void DrawChatbotProfile();

        protected virtual void DrawModuleSettings()
        {
            ExGUILayout.BeginSection(new GUIContent("Modules", "Optional modules that can enhance the chatbot's capabilities."));
            {
                EditorGUILayout.PropertyField(functionManager, GUIContents.FunctionManager);
                EditorGUILayout.PropertyField(sttModule, new GUIContent("Speech-to-Text", "The module used for converting speech input to text."));
                EditorGUILayout.PropertyField(ttsModule, new GUIContent("Text-to-Speech", "The module used for converting text output to speech."));
                EditorGUILayout.PropertyField(imageModule, new GUIContent("Image Generator", "The module used for generating images based on text prompts."));
            }
            ExGUILayout.EndSection();
        }

        protected abstract void DrawAdvancedOptions();
        protected abstract void DrawStreamReceiver();
        protected virtual void DrawEventReceivers()
        {
            ExGUILayout.BeginSection(new GUIContent("Event Receivers", "Event receivers for handling various chatbot events. These can be used to trigger actions when certain events occur in the chat session."));
            {
                EditorGUILayout.PropertyField(chatEventReceiver, GUIContents.ChatEventReceiver);
                EditorGUILayout.PropertyField(toolCallReceiver, GUIContents.ToolCallReceiver);
                DrawStreamReceiver();
                EditorGUILayout.PropertyField(errorReceiver, GUIContents.ErrorReceiver);
            }
            ExGUILayout.EndSection();
        }


    }
}