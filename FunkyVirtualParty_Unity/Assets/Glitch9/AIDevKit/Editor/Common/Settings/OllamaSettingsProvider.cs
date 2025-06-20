using System.Collections.Generic;
using Glitch9.AIDevKit.Ollama;
using Glitch9.Editor;
using Glitch9.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Ollama
{
    internal class OllamaSettingsProvider : ExtendedSettingsProvider<OllamaSettingsProvider, OllamaSettings>
    {
        private const int kLabelWidth = 240;

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            OllamaSettingsProvider provider = new(AIDevKitEditor.Providers.Ollama)
            {
                deactivateHandler = DeactivateHandler,//OllamaSettings.Instance.Save,
                keywords = Keywords
            };
            return provider;
        }

        private static HashSet<string> Keywords { get; } = new()
        {
            "api",
            "auto update",
            "models",
        };

        private static void DeactivateHandler()
        {
            OllamaSettings.Instance.SaveAsset();
        }

        private readonly static GUIContent kTitle = new("Server Configuration", "Configuration for the Ollama server.");
        private readonly static GUIContent kEndpointLabel = new("Endpoint (Required)", "The endpoint for the Ollama API.");
        private readonly static GUIContent kPortLabel = new("Port (Required)", "The port for the Ollama API.");

        private SerializedProperty endpoint;
        private SerializedProperty port;
        private SerializedProperty defaultModel;
        private SerializedProperty enableOllama;

        public OllamaSettingsProvider(string path) : base(path, SettingsScope.User) { }

        protected override void InitializeSettings()
        {
            endpoint = serializedObject.FindProperty(nameof(endpoint));
            port = serializedObject.FindProperty(nameof(port));
            defaultModel = serializedObject.FindProperty(nameof(defaultModel));
            enableOllama = serializedObject.FindProperty(nameof(enableOllama));
        }

        protected override void DrawSettings()
        {
            bool notAvailable = !AIDevKitConfig.IsPro;

            EditorGUI.BeginDisabledGroup(notAvailable);

            EditorGUIUtility.labelWidth = kLabelWidth;

            DrawGeneralSettings();

            DrawDefaultModels();

            DrawUsefulLinks();

            EditorGUIUtility.labelWidth = 0;

            EditorGUI.EndDisabledGroup();

            if (notAvailable) AIDevKitGUI.Presets.DrawProRequiredWarning();
        }

        protected void DrawGeneralSettings()
        {
            ExGUILayout.BeginSection(kTitle);
            {
                EditorGUILayout.PropertyField(enableOllama, new GUIContent("Enable Ollama", "Enable or disable the Ollama server connection."));

                EditorGUI.BeginDisabledGroup(!enableOllama.boolValue);
                {
                    EditorGUILayout.PropertyField(endpoint, kEndpointLabel);
                    EditorGUILayout.PropertyField(port, kPortLabel);
                    if (ExGUILayout.ButtonField("Use Default Configuration"))
                    {
                        endpoint.stringValue = "localhost";
                        port.intValue = 11434;
                    }
                    if (ExGUILayout.ButtonField("Check Connection"))
                    {
                        AIDevKitEditor.CheckOllamaServerStatus();
                    }
                }
                EditorGUI.EndDisabledGroup();

                if (!enableOllama.boolValue)
                {
                    EditorGUILayout.HelpBox("Ollama is disabled. Please enable it to use Ollama features.", MessageType.Warning);
                }
            }
            ExGUILayout.EndSection();
        }

        protected void DrawDefaultModels()
        {
            ExGUILayout.BeginSection(GUIContents.DefaultModelsSectionTitle);
            {
                EditorGUI.BeginDisabledGroup(!enableOllama.boolValue);
                {
                    AIDevKitGUI.LLMPopup(defaultModel, Api.Ollama, GUIContents.DefaultLLM);
                }
                EditorGUI.EndDisabledGroup();
            }
            ExGUILayout.EndSection();
        }


        protected void DrawUsefulLinks()
        {
            ExGUILayout.BeginSection(GUIContents.UsefulLinksSectionTitle);
            {
                AIDevKitGUI.Presets.DrawUrlButtons(
                    ("Ollama Website", "https://ollama.com/"),
                    ("Ollama Download", "https://ollama.com/download"),
                    ("Ollama Models", "https://ollama.com/library")
                );
            }
            ExGUILayout.EndSection();
        }
    }
}