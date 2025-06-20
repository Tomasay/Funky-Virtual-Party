using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.Files
{
    public static class Texture2DLoader
    {
        internal static async UniTask<Texture2D> LoadAsync(string fullPath, string url)
        {
            Texture2D texture = null;

            if (!string.IsNullOrEmpty(fullPath))
            {
                texture = await LoadAsync(fullPath, UnityPathType.Absolute);
            }

            if (texture == null && !string.IsNullOrEmpty(url))
            {
                texture = await LoadAsync(url, UnityPathType.Url);
            }

            return texture;
        }

        public static async UniTask<Texture2D> LoadAsync(string filePath, UnityPathType pathType = UnityPathType.Assets)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            if (pathType == UnityPathType.Resources)
            {
                ResourceRequest request = Resources.LoadAsync<Texture2D>(filePath);
                await request.ToUniTask();
                if (request.asset is Texture2D texture) return texture;
                return null; // 혹은 오류 처리
            }

            if (pathType == UnityPathType.Url)
            {
                return await LoadUrlAsync(filePath);
            }

            if (pathType != UnityPathType.Absolute)
            {
                filePath = UnityPathUtil.ConvertPath(filePath, UnityPathType.Absolute);
            }

            try
            {
                // check if the file exists
                if (!File.Exists(filePath)) throw new FileNotFoundException($"Texture file not found at {filePath}");
                return await LoadFullPathAsync(filePath);
            }
            catch (Exception e)
            {
                //Debug.LogWarning($"Failed to load texture from {filePath}. Error: {e.Message}");
                Debug.LogWarning(e);
                return null;
            }
        }

        private static async UniTask<Texture2D> LoadFullPathAsync(string fullPath)
        {
            byte[] fileData = await File.ReadAllBytesAsync(fullPath);
            Texture2D texture = new(2, 2);
            texture.LoadImage(fileData);
            return texture;
        }

        private static async UniTask<Texture2D> LoadUrlAsync(string url)
        {
            // UnityWebRequest를 사용하여 텍스처를 비동기적으로 로드합니다.
            using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            // SendWebRequest 대신 await를 사용합니다.
            await www.SendWebRequest().WithCancellation(CancellationToken.None);

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new Exception($"Failed to load texture from {url}. Error: {www.error}");
            }
            else
            {
                // 성공적으로 로드된 경우, 다운로드된 텍스처를 반환합니다.
                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }
}