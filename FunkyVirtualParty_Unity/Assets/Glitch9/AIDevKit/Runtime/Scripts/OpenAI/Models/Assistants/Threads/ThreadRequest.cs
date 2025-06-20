using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    public class ThreadRequest : AIRequest
    {
        /// <summary>
        /// A list of messages to start the thread with.
        /// </summary>
        [JsonProperty("messages")] public List<ThreadMessage> Messages { get; set; }

        public class Builder : ModelRequestBuilder<Builder, ThreadRequest>
        {
            public Builder SetMessages(List<ThreadMessage> messages)
            {
                _req.Messages = messages;
                return this;
            }
        }
    }
}