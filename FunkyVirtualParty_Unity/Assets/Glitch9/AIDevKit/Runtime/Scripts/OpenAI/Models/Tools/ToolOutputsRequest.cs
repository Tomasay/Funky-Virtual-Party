using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI
{
    public class ToolOutputsRequest : AIRequest
    {
        /// <summary>
        /// [Required] A list of tools for which the outputs are being submitted.
        /// </summary>
        [JsonProperty("tool_outputs")] public ToolOutput[] ToolOutputs { get; set; }
        [JsonProperty("stream")] public bool? Stream { get; set; }


        public class Builder : ModelRequestBuilder<Builder, ToolOutputsRequest>
        {
            public Builder SetToolOutputs(params ToolOutput[] toolOutputs)
            {
                _req.ToolOutputs = toolOutputs;
                return this;
            }

            public Builder AddToolOutput(string toolCallId, string output)
            {
                if (_req.ToolOutputs == null)
                {
                    _req.ToolOutputs = new ToolOutput[] { new() { ToolCallId = toolCallId, Output = output } };
                }
                else
                {
                    List<ToolOutput> list = new(_req.ToolOutputs)
                    {
                        new() { ToolCallId = toolCallId, Output = output }
                    };
                    _req.ToolOutputs = list.ToArray();
                }

                return this;
            }

            public Builder SetStream(Action<string> onStream)
            {
                _req.StreamHandler = new TextStreamHandler(onReceiveText: onStream);
                return this;
            }
        }
    }

    public class ToolOutput
    {
        /// <summary>
        /// The ID of the Tool call in the required_action object within the run object the Output is being submitted for.
        /// </summary>
        [JsonProperty("tool_call_id")] public string ToolCallId { get; set; }

        /// <summary>
        /// The Output of the Tool call to be submitted to continue the run.
        /// </summary>
        [JsonProperty("output")] public string Output { get; set; }
    }
}