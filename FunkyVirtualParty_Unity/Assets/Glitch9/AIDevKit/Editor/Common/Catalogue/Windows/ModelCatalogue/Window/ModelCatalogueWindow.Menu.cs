using Cysharp.Threading.Tasks;
using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using Glitch9.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public partial class ModelCatalogueWindow
    {
        protected override IEnumerable<ITreeViewMenuEntry> CreateMenuEntries()
        {
            yield return new TreeViewMenuDropdown("File", DrawFileMenu);
            yield return new TreeViewMenuDropdown("Help", DrawHelpMenu);
            yield return new TreeViewMenuDropdown("Preferences", (_) => AIDevKitEditor.ShowPreferencesWindow());
            if (!AIDevKitConfig.IsPro) yield return new TreeViewMenuDropdown("Upgrade to Pro", (_) => AIDevKitEditor.OpenProURL());
            yield return new TreeViewMenuSearchField();
        }

        private void DrawFileMenu(Rect rect)
        {
            GenericMenu menu = new();

            menu.AddItem("Update Model Catalogue", UpdateCatalogue);
            menu.AddItem("Update Ollama Models", UpdateOllamaModels);
            menu.AddItem("Remove Deprecated Models from Catalogue", () =>
            {
                ModelCatalogue.Instance.RemoveDeprecatedEntries();
                TreeView.ReloadTreeView(true, true);
            });

            menu.AddSeparator(string.Empty);

            menu.AddItem($"{GUILabels.GenerateSnippets}/OpenAI Models", () => ModelSnippetGenerator.Generate(Api.OpenAI));
            menu.AddItem($"{GUILabels.GenerateSnippets}/Google Models", () => ModelSnippetGenerator.Generate(Api.Google));
            menu.AddItem($"{GUILabels.GenerateSnippets}/ElevenLabs Models", () => ModelSnippetGenerator.Generate(Api.ElevenLabs));
            menu.AddItem($"{GUILabels.GenerateSnippets}/Ollama Models", () => ModelSnippetGenerator.Generate(Api.Ollama));
            menu.AddItem($"{GUILabels.GenerateSnippets}/OpenRouter Models", () => ModelSnippetGenerator.Generate(Api.OpenRouter));
            menu.AddItem($"{GUILabels.GenerateSnippets} (All)", () => ModelSnippetGenerator.GenerateAllAsync().Forget());

            menu.AddSeparator(string.Empty);

            menu.AddItem($"{GUILabels.ScriptableObjects}/Reload Assets", () => ScriptableObjectUtil.FindAssets(ModelLibrary.DB));
            menu.AddItem($"{GUILabels.ScriptableObjects}/Update Assets", UpdateAssets);
            menu.AddItem($"{GUILabels.ScriptableObjects}/Remove Invalid Assets", ModelLibrary.RemoveInvalidEntries);

            menu.AddSeparator(string.Empty);

            menu.AddItem($"{GUILabels.Export}/Export Model Ids (.txt)", ExportModelIds);

            menu.DropDown(rect);
        }

        private void DrawHelpMenu(Rect rect)
        {
            GenericMenu menu = new();

            menu.AddItem(GUIContents.OnlineDocument, false, () => Application.OpenURL(AIDevKitEditor.OnlineDocUrl));
            menu.AddItem(GUIContents.JoinDiscord, false, () => Application.OpenURL(EditorConfig.DiscordUrl));

            menu.AddSeparator(string.Empty);

            menu.AddItem("Remove Duplicates from Catalogue", () =>
            {
                ModelCatalogue.Instance.RemoveDuplicates();
                TreeView.ReloadTreeView(true, true);
            });

            menu.AddItem("Run Startup Catalogue Update", () =>
            {
                ModelCatalogue.Instance.CheckForUpdatesAsync(true, true).Forget();
            });

            menu.AddSeparator(string.Empty);

            menu.AddItem("Force Update Model Dropdown", () =>
            {
                ModelPopupGUI.ForceUpdateCache();
            });

            menu.DropDown(rect);
        }

        private async void UpdateCatalogue()
        {
            await ModelCatalogue.Instance.UpdateCatalogueAsync();
            TreeView.ReloadTreeView(true, true);
        }

        private async void UpdateOllamaModels()
        {
            await ModelCatalogue.Instance.UpdateOllamaModelsAsync();
            TreeView.ReloadTreeView(true, true);
        }

        private void UpdateAssets()
        {
            List<Model> inMyLibrary = ModelLibrary.ToList();
            if (inMyLibrary.IsNullOrEmpty())
            {
                Debug.LogWarning("No Model Assets found in the library.");
                return;
            }

            foreach (Model model in inMyLibrary)
            {
                if (model == null)
                {
                    Debug.LogWarning("Model is null. Skipping.");
                    continue;
                }

                ModelCatalogueEntry serverData = ModelCatalogue.Instance.GetEntry(model.Id);
                if (serverData == null)
                {
                    Debug.LogWarning($"Model {model.Id} not found in the catalogue. Skipping.");
                    continue;
                }

                model.UpdateData(
                    api: serverData.Api,
                    id: serverData.Id,
                    name: serverData.Name,
                    capability: serverData.Feature,
                    family: serverData.Family,
                    inputModality: serverData.InputModality,
                    outputModality: serverData.OutputModality,
                    legacy: serverData.IsLegacy,
                    familyVersion: serverData.FamilyVersion,
                    inputTokenLimit: serverData.InputTokenLimit,
                    outputTokenLimit: serverData.OutputTokenLimit,
                    modelVersion: serverData.ModelVersion,
                    fineTuned: serverData.IsFineTuned,
                    prices: serverData.GetPrices()
                );

                model.SaveAsset();

                Debug.Log($"Updated {model.Id} model asset.");
            }
        }

        private void ExportModelIds()
        {
            Dictionary<Api, List<string>> modelIds = ModelCatalogue.Instance.GetAllEntryIds();

            /* 형식
            --- OpenAI ---
            model_id_1
            model_id_2
            --- Google ---
            model_id_3
            model_id_4
            */

            string filePath = EditorUtility.SaveFilePanel("Export Model IDs", "", "model_ids.txt", "txt");
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("Export cancelled or invalid file path.");
                return;
            }

            // sort the model IDs by alphabetical order
            foreach (var apiEntry in modelIds)
            {
                apiEntry.Value.Sort();
            }

            using (System.IO.StreamWriter writer = new(filePath))
            {
                foreach (var apiEntry in modelIds)
                {
                    writer.WriteLine($"--- {apiEntry.Key} ---");
                    foreach (string modelId in apiEntry.Value)
                    {
                        writer.WriteLine(modelId);
                    }
                    writer.WriteLine(); // Add a blank line between APIs
                }
            }

            Debug.Log($"Model IDs exported to {filePath}");
            EditorUtility.RevealInFinder(filePath);
        }
    }
}