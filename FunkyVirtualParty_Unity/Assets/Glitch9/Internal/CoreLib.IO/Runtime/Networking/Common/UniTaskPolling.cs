using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace Glitch9.IO.Networking
{
    public static class UniTaskPolling
    {
        public static async UniTask<JObject> PollAsync(
            string url,
            Action<TimeSpan> onProgress = null,
            Func<JObject, bool> isDonePredicate = null,
            TimeSpan? interval = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            isDonePredicate ??= CreateDefaultIsDonePredicate();
            interval ??= TimeSpan.FromSeconds(5);
            timeout ??= TimeSpan.FromMinutes(2);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout.Value);

            DateTime startTime = DateTime.UtcNow;

            while (true)
            {
                if (cts.Token.IsCancellationRequested)
                    throw new TimeoutException("Polling timed out");

                using var request = UnityWebRequest.Get(url);
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                    throw new Exception($"Polling failed: {request.error}");

                var result = JObject.Parse(request.downloadHandler.text);

                if (isDonePredicate(result))
                    return result;

                await UniTask.Delay(interval.Value, cancellationToken: cts.Token);

                if (onProgress != null)
                {
                    var elapsed = DateTime.UtcNow - startTime;
                    onProgress(elapsed);
                }
            }
        }

        private static Func<JObject, bool> CreateDefaultIsDonePredicate()
        {
            return result =>
            {
                var done = result["done"]?.ToObject<bool>() ?? false;
                if (done)
                {
                    var error = result["error"];
                    if (error != null)
                    {
                        throw new Exception($"Error: {error}");
                    }
                }
                return done;
            };
        }
    }
}