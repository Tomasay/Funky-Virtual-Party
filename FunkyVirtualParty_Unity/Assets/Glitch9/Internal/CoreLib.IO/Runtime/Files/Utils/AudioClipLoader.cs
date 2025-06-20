using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.Files
{
    public static class AudioClipLoader
    {
        internal static async UniTask<AudioClip> LoadAsync(string fullPath, string url)
        {
            AudioClip clip = null;

            if (!string.IsNullOrEmpty(fullPath))
            {
                clip = await LoadAsync(fullPath, UnityPathType.Absolute);
            }

            if (clip == null && !string.IsNullOrEmpty(url))
            {
                clip = await LoadAsync(url, UnityPathType.Url);
            }

            return clip;
        }

        public static async UniTask<AudioClip> LoadAsync(string filePath, UnityPathType? pathType = null)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            pathType ??= UnityPathUtil.ResolveType(filePath);

            if (pathType == UnityPathType.Unknown)
            {
                Debug.LogError($"Unknown path type for file path: {filePath}");
                return null;
            }

            if (pathType == UnityPathType.Resources)
            {
                ResourceRequest request = Resources.LoadAsync<AudioClip>(filePath);
                await request.ToUniTask();
                if (request.asset is AudioClip audioClip) return audioClip;
                return null; // 혹은 오류 처리
            }

            if (pathType == UnityPathType.Url)
            {
                return await LoadFullPathOrUrlAsync(filePath);
            }

            filePath = filePath.ToFullPath();
            return await LoadFullPathOrUrlAsync(filePath);
        }

        public static async UniTask<AudioClip> LoadFullPathOrUrlAsync(string fullPathOrUrl, AudioType? audioType = null, bool ignoreLogs = false)
        {
#if UNITY_EDITOR
            if (fullPathOrUrl.Contains(Application.dataPath))
            {
                string relativePath = fullPathOrUrl.ToRelativePath();
                AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(relativePath);
                if (clip != null) return clip;

                if (!ignoreLogs) Debug.LogWarning($"[AudioClipLoader] AudioClip not found in project at path: {relativePath}");
                return null;
            }
#endif

            if (fullPathOrUrl.Contains(Application.dataPath) && fullPathOrUrl.Contains("Resources"))
            {
                string resourcesPath = UnityPathUtil.ExtractResourcesPath(fullPathOrUrl);
                AudioClip clip = Resources.Load<AudioClip>(resourcesPath);
                if (clip != null) return clip;

                if (!ignoreLogs) Debug.LogWarning($"[AudioClipLoader] AudioClip not found in Resources at path: {resourcesPath}");
                return null;
            }

            audioType ??= AudioTypeUtil.ParseFromPath(fullPathOrUrl);
            string uri = fullPathOrUrl.ToFileUri();

            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, audioType.Value);

            try
            {
                await www.SendWebRequest().WithCancellation(CancellationToken.None);

                if (www.result != UnityWebRequest.Result.Success)
                {
                    if (!ignoreLogs) Debug.LogWarning($"[AudioClipLoader] Failed to load audio from URL: {uri} ({www.responseCode})");
                    return null;
                }

                return DownloadHandlerAudioClip.GetContent(www);
            }
            catch (Exception ex)
            {
                if (!ignoreLogs) Debug.LogWarning($"[AudioClipLoader] Exception while loading audio: {uri}\n{ex.Message}");
                return null;
            }
        }

        // WebGL 이외 플랫폼용 MPEG 디코딩 처리 (임시파일로 저장 후 AudioClip으로 변환)
        // outputPath가 존재한다면, 파일을 저장하고 싶다는 의미임으로 temp폴더가 아니라 지정된 outputPath에 저장하도록 한다.
        internal static async UniTask<File<AudioClip>> NonWebGLDecodeAsyncINTERNAL(byte[] audioBytes, string outputPath, AudioType audioType)
        {
            //Debug.Log("Trying to decode MPEG audio file from byte array.");  
            outputPath = RESTApiUtil.ResolveOutputPath(outputPath, audioType);

            // MP3 파일로 저장
            await AudioFileWriter.WriteFileAsync(audioBytes, outputPath);

            // UnityWebRequest로 AudioClip 로드 
            AudioClip clip = await LoadFullPathOrUrlAsync(outputPath, audioType);
            if (clip == null) throw new Exception("Failed to load audio clip.");

            return new File<AudioClip>(clip, outputPath);
        }
    }
}