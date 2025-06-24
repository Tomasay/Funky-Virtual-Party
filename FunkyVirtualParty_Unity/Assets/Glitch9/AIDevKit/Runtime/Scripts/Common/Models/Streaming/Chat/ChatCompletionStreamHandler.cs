using System;

namespace Glitch9.AIDevKit
{
    public abstract class ChatCompletionStreamHandler : AIDevKitStreamHandler<ChatCompletionStreamHandler, ChatCompletionChunk, ChatCompletion, GENResponseTask>
    {
        protected Action<ToolCall[]> _funtionManagerCallback;
        public ChatCompletionStreamHandler() { }
        public ChatCompletionStreamHandler SetFunctionManagerCall(Action<ToolCall[]> onFunctionManagerCall)
        {
            _funtionManagerCallback = onFunctionManagerCall;
            return this;
        }

        public class Builder : StreamHandlerBuilder<Builder>
        {
            // single response
            private Action<string> _onReceiveText;
            private Action<string> _onReceiveRefusal;
            private Action<ToolCall[]> _onReceiveToolCalls;

            // multi response
            private Action<ChatDelta[]> _onReceiveDeltaChunk;

            internal Builder SetOnReceiveText(Action<string> onTextDelta)
            {
                _onReceiveText = onTextDelta;
                return this;
            }

            internal Builder SetOnReceiveRefusal(Action<string> onRefusal)
            {
                _onReceiveRefusal = onRefusal;
                return this;
            }

            internal Builder SetOnReceiveToolCalls(Action<ToolCall[]> onToolCalls)
            {
                _onReceiveToolCalls = onToolCalls;
                return this;
            }

            internal Builder SetOnReceiveDeltaChunk(Action<ChatDelta[]> onDeltaChunk)
            {
                _onReceiveDeltaChunk = onDeltaChunk;
                return this;
            }

            public override ChatCompletionStreamHandler Build()
            {
                if (_onReceiveDeltaChunk != null)
                {
                    return new MultiResponseStreamHandler(
                        handler.onStart,
                        _onReceiveDeltaChunk,
                        handler.onError,
                        handler.onDone
                    );
                }

                return new SingleResponseStreamHandler(
                    handler.onStart,
                    _onReceiveText,
                    _onReceiveRefusal,
                    _onReceiveToolCalls,
                    handler.onError,
                    handler.onDone
                );
            }
        }
    }
}