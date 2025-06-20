using System;

namespace Glitch9.AIDevKit
{
    internal static class ResponseMessageFactory
    {
        // Extracts one (first) content from the chat completion and creates a single ResponseMessage.
        internal static ResponseMessage FromChatCompletion(ChatCompletion chat)
        {
            if (chat == null) throw new ArgumentNullException(nameof(chat));
            return new ResponseMessage(chat.FirstContent()) { Usage = chat.Usage, Tools = chat.GetToolCalls() };
        }

        internal static ResponseMessage FromImage(GeneratedImage image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            return new ResponseMessage(image);
        }

        internal static ResponseMessage ErrorMessage(string errorMessage)
        {
            const string kDefaultError = "I'm sorry, There was an error processing your request.";
            string content = string.IsNullOrEmpty(errorMessage) ? kDefaultError : errorMessage;
            return new ResponseMessage(content);
        }

        internal static ResponseMessage FlaggedMessage(Moderation moderation)
        {
            if (moderation == null || moderation.IsEmpty)
            {
                return ErrorMessage("Your message was flagged by the moderation system.");
            }

            return new ResponseMessage(moderation.FlaggedReason);
        }
    }
}