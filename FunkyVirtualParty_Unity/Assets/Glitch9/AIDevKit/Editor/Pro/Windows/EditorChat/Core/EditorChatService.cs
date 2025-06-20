using System;
using Glitch9.AIDevKit.Advanced.Chat;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal class EditorChatService : EditorChatServiceBase
    {
        internal override bool IsInitialized => true;
        private GENChatTask _task;

        internal EditorChatService(EditorChatWindow window) : base(window) { }

        protected async override void SendRequestINTERNAL(ChatMessage inputMessage, Action<ResponseMessage> onSuccess)
        {
            Model model = EditorChatSettings.CurrentModel;
            if (model == null) throw new Exception("Model is null. This should not happen. Please report this issue.");
            ChatSession session = _window.CurrentSession?.Value ?? throw new Exception("Chat session is null. This should not happen. Please report this issue.");

            try
            {
                _task = session
                    .GENChat(inputMessage)
                    .SetModel(model)
                    //.SetFunctions(EditorChatFunctions.Get())
                    .SetSender(EditorChatConfig.kSenderName)
                    // .SetSpeechOutputOptions(new SpeechOutputOptions
                    // {
                    //     Voice = EditorChatSettings.TextToSpeechVoice
                    // })
                    .SetIgnoreLogs(!EditorChatSettings.DebugMode);

                if (EditorChatSettings.SaveHistory)
                {
                    _task.EnablePromptHistory();
                }

                if (!EditorChatSettings.DisableStream)
                {
                    await _task.StreamAsync(CreateStreamHandler(onSuccess));
                }
                else
                {
                    var res = await _task.ExecuteAsync();

                    if (res != null)
                    {
                        var response = res.FirstResponseMessage();
                        OnReceiveResponse(response);
                        onSuccess?.Invoke(response);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"SendMessageAsync: {e.Message}\n{e.StackTrace}");
                OnRequestFailed(e.Message);
            }
        }

        protected async override void SendEditRequestINTERNAL(int index, string editedContent)
        {
            Model model = EditorChatSettings.CurrentModel;
            if (model == null) throw new Exception("Model is null. This should not happen. Please report this issue.");
            ChatSession session = _window.CurrentSession?.Value ?? throw new Exception("Chat session is null. This should not happen. Please report this issue.");

            try
            {
                _task = session
                    .GENEditChat(index, editedContent)
                    .SetModel(model)
                    .SetSender(EditorChatConfig.kSenderName)
                    .SetIgnoreLogs(!EditorChatSettings.DebugMode);

                if (EditorChatSettings.SaveHistory)
                {
                    _task.EnablePromptHistory();
                }

                if (!EditorChatSettings.DisableStream)
                {
                    await _task.StreamAsync(CreateStreamHandler(null));
                }
                else
                {
                    var result = await _task.ExecuteAsync();
                    OnReceiveResponse(result?.FirstResponseMessage());
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"SendEditRequestINTERNAL: {e.Message}\n{e.StackTrace}");
                OnRequestFailed(e.Message);
            }
        }

        private ChatCompletionStreamHandler CreateStreamHandler(Action<ResponseMessage> onSuccess)
        {
            return new SingleResponseStreamHandler(
                onStart: OnStreamStart,
                onReceiveText: OnStreamDelta,
                onError: OnRequestFailed,
                onDone: (result) =>
                {
                    onSuccess?.Invoke(result?.FirstResponseMessage());
                    OnStreamDone(result);
                }
            );
        }

        protected override void CancelRequestINTERNAL()
        {
            // if (_task == null) return;
            // _task.Cancel();
            // _task = null;
        }
    }
}