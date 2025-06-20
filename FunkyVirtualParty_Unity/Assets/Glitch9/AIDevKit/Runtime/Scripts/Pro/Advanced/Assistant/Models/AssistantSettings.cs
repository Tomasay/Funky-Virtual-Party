using Glitch9.AIDevKit.Components;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    /// <summary>
    /// Represents configuration options for creating and interacting with an OpenAI Assistant tool.
    /// </summary>
    public class AssistantSettings
    {
        /// <summary>
        /// Validates if the configuration options are correctly set.
        /// </summary>
        internal IResult IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(AssistantId)) return Result.Fail("You must provide an ID for the assistant.");
                if (MaxRequestLength < 0 && MaxRequestLength != -1) return Result.Fail("MaxRequestLength must be greater than or equal to 0 or -1.");
                return Result.Success();
            }
        }

        /// <summary>
        /// Required. Unique identifier for the Assistant used for AssistantsAPI.
        /// This is used to locally identify the AssistantsAPI instance and is not related to the OpenAI API.
        /// </summary>
        public string AssistantId { get; set; }

        public IFunctionManager FunctionManager { get; set; }
        public IAssistantEventReceiver AssistantEventReceiver { get; set; }
        public IRunEventReceiver RunEventReceiver { get; set; }
        public IThreadEventReceiver ThreadEventReceiver { get; set; }
        public IRequiredActionListener RequiredActionListener { get; set; }
        public IThreadMessageEventReceiver MessageEventReceiver { get; set; }
        public IStreamingTextEventReceiver TextEventReceiver { get; set; }
        public IToolCallReceiver ToolCallReceiver { get; set; }

        /// <summary>
        /// Give the Assistant access to up to 128 tools.
        /// You can give it access to OpenAI-hosted tools like code_interpreter and file_search,
        /// or call a third-party tools via a function calling.
        /// </summary>
        //public List<ToolCall> Tools { get; set; }

        /// <summary>
        /// Optional. Give the tools like code_interpreter and file_search access to files.
        /// Files are uploaded using the File upload endpoint
        /// and must have the purpose set to assistants to be used with this API.
        /// </summary>
        public ToolResources ToolResources { get; set; }

        /// <summary>
        /// Optional. Forces the AssistantsAPI to use the specified tool on every request.
        /// </summary>
        public ToolChoice ForcedTool { get; set; }

        /// <summary>
        /// Optional. Gets or sets a value indicating whether the assistant should stream responses.
        /// [ASSISTANTS API STREAM IS NOT SUPPORTED YET] Defaults to false.
        /// </summary>
        public bool Stream { get; set; } = false;

        /// <summary>
        /// Optional. Gets or sets the maximum number of characters the assistant's responses can contain.
        /// Use -1 for no limit.
        /// </summary>
        public int MaxRequestLength { get; set; } = -1;

        /// <summary>
        /// Optional. Gets or sets the initial delay in seconds before checking the run status for the first time.
        /// </summary>
        public int RunPollingDelay { get; set; } = AIDevKitConfig.DefaultRunPollingDelay;

        /// <summary>
        /// Optional. Gets or sets the recurring interval in seconds for checking the run status.
        /// </summary>
        public int RunPollingInterval { get; set; } = AIDevKitConfig.DefaultRunPollingInterval;

        /// <summary>
        /// Optional. Gets or sets the timeout in seconds for the run operation.
        /// </summary>
        public int RunPollingTimeout { get; set; } = AIDevKitConfig.DefaultRunPollingTimeout;

        /// <summary>
        /// Optional. If true, <see cref="AssistantController"/> will save the thread messages to PlayerPrefs.
        /// </summary>
        public bool SaveMessages { get; set; } = false;

        public int RequiredActionTimeoutSeconds { get; set; } = AIDevKitConfig.DefaultRequiredActionTimeoutSeconds;
        public bool AutoCancelOnRequiredAction { get; set; } = false;
    }
}
