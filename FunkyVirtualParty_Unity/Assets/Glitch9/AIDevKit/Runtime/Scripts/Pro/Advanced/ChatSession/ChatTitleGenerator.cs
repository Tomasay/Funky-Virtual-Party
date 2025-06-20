using Cysharp.Threading.Tasks;
using Glitch9.IO.Json.Schema;

namespace Glitch9.AIDevKit.Advanced.Chat
{
    internal class ChatTitleGenerator
    {
        [StrictJsonSchema("chat_title_generation_response", Description = "Response for chat title generation.", Strict = false)]
        internal class ChatTitleGenerationResponse
        {
            [JsonSchemaProperty("generated_title",
                Description = "The short title generated for the chat session. (no longer than 8 words)",
                Required = true,
                AdditionalProperties = false)]
            public string GeneratedTitle { get; set; }

            [JsonSchemaProperty("generated",
                Description = "Indicates whether the message contains sufficient context for title generation and whether a title was successfully generated.",
                Required = false,
                AdditionalProperties = false)]
            public bool Generated { get; set; }
        }

        private const string kInstruction = @"
You are a helpful assistant that generates concise and relevant titles for chat sessions based on user messages.
 
Given one user message of a early conversation, evaluate if the message provides enough context to generate a meaningful title.
If it does, generate a short, clear, and descriptive title that captures the main topic or intent of the message. 
";

        internal async UniTask<(string, Usage)> GenerateTitleAsync(Model model, UserMessage lastUserMessage)
        {
            if (lastUserMessage == null || string.IsNullOrWhiteSpace(lastUserMessage.Content)) return (null, null);

            string prompt = lastUserMessage.Content;

            StructuredOutput<ChatTitleGenerationResponse> response = await prompt.GENStruct<ChatTitleGenerationResponse>()
                .SetModel(model)
                .SetInstruction(kInstruction)
                .ExecuteAsync();

            if (response == null) return (null, null);
            ChatTitleGenerationResponse res = response;
            return (res.GeneratedTitle, response.Usage);
        }
    }
}
