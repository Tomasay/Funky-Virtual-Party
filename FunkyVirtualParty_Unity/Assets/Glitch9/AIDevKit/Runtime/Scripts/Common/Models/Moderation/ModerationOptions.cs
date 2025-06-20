using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Configuration options for chat moderation.
    /// </summary>
    [JsonObject]
    public class ModerationOptions
    {
        /// <summary>
        /// Optional. Custom moderation model to use.
        /// This will be ignored for Google API calls 
        /// because Google has its own moderation process.
        /// </summary>
        [JsonProperty] public Model Model { get; set; }

        /// <summary>
        /// A list of unique SafetySetting instances for blocking unsafe content.
        /// that will be enforced on the GenerateTextRequest.prompt and GenerateTextResponse.candidates. 
        /// There should not be more than one setting for each SafetyCategory type. 
        /// The API will block any prompts and responses that fail to meet the thresholds set by these settings. 
        /// This list overrides the default settings for each SafetyCategory specified in the safetySettings. 
        /// If there is no SafetySetting for a given SafetyCategory provided in the list, 
        /// the API will use the default safety setting for that category. 
        /// </summary>
        [JsonProperty] public List<SafetySetting> SafetySettings { get; set; }

        [JsonIgnore] public bool IsValid => SafetySettings != null && SafetySettings.Count > 0;
    }
}