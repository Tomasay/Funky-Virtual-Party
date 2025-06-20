#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Glitch9.CoreLib.IO.Audio
{
    /// <summary>
    /// MP3(MPEG)의 경우 Unity에서 한번에 디코딩할 수 없으므로, 파일로 저장한 후에 AudioClip으로 변환합니다.
    /// <para>WebGL에서는 MPEG을 지원하지 않으므로, WebGL에서 사용할 수 있는 대체 스트리밍 방법을 구현해야 합니다.</para>
    /// </summary>
    public static class WebGLAudioProcessor
    {

        private static UniTaskCompletionSource<File<AudioClip>> _webGLResult;

        [DllImport("__Internal")]
        private static extern void DecodeMP3FromBase64(string base64, string target, string method);

        private const string kReceiverObjectName = "[MPEGDecoderReceiver]";
        private const string kReceiverMethodName = "OnPCMDecoded";

        public static async UniTask<File<AudioClip>> DecodeAsync(byte[] binaryData)
        {
            // WebGLReceiver GameObject가 없으면 생성
            EnsureReceiverExists();

            string base64 = Convert.ToBase64String(binaryData);
            _webGLResult = new UniTaskCompletionSource<File<AudioClip>>();

            // JavaScript 함수 호출
            DecodeMP3FromBase64(base64, kReceiverObjectName, kReceiverMethodName);
            return await _webGLResult.Task;
        }

        // WebGL에서 호출 결과를 받을 GameObject가 없으면 생성
        private static void EnsureReceiverExists()
        {
            if (GameObject.Find(kReceiverObjectName) != null) return;

            var receiverObject = new GameObject(kReceiverObjectName);
            receiverObject.hideFlags = HideFlags.HideAndDontSave;
            receiverObject.AddComponent<ReceiverBehaviour>();
            GameObject.DontDestroyOnLoad(receiverObject);
        }

        // JS에서 SendMessage로 호출하는 메서드
        private class ReceiverBehaviour : MonoBehaviour
        {
            public void OnPCMDecoded(string json)
            {
                try
                {
                    // JSON -> PCMResult 파싱
                    var result = JsonUtility.FromJson<PCMResult>(json);
                    var floatBytes = Convert.FromBase64String(result.pcm);
                    int count = floatBytes.Length / sizeof(float);

                    float[] samples = new float[count];
                    Buffer.BlockCopy(floatBytes, 0, samples, 0, floatBytes.Length);

                    var clip = AudioClip.Create("WebGLDecodedClip", count, 1, result.sampleRate, false);
                    clip.SetData(samples, 0);

                    _webGLResult?.TrySetResult(new File<AudioClip>(clip));
                }
                catch (Exception e)
                {
                    Debug.LogError("[MPEGDecoder] Failed to decode: " + e);
                    _webGLResult?.TrySetResult(null);
                }
            }
        }

        [Serializable]
        private class PCMResult
        {
            public string pcm;
            public int sampleRate;
        } 
    }
}

#endif