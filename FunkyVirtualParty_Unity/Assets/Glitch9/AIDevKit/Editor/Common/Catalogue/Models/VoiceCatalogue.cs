using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.GENTasks;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;
using Glitch9.AIDevKit.Google;

namespace Glitch9.AIDevKit.Editor
{
    internal class VoiceCatalogue : Catalogue<VoiceCatalogue, VoiceCatalogueEntry, IVoiceData>
    {
        protected override string GetCataloguePath() => AIDevKitEditorPath.GetVoiceCataloguePath().FixDoubleAssets();
        internal override VoiceCatalogueEntry CreateEntry(IVoiceData data) => VoiceCatalogueEntry.Create(data);
        protected override List<VoiceCatalogueEntry> GetMissingAPIEntries() => null;
        protected override void ShowCatalogueUpdateWindow(List<IVoiceData> newEntries, List<VoiceCatalogueEntry> deprecatedEntries)
            => VoiceCatalogueUpdateWindow.ShowWindow(newEntries, deprecatedEntries);

        protected override async UniTask<List<IVoiceData>> RetrieveAllEntriesAsync(bool isAutoUpdate = false)
        {
            List<IVoiceData> allVoices = new();
            bool hasAnyApiKey = false;

            try
            {
                if (OpenAISettings.Instance.HasApiKey())
                {
                    hasAnyApiKey = true;

                    // Start Editor Progress bar
                    EditorUtility.DisplayProgressBar("Retrieving Voices", "Retrieving voices from OpenAI...", 0f);
                    AIDevKitDebug.Mark("Retrieving voices from OpenAI...");

                    //var res = await OpenAI.OpenAI.DefaultInstance.Voices.List();
                    //if (res != null && res.Data != null) allVoices.AddRange(res.Data); 
                    allVoices.AddRange(Metadata_OpenAI_Voices.List); // Hard-code OpenAI voices for now, as the API doesn't exist
                }

                if (GenerativeAISettings.Instance.HasApiKey())
                {
                    hasAnyApiKey = true;

                    EditorUtility.DisplayProgressBar("Retrieving Voices", "Retrieving voices from Google...", 0.25f);
                    AIDevKitDebug.Mark("Retrieving voices from Google...");

                    allVoices.AddRange(Metadata_Google_Voices.List); // Hard-code Google voices for now, as the API doesn't exist 
                }

                if (ElevenLabsSettings.Instance.HasApiKey())
                {
                    hasAnyApiKey = true;

                    EditorUtility.DisplayProgressBar("Retrieving Voices", "Retrieving voices from ElevenLabs...", 0.5f);

                    // Max list size is 100, so we need to paginate
                    //var res = await ElevenLabsClient.DefaultInstance.Voices.List(100);
                    var res = await GENTaskManager.ListVoicesAsync(Api.ElevenLabs, new ElevenLabsQuery(1, 100));
                    if (res != null && res.Data != null) allVoices.AddRange(res.Data);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if (!hasAnyApiKey)
            {
                ShowDialog.Error("No API keys found for any voice providers. Please check your preferences. (Edit > Preferences > AIDevKit)");
                return allVoices;
            }

            return allVoices;
        }

        private async UniTask<List<IVoiceData>> RetrieveAllElevenLabsCustomVoicesAsync()
        {
            List<IVoiceData> allVoices = new();

            try
            {
                if (ElevenLabsSettings.Instance.HasApiKey())
                {
                    // Start Editor Progress bar
                    EditorUtility.DisplayProgressBar("Retrieving Voices", "Retrieving voice library from ElevenLabs...", 0f);

                    int currentPage = 1;
                    var cRes = await GENTaskManager.ListCustomVoicesAsync(Api.ElevenLabs, new ElevenLabsQuery(currentPage, 100));
                    if (cRes != null)
                    {
                        if (cRes.Data != null) allVoices.AddRange(cRes.Data);
                        while (cRes.HasMore)
                        {
                            currentPage++;
                            EditorUtility.DisplayProgressBar("Retrieving Voices", $"Retrieving page {currentPage} of shared voices...", currentPage / 20f);
                            //Debug.Log($"Retrieving page {cReq.Page} of shared voices...");
                            //cRes = await ElevenLabsClient.DefaultInstance.VoiceLibrary.List(cReq);
                            //if (cRes != null && cRes.Data != null) allVoices.AddRange(cRes.Data);
                            cRes = await GENTaskManager.ListCustomVoicesAsync(Api.ElevenLabs, new ElevenLabsQuery(currentPage, 100));
                            if (cRes != null && cRes.Data != null) allVoices.AddRange(cRes.Data);
                        }
                    }
                }
                else
                {
                    ShowDialog.Error("ElevenLabs API key not found. Please check your preferences. (Edit > Preferences > AIDevKit)");
                    return allVoices;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }


            return allVoices;
        }

        internal async UniTask UpdateElevenLabsCustomVoicesAsync()
        {
            Debug.Log("Updating ElevenLabs Custom Voices...");

            List<IVoiceData> allVoices = await RetrieveAllElevenLabsCustomVoicesAsync();

            if (allVoices.Count == 0)
            {
                Debug.LogError("Critical Error: No voices found. Please check your API keys and settings.");
                return;
            }

            Debug.Log($"Found {allVoices.Count} voices.");

            AddOrUpdateEntries(allVoices);
        }
    }
}