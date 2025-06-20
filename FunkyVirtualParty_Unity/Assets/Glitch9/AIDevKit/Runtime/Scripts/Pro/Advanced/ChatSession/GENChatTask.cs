using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Glitch9.AIDevKit.Advanced.Chat
{
    /// <summary>
    /// Task for generating text using an LLM model. Supports instructions and role-based prompts.
    /// </summary>
    public class GENChatTask : GENTask<GENChatTask, ChatMessage, ChatCompletion>
    {
        public override bool isWrapperTask => true;
        public GENChatTask(ChatSession chatSession, ChatMessage chatMessage) : base(chatMessage) => session = chatSession;
        internal readonly ChatSession session;
        internal List<ToolCall> tools;
        internal SpeechOutputOptions speechOutputOptions;
        private ChatCompletionStreamHandlerBuilder streamHandlerBuilder;

        // Fluent API Methods --------------------------------------------------------------------------------------------------  
        public GENChatTask SetTools(params ToolCall[] tools)
        {
            if (tools == null || tools.Length == 0) return this;
            this.tools = tools.ToList();
            return this;
        }

        public GENChatTask AddTools(params ToolCall[] tools)
        {
            if (tools == null || tools.Length == 0) return this;
            this.tools ??= new List<ToolCall>();
            this.tools.AddRange(tools);
            return this;
        }

        public GENChatTask SetFunctions(params FunctionCall[] functions)
        {
            if (functions == null || functions.Length == 0) return this;
            tools = functions.ToList().ConvertAll(f => f as ToolCall);
            return this;
        }

        public GENChatTask AddFunctions(params FunctionCall[] functions)
        {
            if (functions == null || functions.Length == 0) return this;
            tools ??= new List<ToolCall>();
            tools.AddRange(functions.ToList().ConvertAll(f => f as ToolCall));
            return this;
        }

        public GENChatTask SetSpeechOutputOptions(SpeechOutputOptions options)
        {
            speechOutputOptions = options;
            return this;
        }

        #region Fluent API Methods - Stream Events

        public GENChatTask OnStreamText(Action<string> onReceiveText, Action<string> onReceiveRefusal = null)
        {
            streamHandlerBuilder ??= new();
            streamHandlerBuilder.SetOnReceiveText(onReceiveText);
            streamHandlerBuilder.SetOnReceiveRefusal(onReceiveRefusal);
            return this;
        }

        public GENChatTask OnStreamToolCalls(Action<ToolCall[]> onToolCalls)
        {
            streamHandlerBuilder ??= new();
            streamHandlerBuilder.SetOnReceiveToolCalls(onToolCalls);
            return this;
        }

        public GENChatTask OnStreamError(Action<string> onError)
        {
            streamHandlerBuilder ??= new();
            streamHandlerBuilder.SetOnError(onError);
            return this;
        }

        public GENChatTask OnStreamDone(Action<ChatCompletion> onDone)
        {
            streamHandlerBuilder ??= new();
            streamHandlerBuilder.SetOnDone(onDone);
            return this;
        }

        protected ChatCompletionStreamHandler GetStreamHandler()
        {
            if (streamHandlerBuilder == null)
                throw new ArgumentNullException(nameof(streamHandlerBuilder), "Stream handler builder is null. Use OnStreamText, OnStreamToolCalls, OnStreamError, or OnStreamDone to set the stream handler.");
            return streamHandlerBuilder.Build();
        }

        #endregion
        // Execution Method --------------------------------------------------------------------------------------------------  
        protected override async UniTask<ChatCompletion> ExecuteAsyncINTERNAL()
        {
            string promptText = prompt.ToString();
            if (string.IsNullOrEmpty(promptText)) throw new ArgumentException("Prompt is empty. Please provide a valid prompt.");

            session.PushInput(prompt);
            GENResponseTask task = CreateResponseTask(promptText);

            ChatCompletion completion = await task.ExecuteAsync();
            if (completion == null) return null;

            session.PushResponse(completion);
            return completion;
        }

        public UniTask StreamAsync(IChatCompletionStreamHandler streamHandler = null)
        {
            string promptText = prompt.ToString();
            if (string.IsNullOrEmpty(promptText)) throw new ArgumentException("Prompt is empty. Please provide a valid prompt.");

            session.PushInput(prompt);

            GENResponseTask task = CreateResponseTask(promptText);
            streamHandler ??= GetStreamHandler();
            streamHandler.SetOnDoneInternal(completion => session.PushResponse(completion));

            return task.StreamAsync(streamHandler);
        }

        private GENResponseTask CreateResponseTask(string promptText)
        {
            Model model = session.Model;
            if (model == null) model = AIDevKitConfig.kDefault_Chat_Model;

            GENResponseTask task = promptText.GENResponse()
                .SetSender(sender)
                .SetIgnoreLogs(_ignoreLogs)
                .SetModel(model)
                .SetInstruction(session.Instructions)
                .SetStartingMessage(session.StartingMessage)
                .SetMessages(session.Messages?.ToArray())
                .SetTools(tools?.ToArray())
                .SetModelSettings(session.ModelOptions)
                .SetReasoningOptions(session.ReasoningOptions)
                .SetSpeechOutputOptions(speechOutputOptions ?? session.SpeechOutputOptions)
                .SetWebSearchOptions(session.WebSearchOptions)
                .SetCancellationToken(token);

            if (prompt is UserMessage userMessage && userMessage.AttachedFiles.IsNotNullOrEmpty())
            {
                AIDevKitDebug.Info($"Attaching {userMessage.AttachedFiles.Count} files to the task.");
                task.Attach(userMessage.AttachedFiles.ToArray());
            }

            return task;
        }
    }
}