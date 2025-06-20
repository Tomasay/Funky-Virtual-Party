using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.Networking
{
    public static class NetworkUtil
    {
        public static async UniTask CheckNetworkAsync(int checkIntervalInMillis, int timeoutInMillis)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                LogService.Warning("Internet is not reachable. Waiting for network connection...");
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutInMillis);

                try
                {
                    await UniTask.RunOnThreadPool(async () =>
                    {
                        await UniTask.Delay(checkIntervalInMillis);
                        while (Application.internetReachability == NetworkReachability.NotReachable)
                        {
                            await UniTask.Delay(checkIntervalInMillis); // Wait for half a second before checking again
                        }
                    }).Timeout(timeout);

                    LogService.Info("Network connection re-established.");
                }
                catch (TimeoutException)
                {
                    LogService.Error("Network check timed out.");
                    throw new TimeoutException("Network connection could not be re-established within the timeout period.");
                }
            }
        }

        public static async UniTask<bool> CheckUrlAsync(string url)
        {
            using var request = UnityWebRequest.Get(url);
            request.timeout = 3; // 타임아웃 3초 설정

            await request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            bool isNetworkError = request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError;
#else
            bool isNetworkError = request.isNetworkError || request.isHttpError;
#endif

            if (isNetworkError)
            {
                Debug.LogWarning($"Connection check failed: {request.error}");
                return false;
            }

            return true;
        }
    }
}