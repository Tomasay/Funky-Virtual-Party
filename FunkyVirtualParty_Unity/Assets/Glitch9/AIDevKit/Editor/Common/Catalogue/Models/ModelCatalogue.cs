using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.GENTasks;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.Ollama;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.AIDevKit.OpenRouter;
using Glitch9.Editor;
using Glitch9.IO.Networking.RESTApi;
using UnityEditor;
using System;

namespace Glitch9.AIDevKit.Editor
{
    internal class ModelCatalogue : Catalogue<ModelCatalogue, ModelCatalogueEntry, IModelData>
    {
        protected override string GetCataloguePath() => AIDevKitEditorPath.GetModelCataloguePath().FixDoubleAssets();
        internal override ModelCatalogueEntry CreateEntry(IModelData data) => ModelCatalogueEntry.Create(data);
        protected override List<ModelCatalogueEntry> GetMissingAPIEntries() => Metadata_ElevenLabs_Models.GetMissingModels();
        protected override void ShowCatalogueUpdateWindow(List<IModelData> newEntries, List<ModelCatalogueEntry> deprecatedEntries)
        => ModelCatalogueUpdateWindow.ShowWindow(newEntries, deprecatedEntries);

        public async UniTask UpdateOllamaModelsAsync()
        {
            bool hasOllama = false;
            List<IModelData> allModels = new();

            try
            {
                hasOllama = await OllamaSettings.CheckConnectionAsync();
            }
            catch
            {
                EditorUtility.ClearProgressBar();
            }

            if (hasOllama)
            {
                var res = await GENTaskManager.ListModelsAsync(Api.Ollama);
                if (res?.Data != null) allModels.AddRange(res.Data);
            }

            if (allModels.IsNullOrEmpty())
            {
                ShowDialog.Error("No models found in Ollama. Please ensure Ollama is running and has models available.");
            }
            else
            {
                AddOrUpdateEntries(allModels);
            }
        }

        protected override async UniTask<List<IModelData>> RetrieveAllEntriesAsync(bool isAutoUpdate = false)
        {
            if (isAutoUpdate && !AIDevKitEditorSettings.AutoUpdateModels)
                return null;

            List<IModelData> allModels = new();
            bool hasAnyApiKey = false;

            try
            {
                if (OpenAISettings.Instance.HasApiKey())
                {
                    hasAnyApiKey = true;
                    await ShowProgressAsync("OpenAI", 0f, async () =>
                    {
                        var res = await GENTaskManager.ListModelsAsync(Api.OpenAI, new CursorQuery(100));
                        if (res?.Data != null) allModels.AddRange(res.Data);
                    });
                }

                if (GenerativeAISettings.Instance.HasApiKey())
                {
                    hasAnyApiKey = true;
                    await ShowProgressAsync("Google", 0.2f, async () =>
                    {
                        var res = await GENTaskManager.ListModelsAsync(Api.Google, new TokenQuery(100));
                        if (res?.Data != null) allModels.AddRange(res.Data);
                    });
                }

                if (ElevenLabsSettings.Instance.HasApiKey())
                {
                    hasAnyApiKey = true;
                    await ShowProgressAsync("ElevenLabs", 0.4f, async () =>
                    {
                        var res = await GENTaskManager.ListModelsAsync(Api.ElevenLabs, new ElevenLabsQuery(1, 100));
                        if (res?.Data != null) allModels.AddRange(res.Data);
                    });
                }

                if (OllamaSettings.IsEnabled)
                {
                    bool hasOllama = false;

                    try
                    {
                        hasOllama = await OllamaSettings.CheckConnectionAsync();
                    }
                    catch
                    {
                        EditorUtility.ClearProgressBar();
                    }

                    if (hasOllama)
                    {
                        hasAnyApiKey = true;
                        await ShowProgressAsync("Ollama", 0.6f, async () =>
                        {
                            var res = await GENTaskManager.ListModelsAsync(Api.Ollama);
                            if (res?.Data != null) allModels.AddRange(res.Data);
                        });
                    }
                }

                if (OpenRouterSettings.Instance.HasApiKey())
                {
                    hasAnyApiKey = true;
                    await ShowProgressAsync("OpenRouter", 0.8f, async () =>
                    {
                        var res = await GENTaskManager.ListModelsAsync(Api.OpenRouter);
                        if (res?.Data != null) allModels.AddRange(res.Data);
                    });
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if (!hasAnyApiKey)
            {
                ShowDialog.Error("No API keys found for any model providers. Please check your preferences. (Edit > Preferences > AIDevKit)");
                return allModels;
            }

            return allModels;

            static async UniTask ShowProgressAsync(string label, float progress, Func<UniTask> task)
            {
                try
                {
                    bool isCanceled = EditorUtility.DisplayCancelableProgressBar("Retrieving Models", $"Retrieving models from {label}...", progress);
                    if (isCanceled) throw new OperationCanceledException("Model retrieval was canceled by the user.");
                    await task();
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }
        }
    }
}