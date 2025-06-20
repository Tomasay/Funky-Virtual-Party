using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.Ollama;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.AIDevKit.OpenRouter;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public partial class ModelCatalogueWindow
    {
        protected override void BottomBar()
        {
            if (TreeView == null) return;

            GUILayout.BeginHorizontal(AIDevKitStyles.SearchBarStyle);
            try
            {
                DrawSearchBar();
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        internal void OnFilterChanged()
        {
            TreeView.ReloadTreeView(true);
        }

        private void ResetFilters()
        {
            ModelCatalogueFilter.ApiProvider = Api.All;
            ModelCatalogueFilter.ModelProvider = Api.All;

            ModelCatalogueFilter.MissingModels = false;
            ModelCatalogueFilter.DefaultModels = false;
            ModelCatalogueFilter.OfficialModels = false;
            ModelCatalogueFilter.CustomModels = false;
            ModelCatalogueFilter.InMyLibrary = false;

            ModelCatalogueFilter.DeprecatedModels = false;
            ModelCatalogueFilter.LegacyModels = false;

            ModelCatalogueFilter.TextGeneration = false;
            ModelCatalogueFilter.StructuredOutputs = false;
            ModelCatalogueFilter.FunctionCalling = false;
            ModelCatalogueFilter.CodeExecution = false;
            ModelCatalogueFilter.FineTuning = false;
            ModelCatalogueFilter.Streaming = false;
            ModelCatalogueFilter.ImageGeneration = false;
            ModelCatalogueFilter.ImageInpainting = false;
            ModelCatalogueFilter.SpeechGeneration = false;
            ModelCatalogueFilter.SpeechRecognition = false;
            ModelCatalogueFilter.SoundFXGeneration = false;
            ModelCatalogueFilter.VoiceChanger = false;
            ModelCatalogueFilter.VideoGeneration = false;
            ModelCatalogueFilter.TextEmbedding = false;
            ModelCatalogueFilter.Moderation = false;
            ModelCatalogueFilter.Search = false;
            ModelCatalogueFilter.Realtime = false;
            ModelCatalogueFilter.ComputerUse = false;

            TreeView.Filter.SearchText = string.Empty;
            TreeView.ReloadTreeView(true);
        }

        private void DrawSearchBar()
        {
            const float kMinWidth = 130f;
            const float kSpace = 10f;

            GUILayout.BeginVertical(GUILayout.MinWidth(kMinWidth));
            {
                EditorGUIUtility.labelWidth = 120f;

                GUILayout.BeginHorizontal();
                try
                {
                    GUILayout.Label($"Displaying {TreeView.ShowingCount}/{TreeView.TotalCount}", EditorStyles.boldLabel, GUILayout.Height(18f), GUILayout.MaxWidth(156f));

                    if (GUILayout.Button(new GUIContent(EditorIcons.Reset, "Reset Filters"), ExStyles.miniButton, GUILayout.Width(20f)))
                    {
                        ResetFilters();
                    }
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }

                EditorGUIUtility.labelWidth = 24f;

                Api apiProvider = ExGUILayout.EnumPopup(new GUIContent("API"), ModelCatalogueFilter.ApiProvider, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(180f));
                if (apiProvider != ModelCatalogueFilter.ApiProvider)
                {
                    ModelCatalogueFilter.ApiProvider = apiProvider;
                    TreeView.ReloadTreeView(true);
                }
            }
            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 110f;
            GUILayout.Space(kSpace);

            GUILayout.BeginVertical();
            {
                bool textGeneration = EditorGUILayout.ToggleLeft("Text Generation", ModelCatalogueFilter.TextGeneration, GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true));
                if (textGeneration != ModelCatalogueFilter.TextGeneration)
                {
                    ModelCatalogueFilter.TextGeneration = textGeneration;
                    TreeView.ReloadTreeView(true);
                }

                bool embedding = EditorGUILayout.ToggleLeft("Text Embedding", ModelCatalogueFilter.TextEmbedding, GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true));
                if (embedding != ModelCatalogueFilter.TextEmbedding)
                {
                    ModelCatalogueFilter.TextEmbedding = embedding;
                    TreeView.ReloadTreeView(true);
                }
            }
            GUILayout.EndVertical();

            GUILayout.Space(kSpace);

            GUILayout.BeginVertical();
            {
                bool imageGeneration = EditorGUILayout.ToggleLeft("Image Generation", ModelCatalogueFilter.ImageGeneration, GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true));
                if (imageGeneration != ModelCatalogueFilter.ImageGeneration)
                {
                    ModelCatalogueFilter.ImageGeneration = imageGeneration;
                    TreeView.ReloadTreeView(true);
                }

                bool videoGeneration = EditorGUILayout.ToggleLeft("Video Generation", ModelCatalogueFilter.VideoGeneration, GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true));
                if (videoGeneration != ModelCatalogueFilter.VideoGeneration)
                {
                    ModelCatalogueFilter.VideoGeneration = videoGeneration;
                    TreeView.ReloadTreeView(true);
                }
            }
            GUILayout.EndVertical();

            GUILayout.Space(kSpace);

            GUILayout.BeginVertical();
            {
                bool speechGeneration = EditorGUILayout.ToggleLeft("Text-to-Speech", ModelCatalogueFilter.SpeechGeneration, GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true));
                if (speechGeneration != ModelCatalogueFilter.SpeechGeneration)
                {
                    ModelCatalogueFilter.SpeechGeneration = speechGeneration;
                    TreeView.ReloadTreeView(true);
                }

                bool speechRecognition = EditorGUILayout.ToggleLeft("Speech-to-Text", ModelCatalogueFilter.SpeechRecognition, GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true));
                if (speechRecognition != ModelCatalogueFilter.SpeechRecognition)
                {
                    ModelCatalogueFilter.SpeechRecognition = speechRecognition;
                    TreeView.ReloadTreeView(true);
                }
            }
            GUILayout.EndVertical();

            GUILayout.Space(kSpace);

            GUILayout.BeginVertical();
            {
                bool moderation = EditorGUILayout.ToggleLeft("Moderation", ModelCatalogueFilter.Moderation, GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true));
                if (moderation != ModelCatalogueFilter.Moderation)
                {
                    ModelCatalogueFilter.Moderation = moderation;
                    TreeView.ReloadTreeView(true);
                }

                bool realtime = EditorGUILayout.ToggleLeft("Realtime", ModelCatalogueFilter.Realtime, GUILayout.MinWidth(kMinWidth), GUILayout.ExpandWidth(true));
                if (realtime != ModelCatalogueFilter.Realtime)
                {
                    ModelCatalogueFilter.Realtime = realtime;
                    TreeView.ReloadTreeView(true);
                }
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 0f;

            GUILayout.BeginVertical();
            {
                if (AIDevKitGUI.TreeView.FilterButton())
                {
                    ModelCatalogueFilterWindow.ShowWindow(this);
                }

                if (AIDevKitGUI.TreeView.FilterResetButton())
                {
                    ResetFilters();
                }
            }
            GUILayout.EndVertical();

            if (AIDevKitGUI.TreeView.GenerateSnippetsButton())
            {
                GenerateSnippets();
            }

            if (AIDevKitGUI.TreeView.UpdateCatalogueButton())
            {
                if (EditorUtility.DisplayDialog("Update Model Catalogue", "This may take a while, do you want to continue?", "Yes", "No"))
                {
                    ModelCatalogue.Instance.UpdateCatalogue((success) =>
                    {
                        if (success)
                        {
                            TreeView.ReloadTreeView(true);
                        }
                    });
                }
            }
        }

        private async void GenerateSnippets()
        {
            if (EditorUtility.DisplayDialog("Generate Snippets", "This may take a while, do you want to continue?", "Yes", "No"))
            {
                await ModelSnippetGenerator.GenerateAllAsync();
                AssetDatabase.Refresh();  // refresh editor
            }
        }
    }
}