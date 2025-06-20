using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Advanced.Chat;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Glitch9.AIDevKit.Components
{
    /// <summary>
    /// A full-featured chatbot component that integrates with the AIDevKit's chat system.
    /// This component allows you to send messages, receive responses, and manage chat sessions.
    /// It supports both streaming and non-streaming responses, and can handle function calls.
    /// </summary>
    [AddComponentMenu("Chatbot")]
    public class Chatbot : ChatbotComponent
    {
        [SerializeField, FormerlySerializedAs("id")] protected string chatSessionId;
        [SerializeField] protected ChatCompletionStreamReceiver streamReceiver;
        [SerializeField] protected WebSearch webSearchModule;

        public string ChatSessionId => chatSessionId;
        public override string Name => Session.Name;
        public override List<ChatMessage> Messages => Session?.Messages;
        public override Model Model => Session.Model;
        public string SystemInstruction => Session.Instructions;
        public string StartingMessage => Session.StartingMessage;
        public override bool Stream => Session.Stream;
        public bool AutoSave => Session.AutoSave;
        public override bool IsInitialized => Session != null;
        public ChatSession Session { get; private set; }

        private void Start()
        {
            if (string.IsNullOrWhiteSpace(chatSessionId)) throw new Exception($"Chat Id must be set for {GetType().Name}");
            Session = ChatSession.LoadFile(chatSessionId);
            if (Session == null) throw new Exception($"Failed to load chat session with ID: {chatSessionId}");
            if (string.IsNullOrWhiteSpace(chatSessionId)) chatSessionId = Session.Id;
        }

        protected override async UniTask<ChatMessage> SendMessageAsyncINTERNAL(ChatMessage inputMessage)
        {
            if (Session == null) throw new Exception($"Chat session is not initialized for {GetType().Name}");
            if (inputMessage == null || string.IsNullOrEmpty(inputMessage.Content)) return null;
            if (chatEventReceiver != null) chatEventReceiver.OnSendMessage(inputMessage);

            if (Session.Stream)
            {
                if (streamReceiver == null) throw new Exception("StreamingTextEventReceiver is null. Please set a stream handler.");
                if (functionManager is IFunctionManager iFunctionManager) streamReceiver.SetFunctionManagerCalls(iFunctionManager.OnReceiveToolCalls);

                await Session.GENChat(inputMessage)
                   .SetModel(Model)
                   .SetSender(Name)
                   .SetIgnoreLogs(!AIDevKitDebug.kDebugMode.Value)
                   .StreamAsync(streamReceiver);

                return null;
            }
            else
            {
                FunctionDeclaration[] functionDeclarations = functionManager.GetFunctionDeclarations();
                List<FunctionCall> functions = new();
                foreach (FunctionDeclaration functionDeclaration in functionDeclarations)
                {
                    if (functionDeclaration == null) continue;
                    functions.Add(new FunctionCall(functionDeclaration));
                }

                ChatCompletion response = await Session.GENChat(inputMessage)
                    .SetModel(Model)
                    .SetSender(Name)
                    .SetFunctions(functions.ToArray())
                    .SetIgnoreLogs(!AIDevKitDebug.kDebugMode.Value)
                    .ExecuteAsync();

                if (chatEventReceiver != null)
                {
                    chatEventReceiver.OnReceiveResponse(response);
                }

                return response.FirstResponseMessage();
            }
        }
    }
}