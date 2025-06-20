using Cysharp.Threading.Tasks;
using Glitch9.IO.Json.Schema;
using System;

namespace Glitch9.AIDevKit
{
    public enum ResponseType
    {
        None = 0,
        Text,
        Image,
        //Audio // not supported yet
    }

    [StrictJsonSchema("message_intent_classification", Description = "Classifies the intent of a message to determine if it should be text or image.", Strict = true)]
    internal class ResponseTypeResponse
    {
        [JsonSchemaProperty("intent_type", Description = "The determined intent type of the input text.", Enum = new string[2] { "text", "image" }, Required = true, AdditionalProperties = false)]
        public string IntentType { get; set; }
    }

    internal class ResponseTypeResolver
    {
        private static class Strings
        {
            internal const string MODEL = "gpt-4o-mini";
            internal const string INSTRUCTIONS = "You are a tool designed to determine the intent behind user inputs within an application, distinguishing whether a response should be in text or image format.";
        }

        internal async UniTask<ResponseType> ResolveAsync(string promptText)
        {
            if (string.IsNullOrWhiteSpace(promptText)) return ResponseType.None;
            ResponseTypeResponse res = await promptText
                .GENStruct<ResponseTypeResponse>()
                .SetModel(Strings.MODEL)
                .SetInstruction(Strings.INSTRUCTIONS)
                .ExecuteAsync() ?? throw new Exception("Failed to get response from request type resolver.");

            return res.IntentType switch
            {
                "text" => ResponseType.Text,
                "image" => ResponseType.Image,
                _ => ResponseType.None
            };
        }
    }
}