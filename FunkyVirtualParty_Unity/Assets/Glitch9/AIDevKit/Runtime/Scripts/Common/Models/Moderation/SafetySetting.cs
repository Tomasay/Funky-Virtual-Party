using System;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Safety setting, affecting the safety-blocking behavior.
    /// Passing a safety setting for a category changes the allowed probability that content is blocked.
    /// </summary>
    [Serializable]
    public class SafetySetting
    {
        /// <summary>
        /// Required. The category for this setting.
        /// </summary>
        [JsonProperty("category")] public HarmCategory Category { get; set; }

        /// <summary>
        /// Required. Controls the probability threshold at which harm is blocked.
        /// </summary>
        [JsonProperty("threshold")] public HarmBlockThreshold Threshold { get; set; }

        public SafetySetting() { }
        public SafetySetting(HarmCategory category, HarmBlockThreshold threshold)
        {
            Category = category;
            Threshold = threshold;
        }
    }
}