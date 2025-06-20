using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Glitch9.AIDevKit.Advanced.Chat
{
    internal class ChatSummaryGenerator
    {
        private const string kInstruction = @"
Progressively summarize the lines of conversation provided, adding onto the previous summary returning a new summary.

EXAMPLE
Current summary:
The user asks what the you thinks of artificial intelligence. You think artificial intelligence is a force for good.

New lines of conversation:
User: Why do you think artificial intelligence is a force for good?
You: Because artificial intelligence will help people reach their full potential.

New summary:
The user asks what you thinks of artificial intelligence. You think artificial intelligence is a force for good because it will help humans reach their full potential.

END OF EXAMPLE

Current summary:
{summary}

New lines of conversation:
{new_lines}
";
        internal async UniTask<(string, Usage)> UpdateSummaryAsync(Model model, string currentSummary, List<ChatMessage> recentMessages)
        {
            if (recentMessages.IsNullOrEmpty()) return (currentSummary, null);
            if (string.IsNullOrWhiteSpace(currentSummary)) currentSummary = "No summary available.";

            string newLines = string.Empty;

            using (StringBuilderPool.Get(out var sb))
            {
                foreach (ChatMessage message in recentMessages)
                {
                    string prefix = message.Role.ToString();
                    sb.AppendLine($"{prefix}: {message.Content}");
                }

                newLines = sb.ToString();
            }

            string prompt = kInstruction
                 .Replace("{summary}", currentSummary)
                 .Replace("{new_lines}", newLines);

            // Call the OpenAI API to get the updated summary

            if (model == null) model = AIDevKitConfig.kDefault_Chat_UtilityModel;

            ChatCompletion response = await prompt.GENResponse()
                .SetModel(model)
                .ExecuteAsync();

            if (response == null) return (currentSummary, null);
            string updatedSummary = response;
            Usage usage = response.Usage;
            if (string.IsNullOrWhiteSpace(updatedSummary)) return (currentSummary, usage);
            updatedSummary = updatedSummary.Trim();
            return (updatedSummary, usage);
        }
    }
}
