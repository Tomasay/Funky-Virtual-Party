using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.OpenAI
{
    /// <summary>
    /// Given some input Text, outputs if the model classifies it as potentially harmful across several categories.
    /// Related guide: https://platform.openai.com/docs/guides/moderation
    /// POST https://api.openai.com/v1/moderations
    /// </summary>
    public class ModerationResponse : AIResponse
    {
        /// <summary>
        /// A list of moderation objects.
        /// </summary>
        [JsonProperty("results")] public List<ModerationDetail> Results { get; set; }

        public override string ToString()
        {
            if (Results.IsNullOrEmpty()) return "No moderation results";
            return string.Join("\n", Results);
        }

        public bool IsFlagged(out List<SafetyRating> results)
        {
            results = new();

            if (Results.IsNullOrEmpty()) return false;

            bool flagged = false;

            foreach (ModerationDetail result in Results)
            {
                if (result.Flagged)
                {
                    flagged = true;
                }

                foreach (KeyValuePair<string, bool> category in result.Categories)
                {
                    if (category.Value)
                    {
                        string harmKey = category.Key;

                        HarmCategory harm = HarmCategoryConverter.Parse(harmKey);

                        if (harm != HarmCategory.None)
                            results.Add(new SafetyRating(harm, result.CategoryScores[category.Key], category.Value));
                    }
                }
            }

            return flagged;
        }
    }

    public class ModerationDetail
    {
        /// <summary>
        /// Whether any of the below categories are flagged.
        /// </summary>
        [JsonProperty("flagged")] public bool Flagged { get; set; }

        /// <summary>
        /// A list of the categories, and whether they are flagged or not.
        /// </summary>
        [JsonProperty("categories")] public Dictionary<string, bool> Categories { get; set; }

        /// <summary>
        /// A list of the categories along with their scores as predicted by model.
        /// </summary>
        [JsonProperty("category_scores")] public Dictionary<string, float> CategoryScores { get; set; }

        public override string ToString()
        {
            return $"Flagged: {Flagged}\nCategories: {string.Join(", ", Categories)}\nCategory Scores: {string.Join(", ", CategoryScores)}";
        }
    }
}