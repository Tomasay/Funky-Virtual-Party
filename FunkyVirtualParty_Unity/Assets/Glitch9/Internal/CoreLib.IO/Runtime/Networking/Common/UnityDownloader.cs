using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.Networking
{
    public class UnityDownloader
    {
        private const int MAX_DL_FAIL_COUNT = 3;

        public static async UniTask<bool> DownloadFileAsync(string uri, string dlLocalPath)
        {
            if (!TryCreateUri(uri, out Uri result)) return false;
            return await DownloadWithRetryAsync(result, dlLocalPath);
        }

        private static async UniTask<bool> DownloadWithRetryAsync(Uri uri, string outputPath)
        {
            outputPath = outputPath.Replace("//", "/"); // Idk why but I make this mistake a lot.

            string dir = Path.GetDirectoryName(outputPath);
            LogService.Info($"Download Dir: {dir} | Download Path: {outputPath}");

            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                LogService.Info($"Creating directory: {dir}");
                Directory.CreateDirectory(dir);
            }

            int attempt = 0;
            while (attempt < MAX_DL_FAIL_COUNT)
            {
                try
                {
                    if (await TryDownloadAsync(uri, outputPath)) return true;
                }
                catch (Exception e)
                {
                    LogService.Error($"Failed to download the file from <color=magenta>{uri}</color>.\nError: {e.Message}");
                }

                attempt++;
            }

            DisplayEditorDialogue($"Failed to download the file after {MAX_DL_FAIL_COUNT} attempts from <color=magenta>{uri}</color>.");
            return false;
        }

        private static async UniTask<bool> TryDownloadAsync(Uri uri, string dlLocalPath)
        {
            using UnityWebRequest www = UnityWebRequest.Get(uri);

            // check if the file already exists and add suffix if it does
            if (File.Exists(dlLocalPath))
            {
                string ext = Path.GetExtension(dlLocalPath);
                string fileName = Path.GetFileNameWithoutExtension(dlLocalPath);
                string dir = Path.GetDirectoryName(dlLocalPath);
                if (string.IsNullOrEmpty(dir)) return false; // This should never happen but just in case :

                string[] files = Directory.GetFiles(dir, $"{fileName}*{ext}");
                if (files.Length > 0)
                {
                    int max = files.Select(f => f.Split('_').Last()).Select(f => int.TryParse(f, out int i) ? i : 0).Max();
                    dlLocalPath = Path.Combine(dir, $"{fileName}_{max + 1}{ext}");
                }
            }

            www.downloadHandler = new DownloadHandlerFile(dlLocalPath);

            // Convert UnityWebRequest's coroutine pattern into a Task
            TaskCompletionSource<bool> completionSource = new();
            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            operation.completed += _ =>
            {
                DisplayEditorDialogue($"File downloaded to <color=magenta>{dlLocalPath}</color>.");
                completionSource.SetResult(true);
            };

            return await completionSource.Task;
        }

        private static bool TryCreateUri(string uri, out Uri result)
        {
            if (!Uri.TryCreate(uri, UriKind.Absolute, out result))
            {
                DisplayEditorDialogue($"Invalid Uri: {uri}", true);
                return false;
            }

            return true;
        }

        private static void DisplayEditorDialogue(string v2, bool isError = false)
        {
            if (isError) LogService.Error(v2);
            //            else LogService.Info(v2);
        }

        #region Audio Download

        public static async UniTask<AudioClip> DownloadAudioClipAsync(string uri, string dlLocalPath, string extension)
        {
            if (!TryCreateUri(uri, out Uri result)) return null;

            AudioType audioType = AudioTypeUtil.ParseFromExtension(extension);

            if (await DownloadWithRetryAsync(result, dlLocalPath))
            {
                return await LoadAudioClipAsync(dlLocalPath, audioType);
            }
            return null;
        }

        public static async UniTask<AudioClip> DownloadLiveAudioClipAsync(string httpUrl, string extension)
        {
            if (!TryCreateUri(httpUrl, out Uri result)) return null;

            AudioType audioType = AudioTypeUtil.ParseFromExtension(extension);

            StringBuilder outputPathSb = new();
            outputPathSb.Append(Application.persistentDataPath);
            outputPathSb.Append("/temp_audio.");
            outputPathSb.Append(extension);
            string outputPath = outputPathSb.ToString();

            if (await DownloadWithRetryAsync(result, outputPath))
            {
                return await LoadAudioClipAsync(outputPath, audioType);
            }
            return null;
        }

        private static async UniTask<AudioClip> LoadAudioClipAsync(string dlLocalPath, AudioType audioType)
        {
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + dlLocalPath, audioType);
            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await UniTask.Yield();
            }

            if (www.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                DisplayEditorDialogue("Download failed: " + dlLocalPath + "\nError:" + www.error);
                return null;
            }

            DisplayEditorDialogue($"File loaded from <color=magenta>{dlLocalPath}</color>.");
            return DownloadHandlerAudioClip.GetContent(www);
        }

        #endregion

        #region Image Download

        public static async UniTask<Texture2D> DownloadTextureAsync(string uri, string dlLocalPath)
        {
            if (!TryCreateUri(uri, out Uri result)) return null;

            if (dlLocalPath.EndsWith(".json")) dlLocalPath = dlLocalPath.Replace(".json", ".png");

            if (await DownloadWithRetryAsync(result, dlLocalPath))
            {
                return await LoadTextureAsync(dlLocalPath);
            }
            return null;
        }

        private static async UniTask<Texture2D> LoadTextureAsync(string dlLocalPath)
        {
            byte[] fileData = await File.ReadAllBytesAsync(dlLocalPath);
            Texture2D texture = new(2, 2);
            texture.LoadImage(fileData);

            DisplayEditorDialogue($"File loaded from <color=magenta>{dlLocalPath}</color>.");
            return texture;
        }

        #endregion
    }
}