using System;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.OpenAI.Assistants;
using Glitch9.AIDevKit.OpenAI;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal class EditorAssistantChatService : EditorChatServiceBase
    {
        private static readonly AssistantSettings kSettings = new()
        {
            AssistantId = EditorChatConfig.kChatbotId,
            // Model = EditorChatConfig.kDefaultModel,
            // Name = EditorChatConfig.kChatbotName,
            // Description = EditorChatConfig.kChatbotDescription,
            // Instructions = EditorChatConfig.kChatbotInstruction,
            // ResponseFormat = TextFormat.Text
        };

        internal AssistantController Api;
        internal override bool IsInitialized => Api != null && Api.IsInitialized;
        internal override string CurrentSessionId => Api?.ThreadId;
        private bool _newThreadError = false;
        private Thread _newThread;

        internal EditorAssistantChatService(EditorChatWindow window) : base(window)
        {
            // AssistantEventHandler handler = new();
            // handler.onTextCreated += window.OnStreamStart;
            // handler.onTextDelta += window.OnStreamDelta;
            // handler.onThreadCreated += OnThreadCreated;
            // handler.onRunStatusChanged += OnRunStatusChanged;

            //kOptions.EventHandler = handler;
            CreateSessionIdDelegate = () => CreateSessionId();

            Api = new(kSettings);
            Api.InitializeAsync().Forget();
        }

        private async UniTask<string> CreateSessionId()
        {
            await Api.CreateThreadAsync();

            // wait until _newThread is set
            while (_newThread == null && !_newThreadError)
            {
                await UniTask.Yield();
            }

            if (_newThreadError) throw new Exception("Failed to create new thread.");
            return _newThread.Id;
        }

        protected async override void SendRequestINTERNAL(ChatMessage inputMessage, Action<ResponseMessage> onSuccess)
        {
            RunRequest.Builder runReqBuilder = new RunRequest.Builder()
                .SetSender(EditorChatConfig.kChatbotId)
                .SetModel(EditorChatSettings.AssistantsAPIModel)
                .SetStream();

            // if (EditorChatSettings.Temperature != null)
            // {
            //     runReqBuilder.SetTemperature(EditorChatSettings.Temperature.Value);
            // }

            // if (EditorChatSettings.TopP != null)
            // {
            //     runReqBuilder.SetTopP(EditorChatSettings.TopP.Value);
            // }

            // if (EditorChatSettings.MaxTokens != null && EditorChatSettings.MaxTokens > 1000)
            // {
            //     runReqBuilder.SetMaxPromptTokens(EditorChatSettings.MaxTokens.Value);
            // }

            try
            {
                //var inputChat = await inputMessage.ToChatMessageAsync();
                ThreadMessage responseMessage = await Api.RequestAsync(inputMessage.ToThreadMessageRequest(), runReqBuilder.Build());
                if (responseMessage != null) _window.FinalizeResponse(responseMessage);
            }
            catch (Exception e)
            {
                OnRequestFailed(e.Message);
                return;
            }
        }

        protected override void SendEditRequestINTERNAL(int index, string editedContent)
        {

        }

        protected async override void CancelRequestINTERNAL() => await Api.CancelRunAsync();
        internal override void OnSetChatSession(string id) => Api.SetThreadAsync(id).Forget();
        internal override void OnDeleteChatSession(string id) => Api.DeleteThreadAsync(id).Forget();

        internal void OnRunStatusChanged(object sender, RunStatus runStatus)
        {
            if (runStatus == RunStatus.Cancelled) CurrentState = EditorChatState.Idle;
        }

        internal void OnThreadCreated(object sender, Thread newThread)
        {
            if (newThread == null)
            {
                _newThreadError = true;
                throw new ArgumentNullException(nameof(newThread));
            }

            if (string.IsNullOrEmpty(newThread.Id))
            {
                _newThreadError = true;
                throw new ArgumentNullException(nameof(newThread.Id));
            }

            _newThread = newThread;
        }
    }
}