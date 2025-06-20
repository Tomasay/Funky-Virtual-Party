using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace Glitch9.AIDevKit.Advanced.Chat
{
    public enum ChatSessionScope
    {
        /// <summary>
        /// Can be used in both Editor and Runtime contexts.
        /// </summary>
        None = 0,

        /// <summary>
        /// This chat session is used only within the Unity Editor (e.g., Editor tooling, assistant, console, etc).
        /// </summary>
        EditorOnly,

        /// <summary>
        /// This chat session is used only at runtime (e.g., in-game AI chat, NPC conversation, etc).
        /// </summary>
        RuntimeOnly,
    }

    public static class ChatSessionScopeExtensions
    {
        public static bool Contains(this ChatSessionScope scope, ChatSessionScope other)
        {
            if (scope == ChatSessionScope.None || other == ChatSessionScope.None)
                return true; // None means it can be used in both contexts
            return (scope & other) == other;
        }
    }

    /* 
        OpenAI Finish Reasons:
            The reason the model stopped generating tokens. 
            This will be stop if the model hit a natural stop point or a provided stop sequence, 
            length if the maximum number of tokens specified in the request was reached, 
            content_filter if content was omitted due to a flag from our content filters, 
            tool_calls if the model called a tool, or function_call (deprecated) if the model called a function.
    */

    /// <summary>
    /// Represents a single chat session, including its metadata, settings, messages, and serialization logic.
    /// Provides support for saving, loading, summarizing, and auto-saving the session.
    /// </summary> 
    [JsonObject]
    public class ChatSession : IData
    {
        public const string kDefaultTitle = "New Chat";

        public static ChatSession CreateFile(string id = null, string name = null, string startingMessage = null) => ChatSessionUtil.CreateSessionFile(id, name, startingMessage);
        public static ChatSession LoadFile(string id) => ChatSessionUtil.LoadSessionFromFile(id);
        public static bool DeleteFile(string id) => ChatSessionUtil.DeleteSessionFile(id);

        // Chat Session Properties -------------------------------------------------------------------------------

        /// <summary>
        /// Unique identifier of the chat session.
        /// </summary>
        [JsonProperty] public string Id { get; private set; }

        /// <summary>
        /// Name(title) of the chat session.
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get => _name ?? kDefaultTitle;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return; // Do not allow empty names
                _name = value;
                OnTitleChanged?.Invoke(_name); // Notify subscribers that the title has changed
            }
        }

        [JsonProperty("Name")] private string _name;
        [JsonProperty("Model")] private Model _model;
        [JsonProperty("UtilityModel")] private Model _utilityModel;

        /// <summary>
        /// Creation timestamp.
        /// </summary>
        [JsonProperty] public UnixTime CreatedAt { get; internal set; } = UnixTime.Now;

        /// <summary>
        /// Last update timestamp.
        /// </summary>
        [JsonProperty] public UnixTime UpdatedAt { get; internal set; } = UnixTime.Now;

        /// <summary>
        /// Whether to hide this chat session from the editor chat window. 
        /// </summary>
        [JsonProperty] public ChatSessionScope Scope { get; set; } = ChatSessionScope.None;

        // Memory (Context) Related Settings ----------------------------------------------------------------------

        /// <summary>
        /// Whether the session should automatically save on update.
        /// </summary>
        [JsonProperty] public bool AutoSave { get; set; } = true;

        /// <summary>
        /// Maximum number of context messages to use before summarizing.
        /// </summary>
        [JsonProperty] public int MaxContextMessages { get; set; } = 20;

        /// <summary>
        /// If enabled, the session will automatically be named based on the first user message with a hint of the topic.
        /// This is useful for quickly identifying sessions without manually naming them.
        /// </summary>
        [JsonProperty] public bool AutoTitle { get; set; } = true;
        [JsonProperty] public bool IsTitleGenerated { get; internal set; } = false;

        /// <summary>
        /// Model used for utility tasks such as conversation summary, auto-naming, and metadata generation.
        /// </summary>
        [JsonIgnore]
        public Model UtilityModel
        {
            get
            {
                if (_utilityModel == null) _utilityModel = AIDevKitConfig.kDefault_Chat_UtilityModel;
                return _utilityModel;
            }
            set
            {
                if (value == null) return;
                _utilityModel = value;
            }
        }

        /// <summary>
        /// The current summary of the session.
        /// </summary>
        [JsonProperty] public string Summary { get; internal set; } = "No summary available.";

        /// <summary>
        /// Time when the summary was last updated.
        /// </summary>
        [JsonProperty] public UnixTime LastSummaryUpdate { get; internal set; } = UnixTime.MinValue;

        // Chat Session Settings --------------------------------------------------------------------------------- 

        /// <summary>
        /// Whether streaming responses are enabled.
        /// </summary>
        [JsonProperty] public bool Stream { get; set; } = true;

        /// <summary>
        /// The AI model used for the chat.
        /// </summary>
        [JsonIgnore]
        public Model Model //{ get; set; } = AIDevKitConfig.kDefault_Chat_Model;
        {
            get
            {
                if (_model == null) _model = AIDevKitConfig.kDefault_Chat_Model;
                return _model;
            }
            set
            {
                if (value == null) return;
                _model = value;
            }
        }

        /// <summary>
        /// Optional parameters for model behavior.
        /// </summary>
        [JsonProperty] public ModelSettings ModelOptions { get; set; }

        /// <summary>
        /// Additional reasoning configuration.
        /// </summary>
        [JsonProperty] public ReasoningOptions ReasoningOptions { get; set; }

        /// <summary>
        /// Settings for web search integration.
        /// </summary>
        [JsonProperty] public WebSearchOptions WebSearchOptions { get; set; }

        /// <summary>
        /// Settings for speech output.
        /// </summary>
        [JsonProperty] public SpeechOutputOptions SpeechOutputOptions { get; set; }

        /// <summary>
        /// Moderation settings.
        /// </summary>
        [JsonProperty] public ModerationOptions ModerationOptions { get; set; }

        /// <summary>
        /// System instructions for the assistant.
        /// </summary>
        [JsonProperty] public string Instructions { get; set; }

        /// <summary>
        /// The message shown to the user at the start of the session.
        /// </summary>
        [JsonProperty] public string StartingMessage { get; set; }


        // Chat Messages -------------------------------------------------------------------------------------- 

        /// <summary>
        /// The last message sent by the user or received from the assistant.
        /// </summary>
        [JsonProperty] public string LastMessage { get; internal set; } = "No messages yet.";

        /// <summary>
        /// List of all chat messages in the session.
        /// </summary>
        [JsonIgnore] public List<ChatMessage> Messages { get => _controller?.GetMessages(); set => _controller?.SetMessages(value); }

        /// <summary>
        /// List of recent chat messages used for summarization.
        /// </summary>
        [JsonIgnore] public List<ChatMessage> RecentMessages => _recentMessages ??= new();

        /// <summary>
        /// The last message received from the assistant in form of raw chat completion.
        /// This is useful for accessing the full response, including tool calls and other metadata.
        /// </summary>
        [JsonIgnore] public ChatCompletion Last => _controller._lastReceivedChatCompletion;
        [JsonIgnore] public Exception LastException => _controller._lastException;
        [JsonProperty] public float TotalCostInUSD { get; internal set; } = 0f;
        [JsonIgnore] public Usage TotalUsage { get => _totalUsage ??= _controller?.CalcTotalUsage(); internal set => _totalUsage = value; }
        [JsonIgnore] public int Count => _messages?.Count ?? 0; // Total number of messages in the session

        // private fields --------------------------------------------------------------------------------------- 
        [JsonProperty] internal Usage _totalUsage;
        [JsonProperty] internal List<ChatMessage> _messages = new(); // This should never ever be null 
        [JsonProperty] internal List<ChatMessage> _recentMessages = new(); // This should never ever be null 

        [JsonIgnore] private readonly ChatSessionController _controller;
        [JsonIgnore] public Action<string> OnTitleChanged; // Action to invoke when the title changes

        [JsonConstructor] public ChatSession() { _controller = new ChatSessionController(this); }
        internal ChatSession(string id, string name = null, string startingMessage = null)
        {
            Id = id;
            Name = string.IsNullOrWhiteSpace(name) ? "New Chat" : name;

            if (!string.IsNullOrWhiteSpace(startingMessage))
            {
                StartingMessage = startingMessage;
                ResponseMessage assistantMessage = new(StartingMessage);
                _messages.Add(assistantMessage);
            }

            _controller = new ChatSessionController(this);
        }

        /// <summary>
        /// Creates a text generation task using this chat session and message as the prompt.
        /// 
        /// Example:
        ///     chatSession.GENChat(chatMessage).SetModel(OpenAIModel.GPT4o).ExecuteAsync();
        /// </summary> 
        public GENChatTask GENChat(ChatMessage chatMessage) => new(this, chatMessage);

        /// <summary>
        /// Edits a user message at the specified index in the chat session.
        /// This will update the message content and timestamp, and remove all messages after the edited message.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="editedContent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"> </exception>
        /// <exception cref="InvalidOperationException"></exception>
        public GENChatTask GENEditChat(int index, string editedContent)
        {
            if (index < 0 || index >= _messages.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range of the chat messages.");
            if (_messages[index].Role != ChatRole.User)
                throw new InvalidOperationException("Only user messages can be edited.");

            ChatMessage editedMessage = _messages[index];
            editedMessage.Timestamp = UnixTime.Now; // Update timestamp to current timeo
            editedMessage.Content = editedContent; // Update content to the new edited content  

            // remove all messages after the edited message
            _messages = _messages.Take(index).ToList();

            return GENChat(editedMessage);
        }

        public void ClearMessages() => _controller.ClearMessages();
        public (ChatMessage lastSent, ChatMessage lastReceived) Rewind() => _controller.Rewind();
        public void AddUsage(Usage usage, Currency cost) => _controller.AddTotalUsage(usage, cost);

        internal void PushInput(ChatMessage inputMessage) => _controller.SetLastInput(inputMessage);
        internal ResponseMessage PushResponse(ChatCompletion chat) => _controller.SetLastOutput(chat);
        internal List<ChatMessage> GetContextMessages() => _controller.GetContextMessages();

        #region File Management 
        public UniTask SaveFileAsync(string customPath = null) => ChatSessionUtil.SaveSessionAsFileAsync(this, customPath);
        public void SaveFile(string customPath = null) => ChatSessionUtil.SaveSessionAsFile(this, customPath);
        public bool DeleteFile() => ChatSessionUtil.DeleteSessionFile(this);
        #endregion File Management

        #region Debugging

        internal void ReCalcTotalUsage() => _totalUsage = _controller.CalcTotalUsage();
        internal void ReCalcTotalCost() => TotalCostInUSD = _controller.CalcTotalCostInUSD();

        #endregion Debugging

        public override string ToString() => ChatSessionUtil.FormatInformation(this);
        public bool Equals(ChatSession other) => other != null && Id == other.Id;
        public override int GetHashCode() => Id?.GetHashCode() ?? 0;
    }
}