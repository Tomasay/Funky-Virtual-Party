using System.Collections.Generic;
using System.Linq;

namespace Glitch9.AIDevKit
{
    public class ChatCompletionRequest : CompletionRequestBase
    {
        #region Local-Managing Variables

        public string StartingMessage { get; set; }
        public string Summary { get; set; }

        #endregion

        /// <summary>
        /// Required. The messages in the conversation.
        /// </summary>
        public List<ChatMessage> Messages { get; set; } = new();

        /// <summary>
        /// Optional. List of tools in JSON for the model to use if supported.
        /// </summary>
        public ToolCall[] Tools { get; set; }

        /// <summary>
        /// Controls which (if any) Function is called by the model.
        /// none means the model will not call a Function and instead generates a message.
        /// auto means the model can pick between generating a message or calling a Function.
        /// Specifying a particular Function via {"type: "Function", "Function": {"name": "my_function"}} forces the model to call that Function.
        /// none is the default when no functions are present. auto is the default if functions are present.
        /// </summary>
        public ToolCall ToolChoice { get; set; }

        /// <summary>
        /// Parameters for audio output. 
        /// Required when audio output is requested with modalities: ["audio"]. 
        /// </summary>
        public SpeechOutputOptions Audio { get; set; }

        /// <summary>
        /// Output types that you would like the model to generate. 
        /// Most models are capable of generating text, which is the default:
        /// </summary>
        public Modality? Modalities { get; set; }

        /// <summary>
        /// This tool searches the web for relevant results to use in a response. Learn more about the web search tool.
        /// </summary>
        public WebSearchOptionsWrapper WebSearchOptions { get; set; }

        /// <summary>
        /// Optional. Specifies the latency tier to use for processing the request. 
        /// This parameter is relevant for customers subscribed to the scale tier service:
        ///
        /// If set to 'auto', and the Project is Scale tier enabled, the system will 
        /// utilize scale tier credits until they are exhausted.
        ///
        /// If set to 'auto', and the Project is not Scale tier enabled, the request 
        /// will be processed using the default service tier with a lower uptime SLA 
        /// and no latency guarentee.
        ///
        /// If set to 'default', the request will be processed using the default service 
        /// tier with a lower uptime SLA and no latency guarentee.
        ///
        /// If set to 'flex', the request will be processed with the Flex Processing 
        /// service tier. Learn more.
        ///
        /// When not set, the default behavior is 'auto'.
        /// When this parameter is set, the response body will include the service_tier utilized.
        /// Defaults to auto
        /// </summary>
        public OpenAIServiceTier? ServiceTier { get; set; }

        public class Builder : CompletionRequestBuilder<Builder, ChatCompletionRequest>
        {
            public Builder SetStartingMessage(string startingMessage)
            {
                if (string.IsNullOrEmpty(startingMessage)) return this;
                _req.StartingMessage = startingMessage;
                return this;
            }

            public Builder SetSummary(string summary)
            {
                if (string.IsNullOrEmpty(summary)) return this;
                _req.Summary = summary;
                return this;
            }

            public Builder SetMessages(List<ChatMessage> messages)
            {
                if (messages.IsNullOrEmpty()) return this;
                _req.Messages = messages;
                return this;
            }

            public Builder PushMessage(ChatMessage message)
            {
                if (message == null) return this;
                _req.Messages ??= new List<ChatMessage>();
                _req.Messages.Add(message);
                return this;
            }

            public Builder SetTools(params ToolCall[] tools)
            {
                if (tools.IsNullOrEmpty()) return this;
                _req.Tools = tools;
                return this;
            }

            public Builder SetToolChoice(ToolCall toolChoice)
            {
                if (toolChoice == null) return this;
                _req.ToolChoice = toolChoice;
                return this;
            }

            public Builder SetFunctions(params FunctionDeclaration[] functions)
            {
                if (functions.IsNullOrEmpty()) return this;
                _req.Tools = functions.Select(f => f != null ? new FunctionCall(f) : null).ToArray();
                return this;
            }

            public Builder SetSpeechOutput(SpeechOutputOptions options = null)
            {
                if (options == null) return this;
                _req.Modalities ??= Modality.Text;
                _req.Modalities |= Modality.Audio;
                _req.Audio = options;
                return this;
            }

            public Builder SetWebSearchOptions(WebSearchOptions webSearchOptions)
            {
                if (webSearchOptions == null) return this;
                _req.WebSearchOptions = new WebSearchOptionsWrapper(webSearchOptions);
                return this;
            }

            public Builder SetModalities(Modality modalities)
            {
                if (modalities == 0) return this;
                _req.Modalities = modalities;
                return this;
            }

            public Builder SetServiceTier(OpenAIServiceTier serviceTier)
            {
                _req.ServiceTier = serviceTier;
                return this;
            }
        }
    }
}
