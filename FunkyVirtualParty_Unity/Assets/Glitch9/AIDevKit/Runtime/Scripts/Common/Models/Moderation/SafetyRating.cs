using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// A safety rating associated with a {@link GenerateContentCandidate}
    /// </summary>
    public class SafetyRating
    {
        [JsonProperty("category")] public HarmCategory Category { get; set; }
        [JsonProperty("probability")] public HarmProbability Probability { get; set; }
        [JsonIgnore] public float Score { get; private set; }
        [JsonIgnore] public bool IsFlagged { get; private set; }

        public SafetyRating(HarmCategory category, float score, bool flagged)
        {
            Category = category;
            Score = score;
            IsFlagged = flagged;
            Probability = HarmProbabilityUtil.GetProbability(score);
        }

        public override string ToString() => $"{Category}({Probability})";
    }
}