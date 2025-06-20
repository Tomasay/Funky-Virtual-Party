using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// A set of the feedback metadata the prompt specified in <see cref="GenerateContentRequest.Contents"/>.
    /// </summary>
    public class PromptFeedback
    {
        [JsonProperty("blockReason")] public StopReason? BlockReason { get; set; }
        [JsonProperty("safetyRatings")] public SafetyRating[] SafetyRatings { get; set; }

        internal static PromptFeedback Safety(SafetyRating[] SafetyRatings)
        {
            return new PromptFeedback
            {
                BlockReason = StopReason.Safety,
                SafetyRatings = SafetyRatings
            };
        }

        public override string ToString()
        {
            if (BlockReason == StopReason.Safety) return $"BlockReason: {BlockReason}, SafetyRatings: {SafetyRatings}";
            return $"BlockReason: {BlockReason}";
        }
    }
}