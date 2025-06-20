using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Glitch9.IO.Networking.RESTApi
{
    internal static class UnityWebRequestHandler
    {
        internal static async UniTask<RESTResponse> SendUnityWebRequestAsync(UnityWebRequest request, float baseDelayInSec, int maxRetries, RESTClient client, CancellationToken? token)
        {
            await SendUnityWebRequestAsyncINTERNAL(request, baseDelayInSec, maxRetries, client, token);

            return new RESTResponse()
            {
                ErrorMessage = request.error,
                StatusCode = request.responseCode,
                IsSuccess = request.result == UnityWebRequest.Result.Success,
            };
        }

        internal static async UniTask<RESTResponse<TResBody>> SendUnityWebRequestAsync<TResBody>(UnityWebRequest request, float baseDelayInSec, int maxRetries, RESTClient client, CancellationToken? token)
        {
            await SendUnityWebRequestAsyncINTERNAL(request, baseDelayInSec, maxRetries, client, token);

            return new RESTResponse<TResBody>()
            {
                ErrorMessage = request.error,
                StatusCode = request.responseCode,
                IsSuccess = request.result == UnityWebRequest.Result.Success,
            };
        }

        private static async UniTask SendUnityWebRequestAsyncINTERNAL(UnityWebRequest request, float baseDelayInSec, int maxRetries, RESTClient client, CancellationToken? token)
        {
            if (request == null) throw new ArgumentNullException("UnityWebRequest is null.");

            const float minDelay = RESTApiV5.Config.kMinUnityWebRequestDelayInSec; // 2 seconds
            float currentDelay = baseDelayInSec < minDelay ? minDelay : baseDelayInSec;

            for (int attempt = 0; attempt < maxRetries; ++attempt)
            {
                //if (request == null) continue;
                if (request.isDone) break;

                if (token == null)
                {
                    await request.SendWebRequest().ToUniTask();
                }
                else
                {
                    await request.SendWebRequest().ToUniTask().AttachExternalCancellation(token.Value);
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return;
                }

                //UnityEngine.Debug.LogError("request.result: " + request.result);
                LogRequestError(request, client);

                if (attempt < maxRetries - 1)
                {
                    //UnityEngine.Debug.LogError("await UniTask.Delay(TimeSpan.FromSeconds(currentDelay));");
                    await UniTask.Delay(TimeSpan.FromSeconds(currentDelay));
                    currentDelay *= 2; // Exponential backoff
                }
            }

            throw new TimeoutException("UnityWebRequest did not complete successfully within the specified number of retries.");
        }

        private static void LogRequestError(UnityWebRequest request, RESTClient client)
        {
            string errorMsg = $"Request to {EndpointFormatter.HideKeyFromEndpoint(request.url)} failed. Error: {request.error}, Response Code: {request.responseCode}";
            client.Logger.Error(errorMsg);

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    client.Logger.Warning("Connection error detected.");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    client.Logger.Warning("Data processing error detected.");
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    client.Logger.Warning("HTTP error detected: " + request.responseCode);
                    break;
                default:
                    client.Logger.Warning("Unknown error occurred.");
                    break;
            }
        }
    }
}