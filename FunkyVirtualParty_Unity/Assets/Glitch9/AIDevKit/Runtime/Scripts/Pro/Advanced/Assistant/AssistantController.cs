using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Components;
using Glitch9.Collections;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OpenAIClient = Glitch9.AIDevKit.OpenAI.OpenAI;

namespace Glitch9.AIDevKit.OpenAI.Assistants
{
    /// <summary>
    /// Formarly <see cref="AssistantsAPIv2"/>, this class is now called <see cref="AssistantController"/>.
    /// The Assistants API allows you to build AI assistants within your own applications.
    /// An Assistant has instructions and can leverage models, tools, and files to respond to user queries.
    /// The Assistants API currently supports three types of tools: Code Interpreter, File Search, and Function calling.
    /// </summary>
    /// <remarks>
    /// AssistantTool now uses <see href="https://platform.openai.com/docs/assistants/overview">Assistants APIv2</see>.
    /// Assistants APIv1 will no longer be used. (Updated on 2024-05-23 by Munchkin)
    /// </remarks>
    public sealed partial class AssistantController
    {
        private const string kName = "AssistantsAPI";

        /// <summary>
        /// The minimum interval between operations in milliseconds.
        /// </summary>
        public const int MIN_INTERNAL_OPERATION_MILLIS = 1000;

        /// <summary>
        /// The minimum interval between requests in milliseconds.
        /// </summary>
        public const int MIN_INTERVAL_REQUEST_MILLIS = 2000;


        public string AssistantId { get; private set; }

        /// <summary>
        /// Gets or sets the name of this assistant.
        /// </summary>
        public string AssistantName => Assistant?.Name ?? "Unknown Assistant";

        /// <summary>
        /// Gets or sets the GPT model used by this assistant.
        /// </summary>
        public Model Model => Assistant?.Model ?? OpenAISettings.DefaultAssistantAPIModel;

        /// <summary>
        /// Gets or sets the description of this assistant.
        /// </summary>
        public string Description => Assistant?.Description;

        /// <summary>
        /// Gets or sets the instructions for this assistant.
        /// </summary>
        public string Instructions => Assistant?.Instructions;

        /// <summary>
        /// Give the Assistant access to up to 128 tools.
        /// You can give it access to OpenAI-hosted tools like code_interpreter and file_search,
        /// or call a third-party tools via a function calling.
        /// </summary>
        public List<ToolCall> Tools
        {
            get => Assistant?.Tools;
            set
            {
                if (value == null) return;
                Assistant.Tools = value;
            }
        }

        /// <summary>
        /// Give the tools like code_interpreter and file_search access to files.
        /// Files are uploaded using the File upload endpoint
        /// and must have the purpose set to assistants to be used with this API.
        /// </summary>
        public ToolResources ToolResources { get; set; }

        /// <summary>
        /// If not null, it forces this AssistantsAPI to use a specific tool on every request.
        /// </summary>
        public ToolChoice ForcedTool { get; set; }

        /// <summary>
        /// Gets or sets the response format for this assistant.
        /// </summary>
        public TextFormat? ResponseFormat => Assistant?.ResponseFormat?.ToEnum<TextFormat>();

        /// <summary>
        /// Gets or sets the metadata associated with this assistant.
        /// </summary>
        public Dictionary<string, string> Metadata => Assistant?.Metadata;

        /// <summary>
        /// Gets or sets the temperature setting for this assistant's responses.
        /// </summary>
        public float Temperature => Assistant?.Temperature ?? AIDevKitConfig.TemperatureDefault;

        /// <summary>
        /// Gets or sets the top-p setting for this assistant's responses.
        /// </summary>
        public float TopP => Assistant?.TopP ?? AIDevKitConfig.TopPDefault;

        /// <summary>
        /// Indicates if the tool is set to stream (not supported yet).
        /// </summary>
        public bool Stream { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum request length.
        /// </summary>
        public int MaxRequestLength { get; set; }

        /// <summary>
        /// Gets the OpenAiClient instance used by this tool.
        /// </summary>
        public OpenAIClient Client => OpenAIClient.DefaultInstance;

        /// <summary>
        /// Gets or sets the default run request options for this tool.
        /// </summary>
        public RunRequest DefaultRunRequest { get; set; }

        /// <summary>
        /// Current assistant object used by this tool.
        /// </summary>
        public Assistant Assistant { get; set; }

        /// <summary>
        /// Current thread object used by this tool.
        /// </summary>
        public Thread Thread { get; set; }

        /// <summary>
        /// Current run object used by this tool.
        /// </summary>
        public Run Run { get; set; }

        /// <summary>
        /// Current run step object used by this tool.
        /// </summary>
        public RunStep RunStep { get; set; }

        /// <summary>
        /// Last request object sent to the AssistantsAPI.
        /// </summary>
        public ThreadMessageRequest LastRequest { get; private set; }

        /// <summary>
        /// Last user message sent to the AssistantsAPI.
        /// </summary>
        public ThreadMessage LastUserMessage { get; private set; }

        /// <summary>
        /// Last assistant message received from the AssistantsAPI.
        /// </summary>
        public ThreadMessage LastAssistantMessage
        {
            get => _lastAssistantMessage;
            private set
            {
                _newAssistantMessageCreated = true;
                _lastAssistantMessage = value;
            }
        }

        private ThreadMessage _lastAssistantMessage;
        private bool _newAssistantMessageCreated = false;

        /// <summary>
        /// Last tool message received from the AssistantsAPI.
        /// </summary>
        public ThreadMessage LastToolMessage { get; private set; }

        /// <summary>
        /// Gets the current run status of the Assistants API.
        /// </summary>
        public RunStatus RunStatus => Run?.Status ?? RunStatus.Null;
        //private bool _isWaitingForAllRequiredActionToBeHandled = false;

        /// <summary>
        /// Gets the current stage of the Assistants API.
        /// </summary>
        public AssistantStatus AssistantStatus { get; private set; }

        public List<string> RequiredActions { get; } = new();

        public int RequiredActionTimeoutSeconds { get; set; } = AIDevKitConfig.DefaultRequiredActionTimeoutSeconds;

        public bool AutoCancelOnRequiredAction { get; set; } = false;

        /// <summary>
        /// Indicates whether the AssistantsAPI instance is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Gets or sets the current thread ID.
        /// </summary>
        public string ThreadId
        {
            get => _threadId.Value;
            set => _threadId.Value = value;
        }

        /// <summary>
        /// Gets the current run ID.
        /// </summary>
        public string RunId => Run?.Id ?? string.Empty;

        /// <summary>
        /// Indicates whether the AssistantsAPI is busy.
        /// </summary>
        public bool IsBusy => AssistantStatus != AssistantStatus.WaitingForInput;

        /// <summary>
        /// Indicates whether the AssistantsAPI requires action.
        /// </summary>
        public bool RequiresAction => RunStatus == RunStatus.RequiresAction;

        /// <summary>
        /// Indicates whether to save thread messages.
        /// </summary>
        public bool SaveThreadMessages { get; set; }

        /// <summary>
        /// Gets the saved messages.
        /// </summary>
        public PrefsDictionary<string, ThreadMessage> SavedMessages { get; }

        // prefs 
        private readonly Prefs<string> _threadId;
        private readonly PrefsList<string> _threadIdList; // This needs to be saved because Threads endpoint does not offer GetList.

        // providers & components
        private readonly AssistantProvider _assistantProvider;
        private readonly ThreadProvider _threadProvider;
        private readonly MessageProvider _messageProvider;
        private readonly RunProvider _runProvider;
        private readonly RunStepProvider _runStepProvider;
        private readonly AssistantEventDispatcher _dispatcher;
        private readonly AssistantLogger _logger;
        private readonly RunPoller _runPoller;
        private readonly AssistantEventRouter _router;
        private readonly IFunctionManager _functionManager;

        // status managing 
        private DateTime _lastRequestTime;
        private List<ToolCall> _missingTools;

        public AssistantController(AssistantSettings settings, RunRequest defaultRunOptions = null)
        {
            IResult optionsValidation = settings.IsValid;
            if (optionsValidation.IsFailure) throw new ArgumentException(optionsValidation.ErrorMessage, "AssistantOptions");

            Stream = settings.Stream;
            RequiredActionTimeoutSeconds = settings.RequiredActionTimeoutSeconds;
            AutoCancelOnRequiredAction = settings.AutoCancelOnRequiredAction;

            _logger = new(this);
            _router = new(settings);
            _runPoller = new(this, _logger, settings);
            _dispatcher = new(this, _logger);
            _functionManager = settings.FunctionManager;

            DefaultRunRequest = defaultRunOptions ?? new();

            // Those providers require event handlers to be added here.
            _assistantProvider = new(this, _logger);
            _assistantProvider.OnCreate += OnAssistantCreated;
            _assistantProvider.OnRetrieve += OnAssistantRetrieved;
            _assistantProvider.OnUpdate += OnAssistantUpdated;

            _threadProvider = new(this, _logger);
            _threadProvider.OnCreate += OnThreadCreated;
            _threadProvider.OnRetrieve += OnThreadRetrieved;
            _threadProvider.OnUpdate += OnThreadUpdated;

            _messageProvider = new(this, _logger);
            _messageProvider.OnCreate += OnMessageCreated;
            _messageProvider.OnRetrieve += OnMessageRetrieved;

            _runProvider = new(this, _logger);
            _runProvider.OnCreate += OnRunCreated;
            _runProvider.OnRetrieve += OnRunRetrieved;
            _runProvider.OnUpdate += OnRunUpdated;

            _runStepProvider = new(this, _logger);
            _runStepProvider.OnRetrieve += OnRunStepRetrieved;

            AssistantId = settings.AssistantId;
            //Tools = settings.Tools;
            ForcedTool = settings.ForcedTool;
            ToolResources = settings.ToolResources;
            SaveThreadMessages = settings.SaveMessages;
            MaxRequestLength = settings.MaxRequestLength;

            IFunctionManager functionManager = settings.FunctionManager;

            if (functionManager != null && !functionManager.IsEmpty)
            {
                FunctionDeclaration[] functionDeclarations = settings.FunctionManager.GetFunctionDeclarations();
                _missingTools = new();

                foreach (FunctionDeclaration functionDeclaration in functionDeclarations)
                {
                    if (functionDeclaration == null) continue;
                    string functionName = functionDeclaration.Name;
                    if (string.IsNullOrEmpty(functionName))
                    {
                        _logger.Error("Function name is null or empty.");
                        continue;
                    }
                    // check if the function name is already in the tools list
                    if (Tools != null && Tools.Any(x => x is FunctionCall functionCall && functionCall.Function.Name == functionName))
                    {
                        continue;
                    }

                    _logger.Info($"Adding missing function '{functionName}' to the tools list.");

                    FunctionCall functionCall = new(functionDeclaration);
                    _missingTools.Add(functionCall);
                }
            }

            // Values that are stored in PlayerPrefs
            SavedMessages = new($"{AssistantId}.SavedMessages");
            _threadId = new($"{AssistantId}.ThreadId", string.Empty);
            _threadIdList = new($"{AssistantId}.ThreadIds");
        }

        /// <summary>
        /// Initializes the AssistantsAPI instance asynchronously.
        /// </summary>
        /// <returns>A UniTask representing the asynchronous operation.</returns>
        public async UniTask InitializeAsync(Action onInitialized = null)
        {
            UpdateStatus(AssistantStatus.Initializing);

            await _threadProvider.RetrieveAsync(ThreadId, true); // Getting thread first, assistant depends on a thread

            await UniTask.Delay(MIN_INTERNAL_OPERATION_MILLIS);

            await _assistantProvider.RetrieveAsync(AssistantId, true);

            if (_missingTools.IsNotNullOrEmpty())
            {
                AssistantRequest req = new AssistantRequest.Builder()
                    .SetTools(_missingTools)
                    .Build();

                await UpdateAssistantAsync(req);
            }

            _missingTools = null;
            IsInitialized = true;
            onInitialized?.Invoke();
            UpdateStatus(AssistantStatus.WaitingForInput);
            _logger.Info($"Initialized successfully with Assistant: {AssistantId} and Thread: {ThreadId}");
        }

        /// <summary>
        /// Cancels the current run operation.
        /// </summary>
        /// <returns>A UniTask representing the asynchronous operation.</returns>
        public async UniTask CancelRunAsync()
        {
            RequiredActions.Clear();

            if (Run == null)
            {
                _logger.Warning("No run to cancel.");
                OnRunStatusChanged(RunStatus.Cancelled);
                return;
            }

            Run = await Client.Beta.Threads.Runs.CancelAsync(Thread.Id, Run.Id);

            if (Stream)
            {
                await RunStatus.WaitUntil(RunStatusType.Terminal);
            }
            else
            {
                await _runPoller.PollUntilConditionsAreMetAsync(Thread, Run, RunStatusCheckType.TerminalStatus);
            }
        }

        /// <summary>
        /// Create a new thread and set it as the current thread.
        /// If deleteCurrent is true, the current thread will be deleted before creating a new one.
        /// </summary>
        /// <param name="deleteCurrent"></param>
        /// <returns></returns>
        public async UniTask CreateThreadAsync(bool deleteCurrent = false)
        {
            if (deleteCurrent) await DeleteThreadAsync(Thread.Id);
            await _threadProvider.CreateAsync();
        }

        /// <summary>
        /// Switch to the existing thread with threadId.
        /// The thread with the threadId must exist.
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        public UniTask<Thread> SetThreadAsync(string threadId) => _threadProvider.RetrieveAsync(threadId);

        /// <summary>
        /// Get all thread ids saved in the PlayerPrefs.
        /// </summary> 
        public List<string> GetThreadIds() => _threadIdList?.ToList() ?? new();

        /// <summary>
        /// Delete the thread with the threadId.
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        public UniTask<bool> DeleteThreadAsync(string threadId) => _threadProvider.DeleteAsync(threadId);

        /// <summary>
        /// Update(modify) the assistant with the <see cref="AssistantRequest"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UniTask<Assistant> UpdateAssistantAsync(AssistantRequest request) => _assistantProvider.UpdateAsync(AssistantId, request);

        #region Request Methods

        /// <summary>
        /// Requests a response from the assistant asynchronously.
        /// </summary>
        /// <param name="textPrompt">The text prompt to send to the assistant.</param>
        /// <param name="customRunRequest">Optional custom run request options.</param>
        /// <returns>A task representing the result of the request.</returns>
        public async UniTask<ThreadMessage> RequestAsync(string textPrompt, RunRequest customRunRequest = null)
        {
            ThrowIfInvalidStatus();
            ThrowIfInvalidPrompt(textPrompt);
            return await HandleRequestAsync(ThreadMessageUtil.CreateMessageRequest(textPrompt), customRunRequest);
        }

        public async UniTask<ThreadMessage> RequestAsync(string textPrompt, RunRequest customRunRequest, params File<Texture2D>[] imageFiles)
        {
            ThrowIfInvalidStatus();
            ThrowIfInvalidPrompt(textPrompt);
            return await HandleRequestAsync(ThreadMessageUtil.CreateMessageRequest(textPrompt, imageFiles), customRunRequest);
        }

        public async UniTask<ThreadMessage> RequestAsync(string textPrompt, RunRequest customRunRequest, params string[] imageUrls)
        {
            ThrowIfInvalidStatus();
            ThrowIfInvalidPrompt(textPrompt);
            return await HandleRequestAsync(ThreadMessageUtil.CreateMessageRequest(textPrompt, imageUrls), customRunRequest);
        }

        public async UniTask<ThreadMessage> RequestAsync(File<AudioClip> audioPrompt, RunRequest customRunRequest = null)
        {
            ThrowIfInvalidStatus();
            ThrowIfInvalidPrompt(audioPrompt);
            return await HandleRequestAsync(new TranscriptionRequest.Builder().SetFile(audioPrompt).Build(), customRunRequest);
        }

        public async UniTask<ThreadMessage> RequestAsync(AudioClip audioPrompt, RunRequest customRunRequest = null)
        {
            ThrowIfInvalidStatus();
            ThrowIfInvalidPrompt(audioPrompt);
            return await HandleRequestAsync(new TranscriptionRequest.Builder().SetFile(audioPrompt).Build(), customRunRequest);
        }

        public async UniTask<ThreadMessage> RequestAsync(TranscriptionRequest transcriptionRequest, RunRequest customRunRequest = null)
        {
            ThrowIfInvalidStatus();
            return await HandleRequestAsync(transcriptionRequest, customRunRequest);
        }

        public async UniTask<ThreadMessage> RequestAsync(ThreadMessageRequest messageRequest, RunRequest customRunRequest = null)
        {
            ThrowIfInvalidStatus();
            return await HandleRequestAsync(messageRequest, customRunRequest);
        }

        private async UniTask<ThreadMessage> HandleRequestAsync(TranscriptionRequest transcriptionRequest, RunRequest customRunRequest = null)
        {
            // Step 1. Convert voice recording (Unity AudioClip) to text (string)
            OpenAITranscript transcription = await Client.Audio.Transcriptions.CreateAsync(transcriptionRequest);
            if (transcription == null || string.IsNullOrEmpty(transcription.Text)) throw new EmptyPromptException(typeof(OpenAITranscript));

            // Step 2. Then redirect to the text request
            return await RequestAsync(transcription.Text, customRunRequest);
        }

        #endregion

        private async UniTask<ThreadMessage> HandleRequestAsync(ThreadMessageRequest messageRequest, RunRequest customRunRequest = null)
        {
            LastRequest = messageRequest;
            LastRequest.Sender = AssistantName;
            _lastRequestTime = DateTime.Now;

            await _messageProvider.CreateAsync();
            Run newRun = await _runProvider.CreateAsync(customRunRequest);

            bool stream;
            if (customRunRequest == null) stream = Stream;
            else stream = customRunRequest.Stream ?? false;

            if (stream)
            {
                _logger.ReqVerbose("Assistant is streaming.");

                if (_router == null) throw new InvalidOperationException("Router is null. Cannot handle streaming.");
                await RunStatus.WaitUntil(RunStatusType.Terminal);
            }
            else
            {
                _logger.ReqVerbose("Assistant is not streaming. Checking for terminal status.");

                Run = newRun;
                await _runPoller.CreateNonStreamingResponseAsync();
            }

            return await GetResultAsync();
        }

        public async UniTask<ThreadMessage> SubmitToolOutputAsync(string toolCallId, string outputToSubmit)
        {
            _logger.ReqVerbose($"Submitting tool output for tool call ID: {toolCallId}");

            ToolOutputsRequest.Builder reqBuilder = new ToolOutputsRequest.Builder().AddToolOutput(toolCallId, outputToSubmit);

            if (Stream) reqBuilder.SetStreamHandler(new TextStreamHandler(onReceiveText: _dispatcher.OnReceiveStreamEvent));

            Run = await Client.Beta.Threads.Runs.SubmitToolOutputsAsync(Thread.Id, Run.Id, reqBuilder.Build());

            if (Run == null)
            {
                await CancelRunAsync();
                throw new EmptyResponseException("Failed to submit tool output.");
            }

            RequiredActions.Remove(toolCallId);

            return await WaitUntilRequiredActionsAreCompleteAsync();
        }

        internal async UniTask<ThreadMessage> HandleRequiredActionsAsync()
        {
            if (AutoCancelOnRequiredAction)
            {
                _logger.ResVerbose("The run requires action, but AutoCancelOnRequiredAction is set to true. Cancelling the run.");
                await CancelRunAsync();
                return null;
            }

            UpdateStatus(AssistantStatus.RequiresAction);

            // Step 1. Check if the run has valid RequiredAction.
            RequiredAction requiredAction = Run.RequiredAction;

            if (requiredAction?.SubmitToolOutputs?.ToolCalls == null || requiredAction.SubmitToolOutputs.ToolCalls.Length == 0)
            {
                await CancelRunAsync();
                throw new InvalidOperationException("The run requires action, but no action is specified by the OpenAI API.");
            }

            // Step 2. Get the required actions and submit the tool outputs.  
            foreach (ToolCall toolCall in requiredAction.SubmitToolOutputs.ToolCalls)
            {
                if (toolCall is not FunctionCall functionCall
                    || functionCall.Function == null
                    || string.IsNullOrEmpty(functionCall.Id)
                    || string.IsNullOrEmpty(functionCall.Function.Name))
                    continue;

                // Step 2.1. Get the function name and arguments from the tool call.
                string functionId = functionCall.Id;
                string functionArgs = functionCall.Args;
                string functionName = functionCall.Name;

                if (_functionManager != null && _functionManager.HasFunction(functionName))
                {
                    _logger.ReqVerbose($"FunctionManager is executing function '{functionName}' with arguments: {functionArgs}");

                    JToken functionResult = _functionManager.ExecuteFunction(functionName, functionArgs);
                    return await SubmitToolOutputAsync(functionId, functionResult.ToString());
                }

                OnRequiredAction(functionCall);
                _logger.ReqVerbose($"Required action '{functionName}' is not handled. Waiting for the user to handle it.");
            }

            UpdateStatus(AssistantStatus.WaitingForSubmitToolOutputs);
            return await WaitUntilRequiredActionsAreCompleteAsync();
        }

        private async UniTask<ThreadMessage> WaitUntilRequiredActionsAreCompleteAsync()
        {
            bool noTimeout = RequiredActionTimeoutSeconds <= 0;

            if (noTimeout)
            {
                await UniTask.WaitUntil(() => RequiredActions.Count == 0);
            }
            else
            {
                DateTime runExpiresAt = DateTime.Now.AddSeconds(RequiredActionTimeoutSeconds);

                // Wait until all actions are handled or until the run expires.
                await UniTask.WaitUntil(() => RequiredActions.Count == 0 || DateTime.Now > runExpiresAt);
                // If the run expires, cancel the run and throw an exception.
                if (DateTime.Now > runExpiresAt)
                {
                    await CancelRunAsync();
                    throw new TimeoutException("The run has expired.");
                }
            }

            if (Stream)
            {
                await RunStatus.WaitUntil(RunStatusType.Terminal);
            }
            else
            {
                await _runPoller.CreateNonStreamingResponseAsync();
            }

            return await GetResultAsync();
        }

        private async UniTask<ThreadMessage> GetResultAsync()
        {
            if (RunStatus == RunStatus.RequiresAction)
                return await HandleRequiredActionsAsync();
            return FinalizeResponseMessage();
        }

        private ThreadMessage FinalizeResponseMessage()
        {
            if (!_newAssistantMessageCreated) throw new InvalidOperationException("No assistant message was created.");

            Usage usage = Run.Usage;

            AssistantUtil.CreatePromptRecord(
                assistanatModel: Model,
                assistantName: AssistantName,
                usageFromRun: usage,
                inputMessage: LastUserMessage,
                outputMessage: LastAssistantMessage
            );

            UpdateStatus(AssistantStatus.WaitingForInput);

            return LastAssistantMessage;
        }

        internal void UpdateStatus(AssistantStatus status)
        {
            AssistantStatus = status;
            _logger.ResVerbose(status.GetStatusMessage());
        }

        internal RunRequest PrepareRunRequest(RunRequest req)
        {
            req.AssistantId = AssistantId;
            req.Model = Model;
            req.ToolChoice = ForcedTool;

            bool stream = req.Stream ?? Stream;

            if (stream)
            {
                AIDevKitDebug.Info("Stream is true, setting up stream handler.");

                req.Stream = true;
                req.StreamHandler = new TextStreamHandler(
                    //onReceiveTextStart: OnTextCreated,
                    onReceiveText: _dispatcher.OnReceiveStreamEvent
                //onReceiveTextDone: OnStreamDone
                );
            }

            return req;
        }

        private void ThrowIfInvalidStatus()
        {
            if (!IsInitialized) throw new InvalidServiceStatusException(kName, "The Assistants API is not initialized.");
            if (RequiresAction) throw new InvalidServiceStatusException(kName, "You can only SubmitToolOutputs() if AssistantsAPI requires action to be taken.");
            if (AssistantStatus != AssistantStatus.WaitingForInput) throw new InvalidServiceStatusException(kName, $"The Assistants API is not ready. Current status: {AssistantStatus}.");
            if (DateTime.Now - _lastRequestTime < TimeSpan.FromMilliseconds(MIN_INTERVAL_REQUEST_MILLIS)) throw new RateLimitExceededException();
        }

        private void ThrowIfInvalidPrompt(string promptText)
        {
            if (string.IsNullOrEmpty(promptText)) throw new EmptyPromptException(typeof(string));
        }

        private void ThrowIfInvalidPrompt<T>(T promptObject) where T : class
        {
            if (promptObject == null) throw new EmptyPromptException(typeof(T));
        }
    }
}
