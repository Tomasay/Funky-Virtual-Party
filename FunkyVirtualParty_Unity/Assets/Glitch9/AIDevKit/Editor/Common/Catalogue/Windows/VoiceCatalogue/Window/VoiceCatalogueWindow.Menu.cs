using Glitch9.Editor.IMGUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Glitch9.Internal;
using Cysharp.Threading.Tasks;
using Glitch9.Editor;

namespace Glitch9.AIDevKit.Editor
{
    public partial class VoiceCatalogueWindow
    {
        protected override IEnumerable<ITreeViewMenuEntry> CreateMenuEntries()
        {
            yield return new TreeViewMenuDropdown("File", DrawFileMenu);
            yield return new TreeViewMenuDropdown("Help", DrawHelpMenu);
            yield return new TreeViewMenuDropdown("Preferences", (_) => AIDevKitEditor.ShowPreferencesWindow());
            if (!AIDevKitConfig.IsPro) yield return new TreeViewMenuDropdown("Upgrade to Pro", (_) => AIDevKitEditor.OpenProURL());
            yield return new TreeViewMenuSearchField();
        }

        private async void UpdateCatalogue()
        {
            await VoiceCatalogue.Instance.UpdateCatalogueAsync();
            TreeView.ReloadTreeView(true, true);
        }

        private async void UpdateElevenLabsCustomVoicesAsync()
        {
            await VoiceCatalogue.Instance.UpdateElevenLabsCustomVoicesAsync();
            TreeView.ReloadTreeView(true, true);
        }

        private void DrawFileMenu(Rect rect)
        {
            GenericMenu menu = new();

            menu.AddItem(new GUIContent("Update Voice Catalogue"), false, UpdateCatalogue);
            menu.AddItem(new GUIContent("Update ElevenLabs Voice Library"), false, UpdateElevenLabsCustomVoicesAsync);

            menu.AddSeparator(string.Empty);

            menu.AddItem(new GUIContent("Generate Snippets/OpenAI Voices"), false, () => VoiceSnippetGenerator.Generate(Api.OpenAI));
            menu.AddItem(new GUIContent("Generate Snippets/ElevenLabs Voices"), false, () => VoiceSnippetGenerator.Generate(Api.ElevenLabs));
            menu.AddItem(new GUIContent("Generate Snippets (All)"), false, () => VoiceSnippetGenerator.GenerateAll());

            menu.AddSeparator(string.Empty);

            menu.AddItem(new GUIContent("Scriptable Objects/Reload Assets"), false, () => ScriptableObjectUtil.FindAssets(VoiceLibrary.DB));
            menu.AddItem(new GUIContent("Scriptable Objects/Update Assets"), false, UpdateAssets);
            menu.AddItem(new GUIContent("Scriptable Objects/Remove Invalid Assets"), false, VoiceLibrary.RemoveInvalidEntries);

            menu.DropDown(rect);
        }


        private void DrawHelpMenu(Rect rect)
        {
            GenericMenu menu = new();

            menu.AddItem(GUIContents.OnlineDocument, false, () => Application.OpenURL(AIDevKitEditor.OnlineDocUrl));
            menu.AddItem(GUIContents.JoinDiscord, false, () => Application.OpenURL(EditorConfig.DiscordUrl));

            // https://www.openai.fm/ (An interactive demo for developers to try the new text-to-speech model in the OpenAI API.)
            menu.AddItem("Official OpenAI TTS Demo", () => Application.OpenURL("https://www.openai.fm/"));

            menu.AddSeparator(string.Empty);

            menu.AddItem("Remove Duplicates from Catalogue", () =>
            {
                VoiceCatalogue.Instance.RemoveDuplicates();
                TreeView.ReloadTreeView(true, true);
            });

            menu.AddItem("Run Startup Catalogue Update", () => VoiceCatalogue.Instance.CheckForUpdatesAsync(true, true).Forget());

            menu.AddSeparator(string.Empty);

            menu.AddItem("Force Update Voice Dropdown", VoicePopupGUI.ForceUpdateCache);
            menu.AddItem("Open Voice Preview Folder", () =>
            {
                string path = AIDevKitEditorPath.GetVoiceSampleBasePath() + "/";

                if (!string.IsNullOrEmpty(path))
                {
                    AIDevKitDebug.Mark("Voice Preview Folder: " + path);
                    EditorUtility.RevealInFinder(path);
                }
                else
                {
                    Debug.LogWarning("Voice preview folder path is empty.");
                }
            });

            menu.DropDown(rect);
        }
        private void UpdateAssets()
        {
            List<Voice> inMyLibrary = VoiceLibrary.ToList();
            if (inMyLibrary.IsNullOrEmpty())
            {
                Debug.LogWarning("No voice Assets found in the library.");
                return;
            }

            foreach (Voice voice in inMyLibrary)
            {
                if (voice == null)
                {
                    Debug.LogWarning("Voice is null. Skipping.");
                    continue;
                }

                VoiceCatalogueEntry serverData = VoiceCatalogue.Instance.GetEntry(voice.Id);
                if (serverData == null)
                {
                    Debug.LogWarning($"Voice {voice.Id} not found in the catalogue. Skipping.");
                    continue;
                }

                voice.UpdateData(
                    api: serverData.Api,
                    id: serverData.Id,
                    name: serverData.Name,
                    gender: serverData.Gender,
                    age: serverData.Age,
                    language: serverData.Language
                );

                voice.SaveAsset();

                Debug.Log($"Updated {voice.Id} voice asset.");
            }
        }
    }
}