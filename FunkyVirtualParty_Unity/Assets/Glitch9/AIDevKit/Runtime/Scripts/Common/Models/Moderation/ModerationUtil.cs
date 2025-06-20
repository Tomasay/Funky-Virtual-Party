using System.Collections.Generic;

namespace Glitch9.AIDevKit
{
    internal static class ModerationUtil
    {
        internal static bool IsBlocked(List<SafetySetting> settings, SafetyRating[] ratings)
        {
            if (settings.IsNullOrEmpty() || ratings.IsNullOrEmpty()) return false;

            bool isBlocked = false;

            foreach (SafetySetting setting in settings)
            {
                if (setting == null) continue;
                foreach (SafetyRating rating in ratings)
                {
                    if (rating == null) continue;

                    if (setting.Category == rating.Category)
                    {
                        isBlocked = rating.Probability.IsBlocked(setting.Threshold);
                        if (isBlocked) break;
                    }
                }
            }

            return isBlocked;
        }
    }
}