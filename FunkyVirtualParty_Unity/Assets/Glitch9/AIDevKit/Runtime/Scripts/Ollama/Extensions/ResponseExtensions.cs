using System;

namespace Glitch9.AIDevKit.Ollama
{
    /// <summary>
    /// Response object wrapped with helper methods.
    /// </summary>
    public static class ResponseExtensions
    {
        public static void SetOutputText(this GenerateResponse res, string text)
        {
            res.Response = text;
        }

        public static void SetOutputText(this ChatResponse res, string text)
        {
            res.Message.Content = text;
        }
    }
}
