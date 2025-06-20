using System.Collections.Generic;

namespace Glitch9.AIDevKit
{
    internal static class ChatCompletionFactory
    {
        internal static ChatCompletion Create(Content content, ToolCall[] tools, Usage usage)
        {
            // AIDevKitDebug.Mark($"ChatCompletionFactory: Text: {content}");

            ChatChoice choice = new()
            {
                Message = new ResponseMessage
                {
                    Content = content,
                    Tools = tools
                }
            };

            return new ChatCompletion
            {
                Choices = new[] { choice },
                Usage = usage,
            };
        }

        internal static ChatCompletion CreateDelta(Content content, ToolCall[] tools, Usage usage)
        {
            ChatChoice choice = new()
            {
                Delta = new ChatDelta
                {
                    Content = content,
                    ToolCalls = tools
                }
            };

            return new ChatCompletion
            {
                Choices = new[] { choice },
                Usage = usage,
            };
        }

        internal static ChatCompletion Create(List<string> contents, List<ToolCall[]> toolCalls, Usage usage)
        {
            List<ChatChoice> choices = new();

            for (int i = 0; i < contents.Count; i++)
            {
                var content = contents[i];
                var tools = toolCalls[i];

                ChatChoice choice = new()
                {
                    Message = new ResponseMessage
                    {
                        Content = content,
                        Tools = tools
                    }
                };

                choices.Add(choice);
            }

            return new ChatCompletion
            {
                Choices = choices.ToArray(),
                Usage = usage,
            };
        }

        internal static ChatCompletion Create(ChatChoice[] choices, Usage usage)
        {
            return new ChatCompletion
            {
                Choices = choices,
                Usage = usage,
            };
        }


        internal static ChatCompletion Error(string content)
        {
            ChatChoice choice = new()
            {
                Message = new ResponseMessage
                {
                    Content = content,
                },
            };

            return new ChatCompletion
            {
                Choices = new[] { choice },
            };
        }

        internal static ChatCompletion FromText(string content, StopReason? finishReason, Usage usage)
        {
            ChatChoice choice = new()
            {
                Message = new ResponseMessage
                {
                    Content = content,
                },
                FinishReason = finishReason
            };

            return new ChatCompletion
            {
                Choices = new[] { choice },
                Usage = usage,
            };
        }

        internal static ChatCompletion FromMessage(ResponseMessage responseMessage, StopReason? finishReason, Usage usage)
        {
            ChatChoice choice = new()
            {
                Message = responseMessage,
                FinishReason = finishReason
            };

            return new ChatCompletion
            {
                Choices = new[] { choice },
                Usage = usage,
            };
        }
    }
}