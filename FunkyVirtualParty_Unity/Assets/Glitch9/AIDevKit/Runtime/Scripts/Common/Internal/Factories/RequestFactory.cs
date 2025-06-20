using System;

namespace Glitch9.AIDevKit
{
    internal static class RequestFactory
    {
        internal static CompletionRequest CreateCompletionRequest(this GENResponseTask task, Type jsonSchemaType, bool isStreaming)
        {
            var req = new CompletionRequest.Builder()
                .SetSender(task.sender)
                .SetIgnoreLogs(task._ignoreLogs)
                .SetModel(task.model)
                .SetModelSettings(task.modelSettings)
                .SetReasoningOptions(task.reasoningOptions)
                .SetCancellationToken(task.token)

                // prompt starts
                .SetInstruction(task.instruction)
                .SetPrompt(task.prompt)
                .SetAttachedFiles(task.attachedFiles)
                .SetJsonSchema(jsonSchemaType);

            if (isStreaming) req.SetStream(true).IncludeUsage();

            return req.Build();
        }

        internal static ChatCompletionRequest CreateChatCompletionRequest(this GENResponseTask task, Type jsonSchemaType, bool isStreaming)
        {
            var req = new ChatCompletionRequest.Builder()
                .SetSender(task.sender)
                .SetIgnoreLogs(task._ignoreLogs)
                .SetModel(task.model)
                .SetN(task.n)
                .SetModelSettings(task.modelSettings)
                .SetReasoningOptions(task.reasoningOptions)
                .SetCancellationToken(task.token)

                // prompt starts
                .SetInstruction(task.instruction)
                .SetPrompt(task.prompt)
                .SetAttachedFiles(task.attachedFiles)
                .SetJsonSchema(jsonSchemaType)
                .SetWebSearchOptions(task.webSearchOptions);


            if (task.tools.IsNotNullOrEmpty()) req.SetTools(task.tools.ToArray());
            if (task.messages.IsNotNullOrEmpty()) req.SetMessages(task.messages);
            if (task.speechOutputOptions != null) req.SetSpeechOutput(task.speechOutputOptions);
            if (isStreaming) req.SetStream(true).IncludeUsage();

            return req.Build();
        }
    }
}