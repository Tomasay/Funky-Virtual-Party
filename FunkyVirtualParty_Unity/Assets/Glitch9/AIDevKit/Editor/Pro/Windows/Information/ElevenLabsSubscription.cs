using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.Editor;
using Glitch9.IO.Networking.RESTApi;
using UnityEditor;
using UnityEngine;
using ElevenLabsClient = Glitch9.AIDevKit.ElevenLabs.ElevenLabs;

namespace Glitch9.AIDevKit.Editor.Pro
{
    [InitializeOnLoad]
    internal static class ElevenLabsSubscription
    {
        static ElevenLabsSubscription()
        {
            AIDevKitEditor.isElevenLabsFreeTierPredicateAsync += IsFreeTierAsync;
            AIDevKitEditor.onShowElevenLabsSubscriptionWindow += ElevenLabsSubscriptionWindow.ShowWindow;
        }

        private const string kUserSubscriptionPrefsKey = "ElevenLabs.UserSubscription";
        public readonly static EPrefs<UserSubscription> UserSubscription = new(kUserSubscriptionPrefsKey, null);

        internal static async UniTask<UserSubscription> GetUserSubscriptionAsync(bool force = false)
        {
            if (force || !UserSubscription.HasValue())
            {
                UserSubscription.Value = await ElevenLabsClient.DefaultInstance.User.GetAsync(new RequestOptions
                {
                    IgnoreLogs = true,
                });

                UserSubscription.Save();

                string tier = UserSubscription.Value?.Tier?.ToLowerInvariant();

                if (!string.IsNullOrEmpty(tier))
                {
                    AIDevKitEditor.SetIsElevenLabsFreeTier(IsFreeTier(tier));
                }
            }

            if (!UserSubscription.HasValue())
            {
                Debug.LogError("Failed to retrieve user subscription from ElevenLabs API. Please try again later.");
            }

            return UserSubscription.Value;
        }

        private static bool IsFreeTier(string tier) => !string.IsNullOrEmpty(tier) && (tier.Contains("free") || tier.Contains("trial"));

        internal static async UniTask<bool> IsFreeTierAsync()
        {
            if (!UserSubscription.HasValue())
            {
                UserSubscription.Value = await ElevenLabsClient.DefaultInstance.User.GetAsync(new RequestOptions
                {
                    IgnoreLogs = true,
                });

                UserSubscription.Save();
            }

            return IsFreeTier(UserSubscription.Value?.Tier?.ToLowerInvariant());
        }
    }
}