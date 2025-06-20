using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.OpenAI.Assistants;
using Glitch9.AIDevKit.OpenAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Chatbot (Assistants API)")]
    public class AssistantChatbot : ChatbotComponent
    {
        [SerializeField] private string assistantId;
        [SerializeField] private bool stream = true;
        [SerializeField] private int requiredActionTimeoutSeconds = AIDevKitConfig.DefaultRequiredActionTimeoutSeconds;
        [SerializeField] private bool autoCancelOnRequiredAction = false;

        // Event Managers / Receivers / Listeners 
        [SerializeField] private StreamingTextEventReceiver streamingTextEventReceiver;
        [SerializeField] private UnityEvent<FunctionCall> onRequiredAction;

        // Lifecycle Event Receivers
        [SerializeField] private AssistantEventReceiver assistantEventReceiver;
        [SerializeField] private ThreadEventReceiver threadEventReceiver;
        [SerializeField] private RunEventReceiver runEventReceiver;
        [SerializeField] private MessageEventReceiver messageEventReceiver;

        public override List<ChatMessage> Messages => new();
        public override string Name => Controller?.AssistantName;
        public override Model Model => Controller?.Model;
        public override bool Stream => stream;
        public override bool IsInitialized => Controller?.IsInitialized ?? false;

        public AssistantController Controller { get; private set; }
        public string CurrentThreadId => Controller?.ThreadId;


        public async UniTask InitializeAsync()
        {
            AssistantSettings settings = new()
            {
                AssistantId = assistantId,
                Stream = stream,
                FunctionManager = functionManager,
                TextEventReceiver = streamingTextEventReceiver,
                ToolCallReceiver = toolCallReceiver,
                AssistantEventReceiver = assistantEventReceiver,
                ThreadEventReceiver = threadEventReceiver,
                RunEventReceiver = runEventReceiver,
                MessageEventReceiver = messageEventReceiver,
                RequiredActionTimeoutSeconds = requiredActionTimeoutSeconds,
                RequiredActionListener = new RequiredActionListener(OnRequiredActionINTERNAL),
                AutoCancelOnRequiredAction = autoCancelOnRequiredAction,
            };

            Controller = new(settings);
            await Controller.InitializeAsync();
        }

        public UniTask CreateThreadAsync() => Controller.CreateThreadAsync();
        public UniTask CancelRunAsync() => Controller.CancelRunAsync();

        protected override async UniTask<ChatMessage> SendMessageAsyncINTERNAL(ChatMessage inputMessage)
        {
            ThreadMessageRequest inputChatRequest = inputMessage.ToThreadMessageRequest();
            return await Controller.RequestAsync(inputChatRequest);
        }

        private void OnRequiredActionINTERNAL(FunctionCall functionCall)
        {
            if (functionCall == null)
            {
                Debug.LogError("Assistant sent a null required action(function call). Cancelling the current run.");
                Controller.CancelRunAsync().Forget();
                return;
            }

            if (onRequiredAction == null)
            {
                Debug.LogError("Assistant requires you to submit tool outputs to continue the conversation, but you have not set up the event receiver. Please set up the event receiver in the Inspector. Cancelling the current run.");
                Controller.CancelRunAsync().Forget();
                return;
            }

            onRequiredAction?.Invoke(functionCall);
        }
    }
}