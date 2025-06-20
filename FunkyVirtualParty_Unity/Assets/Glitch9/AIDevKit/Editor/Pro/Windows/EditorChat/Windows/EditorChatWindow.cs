using System;
using System.Collections.Generic;
using Glitch9.AIDevKit.Advanced.Chat;
using Glitch9.AIDevKit.Editor.Chatbots;
using Glitch9.AIDevKit.Editor.Generation;
using Glitch9.Editor;
using Glitch9.Editor.UIToolkit;
using Glitch9.IO.Files;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    [InitializeOnLoad]
    internal partial class EditorChatWindow : EditorWindow
    {
        static EditorChatWindow()
        {
            AIDevKitEditor.onShowEditorChatWindow += () => ShowWindow();
        }

        internal static void ShowWindow(MonoScript scriptToDebug = null)
        {
            EditorChatWindow window = GetWindow<EditorChatWindow>(AIDevKitEditor.Labels.EditorChat, true);
            window.titleContent = new(AIDevKitEditor.Labels.EditorChat, AIDevKitIcons.Assistant);
            window.minSize = new Vector2(100, 200);
            window.autoRepaintOnSceneChange = true;
            window.ScriptToDebug = scriptToDebug;

            window.Show();
        }

        internal EditorChatServiceBase ChatService { get; private set; }
        internal Result<ChatSession> CurrentSession
        {
            get => _currentSession ??= FetchCurrentSession();
            set => _currentSession = value;
        }

        internal IReadOnlyList<ChatMessage> Messages
        {
            get
            {
                if (CurrentSession == null || CurrentSession.IsError) return null;
                return CurrentSession.Value.Messages;
            }
        }

        internal int MessageCount => Messages?.Count ?? 0;
        internal int DisplayedMessageCount { get; set; } = EditorChatConfig.DefaultDisplayedMessageCount;
        internal UnityObjectProfile SelectedObject => _selectionBinder?.SelectedObject;
        internal EditorChatState CurrentState => ChatService.CurrentState;
        internal Dictionary<string, IFile> AttachedFiles => _attachmentBinder?.AttachedFiles;
        internal Usage TotalUsage => CurrentSession?.Value?.TotalUsage;
        internal float TotalCostInUSD => CurrentSession?.Value?.TotalCostInUSD ?? 0f;
        internal MonoScript ScriptToDebug { get; set; }
        internal bool IsScriptDebugged { get; set; } = false;
        internal bool IsError => CurrentSession == null || CurrentSession.IsError;
        internal bool IsInvalidSession => CurrentSession == null || CurrentSession.IsError;

        internal ProgressBarController ProgressBar => _progressBarController ??= new ProgressBarController(this);
        private ProgressBarController _progressBarController;
        private ILogger _logger;
        private TextField _inputField;
        private ChatScrollViewController _chatScrollView;
        private ChatIMGUIToolbar _imguiToolbar;
        private ChatSelectionBinder _selectionBinder;
        private ChatAttachmentBinder _attachmentBinder;
        private ChatConsoleErrorWatcher _consoleErrorWatcher;
        private Result<ChatSession> _currentSession;
        private Button _scrollToBottomButton;
        private VisualElement _usageContainer;
        private bool _isShowingTempMessages = false;
        private bool _isInitialized = false;

        private void OnEnable()
        {
            _selectionBinder?.OnEnable();
            _isInitialized = true;
            InitializeINTERNAL();
            _isShowingTempMessages = false;
        }

        private void OnDisable()
        {
            _selectionBinder?.OnDisable();
            _isShowingTempMessages = false;
        }

        public void CreateGUI() => rootVisualElement.schedule.Execute(RebuildUI).ExecuteLater(1); // 1프레임 뒤에 실행 
        internal async void RebuildUI()
        {
            UIToolkitUtil.InitializeRootElement(
                root: rootVisualElement,
                style: "EditorChatWindow",
                marker: "styles_editor_chat",
                "ChatMessage", "ChatInputField", "ChatScrollView");

            if (!EditorChatSettings.RequirementsMet)
            {
                EditorChatSettings.RequirementsMet = await EditorChatUtil.CheckRequirementsAsync(_logger);

                if (!EditorChatSettings.RequirementsMet)
                {
                    rootVisualElement.Add(new LoadingSpinner());
                    rootVisualElement.Add(new Label("Please set up an API key in the settings or run your own Ollama server."));
                    return;
                }
            }

            if (!_isInitialized)
            {
                _isInitialized = true;
                InitializeINTERNAL();
            }

            _attachmentBinder?.RegisterCallbacks(rootVisualElement);

            RebuildToolbar();
            RebuildTitleBar();
            RebuildChatArea();
            RebuildAttachmentContainer();
            RebuildSelectionContainer();

            var inputFieldContainer = rootVisualElement.Q<VisualElement>("input-field-container");
            if (inputFieldContainer != null) _attachmentBinder?.RegisterCallbacks(inputFieldContainer);

            _inputField = rootVisualElement.Q<TextField>("input-field");
            var unityTextInput = _inputField.Q("unity-text-input");
            var sendButton = rootVisualElement.Q<Button>("send-button");

            EditorChatUtil.UI.StyleInputField_TextField(_inputField);
            EditorChatUtil.UI.StyleInputField_UnityTextInput(unityTextInput);
            EditorChatUtil.UI.StyleInputField_SendButton(sendButton);

            EditorChatUtil.Event.RegisterInputFieldCallbacks(_inputField, sendButton, RequestWithCurrentPrompt);
            EditorChatUtil.Event.RegisterInputFieldFocusEventToRoot(rootVisualElement, _inputField);

            // 가장 하단에 위치
            if (EditorChatSettings.ShowUsageTracking)
            {
                // add separator line
                var separator = new VisualElement { name = "usage-separator" };
                separator.AddToClassList("usage-separator");
                rootVisualElement.Add(separator);

                RebuildUsageTracking(true);
            }

            ProgressBar.HideProgressBar();
            ChatConsoleErrorWatcher.NotifyIfErrorExists();
        }

        internal void RebuildToolbar()
        {
            var toolbar = rootVisualElement.Q<VisualElement>("toolbar");
            if (toolbar == null)
            {
                _logger?.Error("Toolbar not found in the root visual element.");
                return;
            }

            toolbar.style.display = EditorChatSettings.ShowToolbar ? DisplayStyle.Flex : DisplayStyle.None;

            if (EditorChatSettings.ShowToolbar)
            {
                toolbar.style.display = DisplayStyle.Flex;
                EditorChatUtil.UI.SetupToolbar(toolbar, _imguiToolbar);
            }
            else
            {
                toolbar.style.display = DisplayStyle.None;
            }
        }

        internal void RebuildTitleBar()
        {
            var titleBar = rootVisualElement.Q<VisualElement>("title-bar");
            if (titleBar == null)
            {
                _logger?.Error("Title bar not found in the root visual element.");
                return;
            }

            if (EditorChatSettings.ShowTitleBar)
            {
                titleBar.style.display = DisplayStyle.Flex;
                EditorChatUtil.UI.SetupTitleBar(titleBar, _currentSession?.Value?.Name ?? ChatSession.kDefaultTitle,
                newTitle =>
                {
                    if (string.IsNullOrWhiteSpace(newTitle)) return;
                    _currentSession.Value.Name = newTitle;
                    _currentSession.Value.SaveFile();
                    AIDevKitDebug.Blue($"Current session set to: {newTitle}");
                });
            }
            else
            {
                titleBar.style.display = DisplayStyle.None;
            }
        }

        internal void RebuildUsageTracking(bool forceRebuild = false)
        {
            AIDevKitDebug.Green("Rebuilding usage tracking UI...");

            if (forceRebuild || _usageContainer == null)
            {
                _usageContainer = new VisualElement() { name = "usage-container" };
                _usageContainer.AddToClassList("usage-container");
                rootVisualElement.Add(_usageContainer);
            }

            EditorChatUtil.UI.BuildUsageTrackingUI(_usageContainer, TotalUsage, TotalCostInUSD);
        }

        internal void RebuildChatArea()
        {
            AIDevKitDebug.Green("Rebuilding chat area...");

            if (CurrentSession.IsError)
            {
                BuildSessionErrorView(CurrentSession.ErrorMessage);
            }
            else
            {
                RebuildChatScrollView();
            }

            void BuildSessionErrorView(string errorMessage)
            {
                var errorView = EditorChatUtil.UI.CreateChatErrorView(rootVisualElement); // it's a container (column)
                if (errorView == null)
                {
                    _logger?.Error("Chat Error View could not be created.");
                    return;
                }

                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = "There was an error loading the chat session.";
                }

                // add error message label, then two buttons: Reload and Delete Session 
                var errorMessageLabel = new Label(errorMessage) { name = "error-message-label" };
                errorMessageLabel.AddToClassList("error-message-label");
                errorView.Add(errorMessageLabel);

                // add some spacing
                errorView.Add(new VisualElement { style = { height = 10 } }); // spacer

                var reloadButton = new Button(() =>
                {
                    _currentSession = FetchCurrentSession();
                    RebuildChatArea();
                })
                {
                    text = "Reload Session",
                    name = "reload-button"
                };

                var deleteButton = new Button(() =>
                {
                    string id = EditorChatSettings.CurrentSessionId;
                    if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

                    bool success = ChatSessionManager.DeleteSession(CurrentSession.Value);

                    if (success)
                    {
                        ChatService.OnDeleteChatSession(id);
                        Debug.Log($"Chat session {id} deleted.");
                        CurrentSession.Value = ChatSessionManager.GetLastEditorSession();
                        EditorChatSettings.CurrentSessionId = CurrentSession.Value?.Id;
                        AIDevKitDebug.Blue($"Current session set to: {CurrentSession.Value?.Id}");
                    }
                    else
                    {
                        Debug.LogError($"Failed to delete chat session {id}.");
                    }

                    RebuildChatArea();
                })
                {
                    text = "Delete Session",
                    name = "delete-button"
                };

                reloadButton.AddToClassList("primary-button");
                deleteButton.AddToClassList("primary-button");

                errorView.Add(reloadButton);
                errorView.Add(deleteButton);
            }
        }

        internal void RebuildChatScrollView(bool scrollToBottom = true)
        {
            if (!EditorChatUtil.CheckNull(nameof(ChatScrollViewController), _chatScrollView, _logger))
                return;

            _chatScrollView = new ChatScrollViewController(this);
            _chatScrollView.RebuildUI(Messages, scrollToBottom);
            _attachmentBinder?.RegisterCallbacks(_chatScrollView.ScrollView);
            _scrollToBottomButton = rootVisualElement.Q<Button>("scroll-to-bottom-button");

            var scrollView = _chatScrollView?.ScrollView;
            if (scrollView == null)
            {
                _logger?.Error("ChatScrollView is not initialized. Cannot rebuild chat scroll view.");
                return;
            }

            EditorChatUtil.UI.SetupScrollToBottomButton(_scrollToBottomButton, scrollView);
        }

        internal void RebuildAttachmentContainer()
        {
            if (!EditorChatUtil.CheckNull(nameof(ChatAttachmentBinder), _attachmentBinder, _logger))
                return;

            var attachmentContainer = rootVisualElement.Q<VisualElement>("attachment-container");
            _attachmentBinder?.BuildUI(attachmentContainer);
        }

        internal void RebuildSelectionContainer()
        {
            if (!EditorChatUtil.CheckNull(nameof(ChatSelectionBinder), _selectionBinder, _logger))
                return;

            var selectionContainer = rootVisualElement.Q<VisualElement>("selection-container");
            _selectionBinder?.BuildUI(selectionContainer, AttachedFiles.IsNotNullOrEmpty());
        }

        internal void SetScrollToBottomButtonVisible(bool visible)
        {
            if (_scrollToBottomButton == null)
            {
                _logger?.Error("Scroll to bottom button is not initialized.");
                return;
            }

            _scrollToBottomButton.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        internal void ResetWindow()
        {
            IsScriptDebugged = false;
        }

        #region Request Methods

        internal async void RequestWithCurrentPrompt()
        {
            if (IsInvalidSession) return;

            string prompt = _inputField?.text;
            if (string.IsNullOrEmpty(prompt)) return;

            List<IFile> files = new();
            List<IFile> scriptFiles = new();
            List<string> scriptFilePaths = new();

            if (AttachedFiles.IsNotNullOrEmpty())
            {
                foreach (var kvp in AttachedFiles)
                {
                    string filePath = kvp.Key;
                    IFile file = kvp.Value;

                    if (file == null) continue;

                    if (file.GetType() == typeof(RawFile))
                    {
                        scriptFiles.Add(file);
                        scriptFilePaths.Add(filePath);
                    }
                    else
                    {
                        files.Add(file);
                    }
                }
            }

            UserMessage inputMessage = await EditorChatFactory.CreateUserMessageAsync(prompt, scriptFilePaths);

            if (files.IsNotNullOrEmpty())
            {
                if (EditorChatSettings.DebugMode) AIDevKitDebug.Info($"Adding {files.Count} attachments to the user message.");
                inputMessage.AddAttachments(files);
            }

            if (EditorChatSettings.DebugMode) AIDevKitDebug.Info($"Sending request: {inputMessage.Content}");

            if (ChatService.SendRequest(inputMessage))
            {
                _chatScrollView.AddUserMessage(inputMessage, scriptFiles);
            }

            ClearAttachments();
        }

        internal void RequestWithSystemMessage(string systemPrompt, string displayMessage = null, Action<ResponseMessage> onSuccess = null)
        {
            if (IsInvalidSession || string.IsNullOrEmpty(systemPrompt)) return;
            if (EditorChatSettings.DebugMode) AIDevKitDebug.Info($"Sending request: {systemPrompt}");

            SystemMessage inputMessage = EditorChatFactory.CreateSystemMessage(systemPrompt, displayMessage);

            if (ChatService.SendRequest(inputMessage, onSuccess))
            {
                _chatScrollView.AddUserMessage(inputMessage);
            }
        }

        internal void SubmitEditedMessage(int index, string newContent)
        {
            if (IsInvalidSession || string.IsNullOrEmpty(newContent)) return;
            if (EditorChatSettings.DebugMode) AIDevKitDebug.Info($"Editing message at index {index}: {newContent}");

            if (index < 0 || index >= MessageCount)
            {
                _logger?.Error($"Invalid message index: {index}. Cannot edit message.");
                return;
            }

            if (ChatService.SendEditRequest(index, newContent))
            {
                RebuildChatScrollView();
                if (EditorChatSettings.DebugMode) AIDevKitDebug.Info($"Edited message at index {index}: {newContent}");
            }
        }

        internal void UpdateStreamingText(string delta) => _chatScrollView?.UpdateStreamingResponse(delta);
        internal void FinalizeResponse(ChatMessage message)
        {
            EditorChatUtil.FinalizeResponse(message, _chatScrollView, _logger);
            RebuildUsageTracking();
        }
        internal void CancelRequest() => ChatService?.CancelRequest();

        #endregion Request Methods

        #region GUI Tool Methods 

        internal void StartNewChat()
        {
            CurrentSession = EditorChatUtil.File.GetOrCreateSession(string.Empty, true);
            EditorChatSettings.CurrentSessionId = CurrentSession?.Value?.Id;
            ChatService.OnSetChatSession(EditorChatSettings.CurrentSessionId);
            if (EditorChatSettings.DebugMode) AIDevKitDebug.Blue($"Starting new chat session with ID: {EditorChatSettings.CurrentSessionId}");
            RebuildUI();
        }

        internal void SetChatSession(ChatSession session)
        {
            if (session == null || string.IsNullOrWhiteSpace(session.Id))
            {
                _logger?.Error("Invalid chat session provided.");
                return;
            }

            CurrentSession.Value = session;
            EditorChatSettings.CurrentSessionId = session.Id;
            if (EditorChatSettings.DebugMode) AIDevKitDebug.Blue($"Setting chat session to: {EditorChatSettings.CurrentSessionId}");
            ChatService.OnSetChatSession(EditorChatSettings.CurrentSessionId);
            RebuildUI();
        }

        internal void SaveChatAsText() => EditorChatUtil.File.SaveChatAsText(Messages, CurrentSession);
        internal void ClearAttachments() => _attachmentBinder?.Clear();

        #endregion GUI Tool Methods

        #region Utility Methods     

        // Version2 with no FloatingContainer
        internal void OnConsoleErrorReceived(int count, string message)
        {
            if (_isShowingTempMessages || _chatScrollView == null) return;

            if (count < 1)
            {
                if (EditorChatSettings.DebugMode) AIDevKitDebug.Green("No unresolved errors found.");
                _chatScrollView.ClearTempMessage();
                return;
            }

            if (EditorChatSettings.DebugMode) AIDevKitDebug.Red($"You have {count} unresolved errors.");

            _isShowingTempMessages = true;

            var buttons = new List<ButtonEntry>
            {
                new(label: "Analyze Errors",
                    icon: EditorIcons.ConsoleWindow as Texture2D,
                    callback: () =>  AnalyzeConsoleErrors(count)),

                new(label: "Ignore",
                    icon: EditorIcons.Close as Texture2D,
                    callback: FinalizeTempMessages),
            };

            if (_consoleErrorWatcher.HasScriptErrors(out string sourcePath))
                buttons.Insert(1,
                new(label: "Fix Script Errors",
                    icon: EditorIcons.Generate as Texture2D,
                    callback: () => FixScriptErrors(sourcePath)));

            var tempMessage = EditorChatFactory.CreateTempMessageItem(
                // msg: $"There are {count} errors in the Console that haven’t been resolved.",
                msg: $"Console Errors Detected: {count}\n{message}",
                messageType: MessageType.Error,
                buttons: buttons
            );

            _chatScrollView.AddTempMessage(tempMessage, true);

            if (EditorChatSettings.DebugMode)
            {
                string joinedMessage = _consoleErrorWatcher.GetJoinedMessage();
                AIDevKitDebug.Info($"Unresolved errors:\n{joinedMessage}");
            }
        }

        internal void ShowErrorMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                _logger?.Error("Error message is null or empty. Cannot show error message.");
                return;
            }

            var errorMessage = EditorChatFactory.CreateTempMessageItem(
                msg: message,
                messageType: MessageType.Error,
                buttons: new List<ButtonEntry>
                {
                    new(label: "Close",
                        icon: EditorIcons.Close as Texture2D,
                        callback: FinalizeTempMessages)
                }
            );

            _chatScrollView.AddTempMessage(errorMessage, true);
            _chatScrollView.RemoveStreamingItem();
        }

        private void AnalyzeConsoleErrors(int count)
        {
            string joinedMessage = _consoleErrorWatcher.GetJoinedMessage();
            if (string.IsNullOrEmpty(joinedMessage))
            {
                _logger?.Warning("Joined error message is empty. No error messages to analyze.");
                return;
            }

            RequestWithSystemMessage(
                systemPrompt:
                $"There are {count} unresolved errors in the Unity Console.\n" +
                "Error Details:\n" +
                $"{joinedMessage}\n\n" +
                "Please help analyze these errors and suggest how to fix them.",

                displayMessage:
                "You're asking for help with the current Console errors."
            );

            FinalizeTempMessages();
        }

        private void FixScriptErrors(string sourcePath)
        {
            ClearTempMessages();

            RequestWithSystemMessage(
                systemPrompt:
                "There are unresolved script errors in the Unity Console.\n" +
                "Generate a fixed version of the script(s) that resolves these errors.\n\n" +
                "Error Details:\n" +
                $"{_consoleErrorWatcher.GetJoinedMessage()}\n\n" +
                "Do not make any title, any explanations, transliteration or extra punctuation.",

                displayMessage: "You're asking for a fix for the Console script errors.",

                onSuccess: responseMessage =>
                {
                    if (EditorChatSettings.DebugMode) AIDevKitDebug.Blue("Received response for script fix request.");

                    if (responseMessage == null)
                    {
                        _logger?.Error("Response message is null. Cannot apply script fix.");
                        return;
                    }

                    if (!ScriptExporter.ValidateScript(responseMessage.Content, out string errorMessage))
                    {
                        _logger?.Info($"No script fix needed: {errorMessage}");
                        return;
                    }

                    string fixedScript = responseMessage.Content;

                    if (string.IsNullOrEmpty(fixedScript))
                    {
                        _logger?.Error("Fixed script is empty. Cannot apply script fix.");
                        return;
                    }

                    var buttons = new List<ButtonEntry>
                    {
                        new(label: "Apply Fix",
                            icon: EditorIcons.Check  as Texture2D,
                            callback: () => ApplyScriptFix(fixedScript, sourcePath)),
                        new(label: "Discard Fix",
                            icon: EditorIcons.Close as Texture2D,
                            callback: FinalizeTempMessages)
                    };

                    var tempMessage = EditorChatFactory.CreateTempMessageItem(
                        msg: "Script fix generated successfully.\nPlease review the changes carefully before applying.",
                        messageType: MessageType.Info,
                        buttons: buttons
                    );

                    _chatScrollView.AddTempMessage(tempMessage, true);
                }
            );
            _consoleErrorWatcher.ClearErrors();
            _isShowingTempMessages = false;
        }

        private async void ApplyScriptFix(string fixedScript, string sourcePath)
        {
            if (string.IsNullOrEmpty(fixedScript) || string.IsNullOrEmpty(sourcePath))
            {
                _logger?.Error("Fixed script or source path is empty. Cannot apply script fix.");
                return;
            }

            try
            {
                string className = await ScriptExporter.SaveAsFileAsync(fixedScript, sourcePath);
                if (string.IsNullOrEmpty(className))
                {
                    _logger?.Error("Failed to save the fixed script. Class name is empty.");
                    return;
                }

                _logger?.Info($"Fixed script saved successfully as {className}.cs");
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                _logger?.Error($"Failed to apply script fix: {ex.Message}");
            }
            finally
            {
                FinalizeTempMessages();
            }
        }

        private void InitializeINTERNAL()
        {
            _logger = new DefaultLogger(AIDevKitEditor.Labels.EditorChat);
            _currentSession = FetchCurrentSession();

            // if (EditorChatSettings.ChatType == EditorChatType.OpenAI_AssistantsAPI) 
            //     ChatService = new EditorAssistantChatService(this); 
            // else 
            //     ChatService = new EditorChatService(this); 

            // For now, always use the Default EditorChatService
            ChatService = new EditorChatService(this);

            // initailize ui controllers
            _chatScrollView = new ChatScrollViewController(this);
            _imguiToolbar = new ChatIMGUIToolbar(this);
            _selectionBinder = new ChatSelectionBinder(this);
            _attachmentBinder = new ChatAttachmentBinder(this);
            _consoleErrorWatcher = new ChatConsoleErrorWatcher(this);

            // initialize capsuled Visual Elements
            // _floatingContainer = new VisualElementHandle<VisualElement>(rootVisualElement, "floating-container");
        }

        private Result<ChatSession> FetchCurrentSession()
        => EditorChatUtil.File.GetOrCreateSession(EditorChatSettings.CurrentSessionId);

        private void FinalizeTempMessages()
        {
            _consoleErrorWatcher.ClearErrors();
            ClearTempMessages();
            _isShowingTempMessages = false;
        }

        private void ClearTempMessages()
        {
            _chatScrollView.ClearTempMessage();
        }

        #endregion Utility Methods
    }
}
