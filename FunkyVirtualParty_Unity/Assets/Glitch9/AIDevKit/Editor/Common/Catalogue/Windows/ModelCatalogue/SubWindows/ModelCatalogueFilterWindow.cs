using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEditor;
using UnityEngine;


namespace Glitch9.AIDevKit.Editor
{
    internal class ModelCatalogueFilterWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        internal static void ShowWindow(ModelCatalogueWindow window)
        {
            ModelCatalogueFilterWindow popup = GetWindow<ModelCatalogueFilterWindow>(true, "Model Filter", true);
            popup._window = window;
            popup.Show();
        }

        ModelCatalogueWindow _window;

        private void OnGUI()
        {
            GUILayout.BeginVertical(ExStyles.paddedArea);
            try
            {
                try
                {
                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
                    EditorGUI.BeginChangeCheck();
                    {
                        DrawGUI();
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        _window.OnFilterChanged();
                    }
                }
                finally
                {
                    GUILayout.EndScrollView();
                }
            }
            finally
            {
                GUILayout.EndVertical();
            }
        }

        private void DrawGUI()
        {
            TreeViewGUI.BeginSection("Filter by Capability");
            {
                ModelCatalogueFilter.TextGeneration = AIDevKitGUI.TreeView.BeginFilterSection("Text Generation", ModelCatalogueFilter.TextGeneration);
                {
                    ModelCatalogueFilter.Streaming = EditorGUILayout.ToggleLeft("Streaming", ModelCatalogueFilter.Streaming);
                    ModelCatalogueFilter.FineTuning = EditorGUILayout.ToggleLeft("Fine Tuning", ModelCatalogueFilter.FineTuning);
                    ModelCatalogueFilter.FunctionCalling = EditorGUILayout.ToggleLeft("Function Calling", ModelCatalogueFilter.FunctionCalling);
                    ModelCatalogueFilter.StructuredOutputs = EditorGUILayout.ToggleLeft("Structured Outputs", ModelCatalogueFilter.StructuredOutputs);
                    ModelCatalogueFilter.CodeExecution = EditorGUILayout.ToggleLeft("Code Execution", ModelCatalogueFilter.CodeExecution);
                }
                AIDevKitGUI.TreeView.EndFilterSection();

                ModelCatalogueFilter.ImageGeneration = AIDevKitGUI.TreeView.BeginFilterSection("Image Generation", ModelCatalogueFilter.ImageGeneration);
                {
                    ModelCatalogueFilter.ImageInpainting = EditorGUILayout.ToggleLeft("Inpainting", ModelCatalogueFilter.ImageInpainting);
                }
                AIDevKitGUI.TreeView.EndFilterSection();

                ModelCatalogueFilter.SpeechGeneration = EditorGUILayout.ToggleLeft("Speech Generation", ModelCatalogueFilter.SpeechGeneration);
                ModelCatalogueFilter.SpeechRecognition = EditorGUILayout.ToggleLeft("Speech Recognition", ModelCatalogueFilter.SpeechRecognition);
                ModelCatalogueFilter.VoiceChanger = EditorGUILayout.ToggleLeft("Voice Change", ModelCatalogueFilter.VoiceChanger);
                ModelCatalogueFilter.SoundFXGeneration = EditorGUILayout.ToggleLeft("SoundFX Generation", ModelCatalogueFilter.SoundFXGeneration);
                ModelCatalogueFilter.VideoGeneration = EditorGUILayout.ToggleLeft("Video Generation", ModelCatalogueFilter.VideoGeneration);
                ModelCatalogueFilter.TextEmbedding = EditorGUILayout.ToggleLeft("Text Embedding", ModelCatalogueFilter.TextEmbedding);
                ModelCatalogueFilter.Moderation = EditorGUILayout.ToggleLeft("Moderation", ModelCatalogueFilter.Moderation);
                ModelCatalogueFilter.Search = EditorGUILayout.ToggleLeft("Search", ModelCatalogueFilter.Search);
                ModelCatalogueFilter.Realtime = EditorGUILayout.ToggleLeft("Real-time", ModelCatalogueFilter.Realtime);
                ModelCatalogueFilter.ComputerUse = EditorGUILayout.ToggleLeft("Computer Use", ModelCatalogueFilter.ComputerUse);
                ModelCatalogueFilter.VoiceIsolation = EditorGUILayout.ToggleLeft("Voice Isolation", ModelCatalogueFilter.VoiceIsolation);
            }
            TreeViewGUI.EndSection();


            TreeViewGUI.BeginSection("Filter by Source");
            {
                ModelCatalogueFilter.OfficialModels = EditorGUILayout.ToggleLeft("Official Models", ModelCatalogueFilter.OfficialModels);
                ModelCatalogueFilter.CustomModels = EditorGUILayout.ToggleLeft("Fine-tuned(Custom) Models", ModelCatalogueFilter.CustomModels);
                ModelCatalogueFilter.DefaultModels = EditorGUILayout.ToggleLeft("AIDevKit Default Models", ModelCatalogueFilter.DefaultModels);
            }
            TreeViewGUI.EndSection();

            TreeViewGUI.BeginSection("Filter by Status");
            {
                ModelCatalogueFilter.InMyLibrary = EditorGUILayout.ToggleLeft("In My Library", ModelCatalogueFilter.InMyLibrary);
                ModelCatalogueFilter.MissingModels = EditorGUILayout.ToggleLeft("Not In My Library", ModelCatalogueFilter.MissingModels);
                ModelCatalogueFilter.LegacyModels = EditorGUILayout.ToggleLeft("Legacy Models", ModelCatalogueFilter.LegacyModels);
                ModelCatalogueFilter.DeprecatedModels = EditorGUILayout.ToggleLeft("Deprecated Models", ModelCatalogueFilter.DeprecatedModels);
            }
            TreeViewGUI.EndSection();
        }
    }
}