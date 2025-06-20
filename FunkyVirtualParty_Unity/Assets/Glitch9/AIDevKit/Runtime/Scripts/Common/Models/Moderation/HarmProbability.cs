using System;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Probability that a prompt or candidate matches a harm category.
    /// </summary>
    public enum HarmProbability
    {
        [ApiEnum("HARM_PROBABILITY_UNSPECIFIED")] Unspecified,
        [ApiEnum("NEGLIGIBLE")] Negligible,
        [ApiEnum("LOW")] Low,
        [ApiEnum("MEDIUM")] Medium,
        [ApiEnum("HIGH")] High
    }

    internal static class HarmProbabilityUtil
    {
        internal static HarmProbability GetProbability(float score)
        {
            if (score < 0.1f) return HarmProbability.Negligible;
            if (score < 0.2f) return HarmProbability.Low;
            if (score < 0.5f) return HarmProbability.Medium;
            if (score < 0.8f) return HarmProbability.High;
            return HarmProbability.Unspecified;
        }

        internal static bool IsBlocked(this HarmProbability probability, HarmBlockThreshold threshold)
        {
            float blockValue = threshold.GetThreshold();
            float probabilityValue = probability.GetThreshold();
            return probabilityValue >= blockValue;
        }

        internal static float GetThreshold(this HarmProbability probability)
        {
            return probability switch
            {
                HarmProbability.Unspecified => -1f,
                HarmProbability.Negligible => 0.1f,
                HarmProbability.Low => 0.2f,
                HarmProbability.Medium => 0.5f,
                HarmProbability.High => 0.8f,
                _ => throw new ArgumentOutOfRangeException(nameof(probability), probability, null)
            };
        }
    }
}