using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glitch9.AIDevKit.OpenAI
{
    public class RunRequest : AIRequest
    {
        /// <summary>
        /// [Required] The ID of the AssistantObject to use to execute this run.
        /// </summary>
        [JsonProperty("assistant_id")] public string AssistantId { get; set; }

        /// <summary>
        /// Override the default System message of the AssistantObject. 
        /// This is useful for modifying the behavior on a per-run basis.
        /// </summary>
        [JsonProperty("instructions")] public string Instructions { get; set; }

        /// <summary>
        /// Appends additional instructions at the end of the instructions for the run.
        /// This is useful for modifying the behavior on a per-run basis without overriding other instructions.
        /// </summary>
        [JsonProperty("additional_instructions")] public string AdditionalInstructions { get; set; }

        /// <summary>
        /// Adds additional messages to the thread before creating the run.
        /// </summary>
        [JsonProperty("additional_messages")] public List<ThreadMessage> AdditionalMessages { get; set; }

        /// <summary>
        /// Override the tools the assistant can use for this run.
        /// This is useful for modifying the behavior on a per-run basis.
        /// </summary>
        [JsonProperty("tools")] public List<ToolCall> Tools { get; set; }

        /// <summary>
        /// The maximum number of prompt tokens that may be used over the course of the run.
        /// The run will make a best effort to use only the number of prompt tokens specified,
        /// across multiple turns of the run. If the run exceeds the number of prompt tokens specified,
        /// the run will end with status incomplete. See incomplete_details for more info.
        /// </summary>
        [JsonProperty("max_prompt_tokens")] public int? MaxPromptTokens { get; set; }

        /// <summary>
        /// The maximum number of completion tokens that may be used over the course of the run.
        /// The run will make a best effort to use only the number of completion tokens specified,
        /// across multiple turns of the run. If the run exceeds the number of completion tokens specified,
        /// the run will end with status incomplete. See incomplete_details for more info.
        /// </summary>
        [JsonProperty("max_completion_tokens")] public int? MaxCompletionTokens { get; set; }

        /// <summary>
        /// Controls for how a thread will be truncated prior to the run.
        /// Use this to control the initial context window of the run.
        /// </summary>
        [JsonProperty("truncation_strategy")] public TruncationStrategy TruncationStrategy { get; set; }

        /// <summary>
        /// Controls which (if any) tool is called by the model.
        /// 'none' means the model will not call any tools and instead generates a message.
        /// 'auto' is the default value and means the model can pick between generating a message or calling one or more tools.
        /// required means the model must call one or more tools before responding to the user.
        /// Specifying a particular tool like {"type": "file_search"} or {"type": "function", "function": {"name": "my_function"}} forces the model to call that tool.
        /// </summary>
        [JsonProperty("tool_choice")] public ToolChoice ToolChoice { get; set; }

        /// <summary>
        /// If true, returns a stream of events that happen during the Run as server-sent events,
        /// terminating when the Run enters a terminal state with a data: [DONE] message.
        /// </summary>
        [JsonProperty("stream")] public bool? Stream { get; set; }

        /// <summary>
        /// What sampling temperature to use, between 0 and 2.
        /// Higher values like 0.8 will make the Output more random,
        /// while lower values like 0.2 will make it more focused and deterministic.
        /// We generally recommend altering this or top_p but not both.
        /// </summary>
        [JsonProperty("temperature")] public float? Temperature { get; set; }

        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling,
        /// where the model considers the results of the tokens with top_p probability mass.
        /// So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// We generally recommend altering this or temperature but not both.
        /// </summary>
        [JsonProperty("top_p")] public float? TopP { get; set; }

        public override string ToString()
        {
            return $"RunRequest: {JsonConvert.SerializeObject(this)}";
        }


        public class Builder : ModelRequestBuilder<Builder, RunRequest>
        {

            public Builder SetInstructions(string instructions)
            {
                _req.Instructions = instructions;
                return this;
            }

            public Builder SetTools(params ToolCall[] tools)
            {
                _req.Tools = new List<ToolCall>(tools);
                return this;
            }

            public Builder SetAdditionalInstructions(string instructions)
            {
                _req.AdditionalInstructions = instructions;
                return this;
            }

            public Builder SetAdditionalMessages(params ThreadMessage[] messages)
            {
                _req.AdditionalMessages = new List<ThreadMessage>(messages);
                return this;
            }

            public Builder AddAdditionalMessage(ThreadMessage message)
            {
                _req.AdditionalMessages ??= new List<ThreadMessage>();
                _req.AdditionalMessages.Add(message);
                return this;
            }

            public Builder AddTool(ToolCall tool)
            {
                _req.Tools ??= new List<ToolCall>();
                _req.Tools.Add(tool);
                return this;
            }

            public Builder SetMaxPromptTokens(int maxPromptTokens)
            {
                if (maxPromptTokens == -1) return this;
                _req.MaxPromptTokens = maxPromptTokens;
                return this;
            }

            public Builder SetMaxCompletionTokens(int maxCompletionTokens)
            {
                if (maxCompletionTokens == -1) return this;
                _req.MaxCompletionTokens = maxCompletionTokens;
                return this;
            }

            public Builder SetTruncationStrategy(TruncationStrategy truncationStrategy)
            {
                _req.TruncationStrategy = truncationStrategy;
                return this;
            }

            public Builder SelectTool(ToolType toolType)
            {
                _req.ToolChoice = toolType;
                return this;
            }

            public Builder SelectFunction(string functionName)
            {
                _req.ToolChoice = functionName;
                return this;
            }

            public Builder SetStream(bool stream = true)
            {
                _req.Stream = stream;
                return this;
            }

            public Builder SetTemperature(float temperature)
            {
                if (Math.Abs(temperature - AIDevKitConfig.TemperatureDefault) > Tolerance.FLOAT)
                {
                    _req.Temperature = temperature;
                }

                return this;
            }

            public Builder SetTopP(float topP)
            {
                if (Math.Abs(topP - AIDevKitConfig.TopPDefault) > Tolerance.FLOAT)
                {
                    _req.TopP = topP;
                }

                return this;
            }
        }
    }
}