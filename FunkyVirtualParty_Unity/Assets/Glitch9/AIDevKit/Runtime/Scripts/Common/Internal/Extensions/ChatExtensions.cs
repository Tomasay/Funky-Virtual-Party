using System;

namespace Glitch9.AIDevKit
{
    internal static class ChatExtensions
    {
        internal static void ProcessText(this ChatCompletion c, Func<string, string> textProcessor)
        {
            if (c == null || textProcessor == null) return;

            if (c.Choices.IsNullOrEmpty()) return;

            string text = c.ToString();

            if (string.IsNullOrEmpty(text)) return;

            string newText = textProcessor(text);

            if (newText == text) return;

            foreach (var choice in c.Choices)
            {
                if (choice?.Message is ChatMessage message)
                {
                    message.ReplaceText(newText);
                }
                else if (choice?.Delta is ChatDelta delta)
                {
                    delta.Content = newText;
                }
                else
                {
                    choice.Text = newText;
                }
            }
        }

        internal static void ReplaceText(this ChatMessage m, string newText)
        {
            if (m == null || m.Content == null || newText == null) return;

            m.Content.ReplaceText(newText);
        }
    }
}