using System;
using System.Collections.Generic;
using Glitch9.AIDevKit.Components;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public abstract class ChatbotEditorBaseV2 : UnityEditor.Editor
    {
        protected class ChatbotFeature
        {
            public Texture Icon;
            public string Label;
            public string Tooltip;
            public Func<bool> GetValue;
            public Action<bool> SetValue;
            public Func<bool, bool, bool> HandleChange;
        }

        protected static bool AdvancedView
        {
            get => EditorPrefs.GetBool("AIDevKit.Chatbot.AdvancedView", false);
            set => EditorPrefs.SetBool("AIDevKit.Chatbot.AdvancedView", value);
        }

        private SerializedProperty errorReceiver;

        // Modules
        private SerializedProperty functionManager;
        private SerializedProperty sttModule;
        private SerializedProperty ttsModule;
        private SerializedProperty imageModule;
        private SerializedProperty moderatorModule;

        // Receivers
        private SerializedProperty chatEventReceiver;
        private SerializedProperty toolCallReceiver;

        protected virtual bool Stream { get; set; }
        protected bool Functions { get; set; }
        protected bool ImageGeneration { get; set; }
        protected bool TextToSpeech { get; set; }
        protected bool SpeechToText { get; set; }
        protected bool Moderation { get; set; }

        protected List<ChatbotFeature> features;



        protected virtual void OnEnable()
        {
            errorReceiver = serializedObject.FindProperty(nameof(errorReceiver));

            // Modules
            functionManager = serializedObject.FindProperty(nameof(functionManager));
            sttModule = serializedObject.FindProperty(nameof(sttModule));
            ttsModule = serializedObject.FindProperty(nameof(ttsModule));
            imageModule = serializedObject.FindProperty(nameof(imageModule));
            moderatorModule = serializedObject.FindProperty(nameof(moderatorModule));

            // Receivers
            chatEventReceiver = serializedObject.FindProperty(nameof(chatEventReceiver));
            toolCallReceiver = serializedObject.FindProperty(nameof(toolCallReceiver));

            Functions = functionManager.objectReferenceValue != null;
            ImageGeneration = imageModule.objectReferenceValue != null;
            TextToSpeech = ttsModule.objectReferenceValue != null;
            SpeechToText = sttModule.objectReferenceValue != null;
            Moderation = moderatorModule.objectReferenceValue != null;

            features = new()
            {
                new ChatbotFeature
                {
                    Icon = AIDevKitIcons.Streaming,
                    Label = "Stream",
                    Tooltip = "When enabled, responses are streamed in real-time...",
                    GetValue = () => Stream,
                    SetValue = v => Stream = v,
                    HandleChange = (oldVal, newVal) => HandleStreamToggleChange(oldVal, newVal)
                },
                new ChatbotFeature
                {
                    Icon = AIDevKitIcons.Tools,
                    Label = "Functions",
                    Tooltip = "When enabled, the chatbot can call functions...",
                    GetValue = () => Functions,
                    SetValue = v => Functions = v,
                    HandleChange = (oldVal, newVal) => HandleModuleToggleChange<FunctionManager>(functionManager, oldVal, newVal)
                },
                new ChatbotFeature
                {
                    Icon = AIDevKitIcons.Inpainting,
                    Label = "Image\nGeneration",
                    Tooltip = "Generate images from text prompts.",
                    GetValue = () => ImageGeneration,
                    SetValue = v => ImageGeneration = v,
                    HandleChange = (oldVal, newVal) => HandleModuleToggleChange<ImageGenerator>(imageModule, oldVal, newVal)
                },
                new ChatbotFeature
                {
                    Icon = AIDevKitIcons.TextToSpeech,
                    Label = "Text-to\nSpeech",
                    Tooltip = "Chatbot will speak using TTS.",
                    GetValue = () => TextToSpeech,
                    SetValue = v => TextToSpeech = v,
                    HandleChange = (oldVal, newVal) => HandleModuleToggleChange<TextToSpeech>(ttsModule, oldVal, newVal)
                },
                new ChatbotFeature
                {
                    Icon = AIDevKitIcons.SpeechToText,
                    Label = "Speech-to\nText",
                    Tooltip = "Microphone input is transcribed into text.",
                    GetValue = () => SpeechToText,
                    SetValue = v => SpeechToText = v,
                    HandleChange = (oldVal, newVal) => HandleModuleToggleChange<SpeechToText>(sttModule, oldVal, newVal)
                },
                new ChatbotFeature
                {
                    Icon = AIDevKitIcons.Moderation,
                    Label = "Moderation",
                    Tooltip = "Content moderation enabled.",
                    GetValue = () => Moderation,
                    SetValue = v => Moderation = v,
                    HandleChange = (oldVal, newVal) => HandleModuleToggleChange<Moderator>(moderatorModule, oldVal, newVal)
                }
            };
        }

        protected abstract void DrawUnityComponentLabel();
        private void DrawCoreReceivers()
        {
            ExGUILayout.BeginSection(new GUIContent("Core Receivers", "Core receivers for handling basic chatbot events. These are essential for the chatbot's functionality."));
            {
                EditorGUILayout.PropertyField(chatEventReceiver, GUIContents.ChatEventReceiver);
                EditorGUILayout.PropertyField(toolCallReceiver, GUIContents.ToolCallReceiver);
                EditorGUILayout.PropertyField(errorReceiver, GUIContents.ErrorReceiver);
            }
            ExGUILayout.EndSection();
        }

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
            DrawChatbotProfile();

            DrawChatBehaviour();

            if (AdvancedView)
            {
                DrawModuleSettings();
                DrawEventReceivers();
                DrawAdvancedOptions();
                DrawExtraSettings();
            }
            else
            {
                DrawCoreReceivers();
                DrawChatbotFeatures();
            }

            DrawStatusMessage();
        }

        protected abstract void DrawChatbotProfile();
        protected abstract void DrawChatBehaviour();
        protected abstract void DrawStatusMessage();
        protected virtual void DrawModuleSettings()
        {
            ExGUILayout.BeginSection(new GUIContent("Modules", "Optional modules that can enhance the chatbot's capabilities."));
            {
                EditorGUILayout.PropertyField(functionManager, GUIContents.FunctionManager);
                EditorGUILayout.PropertyField(sttModule, new GUIContent("Speech-to-Text", "The module used for converting speech input to text."));
                EditorGUILayout.PropertyField(ttsModule, new GUIContent("Text-to-Speech", "The module used for converting text output to speech."));
                EditorGUILayout.PropertyField(imageModule, new GUIContent("Image Generator", "The module used for generating images based on text prompts."));
                EditorGUILayout.PropertyField(moderatorModule, new GUIContent("Moderator", "The module used for moderating content and ensuring safety in responses."));
            }
            ExGUILayout.EndSection();
        }

        protected virtual void DrawChatbotFeatures()
        {
            ExGUILayout.BeginSection(new GUIContent("Chatbot Features", "Optional features that can enhance the chatbot's capabilities."));
            {
                const float tileWidth = 64f;
                const float tileMargin = 4f;
                const float totalTileWidth = tileWidth + tileMargin * 2;
                float viewWidth = EditorGUIUtility.currentViewWidth;

                // 줄당 몇 개 들어갈 수 있는지 계산
                int tilesPerRow = Mathf.Max(1, Mathf.FloorToInt(viewWidth / totalTileWidth));

                // 줄 수 만큼 반복해서 나눠서 출력

                for (int i = 0; i < features.Count; i += tilesPerRow)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(15f); // indent 전체 시작

                        for (int j = 0; j < tilesPerRow; j++)
                        {
                            int index = i + j;
                            if (index >= features.Count)
                                break;

                            var feature = features[index];
                            bool oldVal = feature.GetValue();
                            bool newVal = AIDevKitGUI.ModelCapabilityToggle(feature.Icon, feature.Label, feature.Tooltip, oldVal);
                            bool changed = feature.HandleChange?.Invoke(oldVal, newVal) ?? newVal;
                            feature.SetValue?.Invoke(changed);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }

            ExGUILayout.EndSection();
        }

        protected abstract bool HandleStreamToggleChange(bool oldValue, bool newValue);

        protected bool HandleModuleToggleChange<T>(SerializedProperty moduleProperty, bool oldValue, bool newValue) where T : MonoBehaviour
        {
            if (oldValue == newValue) return oldValue; // No change

            if (newValue)
            {
                if (moduleProperty.objectReferenceValue == null)
                {
                    moduleProperty.objectReferenceValue = GetOrCreateModule<T>();
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                RemoveModule<T>();
                moduleProperty.objectReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
            }
            return newValue; // Return the new value after applying changes
        }

        protected void DrawAdvancedViewToggle()
        {
            AdvancedView = GUILayout.Toggle(AdvancedView, new GUIContent("Advanced View", "Enable advanced view to show additional configuration options for the chatbot."), "Button");
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

        protected T GetOrCreateModule<T>() where T : MonoBehaviour
        {
            // 현재 오브젝트와 children에서 T 타입의 모듈을 찾습니다.
            MonoBehaviour target = (MonoBehaviour)this.target;
            T[] modules = target.GetComponentsInChildren<T>(true);

            if (modules.Length > 0)
            {
                return modules[0]; // 첫 번째 모듈을 반환합니다.
            }
            else
            {
                //string childGOName = typeof(T).Name.ToTitleCase();

                // 모듈이 없으면 새로운 GameObject를 생성하고 해당 모듈을 추가합니다.
                // GameObject childGO = new(childGOName);
                // childGO.transform.SetParent(target.transform, false);
                // T newModule = childGO.AddComponent<T>();

                // 자식이 아니라 self에 모듈을 추가합니다. (기획 변경, 초보자 대상으로)
                T newModule = target.gameObject.AddComponent<T>();

                //Debug.Log($"Created new {childGOName} module for {target.name}.");

                // 초보자는 초가되었다는 사실을 모를수 있음으로, Dialog를 표시해서 현재 오브젝트의 하위에 새로 생성된 모듈이 추가되었음을 알립니다.
                // if (EditorUtility.DisplayDialog(
                //     "Module Created",
                //     $"A new {childGOName} module has been created and added as a child of current object '{target.name}'.\n\n" +
                //     "You can now configure it in the inspector.",
                //     "Focus on Module",
                //     "OK"))
                // {
                //     // 새로 생성된 모듈에 포커스를 맞춥니다.
                //     Selection.activeGameObject = childGO;
                //     EditorGUIUtility.PingObject(childGO);
                // }

                return newModule; // 새로 생성한 모듈을 반환합니다;
            }
        }

        protected void RemoveModule<T>() where T : MonoBehaviour
        {
            MonoBehaviour target = (MonoBehaviour)this.target;
            T[] modules = target.GetComponentsInChildren<T>(true);

            if (modules.Length > 0)
            {
                foreach (var module in modules)
                {
                    DestroyImmediate(module);
                }
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogWarning($"No {typeof(T).Name} module found to remove.");
            }
        }
    }
}