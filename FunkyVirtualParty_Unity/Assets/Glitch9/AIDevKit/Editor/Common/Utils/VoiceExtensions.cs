using System.IO;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.GENTasks;
using Glitch9.Editor;
using Glitch9.IO.Files;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal static class VoiceExtensions
    {
        internal static async UniTask<bool> PlayPreviewAsync(this Voice voice)
        {
            if (voice == null)
            {
                Debug.LogWarning("Null voice provided for preview playback.");
                return false;
            }

            string id = voice.Id;

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("Voice ID is null or empty. Cannot play preview.");
                return false;
            }

            VoiceCatalogueEntry item = VoiceCatalogue.Instance.GetEntry(id);

            if (item == null)
            {
                Debug.LogWarning($"VoiceCatalogueEntry not found for ID: {id}");
                return false;
            }

            return await item.PlayPreviewAsync(validationOnly: false);
        }

        internal static async UniTask<bool> PlayPreviewAsync(this VoiceCatalogueEntry item, bool validationOnly = false)
        {
            string url = item.PreviewUrl;
            AudioClip clip;

            if (!string.IsNullOrWhiteSpace(url))
            {
                clip = await AudioClipLoader.LoadFullPathOrUrlAsync(url, ignoreLogs: true);
                if (clip != null)
                {
                    if (!validationOnly) EditorAudioPlayer.Play(clip);
                    return true;
                }
            }

            string absolutePath = AIDevKitEditorPath.GetVoiceSampleFullPath(item.Api, item.Id);

            // if (string.IsNullOrWhiteSpace(absolutePath))
            //     absolutePath = AIDevKitEditorPath.GetVoiceSampleFullPath(item.Api, item.Id);

            if (!string.IsNullOrWhiteSpace(absolutePath) && !File.Exists(absolutePath))
            {
                // Debug.LogWarning($"Preview file not found at path: {absolutePath}. Attempting to fix the file extension...");
                absolutePath = Path.ChangeExtension(absolutePath, ".wav");
            }

            clip = await AudioClipLoader.LoadFullPathOrUrlAsync(absolutePath, ignoreLogs: true);

            if (clip != null)
            {
                if (!validationOnly) EditorAudioPlayer.Play(clip);
                return true;
            }

            Debug.LogWarning($"No preview clip found at path: {absolutePath}. Attempting to create a new preview clip...");

            try
            {
                // start progress bar
                // bool canceled = EditorUtility.DisplayCancelableProgressBar("Creating Voice Preview", "Preview file is not prepared for this voice. Creating a new preview clip...", 0.5f); 
                const string previewPrompt = "Hello, I hope you are having a great day! This is a test voice preview.";
                string absolutePreviewPath = AIDevKitEditorPath.GetVoiceSampleFullPath(item.Api, item.Id);

                AudioClip previewClip = await previewPrompt.GENSpeech()
                    .SetVoice(item.Api, item.Id)
                    .SetEncoding(CoreLib.IO.Audio.AudioEncoding.MP3)
                    .SetOutputPath(absolutePreviewPath)
                    .ExecuteAsync();

                if (previewClip == null)
                    throw new EmptyResponseException(RequestType.Speech);

                Debug.Log($"Preview clip created successfully at: {absolutePreviewPath}");
                AssetDatabase.Refresh();
                EditorAudioPlayer.Play(previewClip);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error creating preview clip: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
            finally
            {
                // clear progress bar
                //EditorUtility.ClearProgressBar();
            }
        }
    }
}