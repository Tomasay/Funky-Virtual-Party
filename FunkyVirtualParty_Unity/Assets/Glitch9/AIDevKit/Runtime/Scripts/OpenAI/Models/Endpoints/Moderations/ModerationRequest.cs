using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class ModerationRequest : AIRequest
    {
        /// <summary>
        /// Required. The input Text to classify
        /// </summary>
        [JsonProperty("input")] public Content Input { get; set; }
        [JsonIgnore] internal List<SafetySetting> SafetySettings { get; set; }

        public class Builder : ModelRequestBuilder<Builder, ModerationRequest>
        {
            public Builder SetPrompt(Content input)
            {
                _req.Input = input;
                return this;
            }

            public Builder SetPrompt(params string[] inputs)
            {
                _req.Input = new Content(inputs);
                return this;
            }

            public Builder SetSafetySettings(IEnumerable<SafetySetting> settings)
            {
                _req.SafetySettings = new List<SafetySetting>(settings);
                return this;
            }

            public Builder SetSafetySettings(params SafetySetting[] settings)
            {
                _req.SafetySettings = new List<SafetySetting>(settings);
                return this;
            }
        }
    }
}
